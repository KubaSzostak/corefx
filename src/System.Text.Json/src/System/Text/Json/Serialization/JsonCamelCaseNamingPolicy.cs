// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

namespace System.Text.Json
{
    internal sealed class JsonCamelCaseNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name)
        {
            // If the first character is a surrogate pair, char.IsUpper will be false,
            // and we return the string unchanged
            if (string.IsNullOrEmpty(name) || !char.IsUpper(name[0]))
            {
                return name;
            }

            char[] chars = name.ToCharArray();

            for (int i = 0; i < chars.Length; i++)
            {
                if (i == 1 && !char.IsUpper(chars[i]))
                {
                    break;
                }

                bool hasNext = (i + 1 < chars.Length);

                // Stop when next char is already lowercase.
                if (i > 0 && hasNext && !char.IsUpper(chars[i + 1]))
                {
                    // If the next char is a space, lowercase current char before exiting.
                    if (chars[i + 1] == ' ')
                    {
                        chars[i] = char.ToLowerInvariant(chars[i]);
                    }

                    break;
                }

                chars[i] = char.ToLowerInvariant(chars[i]);
            }

            return new string(chars);
        }

        public string ConvertName_Custom1(string name)
        {
            if (string.IsNullOrEmpty(name) || !char.IsUpper(name[0]))
            {
                return name;
            }

            char[] chars = name.ToCharArray();

            Debug.Assert(chars.Length > 0 && char.IsUpper(chars[0]));

            chars[0] = char.ToLowerInvariant(chars[0]);

            if ((uint)chars.Length > 1)
            {
                if (!char.IsUpper(chars[1]))
                {
                    goto Done;
                }
            }
            else
            {
                goto Done;
            }

            Debug.Assert(chars.Length > 1 && char.IsUpper(chars[1]));

            for (int i = 1; i < chars.Length - 1; i++)
            {
                char nextChar = chars[i + 1];
                char currentChar = chars[i];

                // Stop when next char is already lowercase.
                if (!char.IsUpper(nextChar))
                {
                    // If the next char is a space, lowercase current char before exiting.
                    if (nextChar == ' ')
                    {
                        chars[i] = char.ToLowerInvariant(currentChar);
                    }

                    goto Done;
                }

                chars[i] = char.ToLowerInvariant(currentChar);
            }
            chars[chars.Length - 1] = char.ToLowerInvariant(chars[chars.Length - 1]);

        Done:
            return new string(chars);
        }
    }
}
