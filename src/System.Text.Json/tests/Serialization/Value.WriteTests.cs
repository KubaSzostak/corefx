// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Text.Encodings.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Xunit;

namespace System.Text.Json.Serialization.Tests
{
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

        public class Temp
        {
            public Dictionary<string, string> Dictionary { get; set; }
        }

        [Theory]
        //[InlineData("lowerCase")]
        //[InlineData("UpperCase")]
        //[InlineData("UPperCase")]
        //[InlineData("lower Case")]
        //[InlineData("loweR Case")]
        //[InlineData("Upper Case")]
        //[InlineData("UppeR Case")]
        //[InlineData("lower case")]
        //[InlineData("loweR case")]
        //[InlineData("Upper case")]
        //[InlineData("UppeR case")]
        //[InlineData("lower cASe")]
        //[InlineData("loweR cASe")]
        //[InlineData("Upper cASe")]
        //[InlineData("UppeR cASe")]
        //[InlineData("UPper Case")]
        //[InlineData("UPpeR Case")]
        //[InlineData("UPper case")]
        //[InlineData("UPpeR case")]
        //[InlineData("UPper cASe")]
        //[InlineData("UPpeR cASe")]
        //[InlineData("IPhone")]
        [InlineData("IPHone")]
        //[InlineData("iPhone")]

        //[InlineData("i")]
        //[InlineData("I")]
        //[InlineData("i ")]
        //[InlineData("I ")]

        //[InlineData("ii")]
        //[InlineData("II")]
        //[InlineData("iI")]
        //[InlineData("Ii")]
        //[InlineData("ii ")]
        //[InlineData("II ")]
        //[InlineData("iI ")]
        //[InlineData("Ii ")]

        //[InlineData("iii")]
        //[InlineData("III")]
        //[InlineData("iiI")]
        //[InlineData("iIi")]
        //[InlineData("Iii")]
        //[InlineData("iII")]
        //[InlineData("IIi")]
        //[InlineData("IiI")]
        //[InlineData("iii ")]
        //[InlineData("III ")]
        //[InlineData("iiI ")]
        //[InlineData("iIi ")]
        //[InlineData("Iii ")]
        //[InlineData("iII ")]
        //[InlineData("IIi ")]
        //[InlineData("IiI ")]

        //[InlineData("IPhone")]
        //[InlineData("IPHone")]
        //[InlineData("IPH one")]
        //[InlineData("IPH One")]
        //[InlineData("IPHONe")]
        //[InlineData("IPHON e")]
        //[InlineData("IPHON E")]
        //[InlineData("IPHONE")]
        public static void TestCasing(string baseString)
        {
            var testObject = new Temp
            {
                Dictionary = new Dictionary<string, string>
                {
                    { baseString, "hi" }
                }
            };

            string json = JsonSerializer.Serialize<Temp>(testObject, new JsonSerializerOptions { DictionaryKeyPolicy = JsonNamingPolicy.CamelCase });
            Console.WriteLine("STJ: " + json);

            testObject = JsonSerializer.Deserialize<Temp>(json);
            json = JsonSerializer.Serialize(testObject);
            Console.WriteLine("STJ-round: " + json);

            DefaultContractResolver contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };

            string expected = JsonConvert.SerializeObject(testObject, new JsonSerializerSettings { ContractResolver = contractResolver });
            Console.WriteLine(expected);

            char[] baseStringArray = baseString.ToCharArray();
            char[] testCharArray = new char[baseStringArray.Length + 2];
            for (int i = 0; i <= baseStringArray.Length; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    testCharArray[j] = baseStringArray[j];
                }
                testCharArray[i] = '\uD835';
                testCharArray[i + 1] = '\uDD80';
                //testCharArray[i] = '\uD801';
                //testCharArray[i + 1] = '\uDC27';
                for (int j = i; j < baseStringArray.Length; j++)
                {
                    testCharArray[j + 2] = baseStringArray[j];
                }

                string testString = new string(testCharArray);

                testObject = new Temp
                {
                    Dictionary = new Dictionary<string, string>
                    {
                        { testString, "hi" }
                    }
                };

                json = JsonSerializer.Serialize<Temp>(testObject, new JsonSerializerOptions { DictionaryKeyPolicy = JsonNamingPolicy.CamelCase });
                Console.WriteLine("STJ: " + json);

                testObject = JsonSerializer.Deserialize<Temp>(json);
                json = JsonSerializer.Serialize(testObject);
                Console.WriteLine("STJ-round: " + json);

                expected = JsonConvert.SerializeObject(testObject, new JsonSerializerSettings { ContractResolver = contractResolver });
                Console.WriteLine(expected);
            }

            baseStringArray = baseString.ToCharArray();
            testCharArray = new char[baseStringArray.Length + 1];
            for (int i = 0; i <= baseStringArray.Length; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    testCharArray[j] = baseStringArray[j];
                }
                testCharArray[i] = '\uD835';
                for (int j = i; j < baseStringArray.Length; j++)
                {
                    testCharArray[j + 1] = baseStringArray[j];
                }

                string testString = new string(testCharArray);

                testObject = new Temp
                {
                    Dictionary = new Dictionary<string, string>
                    {
                        { testString, "hi" }
                    }
                };

                json = JsonSerializer.Serialize<Temp>(testObject, new JsonSerializerOptions { DictionaryKeyPolicy = JsonNamingPolicy.CamelCase });
                Console.WriteLine("STJ: " + json);

                testObject = JsonSerializer.Deserialize<Temp>(json);
                json = JsonSerializer.Serialize(testObject);
                Console.WriteLine("STJ-round: " + json);

                expected = JsonConvert.SerializeObject(testObject, new JsonSerializerSettings { ContractResolver = contractResolver });
                Console.WriteLine(expected);
            }
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
