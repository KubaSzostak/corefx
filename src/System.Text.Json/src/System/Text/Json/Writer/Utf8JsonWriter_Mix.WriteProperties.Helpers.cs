// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Text.Json
{
    public partial class Utf8JsonWriter_Mix
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ValidatePropertyNameAndDepth(ReadOnlySpan<char> propertyName)
        {
            if (propertyName.Length > JsonConstants.MaxCharacterTokenSize || CurrentDepth >= JsonConstants.MaxWriterDepth)
                ThrowHelper.ThrowInvalidOperationOrArgumentException(propertyName, _currentDepth);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ValidatePropertyNameAndDepth(ReadOnlySpan<byte> utf8PropertyName)
        {
            if (utf8PropertyName.Length > JsonConstants.MaxTokenSize || CurrentDepth >= JsonConstants.MaxWriterDepth)
                ThrowHelper.ThrowInvalidOperationOrArgumentException(utf8PropertyName, _currentDepth);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ValidateWritingProperty()
        {
            if (!_writerOptions.SkipValidation)
            {
                if (!_inObject)
                {
                    Debug.Assert(_tokenType != JsonTokenType.StartObject);
                    ThrowHelper.ThrowInvalidOperationException(ExceptionResource.CannotWritePropertyWithinArray, tokenType: _tokenType);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ValidateWritingProperty(byte token)
        {
            if (!_writerOptions.SkipValidation)
            {
                if (!_inObject)
                {
                    Debug.Assert(_tokenType != JsonTokenType.StartObject);
                    ThrowHelper.ThrowInvalidOperationException(ExceptionResource.CannotWritePropertyWithinArray, tokenType: _tokenType);
                }
                UpdateBitStackOnStart(token);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private int WritePropertyNameMinimizedSlow(ReadOnlySpan<byte> escapedPropertyName, int maxLengthRequired)
        {
            throw new NotImplementedException();
        }

        private int WritePropertyNameMinimized(ReadOnlySpan<byte> escapedPropertyName)
        {
            int maxLengthRequired = escapedPropertyName.Length + 4;

            if (maxLengthRequired > DefaultGrowthSize)
            {
                return WritePropertyNameMinimizedSlow(escapedPropertyName, maxLengthRequired);
            }

            if (_buffer.Length - _buffered < maxLengthRequired)
            {
                int minLengthRequired = escapedPropertyName.Length + 3;
                GrowAndEnsure(minLengthRequired, maxLengthRequired);
            }

            Span<byte> output = _buffer.Span;

            int idx = _buffered;

            if (_currentDepth < 0)
            {
                output[idx++] = JsonConstants.ListSeparator;
            }

            output[idx++] = JsonConstants.Quote;

            escapedPropertyName.CopyTo(output.Slice(idx));
            idx += escapedPropertyName.Length;

            output[idx++] = JsonConstants.Quote;
            output[idx++] = JsonConstants.KeyValueSeperator;

            return idx;
        }

        private int WritePropertyNameMinimized_old(ReadOnlySpan<byte> escapedPropertyName)
        {
            if (_buffer.Length < escapedPropertyName.Length + 5)
            {
                GrowAndEnsure(escapedPropertyName.Length + 5);
            }

            Span<byte> output = _buffer.Span;

            int idx = 0;

            if (_currentDepth < 0)
            {
                output[idx++] = JsonConstants.ListSeparator;
            }

            output[idx++] = JsonConstants.Quote;

            escapedPropertyName.CopyTo(output.Slice(idx));
            idx += escapedPropertyName.Length;

            output[idx++] = JsonConstants.Quote;

            output[idx++] = JsonConstants.KeyValueSeperator;

            return idx;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private int WritePropertyNameMinimizedSlow(ReadOnlySpan<byte> escapedPropertyName, byte token, int maxLengthRequired)
        {
            throw new NotImplementedException();
        }

        private int WritePropertyNameMinimized(ReadOnlySpan<byte> escapedPropertyName, byte token)
        {
            int maxLengthRequired = escapedPropertyName.Length + 5;

            if (maxLengthRequired > DefaultGrowthSize)
            {
                return WritePropertyNameMinimizedSlow(escapedPropertyName, token, maxLengthRequired);
            }

            Span<byte> output = GetSpan(maxLengthRequired, escapedPropertyName.Length);

            int idx = _buffered;

            if (_currentDepth < 0)
            {
                output[idx++] = JsonConstants.ListSeparator;
            }

            output[idx++] = JsonConstants.Quote;

            escapedPropertyName.CopyTo(output.Slice(idx));
            idx += escapedPropertyName.Length;

            output[idx++] = JsonConstants.Quote;
            output[idx++] = JsonConstants.KeyValueSeperator;
            output[idx++] = token;

            return idx;
        }

        private Span<byte> GetSpan(int maxLengthRequired, int escapedPropertyNameLength)
        {
            if (_output != null)
            {
                if (_buffer.Length - _buffered < maxLengthRequired)
                {
                    int minLengthRequired = escapedPropertyNameLength + 4;
                    GrowAndEnsure(minLengthRequired, maxLengthRequired);
                }

                return _buffer.Span;
            }
            else
            {
                if (_array.Length - _buffered < maxLengthRequired)
                {
                    GrowAndEnsure(maxLengthRequired);
                }

                return _array;
            }
        }

        private int WritePropertyNameIndented(ReadOnlySpan<byte> escapedPropertyName)
        {
            int idx = 0;
            Span<byte> output = _buffer.Span;
            if (_currentDepth < 0)
            {
                if (_buffer.Length <= idx)
                {
                    GrowAndEnsure();
                    output = _buffer.Span;
                }
                output[idx++] = JsonConstants.ListSeparator;
            }

            if (_tokenType != JsonTokenType.None)
            {
                WriteNewLine(ref idx);
                output = _buffer.Span;
            }

            int indent = Indentation;
            while (true)
            {
                bool result = JsonWriterHelper.TryWriteIndentation(output.Slice(idx), indent, out int bytesWritten);
                idx += bytesWritten;
                if (result)
                {
                    break;
                }
                indent -= bytesWritten;
                AdvanceAndGrow(ref idx);
                output = _buffer.Span;
            }

            if (_buffer.Length <= idx)
            {
                AdvanceAndGrow(ref idx);
                output = _buffer.Span;
            }
            output[idx++] = JsonConstants.Quote;

            CopyLoop(escapedPropertyName, ref idx);
            output = _buffer.Span;

            if (_buffer.Length <= idx)
            {
                AdvanceAndGrow(ref idx);
            }
            output[idx++] = JsonConstants.Quote;

            if (_buffer.Length <= idx)
            {
                AdvanceAndGrow(ref idx);
                output = _buffer.Span;
            }
            output[idx++] = JsonConstants.KeyValueSeperator;

            if (_buffer.Length <= idx)
            {
                AdvanceAndGrow(ref idx);
                output = _buffer.Span;
            }
            output[idx++] = JsonConstants.Space;

            return idx;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private int WritePropertyNameMinimizedSlow(ReadOnlySpan<char> escapedPropertyName, int maxLengthRequired)
        {
            throw new NotImplementedException();
        }

        private int WritePropertyNameMinimized(ReadOnlySpan<char> escapedPropertyName)
        {
            int maxLengthRequired = (escapedPropertyName.Length * 3) + 4;

            if (maxLengthRequired > DefaultGrowthSize)
            {
                return WritePropertyNameMinimizedSlow(escapedPropertyName, maxLengthRequired);
            }

            if (_buffer.Length - _buffered < maxLengthRequired)
            {
                int minLengthRequired = escapedPropertyName.Length + 3;
                GrowAndEnsure(minLengthRequired, maxLengthRequired);
            }

            Span<byte> output = _buffer.Span;

            int idx = _buffered;

            if (_currentDepth < 0)
            {
                output[idx++] = JsonConstants.ListSeparator;
            }

            output[idx++] = JsonConstants.Quote;

            ReadOnlySpan<byte> byteSpan = MemoryMarshal.AsBytes(escapedPropertyName);
            OperationStatus status = JsonWriterHelper.ToUtf8(byteSpan, output.Slice(idx), out int consumed, out int written);
            Debug.Assert(status != OperationStatus.DestinationTooSmall);
            if (status != OperationStatus.Done)
            {
                throw new InvalidOperationException();
            }
            Debug.Assert(consumed == byteSpan.Length);
            idx += written;

            output[idx++] = JsonConstants.Quote;
            output[idx++] = JsonConstants.KeyValueSeperator;

            return idx;
        }

        private int WritePropertyNameMinimized_old(ReadOnlySpan<char> escapedPropertyName)
        {
            int idx = 0;
            Span<byte> output = _buffer.Span;
            if (_currentDepth < 0)
            {
                if (_buffer.Length <= idx)
                {
                    GrowAndEnsure();
                    output = _buffer.Span;
                }
                output[idx++] = JsonConstants.ListSeparator;
            }

            if (_buffer.Length <= idx)
            {
                AdvanceAndGrow(ref idx);
                output = _buffer.Span;
            }
            output[idx++] = JsonConstants.Quote;

            ReadOnlySpan<byte> byteSpan = MemoryMarshal.AsBytes(escapedPropertyName);
            int partialConsumed = 0;
            while (true)
            {
                OperationStatus status = JsonWriterHelper.ToUtf8(byteSpan.Slice(partialConsumed), output.Slice(idx), out int consumed, out int written);
                idx += written;
                if (status == OperationStatus.Done)
                {
                    break;
                }
                partialConsumed += consumed;
                AdvanceAndGrow(ref idx);
                output = _buffer.Span;
            }

            if (_buffer.Length <= idx)
            {
                AdvanceAndGrow(ref idx);
                output = _buffer.Span;
            }
            output[idx++] = JsonConstants.Quote;

            if (_buffer.Length <= idx)
            {
                AdvanceAndGrow(ref idx);
                output = _buffer.Span;
            }
            output[idx++] = JsonConstants.KeyValueSeperator;

            return idx;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private int WritePropertyNameMinimizedSlow(ReadOnlySpan<char> escapedPropertyName, byte token, int maxLengthRequired)
        {
            throw new NotImplementedException();
        }

        private int WritePropertyNameMinimized(ReadOnlySpan<char> escapedPropertyName, byte token)
        {
            int maxLengthRequired = (escapedPropertyName.Length * 3) + 5;

            if (maxLengthRequired > DefaultGrowthSize)
            {
                return WritePropertyNameMinimizedSlow(escapedPropertyName, token, maxLengthRequired);
            }

            Span<byte> output = GetSpan(maxLengthRequired, escapedPropertyName.Length);

            int idx = _buffered;

            if (_currentDepth < 0)
            {
                output[idx++] = JsonConstants.ListSeparator;
            }

            output[idx++] = JsonConstants.Quote;

            ReadOnlySpan<byte> byteSpan = MemoryMarshal.AsBytes(escapedPropertyName);
            OperationStatus status = JsonWriterHelper.ToUtf8(byteSpan, output.Slice(idx), out int consumed, out int written);
            Debug.Assert(status != OperationStatus.DestinationTooSmall);
            if (status != OperationStatus.Done)
            {
                throw new InvalidOperationException();
            }
            Debug.Assert(consumed == byteSpan.Length);
            idx += written;

            output[idx++] = JsonConstants.Quote;
            output[idx++] = JsonConstants.KeyValueSeperator;
            output[idx++] = token;

            return idx;
        }

        private int WritePropertyNameIndented(ReadOnlySpan<char> escapedPropertyName)
        {
            int idx = 0;
            Span<byte> output = _buffer.Span;
            if (_currentDepth < 0)
            {
                if (_buffer.Length <= idx)
                {
                    GrowAndEnsure();
                    output = _buffer.Span;
                }
                output[idx++] = JsonConstants.ListSeparator;
            }

            if (_tokenType != JsonTokenType.None)
            {
                WriteNewLine(ref idx);
                output = _buffer.Span;
            }

            int indent = Indentation;
            while (true)
            {
                bool result = JsonWriterHelper.TryWriteIndentation(output.Slice(idx), indent, out int bytesWritten);
                idx += bytesWritten;
                if (result)
                {
                    break;
                }
                indent -= bytesWritten;
                AdvanceAndGrow(ref idx);
                output = _buffer.Span;
            }

            if (_buffer.Length <= idx)
            {
                AdvanceAndGrow(ref idx);
                output = _buffer.Span;
            }
            output[idx++] = JsonConstants.Quote;

            ReadOnlySpan<byte> byteSpan = MemoryMarshal.AsBytes(escapedPropertyName);
            int partialConsumed = 0;
            while (true)
            {
                OperationStatus status = JsonWriterHelper.ToUtf8(byteSpan.Slice(partialConsumed), output.Slice(idx), out int consumed, out int written);
                idx += written;
                if (status == OperationStatus.Done)
                {
                    break;
                }
                partialConsumed += consumed;
                AdvanceAndGrow(ref idx);
                output = _buffer.Span;
            }

            if (_buffer.Length <= idx)
            {
                AdvanceAndGrow(ref idx);
                output = _buffer.Span;
            }
            output[idx++] = JsonConstants.Quote;

            if (_buffer.Length <= idx)
            {
                AdvanceAndGrow(ref idx);
                output = _buffer.Span;
            }
            output[idx++] = JsonConstants.KeyValueSeperator;

            if (_buffer.Length <= idx)
            {
                AdvanceAndGrow(ref idx);
                output = _buffer.Span;
            }
            output[idx++] = JsonConstants.Space;

            return idx;
        }
    }
}
