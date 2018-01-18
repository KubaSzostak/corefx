// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Xunit.Performance;
using Xunit;

namespace System.Memory.Tests
{
    public class Perf_Span_Fill
    {
        /*//[Benchmark]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(6)]
        [InlineData(7)]
        [InlineData(8)]
        [InlineData(9)]
        [InlineData(10)]
        [InlineData(11)]
        [InlineData(12)]
        [InlineData(13)]
        [InlineData(14)]
        [InlineData(15)]
        [InlineData(16)]
        [InlineData(32)]
        [InlineData(64)]
        [InlineData(100)]
        [InlineData(1000)]
        [InlineData(10000)]
        public void ByteSpan(int size)
        {
            var a = new byte[size];
            var span = new Span<byte>(a);
            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < 10000; i++)
                    {
                        span.Reverse();
                    }
                }
            }
        }


        //[Benchmark]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(6)]
        [InlineData(7)]
        [InlineData(8)]
        [InlineData(9)]
        [InlineData(10)]
        [InlineData(11)]
        [InlineData(12)]
        [InlineData(13)]
        [InlineData(14)]
        [InlineData(15)]
        [InlineData(16)]
        [InlineData(32)]
        [InlineData(64)]
        [InlineData(100)]
        [InlineData(1000)]
        [InlineData(10000)]
        public void IntSpan(int size)
        {
            var a = new int[size];
            var span = new Span<int>(a);
            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < 10000; i++)
                    {
                        span.Reverse();
                    }
                }
            }
        }*/

        [Benchmark]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(4)]
        [InlineData(8)]
        [InlineData(16)]
        [InlineData(32)]
        [InlineData(64)]
        [InlineData(100)]
        [InlineData(1000)]
        [InlineData(10000)]
        public void ByteSpan1(int size)
        {
            var a = new byte[size];
            var span = new Span<byte>(a);
            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < 10000; i++)
                    {
                        span.Reverse1();
                    }
                }
            }
        }

        [Benchmark]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(4)]
        [InlineData(8)]
        [InlineData(16)]
        [InlineData(32)]
        [InlineData(64)]
        [InlineData(100)]
        [InlineData(1000)]
        [InlineData(10000)]
        public void ByteSpan2(int size)
        {
            var a = new byte[size];
            var span = new Span<byte>(a);
            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < 10000; i++)
                    {
                        span.Reverse2();
                    }
                }
            }
        }

        [Benchmark]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(4)]
        [InlineData(8)]
        [InlineData(16)]
        [InlineData(32)]
        [InlineData(64)]
        [InlineData(100)]
        [InlineData(1000)]
        [InlineData(10000)]
        public void GuidSpan1(int size)
        {
            var a = new Guid[size];
            var span = new Span<Guid>(a);
            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < 10000; i++)
                    {
                        span.Reverse1();
                    }
                }
            }
        }

        [Benchmark]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(4)]
        [InlineData(8)]
        [InlineData(16)]
        [InlineData(32)]
        [InlineData(64)]
        [InlineData(100)]
        [InlineData(1000)]
        [InlineData(10000)]
        public void GuidSpan2(int size)
        {
            var a = new Guid[size];
            var span = new Span<Guid>(a);
            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < 10000; i++)
                    {
                        span.Reverse2();
                    }
                }
            }
        }

        //[Benchmark]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(4)]
        [InlineData(8)]
        [InlineData(16)]
        [InlineData(32)]
        [InlineData(64)]
        [InlineData(100)]
        [InlineData(1000)]
        [InlineData(10000)]
        public void GuidSpan3(int size)
        {
            var a = new Guid[size];
            var span = new Span<Guid>(a);
            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < 10000; i++)
                    {
                        span.Reverse3();
                    }
                }
            }
        }

        //[Benchmark]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(4)]
        [InlineData(8)]
        [InlineData(16)]
        [InlineData(32)]
        [InlineData(64)]
        [InlineData(100)]
        [InlineData(1000)]
        [InlineData(10000)]
        public void GuidSpan4(int size)
        {
            var a = new Guid[size];
            var span = new Span<Guid>(a);
            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < 10000; i++)
                    {
                        span.Reverse4();
                    }
                }
            }
        }


        [Benchmark]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(4)]
        [InlineData(8)]
        [InlineData(16)]
        [InlineData(32)]
        [InlineData(64)]
        [InlineData(100)]
        [InlineData(1000)]
        [InlineData(10000)]
        public void StringSpan1(int size)
        {
            var a = new string[size];
            var span = new Span<string>(a);
            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < 10000; i++)
                    {
                        span.Reverse1();
                    }
                }
            }
        }

        [Benchmark]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(4)]
        [InlineData(8)]
        [InlineData(16)]
        [InlineData(32)]
        [InlineData(64)]
        [InlineData(100)]
        [InlineData(1000)]
        [InlineData(10000)]
        public void StringSpan2(int size)
        {
            var a = new string[size];
            var span = new Span<string>(a);
            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < 10000; i++)
                    {
                        span.Reverse2();
                    }
                }
            }
        }

        //[Benchmark]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(4)]
        [InlineData(8)]
        [InlineData(16)]
        [InlineData(32)]
        [InlineData(64)]
        [InlineData(100)]
        [InlineData(1000)]
        [InlineData(10000)]
        public void StringSpan3(int size)
        {
            var a = new string[size];
            var span = new Span<string>(a);
            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < 10000; i++)
                    {
                        span.Reverse3();
                    }
                }
            }
        }

        //[Benchmark]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(4)]
        [InlineData(8)]
        [InlineData(16)]
        [InlineData(32)]
        [InlineData(64)]
        [InlineData(100)]
        [InlineData(1000)]
        [InlineData(10000)]
        public void StringSpan4(int size)
        {
            var a = new string[size];
            var span = new Span<string>(a);
            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < 10000; i++)
                    {
                        span.Reverse4();
                    }
                }
            }
        }

        /*//[Benchmark]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(6)]
        [InlineData(7)]
        [InlineData(8)]
        [InlineData(9)]
        [InlineData(10)]
        [InlineData(11)]
        [InlineData(12)]
        [InlineData(13)]
        [InlineData(14)]
        [InlineData(15)]
        [InlineData(16)]
        [InlineData(32)]
        [InlineData(64)]
        [InlineData(100)]
        [InlineData(1000)]
        [InlineData(10000)]
        public void StringSpan(int size)
        {
            var a = new string[size];
            var span = new Span<string>(a);
            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < 10000; i++)
                    {
                        span.Reverse();
                    }
                }
            }
        }


        //[Benchmark]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(6)]
        [InlineData(7)]
        [InlineData(8)]
        [InlineData(9)]
        [InlineData(10)]
        [InlineData(11)]
        [InlineData(12)]
        [InlineData(13)]
        [InlineData(14)]
        [InlineData(15)]
        [InlineData(16)]
        [InlineData(32)]
        [InlineData(64)]
        [InlineData(100)]
        [InlineData(1000)]
        [InlineData(10000)]
        public void ByteArray(int size)
        {
            var a = new byte[size];
            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < 10000; i++)
                    {
                        Array.Reverse(a);
                    }
                }
            }
        }


        //[Benchmark]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(6)]
        [InlineData(7)]
        [InlineData(8)]
        [InlineData(9)]
        [InlineData(10)]
        [InlineData(11)]
        [InlineData(12)]
        [InlineData(13)]
        [InlineData(14)]
        [InlineData(15)]
        [InlineData(16)]
        [InlineData(32)]
        [InlineData(64)]
        [InlineData(100)]
        [InlineData(1000)]
        [InlineData(10000)]
        public void IntArray(int size)
        {
            var a = new int[size];
            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < 10000; i++)
                    {
                        Array.Reverse(a);
                    }
                }
            }
        }

        //[Benchmark]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(6)]
        [InlineData(7)]
        [InlineData(8)]
        [InlineData(9)]
        [InlineData(10)]
        [InlineData(11)]
        [InlineData(12)]
        [InlineData(13)]
        [InlineData(14)]
        [InlineData(15)]
        [InlineData(16)]
        [InlineData(32)]
        [InlineData(64)]
        [InlineData(100)]
        [InlineData(1000)]
        [InlineData(10000)]
        public void GuidArray(int size)
        {
            var a = new Guid[size];
            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < 10000; i++)
                    {
                        Array.Reverse(a);
                    }
                }
            }
        }

        //[Benchmark]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(6)]
        [InlineData(7)]
        [InlineData(8)]
        [InlineData(9)]
        [InlineData(10)]
        [InlineData(11)]
        [InlineData(12)]
        [InlineData(13)]
        [InlineData(14)]
        [InlineData(15)]
        [InlineData(16)]
        [InlineData(32)]
        [InlineData(64)]
        [InlineData(100)]
        [InlineData(1000)]
        [InlineData(10000)]
        public void StringArray(int size)
        {
            var a = new string[size];
            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < 10000; i++)
                    {
                        Array.Reverse(a);
                    }
                }
            }
        }*/
    }
}
