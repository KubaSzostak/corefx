// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Text.Json
{
    public ref partial struct Utf8JsonWriter_Internal_Ref
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
                if (_pool._rentedBuffer.Length <= _index)
                {
                    GrowAndEnsure();
                }
                _pool._rentedBuffer[_index++] = JsonConstants.ListSeparator;
            }

            if (_pool._rentedBuffer.Length <= _index)
            {
                GrowAndEnsure();
            }
            _pool._rentedBuffer[_index++] = JsonConstants.Quote;

            CopyLoop(escapedPropertyName);

            if (_pool._rentedBuffer.Length <= _index)
            {
                GrowAndEnsure();
            }
            _pool._rentedBuffer[_index++] = JsonConstants.Quote;

            if (_pool._rentedBuffer.Length <= _index)
            {
                GrowAndEnsure();
            }
            _pool._rentedBuffer[_index++] = JsonConstants.KeyValueSeperator;
        }

        private void WritePropertyNameIndented(ReadOnlySpan<byte> escapedPropertyName)
        {
            if (_currentDepth < 0)
            {
                if (_pool._rentedBuffer.Length <= _index)
                {
                    GrowAndEnsure();
                }
                _pool._rentedBuffer[_index++] = JsonConstants.ListSeparator;
            }

            if (_tokenType != JsonTokenType.None)
                WriteNewLine();

            int indent = Indentation;
            while (true)
            {
                bool result = JsonWriterHelper.TryWriteIndentation(_pool._rentedBuffer.AsSpan(_index), indent, out int bytesWritten);
                _index += bytesWritten;
                if (result)
                {
                    break;
                }
                indent -= bytesWritten;
                GrowAndEnsure();
            }

            if (_pool._rentedBuffer.Length <= _index)
            {
                GrowAndEnsure();
            }
            _pool._rentedBuffer[_index++] = JsonConstants.Quote;

            CopyLoop(escapedPropertyName);

            if (_pool._rentedBuffer.Length <= _index)
            {
                GrowAndEnsure();
            }
            _pool._rentedBuffer[_index++] = JsonConstants.Quote;

            if (_pool._rentedBuffer.Length <= _index)
            {
                GrowAndEnsure();
            }
            _pool._rentedBuffer[_index++] = JsonConstants.KeyValueSeperator;

            if (_pool._rentedBuffer.Length <= _index)
            {
                GrowAndEnsure();
            }
            _pool._rentedBuffer[_index++] = JsonConstants.Space;
        }

        private void WritePropertyNameMinimized(ReadOnlySpan<char> escapedPropertyName)
        {
            if (_currentDepth < 0)
            {
                if (_pool._rentedBuffer.Length <= _index)
                {
                    GrowAndEnsure();
                }
                _pool._rentedBuffer[_index++] = JsonConstants.ListSeparator;
            }

            if (_pool._rentedBuffer.Length <= _index)
            {
                GrowAndEnsure();
            }
            _pool._rentedBuffer[_index++] = JsonConstants.Quote;

            ReadOnlySpan<byte> byteSpan = MemoryMarshal.AsBytes(escapedPropertyName);
            int partialConsumed = 0;
            while (true)
            {
                OperationStatus status = JsonWriterHelper.ToUtf8(byteSpan.Slice(partialConsumed), _pool._rentedBuffer.AsSpan(_index), out int consumed, out int written);
                _index += written;
                if (status == OperationStatus.Done)
                {
                    break;
                }
                partialConsumed += consumed;
                GrowAndEnsure();
            }

            if (_pool._rentedBuffer.Length <= _index)
            {
                GrowAndEnsure();
            }
            _pool._rentedBuffer[_index++] = JsonConstants.Quote;

            if (_pool._rentedBuffer.Length <= _index)
            {
                GrowAndEnsure();
            }
            _pool._rentedBuffer[_index++] = JsonConstants.KeyValueSeperator;
        }

        private void WritePropertyNameIndented(ReadOnlySpan<char> escapedPropertyName)
        {
            if (_currentDepth < 0)
            {
                if (_pool._rentedBuffer.Length <= _index)
                {
                    GrowAndEnsure();
                }
                _pool._rentedBuffer[_index++] = JsonConstants.ListSeparator;
            }

            if (_tokenType != JsonTokenType.None)
                WriteNewLine();

            int indent = Indentation;
            while (true)
            {
                bool result = JsonWriterHelper.TryWriteIndentation(_pool._rentedBuffer.AsSpan(_index), indent, out int bytesWritten);
                _index += bytesWritten;
                if (result)
                {
                    break;
                }
                indent -= bytesWritten;
                GrowAndEnsure();
            }

            if (_pool._rentedBuffer.Length <= _index)
            {
                GrowAndEnsure();
            }
            _pool._rentedBuffer[_index++] = JsonConstants.Quote;

            ReadOnlySpan<byte> byteSpan = MemoryMarshal.AsBytes(escapedPropertyName);
            int partialConsumed = 0;
            while (true)
            {
                OperationStatus status = JsonWriterHelper.ToUtf8(byteSpan.Slice(partialConsumed), _pool._rentedBuffer.AsSpan(_index), out int consumed, out int written);
                _index += written;
                if (status == OperationStatus.Done)
                {
                    break;
                }
                partialConsumed += consumed;
                GrowAndEnsure();
            }

            if (_pool._rentedBuffer.Length <= _index)
            {
                GrowAndEnsure();
            }
            _pool._rentedBuffer[_index++] = JsonConstants.Quote;

            if (_pool._rentedBuffer.Length <= _index)
            {
                GrowAndEnsure();
            }
            _pool._rentedBuffer[_index++] = JsonConstants.KeyValueSeperator;

            if (_pool._rentedBuffer.Length <= _index)
            {
                GrowAndEnsure();
            }
            _pool._rentedBuffer[_index++] = JsonConstants.Space;
        }
    }
}
