// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Text.Json
{
    public partial class Utf8JsonWriter_Final
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
                if (_buffer.Length <= _buffered)
                {
                    GrowAndEnsure();
                }
                _buffer[_buffered++] = JsonConstants.ListSeparator;
            }

            if (_buffer.Length <= _buffered)
            {
                GrowAndEnsure();
            }
            _buffer[_buffered++] = JsonConstants.Quote;

            CopyLoop(escapedPropertyName);

            if (_buffer.Length <= _buffered)
            {
                GrowAndEnsure();
            }
            _buffer[_buffered++] = JsonConstants.Quote;

            if (_buffer.Length <= _buffered)
            {
                GrowAndEnsure();
            }
            _buffer[_buffered++] = JsonConstants.KeyValueSeperator;
        }

        private void WritePropertyNameMinimized(ReadOnlySpan<byte> escapedPropertyName, byte token)
        {
            int maxLengthRequired = escapedPropertyName.Length + 5;

            if (_memory.Length - _buffered < maxLengthRequired)
            {
                GrowAndEnsure(maxLengthRequired);
            }

            Span<byte> output = _memory.Span;

            if (_currentDepth < 0)
            {
                output[_buffered++] = JsonConstants.ListSeparator;
            }
            output[_buffered++] = JsonConstants.Quote;

            escapedPropertyName.CopyTo(output.Slice(_buffered));
            _buffered += escapedPropertyName.Length;

            output[_buffered++] = JsonConstants.Quote;
            output[_buffered++] = JsonConstants.KeyValueSeperator;
            output[_buffered++] = token;
        }

        private void WritePropertyNameIndented(ReadOnlySpan<byte> escapedPropertyName)
        {
            if (_currentDepth < 0)
            {
                if (_buffer.Length <= _buffered)
                {
                    GrowAndEnsure();
                }
                _buffer[_buffered++] = JsonConstants.ListSeparator;
            }

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

            if (_buffer.Length <= _buffered)
            {
                GrowAndEnsure();
            }
            _buffer[_buffered++] = JsonConstants.Quote;

            CopyLoop(escapedPropertyName);

            if (_buffer.Length <= _buffered)
            {
                GrowAndEnsure();
            }
            _buffer[_buffered++] = JsonConstants.Quote;

            if (_buffer.Length <= _buffered)
            {
                GrowAndEnsure();
            }
            _buffer[_buffered++] = JsonConstants.KeyValueSeperator;

            if (_buffer.Length <= _buffered)
            {
                GrowAndEnsure();
            }
            _buffer[_buffered++] = JsonConstants.Space;
        }

        private void WritePropertyNameMinimized(ReadOnlySpan<char> escapedPropertyName)
        {
            if (_currentDepth < 0)
            {
                if (_buffer.Length <= _buffered)
                {
                    GrowAndEnsure();
                }
                _buffer[_buffered++] = JsonConstants.ListSeparator;
            }

            if (_buffer.Length <= _buffered)
            {
                GrowAndEnsure();
            }
            _buffer[_buffered++] = JsonConstants.Quote;

            ReadOnlySpan<byte> byteSpan = MemoryMarshal.AsBytes(escapedPropertyName);
            int partialConsumed = 0;
            while (true)
            {
                OperationStatus status = JsonWriterHelper.ToUtf8(byteSpan.Slice(partialConsumed), _buffer.AsSpan(_buffered), out int consumed, out int written);
                _buffered += written;
                if (status == OperationStatus.Done)
                {
                    break;
                }
                partialConsumed += consumed;
                GrowAndEnsure();
            }

            if (_buffer.Length <= _buffered)
            {
                GrowAndEnsure();
            }
            _buffer[_buffered++] = JsonConstants.Quote;

            if (_buffer.Length <= _buffered)
            {
                GrowAndEnsure();
            }
            _buffer[_buffered++] = JsonConstants.KeyValueSeperator;
        }

        private void WritePropertyNameMinimized(ReadOnlySpan<char> escapedPropertyName, byte token)
        {
            int maxLengthRequired = (escapedPropertyName.Length * 3) + 5;

            if (_buffer.Length - _buffered < maxLengthRequired)
            {
                GrowAndEnsure(maxLengthRequired);
            }

            if (_currentDepth < 0)
            {
                _buffer[_buffered++] = JsonConstants.ListSeparator;
            }
            _buffer[_buffered++] = JsonConstants.Quote;

            ReadOnlySpan<byte> byteSpan = MemoryMarshal.AsBytes(escapedPropertyName);
            OperationStatus status = JsonWriterHelper.ToUtf8(byteSpan, _buffer.AsSpan(_buffered), out int consumed, out int written);
            Debug.Assert(status != OperationStatus.DestinationTooSmall);
            if (status != OperationStatus.Done)
            {
                throw new InvalidOperationException();
            }
            Debug.Assert(consumed == byteSpan.Length);
            _buffered += written;

            _buffer[_buffered++] = JsonConstants.Quote;
            _buffer[_buffered++] = JsonConstants.KeyValueSeperator;
            _buffer[_buffered++] = token;
        }

        private void WritePropertyNameIndented(ReadOnlySpan<char> escapedPropertyName)
        {
            if (_currentDepth < 0)
            {
                if (_buffer.Length <= _buffered)
                {
                    GrowAndEnsure();
                }
                _buffer[_buffered++] = JsonConstants.ListSeparator;
            }

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

            if (_buffer.Length <= _buffered)
            {
                GrowAndEnsure();
            }
            _buffer[_buffered++] = JsonConstants.Quote;

            ReadOnlySpan<byte> byteSpan = MemoryMarshal.AsBytes(escapedPropertyName);
            int partialConsumed = 0;
            while (true)
            {
                OperationStatus status = JsonWriterHelper.ToUtf8(byteSpan.Slice(partialConsumed), _buffer.AsSpan(_buffered), out int consumed, out int written);
                _buffered += written;
                if (status == OperationStatus.Done)
                {
                    break;
                }
                partialConsumed += consumed;
                GrowAndEnsure();
            }

            if (_buffer.Length <= _buffered)
            {
                GrowAndEnsure();
            }
            _buffer[_buffered++] = JsonConstants.Quote;

            if (_buffer.Length <= _buffered)
            {
                GrowAndEnsure();
            }
            _buffer[_buffered++] = JsonConstants.KeyValueSeperator;

            if (_buffer.Length <= _buffered)
            {
                GrowAndEnsure();
            }
            _buffer[_buffered++] = JsonConstants.Space;
        }
    }
}
