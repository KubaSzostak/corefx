// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.Buffers.Text;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace System.Text.Json
{
    public partial class Utf8JsonWriter_Mix
    {
        /// <summary>
        /// Writes the <see cref="int"/> value (as a JSON number) as an element of a JSON array.
        /// </summary>
        /// <param name="value">The value to be written as a JSON number as an element of a JSON array.</param>
        /// <exception cref="InvalidOperationException">
        /// Thrown if this would result in an invalid JSON to be written (while validation is enabled).
        /// </exception>
        /// <remarks>
        /// Writes the <see cref="int"/> using the default <see cref="StandardFormat"/> (i.e. 'G'), for example: 32767.
        /// </remarks>
        public void WriteNumberValue(int value)
            => WriteNumberValue((long)value);

        /// <summary>
        /// Writes the <see cref="long"/> value (as a JSON number) as an element of a JSON array.
        /// </summary>
        /// <param name="value">The value to be written as a JSON number as an element of a JSON array.</param>
        /// <exception cref="InvalidOperationException">
        /// Thrown if this would result in an invalid JSON to be written (while validation is enabled).
        /// </exception>
        /// <remarks>
        /// Writes the <see cref="long"/> using the default <see cref="StandardFormat"/> (i.e. 'G'), for example: 32767.
        /// </remarks>
        public void WriteNumberValue(long value)
        {
            ValidateWritingValue();
            if (_writerOptions.Indented)
            {
                WriteNumberValueIndented(value);
            }
            else
            {
                WriteNumberValueMinimized(value);
            }

            SetFlagToAddListSeparatorBeforeNextItem();
            _tokenType = JsonTokenType.Number;
        }

        private void WriteNumberValueMinimized(long value)
        {
            int maxLengthRequired = JsonConstants.MaximumFormatInt64Length + 1;

            Span<byte> output = GetSpan(maxLengthRequired);
            int idx = _buffered;
           
            if (_currentDepth < 0)
            {
                output[idx++] = JsonConstants.ListSeparator;
            }

            bool result = Utf8Formatter.TryFormat(value, output.Slice(idx), out int bytesWritten);
            Debug.Assert(result);

            idx += bytesWritten;

            Advance(idx);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Span<byte> GetSpan(int maxLengthRequired)
        {
            //if (_output != null)
            //{
            //    if (_buffer.Length - _buffered < maxLengthRequired)
            //    {
            //        int minLengthRequired = 1;
            //        GrowAndEnsureBW(minLengthRequired, maxLengthRequired);
            //    }

            //    return _buffer.Span;
            //}
            //else
            //{
            //    if (_array.Length - _buffered < maxLengthRequired)
            //    {
            //        GrowAndEnsure(maxLengthRequired);
            //    }

            //    return _array;
            //}

            if (_array.Length - _buffered < maxLengthRequired)
            {
                GrowAndEnsure(maxLengthRequired);
            }

            return _array;
        }

        private void WriteNumberValueIndented(long value)
        {
            int idx = WriteCommaAndFormattingPreamble();

            WriteNumberValueFormatLoop(value, ref idx);

            Advance(idx);
        }
    }
}
