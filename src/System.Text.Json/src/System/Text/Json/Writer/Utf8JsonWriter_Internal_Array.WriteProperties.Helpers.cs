// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Text.Json
{
    public partial class Utf8JsonWriter_Internal_Array
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

        private void WritePropertyNameMinimized(ReadOnlySpan<byte> escapedPropertyName)
        {
            if (_currentDepth < 0)
            {
                if (_rentedBuffer.Length <= _index)
                {
                    GrowAndEnsure();
                }
                _rentedBuffer[_index++] = JsonConstants.ListSeparator;
            }

            if (_rentedBuffer.Length <= _index)
            {
                GrowAndEnsure();
            }
            _rentedBuffer[_index++] = JsonConstants.Quote;

            CopyLoop(escapedPropertyName);

            if (_rentedBuffer.Length <= _index)
            {
                GrowAndEnsure();
            }
            _rentedBuffer[_index++] = JsonConstants.Quote;

            if (_rentedBuffer.Length <= _index)
            {
                GrowAndEnsure();
            }
            _rentedBuffer[_index++] = JsonConstants.KeyValueSeperator;
        }

        //[MethodImpl(MethodImplOptions.NoInlining)]
        private void WritePropertyNameMinimized(ReadOnlySpan<byte> escapedPropertyName, byte token)
        {
            int maxLengthRequired = escapedPropertyName.Length + 5;

            if (_rentedBuffer.Length - _index < maxLengthRequired)
            {
                GrowAndEnsure(maxLengthRequired);
            }

            //var local = _tempMemoryField.Span;

            if (_currentDepth < 0)
            {
                _rentedBuffer[_index++] = JsonConstants.ListSeparator;
            }
            _rentedBuffer[_index++] = JsonConstants.Quote;

            escapedPropertyName.CopyTo(_rentedBuffer.AsSpan(_index));
            _index += escapedPropertyName.Length;

            _rentedBuffer[_index++] = JsonConstants.Quote;
            _rentedBuffer[_index++] = JsonConstants.KeyValueSeperator;
            _rentedBuffer[_index++] = token;
        }

        private void WritePropertyNameIndented(ReadOnlySpan<byte> escapedPropertyName)
        {
            if (_currentDepth < 0)
            {
                if (_rentedBuffer.Length <= _index)
                {
                    GrowAndEnsure();
                }
                _rentedBuffer[_index++] = JsonConstants.ListSeparator;
            }

            if (_tokenType != JsonTokenType.None)
                WriteNewLine();

            int indent = Indentation;
            while (true)
            {
                bool result = JsonWriterHelper.TryWriteIndentation(_rentedBuffer.AsSpan(_index), indent, out int bytesWritten);
                _index += bytesWritten;
                if (result)
                {
                    break;
                }
                indent -= bytesWritten;
                GrowAndEnsure();
            }

            if (_rentedBuffer.Length <= _index)
            {
                GrowAndEnsure();
            }
            _rentedBuffer[_index++] = JsonConstants.Quote;

            CopyLoop(escapedPropertyName);

            if (_rentedBuffer.Length <= _index)
            {
                GrowAndEnsure();
            }
            _rentedBuffer[_index++] = JsonConstants.Quote;

            if (_rentedBuffer.Length <= _index)
            {
                GrowAndEnsure();
            }
            _rentedBuffer[_index++] = JsonConstants.KeyValueSeperator;

            if (_rentedBuffer.Length <= _index)
            {
                GrowAndEnsure();
            }
            _rentedBuffer[_index++] = JsonConstants.Space;
        }

        private void WritePropertyNameMinimized(ReadOnlySpan<char> escapedPropertyName)
        {
            if (_currentDepth < 0)
            {
                if (_rentedBuffer.Length <= _index)
                {
                    GrowAndEnsure();
                }
                _rentedBuffer[_index++] = JsonConstants.ListSeparator;
            }

            if (_rentedBuffer.Length <= _index)
            {
                GrowAndEnsure();
            }
            _rentedBuffer[_index++] = JsonConstants.Quote;

            ReadOnlySpan<byte> byteSpan = MemoryMarshal.AsBytes(escapedPropertyName);
            int partialConsumed = 0;
            while (true)
            {
                OperationStatus status = JsonWriterHelper.ToUtf8(byteSpan.Slice(partialConsumed), _rentedBuffer.AsSpan(_index), out int consumed, out int written);
                _index += written;
                if (status == OperationStatus.Done)
                {
                    break;
                }
                partialConsumed += consumed;
                GrowAndEnsure();
            }

            if (_rentedBuffer.Length <= _index)
            {
                GrowAndEnsure();
            }
            _rentedBuffer[_index++] = JsonConstants.Quote;

            if (_rentedBuffer.Length <= _index)
            {
                GrowAndEnsure();
            }
            _rentedBuffer[_index++] = JsonConstants.KeyValueSeperator;
        }

        private void WritePropertyNameMinimized(ReadOnlySpan<char> escapedPropertyName, byte token)
        {
            int maxLengthRequired = (escapedPropertyName.Length * 3) + 5;

            if (_rentedBuffer.Length - _index < maxLengthRequired)
            {
                GrowAndEnsure(maxLengthRequired);
            }

            //var local = _tempMemoryField.Span;

            if (_currentDepth < 0)
            {
                _rentedBuffer[_index++] = JsonConstants.ListSeparator;
            }
            _rentedBuffer[_index++] = JsonConstants.Quote;

            ReadOnlySpan<byte> byteSpan = MemoryMarshal.AsBytes(escapedPropertyName);
            OperationStatus status = JsonWriterHelper.ToUtf8(byteSpan, _rentedBuffer.AsSpan(_index), out int consumed, out int written);
            Debug.Assert(status != OperationStatus.DestinationTooSmall);
            if (status != OperationStatus.Done)
            {
                throw new InvalidOperationException();
            }
            Debug.Assert(consumed == byteSpan.Length);
            _index += written;

            _rentedBuffer[_index++] = JsonConstants.Quote;
            _rentedBuffer[_index++] = JsonConstants.KeyValueSeperator;
            _rentedBuffer[_index++] = token;
        }

        private void WritePropertyNameIndented(ReadOnlySpan<char> escapedPropertyName)
        {
            if (_currentDepth < 0)
            {
                if (_rentedBuffer.Length <= _index)
                {
                    GrowAndEnsure();
                }
                _rentedBuffer[_index++] = JsonConstants.ListSeparator;
            }

            if (_tokenType != JsonTokenType.None)
                WriteNewLine();

            int indent = Indentation;
            while (true)
            {
                bool result = JsonWriterHelper.TryWriteIndentation(_rentedBuffer.AsSpan(_index), indent, out int bytesWritten);
                _index += bytesWritten;
                if (result)
                {
                    break;
                }
                indent -= bytesWritten;
                GrowAndEnsure();
            }

            if (_rentedBuffer.Length <= _index)
            {
                GrowAndEnsure();
            }
            _rentedBuffer[_index++] = JsonConstants.Quote;

            ReadOnlySpan<byte> byteSpan = MemoryMarshal.AsBytes(escapedPropertyName);
            int partialConsumed = 0;
            while (true)
            {
                OperationStatus status = JsonWriterHelper.ToUtf8(byteSpan.Slice(partialConsumed), _rentedBuffer.AsSpan(_index), out int consumed, out int written);
                _index += written;
                if (status == OperationStatus.Done)
                {
                    break;
                }
                partialConsumed += consumed;
                GrowAndEnsure();
            }

            if (_rentedBuffer.Length <= _index)
            {
                GrowAndEnsure();
            }
            _rentedBuffer[_index++] = JsonConstants.Quote;

            if (_rentedBuffer.Length <= _index)
            {
                GrowAndEnsure();
            }
            _rentedBuffer[_index++] = JsonConstants.KeyValueSeperator;

            if (_rentedBuffer.Length <= _index)
            {
                GrowAndEnsure();
            }
            _rentedBuffer[_index++] = JsonConstants.Space;
        }
    }
}
