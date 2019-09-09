// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace System.Text.Json
{
    [DebuggerDisplay("Current: ClassType.{Current.JsonClassInfo.ClassType}, {Current.JsonClassInfo.Type.Name}")]
    internal struct ReadStack
    {
        internal static readonly char[] s_specialCharacters = { '.', ' ', '\'', '/', '"', '[', ']', '(', ')', '\t', '\n', '\r', '\f', '\b', '\\', '\u0085', '\u2028', '\u2029' };

        // A field is used instead of a property to avoid value semantics.
        public ReadStackFrame Current;

        /// <summary>
        /// Bytes consumed in the current loop
        /// </summary>
        public long BytesConsumed;

        /// <summary>
        /// Flag to let us know that we need to read ahead in the inner read loop.
        /// </summary>
        public bool ReadAhead;

        private List<ReadStackFrame> _previous;
        public int _index;

        public void Push()
        {
            if (_previous == null)
            {
                _previous = new List<ReadStackFrame>();
            }

            Debug.Assert(_index <= _previous.Count);

            if (_index == _previous.Count)
            {
                // Need to allocate a new array element.
                _previous.Add(Current);
            }
            else
            {
                // Use a previously allocated slot.
                _previous[_index] = Current;
            }

            Current.Reset();
            _index++;
        }

        public void Pop()
        {
            Debug.Assert(_index > 0);
            Current = _previous[--_index];
        }

        public bool IsLastFrame => _index == 0;

        // Return a JSONPath using simple dot-notation when possible. When special characters are present, bracket-notation is used:
        // $.x.y[0].z
        // $['PropertyName.With.Special.Chars']
        public string GetJsonPath()
        {
            var sb = new StringBuilder("$");

            for (int i = 0; i < _index; i++)
            {
                ReadStackFrame previousFrame = _previous[i];
                AppendStackFrame(sb, ref previousFrame);
            }

            AppendStackFrame(sb, ref Current);
            return sb.ToString();
        }

        private void AppendStackFrame(StringBuilder sb, ref ReadStackFrame frame)
        {
            // Append the property name.
            string propertyName = GetPropertyName(ref frame);
            AppendPropertyName(sb, propertyName);

            if (frame.JsonClassInfo != null)
            {
                if (frame.IsProcessingDictionary)
                {
                    // For dictionaries add the key.
                    AppendPropertyName(sb, frame.KeyName);
                }
                else if (frame.IsProcessingEnumerable)
                {
                    // For enumerables add the index.
                    IList list = frame.TempEnumerableValues;
                    if (list == null && frame.ReturnValue != null)
                    {
                        list = (IList)frame.JsonPropertyInfo?.GetValueAsObject(frame.ReturnValue);
                    }

                    if (list != null)
                    {
                        sb.Append(@"[");
                        sb.Append(list.Count);
                        sb.Append(@"]");
                    }
                }
            }
        }

        private void AppendPropertyName(StringBuilder sb, string propertyName)
        {
            if (propertyName != null)
            {
                if (propertyName.IndexOfAny(s_specialCharacters) != -1)
                {
                    sb.Append(@"['");
                    sb.Append(propertyName);
                    sb.Append(@"']");
                }
                else
                {
                    sb.Append('.');
                    sb.Append(propertyName);
                }
            }
        }

        private string GetPropertyName(ref ReadStackFrame frame)
        {
            return frame.KeyName;
        }
    }
}
