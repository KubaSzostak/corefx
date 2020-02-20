// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.Buffers.Text;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text.Encodings.Web;
using System.Text.Json.Tests;
using System.Text.Unicode;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using NuGet.Frameworks;
using Xunit;

namespace System.Collections.Immutable
{
    //public class ImmutableArray<T> : IEnumerable<T>
    //{
    //    public IEnumerator<T> GetEnumerator()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    IEnumerator IEnumerable.GetEnumerator()
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}

namespace System.Text.Json.Serialization.Tests
{
    //public static class Extensions
    //{
    //    public static string SortProperties(this JsonElement jsonElement)
    //    {
    //        using (var stream = new MemoryStream())
    //        {
    //            using (var writer = new Utf8JsonWriter(stream))
    //            {
    //                jsonElement.SortPropertiesCore(writer);
    //            }
    //            return Encoding.UTF8.GetString(stream.ToArray());
    //        }
    //    }

    //    public static string SortPropertiesIBW(this JsonElement jsonElement)
    //    {
    //        var output = new ArrayBufferWriter<byte>();
    //        using (var writer = new Utf8JsonWriter(output))
    //        {
    //            jsonElement.SortPropertiesCore(writer);
    //        }
    //        return Encoding.UTF8.GetString(output.WrittenSpan);
    //    }

    //    public static string SortPropertiesIBW(this JsonElement jsonElement, ArrayBufferWriter<byte> output)
    //    {
    //        using (var writer = new Utf8JsonWriter(output))
    //        {
    //            jsonElement.SortPropertiesCore(writer);
    //        }
    //        return Encoding.UTF8.GetString(output.WrittenSpan);
    //    }

    //    private static void SortPropertiesCore(this JsonElement jsonElement, Utf8JsonWriter writer)
    //    {
    //        switch (jsonElement.ValueKind)
    //        {
    //            case JsonValueKind.Undefined:
    //                throw new InvalidOperationException();
    //            case JsonValueKind.Object:
    //                jsonElement.SortObjectProperties(writer);
    //                break;
    //            case JsonValueKind.Array:
    //                jsonElement.SortArrayProperties(writer);
    //                break;
    //            case JsonValueKind.String:
    //            case JsonValueKind.Number:
    //            case JsonValueKind.True:
    //            case JsonValueKind.False:
    //            case JsonValueKind.Null:
    //                jsonElement.WriteTo(writer);
    //                break;
    //        };
    //    }

    //    private static void SortObjectProperties(this JsonElement jObject, Utf8JsonWriter writer)
    //    {
    //        Debug.Assert(jObject.ValueKind == JsonValueKind.Object);

    //        writer.WriteStartObject();
    //        foreach (JsonProperty prop in jObject.EnumerateObject().OrderBy(p => p.Name))
    //        {
    //            writer.WritePropertyName(prop.Name);
    //            prop.Value.WriteElementHelper(writer);
    //        }
    //        writer.WriteEndObject();
    //    }

    //    private static void SortArrayProperties(this JsonElement jArray, Utf8JsonWriter writer)
    //    {
    //        Debug.Assert(jArray.ValueKind == JsonValueKind.Array);

    //        writer.WriteStartArray();
    //        foreach (JsonElement item in jArray.EnumerateArray())
    //        {
    //            item.WriteElementHelper(writer);
    //        }
    //        writer.WriteEndArray();
    //    }

    //    private static void WriteElementHelper(this JsonElement item, Utf8JsonWriter writer)
    //    {
    //        Debug.Assert(item.ValueKind != JsonValueKind.Undefined);

    //        if (item.ValueKind == JsonValueKind.Object)
    //        {
    //            item.SortObjectProperties(writer);
    //        }
    //        else if (item.ValueKind == JsonValueKind.Array)
    //        {
    //            item.SortArrayProperties(writer);
    //        }
    //        else
    //        {
    //            item.WriteTo(writer);
    //        }
    //    }

    //    public static string SortPropertiesIBW_Custom(this JsonElement jsonElement, ArrayBufferWriter<byte> output)
    //    {
    //        using (var writer = new Utf8JsonWriter(output))
    //        {
    //            jsonElement.SortPropertiesCore_Custom(writer);
    //        }
    //        return Encoding.UTF8.GetString(output.WrittenSpan);
    //    }

    //    private static void SortPropertiesCore_Custom(this JsonElement jsonElement, Utf8JsonWriter writer)
    //    {
    //        switch (jsonElement.ValueKind)
    //        {
    //            case JsonValueKind.Undefined:
    //                throw new InvalidOperationException();
    //            case JsonValueKind.Object:
    //                jsonElement.SortObjectProperties_Custom(writer);
    //                break;
    //            case JsonValueKind.Array:
    //                jsonElement.SortArrayProperties_Custom(writer);
    //                break;
    //            case JsonValueKind.String:
    //            case JsonValueKind.Number:
    //            case JsonValueKind.True:
    //            case JsonValueKind.False:
    //            case JsonValueKind.Null:
    //                jsonElement.WriteTo(writer);
    //                break;
    //        };
    //    }

    //    private static void SortObjectProperties_Custom(this JsonElement jObject, Utf8JsonWriter writer)
    //    {
    //        Debug.Assert(jObject.ValueKind == JsonValueKind.Object);

    //        var propertyNames = new List<string>();
    //        foreach (JsonProperty prop in jObject.EnumerateObject())
    //        {
    //            propertyNames.Add(prop.Name);
    //        }
    //        propertyNames.Sort();

    //        writer.WriteStartObject();
    //        foreach (string name in propertyNames)
    //        {
    //            writer.WritePropertyName(name);
    //            jObject.GetProperty(name).WriteElementHelper_Custom(writer);
    //        }
    //        writer.WriteEndObject();
    //    }

    //    private static void SortArrayProperties_Custom(this JsonElement jArray, Utf8JsonWriter writer)
    //    {
    //        Debug.Assert(jArray.ValueKind == JsonValueKind.Array);

    //        writer.WriteStartArray();
    //        foreach (JsonElement item in jArray.EnumerateArray())
    //        {
    //            item.WriteElementHelper_Custom(writer);
    //        }
    //        writer.WriteEndArray();
    //    }

    //    private static void WriteElementHelper_Custom(this JsonElement item, Utf8JsonWriter writer)
    //    {
    //        Debug.Assert(item.ValueKind != JsonValueKind.Undefined);

    //        if (item.ValueKind == JsonValueKind.Object)
    //        {
    //            item.SortObjectProperties_Custom(writer);
    //        }
    //        else if (item.ValueKind == JsonValueKind.Array)
    //        {
    //            item.SortArrayProperties_Custom(writer);
    //        }
    //        else
    //        {
    //            item.WriteTo(writer);
    //        }
    //    }

    //    public static JsonElement SortProperties_old(this JsonElement jObject)
    //    {
    //        using (var stream = new MemoryStream())
    //        {
    //            using (var writer = new Utf8JsonWriter(stream))
    //            {
    //                writer.WriteStartObject();
    //                foreach (JsonProperty prop in jObject.EnumerateObject().OrderBy(p => p.Name))
    //                {
    //                    writer.WritePropertyName(prop.Name);
    //                    if (prop.Value.ValueKind == JsonValueKind.Object)
    //                    {
    //                        prop.Value.SortProperties_old().WriteTo(writer);
    //                    }
    //                    else if (prop.Value.ValueKind == JsonValueKind.Array)
    //                    {
    //                        prop.Value.SortArrayProperties_old().WriteTo(writer);
    //                    }
    //                    else
    //                    {
    //                        prop.Value.WriteTo(writer);
    //                    }
    //                }
    //                writer.WriteEndObject();
    //                writer.Flush();
    //                return JsonDocument.Parse(stream.ToArray()).RootElement;
    //            }
    //        }
    //    }

    //    public static JsonElement SortArrayProperties_old(this JsonElement jArray)
    //    {
    //        if (jArray.GetArrayLength() == 0)
    //        {
    //            return jArray;
    //        }

    //        using (var stream = new MemoryStream())
    //        {
    //            using (var writer = new Utf8JsonWriter(stream))
    //            {
    //                writer.WriteStartArray();
    //                foreach (JsonElement item in jArray.EnumerateArray())
    //                {
    //                    if (item.ValueKind == JsonValueKind.Object)
    //                    {
    //                        item.SortProperties_old().WriteTo(writer);
    //                    }
    //                    else if (item.ValueKind == JsonValueKind.Array)
    //                    {
    //                        item.SortArrayProperties_old().WriteTo(writer);
    //                    }
    //                }
    //                writer.WriteEndArray();
    //                writer.Flush();
    //                return JsonDocument.Parse(stream.ToArray()).RootElement;
    //            }
    //        }
    //    }
    //}

    public class Foo<T> where T : unmanaged
    {
        public Foo()
        {

        }

        public override string ToString()
        {
            return typeof(T).Name;
        }
    }

    public class BadNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name)
        {
            return null;
        }
    }

    public static partial class ReadValueTests
    {
        public class Foo
        {
            public object MyObj { get; set; }
            public Bar MyPoco { get; set; }
            public List<Foo> MyList { get; set; }
        }

        public class Bar
        {

        }

        public class MyEquality : IEqualityComparer
        {
            public new bool Equals(object x, object y)
            {
                return false;
            }

            public int GetHashCode(object obj)
            {
                return 0;
            }
        }

        //[JsonConverter(null)]
        public class Baz
        {
            public string Temp { get; set; }
        }

        public class MyConverter : JsonConverter<object>
        {
            public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                throw new NotImplementedException();
            }

            public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
            {
                throw new NotImplementedException();
            }
        }

        public class TempType : Type, IEnumerable
        {
            public override Assembly Assembly => throw new NotImplementedException();

            public override string AssemblyQualifiedName => throw new NotImplementedException();

            public override Type BaseType => throw new NotImplementedException();

            public override string FullName => null;

            public override Guid GUID => throw new NotImplementedException();

            public override Module Module => throw new NotImplementedException();

            public override string Namespace => throw new NotImplementedException();

            public override Type UnderlyingSystemType => throw new NotImplementedException();

            public override string Name => throw new NotImplementedException();

            public override ConstructorInfo[] GetConstructors(BindingFlags bindingAttr)
            {
                throw new NotImplementedException();
            }

            public override object[] GetCustomAttributes(bool inherit)
            {
                throw new NotImplementedException();
            }

            public override object[] GetCustomAttributes(Type attributeType, bool inherit)
            {
                throw new NotImplementedException();
            }

            public override Type GetElementType()
            {
                throw new NotImplementedException();
            }

            public IEnumerator GetEnumerator()
            {
                throw new NotImplementedException();
            }

            public override EventInfo GetEvent(string name, BindingFlags bindingAttr)
            {
                throw new NotImplementedException();
            }

            public override EventInfo[] GetEvents(BindingFlags bindingAttr)
            {
                throw new NotImplementedException();
            }

            public override FieldInfo GetField(string name, BindingFlags bindingAttr)
            {
                throw new NotImplementedException();
            }

            public override FieldInfo[] GetFields(BindingFlags bindingAttr)
            {
                throw new NotImplementedException();
            }

            public override Type GetInterface(string name, bool ignoreCase)
            {
                throw new NotImplementedException();
            }

            public override Type[] GetInterfaces()
            {
                throw new NotImplementedException();
            }

            public override MemberInfo[] GetMembers(BindingFlags bindingAttr)
            {
                throw new NotImplementedException();
            }

            public override MethodInfo[] GetMethods(BindingFlags bindingAttr)
            {
                throw new NotImplementedException();
            }

            public override Type GetNestedType(string name, BindingFlags bindingAttr)
            {
                throw new NotImplementedException();
            }

            public override Type[] GetNestedTypes(BindingFlags bindingAttr)
            {
                throw new NotImplementedException();
            }

            public override PropertyInfo[] GetProperties(BindingFlags bindingAttr)
            {
                throw new NotImplementedException();
            }

            public override object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters)
            {
                throw new NotImplementedException();
            }

            public override bool IsDefined(Type attributeType, bool inherit)
            {
                throw new NotImplementedException();
            }

            protected override TypeAttributes GetAttributeFlagsImpl()
            {
                throw new NotImplementedException();
            }

            protected override ConstructorInfo GetConstructorImpl(BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
            {
                throw new NotImplementedException();
            }

            protected override MethodInfo GetMethodImpl(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
            {
                throw new NotImplementedException();
            }

            protected override PropertyInfo GetPropertyImpl(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers)
            {
                throw new NotImplementedException();
            }

            protected override bool HasElementTypeImpl()
            {
                throw new NotImplementedException();
            }

            protected override bool IsArrayImpl()
            {
                throw new NotImplementedException();
            }

            protected override bool IsByRefImpl()
            {
                throw new NotImplementedException();
            }

            protected override bool IsCOMObjectImpl()
            {
                throw new NotImplementedException();
            }

            protected override bool IsPointerImpl()
            {
                throw new NotImplementedException();
            }

            protected override bool IsPrimitiveImpl()
            {
                throw new NotImplementedException();
            }
        }

        public class MyClass
        {
            public MyInterface stuff { get; set; }
        }

        class A
        {
            public string Name { get; set; }
        }

        [JsonConverter(typeof(MyUserConverter))]
        public class User
        {
            public string UserName { get; private set; }
            public bool Enabled { get; private set; }

            public User()
            {
            }

            [JsonConstructor]
            public User(string userName, bool enabled)
            {
                UserName = userName;
                Enabled = enabled;
            }
        }

        public class MyUserConverter : JsonConverter<User>
        {
            public override User Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                string username = default;
                bool enabled = default;

                reader.Read();
                if (reader.ValueTextEquals("UserName"))
                {
                    reader.Read();
                    username = reader.GetString();
                }

                reader.Read();
                if (reader.ValueTextEquals("Enabled"))
                {
                    reader.Read();
                    enabled = reader.GetBoolean();
                }

                reader.Read();

                return new User(username, enabled);
            }

            public override void Write(Utf8JsonWriter writer, User value, JsonSerializerOptions options)
            {
                writer.WriteStartObject();
                writer.WriteString(nameof(value.UserName), value.UserName);
                writer.WriteBoolean(nameof(value.Enabled), value.Enabled);
                writer.WriteEndObject();
            }
        }

        public class MyGeneric<T> where T : struct
        {

        }

        private static void MyMethod<T>(List<T> param)
        {

        }

        public interface MyInterface : IDictionary<string, object>
        {
            void Remove();
        }


        public class ThousandSmallClassArray
        {
            private const int ITEMS = 1;
            public SmallClass[] Arr { get; set; }
            public ThousandSmallClassArray()
            {
                Arr = new List<SmallClass>(Enumerable.Range(0, ITEMS).Select(x => new SmallClass())).ToArray();
            }
        }


        public class SmallClass
        {
            public string Name { get; set; }
            public int NumPurchases { get; set; }
            public bool IsVIP { get; set; }
            public SmallClass()
            {
                Name = "Bill Gates";
                NumPurchases = 1200;
                IsVIP = true;
            }
        }


        private class DBNullConverter : JsonConverter<DBNull>
        {
            public override DBNull Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                Debug.Assert(typeToConvert == typeof(DBNull));

                if (reader.TokenType != JsonTokenType.Null)
                {
                    throw new JsonException();
                }

                return DBNull.Value;
            }

            public override void Write(Utf8JsonWriter writer, DBNull value, JsonSerializerOptions options)
            {
                writer.WriteNullValue();
            }
        }

        public class SomeClass
        {
            public object MyVal { get; set; }
        }

        public class MyConvert : JsonConverter<List<int>>
        {
            public override List<int> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                throw new NotImplementedException();
            }

            public override void Write(Utf8JsonWriter writer, List<int> value, JsonSerializerOptions options)
            {
                writer.WriteStartArray();
                writer.WriteEndArray();
                //throw new NotImplementedException();
            }
        }


        public class RandomObject
        {
            public string myint { get; set; }
        }

        public class MyStringConverter : JsonConverter<string>
        {
            public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                return null;
            }

            public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
            {
                throw new NotImplementedException();
            }
        }


        public class MyList : IList<object>
        {
            List<object> _list = new List<object>();

            public object this[int index] { get => _list[index]; set => _list[index] = value; }

            public int Count => _list.Count;

            public bool IsReadOnly => throw new NotImplementedException();

            public void Add(object item)
            {
                _list.Add(item);
            }

            public void Clear()
            {
                throw new NotImplementedException();
            }

            public bool Contains(object item)
            {
                throw new NotImplementedException();
            }

            public void CopyTo(object[] array, int arrayIndex)
            {
                throw new NotImplementedException();
            }

            public IEnumerator<object> GetEnumerator()
            {
                throw new NotImplementedException();
            }

            public int IndexOf(object item)
            {
                throw new NotImplementedException();
            }

            public void Insert(int index, object item)
            {
                throw new NotImplementedException();
            }

            public bool Remove(object item)
            {
                throw new NotImplementedException();
            }

            public void RemoveAt(int index)
            {
                throw new NotImplementedException();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return _list.GetEnumerator();
            }
        }

        public class MyConvertList : JsonConverter<MyList>
        {
            public override MyList Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                return null;
            }

            public override void Write(Utf8JsonWriter writer, MyList value, JsonSerializerOptions options)
            {
            }
        }


        public class OverFlowClass
        {
            public int value { get; set; }
            [JsonExtensionData]
            public Dictionary<string, object> overflow { get; set; }
        }


        public class MyFactory : JsonConverterFactory
        {
            public override bool CanConvert(Type typeToConvert)
            {
                return typeToConvert != typeof(int);
            }

            public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
            {
                JsonConverter converter = options.GetConverter(typeof(int));

                return converter;
            }
        }

        public class MyIntConverter : JsonConverter<int>
        {
            public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                return 0;
            }

            public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options)
            {
                return;
            }
        }

        public class GetSet
        {
            public List<string> mystr { internal get; set; } = new List<string>();
        }

        public class MyRandom
        {
            public ImmutableDictionary<string, int> MyImm { get; set; } = ImmutableDictionary.Create<string, int>();
        }

        public class DictionaryWrapper : IDictionary<string, object>
        {
            private readonly IDictionary<string, object> _dict;

            public DictionaryWrapper(IDictionary<string, object> dict)
            {
                _dict = dict;
            }

            public object this[string key] { get => _dict[key]; set => _dict[key] = value; }

            public ICollection<string> Keys => _dict.Keys;

            public ICollection<object> Values => _dict.Values;

            public int Count => _dict.Count;

            public bool IsReadOnly => false;

            public object Current => throw new NotImplementedException();

            public void Add(string key, object value)
            {
                _dict.Add(key, value);
            }

            public void Add(KeyValuePair<string, object> item)
            {
                _dict.Add(item.Key, item.Value);
            }

            public void Clear()
            {
                _dict.Clear();
            }

            public bool Contains(KeyValuePair<string, object> item)
            {
                return _dict.Contains(item);
            }

            public bool ContainsKey(string key)
            {
                return _dict.ContainsKey(key);
            }

            public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
            {
                _dict.CopyTo(array, arrayIndex);
            }

            public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
            {
                yield return new KeyValuePair<string, object>("foo", 1);
                yield return new KeyValuePair<string, object>("foo", 2);
                yield return new KeyValuePair<string, object>("foo", 3);
            }

            public bool MoveNext()
            {
                throw new NotImplementedException();
            }

            public bool Remove(string key)
            {
                return _dict.Remove(key);
            }

            public bool Remove(KeyValuePair<string, object> item)
            {
                return _dict.Remove(item);
            }

            public void Reset()
            {
                throw new NotImplementedException();
            }

            public bool TryGetValue(string key, out object value)
            {
                return _dict.TryGetValue(key, out value);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        public class Baz1
        {
            [JsonPropertyName(null)]
            public Dictionary<string, object>[] myrandom { get; set; }// = new Dictionary<string, object>[1];
        }

        public class MyTempConverter : JsonConverter<int>
        {
            public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                JsonConverter c = options.GetConverter(null);
                Console.WriteLine(c.CanConvert(null));
                throw new NotImplementedException();
            }

            public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options)
            {
                throw new NotImplementedException();
            }
        }

        public struct MyStruct123
        {
        }

        class CustomConverter : JsonConverter<IContent>
        {
            public override IContent Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                var sideReader = reader;

                sideReader.Read();
                sideReader.Read();
                var type = sideReader.GetString();

                return type switch
                {
                    "array" => JsonSerializer.Deserialize<DeepArray>(ref reader, options),
                    _ => throw new JsonException()
                };
            }

            public override void Write(Utf8JsonWriter writer, IContent value, JsonSerializerOptions options)
            {
                throw new NotImplementedException();
            }
        }

        interface IContent { }

        class DeepArray : IContent
        {
            public JsonElement array { get; set; }
        }

        public class Entity
        {
            public int Id { get; set; }
        }

        public class Player : Entity
        {
            public string Name { get; set; }
        }

        public class PlayerConverter : JsonConverter<Player>
        {
            public override Player Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                throw new NotImplementedException();
            }

            public override void Write(Utf8JsonWriter writer, Player value, JsonSerializerOptions options)
            {
                writer.WriteStartObject();
                writer.WriteNumber(nameof(value.Id), value.Id);
                writer.WriteString(nameof(value.Name), value.Name);
                writer.WriteEndObject();
            }
        }

        public class StructFoo
        {
            public Type MyType { get; set; }
        }

        public class SearchResult
        {
            //[JsonPropertyName("id")]
            public string Id { get; set; }

            //[JsonPropertyName("version")]
            public string Version { get; set; }

            //[JsonPropertyName("description")]
            public string Description { get; set; }

            //[JsonPropertyName("versions")]
            public List<SearchResultVersion> Versions { get; set; }

            //[JsonPropertyName("authors")]
            public List<string> Authors { get; set; }

            //[JsonPropertyName("iconUrl")]
            public string IconUrl { get; set; }

            //[JsonPropertyName("licenseUrl")]
            public string LicenseUrl { get; set; }

            //[JsonPropertyName("owners")]
            public List<string> Owners { get; set; }

            //[JsonPropertyName("projectUrl")]
            public string ProjectUrl { get; set; }

            //[JsonPropertyName("registration")]
            public string Registration { get; set; }

            //[JsonPropertyName("summary")]
            public string Summary { get; set; }

            //[JsonPropertyName("tags")]
            public List<string> Tags { get; set; }

            //[JsonPropertyName("title")]
            public string Title { get; set; }

            //[JsonPropertyName("totalDownloads")]
            public int TotalDownloads { get; set; }

            //[JsonPropertyName("verified")]
            public bool Verified { get; set; }
        }

        public class SearchResults
        {
            //[JsonPropertyName("totalHits")]
            public int TotalHits { get; set; }

            //[JsonPropertyName("data")]
            public List<SearchResult> Data { get; set; }
        }

        public class SearchResultVersion
        {
            //[JsonPropertyName("@id")]
            public string Id { get; set; }

            //[JsonPropertyName("version")]
            public string Version { get; set; }

            //[JsonPropertyName("downloads")]
            public int Downloads { get; set; }
        }

        // the view models come from a real world app called "AllReady"
        [Serializable]


        public class LoginViewModel
        {
            public string Email { get; set; }
            public string Password { get; set; }
            public bool RememberMe { get; set; }
        }

        [Serializable]


        public class Location
        {
            public int Id { get; set; }
            public string Address1 { get; set; }
            public string Address2 { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            public string PostalCode { get; set; }
            public string Name { get; set; }
            public string PhoneNumber { get; set; }
            public string Country { get; set; }
        }

        [Serializable]


        public class ActiveOrUpcomingCampaign
        {
            public int Id { get; set; }
            public string ImageUrl { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public DateTimeOffset StartDate { get; set; }
            public DateTimeOffset EndDate { get; set; }
        }

        [Serializable]


        public class ActiveOrUpcomingEvent
        {
            public int Id { get; set; }
            public string ImageUrl { get; set; }
            public string Name { get; set; }
            public string CampaignName { get; set; }
            public string CampaignManagedOrganizerName { get; set; }
            public string Description { get; set; }
            public DateTimeOffset StartDate { get; set; }
            public DateTimeOffset EndDate { get; set; }
        }

        [Serializable]


        public class CampaignSummaryViewModel
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public string ImageUrl { get; set; }
            public string OrganizationName { get; set; }
            public string Headline { get; set; }
        }

        [Serializable]


        public class IndexViewModel
        {
            public List<ActiveOrUpcomingEvent> ActiveOrUpcomingEvents { get; set; }
            public CampaignSummaryViewModel FeaturedCampaign { get; set; }
            public bool IsNewAccount { get; set; }
            public bool HasFeaturedCampaign => FeaturedCampaign != null;
        }

        [Serializable]


        public class MyEventsListerViewModel
        {
            // the orginal type defined these fields as IEnumerable,
            // but XmlSerializer failed to serialize them with "cannot serialize member because it is an interface" error
            public List<MyEventsListerItem> CurrentEvents { get; set; } = new List<MyEventsListerItem>();
            public List<MyEventsListerItem> FutureEvents { get; set; } = new List<MyEventsListerItem>();
            public List<MyEventsListerItem> PastEvents { get; set; } = new List<MyEventsListerItem>();
        }

        [Serializable]


        public class MyEventsListerItem
        {
            public int EventId { get; set; }
            public string EventName { get; set; }
            public DateTimeOffset StartDate { get; set; }
            public DateTimeOffset EndDate { get; set; }
            public string TimeZone { get; set; }
            public string Campaign { get; set; }
            public string Organization { get; set; }
            public int VolunteerCount { get; set; }

            public List<MyEventsListerItemTask> Tasks { get; set; } = new List<MyEventsListerItemTask>();
        }

        [Serializable]


        public class MyEventsListerItemTask
        {
            public string Name { get; set; }
            public DateTimeOffset? StartDate { get; set; }
            public DateTimeOffset? EndDate { get; set; }


            public string FormattedDate
            {
                get
                {
                    if (!StartDate.HasValue || !EndDate.HasValue)
                    {
                        return null;
                    }

                    var startDateString = string.Format("{0:g}", StartDate.Value);
                    var endDateString = string.Format("{0:g}", EndDate.Value);

                    return string.Format($"From {startDateString} to {endDateString}");
                }
            }
        }

        public class GenericUserTestDeserialize<T>
        {
            private string _contents;
            private byte[] _contentsUtf8;
            private Stream _contentStream;

            public GenericUserTestDeserialize(bool UseFileStream)
            {
                string path = @"E:\GitHub\Fork\Benchmarks\TestData\JsonParsingBenchmark\";

                if (typeof(T) == typeof(LoginViewModel))
                    path += nameof(LoginViewModel);
                else if (typeof(T) == typeof(Location))
                    path += nameof(Location);
                else if (typeof(T) == typeof(IndexViewModel))
                    path += nameof(IndexViewModel);
                else if (typeof(T) == typeof(MyEventsListerViewModel))
                    path += nameof(MyEventsListerViewModel);
                else
                    path += nameof(T);

                path += ".json";

                //_contents = File.ReadAllText(@"E:\GitHub\Fork\Benchmarks\TestData\JsonParsingBenchmark\dotnet-core.json");

                _contents = File.ReadAllText(path);
                _contentsUtf8 = Encoding.UTF8.GetBytes(_contents);
                _contentStream = new MemoryStream(_contentsUtf8);
                if (UseFileStream)
                {
                    //_contentStream = new FileStream(@"E:\GitHub\Fork\Benchmarks\TestData\JsonParsingBenchmark\dotnet-core.json", FileMode.Open);
                    _contentStream = new FileStream(path, FileMode.Open);
                }
            }

            public T DeserializeFromStreamSync()
            {
                _contentStream.Position = 0;
                ReadOnlySpan<byte> span;
                if (_contentStream is MemoryStream memoryStream)
                {
                    span = new ReadOnlySpan<byte>(memoryStream.ToArray());
                }
                else
                {
                    byte[] buffer = new byte[16 * 1024];
                    using (MemoryStream ms = new MemoryStream())
                    {
                        int read;
                        while ((read = _contentStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            ms.Write(buffer, 0, read);
                        }

                        span = new ReadOnlySpan<byte>(ms.ToArray());
                    }

                }

                return JsonSerializer.Deserialize<T>(span);
            }

            private static ReadOnlySpan<byte> Utf8Bom => new byte[] { 0xEF, 0xBB, 0xBF };

            private const int UnseekableStreamInitialRentSize = 4096;

            public T DeserializeFromStreamSync_New()
            {
                _contentStream.Position = 0;
                int written = 0;
                byte[] rented = null;

                ReadOnlySpan<byte> utf8Bom = Utf8Bom;

                try
                {
                    if (_contentStream.CanSeek)
                    {
                        // Ask for 1 more than the length to avoid resizing later,
                        // which is unnecessary in the common case where the stream length doesn't change.
                        long expectedLength = Math.Max(utf8Bom.Length, _contentStream.Length - _contentStream.Position) + 1;
                        rented = ArrayPool<byte>.Shared.Rent(checked((int)expectedLength));
                    }
                    else
                    {
                        rented = ArrayPool<byte>.Shared.Rent(UnseekableStreamInitialRentSize);
                    }

                    int lastRead;

                    // Read up to 3 bytes to see if it's the UTF-8 BOM
                    do
                    {
                        // No need for checking for growth, the minimal rent sizes both guarantee it'll fit.
                        Debug.Assert(rented.Length >= utf8Bom.Length);

                        lastRead = _contentStream.Read(
                            rented,
                            written,
                            utf8Bom.Length - written);

                        written += lastRead;
                    } while (lastRead > 0 && written < utf8Bom.Length);

                    // If we have 3 bytes, and they're the BOM, reset the write position to 0.
                    if (written == utf8Bom.Length &&
                        utf8Bom.SequenceEqual(rented.AsSpan(0, utf8Bom.Length)))
                    {
                        written = 0;
                    }

                    do
                    {
                        if (rented.Length == written)
                        {
                            byte[] toReturn = rented;
                            rented = ArrayPool<byte>.Shared.Rent(checked(toReturn.Length * 2));
                            Buffer.BlockCopy(toReturn, 0, rented, 0, toReturn.Length);
                            // Holds document content, clear it.
                            ArrayPool<byte>.Shared.Return(toReturn, clearArray: true);
                        }

                        lastRead = _contentStream.Read(rented, written, rented.Length - written);
                        written += lastRead;
                    } while (lastRead > 0);

                    return JsonSerializer.Deserialize<T>(rented.AsSpan(0, written));
                }
                finally
                {
                    if (rented != null)
                    {
                        // Holds document content, clear it before returning it.
                        rented.AsSpan(0, written).Clear();
                        ArrayPool<byte>.Shared.Return(rented);
                    }
                }
            }

            public async Task<T> DeserializeFromStreamAsync()
            {
                _contentStream.Position = 0;
                T value = await JsonSerializer.DeserializeAsync<T>(_contentStream);
                return value;
            }
        }

        private static Stream UserMethod<T>(T input)
        {
            MemoryStream streamPayload = new MemoryStream();
            using Utf8JsonWriter utf8JsonWriter = new Utf8JsonWriter(streamPayload);
            JsonSerializer.Serialize<T>(utf8JsonWriter, input);
            streamPayload.Position = 0;
            return streamPayload;
        }

        internal sealed class CosmosTextJsonSerializer
        {
            // Before
            public Stream UserMethod<T>(T input)
            {
                var streamPayload = new MemoryStream();
                var writer = new Utf8JsonWriter(streamPayload);
                JsonSerializer.Serialize(writer, input);
                streamPayload.Position = 0;
                return streamPayload;
            }

            [ThreadStatic]
            private Utf8JsonWriter _writer;

            private readonly ConcurrentStack<Utf8JsonWriter> _writers = new ConcurrentStack<Utf8JsonWriter>();

            // After
            public Stream UserMethod_ThreadStatic_Cached<T>(T input)
            {
                var streamPayload = new MemoryStream();

                if (_writer == null)
                {
                    _writer = new Utf8JsonWriter(streamPayload);
                }
                else
                {
                    _writer.Reset(streamPayload);
                }

                JsonSerializer.Serialize(_writer, input);
                streamPayload.Position = 0;
                return streamPayload;
            }

            public Stream UserMethod_Cached<T>(T input)
            {
                var streamPayload = new MemoryStream();

                if (!_writers.TryPop(out Utf8JsonWriter writer))
                {
                    writer = new Utf8JsonWriter(streamPayload);
                }
                else
                {
                    writer.Reset(streamPayload);
                }

                JsonSerializer.Serialize(writer, input);
                _writers.Push(writer);
                streamPayload.Position = 0;
                return streamPayload;
            }
        }

        private static Stream UserMethod_Cached<T>(T input)
        {
            var streamPayload = new MemoryStream();
            var writer = new Utf8JsonWriter(streamPayload);
            //_writer.Reset(streamPayload);
            JsonSerializer.Serialize(writer, input);
            streamPayload.Position = 0;
            return streamPayload;
        }

        private static Stream UserMethod_Formatted_Cached<T>(T input)
        {
            var streamPayload = new MemoryStream();
            var writerFormatted = new Utf8JsonWriter(streamPayload, new JsonWriterOptions { Indented = true });
            //_writerFormatted.Reset(streamPayload);
            JsonSerializer.Serialize(writerFormatted, input);
            streamPayload.Position = 0;
            return streamPayload;
        }

        //[Fact]
        private static void MultipleThreadsLooping()
        {
            const int Iterations = 100;

            for (int i = 0; i < Iterations; i++)
            {
                MultipleThreads();
            }
        }

        private static string _expected;
        private static string _expectedFormatted;

        private static Utf8JsonWriter _writer;
        private static Utf8JsonWriter _writerFormatted;

        //[Fact]
        private static void MultipleThreads()
        {
            {
                var obj = new SimpleTestClass();
                obj.Initialize();
                _expected = JsonSerializer.Serialize(obj);
                _writer = new Utf8JsonWriter(new MemoryStream(), new JsonWriterOptions { Indented = false });
            }

            {
                var obj = new SimpleTestClass();
                obj.Initialize();
                _expectedFormatted = JsonSerializer.Serialize(obj, new JsonSerializerOptions { WriteIndented = true });
                _writerFormatted = new Utf8JsonWriter(new MemoryStream(), new JsonWriterOptions { Indented = true });
            }

            // Use local options to avoid obtaining already cached metadata from the default options.
            var options = new JsonSerializerOptions();

            // Verify the test class has >64 properties since that is a threshold for using the fallback dictionary.
            Assert.True(typeof(SimpleTestClass).GetProperties(BindingFlags.Instance | BindingFlags.Public).Length > 64);

            void DeserializeObjectMinimal()
            {
                SimpleTestClass obj = JsonSerializer.Deserialize<SimpleTestClass>(@"{""MyDecimal"" : 3.3}", options);
            };

            void DeserializeObjectFlipped()
            {
                SimpleTestClass obj = JsonSerializer.Deserialize<SimpleTestClass>(SimpleTestClass.s_json_flipped, options);
                obj.Verify();
            };

            void DeserializeObjectNormal()
            {
                SimpleTestClass obj = JsonSerializer.Deserialize<SimpleTestClass>(SimpleTestClass.s_json, options);
                obj.Verify();
            };

            void SerializeObject()
            {
                var obj = new SimpleTestClass();
                obj.Initialize();
                var temp = new CosmosTextJsonSerializer();
                Stream str = temp.UserMethod_Cached<SimpleTestClass>(obj);
                Assert.Equal(_expected, Encoding.UTF8.GetString(((MemoryStream)str).ToArray()));
                Console.WriteLine("A: " + Encoding.UTF8.GetString(((MemoryStream)str).ToArray()));
            };

            void SerializeObject2()
            {
                var obj = new SimpleTestClass();
                obj.Initialize();
                var temp = new CosmosTextJsonSerializer();
                Stream str = temp.UserMethod_ThreadStatic_Cached<SimpleTestClass>(obj);
                Assert.Equal(_expected, Encoding.UTF8.GetString(((MemoryStream)str).ToArray()));
                Console.WriteLine("B: " + Encoding.UTF8.GetString(((MemoryStream)str).ToArray()));
            };

            const int ThreadCount = 8;
            const int ConcurrentTestsCount = 6;
            Task[] tasks = new Task[ThreadCount * ConcurrentTestsCount];

            for (int i = 0; i < tasks.Length; i += ConcurrentTestsCount)
            {
                // Create race condition to populate the sorted property cache with different json ordering.
                tasks[i + 0] = Task.Run(() => DeserializeObjectMinimal());
                tasks[i + 1] = Task.Run(() => SerializeObject());
                tasks[i + 2] = Task.Run(() => SerializeObject2());
                tasks[i + 3] = Task.Run(() => DeserializeObjectFlipped());
                tasks[i + 4] = Task.Run(() => DeserializeObjectNormal());

                // Ensure no exceptions on serialization
                tasks[i + 5] = Task.Run(() => SerializeObject());
            };

            Task.WaitAll(tasks);
        }

        public struct ImmutablePoint
        {
            public ImmutablePoint(int x, int y)
            {
                X = x;
                Y = y;
            }

            public int X { get; }
            public int Y { get; }
        }

        public class ImmutablePointConverter : JsonConverter<ImmutablePoint>

        {
            private const string XName = "X";
            private const string YName = "Y";

            public override ImmutablePoint Read(
                ref Utf8JsonReader reader,
                Type typeToConvert,
                JsonSerializerOptions options)
            {
                if (reader.TokenType != JsonTokenType.StartObject)
                {
                    throw new JsonException();
                };

                int x = default;
                bool xSet = false;

                int y = default;
                bool ySet = false;

                // Get the first property.
                reader.Read();
                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    throw new JsonException();
                }

                string propertyName = reader.GetString();
                if (propertyName == XName)
                {
                    x = ReadProperty(ref reader, options);
                    xSet = true;
                }
                else if (propertyName == YName)
                {
                    y = ReadProperty(ref reader, options);
                    ySet = true;
                }
                else
                {
                    throw new JsonException();
                }

                // Get the second property.
                reader.Read();
                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    throw new JsonException();
                }

                propertyName = reader.GetString();
                if (propertyName == XName)
                {
                    x = ReadProperty(ref reader, options);
                    xSet = true;
                }
                else if (propertyName == YName)
                {
                    y = ReadProperty(ref reader, options);
                    ySet = true;
                }
                else
                {
                    throw new JsonException();
                }

                if (!xSet || !ySet)
                {
                    throw new JsonException();
                }

                reader.Read();

                if (reader.TokenType != JsonTokenType.EndObject)
                {
                    throw new JsonException();
                }

                return new ImmutablePoint(x, y);
            }

            public int ReadProperty(ref Utf8JsonReader reader, JsonSerializerOptions options)
            {
                if (options?.GetConverter(typeof(int)) is JsonConverter<int> intConverter)
                {
                    reader.Read();
                    return intConverter.Read(ref reader, typeof(int), options);
                }
                else
                {
                    throw new JsonException();
                }
            }

            public override void Write(
                Utf8JsonWriter writer,
                ImmutablePoint value,
                JsonSerializerOptions options)
            {
                // Don't pass in options when recursively calling Serialize.
                JsonSerializer.Serialize(writer, value, options);
            }
        }

        public class LongToStringConverter : JsonConverter<long>
        {
            public override long Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.String)
                {
                    ReadOnlySpan<byte> span = reader.HasValueSequence ? reader.ValueSequence.ToArray() : reader.ValueSpan;
                    if (Utf8Parser.TryParse(span, out long number, out int bytesConsumed) && span.Length == bytesConsumed)
                        return number;

                    if (long.TryParse(reader.GetString(), out number))
                        return number;
                }

                return reader.GetInt64();
            }

            public override void Write(Utf8JsonWriter writer, long value, JsonSerializerOptions options)
            {
                writer.WriteStringValue(value.ToString());
            }
        }

        //[Newtonsoft.Json.JsonConverter(typeof(MyBazTextConverterA))]
        [JsonConverter(typeof(MyBazTextConverter1))]
        public class BazText
        {
            public int MyValue { get; set; }
        }

        public class MyBazTextConverterA : Newtonsoft.Json.JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return true;
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }

            public override void WriteJson(JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }
        }

        public class MyBazTextConverterB : Newtonsoft.Json.JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return true;
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
            {
                return null;
            }

            public override void WriteJson(JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }
        }

        public class MyBazTextConverter1 : JsonConverter<BazText>
        {
            public override BazText Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                throw new NotImplementedException();
            }

            public override void Write(Utf8JsonWriter writer, BazText value, JsonSerializerOptions options)
            {
                writer.WriteStartObject();
            }
        }

        public class MyBazTextConverter2 : JsonConverter<BazText>
        {
            public override BazText Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                reader.Read();
                reader.Read();
                return null;
            }

            public override void Write(Utf8JsonWriter writer, BazText value, JsonSerializerOptions options)
            {
                writer.WriteNullValue();
            }
        }

        public struct MyTempStruct
        {
            public DateTime? Foo
            {
                get
                {
                    return null;
                }
                set
                {

                }
            }
        }

        public class MyTempClass
        {
            public DateTime? Foo
            {
                get
                {
                    return null;
                }
                set
                {

                }
            }

            public DateTimeOffset? Bar { get; set; } = null;
        }

        public class DateTimeOffsetNullHandlingConverter : JsonConverter<DateTimeOffset>

        {
            public override DateTimeOffset Read(
                ref Utf8JsonReader reader,
                Type typeToConvert,
                JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.Null)
                {
                    return default;
                }
                return reader.GetDateTimeOffset();
            }

            public override void Write(
                Utf8JsonWriter writer,
                DateTimeOffset value,
                JsonSerializerOptions options)
            {
                writer.WriteStringValue(value);
            }
        }

        public class TestingClass
        {
            public string myString { get; set; }
        }

        public interface IUser
        {
            string FirstName { get; set; }
            string LastName { get; set; }
            string Email { get; set; }
            string Mobile { get; set; }
            string Id { get; set; }
            string Role { get; set; }
        }

        public class User1 : IUser
        {
            [JsonPropertyName("first_name")]
            public string FirstName { get; set; }

            [JsonPropertyName("last_name")]
            public string LastName { get; set; }

            [JsonPropertyName("email")]
            public string Email { get; set; }

            [JsonPropertyName("mobile")]
            public string Mobile { get; set; }

            [JsonPropertyName("id")]
            public string Id { get; set; }

            [JsonPropertyName("role")]
            public string Role { get; set; }

            [JsonIgnore]
            public string Token { get; set; }
        }

        public static string Merge(string originalJson, string newContent)
        {
            var outputBuffer = new ArrayBufferWriter<byte>();

            using (JsonDocument jDoc1 = JsonDocument.Parse(originalJson))
            using (JsonDocument jDoc2 = JsonDocument.Parse(newContent))
            using (var jsonWriter = new Utf8JsonWriter(outputBuffer, new JsonWriterOptions { Indented = true }))
            {
                JsonElement root1 = jDoc1.RootElement;
                JsonElement root2 = jDoc2.RootElement;

                if (root1.ValueKind != JsonValueKind.Array && root1.ValueKind != JsonValueKind.Object)
                {
                    throw new InvalidOperationException($"The original JSON document to merge new content into must be a container type. Instead it is {root1.ValueKind}.");
                }

                if (root1.ValueKind != root2.ValueKind)
                {
                    return originalJson;
                }

                if (root1.ValueKind == JsonValueKind.Array)
                {
                    MergeArrays(jsonWriter, root1, root2);
                }
                else
                {
                    MergeObjects(jsonWriter, root1, root2);
                }
            }

            return Encoding.UTF8.GetString(outputBuffer.WrittenSpan);
        }

        private static void MergeObjects(Utf8JsonWriter jsonWriter, JsonElement root1, JsonElement root2)
        {
            Debug.Assert(root1.ValueKind == JsonValueKind.Object);
            Debug.Assert(root2.ValueKind == JsonValueKind.Object);

            jsonWriter.WriteStartObject();

            // Write all the properties of the first document.
            // If a property exists in both documents, either:
            // * Merge them, if the value kinds match (e.g. both are objects or arrays),
            // * Completely override the value of the first with the one from the second, if the value kind mismatches (e.g. one is object, while the other is an array or string),
            // * Or favor the value of the first (regardless of what it may be), if the second one is null (i.e. don't override the first).
            foreach (JsonProperty property in root1.EnumerateObject())
            {
                string propertyName = property.Name;

                JsonValueKind newValueKind;

                if (root2.TryGetProperty(propertyName, out JsonElement newValue) && (newValueKind = newValue.ValueKind) != JsonValueKind.Null)
                {
                    jsonWriter.WritePropertyName(propertyName);

                    JsonElement originalValue = property.Value;
                    JsonValueKind originalValueKind = originalValue.ValueKind;

                    if (newValueKind == JsonValueKind.Object && originalValueKind == JsonValueKind.Object)
                    {
                        MergeObjects(jsonWriter, originalValue, newValue); // Recursive call
                    }
                    else if (newValueKind == JsonValueKind.Array && originalValueKind == JsonValueKind.Array)
                    {
                        MergeArrays(jsonWriter, originalValue, newValue);
                    }
                    else
                    {
                        newValue.WriteTo(jsonWriter);
                    }
                }
                else
                {
                    property.WriteTo(jsonWriter);
                }
            }

            // Write all the properties of the second document that are unique to it.
            foreach (JsonProperty property in root2.EnumerateObject())
            {
                if (!root1.TryGetProperty(property.Name, out _))
                {
                    property.WriteTo(jsonWriter);
                }
            }

            jsonWriter.WriteEndObject();
        }

        private static void MergeArrays(Utf8JsonWriter jsonWriter, JsonElement root1, JsonElement root2)
        {
            Debug.Assert(root1.ValueKind == JsonValueKind.Array);
            Debug.Assert(root2.ValueKind == JsonValueKind.Array);

            jsonWriter.WriteStartArray();

            // Write all the elements from both JSON arrays
            foreach (JsonElement element in root1.EnumerateArray())
            {
                element.WriteTo(jsonWriter);
            }
            foreach (JsonElement element in root2.EnumerateArray())
            {
                element.WriteTo(jsonWriter);
            }

            jsonWriter.WriteEndArray();
        }

        [Fact]
        public static void JsonDocumentMergeTest_ComparedToJContainerMerge()
        {
            string jsonString1 = @"{
                ""throw"": null,
                ""duplicate"": null,
                ""id"": 1,
                ""xyz"": null,
                ""nullOverride2"": false,
                ""nullOverride1"": null,
                ""william"": ""shakespeare"",
                ""complex"": {""overwrite"": ""no"", ""type"": ""string"", ""original"": null, ""another"":[]},
                ""nested"": [7, {""another"": true}],
                ""nestedObject"": {""another"": true}
            }";

            string jsonString2 = @"{
                ""william"": ""dafoe"",
                ""duplicate"": null,
                ""foo"": ""bar"",
                ""baz"": {""temp"": 4},
                ""xyz"": [1, 2, 3],
                ""nullOverride1"": true,
                ""nullOverride2"": null,
                ""nested"": [1, 2, 3, null, {""another"": false}],
                ""nestedObject"": [""wow""],
                ""complex"": {""temp"": true, ""overwrite"": ""ok"", ""type"": 14},
                ""temp"": null
            }";

            JObject jObj1 = JObject.Parse(jsonString1);
            JObject jObj2 = JObject.Parse(jsonString2);

            jObj1.Merge(jObj2);
            jObj2.Merge(JObject.Parse(jsonString1));

            Assert.Equal(jObj1.ToString(), Merge(jsonString1, jsonString2));
            Assert.Equal(jObj2.ToString(), Merge(jsonString2, jsonString1));
        }

        public class Base
        {
            public string Name { get; set; }
            public int Id { get; set; }
        }

        public class Derived : Base
        {
            public string Value { get; set; }
        }

        public class PolymorphicDeep
        {
            public object BaseArray { get; set; }
        }

        public class MonthlyForecast
        {
            public string Month { get; set; }
            public List<WeatherForecast> Forecasts { get; set; }  // Change to List<object> for polymorphic behavior
        }

        public class WeatherForecast
        {
            public DateTimeOffset Date { get; set; }
            public int TemperatureCelsius { get; set; }
            public string Summary { get; set; }
        }

        public class WeatherForecastDerived : WeatherForecast
        {
            public int WindSpeed { get; set; }
        }

        public interface IMyInterface
        {
            public int Count { get; set; }
        }

        public class MyImplementation : IMyInterface
        {
            public int Count { get; set; }
            public string Name { get; set; }
        }

        public class PolymorphicTestInterface
        {
            public IMyInterface MyObject { get; set; } // Change this to object to get the desired behavior
        }

        public class MyList1 : List<IMyList>, IMyList
        {

        }

        public interface IMyList : IList
        {

        }

        public class Utf8ReaderFromFile
        {
            private static readonly byte[] s_nameUtf8 = Encoding.UTF8.GetBytes("name");
            private static ReadOnlySpan<byte> Utf8Bom => new byte[] { 0xEF, 0xBB, 0xBF };

            public static void Run()
            {
                // Read as UTF-16 and transcode to UTF-8 to convert to a ReadOnlySpan<byte>
                //string fileName = @"C:\Users\ahkha\Desktop\jsonWithUtf8BOM.json";
                //string jsonString = File.ReadAllText(fileName);
                //ReadOnlySpan<byte> jsonReadOnlySpan = Encoding.UTF8.GetBytes(jsonString);

                // Or ReadAllBytes if the file encoding is UTF-8:
                string fileName = @"C:\Users\ahkha\Desktop\jsonWithUtf8BOM.json";
                ReadOnlySpan<byte> jsonReadOnlySpan = File.ReadAllBytes(fileName);
                if (jsonReadOnlySpan.StartsWith(Utf8Bom))
                {
                    jsonReadOnlySpan = jsonReadOnlySpan.Slice(Utf8Bom.Length);
                }

                int count = 0;
                int total = 0;

                var reader = new Utf8JsonReader(jsonReadOnlySpan);

                while (reader.Read())
                {
                    JsonTokenType tokenType = reader.TokenType;

                    switch (tokenType)
                    {
                        case JsonTokenType.StartObject:
                            total++;
                            break;
                        case JsonTokenType.PropertyName:
                            if (reader.ValueTextEquals(s_nameUtf8))
                            {
                                // Assume valid JSON, known schema
                                reader.Read();
                                if (reader.GetString().EndsWith("University"))
                                {
                                    count++;
                                }
                            }
                            break;
                    }
                }
                Console.WriteLine($"{count} out of {total} have names that end with 'University'");
            }
        }

        public class Tours
        {
            [JsonPropertyName("types")]
            public List<UserType> Types { get; set; }
        }

        [JsonConverter(typeof(CustomUserTypeConverter))]
        public class UserType
        {
            public string Key { get; set; }
            public Dictionary<string, int> Values { get; set; }
        }

        public class CustomUserTypeConverter : JsonConverter<UserType>
        {
            public override UserType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                var result = new UserType();

                if (!reader.Read())
                {
                    throw new JsonException($"Incomplete JSON.");
                }

                if (reader.TokenType != JsonTokenType.EndArray)
                {
                    result.Key = reader.GetString();

                    ReadAndValidate(ref reader, JsonTokenType.StartArray);

                    int depthSnapshot = reader.CurrentDepth;

                    var values = new Dictionary<string, int>();

                    do
                    {
                        reader.Read();
                        if (reader.TokenType != JsonTokenType.StartArray && reader.TokenType != JsonTokenType.EndArray)
                        {
                            throw new JsonException($"Invalid JSON payload. Expected Start or End Array. TokenType: {reader.TokenType}, Depth: {reader.CurrentDepth}.");
                        }

                        if (reader.CurrentDepth <= depthSnapshot)
                        {
                            break;
                        }

                        reader.Read();

                        if (reader.TokenType != JsonTokenType.EndArray)
                        {
                            string key = reader.GetString();

                            reader.Read();
                            int value = reader.GetInt32();
                            values.Add(key, value);

                            ReadAndValidate(ref reader, JsonTokenType.EndArray);
                        }

                    } while (true);

                    ReadAndValidate(ref reader, JsonTokenType.EndArray);

                    result.Values = values;
                }

                return result;
            }

            private void ReadAndValidate(ref Utf8JsonReader reader, JsonTokenType expectedTokenType)
            {
                bool readNext = reader.Read();
                if (!readNext || reader.TokenType != expectedTokenType)
                {
                    string message = readNext ?
                        $"Invalid JSON payload. TokenType: {reader.TokenType}, Depth: {reader.CurrentDepth}, Expected: {expectedTokenType}" :
                        $"Incomplete JSON. Expected: {expectedTokenType}";
                    throw new JsonException(message);
                }
            }

            public override void Write(Utf8JsonWriter writer, UserType value, JsonSerializerOptions options)
            {
                throw new NotImplementedException();
            }
        }

        private static Tours ParseJson(string json)
        {
            Tours tours = JsonSerializer.Deserialize<Tours>(json);
            return tours;
        }

        public class Rootobject
        {
            public object[][] types { get; set; }
        }

        private static void AccessValues(Tours tours)
        {
            foreach (UserType data in tours.Types)
            {
                string typeName = data.Key; // "tour_type"

                foreach (KeyValuePair<string, int> pairs in data.Values)
                {
                    string key = pairs.Key; // "groups" or "individual
                    int value = pairs.Value; // 1 or 2
                }
            }
        }

        public class ValueTupleFactory : JsonConverterFactory
        {
            public override bool CanConvert(Type typeToConvert)
            {
                Type iTuple = typeToConvert.GetInterface("System.Runtime.CompilerServices.ITuple");
                return iTuple != null;
            }

            public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
            {
                Type[] genericArguments = typeToConvert.GetGenericArguments();

                Type converterType = genericArguments.Length switch
                {
                    1 => typeof(ValueTupleConverter<>).MakeGenericType(genericArguments),
                    2 => typeof(ValueTupleConverter<,>).MakeGenericType(genericArguments),
                    3 => typeof(ValueTupleConverter<,,>).MakeGenericType(genericArguments),
                    // And add other cases as needed
                    _ => throw new NotSupportedException(),
                };
                return (JsonConverter)Activator.CreateInstance(converterType);
            }
        }

        public class ValueTupleConverter<T1> : JsonConverter<ValueTuple<T1>>
        {
            public override ValueTuple<T1> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                ValueTuple<T1> result = default;

                if (!reader.Read())
                {
                    throw new JsonException();
                }

                while (reader.TokenType != JsonTokenType.EndObject)
                {
                    if (reader.ValueTextEquals("Item1") && reader.Read())
                    {
                        result.Item1 = JsonSerializer.Deserialize<T1>(ref reader, options);
                    }
                    else
                    {
                        throw new JsonException();
                    }
                    reader.Read();
                }

                return result;
            }

            public override void Write(Utf8JsonWriter writer, ValueTuple<T1> value, JsonSerializerOptions options)
            {
                writer.WriteStartObject();
                writer.WritePropertyName("Item1");
                JsonSerializer.Serialize<T1>(writer, value.Item1, options);
                writer.WriteEndObject();
            }
        }

        public class ValueTupleConverter<T1, T2> : JsonConverter<ValueTuple<T1, T2>>
        {
            public override (T1, T2) Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                (T1, T2) result = default;

                if (!reader.Read())
                {
                    throw new JsonException();
                }

                while (reader.TokenType != JsonTokenType.EndObject)
                {
                    if (reader.ValueTextEquals("Item1") && reader.Read())
                    {
                        result.Item1 = JsonSerializer.Deserialize<T1>(ref reader, options);
                    }
                    else if (reader.ValueTextEquals("Item2") && reader.Read())
                    {
                        result.Item2 = JsonSerializer.Deserialize<T2>(ref reader, options);
                    }
                    else
                    {
                        throw new JsonException();
                    }
                    reader.Read();
                }

                return result;
            }

            public override void Write(Utf8JsonWriter writer, (T1, T2) value, JsonSerializerOptions options)
            {
                writer.WriteStartObject();
                writer.WritePropertyName("Item1");
                JsonSerializer.Serialize<T1>(writer, value.Item1, options);
                writer.WritePropertyName("Item2");
                JsonSerializer.Serialize<T2>(writer, value.Item2, options);
                writer.WriteEndObject();
            }
        }

        public class ValueTupleConverter<T1, T2, T3> : JsonConverter<ValueTuple<T1, T2, T3>>
        {
            public override (T1, T2, T3) Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                (T1, T2, T3) result = default;

                if (!reader.Read())
                {
                    throw new JsonException();
                }

                while (reader.TokenType != JsonTokenType.EndObject)
                {
                    if (reader.ValueTextEquals("Item1") && reader.Read())
                    {
                        result.Item1 = JsonSerializer.Deserialize<T1>(ref reader, options);
                    }
                    else if (reader.ValueTextEquals("Item2") && reader.Read())
                    {
                        result.Item2 = JsonSerializer.Deserialize<T2>(ref reader, options);
                    }
                    else if (reader.ValueTextEquals("Item3") && reader.Read())
                    {
                        result.Item3 = JsonSerializer.Deserialize<T3>(ref reader, options);
                    }
                    else
                    {
                        throw new JsonException();
                    }
                    reader.Read();
                }

                return result;
            }

            public override void Write(Utf8JsonWriter writer, (T1, T2, T3) value, JsonSerializerOptions options)
            {
                writer.WriteStartObject();
                writer.WritePropertyName("Item1");
                JsonSerializer.Serialize<T1>(writer, value.Item1, options);
                writer.WritePropertyName("Item2");
                JsonSerializer.Serialize<T2>(writer, value.Item2, options);
                writer.WritePropertyName("Item3");
                JsonSerializer.Serialize<T3>(writer, value.Item3, options);
                writer.WriteEndObject();
            }
        }

        public class WeatherForecastValueTuple
        {
            public ValueTuple<string> Temp { get; set; }
            public (int x, int y) Location { get; set; }
            public (int a, Poco b, ValueTuple<string> c) Thruple { get; set; }
        }

        public class Poco
        {
            public int Alpha { get; set; }
            public (List<int>, string) Beta { get; set; }
        }

        private static void SupportValueTuple()
        {
            var objectWithValueTuple = new WeatherForecastValueTuple
            {
                Location = (3, 7),
                Temp = new ValueTuple<string>("FOO"),
                Thruple = (
                    8,
                    new Poco
                    {
                        Alpha = 9,
                        Beta = (new List<int>() { -1, 0, -1 }, "a list")
                    },
                    new ValueTuple<string>("Bar"))
            };

            var options = new JsonSerializerOptions();
            options.Converters.Add(new ValueTupleFactory());

            string jsonString = JsonSerializer.Serialize(objectWithValueTuple, options);
            Console.WriteLine(jsonString);

            WeatherForecastValueTuple roundTrip = JsonSerializer.Deserialize<WeatherForecastValueTuple>(jsonString, options);
            Assert.Equal((3, 7), roundTrip.Location);
            Assert.Equal(new ValueTuple<string>("FOO"), roundTrip.Temp);
        }

        public class RequestPayload
        {
            public string MessageName { get; set; }

            public object Payload { get; set; }

            [JsonIgnore]
            public Type PayloadType { get; set; }
        }

        public class ExpectedType1 { }
        public class ExpectedType2 { }
        public class ExpectedType3 { }

        public class CustomJsonConverterForType : JsonConverter<Type>
        {
            public override Type Read(ref Utf8JsonReader reader, Type typeToConvert,
                JsonSerializerOptions options)
            {
                TypeDiscriminator typeDiscriminator = (TypeDiscriminator)reader.GetInt32();

                Type type = typeDiscriminator switch
                {
                    TypeDiscriminator.ExpectedType1 => typeof(ExpectedType1),
                    TypeDiscriminator.ExpectedType2 => typeof(ExpectedType2),
                    TypeDiscriminator.ExpectedType3 => typeof(ExpectedType3),
                    _ => throw new NotSupportedException(),
                };
                return type;
            }

            public override void Write(Utf8JsonWriter writer, Type value,
                JsonSerializerOptions options)
            {
                if (value == typeof(ExpectedType1))
                {
                    writer.WriteNumberValue((int)TypeDiscriminator.ExpectedType1);
                }
                else if (value == typeof(ExpectedType2))
                {
                    writer.WriteNumberValue((int)TypeDiscriminator.ExpectedType2);
                }
                else if (value == typeof(ExpectedType3))
                {
                    writer.WriteNumberValue((int)TypeDiscriminator.ExpectedType3);
                }
                else
                {
                    throw new NotSupportedException();
                }
            }

            private enum TypeDiscriminator
            {
                ExpectedType1 = 1,
                ExpectedType2 = 2,
                ExpectedType3 = 3,
            }
        }

        private static void ObjectPropertyExample()
        {
            using JsonDocument doc = JsonDocument.Parse("{\"Name\":\"Darshana\"}");
            JsonElement payload = doc.RootElement.Clone();

            var requestPayload = new RequestPayload
            {
                MessageName = "message",
                Payload = payload
            };

            string json = JsonSerializer.Serialize(requestPayload);
            Console.WriteLine(json);
            // {"MessageName":"message","Payload":{"Name":"Darshana"}}

            RequestPayload roundtrip = JsonSerializer.Deserialize<RequestPayload>(json);

            JsonElement element = (JsonElement)roundtrip.Payload;
            string name = element.GetProperty("Name").GetString();
            Assert.Equal("Darshana", name);
        }

        public class StackConverterFactory : JsonConverterFactory
        {
            public override bool CanConvert(Type typeToConvert)
            {
                throw new NotImplementedException();
            }

            public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
            {
                throw new NotImplementedException();
            }
        }

        public class NuGetAuthorsConverter : JsonConverter<object>
        {
            public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                return null;
            }

            public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
            {
                throw new NotImplementedException();
            }
        }

        public class NuGetTest
        {
            public List<string> authors { get; set; }
        }

        public class JsonConverterFactoryForStackOfT : JsonConverterFactory
        {
            public override bool CanConvert(Type typeToConvert)
            {
                return typeToConvert.IsGenericType && typeToConvert.GetGenericTypeDefinition() == typeof(Stack<>);
            }

            public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
            {
                Debug.Assert(typeToConvert.IsGenericType && typeToConvert.GetGenericTypeDefinition() == typeof(Stack<>));

                Type elementType = typeToConvert.GetGenericArguments()[0];

                JsonConverter converter = (JsonConverter)Activator.CreateInstance(
                    typeof(JsonConverterForStackOfT<>).MakeGenericType(new Type[] { elementType }),
                    BindingFlags.Instance | BindingFlags.Public,
                    binder: null,
                    args: null,
                    culture: null)!;

                return converter;
            }
        }

        public class JsonConverterForStackOfT<T> : JsonConverter<Stack<T>>
        {
            public override Stack<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType != JsonTokenType.StartArray || !reader.Read())
                {
                    throw new JsonException();
                }

                var elements = new Stack<T>();

                while (reader.TokenType != JsonTokenType.EndArray)
                {
                    elements.Push(JsonSerializer.Deserialize<T>(ref reader, options));

                    if (!reader.Read())
                    {
                        throw new JsonException();
                    }
                }

                return elements;
            }

            public override void Write(Utf8JsonWriter writer, Stack<T> value, JsonSerializerOptions options)
            {
                writer.WriteStartArray();

                var reversed = new Stack<T>(value);

                foreach (T item in reversed)
                {
                    JsonSerializer.Serialize(writer, item, options);
                }

                writer.WriteEndArray();
            }
        }

        private static void StackBehavior()
        {
            Stack<int> stack = JsonSerializer.Deserialize<Stack<int>>("[1, 2, 3]");
            Console.WriteLine(stack.Peek()); // top of stack is 3

            string serialized = JsonSerializer.Serialize(stack);
            Console.WriteLine(serialized); // stack contents are reversed on serialize [3, 2, 1]

            // This matches Newtonsoft.Json behavior
            stack = JsonConvert.DeserializeObject<Stack<int>>("[1, 2, 3]");
            Console.WriteLine(stack.Peek()); // top of stack is 3

            serialized = JsonConvert.SerializeObject(stack);
            Console.WriteLine(serialized); // stack contents are reversed on serialize [3, 2, 1]

            var options = new JsonSerializerOptions
            {
                Converters = { new JsonConverterFactoryForStackOfT() },
            };

            stack = JsonSerializer.Deserialize<Stack<int>>("[1, 2, 3]", options);
            Console.WriteLine(stack.Peek()); // top of stack is 3

            serialized = JsonSerializer.Serialize(stack, options);
            Console.WriteLine(serialized); // round-trips exactly [1, 2, 3]

            var s = new Stack();
            s.Push(1);
            s.Push(2);
            s.Push(3);

            serialized = JsonSerializer.Serialize(s);
            Console.WriteLine(serialized); // stack contents are reversed on serialize [3, 2, 1]

            ImmutableStack<int> y = ImmutableStack.Create(1, 2, 3);

            serialized = JsonSerializer.Serialize(y);
            Console.WriteLine(serialized); // stack contents are reversed on serialize [3, 2, 1]

            MyStack z = new MyStack(new List<int> { 1, 2, 3 });

            serialized = JsonSerializer.Serialize(z);
            Console.WriteLine(serialized); // stack contents are reversed on serialize [3, 2, 1]
        }

        public class MyStack : IImmutableStack<int>
        {
            private readonly List<int> _input;

            public MyStack(List<int> input)
            {
                _input = input;
            }

            public bool IsEmpty => throw new NotImplementedException();

            public IImmutableStack<int> Clear()
            {
                throw new NotImplementedException();
            }

            public IEnumerator<int> GetEnumerator()
            {
                return _input.GetEnumerator();
            }

            public int Peek()
            {
                throw new NotImplementedException();
            }

            public IImmutableStack<int> Pop()
            {
                throw new NotImplementedException();
            }

            public IImmutableStack<int> Push(int value)
            {
                throw new NotImplementedException();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return ((IEnumerable)_input).GetEnumerator();
            }
        }

        public class CasingOutput
        {
            public int PascalCase { get; set; }
            public int camelCase { get; set; }
            public int ranDoMCAse { get; set; }
        }

        public class DataTableConverter : JsonConverter<DataTable>
        {
            public override DataTable Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                throw new NotImplementedException();
            }

            public override void Write(Utf8JsonWriter writer, DataTable value, JsonSerializerOptions options)
            {
                writer.WriteStartArray();

                foreach (DataRow row in value.Rows)
                {
                    writer.WriteStartObject();
                    foreach (DataColumn column in row.Table.Columns)
                    {
                        object columnValue = row[column];

                        // If necessary:
                        if (options.IgnoreNullValues)
                        {
                            // Do null checks on the values here and skip writing.
                        }

                        writer.WritePropertyName(column.ColumnName);
                        JsonSerializer.Serialize(writer, columnValue, options);
                    }
                    writer.WriteEndObject();
                }

                writer.WriteEndArray();
            }
        }

        public class DataSetConverter : JsonConverter<DataSet>
        {
            private JsonConverter<DataTable> _dataTableConverter;

            public override DataSet Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                throw new NotImplementedException();
            }

            public override void Write(Utf8JsonWriter writer, DataSet value, JsonSerializerOptions options)
            {
                if (_dataTableConverter == null && value.Tables.Count != 0)
                {
                    _dataTableConverter = (JsonConverter<DataTable>)options.GetConverter(typeof(DataTable));
                    Debug.Assert(_dataTableConverter.GetType() == typeof(DataTableConverter));
                }

                writer.WriteStartObject();
                foreach (DataTable table in value.Tables)
                {
                    writer.WritePropertyName(table.TableName);

                    //((JsonConverter<DataTable>)options.GetConverter(typeof(DataTable))).Write(writer, table, options);
                    _dataTableConverter.Write(writer, table, options);
                    //JsonSerializer.Serialize(writer, table, options);
                }
                writer.WriteEndObject();
            }
        }

        private static void DataSet_Serialization_WithSystemTextJson()
        {
            var options = new JsonSerializerOptions()
            {
                Converters = { new DataTableConverter(), new DataSetConverter() }
            };

            (DataTable table, DataSet dataSet) = GetDataSetAndTable();

            string jsonDataTable = JsonSerializer.Serialize(table, options);
            // [{"id":0,"item":"item 0"},{"id":1,"item":"item 1"}]
            Console.WriteLine(jsonDataTable);

            string jsonDataSet = JsonSerializer.Serialize(dataSet, options);
            // {"Table1":[{"id":0,"item":"item 0"},{"id":1,"item":"item 1"}]}
            Console.WriteLine(jsonDataSet);

            (DataTable _, DataSet set) = GetDataSetAndTable(1_000);

            Console.WriteLine(JsonConvert.SerializeObject(set, new JsonSerializerSettings { Formatting = Formatting.Indented }));

            // Local function to create a sample DataTable and DataSet
            (DataTable, DataSet) GetDataSetAndTable(int limit = 1)
            {
                dataSet = new DataSet("dataSet");

                int j = 0;
                do
                {
                    table = new DataTable();
                    DataColumn idColumn = new DataColumn("id", typeof(int))
                    {
                        AutoIncrement = true
                    };

                    DataColumn itemColumn = new DataColumn("item");

                    table.Columns.Add(idColumn);
                    table.Columns.Add(itemColumn);

                    dataSet.Tables.Add(table);

                    for (int i = 0; i < 2; i++)
                    {
                        DataRow newRow = table.NewRow();
                        newRow["item"] = "item " + i;
                        table.Rows.Add(newRow);
                    }
                    j++;
                } while (j < limit);

                dataSet.AcceptChanges();

                return (table, dataSet);
            }
        }

        //This is an arbitrary enum, this could be 'Gender', in your case
        public enum TestEnum
        {
            value1,
            value2,
            value3,
            value4,
        }

        public class TestsViewModel_Option1
        {
            // In your case, this property could be called 'Genders' to be self-documenting
            [JsonPropertyName("enum")]
            public List<TestEnum> ListOfEnums { get; set; }
        }

        public class TestsViewModel_Option2
        {
            // Or use fixed-size array, TestEnum[], if needed
            public List<TestEnum> @enum { get; set; }
        }

        private static void SerializeListOfEnums()
        {
            var model1 = new TestsViewModel_Option1
            {
                ListOfEnums = { TestEnum.value1, TestEnum.value3 }
            };

            var options = new JsonSerializerOptions
            {
                Converters = { new JsonStringEnumConverter() }
            };

            // {"enum":["value1","value3"]}
            Console.WriteLine(JsonSerializer.Serialize(model1, options));

            var model2 = new TestsViewModel_Option2
            {
                @enum = { TestEnum.value1, TestEnum.value3 }
            };

            // {"enum":["value1","value3"]}
            Console.WriteLine(JsonSerializer.Serialize(model2, options));
        }

        public class AzureTableStorageWithString
        {
            public int PartitionKey { get; set; }
            public int RowKey { get; set; }
            public string RemainingJson { get; set; }
        }

        public class AzureTableStorageWithJsonElement
        {
            public int PartitionKey { get; set; }
            public int RowKey { get; set; }
            public JsonElement RemainingJson { get; set; }
        }

        public class Rest
        {
            public LoginViewModel Login { get; set; }
            public Location Loc { get; set; }
            public IndexViewModel Index { get; set; }
        }

        public class ConverterWithJsonDocumentParseWithString : JsonConverter<AzureTableStorageWithString>
        {
            public override AzureTableStorageWithString Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                var result = new AzureTableStorageWithString();

                while (true)
                {
                    if (!reader.Read())
                    {
                        throw new JsonException();
                    }

                    if (reader.TokenType == JsonTokenType.EndObject)
                    {
                        break;
                    }

                    string propertyName = reader.GetString();

                    if (!reader.Read())
                    {
                        throw new JsonException();
                    }

                    switch (propertyName)
                    {
                        case "PartitionKey":
                            result.PartitionKey = reader.GetInt32();
                            break;
                        case "RowKey":
                            result.RowKey = reader.GetInt32();
                            break;
                        case "RemainingJson":
                            using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
                            {
                                result.RemainingJson = doc.RootElement.GetRawText();
                            }
                            break;
                    }
                }

                return result;
            }

            public override void Write(Utf8JsonWriter writer, AzureTableStorageWithString value, JsonSerializerOptions options)
            {
                writer.WriteStartObject();
                writer.WriteNumber(nameof(value.PartitionKey), value.PartitionKey);
                writer.WriteNumber(nameof(value.RowKey), value.RowKey);

                writer.WritePropertyName(nameof(value.RemainingJson));
                using (JsonDocument doc = JsonDocument.Parse(value.RemainingJson))
                {
                    doc.WriteTo(writer);
                }

                writer.WriteEndObject();
            }
        }

        public class ConverterWithJsonDocumentParseWithJsonElement : JsonConverter<AzureTableStorageWithJsonElement>
        {
            public override AzureTableStorageWithJsonElement Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                var result = new AzureTableStorageWithJsonElement();

                while (true)
                {
                    if (!reader.Read())
                    {
                        throw new JsonException();
                    }

                    if (reader.TokenType == JsonTokenType.EndObject)
                    {
                        break;
                    }

                    string propertyName = reader.GetString();

                    if (!reader.Read())
                    {
                        throw new JsonException();
                    }

                    switch (propertyName)
                    {
                        case "PartitionKey":
                            result.PartitionKey = reader.GetInt32();
                            break;
                        case "RowKey":
                            result.RowKey = reader.GetInt32();
                            break;
                        case "RemainingJson":
                            using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
                            {
                                result.RemainingJson = doc.RootElement.Clone();
                            }
                            break;
                    }
                }

                return result;
            }

            public override void Write(Utf8JsonWriter writer, AzureTableStorageWithJsonElement value, JsonSerializerOptions options)
            {
                writer.WriteStartObject();
                writer.WriteNumber(nameof(value.PartitionKey), value.PartitionKey);
                writer.WriteNumber(nameof(value.RowKey), value.RowKey);

                writer.WritePropertyName(nameof(value.RemainingJson));
                value.RemainingJson.WriteTo(writer);

                writer.WriteEndObject();
            }
        }

        public class ConverterWithJsonDocumentParseWithWriteRaw : JsonConverter<AzureTableStorageWithString>
        {
            public override AzureTableStorageWithString Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                var result = new AzureTableStorageWithString();

                while (true)
                {
                    if (!reader.Read())
                    {
                        throw new JsonException();
                    }

                    if (reader.TokenType == JsonTokenType.EndObject)
                    {
                        break;
                    }

                    string propertyName = reader.GetString();

                    if (!reader.Read())
                    {
                        throw new JsonException();
                    }

                    switch (propertyName)
                    {
                        case "PartitionKey":
                            result.PartitionKey = reader.GetInt32();
                            break;
                        case "RowKey":
                            result.RowKey = reader.GetInt32();
                            break;
                        case "RemainingJson":
                            using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
                            {
                                result.RemainingJson = doc.RootElement.GetRawText();
                            }
                            break;
                    }
                }

                return result;
            }

            public override void Write(Utf8JsonWriter writer, AzureTableStorageWithString value, JsonSerializerOptions options)
            {
                writer.WriteStartObject();
                writer.WriteNumber(nameof(value.PartitionKey), value.PartitionKey);
                writer.WriteNumber(nameof(value.RowKey), value.RowKey);

                writer.WritePropertyName(nameof(value.RemainingJson));

                writer.Flush();

                _output.Write(Encoding.UTF8.GetBytes(value.RemainingJson));

                writer.WriteEndObject();
            }
        }

        private static JsonSerializerOptions _optionsWithString;
        private static JsonSerializerOptions _optionsWithJsonElement;
        private static JsonSerializerOptions _optionsWithRaw;
        private static AzureTableStorageWithString _valueWithString;
        private static AzureTableStorageWithJsonElement _valueWithJsonElement;
        private static ArrayBufferWriter<byte> _output;

        private static LoginViewModel CreateLoginViewModel()
            => new LoginViewModel
            {
                Email = "name.familyname@not.com",
                Password = "abcdefgh123456!@",
                RememberMe = true
            };

        private static Location CreateLocation()
            => new Location
            {
                Id = 1234,
                Address1 = "The Street Name",
                Address2 = "20/11",
                City = "The City",
                State = "The State",
                PostalCode = "abc-12",
                Name = "Nonexisting",
                PhoneNumber = "+0 11 222 333 44",
                Country = "The Greatest"
            };

        private static IndexViewModel CreateIndexViewModel()
            => new IndexViewModel
            {
                IsNewAccount = false,
                FeaturedCampaign = new CampaignSummaryViewModel
                {
                    Description = "Very nice campaing",
                    Headline = "The Headline",
                    Id = 234235,
                    OrganizationName = "The Company XYZ",
                    ImageUrl = "https://www.dotnetfoundation.org/theme/img/carousel/foundation-diagram-content.png",
                    Title = "Promoting Open Source"
                },
                ActiveOrUpcomingEvents = Enumerable.Repeat(
                    new ActiveOrUpcomingEvent
                    {
                        Id = 10,
                        CampaignManagedOrganizerName = "Name FamiltyName",
                        CampaignName = "The very new campaing",
                        Description = "The .NET Foundation works with Microsoft and the broader industry to increase the exposure of open source projects in the .NET community and the .NET Foundation. The .NET Foundation provides access to these resources to projects and looks to promote the activities of our communities.",
                        EndDate = DateTime.UtcNow.AddYears(1),
                        Name = "Just a name",
                        ImageUrl = "https://www.dotnetfoundation.org/theme/img/carousel/foundation-diagram-content.png",
                        StartDate = DateTime.UtcNow
                    },
                    count: 20).ToList()
            };

        internal static T Generate<T>()
        {
            if (typeof(T) == typeof(LoginViewModel))
                return (T)(object)CreateLoginViewModel();
            if (typeof(T) == typeof(Location))
                return (T)(object)CreateLocation();
            if (typeof(T) == typeof(IndexViewModel))
                return (T)(object)CreateIndexViewModel();

            throw new NotImplementedException();
        }

        private static void AzureStoragePerf()
        {
            _optionsWithString = new JsonSerializerOptions()
            {
                Converters = { new ConverterWithJsonDocumentParseWithString() }
            };

            _optionsWithJsonElement = new JsonSerializerOptions()
            {
                Converters = { new ConverterWithJsonDocumentParseWithJsonElement() }
            };

            _optionsWithRaw = new JsonSerializerOptions()
            {
                Converters = { new ConverterWithJsonDocumentParseWithWriteRaw() }
            };

            var rest = new Rest()
            {
                Login = Generate<LoginViewModel>(),
                Loc = Generate<Location>(),
                Index = Generate<IndexViewModel>()
            };

            _valueWithString = new AzureTableStorageWithString
            {
                PartitionKey = 1,
                RowKey = 1,
                RemainingJson = JsonSerializer.Serialize(rest, new JsonSerializerOptions() { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping })
            };

            JsonElement element = default;
            using (JsonDocument doc = JsonDocument.Parse(JsonSerializer.Serialize(rest, new JsonSerializerOptions() { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping })))
            {
                element = doc.RootElement.Clone();
            }

            _valueWithJsonElement = new AzureTableStorageWithJsonElement
            {
                PartitionKey = 1,
                RowKey = 1,
                RemainingJson = element
            };

            _output = new ArrayBufferWriter<byte>();

            using (var writer = new Utf8JsonWriter(_output, new JsonWriterOptions { SkipValidation = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping }))
            {
                JsonSerializer.Serialize(writer, _valueWithString, _optionsWithString);
            }
            Console.WriteLine(Encoding.UTF8.GetString(_output.WrittenSpan));
            Console.WriteLine(_output.WrittenCount);
            _output.Clear();

            using (var writer = new Utf8JsonWriter(_output, new JsonWriterOptions { SkipValidation = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping }))
            {
                JsonSerializer.Serialize(writer, _valueWithJsonElement, _optionsWithJsonElement);
            }
            Console.WriteLine(Encoding.UTF8.GetString(_output.WrittenSpan));
            Console.WriteLine(_output.WrittenCount);
            _output.Clear();

            using (var writer = new Utf8JsonWriter(_output, new JsonWriterOptions { SkipValidation = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping }))
            {
                JsonSerializer.Serialize(writer, _valueWithString, _optionsWithRaw);
            }
            Console.WriteLine(Encoding.UTF8.GetString(_output.WrittenSpan));
            Console.WriteLine(_output.WrittenCount);
            _output.Clear();
            Console.WriteLine(_output.WrittenCount);
        }

        private const string _jsonStringPascalCase = "{\"MyString\" : \"abc\", \"MyInteger\" : 123, \"MyList\" : [\"abc\", \"123\"]}";
        private const string _jsonStringCamelCase = "{\"myString\" : \"abc\", \"myInteger\" : 123, \"myList\" : [\"abc\", \"123\"]}";

        private static void CaseSensitive()
        {
            var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };

            var c1 = JsonSerializer.Deserialize<MyClass1>(_jsonStringPascalCase);

            var c2 = JsonSerializer.Deserialize<MyClass1>(_jsonStringPascalCase, options);

            var c3  = JsonSerializer.Deserialize<MyClass1>(_jsonStringCamelCase);

            var c4 = JsonSerializer.Deserialize<MyClass1>(_jsonStringCamelCase, options);

            var c5 = Newtonsoft.Json.JsonConvert.DeserializeObject<MyClass1>(_jsonStringCamelCase);

            Console.WriteLine(c5.MyInteger);
        }

        public class MyClass1
        {
            public int MyInteger { get; set; }

            public string MyString { get; set; }

            public List<string> MyList { get; set; }
        }

        private static void DomTest()
        {
            using (var doc = JsonDocument.Parse("{\"AAAA\":1}"))
            {
            }
        }

        private static void ReaderTest()
        {
            var jsonString = @"{
                ""fruits"": [""apple"", ""banana"", ""pear""],
                ""vegtables"": [""tomato"", ""broccli""],
                ""likesFruits"": false
            }";

            var bytes = System.Text.Encoding.UTF8.GetBytes(jsonString.Trim());
            var stream = new MemoryStream(bytes);

            var buffer = new byte[10];
            var span = new Span<byte>(buffer);

            // fill the buffer
            var read = stream.Read(span);

            var reader = new Utf8JsonReader(span, false, state: default);
            System.Console.WriteLine($"Current string is: {System.Text.Encoding.UTF8.GetString(span)}");

            // try to find the likesFruits value
            while (reader.TokenType != JsonTokenType.PropertyName || reader.GetString() != "likesFruits")
            {
                if (!reader.Read())
                { //exception occurs here processing first element, since the span boundary is on a quote mark.
                    stream.Read(span);
                    System.Console.WriteLine($"Current string is: {System.Text.Encoding.UTF8.GetString(span)}");
                    reader = new Utf8JsonReader(span, false, reader.CurrentState);
                }

            }

            System.Console.WriteLine($"Found element: {reader.GetString()}");
            if (!reader.Read())
            {
                stream.Read(span);
                reader = new Utf8JsonReader(span, false, reader.CurrentState);
                reader.Read();
            }
            System.Console.WriteLine($"Got value: {reader.GetBoolean()}");
        }

        [Fact]
        private static void BadNamingPolicy_ThrowsInvalidOperation()
        {
            ReaderTest();
            DomTest();
            CaseSensitive();
            AzureStoragePerf();
            SerializeListOfEnums();
            ObjectPropertyExample();
            DataSet_Serialization_WithSystemTextJson();

            var casing = new CasingOutput
            {
                PascalCase = 1,
                camelCase = 2,
                ranDoMCAse = 3
            };

            // {"PascalCase":1,"camelCase":2,"ranDoMCAse":3}

            DefaultContractResolver contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };

            Console.WriteLine(JsonConvert.SerializeObject(casing, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.Indented
            }));

            StackBehavior();

            WeatherForecast forecast21 = null;
            Console.WriteLine(JsonSerializer.Serialize(forecast21, new JsonSerializerOptions { IgnoreNullValues = true })); // throw? write empty? - writes "null" today

            var nugetOptions = new JsonSerializerOptions();
            nugetOptions.Converters.Add(new NuGetAuthorsConverter());
            var res1 = JsonSerializer.Deserialize<NuGetTest>("{\"authors\": \"test author\"}", nugetOptions);

            var res2 = JsonSerializer.Deserialize<NuGetTest>("{\"authors\": [\"author1\", \"author2\"]}", nugetOptions);

            var options1Add = new JsonSerializerOptions();
            options1Add.Converters.Add(new StackConverterFactory());

            Console.WriteLine(options1Add.Converters.Count);

            var options2Add = new JsonSerializerOptions
            {
                Converters = { new StackConverterFactory() },
            };
            Console.WriteLine(options2Add.Converters.Count);

            SupportValueTuple();
            var vtupleObject0 = new WeatherForecastValueTuple
            {
                Location = (3, 7),
                Temp = new ValueTuple<string>("FOO"),
                Thruple = (
                    8,
                    new Poco
                    {
                        Alpha = 9,
                        Beta = (new List<int>() { -1, 0, -1 }, "a list")
                    },
                    new ValueTuple<string>("Bar"))
            };

            // "{\"Temp\":{\"Item1\":\"FOO\"},\"Location\":{ \"Item1\":3,\"Item2\":7},\"Thruple\":{ \"Item1\":8,\"Item2\":{ \"Alpha\":9,\"Beta\":{ \"Item1\":[-1,0,-1],\"Item2\":\"a list\"}},\"Item3\":{\"Item1\":\"Bar\"}}}"
            string str = JsonConvert.SerializeObject(vtupleObject0);
            Console.WriteLine(str);

            WeatherForecastValueTuple vtupleObject1 = JsonConvert.DeserializeObject<WeatherForecastValueTuple>(str);
            WeatherForecastValueTuple vtupleObject2 = JsonSerializer.Deserialize<WeatherForecastValueTuple>("{\"Location\":{\"Item1\": 1, \"Item2\": 2}}");

            var vtupleOptions = new JsonSerializerOptions
            {
                Converters = { new ValueTupleFactory() }
            };

            vtupleOptions.Converters.Add(new ValueTupleFactory());

            vtupleObject2 = JsonSerializer.Deserialize<WeatherForecastValueTuple>(str, vtupleOptions);

            Utf8ReaderFromFile.Run();

            string stackOverFlowString1 = @"{
    ""types"":
    [
        [
            ""tour_type"",
            [
                [""groups"", 1],
                [""individual"", 2]
            ]
        ]
    ]
}";
            Tours tours = JsonSerializer.Deserialize<Tours>(stackOverFlowString1);

            foreach (UserType data in tours.Types)
            {
                string typeName = data.Key; // "tour_type"

                foreach (KeyValuePair<string, int> pairs in data.Values)
                {
                    string key = pairs.Key; // "groups" or "individual
                    int value = pairs.Value; // 1 or 2
                }
            }

            Rootobject root = JsonSerializer.Deserialize<Rootobject>(stackOverFlowString1);
            Assert.Equal(1, tours.Types.Count);
            Assert.Equal("tour_type", tours.Types[0].Key);
            Assert.Equal(2, tours.Types[0].Values.Count);

            stackOverFlowString1 = @"{
    ""types"":
    [
        [
            ""tour_type"",
            [
                [""groups"", 1],
                [],
                [""individual"", 2],
                []
            ]
        ],
        []
    ]
}";

            tours = JsonSerializer.Deserialize<Tours>(stackOverFlowString1);
            Assert.Equal(2, tours.Types.Count);
            Assert.Equal("tour_type", tours.Types[0].Key);
            Assert.Equal(2, tours.Types[0].Values.Count);
            Assert.Null(tours.Types[1].Key);

            stackOverFlowString1 = @"{
    ""types"":
    [
        [
            ""tour_type"",
            []
        ]
    ]
}";

            tours = JsonSerializer.Deserialize<Tours>(stackOverFlowString1);
            Assert.Equal(1, tours.Types.Count);
            Assert.Equal("tour_type", tours.Types[0].Key);
            Assert.Equal(0, tours.Types[0].Values.Count);

            var myImpl = new MyImplementation
            {
                Count = 5,
                Name = "Implementation"
            };

            var polymorphicObject = new PolymorphicTestInterface
            {
                MyObject = myImpl
            };

            // {"MyObject":{"Count":5}}
            Console.WriteLine(JsonSerializer.Serialize(polymorphicObject));

            // {"MyObject":{"Count":5,"Name":"Implementation"}}
            Console.WriteLine(JsonConvert.SerializeObject(polymorphicObject));

            var textWriter = new StringWriter();
            using (JsonWriter jsonWriter = new JsonTextWriter(textWriter))
            {
                jsonWriter.StringEscapeHandling = StringEscapeHandling.Default;
                jsonWriter.WriteValue("hello<>!&é");
            }
            Console.WriteLine(textWriter.ToString()); // "hello<>!&é"

            textWriter = new StringWriter();
            using (JsonWriter jsonWriter = new JsonTextWriter(textWriter))
            {
                jsonWriter.StringEscapeHandling = StringEscapeHandling.EscapeHtml;
                jsonWriter.WriteValue("hello<>!&é");
            }
            Console.WriteLine(textWriter.ToString()); // "hello\u003c\u003e!\u0026é"

            textWriter = new StringWriter();
            using (JsonWriter jsonWriter = new JsonTextWriter(textWriter))
            {
                jsonWriter.StringEscapeHandling = StringEscapeHandling.EscapeNonAscii;
                jsonWriter.WriteValue("hello<>!&é");
            }
            Console.WriteLine(textWriter.ToString()); // "hello<>!&\u00e9"

            var outputStream = new MemoryStream();
            using (var jsonWriter = new Utf8JsonWriter(outputStream))
            {
                jsonWriter.WriteStringValue("hello<>!&é");
            }
            Console.WriteLine(Encoding.UTF8.GetString(outputStream.ToArray())); // "hello\u003C\u003E!\u0026\u00E9"

            var forecasts = new List<WeatherForecast> // Change to List<object> for polymorphic behavior
            {
                new WeatherForecast()
                {
                    Date = DateTimeOffset.ParseExact("2019-01-01", "yyyy-MM-dd", CultureInfo.InvariantCulture),
                    TemperatureCelsius = 25,
                    Summary = "Warm",
                },
                new WeatherForecastDerived()
                {
                    Date = DateTimeOffset.ParseExact("2019-01-02", "yyyy-MM-dd", CultureInfo.InvariantCulture),
                    TemperatureCelsius = 22,
                    Summary = "Warm",
                    WindSpeed = 40
                }
            };

            MonthlyForecast monthlyForecast = new MonthlyForecast
            {
                Month = "January",
                Forecasts = forecasts
            };

            // {"Month":"January","Forecasts":[{"Date":"2019-01-01T00:00:00-08:00","TemperatureCelsius":25,"Summary":"Warm"},{"Date":"2019-01-02T00:00:00-08:00","TemperatureCelsius":22,"Summary":"Warm"}]}
            // Note: We are missing WindSpeed
            // Change the List<WeatherForecast> to List<object> and you get the desired behavior:
            // {"Month":"January","Forecasts":[{"Date":"2019-01-01T00:00:00-08:00","TemperatureCelsius":25,"Summary":"Warm"},{"WindSpeed":40,"Date":"2019-01-02T00:00:00-08:00","TemperatureCelsius":22,"Summary":"Warm"}]}
            // Note, in this case, I didn't need to change the root TValue to be object, because that is not what caused the inheritence issue
            Console.WriteLine(JsonSerializer.Serialize<MonthlyForecast>(monthlyForecast));

            var polymorphicList = new Base[2]
            {
                new Base()
                {
                    Name = "Base",
                    Id = 1
                },
                new Derived()
                {
                    Name = "Derived",
                    Value = "Hello"
                }
            };

            var polymorphicDeepProperty = new PolymorphicDeep
            {
                BaseArray = polymorphicList
            };

            // [{"Name":"Base","Id":1},{"Name":"Derived","Id":0}]
            // Where is "Value": "Hello"?
            Console.WriteLine(JsonSerializer.Serialize<object>(polymorphicList));

            // {"BaseArray":[{"Name":"Base","Id":1},{"Name":"Derived","Id":0}]}
            // Where is "Value": "Hello"?
            Console.WriteLine(JsonSerializer.Serialize<object>(polymorphicDeepProperty));

            string jsonString1 = @"{
                ""throw"": null,
                ""duplicate"": null,
                ""id"": 1,
                ""xyz"": null,
                ""nullOverride2"": false,
                ""nullOverride1"": null,
                ""william"": ""shakespeare"",
                ""complex"": {""overwrite"": ""no"", ""type"": ""string"", ""original"": null, ""another"":[]},
                ""nested"": [7, {""another"": true}],
                ""nestedObject"": {""another"": true}
            }";

            string jsonString2 = @"{
                ""william"": ""dafoe"",
                ""duplicate"": null,
                ""foo"": ""bar"",
                ""baz"": {""temp"": 4},
                ""xyz"": [1, 2, 3],
                ""nullOverride1"": true,
                ""nullOverride2"": null,
                ""nested"": [1, 2, 3, null, {""another"": false}],
                ""nestedObject"": [""wow""],
                ""complex"": {""temp"": true, ""overwrite"": ""ok"", ""type"": 14},
                ""temp"": null
            }";

            JObject jObj1 = JObject.Parse(jsonString1);
            JObject jObj2 = JObject.Parse(jsonString2);

            jObj1.Merge(jObj2);
            jObj2.Merge(JObject.Parse(jsonString1));

            Assert.Equal(jObj1.ToString(), Merge(jsonString1, jsonString2));
            Assert.Equal(jObj2.ToString(), Merge(jsonString2, jsonString1));

            var stringReader = new StringReader("{\"MyString\": \"hi\"}, {\"MyString\":\"Another payload\"}");

            var jsonTextReader = new JsonTextReader(stringReader)
            {
                SupportMultipleContent = true
            };

            var jsonSerializer = new Newtonsoft.Json.JsonSerializer();
            Console.WriteLine(JsonConvert.SerializeObject(jsonSerializer.Deserialize<TestingClass>(jsonTextReader))); // {"MyString":"hi"}
            jsonTextReader.Read();
            Console.WriteLine(JsonConvert.SerializeObject(jsonSerializer.Deserialize<TestingClass>(jsonTextReader))); // {"MyString":"Another payload"}

            Console.WriteLine(JsonConvert.SerializeObject(JsonConvert.DeserializeObject<TestingClass>("{\"MyDictionary': {\"hi\":\"byte\"}}, {\"MyDictionary\": {\"hi2\":\"byte2\"}}")));

            Console.WriteLine(JsonSerializer.Serialize(new KeyValuePair<string, string>("MyKey", "MyValue")));


            var objToSerialize = new MyTempStruct();
            string myJStr = JsonSerializer.Serialize(objToSerialize);
            JsonSerializer.Deserialize<MyTempStruct>(myJStr);

            var objToSerialize2 = new MyTempClass();
            myJStr = JsonSerializer.Serialize(objToSerialize2);
            JsonSerializer.Deserialize<MyTempClass>(myJStr);


            //var tempValue = JsonSerializer.Deserialize<int?>("123");

            try
            {
                Console.WriteLine(JsonSerializer.Serialize(new BazText()));
            }
            catch (JsonException ex)
            {
                Console.WriteLine(ex.Path == null);
            }
            using var stream123 = File.Open(@"C:\Users\ahkha\Desktop\WeatherForecastAsyncUtf8.json", FileMode.Open, FileAccess.Read);
            var jsonModel = JsonSerializer.DeserializeAsync<Foo>(stream123).Result;

            JsonSerializerOptions bgasgsga = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.CjkUnifiedIdeographs)
            };

            var agasg = new A { Name = "你好" };
            var ssafgas = JsonSerializer.Serialize(agasg, bgasgsga);
            Console.WriteLine(ssafgas);

            string jstr12 = "hello\"john\"";
            Console.WriteLine(JsonConvert.SerializeObject(jstr12));
            Console.WriteLine(JsonSerializer.Serialize(jstr12));

            StringBuilder abayW = new StringBuilder();
            for (int i = 0; i < 2_000; i++)
                abayW.Append("[");
            for (int i = 0; i < 2_000; i++)
                abayW.Append("]");

            object myObj123 = JsonConvert.DeserializeObject(abayW.ToString());

            JsonElement elem = JsonDocument.Parse("1234.567").RootElement;
            Console.WriteLine(JsonSerializer.Serialize(elem));


            var tempopts = new JsonSerializerOptions();
            tempopts.Converters.Add(new MyBazTextConverter2());

            var b1234 = JsonSerializer.Deserialize<BazText>("{}", tempopts);

            b1234 = JsonConvert.DeserializeObject<BazText>("{}", new MyBazTextConverterB());

            string jstr1 = "[{\"Color\":\"Red\"},{\"Color\":\"Green\"},,]";
            JsonConvert.DeserializeObject(jstr1);

            var jsonByte = Encoding.UTF8.GetBytes("null");

            var abw = new ArrayBufferWriter<byte>();
            using var writer = new Utf8JsonWriter(abw);
            writer.WriteStartObject();
            writer.WritePropertyName("sub object");
            using (Stream stream1 = new MemoryStream(jsonByte))
            {
                JsonDocument doc = JsonDocument.Parse(stream1);
                doc.WriteTo(writer);
            }
            writer.WriteEndObject();
            writer.Flush();
            Console.WriteLine(Encoding.UTF8.GetString(abw.WrittenSpan));

            var point1 = new ImmutablePoint(1, 2);
            var point2 = new ImmutablePoint(3, 4);
            var points = new List<ImmutablePoint> { point1, point2 };

            var deserializeOptions = new JsonSerializerOptions();
            deserializeOptions.WriteIndented = true;
            deserializeOptions.Converters.Add(new ImmutablePointConverter());

            var kvp = new KeyValuePair<object, int>("foo", 0);
            KeyValuePair<object, int> parent = default;

            for (int i = 0; i < 700; i++)
            {
                parent = new KeyValuePair<object, int>(kvp, i);
                kvp = parent;
            }

            Console.WriteLine(JsonSerializer.Serialize(parent));

            JsonSerializer.Serialize(points, deserializeOptions);

            string jsonString = @"{""Date"": null,""TemperatureCelsius"": 25,""Summary"":""Hot""}";
            JsonSerializer.Deserialize<WeatherForecast>(jsonString);

            Dictionary<string, object> testItem = new Dictionary<string, object> { { "id", "MyCustomSerilizerTestId1" }, { "status", "MyTestPk" }, { "description", null } };

            Console.WriteLine(JsonSerializer.Serialize(testItem, new JsonSerializerOptions() { IgnoreNullValues = true }));
            Console.WriteLine(JsonConvert.SerializeObject(testItem, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));



            _writer = new Utf8JsonWriter(new MemoryStream(), new JsonWriterOptions { Indented = true });

            var tempA1 = new GenericUserTestDeserialize<LoginViewModel>(false);
            LoginViewModel xyzabc = tempA1.DeserializeFromStreamAsync().Result;

            Stream myStream = UserMethod<LoginViewModel>(xyzabc);


            var tempA2 = new GenericUserTestDeserialize<IndexViewModel>(false);
            var tempA3 = new GenericUserTestDeserialize<Location>(false);
            var tempA4 = new GenericUserTestDeserialize<MyEventsListerViewModel>(false);

            var tempB1 = new GenericUserTestDeserialize<LoginViewModel>(true);
            var tempB2 = new GenericUserTestDeserialize<IndexViewModel>(true);
            var tempB3 = new GenericUserTestDeserialize<Location>(true);
            var tempB4 = new GenericUserTestDeserialize<MyEventsListerViewModel>(true);

            var xyz_a1_0 = tempA1.DeserializeFromStreamSync();
            var xyz_a1_1 = tempA1.DeserializeFromStreamSync_New();

            var xyz_a2_0 = tempA2.DeserializeFromStreamSync();
            var xyz_a2_1 = tempA2.DeserializeFromStreamSync_New();

            var xyz_a3_0 = tempA3.DeserializeFromStreamSync();
            var xyz_a3_1 = tempA3.DeserializeFromStreamSync_New();

            var xyz_a4_0 = tempA4.DeserializeFromStreamSync();
            var xyz_a4_1 = tempA4.DeserializeFromStreamSync_New();

            var xyz_b1_0 = tempB1.DeserializeFromStreamSync();
            var xyz_b1_1 = tempB1.DeserializeFromStreamSync_New();

            var xyz_b2_0 = tempB2.DeserializeFromStreamSync();
            var xyz_b2_1 = tempB2.DeserializeFromStreamSync_New();

            var xyz_b3_0 = tempB3.DeserializeFromStreamSync();
            var xyz_b3_1 = tempB3.DeserializeFromStreamSync_New();

            var xyz_b4_0 = tempB4.DeserializeFromStreamSync();
            var xyz_b4_1 = tempB4.DeserializeFromStreamSync_New();

            string path = @"E:\Other\trash\LargeTestJsonFiles\TestFile.json";
            var _contentStreamF = new FileStream(path, FileMode.Open);
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = _contentStreamF.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }

                var span = new ReadOnlySpan<byte>(ms.ToArray());
            }


            var _contents = File.ReadAllText(@"E:\GitHub\Fork\Benchmarks\TestData\JsonParsingBenchmark\dotnet-core.json");
            var _contentsUtf8 = Encoding.UTF8.GetBytes(_contents);
            var _contentStream = new MemoryStream(_contentsUtf8);

            var _jsonOptions1 = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var retObj = JsonSerializer.Deserialize<SearchResults>(_contentsUtf8, _jsonOptions1);


            var myException = new JsonException("MyString", new InvalidOperationException());

            try
            {
                throw myException;

            }
            catch (JsonException ex)
            {
                string jsonserialziedException = JsonConvert.SerializeObject(ex);

                JsonException roundtripException = JsonSerializer.Deserialize<JsonException>(jsonserialziedException);

                //jsonserialziedException = JsonSerializer.Serialize(ex);

                string searchString = "\"InnerException\":{";

                Console.WriteLine(jsonserialziedException);

                jsonserialziedException = jsonserialziedException.Insert(jsonserialziedException.IndexOf(searchString) + searchString.Length, "\"$type\":\"" + typeof(InvalidOperationException).AssemblyQualifiedName + "\", ");

                Console.WriteLine(jsonserialziedException);

                roundtripException = JsonConvert.DeserializeObject<JsonException>(jsonserialziedException);
            }

            var mystructfoo = new StructFoo();
            mystructfoo.MyType = typeof(string);

            string jsonSerialized = "{\"MyType\": " + JsonConvert.SerializeObject(typeof(string)) + "}";
            Console.WriteLine(jsonSerialized);
            jsonSerialized = JsonConvert.SerializeObject(mystructfoo);

            var newbuilder = new StringBuilder();
            newbuilder.Append(".String");
            for (int i = 0; i < 1; i++)
            {
                newbuilder.Append("[]");
            }

            string replaced = newbuilder.ToString();


            jsonSerialized = jsonSerialized.Replace(".String", replaced);
            Console.WriteLine(jsonSerialized);

            StructFoo myType = JsonConvert.DeserializeObject<StructFoo>(jsonSerialized);

            StructFoo mystruct123 = JsonSerializer.Deserialize<StructFoo>(Encoding.UTF8.GetBytes("null"));

            var options = new JsonSerializerOptions();
            options.Converters.Add(new PlayerConverter());

            Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(new Player { Name = "noname", Id = 1 }, options));

            var localReader = new Utf8JsonReader(Encoding.UTF8.GetBytes("[null,]"), new JsonReaderOptions { AllowTrailingCommas = true });
            localReader.Read(); Debug.Assert(localReader.TokenType == JsonTokenType.StartArray);
            localReader.Read(); Debug.Assert(localReader.TokenType == JsonTokenType.Null);
            string firstElement = localReader.GetString();
            localReader.Read(); Debug.Assert(localReader.TokenType == JsonTokenType.EndArray);

            localReader = new Utf8JsonReader(Encoding.UTF8.GetBytes("[null,]"), new JsonReaderOptions { AllowTrailingCommas = true });

            // System.Text.Json.JsonException : The JSON array contains a trailing comma at the end which is not supported in this mode. Change the reader options. Path: $[1] | LineNumber: 0 | BytePositionInLine: 6.
            //string[] returnedValue = JsonSerializer.Deserialize<string[]>(ref localReader);

            localReader.Read();
            localReader.Read();

            localReader = new Utf8JsonReader(Encoding.UTF8.GetBytes("[[[null]], 5]"), new JsonReaderOptions { MaxDepth = 3 });

            localReader.Read();
            localReader.Read();

            JsonSerializer.Deserialize<string[][]>(ref localReader);
            localReader.Read();
            Assert.Equal(5, localReader.GetInt32());
            localReader.Read();
            Assert.Equal(JsonTokenType.EndArray, localReader.TokenType);
            Assert.False(localReader.Read());

            var jsonPayload = File.ReadAllText(@"C:\Users\ahkha\Desktop\Sample.json");

            var serializationOptions = new JsonSerializerOptions { MaxDepth = 150 };
            serializationOptions.Converters.Add(new CustomConverter());

            // works fine
            var direct = JsonSerializer.Deserialize<DeepArray>(jsonPayload, serializationOptions);

            // fails with exception:
            // "The maximum configured depth of 64 has been exceeded. Cannot read next JSON array"
            var custom = JsonSerializer.Deserialize<IContent>(jsonPayload, serializationOptions);

            string mynullstr = JsonSerializer.Serialize(null, null);

            MyStruct123 mystruct = JsonSerializer.Deserialize<MyStruct123>("null"); // throws NRE rather than InvalidOperationException

            var stream = new MemoryStream(Encoding.UTF8.GetBytes("null"));
            //object xxy = await JsonSerializer.DeserializeAsync(stream, typeof(string));

            var optrand = new JsonSerializerOptions();
            optrand.Converters.Add(new MyTempConverter());

            JsonSerializer.Deserialize<int>("5", optrand);

            //Baz1 myint1 = JsonSerializer.Deserialize<Baz1>(Encoding.UTF8.GetBytes("null"));
            var b1 = JsonSerializer.Deserialize<Baz1>("{\"myrandom\":[{\"key\":\"value\"}]}");

            //var table = new Hashtable();
            //DateTime? nullInt = DateTime.Now;
            //table.Add("key", nullInt);

            //JsonSerializer.Serialize(table);

            var listOfObjects = new Dictionary<string, object>();
            listOfObjects.Add("key", 5);

            var poco = new DictionaryWrapper(listOfObjects);

            Console.WriteLine(JsonSerializer.Serialize(poco));

            string s0 = null;
            JsonNode sNode = s0;

            bool result = sNode == null;

            JsonNull someNode = (JsonNull)JsonNode.Parse("null");

#pragma warning disable CS0252 // Possible unintended reference comparison; left hand side needs cast
            bool result2 = someNode == new JsonNull();
#pragma warning restore CS0252 // Possible unintended reference comparison; left hand side needs cast

            Console.WriteLine(result2);

            Console.WriteLine(JsonSerializer.Serialize(null, null));


            var mytemp = JsonSerializer.Deserialize<GetSet>("{\"mystr\": [\"foo\"]}");

            var retVal = JsonSerializer.Deserialize<MyRandom>("{\"MyImm\":{\"abc\":123}}");

            Dictionary<string, Foo> x1 = new Dictionary<string, Foo>();
            x1.Add("bar", null);

            JsonSerializer.Serialize(x1);

            var myrandOpts = new JsonSerializerOptions();
            myrandOpts.Converters.Add(new MyFactory());
            JsonSerializer.Serialize<int>(5, myrandOpts);

            JsonElement.ArrayEnumerator defaultEnum = default;
            Console.WriteLine(defaultEnum.Current.ToString()); // would fail the Debug.Assert.

            var builder = new StringBuilder();
            builder.Append("[");
            for (int i = 0; i < 1_000; i++)
            {
                builder.Append($"{i},");
            }
            builder.Append("-1");
            builder.Append("]");

            var _instance = builder.ToString();
            var _utf8Bytes = Encoding.UTF8.GetBytes(_instance);
            JsonSerializer.Deserialize<List<int>>(_utf8Bytes);

            JsonSerializer.Deserialize<OverFlowClass>("{\"myoverflow\": 1, \"value\": []}");

            var mytempopts = new JsonSerializerOptions();
            mytempopts.Converters.Add(new MyStringConverter());
            var myrandobj = JsonSerializer.Deserialize<RandomObject>("{\"myint\":\"5\"}");
            myrandobj.myint = null;
            JsonSerializer.Serialize(myrandobj);

            var _myoptions = new JsonSerializerOptions();
            _myoptions.Converters.Add(new MyConvert());
            _myoptions.Converters.Add(new MyConvertList());

            MyList[] mylistobject = new MyList[2];
            mylistobject[0] = new MyList { 5 };

            JsonSerializer.Deserialize<List<int>>("\"hello\"");

            var u = new SomeClass();
            u.MyVal = DBNull.Value;

            var _optionsWithConverter = new JsonSerializerOptions();
            _optionsWithConverter.Converters.Add(new DBNullConverter());
            _optionsWithConverter.IgnoreNullValues = true;

            string mystringvalue = JsonSerializer.Serialize(u, _optionsWithConverter);

            string newton = JsonConvert.SerializeObject(u, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            string Contents = JsonSerializer.Serialize(new ThousandSmallClassArray());
            var tobj1 = JsonSerializer.Deserialize<ThousandSmallClassArray>(Contents, _jsonOptions);

            tobj1 = JsonSerializer.Deserialize<ThousandSmallClassArray>(Contents, _jsonOptions);

            string json = @"{
  ""UserName"": ""domain\\username"",
  ""Enabled"": true
}";
            MethodInfo[] methods = typeof(ReadValueTests).GetMethods(BindingFlags.Static | BindingFlags.NonPublic);
            var parameters = methods[0].GetParameters();
            Console.WriteLine(parameters[0].ParameterType.FullName == null);

            JsonSerializer.Deserialize("{}", parameters[0].ParameterType);
            var tempobj = JsonSerializer.Deserialize<MyClass>("{\"stuff\":{}}");

            var opts = new JsonSerializerOptions();
            //opts.Converters.Add(new MyUserConverter());

            User user = JsonSerializer.Deserialize<User>(json, opts);

            Console.WriteLine(user.UserName);

            Console.WriteLine(JsonSerializer.Serialize(user));

            WeatherForecast obj1 = JsonConvert.DeserializeObject<WeatherForecast>("{\"Date\": \"2012-03-21T05:40Z\"}", new JsonSerializerSettings { DateParseHandling = DateParseHandling.None });


            var options123 = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic, UnicodeRanges.GreekExtended),
                WriteIndented = true
            };

            var forecast = new WeatherForecast();
            forecast.Date = DateTimeOffset.Now;
            forecast.TemperatureCelsius = 0;
            forecast.Summary = "жарко";
            var s = JsonSerializer.Serialize(forecast, options123);
            Console.WriteLine(s);

            //var attr = new JsonConverterAttribute(null);

            object myObj = Activator.CreateInstance(typeof(int?));

            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<char>("null"));
            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<char>(""));
            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<char>("1234"));
            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<char>("\"stringTooLong\""));
            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<char>("\"\u0059\"B"));
            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<char>("\"\uD800\uDC00\""));
            Assert.Equal('a', JsonSerializer.Deserialize<char>("\"a\""));
            Assert.Equal('Y', JsonSerializer.Deserialize<char>("\"\u0059\""));

            try
            {
                var obj = JsonSerializer.Deserialize<object>("true1");
            }
            catch (JsonException ex)
            {
                Console.WriteLine(ex.Message);
            }

            var options1 = new JsonSerializerOptions();
            options1.Converters.Add(new MyConverter());

            var b = new Baz();
            b.Temp = "HI";

            //string str = null;
            JsonSerializer.Serialize(b, options123);

            JsonConverter x = options123.GetConverter(typeof(DateTime));
            Console.WriteLine(x.CanConvert(null));


            //string json = JsonSerializer.Serialize(b, options);
            Console.WriteLine(x);
            //JsonElement.ArrayEnumerator defaultEnum = default;
            //defaultEnum.MoveNext();
            //Console.WriteLine(defaultEnum.Current.ToString());

            //JsonElement elem = default;
            //elem.EnumerateArray();
            //elem.ValueEquals("");

            //List<JsonNode> list = new List<JsonNode>();
            //list.Add(JsonNode.Parse("123"));
            //list.Add(JsonNode.Parse("null"));
            //list.Add(null);

            ////JsonArray arr = new JsonArray((IEnumerable<int>)null);
            ////foreach (var item in arr)
            ////{
            ////    Console.WriteLine(item.ToJsonString());
            ////}

            //var jsonArray = new JsonArray { 1, null, 2 };

            //Assert.Equal(3, jsonArray.Count);
            //Assert.Equal(1, ((JsonNumber)jsonArray[0]).GetInt32());
            ////Assert.Equal(2, ((JsonNumber)jsonArray[1]).GetInt32());

            //JsonNode[] temp = list.ToArray();
            //((ICollection<JsonNode>)jsonArray).CopyTo(temp, 0);

            //Console.WriteLine(temp.Length);

            //JsonArrayEnumerator x = default;
            //var y = x.Current;
            //Console.WriteLine(y == null);
            ////Assert.False(x.MoveNext());

            ////JsonArrayEnumerator z = new JsonArrayEnumerator(null);

            //JsonObject obj = new JsonObject();
            //obj.Add("", null);
            //Console.WriteLine(obj.ToJsonString());
            //obj.TryGetPropertyValue(null, out JsonNode node);
        }

        [Fact]
        public static void NullTypeThrows()
        {
            var partOfSpeech1 = JsonDocument.Parse("\"NOUN\"").RootElement;
            var partOfSpeech2 = JsonDocument.Parse("\"ADJ\"").RootElement;
            var val = (PartOfSpeech)((int)Helper2(partOfSpeech1) | (int)Helper2(partOfSpeech2));

            var f = new Foo<bool>();
            Console.WriteLine(f.ToString());
        }

        private static PartOfSpeech Helper2(JsonElement partOfSpeech)
        {
            string speech = partOfSpeech.GetString();
            switch (speech)
            {
                case "ADJ":
                    return PartOfSpeech.Adjective;
                case "ADV":
                    return PartOfSpeech.Adverb;
                case "CONJ":
                    return PartOfSpeech.Conjunction;
                case "DET":
                    return PartOfSpeech.Determiner;
                case "MODAL":
                    return PartOfSpeech.Modal;
                case "NOUN":
                    return PartOfSpeech.Noun;
                case "PREP":
                    return PartOfSpeech.Pronoun;
                case "PRON":
                    return PartOfSpeech.Verb;
                case "VERB":
                    return PartOfSpeech.Other;
                default:
                    Debug.Fail("unknown part of speech : " + partOfSpeech.GetString());
                    return PartOfSpeech.Other;
            };
        }

        public enum PartOfSpeech
        {
            Adjective,
            Adverb,
            Conjunction,
            Other,
            Verb,
            Pronoun,
            Preposition,
            Noun,
            Modal,
            Determiner
        }

        /*public static string SortProperties(this JsonElement jsonElement)
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = new Utf8JsonWriter(stream))
                {
                    switch (jsonElement.ValueKind)
                    {
                        case JsonValueKind.Undefined:
                            throw new InvalidOperationException();
                        case JsonValueKind.Object:
                            jsonElement.SortObjectProperties(writer);
                            break;
                        case JsonValueKind.Array:
                            jsonElement.SortArrayProperties(writer);
                            break;
                        case JsonValueKind.String:
                        case JsonValueKind.Number:
                        case JsonValueKind.True:
                        case JsonValueKind.False:
                        case JsonValueKind.Null:
                            jsonElement.WriteTo(writer);
                            break;
                    };
                    writer.Flush();
                    return Encoding.UTF8.GetString(stream.ToArray());
                }
            }
        }

        private static void SortObjectProperties(this JsonElement jObject, Utf8JsonWriter writer)
        {
            Debug.Assert(jObject.ValueKind == JsonValueKind.Object);

            writer.WriteStartObject();
            foreach (JsonProperty prop in jObject.EnumerateObject().OrderBy(p => p.Name))
            {
                writer.WritePropertyName(prop.Name);
                prop.Value.WriteElementHelper(writer);
            }
            writer.WriteEndObject();
        }

        private static void SortArrayProperties(this JsonElement jArray, Utf8JsonWriter writer)
        {
            Debug.Assert(jArray.ValueKind == JsonValueKind.Array);

            writer.WriteStartArray();
            foreach (JsonElement item in jArray.EnumerateArray())
            {
                item.WriteElementHelper(writer);
            }
            writer.WriteEndArray();
        }

        private static void WriteElementHelper(this JsonElement item, Utf8JsonWriter writer)
        {
            Debug.Assert(item.ValueKind != JsonValueKind.Undefined);

            if (item.ValueKind == JsonValueKind.Object)
            {
                item.SortObjectProperties(writer);
            }
            else if (item.ValueKind == JsonValueKind.Array)
            {
                item.SortArrayProperties(writer);
            }
            else
            {
                item.WriteTo(writer);
            }
        }

        public static JsonElement SortProperties_old(this JsonElement jObject)
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = new Utf8JsonWriter(stream))
                {
                    writer.WriteStartObject();
                    foreach (JsonProperty prop in jObject.EnumerateObject().OrderBy(p => p.Name))
                    {
                        writer.WritePropertyName(prop.Name);
                        if (prop.Value.ValueKind == JsonValueKind.Object)
                        {
                            prop.Value.SortProperties_old().WriteTo(writer);
                        }
                        else if (prop.Value.ValueKind == JsonValueKind.Array)
                        {
                            prop.Value.SortArrayProperties_old().WriteTo(writer);
                        }
                        else
                        {
                            prop.Value.WriteTo(writer);
                        }
                    }
                    writer.WriteEndObject();
                    writer.Flush();
                    return JsonDocument.Parse(stream.ToArray()).RootElement;
                }
            }
        }

        public static JsonElement SortArrayProperties_old(this JsonElement jArray)
        {
            if (jArray.GetArrayLength() == 0)
            {
                return jArray;
            }

            using (var stream = new MemoryStream())
            {
                using (var writer = new Utf8JsonWriter(stream))
                {
                    writer.WriteStartArray();
                    foreach (JsonElement item in jArray.EnumerateArray())
                    {
                        if (item.ValueKind == JsonValueKind.Object)
                        {
                            item.SortProperties_old().WriteTo(writer);
                        }
                        else if (item.ValueKind == JsonValueKind.Array)
                        {
                            item.SortArrayProperties_old().WriteTo(writer);
                        }
                    }
                    writer.WriteEndArray();
                    writer.Flush();
                    return JsonDocument.Parse(stream.ToArray()).RootElement;
                }
            }
        }*/

        [Fact]
        public static void SerializerOptionsStillApply()
        {
            var options = new JsonSerializerOptions();
            options.PropertyNameCaseInsensitive = true;

            byte[] utf8 = Encoding.UTF8.GetBytes(@"{""myint16"":1}");
            var reader = new Utf8JsonReader(utf8, isFinalBlock: true, state: default);

            SimpleTestClass obj = JsonSerializer.Deserialize<SimpleTestClass>(ref reader, options);
            Assert.Equal(1, obj.MyInt16);

            Assert.Equal(JsonTokenType.EndObject, reader.TokenType);
        }

        [Fact]
        public static void ReaderOptionsWinMaxDepth()
        {
            byte[] utf8 = Encoding.UTF8.GetBytes("[[]]");

            var readerOptions = new JsonReaderOptions
            {
                MaxDepth = 1,
            };

            var serializerOptions = new JsonSerializerOptions
            {
                MaxDepth = 5,
            };

            var state = new JsonReaderState(readerOptions);

            Assert.Throws<JsonException>(() =>
            {
                var reader = new Utf8JsonReader(utf8, isFinalBlock: false, state);
                JsonSerializer.Deserialize(ref reader, typeof(int), serializerOptions);
            });
        }

        [Fact]
        public static void ReaderOptionsWinTrailingCommas()
        {
            byte[] utf8 = Encoding.UTF8.GetBytes("[1, 2, 3,]");

            var serializerOptions = new JsonSerializerOptions
            {
                AllowTrailingCommas = true,
            };

            Assert.Throws<JsonException>(() =>
            {
                var reader = new Utf8JsonReader(utf8, isFinalBlock: false, state: default);
                JsonSerializer.Deserialize(ref reader, typeof(int), serializerOptions);
            });
        }

        [Fact]
        public static void ReaderOptionsWinComments()
        {
            byte[] utf8 = Encoding.UTF8.GetBytes("[1, 2, 3]/* some comment */");

            var serializerOptions = new JsonSerializerOptions
            {
                ReadCommentHandling = JsonCommentHandling.Skip,
            };

            Assert.Throws<JsonException>(() =>
            {
                var reader = new Utf8JsonReader(utf8, isFinalBlock: false, state: default);
                JsonSerializer.Deserialize(ref reader, typeof(int), serializerOptions);
            });
        }

        [Fact]
        public static void OnInvalidReaderIsRestored()
        {
            byte[] utf8 = Encoding.UTF8.GetBytes("[1, 2, 3}");

            var reader = new Utf8JsonReader(utf8, isFinalBlock: true, state: default);
            reader.Read();
            long previous = reader.BytesConsumed;

            try
            {
                JsonSerializer.Deserialize(ref reader, typeof(int[]));
                Assert.True(false, "Expected ReadValue to throw JsonException for invalid JSON.");
            }
            catch (JsonException) { }

            Assert.Equal(previous, reader.BytesConsumed);
            Assert.Equal(JsonTokenType.StartArray, reader.TokenType);
        }

        [Fact]
        public static void DataRemaining()
        {
            byte[] utf8 = Encoding.UTF8.GetBytes("{\"Foo\":\"abc\", \"Bar\":123}");

            var reader = new Utf8JsonReader(utf8, isFinalBlock: true, state: default);
            reader.Read();

            SimpleType instance = JsonSerializer.Deserialize<SimpleType>(ref reader);
            Assert.Equal("abc", instance.Foo);

            Assert.Equal(utf8.Length, reader.BytesConsumed);
            Assert.Equal(JsonTokenType.EndObject, reader.TokenType);
        }

        public class SimpleType
        {
            public string Foo { get; set; }
        }

        [Fact]
        public static void ReadPropertyName()
        {
            byte[] utf8 = Encoding.UTF8.GetBytes("{\"Foo\":[1, 2, 3]}");

            var reader = new Utf8JsonReader(utf8, isFinalBlock: true, state: default);
            reader.Read();
            reader.Read();
            Assert.Equal(JsonTokenType.PropertyName, reader.TokenType);

            try
            {
                JsonSerializer.Deserialize<SimpleTypeWithArray>(ref reader);
                Assert.True(false, "Expected ReadValue to throw JsonException for type mismatch.");
            }
            catch (JsonException) { }

            Assert.Equal(JsonTokenType.PropertyName, reader.TokenType);

            int[] instance = JsonSerializer.Deserialize<int[]>(ref reader);
            Assert.Equal(new int[] { 1, 2, 3 }, instance);

            Assert.Equal(utf8.Length - 1, reader.BytesConsumed);
            Assert.Equal(JsonTokenType.EndArray, reader.TokenType);

            Assert.True(reader.Read());
            Assert.Equal(JsonTokenType.EndObject, reader.TokenType);
            Assert.False(reader.Read());
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public static void ReadObjectMultiSegment(bool isFinalBlock)
        {
            byte[] utf8 = Encoding.UTF8.GetBytes("[1, 2, {\"Foo\":[1, 2, 3]}]");
            ReadOnlySequence<byte> sequence = JsonTestHelper.CreateSegments(utf8);

            var reader = new Utf8JsonReader(sequence, isFinalBlock, state: default);
            reader.Read();
            reader.Read();
            reader.Read();
            reader.Read();
            Assert.Equal(JsonTokenType.StartObject, reader.TokenType);

            SimpleTypeWithArray instance = JsonSerializer.Deserialize<SimpleTypeWithArray>(ref reader);

            Assert.Equal(JsonTokenType.EndObject, reader.TokenType);
            Assert.Equal(new int[] { 1, 2, 3 }, instance.Foo);
            Assert.Equal(utf8.Length - 1, reader.BytesConsumed);

            Assert.True(reader.Read());
            Assert.Equal(JsonTokenType.EndArray, reader.TokenType);
            Assert.False(reader.Read());
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public static void NotEnoughData(bool isFinalBlock)
        {
            {
                byte[] utf8 = Encoding.UTF8.GetBytes("\"start of string");

                var reader = new Utf8JsonReader(utf8, isFinalBlock, state: default);
                Assert.Equal(0, reader.BytesConsumed);
                Assert.Equal(JsonTokenType.None, reader.TokenType);

                try
                {
                    JsonSerializer.Deserialize<SimpleTypeWithArray>(ref reader);
                    Assert.True(false, "Expected ReadValue to throw JsonException for not enough data.");
                }
                catch (JsonException) { }

                Assert.Equal(0, reader.BytesConsumed);
                Assert.Equal(JsonTokenType.None, reader.TokenType);
            }

            {
                byte[] utf8 = Encoding.UTF8.GetBytes("{");

                var reader = new Utf8JsonReader(utf8, isFinalBlock, state: default);
                reader.Read();

                Assert.Equal(1, reader.BytesConsumed);
                Assert.Equal(JsonTokenType.StartObject, reader.TokenType);

                try
                {
                    JsonSerializer.Deserialize<SimpleTypeWithArray>(ref reader);
                    Assert.True(false, "Expected ReadValue to throw JsonException for not enough data.");
                }
                catch (JsonException) { }

                Assert.Equal(1, reader.BytesConsumed);
                Assert.Equal(JsonTokenType.StartObject, reader.TokenType);
            }
        }

        [Fact]
        public static void EndObjectOrArrayIsInvalid()
        {
            byte[] utf8 = Encoding.UTF8.GetBytes("[{}]");

            var reader = new Utf8JsonReader(utf8, isFinalBlock: true, state: default);
            reader.Read();
            reader.Read();
            reader.Read();
            Assert.Equal(JsonTokenType.EndObject, reader.TokenType);

            try
            {
                JsonSerializer.Deserialize<SimpleTypeWithArray>(ref reader);
                Assert.True(false, "Expected ReadValue to throw JsonException for invalid token.");
            }
            catch (JsonException ex)
            {
                Assert.Equal(0, ex.LineNumber);
                Assert.Equal(3, ex.BytePositionInLine);
                Assert.Equal("$", ex.Path);
            }

            Assert.Equal(JsonTokenType.EndObject, reader.TokenType);

            reader.Read();
            Assert.Equal(JsonTokenType.EndArray, reader.TokenType);

            try
            {
                JsonSerializer.Deserialize<SimpleTypeWithArray>(ref reader);
                Assert.True(false, "Expected ReadValue to throw JsonException for invalid token.");
            }
            catch (JsonException ex)
            {
                Assert.Equal(0, ex.LineNumber);
                Assert.Equal(4, ex.BytePositionInLine);
                Assert.Equal("$", ex.Path);
            }

            Assert.Equal(JsonTokenType.EndArray, reader.TokenType);
        }

        public class SimpleTypeWithArray
        {
            public int[] Foo { get; set; }
        }

        [Theory]
        [InlineData("1234", typeof(int), 1234)]
        [InlineData("null", typeof(string), null)]
        [InlineData("true", typeof(bool), true)]
        [InlineData("false", typeof(bool), false)]
        [InlineData("\"my string\"", typeof(string), "my string")]
        public static void Primitives(string jsonString, Type type, object expected)
        {
            byte[] utf8 = Encoding.UTF8.GetBytes(jsonString);

            var reader = new Utf8JsonReader(utf8, isFinalBlock: true, state: default);
            Assert.Equal(JsonTokenType.None, reader.TokenType);

            object obj = JsonSerializer.Deserialize(ref reader, type);
            Assert.False(reader.HasValueSequence);
            Assert.Equal(utf8.Length, reader.BytesConsumed);
            Assert.Equal(expected, obj);

            Assert.False(reader.Read());
        }

        [Theory]
        [InlineData("1234", typeof(int), 1234)]
        [InlineData("null", typeof(string), null)]
        [InlineData("true", typeof(bool), true)]
        [InlineData("false", typeof(bool), false)]
        [InlineData("\"my string\"", typeof(string), "my string")]
        public static void PrimitivesMultiSegment(string jsonString, Type type, object expected)
        {
            byte[] utf8 = Encoding.UTF8.GetBytes(jsonString);
            ReadOnlySequence<byte> sequence = JsonTestHelper.CreateSegments(utf8);

            var reader = new Utf8JsonReader(sequence, isFinalBlock: true, state: default);
            Assert.Equal(JsonTokenType.None, reader.TokenType);

            object obj = JsonSerializer.Deserialize(ref reader, type);
            Assert.True(reader.HasValueSequence);
            Assert.Equal(utf8.Length, reader.BytesConsumed);
            Assert.Equal(expected, obj);

            Assert.False(reader.Read());
        }

        [Fact]
        public static void EnableComments()
        {
            string json = "3";

            var options = new JsonReaderOptions
            {
                CommentHandling = JsonCommentHandling.Allow,
            };

            byte[] utf8 = Encoding.UTF8.GetBytes(json);

            AssertExtensions.Throws<ArgumentException>(
                "reader",
                () =>
                {
                    var state = new JsonReaderState(options);
                    var reader = new Utf8JsonReader(utf8, isFinalBlock: false, state);
                    JsonSerializer.Deserialize(ref reader, typeof(int));
                });

            AssertExtensions.Throws<ArgumentException>(
                "reader",
                () =>
                {
                    var state = new JsonReaderState(options);
                    var reader = new Utf8JsonReader(utf8, isFinalBlock: true, state);
                    JsonSerializer.Deserialize(ref reader, typeof(int));
                });

            AssertExtensions.Throws<ArgumentException>(
               "reader",
               () =>
               {
                   var state = new JsonReaderState(options);
                   var reader = new Utf8JsonReader(utf8, isFinalBlock: false, state);
                   JsonSerializer.Deserialize<int>(ref reader);
               });

            AssertExtensions.Throws<ArgumentException>(
                "reader",
                () =>
                {
                    var state = new JsonReaderState(options);
                    var reader = new Utf8JsonReader(utf8, isFinalBlock: true, state);
                    JsonSerializer.Deserialize<int>(ref reader);
                });
        }

        [Fact]
        public static void ReadDefaultReader()
        {
            Assert.ThrowsAny<JsonException>(() =>
            {
                Utf8JsonReader reader = default;
                JsonSerializer.Deserialize(ref reader, typeof(int));
            });

            Assert.ThrowsAny<JsonException>(() =>
            {
                Utf8JsonReader reader = default;
                JsonSerializer.Deserialize<int>(ref reader);
            });
        }

        [Fact]
        public static void ReadSimpleStruct()
        {
            byte[] utf8 = Encoding.UTF8.GetBytes(SimpleTestStruct.s_json);
            var reader = new Utf8JsonReader(utf8, isFinalBlock: true, state: default);
            SimpleTestStruct testStruct = JsonSerializer.Deserialize<SimpleTestStruct>(ref reader);
            testStruct.Verify();

            reader = new Utf8JsonReader(utf8, isFinalBlock: true, state: default);
            object obj = JsonSerializer.Deserialize(ref reader, typeof(SimpleTestStruct));
            ((SimpleTestStruct)obj).Verify();
        }

        [Fact]
        public static void ReadClasses()
        {
            {
                byte[] utf8 = Encoding.UTF8.GetBytes(TestClassWithNestedObjectInner.s_json);
                var reader = new Utf8JsonReader(utf8, isFinalBlock: true, state: default);
                TestClassWithNestedObjectInner testStruct = JsonSerializer.Deserialize<TestClassWithNestedObjectInner>(ref reader);
                testStruct.Verify();

                reader = new Utf8JsonReader(utf8, isFinalBlock: true, state: default);
                object obj = JsonSerializer.Deserialize(ref reader, typeof(TestClassWithNestedObjectInner));
                ((TestClassWithNestedObjectInner)obj).Verify();
            }

            {
                var reader = new Utf8JsonReader(TestClassWithNestedObjectOuter.s_data, isFinalBlock: true, state: default);
                TestClassWithNestedObjectOuter testStruct = JsonSerializer.Deserialize<TestClassWithNestedObjectOuter>(ref reader);
                testStruct.Verify();

                reader = new Utf8JsonReader(TestClassWithNestedObjectOuter.s_data, isFinalBlock: true, state: default);
                object obj = JsonSerializer.Deserialize(ref reader, typeof(TestClassWithNestedObjectOuter));
                ((TestClassWithNestedObjectOuter)obj).Verify();
            }

            {
                var reader = new Utf8JsonReader(TestClassWithObjectList.s_data, isFinalBlock: true, state: default);
                TestClassWithObjectList testStruct = JsonSerializer.Deserialize<TestClassWithObjectList>(ref reader);
                testStruct.Verify();

                reader = new Utf8JsonReader(TestClassWithObjectList.s_data, isFinalBlock: true, state: default);
                object obj = JsonSerializer.Deserialize(ref reader, typeof(TestClassWithObjectList));
                ((TestClassWithObjectList)obj).Verify();
            }

            {
                var reader = new Utf8JsonReader(TestClassWithObjectArray.s_data, isFinalBlock: true, state: default);
                TestClassWithObjectArray testStruct = JsonSerializer.Deserialize<TestClassWithObjectArray>(ref reader);
                testStruct.Verify();

                reader = new Utf8JsonReader(TestClassWithObjectArray.s_data, isFinalBlock: true, state: default);
                object obj = JsonSerializer.Deserialize(ref reader, typeof(TestClassWithObjectArray));
                ((TestClassWithObjectArray)obj).Verify();
            }

            {
                var reader = new Utf8JsonReader(TestClassWithObjectIEnumerableT.s_data, isFinalBlock: true, state: default);
                TestClassWithObjectIEnumerableT testStruct = JsonSerializer.Deserialize<TestClassWithObjectIEnumerableT>(ref reader);
                testStruct.Verify();

                reader = new Utf8JsonReader(TestClassWithObjectIEnumerableT.s_data, isFinalBlock: true, state: default);
                object obj = JsonSerializer.Deserialize(ref reader, typeof(TestClassWithObjectIEnumerableT));
                ((TestClassWithObjectIEnumerableT)obj).Verify();
            }

            {
                var reader = new Utf8JsonReader(TestClassWithStringToPrimitiveDictionary.s_data, isFinalBlock: true, state: default);
                TestClassWithStringToPrimitiveDictionary testStruct = JsonSerializer.Deserialize<TestClassWithStringToPrimitiveDictionary>(ref reader);
                testStruct.Verify();

                reader = new Utf8JsonReader(TestClassWithStringToPrimitiveDictionary.s_data, isFinalBlock: true, state: default);
                object obj = JsonSerializer.Deserialize(ref reader, typeof(TestClassWithStringToPrimitiveDictionary));
                ((TestClassWithStringToPrimitiveDictionary)obj).Verify();
            }
        }

        [Fact]
        public static void ReadPartial()
        {
            byte[] utf8 = Encoding.UTF8.GetBytes("[1, 2, 3]");
            var reader = new Utf8JsonReader(utf8, isFinalBlock: true, state: default);
            reader.Read();
            int[] array = JsonSerializer.Deserialize<int[]>(ref reader);
            var expected = new int[3] { 1, 2, 3 };
            Assert.Equal(expected, array);

            reader = new Utf8JsonReader(utf8, isFinalBlock: true, state: default);
            reader.Read();
            object obj = JsonSerializer.Deserialize(ref reader, typeof(int[]));
            Assert.Equal(expected, obj);

            reader = new Utf8JsonReader(utf8, isFinalBlock: true, state: default);
            reader.Read();
            reader.Read();
            int number = JsonSerializer.Deserialize<int>(ref reader);
            Assert.Equal(1, number);

            reader = new Utf8JsonReader(utf8, isFinalBlock: true, state: default);
            reader.Read();
            reader.Read();
            obj = JsonSerializer.Deserialize(ref reader, typeof(int));
            Assert.Equal(1, obj);
        }

        [Theory]
        [InlineData("0,1")]
        [InlineData("0 1")]
        [InlineData("0 {}")]
        public static void TooMuchJson(string json)
        {
            byte[] jsonBytes = Encoding.UTF8.GetBytes(json);

            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<int>(json));
            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<int>(jsonBytes));
            Assert.Throws<JsonException>(() => JsonSerializer.DeserializeAsync<int>(new MemoryStream(jsonBytes)).Result);

            // Using a reader directly doesn't throw.
            Utf8JsonReader reader = new Utf8JsonReader(jsonBytes);
            JsonSerializer.Deserialize<int>(ref reader);
            Assert.Equal(1, reader.BytesConsumed);
        }

        [Theory]
        [InlineData("0/**/")]
        [InlineData("0 /**/")]
        public static void TooMuchJsonWithComments(string json)
        {
            byte[] jsonBytes = Encoding.UTF8.GetBytes(json);
            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<int>(json));
            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<int>(jsonBytes));
            Assert.Throws<JsonException>(() => JsonSerializer.DeserializeAsync<int>(new MemoryStream(jsonBytes)).Result);

            // Using a reader directly doesn't throw.
            Utf8JsonReader reader = new Utf8JsonReader(jsonBytes);
            JsonSerializer.Deserialize<int>(ref reader);
            Assert.Equal(1, reader.BytesConsumed);

            // Use JsonCommentHandling.Skip

            var options = new JsonSerializerOptions();
            options.ReadCommentHandling = JsonCommentHandling.Skip;
            JsonSerializer.Deserialize<int>(json, options);
            JsonSerializer.Deserialize<int>(jsonBytes, options);
            int result = JsonSerializer.DeserializeAsync<int>(new MemoryStream(jsonBytes), options).Result;

            // Using a reader directly doesn't throw.
            reader = new Utf8JsonReader(jsonBytes);
            JsonSerializer.Deserialize<int>(ref reader, options);
            Assert.Equal(1, reader.BytesConsumed);
        }

        [Theory]
        [InlineData("[")]
        [InlineData("[0")]
        [InlineData("[0,")]
        public static void TooLittleJsonForIntArray(string json)
        {
            byte[] jsonBytes = Encoding.UTF8.GetBytes(json);

            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<int[]>(json));
            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<int[]>(jsonBytes));
            Assert.Throws<JsonException>(() => JsonSerializer.DeserializeAsync<int[]>(new MemoryStream(jsonBytes)).Result);

            // Using a reader directly throws since it can't read full int[].
            Utf8JsonReader reader = new Utf8JsonReader(jsonBytes);
            try
            {
                JsonSerializer.Deserialize<int[]>(ref reader);
                Assert.True(false, "Expected exception.");
            }
            catch (JsonException) { }

            Assert.Equal(0, reader.BytesConsumed);
        }
    }
}
