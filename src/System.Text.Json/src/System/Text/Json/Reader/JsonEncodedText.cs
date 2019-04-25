// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.Diagnostics;
using System.Text.Encodings.Web;

namespace System.Text.Json
{
    public sealed class JsonEncodedText : IEquatable<JsonEncodedText>
    {
        private readonly string _value;
        private readonly byte[] _utf8Value;
        private readonly int _idx;
        private readonly JavaScriptEncoder _encoder;

        public byte[] EncodedUtf8String => _utf8Value;  // OR ROS<byte>?

        public string EncodedString => _value;  // OR ROS<char>?

        [CLSCompliant(false)]
        public JavaScriptEncoder Encoder => _encoder;

        // Reject any invalid UTF-8 data rather than silently replacing.
        private static readonly UTF8Encoding s_utf8Encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);

        public JsonEncodedText(string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            _encoder = JavaScriptEncoder.Default;

            if (value.Length == 0)
            {
                _value = string.Empty;
                _utf8Value = Array.Empty<byte>();
                _idx = -1;
            }
            else
            {
                JsonWriterHelper.ValidateValue(value);

                //_idx = JsonWriterHelper.NeedsEscaping(value);
                //if (_idx != -1)
                //{
                //    _value = GetEscapedString(value, _idx);
                //}
                //else
                //{
                //    _value = value;
                //}

                unsafe
                {
                    fixed (char* strPtr = value)
                    {
                        _idx = _encoder.FindFirstCharacterToEncode(strPtr, value.Length);

                        if (_idx != -1)
                        {
                            _value = _encoder.Encode(value);
                        }
                        else
                        {
                            _value = value;
                        }
                    }
                }

                _utf8Value = s_utf8Encoding.GetBytes(_value);
            }
        }

        [CLSCompliant(false)]
        public JsonEncodedText(string value, JavaScriptEncoder encoder) // OR optional encoder parameter where null means default
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            _encoder = encoder ?? throw new ArgumentNullException(nameof(encoder));

            if (value.Length == 0)
            {
                _value = string.Empty;
                _utf8Value = Array.Empty<byte>();
                _idx = -1;
            }
            else
            {
                JsonWriterHelper.ValidateValue(value);

                unsafe
                {
                    fixed (char* strPtr = value)
                    {
                        _idx = _encoder.FindFirstCharacterToEncode(strPtr, value.Length);

                        if (_idx != -1)
                        {
                            _value = _encoder.Encode(value);
                        }
                        else
                        {
                            _value = value;
                        }
                    }
                }

                _utf8Value = s_utf8Encoding.GetBytes(_value);
            }
        }

        public JsonEncodedText(ReadOnlySpan<char> value)
        {
            _encoder = JavaScriptEncoder.Default;

            if (value.Length == 0)
            {
                _value = string.Empty;
                _utf8Value = Array.Empty<byte>();
                _idx = -1;
            }
            else
            {
                JsonWriterHelper.ValidateValue(value);

                _idx = JsonWriterHelper.NeedsEscaping(value);
                if (_idx != -1)
                {
                    _value = GetEscapedString(value, _idx);
                }
                else
                {
                    _value = value.ToString();
                }
                _utf8Value = s_utf8Encoding.GetBytes(_value);
            }
        }

        [CLSCompliant(false)]
        public JsonEncodedText(ReadOnlySpan<char> value, JavaScriptEncoder encoder)
        {
            _encoder = encoder ?? throw new ArgumentNullException(nameof(encoder));

            if (value.Length == 0)
            {
                _value = string.Empty;
                _utf8Value = Array.Empty<byte>();
                _idx = -1;
            }
            else
            {
                JsonWriterHelper.ValidateValue(value);

                _idx = JsonWriterHelper.NeedsEscaping(value);
                if (_idx != -1)
                {
                    _value = GetEscapedString(value, _idx);
                }
                else
                {
                    _value = value.ToString();
                }
                _utf8Value = s_utf8Encoding.GetBytes(_value);
            }
        }

        public JsonEncodedText(ReadOnlySpan<byte> utf8Value)
        {
            _encoder = JavaScriptEncoder.Default;

            if (utf8Value.Length == 0)
            {
                _value = string.Empty;
                _utf8Value = Array.Empty<byte>();
                _idx = -1;
            }
            else
            {
                JsonWriterHelper.ValidateValue(utf8Value);

                _idx = JsonWriterHelper.NeedsEscaping(utf8Value);
                if (_idx != -1)
                {
                    _utf8Value = GetEscapedString(utf8Value, _idx);
                }
                else
                {
                    _utf8Value = utf8Value.ToArray();
                }

                _value = s_utf8Encoding.GetString(_utf8Value);
            }
        }

        [CLSCompliant(false)]
        public JsonEncodedText(ReadOnlySpan<byte> utf8Value, JavaScriptEncoder encoder)
        {
            _encoder = encoder ?? throw new ArgumentNullException(nameof(encoder));

            if (utf8Value.Length == 0)
            {
                _value = string.Empty;
                _utf8Value = Array.Empty<byte>();
                _idx = -1;
            }
            else
            {
                JsonWriterHelper.ValidateValue(utf8Value);

                _idx = JsonWriterHelper.NeedsEscaping(utf8Value);
                if (_idx != -1)
                {
                    _utf8Value = GetEscapedString(utf8Value, _idx);
                }
                else
                {
                    _utf8Value = utf8Value.ToArray();
                }

                _value = s_utf8Encoding.GetString(_utf8Value);
            }
        }

        private string GetEscapedString(ReadOnlySpan<char> value, int firstEscapeIndexVal)
        {
            Debug.Assert(int.MaxValue / JsonConstants.MaxExpansionFactorWhileEscaping >= value.Length);
            Debug.Assert(firstEscapeIndexVal >= 0 && firstEscapeIndexVal < value.Length);

            char[] valueArray = null;

            int length = JsonWriterHelper.GetMaxEscapedLength(value.Length, firstEscapeIndexVal);

            Span<char> escapedValue = length <= JsonConstants.StackallocThreshold ?
                stackalloc char[length] :
                (valueArray = ArrayPool<char>.Shared.Rent(length));

            JsonWriterHelper.EscapeString(value, escapedValue, firstEscapeIndexVal, out int written);

            string escapedString = escapedValue.Slice(0, written).ToString();

            if (valueArray != null)
            {
                ArrayPool<char>.Shared.Return(valueArray);
            }
            return escapedString;
        }

        private byte[] GetEscapedString(ReadOnlySpan<byte> utf8Value, int firstEscapeIndexVal)
        {
            Debug.Assert(int.MaxValue / JsonConstants.MaxExpansionFactorWhileEscaping >= utf8Value.Length);
            Debug.Assert(firstEscapeIndexVal >= 0 && firstEscapeIndexVal < utf8Value.Length);

            byte[] valueArray = null;

            int length = JsonWriterHelper.GetMaxEscapedLength(utf8Value.Length, firstEscapeIndexVal);

            Span<byte> escapedValue = length <= JsonConstants.StackallocThreshold ?
                stackalloc byte[length] :
                (valueArray = ArrayPool<byte>.Shared.Rent(length));

            JsonWriterHelper.EscapeString(utf8Value, escapedValue, firstEscapeIndexVal, out int written);

            byte[] escapedString = escapedValue.Slice(0, written).ToArray();

            if (valueArray != null)
            {
                ArrayPool<byte>.Shared.Return(valueArray);
            }

            return escapedString;
        }

        [CLSCompliant(false)]
        public bool IsAlreadyEncoded(JavaScriptEncoder encoder)
        {
            if (encoder == _encoder)
            {
                return true;
            }

            unsafe
            {
                fixed (char* strPtr = _value)
                {
                    int idx = _encoder.FindFirstCharacterToEncode(strPtr, _value.Length);

                    if (idx != -1)
                    {
                        return _value.Equals(_encoder.Encode(_value));
                    }
                }
            }

            return true;
        }

        public bool Equals(JsonEncodedText other)
        {
            if (_value == other._value)
            {
                return true;
            }

            if (_value.Length != other._value.Length)
            {
                return false;
            }

            return _utf8Value.AsSpan().SequenceEqual(other._utf8Value);
        }

        public override string ToString()
        {
            return _value;
        }
    }
}
