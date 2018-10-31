// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Memory.Tests;
using System.MemoryTests;
using System.Runtime.CompilerServices;
using Microsoft.Xunit.Performance;
using Xunit;

namespace System.Buffers.Tests
{
    public class Perf_ReadOnlySequence_First
    {
        private const int InnerCount = 100_000;

        [Benchmark(InnerIterationCount = InnerCount)]
        [InlineData(10_000, 100)]
        private static void Byte_Array(int bufSize, int bufOffset)
        {
            var buffer = new ReadOnlySequence<byte>(new byte[bufSize], bufOffset, bufSize - 2 * bufOffset);

            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        var reader = new BufferReader1<byte>(buffer);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = InnerCount)]
        [InlineData(10_000, 100)]
        private static void Byte_MemoryManager(int bufSize, int bufOffset)
        {
            var manager = new CustomMemoryForTest<byte>(new byte[bufSize], bufOffset, bufSize - 2 * bufOffset);
            var buffer = new ReadOnlySequence<byte>(manager.Memory);

            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        var reader = new BufferReader1<byte>(buffer);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = InnerCount)]
        [InlineData(10_000, 100)]
        private static void Byte_SingleSegment(int bufSize, int bufOffset)
        {
            var segment1 = new BufferSegment<byte>(new byte[bufSize]);
            var buffer = new ReadOnlySequence<byte>(segment1, bufOffset, segment1, bufSize - bufOffset);

            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        var reader = new BufferReader1<byte>(buffer);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = InnerCount)]
        [InlineData(10_000, 100)]
        private static void Byte_MultiSegment(int bufSize, int bufOffset)
        {
            var segment1 = new BufferSegment<byte>(new byte[bufSize / 10]);
            BufferSegment<byte> segment2 = segment1;
            for (int j = 0; j < 10; j++)
                segment2 = segment2.Append(new byte[bufSize / 10]);
            var buffer = new ReadOnlySequence<byte>(segment1, bufOffset, segment2, bufSize / 10 - bufOffset);

            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        var reader = new BufferReader1<byte>(buffer);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = InnerCount)]
        private static void Byte_Empty()
        {
            ReadOnlySequence<byte> buffer = ReadOnlySequence<byte>.Empty;

            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        var reader = new BufferReader1<byte>(buffer);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = InnerCount)]
        private static void Byte_Default()
        {
            ReadOnlySequence<byte> buffer = default;

            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        var reader = new BufferReader1<byte>(buffer);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = InnerCount)]
        [InlineData(10_000, 100)]
        private static void Char_Array(int bufSize, int bufOffset)
        {
            var buffer = new ReadOnlySequence<char>(new char[bufSize], bufOffset, bufSize - 2 * bufOffset);

            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        var reader = new BufferReader1<char>(buffer);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = InnerCount)]
        [InlineData(10_000, 100)]
        private static void Char_MemoryManager(int bufSize, int bufOffset)
        {
            var manager = new CustomMemoryForTest<char>(new char[bufSize], bufOffset, bufSize - 2 * bufOffset);
            var buffer = new ReadOnlySequence<char>(manager.Memory);

            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        var reader = new BufferReader1<char>(buffer);
                    }
                }
            }
        }


        [Benchmark(InnerIterationCount = InnerCount)]
        [InlineData(10_000, 100)]
        private static void Char_SingleSegment(int bufSize, int bufOffset)
        {
            var segment1 = new BufferSegment<char>(new char[bufSize]);
            var buffer = new ReadOnlySequence<char>(segment1, bufOffset, segment1, bufSize - bufOffset);

            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        var reader = new BufferReader1<char>(buffer);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = InnerCount)]
        [InlineData(10_000, 100)]
        private static void Char_MultiSegment(int bufSize, int bufOffset)
        {
            var segment1 = new BufferSegment<char>(new char[bufSize / 10]);
            BufferSegment<char> segment2 = segment1;
            for (int j = 0; j < 10; j++)
                segment2 = segment2.Append(new char[bufSize / 10]);

            var buffer = new ReadOnlySequence<char>(segment1, bufOffset, segment2, bufSize / 10 - bufOffset);

            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        var reader = new BufferReader1<char>(buffer);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = InnerCount)]
        [InlineData(10_000, 100)]
        private static void String(int bufSize, int bufOffset)
        {
            ReadOnlyMemory<char> memory = new string('a', bufSize).AsMemory();
            memory = memory.Slice(bufOffset, bufSize - 2 * bufOffset);
            var buffer = new ReadOnlySequence<char>(memory);

            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        var reader = new BufferReader1<char>(buffer);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = InnerCount)]
        private static void Char_Empty()
        {
            ReadOnlySequence<char> buffer = ReadOnlySequence<char>.Empty;

            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        var reader = new BufferReader1<char>(buffer);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = InnerCount)]
        private static void Char_Default()
        {
            ReadOnlySequence<char> buffer = default;

            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        var reader = new BufferReader1<char>(buffer);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = InnerCount)]
        [InlineData(10_000, 100)]
        private static void zByte_Array_2(int bufSize, int bufOffset)
        {
            var buffer = new ReadOnlySequence<byte>(new byte[bufSize], bufOffset, bufSize - 2 * bufOffset);

            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        var reader = new BufferReader2<byte>(buffer);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = InnerCount)]
        [InlineData(10_000, 100)]
        private static void zByte_MemoryManager_2(int bufSize, int bufOffset)
        {
            var manager = new CustomMemoryForTest<byte>(new byte[bufSize], bufOffset, bufSize - 2 * bufOffset);
            var buffer = new ReadOnlySequence<byte>(manager.Memory);

            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        var reader = new BufferReader2<byte>(buffer);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = InnerCount)]
        [InlineData(10_000, 100)]
        private static void zByte_SingleSegment_2(int bufSize, int bufOffset)
        {
            var segment1 = new BufferSegment<byte>(new byte[bufSize]);
            var buffer = new ReadOnlySequence<byte>(segment1, bufOffset, segment1, bufSize - bufOffset);

            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        var reader = new BufferReader2<byte>(buffer);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = InnerCount)]
        [InlineData(10_000, 100)]
        private static void zByte_MultiSegment_2(int bufSize, int bufOffset)
        {
            var segment1 = new BufferSegment<byte>(new byte[bufSize / 10]);
            BufferSegment<byte> segment2 = segment1;
            for (int j = 0; j < 10; j++)
                segment2 = segment2.Append(new byte[bufSize / 10]);
            var buffer = new ReadOnlySequence<byte>(segment1, bufOffset, segment2, bufSize / 10 - bufOffset);

            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        var reader = new BufferReader2<byte>(buffer);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = InnerCount)]
        private static void zByte_Empty_2()
        {
            ReadOnlySequence<byte> buffer = ReadOnlySequence<byte>.Empty;

            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        var reader = new BufferReader2<byte>(buffer);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = InnerCount)]
        private static void zByte_Default_2()
        {
            ReadOnlySequence<byte> buffer = default;

            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        var reader = new BufferReader2<byte>(buffer);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = InnerCount)]
        [InlineData(10_000, 100)]
        private static void zChar_Array_2(int bufSize, int bufOffset)
        {
            var buffer = new ReadOnlySequence<char>(new char[bufSize], bufOffset, bufSize - 2 * bufOffset);

            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        var reader = new BufferReader2<char>(buffer);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = InnerCount)]
        [InlineData(10_000, 100)]
        private static void zChar_MemoryManager_2(int bufSize, int bufOffset)
        {
            var manager = new CustomMemoryForTest<char>(new char[bufSize], bufOffset, bufSize - 2 * bufOffset);
            var buffer = new ReadOnlySequence<char>(manager.Memory);

            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        var reader = new BufferReader2<char>(buffer);
                    }
                }
            }
        }


        [Benchmark(InnerIterationCount = InnerCount)]
        [InlineData(10_000, 100)]
        private static void zChar_SingleSegment_2(int bufSize, int bufOffset)
        {
            var segment1 = new BufferSegment<char>(new char[bufSize]);
            var buffer = new ReadOnlySequence<char>(segment1, bufOffset, segment1, bufSize - bufOffset);

            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        var reader = new BufferReader2<char>(buffer);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = InnerCount)]
        [InlineData(10_000, 100)]
        private static void zChar_MultiSegment_2(int bufSize, int bufOffset)
        {
            var segment1 = new BufferSegment<char>(new char[bufSize / 10]);
            BufferSegment<char> segment2 = segment1;
            for (int j = 0; j < 10; j++)
                segment2 = segment2.Append(new char[bufSize / 10]);

            var buffer = new ReadOnlySequence<char>(segment1, bufOffset, segment2, bufSize / 10 - bufOffset);

            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        var reader = new BufferReader2<char>(buffer);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = InnerCount)]
        [InlineData(10_000, 100)]
        private static void zString_2(int bufSize, int bufOffset)
        {
            ReadOnlyMemory<char> memory = new string('a', bufSize).AsMemory();
            memory = memory.Slice(bufOffset, bufSize - 2 * bufOffset);
            var buffer = new ReadOnlySequence<char>(memory);

            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        var reader = new BufferReader2<char>(buffer);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = InnerCount)]
        private static void zChar_Empty_2()
        {
            ReadOnlySequence<char> buffer = ReadOnlySequence<char>.Empty;

            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        var reader = new BufferReader2<char>(buffer);
                    }
                }
            }
        }

        [Benchmark(InnerIterationCount = InnerCount)]
        private static void zChar_Default_2()
        {
            ReadOnlySequence<char> buffer = default;

            foreach (BenchmarkIteration iteration in Benchmark.Iterations)
            {
                using (iteration.StartMeasurement())
                {
                    for (int i = 0; i < Benchmark.InnerIterationCount; i++)
                    {
                        var reader = new BufferReader2<char>(buffer);
                    }
                }
            }
        }
    }

    public ref partial struct BufferReader1<T> where T : unmanaged, IEquatable<T>
    {
        private SequencePosition _currentPosition;
        private SequencePosition _nextPosition;
        private bool _moreData;

        /// <summary>
        /// The underlying <see cref="ReadOnlySequence{T}"/> for the reader.
        /// </summary>
        public ReadOnlySequence<T> Sequence { get; }

        /// <summary>
        /// The current segment in the <see cref="Sequence"/>.
        /// </summary>
        public ReadOnlySpan<T> CurrentSpan { get; private set; }

        /// <summary>
        /// The index in the <see cref="CurrentSpan"/>.
        /// </summary>
        public int CurrentSpanIndex { get; private set; }

        /// <summary>
        /// The total number of {T}s processed by the reader.
        /// </summary>
        public int Consumed { get; private set; }

        public BufferReader1(ReadOnlySequence<T> buffer)
        {
            CurrentSpanIndex = 0;
            Consumed = 0;
            Sequence = buffer;
            _currentPosition = buffer.Start;
            _nextPosition = default;
            CurrentSpan = buffer.First.Span;
            _moreData = CurrentSpan.Length > 0;

            if (!buffer.IsSingleSegment)
            {
                ReadOnlySequenceSegment<T> segment = Unsafe.As<ReadOnlySequenceSegment<T>>(_currentPosition.GetObject());
                _nextPosition = new SequencePosition(segment.Next, 0);

                if (!_moreData)
                {
                    _moreData = true;
                    GetNextSpan();
                }
            }
        }

        /// <summary>
        /// Get the next segment with available space, if any.
        /// </summary>
        private void GetNextSpan()
        {
            if (!Sequence.IsSingleSegment)
            {
                SequencePosition previousNextPosition = _nextPosition;
                while (Sequence.TryGet(ref _nextPosition, out ReadOnlyMemory<T> memory, advance: true))
                {
                    _currentPosition = previousNextPosition;
                    if (memory.Length > 0)
                    {
                        CurrentSpan = memory.Span;
                        CurrentSpanIndex = 0;
                        return;
                    }
                    else
                    {
                        CurrentSpan = default;
                        CurrentSpanIndex = 0;
                        previousNextPosition = _nextPosition;
                    }
                }
            }
            _moreData = false;
        }
    }

    public ref partial struct BufferReader2<T> where T : unmanaged, IEquatable<T>
    {
        private SequencePosition _currentPosition;
        private SequencePosition _nextPosition;
        private bool _moreData;

        /// <summary>
        /// The underlying <see cref="ReadOnlySequence{T}"/> for the reader.
        /// </summary>
        public ReadOnlySequence<T> Sequence { get; }

        /// <summary>
        /// The current segment in the <see cref="Sequence"/>.
        /// </summary>
        public ReadOnlySpan<T> CurrentSpan { get; private set; }

        /// <summary>
        /// The index in the <see cref="CurrentSpan"/>.
        /// </summary>
        public int CurrentSpanIndex { get; private set; }

        /// <summary>
        /// The total number of {T}s processed by the reader.
        /// </summary>
        public int Consumed { get; private set; }

        public BufferReader2(ReadOnlySequence<T> buffer)
        {
            CurrentSpanIndex = 0;
            Consumed = 0;
            Sequence = buffer;
            _currentPosition = buffer.Start;
            _nextPosition = default;
            CurrentSpan = buffer.FirstSpan;
            _moreData = CurrentSpan.Length > 0;

            if (!buffer.IsSingleSegment)
            {
                ReadOnlySequenceSegment<T> segment = Unsafe.As<ReadOnlySequenceSegment<T>>(_currentPosition.GetObject());
                _nextPosition = new SequencePosition(segment.Next, 0);

                if (!_moreData)
                {
                    _moreData = true;
                    GetNextSpan();
                }
            }
        }

        /// <summary>
        /// Get the next segment with available space, if any.
        /// </summary>
        private void GetNextSpan()
        {
            if (!Sequence.IsSingleSegment)
            {
                SequencePosition previousNextPosition = _nextPosition;
                while (Sequence.TryGet(ref _nextPosition, out ReadOnlyMemory<T> memory, advance: true))
                {
                    _currentPosition = previousNextPosition;
                    if (memory.Length > 0)
                    {
                        CurrentSpan = memory.Span;
                        CurrentSpanIndex = 0;
                        return;
                    }
                    else
                    {
                        CurrentSpan = default;
                        CurrentSpanIndex = 0;
                        previousNextPosition = _nextPosition;
                    }
                }
            }
            _moreData = false;
        }
    }
}
