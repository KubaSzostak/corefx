// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

namespace System.Text.Json
{
    public partial class Utf8JsonWriter_Fetch
    {
        private void ValidateWritingValue()
        {
            if (!_writerOptions.SkipValidation)
            {
                if (_inObject)
                {
                    Debug.Assert(_tokenType != JsonTokenType.None && _tokenType != JsonTokenType.StartArray);
                    ThrowHelper.ThrowInvalidOperationException(ExceptionResource.CannotWriteValueWithinObject, tokenType: _tokenType);
                }
                else
                {
                    if (!_isNotPrimitive && _tokenType != JsonTokenType.None)
                    {
                        ThrowHelper.ThrowInvalidOperationException(ExceptionResource.CannotWriteValueAfterPrimitive, tokenType: _tokenType);
                    }
                }
            }
        }

        private int WriteCommaAndFormattingPreamble(ref Span<byte> buffer)
        {
            int idx = 0;
            WriteListSeparator(ref buffer, ref idx);
            WriteFormattingPreamble(ref buffer, ref idx);
            return idx;
        }

        private void WriteFormattingPreamble(ref Span<byte> buffer, ref int idx)
        {
            if (_tokenType != JsonTokenType.None)
                WriteNewLine(ref buffer, ref idx);

            int indent = Indentation;
            while (true)
            {
                bool result = JsonWriterHelper.TryWriteIndentation(buffer.Slice(idx), indent, out int bytesWritten);
                idx += bytesWritten;
                if (result)
                {
                    break;
                }
                indent -= bytesWritten;
                AdvanceAndGrow(ref buffer, ref idx);
            }
        }

        private void WriteListSeparator(ref Span<byte> buffer, ref int idx)
        {
            if (_currentDepth < 0)
            {
                if (buffer.Length <= idx)
                {
                    GrowAndEnsure(ref buffer);
                }
                buffer[idx++] = JsonConstants.ListSeparator;
            }
        }
    }
}
