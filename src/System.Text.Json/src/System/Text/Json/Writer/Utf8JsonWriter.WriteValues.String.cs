// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.Buffers.Text;
using System.Diagnostics;

namespace System.Text.Json
{
    public sealed partial class Utf8JsonWriter
    {
        /// <summary>
        /// Writes the pre-encoded text value (as a JSON string) as an element of a JSON array.
        /// </summary>
        /// <param name="value">The JSON-encoded value to write.</param>
        /// <exception cref="InvalidOperationException">
        /// Thrown if this would result in invalid JSON being written (while validation is enabled).
        /// </exception>
        public void WriteStringValue(JsonEncodedText value)
            => WriteStringValueHelper(value.EncodedUtf8Bytes);

        private void WriteStringValueHelper(ReadOnlySpan<byte> utf8Value)
        {
            Debug.Assert(utf8Value.Length <= JsonConstants.MaxUnescapedTokenSize);

            WriteStringByOptions(utf8Value);

            SetFlagToAddListSeparatorBeforeNextItem();
            _tokenType = JsonTokenType.String;
        }

        /// <summary>
        /// Writes the string text value (as a JSON string) as an element of a JSON array.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when the specified value is too large.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown if this would result in invalid JSON being written (while validation is enabled).
        /// </exception>
        /// <remarks>
        /// <para>
        /// The value is escaped before writing.</para>
        /// <para>
        /// If <paramref name="value"/> is <see langword="null"/> the JSON null value is written,
        /// as if <see cref="WriteNullValue"/> was called.
        /// </para>
        /// </remarks>
        public void WriteStringValue(string value)
        {
            //if (string.IsNullOrEmpty(value))
            //{
            //    if (value == null)
            //    {
            //        WriteNullValue();
            //    }
            //    else
            //    {
            //        WriteEmptyStringValue();
            //    }
            //}
            if (value == null)
            {
                WriteNullValue();
            }
            else
            {
                WriteStringValue(value.AsSpan());
            }
        }

        private void WriteEmptyStringValue()
        {
            WriteEmptyStringByOptions();

            SetFlagToAddListSeparatorBeforeNextItem();
            _tokenType = JsonTokenType.String;
        }

        private void WriteEmptyStringByOptions()
        {
            ValidateWritingValue();
            if (_options.Indented)
            {
                WriteEmptyStringIndented();
            }
            else
            {
                WriteEmptyStringMinimized();
            }
        }

        private void WriteEmptyStringMinimized()
        {
            // 2 quotes, and optionally, 1 list separator => 2 + 1
            int maxRequired = 3;

            if (_memory.Length - BytesPending < maxRequired)
            {
                Grow(maxRequired);
            }

            Span<byte> output = _memory.Span;

            if (_currentDepth < 0)
            {
                output[BytesPending++] = JsonConstants.ListSeparator;
            }
            output[BytesPending++] = JsonConstants.Quote;
            output[BytesPending++] = JsonConstants.Quote;
        }

        private void WriteEmptyStringIndented()
        {
            int indent = Indentation;
            Debug.Assert(indent <= 2 * JsonConstants.MaxWriterDepth);

            // 2 quotes => indent + 2
            // Optionally, 1 list separator, and 1-2 bytes for new line
            int maxRequired = indent + 3 + s_newLineLength;

            if (_memory.Length - BytesPending < maxRequired)
            {
                Grow(maxRequired);
            }

            Span<byte> output = _memory.Span;

            if (_currentDepth < 0)
            {
                output[BytesPending++] = JsonConstants.ListSeparator;
            }

            if (_tokenType != JsonTokenType.PropertyName)
            {
                if (_tokenType != JsonTokenType.None)
                {
                    WriteNewLine(output);
                }
                JsonWriterHelper.WriteIndentation(output.Slice(BytesPending), indent);
                BytesPending += indent;
            }

            output[BytesPending++] = JsonConstants.Quote;
            output[BytesPending++] = JsonConstants.Quote;
        }

        /// <summary>
        /// Writes the text value (as a JSON string) as an element of a JSON array.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when the specified value is too large.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown if this would result in invalid JSON being written (while validation is enabled).
        /// </exception>
        /// <remarks>
        /// The value is escaped before writing.
        /// </remarks>
        public void WriteStringValue(ReadOnlySpan<char> value)
        {
            JsonWriterHelper.ValidateValue(value);

            WriteStringEscape(value);

            SetFlagToAddListSeparatorBeforeNextItem();
            _tokenType = JsonTokenType.String;
        }

        private void WriteStringEscape(ReadOnlySpan<char> value)
        {
            int valueIdx = JsonWriterHelper.NeedsEscaping(value, _options.Encoder);

            Debug.Assert(valueIdx >= -1 && valueIdx < value.Length);

            if (valueIdx != -1)
            {
                WriteStringEscapeValue(value, valueIdx);
            }
            else
            {
                WriteStringByOptions(value);
            }
        }

        private void WriteStringByOptions(ReadOnlySpan<char> value)
        {
            ValidateWritingValue();
            if (_options.Indented)
            {
                WriteStringIndented(value);
            }
            else
            {
                WriteStringMinimized(value);
            }
        }

        // TODO: https://github.com/dotnet/corefx/issues/36958
        private void WriteStringMinimized(ReadOnlySpan<char> escapedValue)
        {
            Debug.Assert(escapedValue.Length < (int.MaxValue / JsonConstants.MaxExpansionFactorWhileTranscoding) - 3);

            // All ASCII, 2 quotes => escapedValue.Length + 2
            // Optionally, 1 list separator, and up to 3x growth when transcoding
            int maxRequired = (escapedValue.Length * JsonConstants.MaxExpansionFactorWhileTranscoding) + 3;

            if (_memory.Length - BytesPending < maxRequired)
            {
                Grow(maxRequired);
            }

            Span<byte> output = _memory.Span;

            if (_currentDepth < 0)
            {
                output[BytesPending++] = JsonConstants.ListSeparator;
            }
            output[BytesPending++] = JsonConstants.Quote;

            TranscodeAndWrite(escapedValue, output);

            output[BytesPending++] = JsonConstants.Quote;
        }

        // TODO: https://github.com/dotnet/corefx/issues/36958
        private void WriteStringIndented(ReadOnlySpan<char> escapedValue)
        {
            int indent = Indentation;
            Debug.Assert(indent <= 2 * JsonConstants.MaxWriterDepth);

            Debug.Assert(escapedValue.Length < (int.MaxValue / JsonConstants.MaxExpansionFactorWhileTranscoding) - indent - 3 - s_newLineLength);

            // All ASCII, 2 quotes => indent + escapedValue.Length + 2
            // Optionally, 1 list separator, 1-2 bytes for new line, and up to 3x growth when transcoding
            int maxRequired = indent + (escapedValue.Length * JsonConstants.MaxExpansionFactorWhileTranscoding) + 3 + s_newLineLength;

            if (_memory.Length - BytesPending < maxRequired)
            {
                Grow(maxRequired);
            }

            Span<byte> output = _memory.Span;

            if (_currentDepth < 0)
            {
                output[BytesPending++] = JsonConstants.ListSeparator;
            }

            if (_tokenType != JsonTokenType.PropertyName)
            {
                if (_tokenType != JsonTokenType.None)
                {
                    WriteNewLine(output);
                }
                JsonWriterHelper.WriteIndentation(output.Slice(BytesPending), indent);
                BytesPending += indent;
            }

            output[BytesPending++] = JsonConstants.Quote;

            TranscodeAndWrite(escapedValue, output);

            output[BytesPending++] = JsonConstants.Quote;
        }

        private void WriteStringEscapeValue(ReadOnlySpan<char> value, int firstEscapeIndexVal)
        {
            Debug.Assert(int.MaxValue / JsonConstants.MaxExpansionFactorWhileEscaping >= value.Length);
            Debug.Assert(firstEscapeIndexVal >= 0 && firstEscapeIndexVal < value.Length);

            char[] valueArray = null;

            int length = JsonWriterHelper.GetMaxEscapedLength(value.Length, firstEscapeIndexVal);

            Span<char> escapedValue = length <= JsonConstants.StackallocThreshold ?
                stackalloc char[length] :
                (valueArray = ArrayPool<char>.Shared.Rent(length));

            JsonWriterHelper.EscapeString(value, escapedValue, firstEscapeIndexVal, _options.Encoder, out int written);

            WriteStringByOptions(escapedValue.Slice(0, written));

            if (valueArray != null)
            {
                ArrayPool<char>.Shared.Return(valueArray);
            }
        }

        /// <summary>
        /// Writes the UTF-8 text value (as a JSON string) as an element of a JSON array.
        /// </summary>
        /// <param name="utf8Value">The UTF-8 encoded value to be written as a JSON string element of a JSON array.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when the specified value is too large.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown if this would result in invalid JSON being written (while validation is enabled).
        /// </exception>
        /// <remarks>
        /// The value is escaped before writing.
        /// </remarks>
        public void WriteStringValue(ReadOnlySpan<byte> utf8Value)
        {
            JsonWriterHelper.ValidateValue(utf8Value);

            WriteStringByOptions(utf8Value);

            SetFlagToAddListSeparatorBeforeNextItem();
            _tokenType = JsonTokenType.String;
        }

        private void WriteStringByOptions(ReadOnlySpan<byte> utf8Value)
        {
            ValidateWritingValue();
            if (_options.Indented)
            {
                WriteStringIndented(utf8Value);
            }
            else
            {
                WriteStringMinimized(utf8Value);
            }
        }

        // TODO: https://github.com/dotnet/corefx/issues/36958
        private void WriteStringMinimized(ReadOnlySpan<byte> utf8Value)
        {
            Debug.Assert(utf8Value.Length < int.MaxValue - 3);

            int minRequired = (utf8Value.Length * JsonConstants.MaxExpansionFactorWhileEscaping) + 2; // 2 quotes
            int maxRequired = minRequired + 1; // Optionally, 1 list separator

            if (_memory.Length - BytesPending < maxRequired)
            {
                Grow(maxRequired);
            }

            Span<byte> output = _memory.Span;

            if (_currentDepth < 0)
            {
                output[BytesPending++] = JsonConstants.ListSeparator;
            }
            output[BytesPending++] = JsonConstants.Quote;

            int bytesWritten = EscapeAndCopy(utf8Value, output.Slice(BytesPending));
            BytesPending += bytesWritten;

            output[BytesPending++] = JsonConstants.Quote;
        }

        private int EscapeAndCopy(ReadOnlySpan<byte> utf8Value, Span<byte> destination)
        {
            if (_options.Encoder != null)
            {
                // go to slow path
            }

            int written = 0;
            int bytesConsumed;
            while (utf8Value.Length > 0)
            {
                int valueIdx = JsonWriterHelper.NeedsEscaping(utf8Value, _options.Encoder);
                Debug.Assert(valueIdx >= -1 && valueIdx < utf8Value.Length);
                if (valueIdx != -1)
                {
                    utf8Value.Slice(0, valueIdx).CopyTo(destination.Slice(written));
                    written += valueIdx;
                    byte val = utf8Value[valueIdx];
                    if (val > 0x7F)
                    {
                        // go to slow path
                    }
                    EscapeNextByte(val, destination, ref written);
                    bytesConsumed = valueIdx + 1;
                }
                else
                {
                    utf8Value.CopyTo(destination.Slice(written));
                    written += utf8Value.Length;
                    bytesConsumed = utf8Value.Length;
                }
                utf8Value = utf8Value.Slice(bytesConsumed);
            }
            return written;
        }

        private static void EscapeNextByte(byte value, Span<byte> destination, ref int written)
        {
            destination[written++] = (byte)'\\';
            switch (value)
            {
                case JsonConstants.Quote:
                    // Optimize for the common quote case.
                    destination[written++] = (byte)'u';
                    destination[written++] = (byte)'0';
                    destination[written++] = (byte)'0';
                    destination[written++] = (byte)'2';
                    destination[written++] = (byte)'2';
                    break;
                case JsonConstants.LineFeed:
                    destination[written++] = (byte)'n';
                    break;
                case JsonConstants.CarriageReturn:
                    destination[written++] = (byte)'r';
                    break;
                case JsonConstants.Tab:
                    destination[written++] = (byte)'t';
                    break;
                case JsonConstants.BackSlash:
                    destination[written++] = (byte)'\\';
                    break;
                case JsonConstants.BackSpace:
                    destination[written++] = (byte)'b';
                    break;
                case JsonConstants.FormFeed:
                    destination[written++] = (byte)'f';
                    break;
                default:
                    destination[written++] = (byte)'u';

                    bool result = Utf8Formatter.TryFormat(value, destination.Slice(written), out int bytesWritten, format: s_hexStandardFormat);
                    Debug.Assert(result);
                    Debug.Assert(bytesWritten == 4);
                    written += bytesWritten;
                    break;
            }
        }

        private static readonly StandardFormat s_hexStandardFormat = new StandardFormat('X', 4);

        // TODO: https://github.com/dotnet/corefx/issues/36958
        private void WriteStringIndented(ReadOnlySpan<byte> utf8Value)
        {
            int indent = Indentation;
            Debug.Assert(indent <= 2 * JsonConstants.MaxWriterDepth);

            Debug.Assert(utf8Value.Length < int.MaxValue - indent - 3 - s_newLineLength);

            int minRequired = indent + utf8Value.Length + 2; // 2 quotes
            int maxRequired = minRequired + 1 + s_newLineLength; // Optionally, 1 list separator and 1-2 bytes for new line

            if (_memory.Length - BytesPending < maxRequired)
            {
                Grow(maxRequired);
            }

            Span<byte> output = _memory.Span;

            if (_currentDepth < 0)
            {
                output[BytesPending++] = JsonConstants.ListSeparator;
            }

            if (_tokenType != JsonTokenType.PropertyName)
            {
                if (_tokenType != JsonTokenType.None)
                {
                    WriteNewLine(output);
                }
                JsonWriterHelper.WriteIndentation(output.Slice(BytesPending), indent);
                BytesPending += indent;
            }

            output[BytesPending++] = JsonConstants.Quote;

            utf8Value.CopyTo(output.Slice(BytesPending));
            BytesPending += utf8Value.Length;

            output[BytesPending++] = JsonConstants.Quote;
        }

        private void WriteStringEscapeValue(ReadOnlySpan<byte> utf8Value, int firstEscapeIndexVal)
        {
            Debug.Assert(int.MaxValue / JsonConstants.MaxExpansionFactorWhileEscaping >= utf8Value.Length);
            Debug.Assert(firstEscapeIndexVal >= 0 && firstEscapeIndexVal < utf8Value.Length);

            byte[] valueArray = null;

            int length = JsonWriterHelper.GetMaxEscapedLength(utf8Value.Length, firstEscapeIndexVal);

            Span<byte> escapedValue = length <= JsonConstants.StackallocThreshold ?
                stackalloc byte[length] :
                (valueArray = ArrayPool<byte>.Shared.Rent(length));

            JsonWriterHelper.EscapeString(utf8Value, escapedValue, firstEscapeIndexVal, _options.Encoder, out int written);

            WriteStringByOptions(escapedValue.Slice(0, written));

            if (valueArray != null)
            {
                ArrayPool<byte>.Shared.Return(valueArray);
            }
        }
    }
}
