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

        private readonly Pipe _pipe;

        [Fact]
        public async Task ReadSimpleObjectPipeAsync()
        {
            byte[] bytes = SimpleTestClass.s_data;

            await _pipe.Writer.WriteAsync(bytes);
            await _pipe.Writer.FlushAsync();

            JsonSerializerOptions options = new JsonSerializerOptions
            {
                DefaultBufferSize = 1
            };

            SimpleTestClass obj = await JsonSerializer.ReadAsync<SimpleTestClass>(_pipe.Reader, options);
            obj.Verify();
        }
    }
}
