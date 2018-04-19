// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace System.Memory.Tests
{
    public class ReadOnlySequenceTestsCommonByte: ReadOnlySequenceTestsCommon<byte>
    {
        #region Constructor 

        [Fact]
        public void Ctor_Array_Offset()
        {
            var buffer = new ReadOnlySequence<byte>(new byte[] { 1, 2, 3, 4, 5 }, 2, 3);
            Assert.Equal(buffer.ToArray(), new byte[] { 3, 4, 5 });
        }

        [Fact]
        public void Ctor_Array_NoOffset()
        {
            var buffer = new ReadOnlySequence<byte>(new byte[] { 1, 2, 3, 4, 5 });
            Assert.Equal(buffer.ToArray(), new byte[] { 1, 2, 3, 4, 5 });
        }

        [Fact]
        public void Ctor_Memory()
        {
            var memory = new ReadOnlyMemory<byte>(new byte[] { 1, 2, 3, 4, 5 });
            var buffer = new ReadOnlySequence<byte>(memory.Slice(2, 3));
            Assert.Equal(new byte[] { 3, 4, 5 }, buffer.ToArray());
        }

        #endregion

        [Fact]
        public void HelloWorldAcrossTwoBlocks()
        {
            //     block 1       ->    block2
            // [padding..hello]  ->  [  world   ]
            const int blockSize = 4096;

            byte[] items = Encoding.ASCII.GetBytes("Hello World");
            byte[] firstItems = Enumerable.Repeat((byte)'a', blockSize - 5).Concat(items.Take(5)).ToArray();
            byte[] secondItems = items.Skip(5).Concat(Enumerable.Repeat((byte)'a', blockSize - (items.Length - 5))).ToArray();

            var firstSegment = new BufferSegment<byte>(firstItems);
            BufferSegment<byte> secondSegment = firstSegment.Append(secondItems);

            var buffer = new ReadOnlySequence<byte>(firstSegment, 0, secondSegment, items.Length - 5);
            Assert.False(buffer.IsSingleSegment);
            ReadOnlySequence<byte> helloBuffer = buffer.Slice(blockSize - 5);
            Assert.False(helloBuffer.IsSingleSegment);
            var memory = new List<ReadOnlyMemory<byte>>();
            foreach (ReadOnlyMemory<byte> m in helloBuffer)
            {
                memory.Add(m);
            }

            List<ReadOnlyMemory<byte>> spans = memory;

            Assert.Equal(2, memory.Count);
            var helloBytes = new byte[spans[0].Length];
            spans[0].Span.CopyTo(helloBytes);
            var worldBytes = new byte[spans[1].Length];
            spans[1].Span.CopyTo(worldBytes);
            Assert.Equal("Hello", Encoding.ASCII.GetString(helloBytes));
            Assert.Equal(" World", Encoding.ASCII.GetString(worldBytes));
        }

        [Fact]
        public static void SliceStartPositionAndLength()
        {
            var segment1 = new BufferSegment<byte>(new byte[10]);
            BufferSegment<byte> segment2 = segment1.Append(new byte[10]);

            var buffer = new ReadOnlySequence<byte>(segment1, 0, segment2, 10);

            ReadOnlySequence<byte> sliced = buffer.Slice(buffer.GetPosition(10), 10);
            Assert.Equal(10, sliced.Length);

            Assert.Equal(segment2, sliced.Start.GetObject());
            Assert.Equal(segment2, sliced.End.GetObject());

            Assert.Equal(0, sliced.Start.GetInteger());
            Assert.Equal(10, sliced.End.GetInteger());
        }

        [Fact]
        public static void SliceMismatchSequencePositions()
        {
            var segment1 = new BufferSegment<byte>(new byte[10]);
            BufferSegment<byte> segment2 = segment1.Append(new byte[10]);

            var buffer1 = new ReadOnlySequence<byte>(segment1, 0, segment1, 10);
            var buffer2 = new ReadOnlySequence<byte>(segment1, 0, segment2, 10);

            // mismatch of SequencePositions from different buffers is not supported
            ReadOnlySequence<byte> sliced = buffer2.Slice(10, buffer1.End);

            // This mismatch will happen, what should happen?
            Assert.Equal(segment2, sliced.Start.GetObject());
            // This mismatch will happen, what should happen?
            Assert.Equal(segment1, sliced.End.GetObject());

            // This is valid and passes as expected
            sliced = buffer1.Slice(10, buffer1.End);

            Assert.Equal(segment1, sliced.Start.GetObject());
            Assert.Equal(segment1, sliced.End.GetObject());

            // This is valid and passes as expected
            sliced = buffer2.Slice(10, buffer2.End);

            Assert.Equal(segment2, sliced.Start.GetObject());
            Assert.Equal(segment2, sliced.End.GetObject());
        }

        [Fact]
        public static void SliceSequencePositionComparison()
        {
            var segment1 = new BufferSegment<byte>(new byte[0]);
            BufferSegment<byte> segment2 = segment1.Append(new byte[10]);

            var buffer = new ReadOnlySequence<byte>(segment1, 0, segment2, 10);

            ReadOnlySequence<byte> sliced = buffer.Slice(0, buffer.Start);

            // This fails, sliced.Start.GetObject() == segment2 instead of segment1, currently
            // This is a known issue related to SequencePosition comparisons
            Assert.Equal(segment1, sliced.Start.GetObject());
            Assert.Equal(segment1, sliced.End.GetObject());

            Assert.Equal(0, sliced.Start.GetInteger());
            Assert.Equal(0, sliced.End.GetInteger());
        }

        [Fact]
        public static void SliceAllOperationsAreEqual()
        {
            var segment1 = new BufferSegment<byte>(new byte[10]);
            BufferSegment<byte> segment2 = segment1.Append(new byte[10]);

            var buffer = new ReadOnlySequence<byte>(segment1, 0, segment2, 10);

            for (int n = 0; n < 10; n++)
            {
                SequencePosition pos = buffer.GetPosition(n);
                ReadOnlySequence<byte> s1 = buffer.Slice(pos);
                ReadOnlySequence<byte> s2 = buffer.Slice(pos, buffer.End);
                ReadOnlySequence<byte> s3 = buffer.Slice(pos, buffer.Length - n);

                Assert.Equal(segment1, s1.Start.GetObject());
                Assert.Equal(segment2, s1.End.GetObject());

                Assert.Equal(n, s1.Start.GetInteger());
                Assert.Equal(10, s1.End.GetInteger());

                Assert.Equal(segment1, s2.Start.GetObject());
                Assert.Equal(segment2, s2.End.GetObject());

                Assert.Equal(n, s2.Start.GetInteger());
                Assert.Equal(10, s2.End.GetInteger());

                Assert.Equal(segment1, s3.Start.GetObject());
                Assert.Equal(segment2, s3.End.GetObject());

                Assert.Equal(n, s3.Start.GetInteger());
                Assert.Equal(10, s3.End.GetInteger());
            }

            for (int n = 10; n <= 20; n++)
            {
                SequencePosition pos = buffer.GetPosition(n);
                ReadOnlySequence<byte> s1 = buffer.Slice(pos);
                ReadOnlySequence<byte> s2 = buffer.Slice(pos, buffer.End);
                ReadOnlySequence<byte> s3 = buffer.Slice(pos, buffer.Length - n);

                Assert.Equal(segment2, s1.Start.GetObject());
                Assert.Equal(segment2, s1.End.GetObject());

                Assert.Equal(n - 10, s1.Start.GetInteger());
                Assert.Equal(10, s1.End.GetInteger());

                Assert.Equal(segment2, s2.Start.GetObject());
                Assert.Equal(segment2, s2.End.GetObject());

                Assert.Equal(n - 10, s2.Start.GetInteger());
                Assert.Equal(10, s2.End.GetInteger());

                Assert.Equal(segment2, s3.Start.GetObject());
                Assert.Equal(segment2, s3.End.GetObject());

                Assert.Equal(n - 10, s3.Start.GetInteger());
                Assert.Equal(10, s3.End.GetInteger());
            }
        }


        [Fact]
        public static void SliceStartAndEndPosition()
        {
            var segment1 = new BufferSegment<byte>(new byte[10]);
            BufferSegment<byte> segment2 = segment1.Append(new byte[10]);

            var buffer = new ReadOnlySequence<byte>(segment1, 0, segment2, 10);

            ReadOnlySequence<byte> sliced = buffer.Slice(10, buffer.GetPosition(20));
            Assert.Equal(10, sliced.Length);

            Assert.Equal(segment2, sliced.Start.GetObject());
            Assert.Equal(segment2, sliced.End.GetObject());

            Assert.Equal(0, sliced.Start.GetInteger());
            Assert.Equal(10, sliced.End.GetInteger());
        }
    }
}
