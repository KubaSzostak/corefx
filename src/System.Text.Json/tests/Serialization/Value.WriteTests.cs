// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using Xunit;

namespace System.Text.Json.Serialization.Tests
{
    public static partial class ValueTests
    {
        [Fact]
        public static void WritePrimitives()
        {
            {
                Assert.Equal("\"AQID\"", JsonSerializer.ToString<byte[]>(new byte[] { 1, 2, 3 }));
            }

            {
                var stream = new MemoryStream();
                using var writer = new Utf8JsonWriter(stream);
                writer.WriteStartObject();
                writer.WritePropertyName(Encoding.UTF8.GetBytes("foo"));
                JsonSerializer.WriteValue<byte[]>(writer, new byte[] { 1, 2, 3 });
                writer.WriteEndObject();
                writer.Flush();

                Assert.Equal("{\"foo\":\"AQID\"}", Encoding.UTF8.GetString(stream.ToArray()));
            }

            {
                var stream = new MemoryStream();
                using var writer = new Utf8JsonWriter(stream);
                writer.WriteStartObject();
                JsonSerializer.WriteProperty<byte[]>(writer, JsonEncodedText.Encode(Encoding.UTF8.GetBytes("foo")), new byte[] { 1, 2, 3 });
                writer.WriteEndObject();
                writer.Flush();

                Assert.Equal("{\"foo\":\"AQID\"}", Encoding.UTF8.GetString(stream.ToArray()));
            }


            {
                string json = JsonSerializer.ToString(1);
                Assert.Equal("1", json);
            }

            {
                int? value = 1;
                string json = JsonSerializer.ToString(value);
                Assert.Equal("1", json);
            }

            {
                int? value = null;
                string json = JsonSerializer.ToString(value);
                Assert.Equal("null", json);
            }

            {
                Span<byte> json = JsonSerializer.ToUtf8Bytes(1);
                Assert.Equal(Encoding.UTF8.GetBytes("1"), json.ToArray());
            }

            {
                string json = JsonSerializer.ToString(long.MaxValue);
                Assert.Equal(long.MaxValue.ToString(), json);
            }

            {
                Span<byte> json = JsonSerializer.ToUtf8Bytes(long.MaxValue);
                Assert.Equal(Encoding.UTF8.GetBytes(long.MaxValue.ToString()), json.ToArray());
            }

            {
                string json = JsonSerializer.ToString("Hello");
                Assert.Equal(@"""Hello""", json);
            }

            {
                Span<byte> json = JsonSerializer.ToUtf8Bytes("Hello");
                Assert.Equal(Encoding.UTF8.GetBytes(@"""Hello"""), json.ToArray());
            }
        }
    }
}
