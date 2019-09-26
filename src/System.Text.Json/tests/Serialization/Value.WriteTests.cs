// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using Newtonsoft.Json;
using Xunit;

namespace System.Text.Json.Serialization.Tests
{
    public static partial class SqlMapper
    {
        internal sealed class DapperTable
        {
            private string[] fieldNames;
            private readonly Dictionary<string, int> fieldNameLookup;

            internal string[] FieldNames => fieldNames;

            public DapperTable(string[] fieldNames)
            {
                this.fieldNames = fieldNames ?? throw new ArgumentNullException(nameof(fieldNames));

                fieldNameLookup = new Dictionary<string, int>(fieldNames.Length, StringComparer.Ordinal);
                // if there are dups, we want the **first** key to be the "winner" - so iterate backwards
                for (int i = fieldNames.Length - 1; i >= 0; i--)
                {
                    string key = fieldNames[i];
                    if (key != null) fieldNameLookup[key] = i;
                }
            }

            internal int IndexOfName(string name)
            {
                return (name != null && fieldNameLookup.TryGetValue(name, out int result)) ? result : -1;
            }

            internal int AddField(string name)
            {
                if (name == null) throw new ArgumentNullException(nameof(name));
                if (fieldNameLookup.ContainsKey(name)) throw new InvalidOperationException("Field already exists: " + name);
                int oldLen = fieldNames.Length;
                Array.Resize(ref fieldNames, oldLen + 1); // yes, this is sub-optimal, but this is not the expected common case
                fieldNames[oldLen] = name;
                fieldNameLookup[name] = oldLen;
                return oldLen;
            }

            internal bool FieldExists(string key) => key != null && fieldNameLookup.ContainsKey(key);

            public int FieldCount => fieldNames.Length;
        }

        internal sealed partial class DapperRow
            : IDictionary<string, object>
            , IReadOnlyDictionary<string, object>
        {
            private readonly DapperTable table;
            private object[] values;

            public DapperRow(DapperTable table, object[] values)
            {
                this.table = table ?? throw new ArgumentNullException(nameof(table));
                this.values = values ?? throw new ArgumentNullException(nameof(values));
            }

            private sealed class DeadValue
            {
                public static readonly DeadValue Default = new DeadValue();
                private DeadValue() { /* hiding constructor */ }
            }

            int ICollection<KeyValuePair<string, object>>.Count
            {
                get
                {
                    int count = 0;
                    for (int i = 0; i < values.Length; i++)
                    {
                        if (!(values[i] is DeadValue)) count++;
                    }
                    return count;
                }
            }

            public bool TryGetValue(string key, out object value)
                => TryGetValue(table.IndexOfName(key), out value);

            internal bool TryGetValue(int index, out object value)
            {
                if (index < 0)
                { // doesn't exist
                    value = null;
                    return false;
                }
                // exists, **even if** we don't have a value; consider table rows heterogeneous
                value = index < values.Length ? values[index] : null;
                if (value is DeadValue)
                { // pretend it isn't here
                    value = null;
                    return false;
                }
                return true;
            }

            public override string ToString()
            {
                var sb = new StringBuilder().Append("{DapperRow");
                foreach (var kv in this)
                {
                    var value = kv.Value;
                    sb.Append(", ").Append(kv.Key);
                    if (value != null)
                    {
                        sb.Append(" = '").Append(kv.Value).Append('\'');
                    }
                    else
                    {
                        sb.Append(" = NULL");
                    }
                }

                return sb.Append('}').ToString();
            }

            public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
            {
                var names = table.FieldNames;
                for (var i = 0; i < names.Length; i++)
                {
                    object value = i < values.Length ? values[i] : null;
                    if (!(value is DeadValue))
                    {
                        yield return new KeyValuePair<string, object>(names[i], value);
                    }
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            #region Implementation of ICollection<KeyValuePair<string,object>>

            void ICollection<KeyValuePair<string, object>>.Add(KeyValuePair<string, object> item)
            {
                IDictionary<string, object> dic = this;
                dic.Add(item.Key, item.Value);
            }

            void ICollection<KeyValuePair<string, object>>.Clear()
            { // removes values for **this row**, but doesn't change the fundamental table
                for (int i = 0; i < values.Length; i++)
                    values[i] = DeadValue.Default;
            }

            bool ICollection<KeyValuePair<string, object>>.Contains(KeyValuePair<string, object> item)
            {
                return TryGetValue(item.Key, out object value) && Equals(value, item.Value);
            }

            void ICollection<KeyValuePair<string, object>>.CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
            {
                foreach (var kv in this)
                {
                    array[arrayIndex++] = kv; // if they didn't leave enough space; not our fault
                }
            }

            bool ICollection<KeyValuePair<string, object>>.Remove(KeyValuePair<string, object> item)
            {
                IDictionary<string, object> dic = this;
                return dic.Remove(item.Key);
            }

            bool ICollection<KeyValuePair<string, object>>.IsReadOnly => false;
            #endregion

            #region Implementation of IDictionary<string,object>

            bool IDictionary<string, object>.ContainsKey(string key)
            {
                int index = table.IndexOfName(key);
                if (index < 0 || index >= values.Length || values[index] is DeadValue) return false;
                return true;
            }

            void IDictionary<string, object>.Add(string key, object value)
            {
                SetValue(key, value, true);
            }

            bool IDictionary<string, object>.Remove(string key)
                => Remove(table.IndexOfName(key));

            internal bool Remove(int index)
            {
                if (index < 0 || index >= values.Length || values[index] is DeadValue) return false;
                values[index] = DeadValue.Default;
                return true;
            }

            object IDictionary<string, object>.this[string key]
            {
                get { TryGetValue(key, out object val); return val; }
                set { SetValue(key, value, false); }
            }

            public object SetValue(string key, object value)
            {
                return SetValue(key, value, false);
            }

            private object SetValue(string key, object value, bool isAdd)
            {
                if (key == null) throw new ArgumentNullException(nameof(key));
                int index = table.IndexOfName(key);
                if (index < 0)
                {
                    index = table.AddField(key);
                }
                else if (isAdd && index < values.Length && !(values[index] is DeadValue))
                {
                    // then semantically, this value already exists
                    throw new ArgumentException("An item with the same key has already been added", nameof(key));
                }
                return SetValue(index, value);
            }
            internal object SetValue(int index, object value)
            {
                int oldLength = values.Length;
                if (oldLength <= index)
                {
                    // we'll assume they're doing lots of things, and
                    // grow it to the full width of the table
                    Array.Resize(ref values, table.FieldCount);
                    for (int i = oldLength; i < values.Length; i++)
                    {
                        values[i] = DeadValue.Default;
                    }
                }
                return values[index] = value;
            }

            ICollection<string> IDictionary<string, object>.Keys
            {
                get { return this.Select(kv => kv.Key).ToArray(); }
            }

            ICollection<object> IDictionary<string, object>.Values
            {
                get { return this.Select(kv => kv.Value).ToArray(); }
            }

            #endregion


            #region Implementation of IReadOnlyDictionary<string,object>


            int IReadOnlyCollection<KeyValuePair<string, object>>.Count
            {
                get
                {
                    return values.Count(t => !(t is DeadValue));
                }
            }

            bool IReadOnlyDictionary<string, object>.ContainsKey(string key)
            {
                int index = table.IndexOfName(key);
                return index >= 0 && index < values.Length && !(values[index] is DeadValue);
            }

            object IReadOnlyDictionary<string, object>.this[string key]
            {
                get { TryGetValue(key, out object val); return val; }
            }

            IEnumerable<string> IReadOnlyDictionary<string, object>.Keys
            {
                get { return this.Select(kv => kv.Key); }
            }

            IEnumerable<object> IReadOnlyDictionary<string, object>.Values
            {
                get { return this.Select(kv => kv.Value); }
            }

            #endregion
        }
    }

    public static partial class ValueTests
    {
        [Fact]
        public static void WriteStringWithRelaxedEscaper()
        {
            string inputString = ">><++>>>\">>\\>>&>>>\u6f22\u5B57>>>"; // Non-ASCII text should remain unescaped. \u6f22 = \u6C49, \u5B57 = \u5B57

            string actual = JsonSerializer.Serialize(inputString, new JsonSerializerOptions { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping });
            string expected = "\">><++>>>\\\">>\\\\>>&>>>\u6f22\u5B57>>>\"";
            Assert.Equal(JsonConvert.SerializeObject(inputString), actual);
            Assert.Equal(expected, actual);
            Assert.NotEqual(expected, JsonSerializer.Serialize(inputString));
        }

        // https://github.com/dotnet/corefx/issues/40979
        [Fact]
        public static void EscapingShouldntStackOverflow_40979()
        {
            var test = new { Name = "\u6D4B\u8A6611" };

            var options = new JsonSerializerOptions { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            string result = JsonSerializer.Serialize(test, options);

            Assert.Equal("{\"name\":\"\u6D4B\u8A6611\"}", result);
        }

        [Fact]
        public static void DapperTest_CustomIDictionary()
        {
            List<object> installations = new List<object>();
            installations.Add(new SqlMapper.DapperRow(new SqlMapper.DapperTable(new string[] { "Value", "Text" }), new object[] { "51", "2nd Steel" }));

            string json = JsonSerializer.Serialize(installations);
            Console.WriteLine(json);
        }

        [Fact]
        public static void WritePrimitives()
        {
            {
                string json = JsonSerializer.Serialize(1);
                Assert.Equal("1", json);
            }

            {
                int? value = 1;
                string json = JsonSerializer.Serialize(value);
                Assert.Equal("1", json);
            }

            {
                int? value = null;
                string json = JsonSerializer.Serialize(value);
                Assert.Equal("null", json);
            }

            {
                string json = JsonSerializer.Serialize((string)null);
                Assert.Equal("null", json);
            }

            {
                Span<byte> json = JsonSerializer.SerializeToUtf8Bytes(1);
                Assert.Equal(Encoding.UTF8.GetBytes("1"), json.ToArray());
            }

            {
                string json = JsonSerializer.Serialize(long.MaxValue);
                Assert.Equal(long.MaxValue.ToString(), json);
            }

            {
                Span<byte> json = JsonSerializer.SerializeToUtf8Bytes(long.MaxValue);
                Assert.Equal(Encoding.UTF8.GetBytes(long.MaxValue.ToString()), json.ToArray());
            }

            {
                string json = JsonSerializer.Serialize("Hello");
                Assert.Equal(@"""Hello""", json);
            }

            {
                Span<byte> json = JsonSerializer.SerializeToUtf8Bytes("Hello");
                Assert.Equal(Encoding.UTF8.GetBytes(@"""Hello"""), json.ToArray());
            }

            {
                Uri uri = new Uri("https://domain/path");
                Assert.Equal(@"""https://domain/path""", JsonSerializer.Serialize(uri));
            }

            {
                Uri.TryCreate("~/path", UriKind.RelativeOrAbsolute, out Uri uri);
                Assert.Equal(@"""~/path""", JsonSerializer.Serialize(uri));
            }

            // The next two scenarios validate that we're NOT using Uri.ToString() for serializing Uri. The serializer
            // will escape backslashes and ampersands, but otherwise should be the same as the output of Uri.OriginalString.

            {
                // ToString would collapse the relative segment
                Uri uri = new Uri("http://a/b/../c");
                Assert.Equal(@"""http://a/b/../c""", JsonSerializer.Serialize(uri));
            }

            {
                // "%20" gets turned into a space by Uri.ToString()
                // https://coding.abel.nu/2014/10/beware-of-uri-tostring/
                Uri uri = new Uri("http://localhost?p1=Value&p2=A%20B%26p3%3DFooled!");
                Assert.Equal(@"""http://localhost?p1=Value\u0026p2=A%20B%26p3%3DFooled!""", JsonSerializer.Serialize(uri));
            }
        }
    }
}
