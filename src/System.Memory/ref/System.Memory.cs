// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------

namespace System
{
    public readonly partial struct SequencePosition : System.IEquatable<System.SequencePosition>
    {
        private readonly object? _dummy;
        public SequencePosition(object? @object, int integer) { throw null; }
        [System.ComponentModel.EditorBrowsableAttribute(1)]
        public override bool Equals(object? obj) { throw null; }
        public bool Equals(System.SequencePosition other) { throw null; }
        [System.ComponentModel.EditorBrowsableAttribute(1)]
        public override int GetHashCode() { throw null; }
        [System.ComponentModel.EditorBrowsableAttribute(1)]
        public int GetInteger() { throw null; }
        [System.ComponentModel.EditorBrowsableAttribute(1)]
        public object? GetObject() { throw null; }
    }
}
namespace System.Buffers
{
    public sealed partial class ArrayBufferWriter<T> : System.Buffers.IBufferWriter<T>
    {
        public ArrayBufferWriter() { }
        public ArrayBufferWriter(int initialCapacity) { }
        public int Capacity { get { throw null; } }
        public int FreeCapacity { get { throw null; } }
        public int WrittenCount { get { throw null; } }
        public System.ReadOnlyMemory<T> WrittenMemory { get { throw null; } }
        public System.ReadOnlySpan<T> WrittenSpan { get { throw null; } }
        public void Advance(int count) { }
        public void Clear() { }
        public System.Memory<T> GetMemory(int sizeHint = 0) { throw null; }
        public System.Span<T> GetSpan(int sizeHint = 0) { throw null; }
    }
    public static partial class BuffersExtensions
    {
        public static void CopyTo<T>(this in System.Buffers.ReadOnlySequence<T> source, System.Span<T> destination) { }
        public static System.SequencePosition? PositionOf<T>(this in System.Buffers.ReadOnlySequence<T> source, T value) where T : System.IEquatable<T> { throw null; }
        public static T[] ToArray<T>(this in System.Buffers.ReadOnlySequence<T> sequence) { throw null; }
        public static void Write<T>(this System.Buffers.IBufferWriter<T> writer, System.ReadOnlySpan<T> value) { }
    }
    public partial interface IBufferWriter<T>
    {
        void Advance(int count);
        System.Memory<T> GetMemory(int sizeHint = 0);
        System.Span<T> GetSpan(int sizeHint = 0);
    }
    public abstract partial class MemoryPool<T> : System.IDisposable
    {
        protected MemoryPool() { }
        public abstract int MaxBufferSize { get; }
        public static System.Buffers.MemoryPool<T> Shared { get { throw null; } }
        public void Dispose() { }
        protected abstract void Dispose(bool disposing);
        public abstract System.Buffers.IMemoryOwner<T> Rent(int minBufferSize = -1);
    }
    public abstract partial class ReadOnlySequenceSegment<T>
    {
        protected ReadOnlySequenceSegment() { }
        public System.ReadOnlyMemory<T> Memory { get { throw null; } protected set { } }
        public System.Buffers.ReadOnlySequenceSegment<T>? Next { get { throw null; } protected set { } }
        public long RunningIndex { get { throw null; } protected set { } }
    }
    public readonly partial struct ReadOnlySequence<T>
    {
        private readonly object _dummy;
        public static readonly System.Buffers.ReadOnlySequence<T> Empty;
        public ReadOnlySequence(System.Buffers.ReadOnlySequenceSegment<T> startSegment, int startIndex, System.Buffers.ReadOnlySequenceSegment<T> endSegment, int endIndex) { throw null; }
        public ReadOnlySequence(System.ReadOnlyMemory<T> memory) { throw null; }
        public ReadOnlySequence(T[] array) { throw null; }
        public ReadOnlySequence(T[] array, int start, int length) { throw null; }
        public System.SequencePosition End { get { throw null; } }
        public System.ReadOnlyMemory<T> First { get { throw null; } }
        public System.ReadOnlySpan<T> FirstSpan { get { throw null; } }
        public bool IsEmpty { get { throw null; } }
        public bool IsSingleSegment { get { throw null; } }
        public long Length { get { throw null; } }
        public System.SequencePosition Start { get { throw null; } }
        public System.Buffers.ReadOnlySequence<T>.Enumerator GetEnumerator() { throw null; }
        public System.SequencePosition GetPosition(long offset) { throw null; }
        public System.SequencePosition GetPosition(long offset, System.SequencePosition origin) { throw null; }
        public System.Buffers.ReadOnlySequence<T> Slice(int start, int length) { throw null; }
        public System.Buffers.ReadOnlySequence<T> Slice(int start, System.SequencePosition end) { throw null; }
        public System.Buffers.ReadOnlySequence<T> Slice(long start) { throw null; }
        public System.Buffers.ReadOnlySequence<T> Slice(long start, long length) { throw null; }
        public System.Buffers.ReadOnlySequence<T> Slice(long start, System.SequencePosition end) { throw null; }
        public System.Buffers.ReadOnlySequence<T> Slice(System.SequencePosition start) { throw null; }
        public System.Buffers.ReadOnlySequence<T> Slice(System.SequencePosition start, int length) { throw null; }
        public System.Buffers.ReadOnlySequence<T> Slice(System.SequencePosition start, long length) { throw null; }
        public System.Buffers.ReadOnlySequence<T> Slice(System.SequencePosition start, System.SequencePosition end) { throw null; }
        public override string ToString() { throw null; }
        public bool TryGet(ref System.SequencePosition position, out System.ReadOnlyMemory<T> memory, bool advance = true) { throw null; }
        public partial struct Enumerator
        {
            private object _dummy;
            public Enumerator(in System.Buffers.ReadOnlySequence<T> sequence) { throw null; }
            public System.ReadOnlyMemory<T> Current { get { throw null; } }
            public bool MoveNext() { throw null; }
        }
    }
    public static partial class SequenceReaderExtensions
    {
        public static bool TryReadBigEndian(this ref System.Buffers.SequenceReader<byte> reader, out short value) { throw null; }
        public static bool TryReadBigEndian(this ref System.Buffers.SequenceReader<byte> reader, out int value) { throw null; }
        public static bool TryReadBigEndian(this ref System.Buffers.SequenceReader<byte> reader, out long value) { throw null; }
        public static bool TryReadLittleEndian(this ref System.Buffers.SequenceReader<byte> reader, out short value) { throw null; }
        public static bool TryReadLittleEndian(this ref System.Buffers.SequenceReader<byte> reader, out int value) { throw null; }
        public static bool TryReadLittleEndian(this ref System.Buffers.SequenceReader<byte> reader, out long value) { throw null; }
    }
    public ref partial struct SequenceReader<T> where T : struct
    {
        private object _dummy;
        public SequenceReader(System.Buffers.ReadOnlySequence<T> sequence) { throw null; }
        public readonly long Consumed { get { throw null; } }
        public readonly System.ReadOnlySpan<T> CurrentSpan { get { throw null; } }
        public readonly int CurrentSpanIndex { get { throw null; } }
        public readonly bool End { get { throw null; } }
        public readonly long Length { get { throw null; } }
        public readonly System.SequencePosition Position { get { throw null; } }
        public readonly long Remaining { get { throw null; } }
        public readonly System.Buffers.ReadOnlySequence<T> Sequence { get { throw null; } }
        public readonly System.ReadOnlySpan<T> UnreadSpan { get { throw null; } }
        public void Advance(long count) { }
        public long AdvancePast(T value) { throw null; }
        public long AdvancePastAny(System.ReadOnlySpan<T> values) { throw null; }
        public long AdvancePastAny(T value0, T value1) { throw null; }
        public long AdvancePastAny(T value0, T value1, T value2) { throw null; }
        public long AdvancePastAny(T value0, T value1, T value2, T value3) { throw null; }
        public bool IsNext(System.ReadOnlySpan<T> next, bool advancePast = false) { throw null; }
        public bool IsNext(T next, bool advancePast = false) { throw null; }
        public void Rewind(long count) { }
        public bool TryAdvanceTo(T delimiter, bool advancePastDelimiter = true) { throw null; }
        public bool TryAdvanceToAny(System.ReadOnlySpan<T> delimiters, bool advancePastDelimiter = true) { throw null; }
        public readonly bool TryCopyTo(System.Span<T> destination) { throw null; }
        public readonly bool TryPeek(out T value) { throw null; }
        public bool TryRead(out T value) { throw null; }
        public bool TryReadTo(out System.Buffers.ReadOnlySequence<T> sequence, System.ReadOnlySpan<T> delimiter, bool advancePastDelimiter = true) { throw null; }
        public bool TryReadTo(out System.Buffers.ReadOnlySequence<T> sequence, T delimiter, bool advancePastDelimiter = true) { throw null; }
        public bool TryReadTo(out System.Buffers.ReadOnlySequence<T> sequence, T delimiter, T delimiterEscape, bool advancePastDelimiter = true) { throw null; }
        public bool TryReadTo(out System.ReadOnlySpan<T> span, T delimiter, bool advancePastDelimiter = true) { throw null; }
        public bool TryReadTo(out System.ReadOnlySpan<T> span, T delimiter, T delimiterEscape, bool advancePastDelimiter = true) { throw null; }
        public bool TryReadToAny(out System.Buffers.ReadOnlySequence<T> sequence, System.ReadOnlySpan<T> delimiters, bool advancePastDelimiter = true) { throw null; }
        public bool TryReadToAny(out System.ReadOnlySpan<T> span, System.ReadOnlySpan<T> delimiters, bool advancePastDelimiter = true) { throw null; }
    }
}
namespace System.Buffers.Text
{
    public static partial class Base64
    {
        public static System.Buffers.OperationStatus DecodeFromUtf8(System.ReadOnlySpan<byte> utf8, System.Span<byte> bytes, out int bytesConsumed, out int bytesWritten, bool isFinalBlock = true) { throw null; }
        public static System.Buffers.OperationStatus DecodeFromUtf8InPlace(System.Span<byte> buffer, out int bytesWritten) { throw null; }
        public static System.Buffers.OperationStatus EncodeToUtf8(System.ReadOnlySpan<byte> bytes, System.Span<byte> utf8, out int bytesConsumed, out int bytesWritten, bool isFinalBlock = true) { throw null; }
        public static System.Buffers.OperationStatus EncodeToUtf8InPlace(System.Span<byte> buffer, int dataLength, out int bytesWritten) { throw null; }
        public static int GetMaxDecodedFromUtf8Length(int length) { throw null; }
        public static int GetMaxEncodedToUtf8Length(int length) { throw null; }
    }
}
namespace System.Runtime.InteropServices
{
    public static partial class SequenceMarshal
    {
        public static bool TryGetArray<T>(System.Buffers.ReadOnlySequence<T> sequence, out System.ArraySegment<T> segment) { throw null; }
        public static bool TryGetReadOnlyMemory<T>(System.Buffers.ReadOnlySequence<T> sequence, out System.ReadOnlyMemory<T> memory) { throw null; }
        public static bool TryGetReadOnlySequenceSegment<T>(System.Buffers.ReadOnlySequence<T> sequence, [System.Diagnostics.CodeAnalysis.NotNullWhenAttribute(true)] out System.Buffers.ReadOnlySequenceSegment<T>? startSegment, out int startIndex, [System.Diagnostics.CodeAnalysis.NotNullWhenAttribute(true)] out System.Buffers.ReadOnlySequenceSegment<T>? endSegment, out int endIndex) { throw null; }
        public static bool TryRead<T>(ref System.Buffers.SequenceReader<byte> reader, out T value) where T : struct { throw null; }
    }
}
