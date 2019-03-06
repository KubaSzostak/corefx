// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.IO.Pipelines;
using System.Threading.Tasks;
using Xunit;

namespace System.Text.Json.Serialization.Tests
{
    public partial class PipeTests : IDisposable
    {
        public PipeTests()
        {
            _pipe = new Pipe(new PipeOptions());
        }

        public void Dispose()
        {
            _pipe.Writer.Complete();
            _pipe.Reader.Complete();
        }

        private Pipe _pipe;

        [Fact]
        public async Task ReadSimpleObjectPipeAsync()
        {

            //using (MemoryStream stream = new MemoryStream(SimpleTestClass.s_data))
            //{
            //    JsonSerializerOptions options = new JsonSerializerOptions
            //    {
            //        DefaultBufferSize = 1
            //    };

            //    for (int i = 0; i < 100_000; i++)
            //    {
            //        stream.Seek(0, SeekOrigin.Begin);
            //        SimpleTestClass obj = await JsonSerializer.ReadAsync<SimpleTestClass>(stream, options);
            //        obj.Verify();
            //    }
            //}

            byte[] bytes = SimpleTestClass.s_data;

            await _pipe.Writer.WriteAsync(bytes);
            _pipe.Writer.Complete();

            JsonSerializerOptions options = new JsonSerializerOptions
            {
                DefaultBufferSize = 1
            };

            for (int i = 0; i < 100_000; i++)
            {
                _pipe = new Pipe(new PipeOptions());
                await _pipe.Writer.WriteAsync(bytes);
                _pipe.Writer.Complete();
                SimpleTestClass obj = await JsonSerializer.ReadAsync<SimpleTestClass>(_pipe.Reader, options);
                obj.Verify();
            }
        }
    }
}
