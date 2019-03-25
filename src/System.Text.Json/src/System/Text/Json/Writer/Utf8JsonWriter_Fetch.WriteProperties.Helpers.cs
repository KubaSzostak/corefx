// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Text.Json
{
    public partial class Utf8JsonWriter_Fetch
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

        private int WritePropertyNameMinimized(ref Span<byte> buffer, ReadOnlySpan<byte> escapedPropertyName)
        {
            int idx = 0;
            if (_currentDepth < 0)
            {
                if (buffer.Length <= idx)
                {
                    GrowAndEnsure(ref buffer);
                }
                buffer[idx++] = JsonConstants.ListSeparator;
            }

            if (buffer.Length <= idx)
            {
                AdvanceAndGrow(ref buffer, ref idx);
            }
            buffer[idx++] = JsonConstants.Quote;

            CopyLoop(ref buffer, escapedPropertyName, ref idx);

            if (buffer.Length <= idx)
            {
                AdvanceAndGrow(ref buffer, ref idx);
            }
            buffer[idx++] = JsonConstants.Quote;

            if (buffer.Length <= idx)
            {
                AdvanceAndGrow(ref buffer, ref idx);
            }
            buffer[idx++] = JsonConstants.KeyValueSeperator;

            return idx;
        }

        private int WritePropertyNameIndented(ref Span<byte> buffer, ReadOnlySpan<byte> escapedPropertyName)
        {
            int idx = 0;
            if (_currentDepth < 0)
            {
                if (buffer.Length <= idx)
                {
                    GrowAndEnsure(ref buffer);
                }
                buffer[idx++] = JsonConstants.ListSeparator;
            }

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

            if (buffer.Length <= idx)
            {
                AdvanceAndGrow(ref buffer, ref idx);
            }
            buffer[idx++] = JsonConstants.Quote;

            CopyLoop(ref buffer, escapedPropertyName, ref idx);

            if (buffer.Length <= idx)
            {
                AdvanceAndGrow(ref buffer, ref idx);
            }
            buffer[idx++] = JsonConstants.Quote;

            if (buffer.Length <= idx)
            {
                AdvanceAndGrow(ref buffer, ref idx);
            }
            buffer[idx++] = JsonConstants.KeyValueSeperator;

            if (buffer.Length <= idx)
            {
                AdvanceAndGrow(ref buffer, ref idx);
            }
            buffer[idx++] = JsonConstants.Space;

            return idx;
        }

        private int WritePropertyNameMinimized(ref Span<byte> buffer, ReadOnlySpan<char> escapedPropertyName)
        {
            int idx = 0;
            if (_currentDepth < 0)
            {
                if (buffer.Length <= idx)
                {
                    GrowAndEnsure(ref buffer);
                }
                buffer[idx++] = JsonConstants.ListSeparator;
            }

            if (buffer.Length <= idx)
            {
                AdvanceAndGrow(ref buffer, ref idx);
            }
            buffer[idx++] = JsonConstants.Quote;

            ReadOnlySpan<byte> byteSpan = MemoryMarshal.AsBytes(escapedPropertyName);
            int partialConsumed = 0;
            while (true)
            {
                OperationStatus status = JsonWriterHelper.ToUtf8(byteSpan.Slice(partialConsumed), buffer.Slice(idx), out int consumed, out int written);
                idx += written;
                if (status == OperationStatus.Done)
                {
                    break;
                }
                partialConsumed += consumed;
                AdvanceAndGrow(ref buffer, ref idx);
            }

            if (buffer.Length <= idx)
            {
                AdvanceAndGrow(ref buffer, ref idx);
            }
            buffer[idx++] = JsonConstants.Quote;

            if (buffer.Length <= idx)
            {
                AdvanceAndGrow(ref buffer, ref idx);
            }
            buffer[idx++] = JsonConstants.KeyValueSeperator;

            return idx;
        }

        private int WritePropertyNameIndented(ref Span<byte> buffer, ReadOnlySpan<char> escapedPropertyName)
        {
            int idx = 0;
            if (_currentDepth < 0)
            {
                if (buffer.Length <= idx)
                {
                    GrowAndEnsure(ref buffer);
                }
                buffer[idx++] = JsonConstants.ListSeparator;
            }

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

            if (buffer.Length <= idx)
            {
                AdvanceAndGrow(ref buffer, ref idx);
            }
            buffer[idx++] = JsonConstants.Quote;

            ReadOnlySpan<byte> byteSpan = MemoryMarshal.AsBytes(escapedPropertyName);
            int partialConsumed = 0;
            while (true)
            {
                OperationStatus status = JsonWriterHelper.ToUtf8(byteSpan.Slice(partialConsumed), buffer.Slice(idx), out int consumed, out int written);
                idx += written;
                if (status == OperationStatus.Done)
                {
                    break;
                }
                partialConsumed += consumed;
                AdvanceAndGrow(ref buffer, ref idx);
            }

            if (buffer.Length <= idx)
            {
                AdvanceAndGrow(ref buffer, ref idx);
            }
            buffer[idx++] = JsonConstants.Quote;

            if (buffer.Length <= idx)
            {
                AdvanceAndGrow(ref buffer, ref idx);
            }
            buffer[idx++] = JsonConstants.KeyValueSeperator;

            if (buffer.Length <= idx)
            {
                AdvanceAndGrow(ref buffer, ref idx);
            }
            buffer[idx++] = JsonConstants.Space;

            return idx;
        }
    }
}
