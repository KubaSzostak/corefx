// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace System.Text.Json
{
    /// <summary>
    /// Provides a high-performance API for forward-only, non-cached writing of UTF-8 encoded JSON text.
    /// It writes the text sequentially with no caching and adheres to the JSON RFC
    /// by default (https://tools.ietf.org/html/rfc8259), with the exception of writing comments.
    /// </summary>
    /// <remarks>
    /// When the user attempts to write invalid JSON and validation is enabled, it throws
    /// a <see cref="InvalidOperationException"/> with a context specific error message.
    /// Since this type is a ref struct, it does not directly support async. However, it does provide
    /// support for reentrancy to write partial data, and continue writing in chunks.
    /// To be able to format the output with indentation and whitespace OR to skip validation, create an instance of 
    /// <see cref="JsonWriterState"/> and pass that in to the writer.
    /// </remarks>
    public partial class Utf8JsonWriter_Internal_Interlocked : IDisposable
    {
        private const int StackallocThreshold = 256;
        private const int DefaultGrowthSize = 4096;

        private byte[] _rentedBuffer;
        private int _index;

        private bool _inObject;
        private bool _isNotPrimitive;
        private JsonTokenType _tokenType;
        private readonly JsonWriterOptions _writerOptions;
        private BitStack _bitStack;

        // The highest order bit of _currentDepth is used to discern whether we are writing the first item in a list or not.
        // if (_currentDepth >> 31) == 1, add a list separator before writing the item
        // else, no list separator is needed since we are writing the first item.
        private int _currentDepth;

        //private static int _useCount = 0;

        private int Indentation => CurrentDepth * JsonConstants.SpacesPerIndent;

        public long BytesWritten
        {
            get
            {
                return _index;
            }
        }

        private int _callCount;

        private struct SingleThread : IDisposable
        {
            private Utf8JsonWriter_Internal_Interlocked _this;

            public static SingleThread Lock(Utf8JsonWriter_Internal_Interlocked writer)
            {
                if (Interlocked.Increment(ref writer._callCount) != 1)
                {
                    Interlocked.Decrement(ref writer._callCount);
                    ThrowHelper.ThrowInvalidOperationException("Multiple threads are accessing a shared writer.");
                }
                return new SingleThread { _this = writer };
            }

            public void Dispose()
            {
                if (_this != null)
                {
                    Interlocked.Decrement(ref _this._callCount);
                    _this = null;
                }
            }
        }

        /// <summary>
        /// Tracks the recursive depth of the nested objects / arrays within the JSON text
        /// written so far. This provides the depth of the current token.
        /// </summary>
        public int CurrentDepth => _currentDepth & JsonConstants.RemoveFlagsBitMask;

        private const int MinimumBufferSize = 256;

        /// <summary>
        /// Constructs a new <see cref="Utf8JsonWriter"/> instance.
        /// </summary>
        /// <param name="state">If this is the first call to the ctor, pass in a default state. Otherwise,
        /// capture the state from the previous instance of the <see cref="Utf8JsonWriter"/> and pass that back.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the instance of <see cref="IBufferWriter{Byte}" /> that is passed in is null.
        /// </exception>
        /// <remarks>
        /// Since this type is a ref struct, it is a stack-only type and all the limitations of ref structs apply to it.
        /// This is the reason why the ctor accepts a <see cref="JsonWriterState"/>.
        /// </remarks>
        public Utf8JsonWriter_Internal_Interlocked(JsonWriterState state = default)
        {
            _rentedBuffer = ArrayPool<byte>.Shared.Rent(MinimumBufferSize);

            _inObject = state._inObject;
            _isNotPrimitive = state._isNotPrimitive;
            _tokenType = state._tokenType;
            _writerOptions = state._writerOptions;
            _bitStack = state._bitStack;

            _currentDepth = state._currentDepth;
        }

        public void Flush(Stream stream, bool isFinalBlock = true)
        {
            using SingleThread locked = SingleThread.Lock(this);

            if (isFinalBlock && !_writerOptions.SkipValidation && (CurrentDepth != 0 || _tokenType == JsonTokenType.None))
                ThrowHelper.ThrowInvalidOperationException_DepthNonZeroOrEmptyJson(_currentDepth);

            stream.Write(_rentedBuffer);
            ClearHelper();

            //Unlock();
        }

        public void Flush(IBufferWriter<byte> output, bool isFinalBlock = true)
        {
            using SingleThread locked = SingleThread.Lock(this);

            if (isFinalBlock && !_writerOptions.SkipValidation && (CurrentDepth != 0 || _tokenType == JsonTokenType.None))
                ThrowHelper.ThrowInvalidOperationException_DepthNonZeroOrEmptyJson(_currentDepth);

            Span<byte> buffer = output.GetSpan(_index);
            _rentedBuffer.AsSpan(0, _index).CopyTo(buffer);
            output.Advance(_index);
            ClearHelper();

            //Unlock();
        }

        public async Task FlushAsync(Stream stream, CancellationToken cancellationToken = default, bool isFinalBlock = true)
        {
            using SingleThread locked = SingleThread.Lock(this);

            if (isFinalBlock && !_writerOptions.SkipValidation && (CurrentDepth != 0 || _tokenType == JsonTokenType.None))
                ThrowHelper.ThrowInvalidOperationException_DepthNonZeroOrEmptyJson(_currentDepth);

            await stream.WriteAsync(_rentedBuffer, cancellationToken);
            ClearHelper();

            //Unlock();
        }

        /// <summary>
        /// Writes the beginning of a JSON array.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the depth of the JSON has exceeded the maximum depth of 1000 
        /// OR if this would result in an invalid JSON to be written (while validation is enabled).
        /// </exception>
        public void WriteStartArray()
        {
            using SingleThread locked = SingleThread.Lock(this);

            WriteStart(JsonConstants.OpenBracket);
            _tokenType = JsonTokenType.StartArray;

            //Unlock();
        }

        /// <summary>
        /// Writes the beginning of a JSON object.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the depth of the JSON has exceeded the maximum depth of 1000 
        /// OR if this would result in an invalid JSON to be written (while validation is enabled).
        /// </exception>
        public void WriteStartObject()
        {
            using SingleThread locked = SingleThread.Lock(this);

            WriteStart(JsonConstants.OpenBrace);
            _tokenType = JsonTokenType.StartObject;

            //Unlock();
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //private void Lock1()
        //{
        //    if (Interlocked.Increment(ref _useCount) != 1)
        //    {
        //        ThrowHelper.ThrowInvalidOperationException("Multiple threads are accessing a shared writer.");
        //    }
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //private void Unlock2()
        //{
        //    Interlocked.Decrement(ref _useCount);
        //}

        private void WriteStart(byte token)
        {
            if (CurrentDepth >= JsonConstants.MaxWriterDepth)
                ThrowHelper.ThrowInvalidOperationException(ExceptionResource.DepthTooLarge, _currentDepth);

            if (_writerOptions.IndentedOrNotSkipValidation)
            {
                WriteStartSlow(token);
            }
            else
            {
                WriteStartMinimized(token);
            }

            _currentDepth &= JsonConstants.RemoveFlagsBitMask;
            _currentDepth++;
            _isNotPrimitive = true;
        }

        private void WriteStartMinimized(byte token)
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
            _rentedBuffer[_index++] = token;
        }

        private void WriteStartSlow(byte token)
        {
            Debug.Assert(_writerOptions.Indented || !_writerOptions.SkipValidation);

            if (_writerOptions.Indented)
            {
                if (!_writerOptions.SkipValidation)
                {
                    ValidateStart();
                    UpdateBitStackOnStart(token);
                }
                WriteStartIndented(token);
            }
            else
            {
                Debug.Assert(!_writerOptions.SkipValidation);
                ValidateStart();
                UpdateBitStackOnStart(token);
                WriteStartMinimized(token);
            }
        }

        private void ValidateStart()
        {
            if (_inObject)
            {
                Debug.Assert(_tokenType != JsonTokenType.None && _tokenType != JsonTokenType.StartArray);
                ThrowHelper.ThrowInvalidOperationException(ExceptionResource.CannotStartObjectArrayWithoutProperty, tokenType: _tokenType);
            }
            else
            {
                Debug.Assert(_tokenType != JsonTokenType.StartObject);
                if (_tokenType != JsonTokenType.None && (!_isNotPrimitive || CurrentDepth == 0))
                {
                    ThrowHelper.ThrowInvalidOperationException(ExceptionResource.CannotStartObjectArrayAfterPrimitiveOrClose, tokenType: _tokenType);
                }
            }
        }

        private void WriteStartIndented(byte token)
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
            _rentedBuffer[_index++] = token;
        }

        /// <summary>
        /// Writes the beginning of a JSON array with a property name as the key.
        /// </summary>
        /// <param name="utf8PropertyName">The UTF-8 encoded property name of the JSON array to be written.</param>
        /// <param name="escape">If this is set to false, the writer assumes the property name is properly escaped and skips the escaping step.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when the specified property name is too large.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the depth of the JSON has exceeded the maximum depth of 1000 
        /// OR if this would result in an invalid JSON to be written (while validation is enabled).
        /// </exception>
        public void WriteStartArray(ReadOnlySpan<byte> utf8PropertyName, bool escape = true)
        {
            using SingleThread locked = SingleThread.Lock(this);

            ValidatePropertyNameAndDepth(utf8PropertyName);

            if (escape)
            {
                WriteStartEscape(utf8PropertyName, JsonConstants.OpenBracket);
            }
            else
            {
                WriteStartByOptions(utf8PropertyName, JsonConstants.OpenBracket);
            }

            _currentDepth &= JsonConstants.RemoveFlagsBitMask;
            _currentDepth++;
            _isNotPrimitive = true;
            _tokenType = JsonTokenType.StartArray;

            //Unlock();
        }

        /// <summary>
        /// Writes the beginning of a JSON object with a property name as the key.
        /// </summary>
        /// <param name="utf8PropertyName">The UTF-8 encoded property name of the JSON object to be written.</param>
        /// <param name="escape">If this is set to false, the writer assumes the property name is properly escaped and skips the escaping step.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when the specified property name is too large.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the depth of the JSON has exceeded the maximum depth of 1000 
        /// OR if this would result in an invalid JSON to be written (while validation is enabled).
        /// </exception>
        public void WriteStartObject(ReadOnlySpan<byte> utf8PropertyName, bool escape = true)
        {
            using SingleThread locked = SingleThread.Lock(this);

            ValidatePropertyNameAndDepth(utf8PropertyName);

            if (escape)
            {
                WriteStartEscape(utf8PropertyName, JsonConstants.OpenBrace);
            }
            else
            {
                WriteStartByOptions(utf8PropertyName, JsonConstants.OpenBrace);
            }

            _currentDepth &= JsonConstants.RemoveFlagsBitMask;
            _currentDepth++;
            _isNotPrimitive = true;
            _tokenType = JsonTokenType.StartObject;

            //Unlock();
        }

        private void WriteStartEscape(ReadOnlySpan<byte> utf8PropertyName, byte token)
        {
            int propertyIdx = JsonWriterHelper.NeedsEscaping(utf8PropertyName);

            Debug.Assert(propertyIdx >= -1 && propertyIdx < int.MaxValue / 2);

            if (propertyIdx != -1)
            {
                WriteStartEscapeProperty(utf8PropertyName, token, propertyIdx);
            }
            else
            {
                WriteStartByOptions(utf8PropertyName, token);
            }
        }

        private void WriteStartByOptions(ReadOnlySpan<byte> utf8PropertyName, byte token)
        {
            ValidateWritingProperty(token);
            if (_writerOptions.Indented)
            {
                WritePropertyNameIndented(utf8PropertyName);
            }
            else
            {
                WritePropertyNameMinimized(utf8PropertyName);
            }

            if (_rentedBuffer.Length <= _index)
            {
                GrowAndEnsure();
            }

            _rentedBuffer[_index++] = token;
        }

        private void WriteStartEscapeProperty(ReadOnlySpan<byte> utf8PropertyName, byte token, int firstEscapeIndexProp)
        {
            Debug.Assert(int.MaxValue / JsonConstants.MaxExpansionFactorWhileEscaping >= utf8PropertyName.Length);
            Debug.Assert(firstEscapeIndexProp >= 0 && firstEscapeIndexProp < utf8PropertyName.Length);

            byte[] propertyArray = null;

            int length = JsonWriterHelper.GetMaxEscapedLength(utf8PropertyName.Length, firstEscapeIndexProp);
            Span<byte> escapedPropertyName;
            if (length > StackallocThreshold)
            {
                propertyArray = ArrayPool<byte>.Shared.Rent(length);
                escapedPropertyName = propertyArray;
            }
            else
            {
                // Cannot create a span directly since it gets passed to instance methods on a ref struct.
                unsafe
                {
                    byte* ptr = stackalloc byte[length];
                    escapedPropertyName = new Span<byte>(ptr, length);
                }
            }

            JsonWriterHelper.EscapeString(utf8PropertyName, escapedPropertyName, firstEscapeIndexProp, out int written);

            WriteStartByOptions(escapedPropertyName.Slice(0, written), token);

            if (propertyArray != null)
            {
                ArrayPool<byte>.Shared.Return(propertyArray);
            }
        }

        /// <summary>
        /// Writes the beginning of a JSON array with a property name as the key.
        /// </summary>
        /// <param name="propertyName">The UTF-16 encoded property name of the JSON array to be transcoded and written as UTF-8.</param>
        /// <param name="escape">If this is set to false, the writer assumes the property name is properly escaped and skips the escaping step.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when the specified property name is too large.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the depth of the JSON has exceeded the maximum depth of 1000 
        /// OR if this would result in an invalid JSON to be written (while validation is enabled).
        /// </exception>
        public void WriteStartArray(string propertyName, bool escape = true)
            => WriteStartArray(propertyName.AsSpan(), escape);

        /// <summary>
        /// Writes the beginning of a JSON object with a property name as the key.
        /// </summary>
        /// <param name="propertyName">The UTF-16 encoded property name of the JSON object to be transcoded and written as UTF-8.</param>
        /// <param name="escape">If this is set to false, the writer assumes the property name is properly escaped and skips the escaping step.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when the specified property name is too large.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the depth of the JSON has exceeded the maximum depth of 1000 
        /// OR if this would result in an invalid JSON to be written (while validation is enabled).
        /// </exception>
        public void WriteStartObject(string propertyName, bool escape = true)
            => WriteStartObject(propertyName.AsSpan(), escape);

        /// <summary>
        /// Writes the beginning of a JSON array with a property name as the key.
        /// </summary>
        /// <param name="propertyName">The UTF-16 encoded property name of the JSON array to be transcoded and written as UTF-8.</param>
        /// <param name="escape">If this is set to false, the writer assumes the property name is properly escaped and skips the escaping step.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when the specified property name is too large.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the depth of the JSON has exceeded the maximum depth of 1000 
        /// OR if this would result in an invalid JSON to be written (while validation is enabled).
        /// </exception>
        public void WriteStartArray(ReadOnlySpan<char> propertyName, bool escape = true)
        {
            using SingleThread locked = SingleThread.Lock(this);

            ValidatePropertyNameAndDepth(propertyName);

            if (escape)
            {
                WriteStartEscape(propertyName, JsonConstants.OpenBracket);
            }
            else
            {
                WriteStartByOptions(propertyName, JsonConstants.OpenBracket);
            }

            _currentDepth &= JsonConstants.RemoveFlagsBitMask;
            _currentDepth++;
            _isNotPrimitive = true;
            _tokenType = JsonTokenType.StartArray;

            //Unlock();
        }

        /// <summary>
        /// Writes the beginning of a JSON object with a property name as the key.
        /// </summary>
        /// <param name="propertyName">The UTF-16 encoded property name of the JSON object to be transcoded and written as UTF-8.</param>
        /// <param name="escape">If this is set to false, the writer assumes the property name is properly escaped and skips the escaping step.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when the specified property name is too large.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the depth of the JSON has exceeded the maximum depth of 1000 
        /// OR if this would result in an invalid JSON to be written (while validation is enabled).
        /// </exception>
        public void WriteStartObject(ReadOnlySpan<char> propertyName, bool escape = true)
        {
            using SingleThread locked = SingleThread.Lock(this);

            ValidatePropertyNameAndDepth(propertyName);

            if (escape)
            {
                WriteStartEscape(propertyName, JsonConstants.OpenBrace);
            }
            else
            {
                WriteStartByOptions(propertyName, JsonConstants.OpenBrace);
            }

            _currentDepth &= JsonConstants.RemoveFlagsBitMask;
            _currentDepth++;
            _isNotPrimitive = true;
            _tokenType = JsonTokenType.StartObject;

            //Unlock();
        }

        private void WriteStartEscape(ReadOnlySpan<char> propertyName, byte token)
        {
            int propertyIdx = JsonWriterHelper.NeedsEscaping(propertyName);

            Debug.Assert(propertyIdx >= -1 && propertyIdx < int.MaxValue / 2);

            if (propertyIdx != -1)
            {
                WriteStartEscapeProperty(propertyName, token, propertyIdx);
            }
            else
            {
                WriteStartByOptions(propertyName, token);
            }
        }

        private void WriteStartByOptions(ReadOnlySpan<char> propertyName, byte token)
        {
            ValidateWritingProperty(token);
            if (_writerOptions.Indented)
            {
                WritePropertyNameIndented(propertyName);
            }
            else
            {
                WritePropertyNameMinimized(propertyName);
            }

            if (_rentedBuffer.Length <= _index)
            {
                GrowAndEnsure();
            }

            _rentedBuffer[_index++] = token;
        }

        private void WriteStartEscapeProperty(ReadOnlySpan<char> propertyName, byte token, int firstEscapeIndexProp)
        {
            Debug.Assert(int.MaxValue / JsonConstants.MaxExpansionFactorWhileEscaping >= propertyName.Length);
            Debug.Assert(firstEscapeIndexProp >= 0 && firstEscapeIndexProp < propertyName.Length);

            char[] propertyArray = null;

            int length = JsonWriterHelper.GetMaxEscapedLength(propertyName.Length, firstEscapeIndexProp);
            Span<char> escapedPropertyName;
            if (length > StackallocThreshold)
            {
                propertyArray = ArrayPool<char>.Shared.Rent(length);
                escapedPropertyName = propertyArray;
            }
            else
            {
                // Cannot create a span directly since it gets passed to instance methods on a ref struct.
                unsafe
                {
                    char* ptr = stackalloc char[length];
                    escapedPropertyName = new Span<char>(ptr, length);
                }
            }
            JsonWriterHelper.EscapeString(propertyName, escapedPropertyName, firstEscapeIndexProp, out int written);

            WriteStartByOptions(escapedPropertyName.Slice(0, written), token);

            if (propertyArray != null)
            {
                ArrayPool<char>.Shared.Return(propertyArray);
            }
        }

        /// <summary>
        /// Writes the end of a JSON array.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown if this would result in an invalid JSON to be written (while validation is enabled).
        /// </exception>
        public void WriteEndArray()
        {
            using SingleThread locked = SingleThread.Lock(this);

            WriteEnd(JsonConstants.CloseBracket);
            _tokenType = JsonTokenType.EndArray;

            //Unlock();
        }

        /// <summary>
        /// Writes the end of a JSON object.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown if this would result in an invalid JSON to be written (while validation is enabled).
        /// </exception>
        public void WriteEndObject()
        {
            using SingleThread locked = SingleThread.Lock(this);

            WriteEnd(JsonConstants.CloseBrace);
            _tokenType = JsonTokenType.EndObject;

            //Unlock();
        }

        private void WriteEnd(byte token)
        {
            if (_writerOptions.IndentedOrNotSkipValidation)
            {
                WriteEndSlow(token);
            }
            else
            {
                WriteEndMinimized(token);
            }

            SetFlagToAddListSeparatorBeforeNextItem();
            // Necessary if WriteEndX is called without a corresponding WriteStartX first.
            if (CurrentDepth != 0)
            {
                _currentDepth--;
            }
        }

        private void WriteEndMinimized(byte token)
        {
            if (_rentedBuffer.Length <= _index)
            {
                GrowAndEnsure();
            }

            _rentedBuffer[_index++] = token;
        }

        private void WriteEndSlow(byte token)
        {
            Debug.Assert(_writerOptions.Indented || !_writerOptions.SkipValidation);

            if (_writerOptions.Indented)
            {
                if (!_writerOptions.SkipValidation)
                {
                    ValidateEnd(token);
                }
                WriteEndIndented(token);
            }
            else
            {
                Debug.Assert(!_writerOptions.SkipValidation);
                ValidateEnd(token);
                WriteEndMinimized(token);
            }
        }

        private void ValidateEnd(byte token)
        {
            if (_bitStack.CurrentDepth <= 0)
                ThrowHelper.ThrowInvalidOperationException(ExceptionResource.MismatchedObjectArray, token);

            if (token == JsonConstants.CloseBracket)
            {
                if (_inObject)
                {
                    Debug.Assert(_tokenType != JsonTokenType.None);
                    ThrowHelper.ThrowInvalidOperationException(ExceptionResource.MismatchedObjectArray, token);
                }
            }
            else
            {
                Debug.Assert(token == JsonConstants.CloseBrace);

                if (!_inObject)
                {
                    ThrowHelper.ThrowInvalidOperationException(ExceptionResource.MismatchedObjectArray, token);
                }
            }

            _inObject = _bitStack.Pop();
        }

        private void WriteEndIndented(byte token)
        {
            // Do not format/indent empty JSON object/array.
            if (_tokenType == JsonTokenType.StartObject || _tokenType == JsonTokenType.StartArray)
            {
                WriteEndMinimized(token);
            }
            else
            {
                WriteNewLine();
                int indent = Indentation;
                // Necessary if WriteEndX is called without a corresponding WriteStartX first.
                if (indent != 0)
                {
                    // The end token should be at an outer indent and since we haven't updated
                    // current depth yet, explicitly subtract here.
                    indent -= JsonConstants.SpacesPerIndent;
                }
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
                _rentedBuffer[_index++] = token;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteNewLine()
        {
            // Write '\r\n' OR '\n', depending on OS
            if (Environment.NewLine.Length == 2)
            {
                if (_rentedBuffer.Length <= _index)
                {
                    GrowAndEnsure();
                }
                _rentedBuffer[_index++] = JsonConstants.CarriageReturn;
            }

            if (_rentedBuffer.Length <= _index)
            {
                GrowAndEnsure();
            }
            _rentedBuffer[_index++] = JsonConstants.LineFeed;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UpdateBitStackOnStart(byte token)
        {
            if (token == JsonConstants.OpenBracket)
            {
                _bitStack.PushFalse();
                _inObject = false;
            }
            else
            {
                Debug.Assert(token == JsonConstants.OpenBrace);
                _bitStack.PushTrue();
                _inObject = true;
            }
        }

        private void GrowAndEnsure()
        {
            int previousSpanLength = _rentedBuffer.Length;
            CheckAndResizeBuffer(_rentedBuffer.Length + DefaultGrowthSize);
            if (_rentedBuffer.Length <= previousSpanLength)
            {
                ThrowHelper.ThrowArgumentException(ExceptionResource.FailedToGetLargerSpan);
            }
        }

        private void CheckAndResizeBuffer(int sizeHint)
        {
            Debug.Assert(_rentedBuffer != null);
            Debug.Assert(sizeHint >= 0);
            Debug.Assert(sizeHint == DefaultGrowthSize);

            int availableSpace = _rentedBuffer.Length - _index;

            if (sizeHint > availableSpace)
            {
                int growBy = Math.Max(sizeHint, _rentedBuffer.Length);

                int newSize = checked(_rentedBuffer.Length + growBy);

                byte[] oldBuffer = _rentedBuffer;

                _rentedBuffer = ArrayPool<byte>.Shared.Rent(newSize);

                Debug.Assert(oldBuffer.Length >= _index);
                Debug.Assert(_rentedBuffer.Length >= _index);

                Span<byte> previousBuffer = oldBuffer.AsSpan(0, _index);
                previousBuffer.CopyTo(_rentedBuffer);
                previousBuffer.Clear();
                ArrayPool<byte>.Shared.Return(oldBuffer);
            }

            Debug.Assert(_rentedBuffer.Length - _index > 0);
            Debug.Assert(_rentedBuffer.Length - _index >= sizeHint);
        }

        private void GrowAndEnsure(int minimumSize)
        {
            CheckAndResizeBuffer(minimumSize);
            if (_rentedBuffer.Length < minimumSize)
            {
                ThrowHelper.ThrowArgumentException(ExceptionResource.FailedToGetMinimumSizeSpan, minimumSize);
            }
        }

        private static void ThrowInvalidOperationException(int capacity)
        {
            throw new InvalidOperationException($"Cannot advance past the end of the buffer, which has a size of {capacity}.");
        }

        private void CopyLoop(ReadOnlySpan<byte> span)
        {
            while (true)
            {
                if (span.Length <= _rentedBuffer.Length - _index)
                {
                    span.CopyTo(_rentedBuffer.AsSpan(_index));
                    _index += span.Length;
                    break;
                }

                span.Slice(0, _rentedBuffer.Length - _index).CopyTo(_rentedBuffer.AsSpan(_index));
                span = span.Slice(_rentedBuffer.Length - _index);
                _index = _rentedBuffer.Length;
                GrowAndEnsure();
            }
        }

        private void SetFlagToAddListSeparatorBeforeNextItem()
        {
            _currentDepth |= 1 << 31;
        }

        public void Dispose()
        {
            using SingleThread locked = SingleThread.Lock(this);

            if (_rentedBuffer == null)
            {
                return;
            }

            ClearHelper();
            ArrayPool<byte>.Shared.Return(_rentedBuffer);
            _rentedBuffer = null;

            //Unlock();
        }

        public void Clear()
        {
            using SingleThread locked = SingleThread.Lock(this);

            CheckIfDisposed();

            ClearHelper();

            //Unlock();
        }

        private void ClearHelper()
        {
            Debug.Assert(_rentedBuffer != null);
            _rentedBuffer.AsSpan(0, _index).Clear();
            _index = 0;
        }

        private void CheckIfDisposed()
        {
            if (_rentedBuffer == null)
                throw new ObjectDisposedException(nameof(Utf8JsonWriter_Internal));
        }
    }
}
