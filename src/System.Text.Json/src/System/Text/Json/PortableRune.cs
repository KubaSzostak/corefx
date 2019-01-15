// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace System.Text.Json
{
    // Implementation borrowed from https://github.com/dotnet/corefx/blob/master/src/Common/src/CoreLib/System/Text/Rune.cs
    internal readonly struct PortableRune
    {
        private readonly uint _value;

        /// <summary>
        /// Creates a <see cref="PortableRune"/> from the provided Unicode scalar value.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <paramref name="value"/> does not represent a value Unicode scalar value.
        /// </exception>
        public PortableRune(uint value)
        {
            Debug.Assert(IsValidUnicodeScalar(value));
            _value = value;
        }

        /// <summary>
        /// A <see cref="PortableRune"/> instance that represents the Unicode replacement character U+FFFD.
        /// </summary>
        public static PortableRune ReplacementChar => new PortableRune(0xFFFDU);

        /// <summary>
        /// Returns the Unicode scalar value as an integer.
        /// </summary>
        public int Value => (int)_value;

        /// <summary>
        /// Returns <see langword="true"/> iff <paramref name="value"/> is a valid Unicode scalar
        /// value, i.e., is in [ U+0000..U+D7FF ], inclusive; or [ U+E000..U+10FFFF ], inclusive.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValidUnicodeScalar(uint value)
        {
            // By XORing the incoming value with 0xD800, surrogate code points
            // are moved to the range [ U+0000..U+07FF ], and all valid scalar
            // values are clustered into the single range [ U+0800..U+10FFFF ],
            // which allows performing a single fast range check.

            return JsonWriterHelper.IsInRangeInclusive(value ^ 0xD800U, 0x800U, 0x10FFFFU);
        }
    }
}
