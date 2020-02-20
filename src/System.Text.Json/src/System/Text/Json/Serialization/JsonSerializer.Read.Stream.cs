// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using System.Buffers;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace System.Text.Json
{
    public static partial class JsonSerializer
    {
        /// <summary>
        /// Read the UTF-8 encoded text representing a single JSON value into a <typeparamref name="TValue"/>.
        /// The Stream will be read to completion.
        /// </summary>
        /// <returns>A <typeparamref name="TValue"/> representation of the JSON value.</returns>
        /// <param name="utf8Json">JSON data to parse.</param>
        /// <param name="options">Options to control the behavior during reading.</param>
        /// <param name="cancellationToken">
        /// The <see cref="System.Threading.CancellationToken"/> which may be used to cancel the read operation.
        /// </param>
        /// <exception cref="JsonException">
        /// Thrown when the JSON is invalid,
        /// <typeparamref name="TValue"/> is not compatible with the JSON,
        /// or when there is remaining data in the Stream.
        /// </exception>
        public static ValueTask<TValue> DeserializeAsync<TValue>(
            Stream utf8Json,
            JsonSerializerOptions? options = null,
            CancellationToken cancellationToken = default)
        {
            if (utf8Json == null)
                throw new ArgumentNullException(nameof(utf8Json));

            //if (utf8Json.CanSeek && utf8Json.Length - utf8Json.Position <= 999_999)
            //{
            //    int written = 0;
            //    byte[]? rented = null;

            //    ReadOnlySpan<byte> utf8Bom = JsonConstants.Utf8Bom;

            //    try
            //    {
            //        long expectedLength = Math.Max(utf8Bom.Length, utf8Json.Length - utf8Json.Position);
            //        rented = ArrayPool<byte>.Shared.Rent(checked((int)expectedLength));


            //        int lastRead;

            //        // Read up to 3 bytes to see if it's the UTF-8 BOM
            //        do
            //        {
            //            // No need for checking for growth, the minimal rent sizes both guarantee it'll fit.
            //            Debug.Assert(rented.Length >= utf8Bom.Length);

            //            lastRead = utf8Json.Read(
            //                rented,
            //                written,
            //                utf8Bom.Length - written);

            //            written += lastRead;
            //        } while (lastRead > 0 && written < utf8Bom.Length);

            //        // If we have 3 bytes, and they're the BOM, reset the write position to 0.
            //        if (written == utf8Bom.Length &&
            //            utf8Bom.SequenceEqual(rented.AsSpan(0, utf8Bom.Length)))
            //        {
            //            written = 0;
            //        }

            //        do
            //        {
            //            if (rented.Length == written)
            //            {
            //                byte[] toReturn = rented;
            //                rented = ArrayPool<byte>.Shared.Rent(checked(toReturn.Length * 2));
            //                Buffer.BlockCopy(toReturn, 0, rented, 0, toReturn.Length);
            //                // Holds document content, clear it.
            //                ArrayPool<byte>.Shared.Return(toReturn, clearArray: true);
            //            }

            //            lastRead = utf8Json.Read(rented, written, rented.Length - written);
            //            written += lastRead;
            //        } while (lastRead > 0);

            //        return ReadCore
            //    }
            //    finally
            //    {
            //        if (rented != null)
            //        {
            //            // Holds document content, clear it before returning it.
            //            rented.AsSpan(0, written).Clear();
            //            ArrayPool<byte>.Shared.Return(rented);
            //        }
            //    }
            //}



            return ReadAsync<TValue>(utf8Json, typeof(TValue), options, cancellationToken);
        }

        /// <summary>
        /// Read the UTF-8 encoded text representing a single JSON value into a <paramref name="returnType"/>.
        /// The Stream will be read to completion.
        /// </summary>
        /// <returns>A <paramref name="returnType"/> representation of the JSON value.</returns>
        /// <param name="utf8Json">JSON data to parse.</param>
        /// <param name="returnType">The type of the object to convert to and return.</param>
        /// <param name="options">Options to control the behavior during reading.</param>
        /// <param name="cancellationToken">
        /// The <see cref="System.Threading.CancellationToken"/> which may be used to cancel the read operation.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown if <paramref name="utf8Json"/> or <paramref name="returnType"/> is null.
        /// </exception>
        /// <exception cref="JsonException">
        /// Thrown when the JSON is invalid,
        /// the <paramref name="returnType"/> is not compatible with the JSON,
        /// or when there is remaining data in the Stream.
        /// </exception>
        public static ValueTask<object?> DeserializeAsync(
            Stream utf8Json,
            Type returnType,
            JsonSerializerOptions? options = null,
            CancellationToken cancellationToken = default)
        {
            if (utf8Json == null)
                throw new ArgumentNullException(nameof(utf8Json));

            if (returnType == null)
                throw new ArgumentNullException(nameof(returnType));

            return ReadAsync<object?>(utf8Json, returnType, options, cancellationToken);
        }

        private static async ValueTask<TValue> ReadAsync<TValue>(
            Stream utf8Json,
            Type returnType,
            JsonSerializerOptions? options = null,
            CancellationToken cancellationToken = default)
        {
            if (options == null)
            {
                options = JsonSerializerOptions.s_defaultOptions;
            }

            ReadStack readStack = default;
            readStack.Current.Initialize(returnType, options);

            var readerState = new JsonReaderState(options.GetReaderOptions());

            // todo: switch to ArrayBuffer implementation to handle and simplify the allocs?
            int utf8BomLength = JsonConstants.Utf8Bom.Length;
            byte[] buffer = ArrayPool<byte>.Shared.Rent(Math.Max(options.DefaultBufferSize, utf8BomLength));
            int bytesInBuffer = 0;
            int clearMax = 0;
            bool firstIteration = true;

            try
            {
                while (true)
                {
                    // Read from the stream until either our buffer is filled or we hit EOF.
                    // Calling ReadCore is relatively expensive, so we minimize the number of times
                    // we need to call it.
                    bool isFinalBlock = false;
                    while (true)
                    {
                        int bytesRead = await utf8Json.ReadAsync(
#if BUILDING_INBOX_LIBRARY
                            buffer.AsMemory(bytesInBuffer),
#else
                            buffer, bytesInBuffer, buffer.Length - bytesInBuffer,
#endif
                            cancellationToken).ConfigureAwait(false);

                        if (bytesRead == 0)
                        {
                            isFinalBlock = true;
                            break;
                        }

                        bytesInBuffer += bytesRead;

                        if (bytesInBuffer == buffer.Length)
                        {
                            break;
                        }
                    }

                    if (bytesInBuffer > clearMax)
                    {
                        clearMax = bytesInBuffer;
                    }

                    int start = 0;
                    if (firstIteration)
                    {
                        firstIteration = false;
                        // Handle the UTF-8 BOM if present
                        Debug.Assert(buffer.Length >= JsonConstants.Utf8Bom.Length);
                        if (buffer.AsSpan().StartsWith(JsonConstants.Utf8Bom))
                        {
                            start += utf8BomLength;
                            bytesInBuffer -= utf8BomLength;
                        }
                    }

                    // Process the data available
                    ReadCore(
                        options,
                        ref readStack,
                        new ReadOnlySpan<byte>(buffer, start, bytesInBuffer),
                        isFinalBlock,
                        ref readerState);

                    if (isFinalBlock)
                    {
                        // The reader should have thrown if we have remaining bytes.
                        Debug.Assert(bytesInBuffer == checked((int)readStack.BytesConsumed));
                        break;
                    }

                    if (readStack.BytesConsumed > bytesInBuffer)
                    {
                        throw new InvalidOperationException($"{bytesInBuffer},{readStack.BytesConsumed}");
                    }

                    Debug.Assert(readStack.BytesConsumed <= bytesInBuffer);
                    int bytesConsumed = checked((int)readStack.BytesConsumed);

                    bytesInBuffer -= bytesConsumed;

                    // Check if we need to shift or expand the buffer because there wasn't enough data to complete deserialization.
                    if ((uint)bytesInBuffer > ((uint)buffer.Length / 2))
                    {
                        // We have less than half the buffer available, double the buffer size.
                        byte[] dest = ArrayPool<byte>.Shared.Rent((buffer.Length < (int.MaxValue / 2)) ? buffer.Length * 2 : int.MaxValue);

                        // Copy the unprocessed data to the new buffer while shifting the processed bytes.
                        Buffer.BlockCopy(buffer, bytesConsumed + start, dest, 0, bytesInBuffer);

                        new Span<byte>(buffer, 0, clearMax).Clear();
                        ArrayPool<byte>.Shared.Return(buffer);

                        clearMax = bytesInBuffer;
                        buffer = dest;
                    }
                    else if (bytesInBuffer != 0)
                    {
                        // Shift the processed bytes to the beginning of buffer to make more room.
                        Buffer.BlockCopy(buffer, bytesConsumed + start, buffer, 0, bytesInBuffer);
                    }
                }
            }
            finally
            {
                // Clear only what we used and return the buffer to the pool
                new Span<byte>(buffer, 0, clearMax).Clear();
                ArrayPool<byte>.Shared.Return(buffer);
            }

            return (TValue)readStack.Current.ReturnValue!;
        }

        private static void ReadCore(
            JsonSerializerOptions options,
            ref ReadStack readStack,
            ReadOnlySpan<byte> buffer,
            bool isFinalBlock,
            ref JsonReaderState readerState)
        {
            // If we haven't read in the entire stream's payload we'll need to signify that we want
            // to enable read ahead behaviors to ensure we have complete json objects and arrays
            // ({}, []) when needed. (Notably to successfully parse JsonElement via JsonDocument
            // to assign to object and JsonElement properties in the constructed .NET object.)
            readStack._readAhead = !isFinalBlock;
            readStack.BytesConsumed = 0;

            var reader = new Utf8JsonReader(buffer, isFinalBlock, readerState);

            ReadCore(
                options,
                ref readStack,
                ref reader);

            if (!isFinalBlock)
            {
                readerState = reader.CurrentState;
            }
        }
    }
}
