// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

namespace System.Text.Json
{
    public partial class Utf8JsonWriter_Final
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

        private void WriteCommaAndFormattingPreamble()
        {
            WriteListSeparator();
            WriteFormattingPreamble();
        }

        private void WriteFormattingPreamble()
        {
            if (_tokenType != JsonTokenType.None)
                WriteNewLine();

            int indent = Indentation;
            while (true)
            {
                bool result = JsonWriterHelper.TryWriteIndentation(_buffer.AsSpan(_buffered), indent, out int bytesWritten);
                _buffered += bytesWritten;
                if (result)
                {
                    break;
                }
                indent -= bytesWritten;
                GrowAndEnsure();
            }
        }

        private void WriteListSeparator()
        {
            if (_currentDepth < 0)
            {
                if (_buffer.Length <= _buffered)
                {
                    GrowAndEnsure();
                }
                _buffer[_buffered++] = JsonConstants.ListSeparator;
            }
        }
    }
}
