// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------

namespace System
{
    public partial class AppDomainUnloadedException : System.SystemException
    {
        public AppDomainUnloadedException() { }
        protected AppDomainUnloadedException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
        public AppDomainUnloadedException(string? message) { }
        public AppDomainUnloadedException(string? message, System.Exception? innerException) { }
    }
    public sealed partial class ApplicationId
    {
        public ApplicationId(byte[] publicKeyToken, string name, System.Version version, string? processorArchitecture, string? culture) { }
        public string? Culture { get { throw null; } }
        public string Name { get { throw null; } }
        public string? ProcessorArchitecture { get { throw null; } }
        public byte[] PublicKeyToken { get { throw null; } }
        public System.Version Version { get { throw null; } }
        public System.ApplicationId Copy() { throw null; }
        public override bool Equals(object? o) { throw null; }
        public override int GetHashCode() { throw null; }
        public override string ToString() { throw null; }
    }
    public abstract partial class ContextBoundObject : System.MarshalByRefObject
    {
        protected ContextBoundObject() { }
    }
    public partial class ContextMarshalException : System.SystemException
    {
        public ContextMarshalException() { }
        protected ContextMarshalException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
        public ContextMarshalException(string? message) { }
        public ContextMarshalException(string? message, System.Exception? inner) { }
    }
    [System.AttributeUsageAttribute(256, Inherited=false)]
    public partial class ContextStaticAttribute : System.Attribute
    {
        public ContextStaticAttribute() { }
    }
    public enum LoaderOptimization
    {
        NotSpecified = 0,
        SingleDomain = 1,
        MultiDomain = 2,
        [System.ObsoleteAttribute("This method has been deprecated. Please use Assembly.Load() instead. https://go.microsoft.com/fwlink/?linkid=14202")]
        DomainMask = 3,
        MultiDomainHost = 3,
        [System.ObsoleteAttribute("This method has been deprecated. Please use Assembly.Load() instead. https://go.microsoft.com/fwlink/?linkid=14202")]
        DisallowBindings = 4,
    }
    [System.AttributeUsageAttribute(64)]
    public sealed partial class LoaderOptimizationAttribute : System.Attribute
    {
        public LoaderOptimizationAttribute(byte value) { }
        public LoaderOptimizationAttribute(System.LoaderOptimization value) { }
        public System.LoaderOptimization Value { get { throw null; } }
    }
    public static partial class StringNormalizationExtensions
    {
        public static bool IsNormalized(this string strInput) { throw null; }
        public static bool IsNormalized(this string strInput, System.Text.NormalizationForm normalizationForm) { throw null; }
        public static string Normalize(this string strInput) { throw null; }
        public static string Normalize(this string strInput, System.Text.NormalizationForm normalizationForm) { throw null; }
    }
}
namespace System.CodeDom.Compiler
{
    public partial class IndentedTextWriter : System.IO.TextWriter
    {
        public const string DefaultTabString = "    ";
        public IndentedTextWriter(System.IO.TextWriter writer) { }
        public IndentedTextWriter(System.IO.TextWriter writer, string tabString) { }
        public override System.Text.Encoding Encoding { get { throw null; } }
        public int Indent { get { throw null; } set { } }
        public System.IO.TextWriter InnerWriter { get { throw null; } }
        [System.Diagnostics.CodeAnalysis.AllowNullAttribute]
        public override string NewLine { get { throw null; } set { } }
        public override void Close() { }
        public override void Flush() { }
        protected virtual void OutputTabs() { }
        public override void Write(bool value) { }
        public override void Write(char value) { }
        public override void Write(char[]? buffer) { }
        public override void Write(char[] buffer, int index, int count) { }
        public override void Write(double value) { }
        public override void Write(int value) { }
        public override void Write(long value) { }
        public override void Write(object? value) { }
        public override void Write(float value) { }
        public override void Write(string? s) { }
        public override void Write(string format, object? arg0) { }
        public override void Write(string format, object? arg0, object? arg1) { }
        public override void Write(string format, params object?[] arg) { }
        public override void WriteLine() { }
        public override void WriteLine(bool value) { }
        public override void WriteLine(char value) { }
        public override void WriteLine(char[]? buffer) { }
        public override void WriteLine(char[] buffer, int index, int count) { }
        public override void WriteLine(double value) { }
        public override void WriteLine(int value) { }
        public override void WriteLine(long value) { }
        public override void WriteLine(object? value) { }
        public override void WriteLine(float value) { }
        public override void WriteLine(string? s) { }
        public override void WriteLine(string format, object? arg0) { }
        public override void WriteLine(string format, object? arg0, object? arg1) { }
        public override void WriteLine(string format, params object?[] arg) { }
        [System.CLSCompliantAttribute(false)]
        public override void WriteLine(uint value) { }
        public void WriteLineNoTabs(string? s) { }
    }
}
namespace System.Diagnostics
{
    public partial class Stopwatch
    {
        public static readonly long Frequency;
        public static readonly bool IsHighResolution;
        public Stopwatch() { }
        public System.TimeSpan Elapsed { get { throw null; } }
        public long ElapsedMilliseconds { get { throw null; } }
        public long ElapsedTicks { get { throw null; } }
        public bool IsRunning { get { throw null; } }
        public static long GetTimestamp() { throw null; }
        public void Reset() { }
        public void Restart() { }
        public void Start() { }
        public static System.Diagnostics.Stopwatch StartNew() { throw null; }
        public void Stop() { }
    }
}
namespace System.IO
{
    public sealed partial class BufferedStream : System.IO.Stream
    {
        public BufferedStream(System.IO.Stream stream) { }
        public BufferedStream(System.IO.Stream stream, int bufferSize) { }
        public int BufferSize { get { throw null; } }
        public override bool CanRead { get { throw null; } }
        public override bool CanSeek { get { throw null; } }
        public override bool CanWrite { get { throw null; } }
        public override long Length { get { throw null; } }
        public override long Position { get { throw null; } set { } }
        public System.IO.Stream UnderlyingStream { get { throw null; } }
        public override System.IAsyncResult BeginRead(byte[] buffer, int offset, int count, System.AsyncCallback callback, object? state) { throw null; }
        public override System.IAsyncResult BeginWrite(byte[] buffer, int offset, int count, System.AsyncCallback callback, object? state) { throw null; }
        public override void CopyTo(System.IO.Stream destination, int bufferSize) { }
        public override System.Threading.Tasks.Task CopyToAsync(System.IO.Stream destination, int bufferSize, System.Threading.CancellationToken cancellationToken) { throw null; }
        protected override void Dispose(bool disposing) { }
        public override System.Threading.Tasks.ValueTask DisposeAsync() { throw null; }
        public override int EndRead(System.IAsyncResult asyncResult) { throw null; }
        public override void EndWrite(System.IAsyncResult asyncResult) { }
        public override void Flush() { }
        public override System.Threading.Tasks.Task FlushAsync(System.Threading.CancellationToken cancellationToken) { throw null; }
        public override int Read(byte[] array, int offset, int count) { throw null; }
        public override int Read(System.Span<byte> destination) { throw null; }
        public override System.Threading.Tasks.Task<int> ReadAsync(byte[] buffer, int offset, int count, System.Threading.CancellationToken cancellationToken) { throw null; }
        public override System.Threading.Tasks.ValueTask<int> ReadAsync(System.Memory<byte> buffer, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
        public override int ReadByte() { throw null; }
        public override long Seek(long offset, System.IO.SeekOrigin origin) { throw null; }
        public override void SetLength(long value) { }
        public override void Write(byte[] array, int offset, int count) { }
        public override void Write(System.ReadOnlySpan<byte> buffer) { }
        public override System.Threading.Tasks.Task WriteAsync(byte[] buffer, int offset, int count, System.Threading.CancellationToken cancellationToken) { throw null; }
        public override System.Threading.Tasks.ValueTask WriteAsync(System.ReadOnlyMemory<byte> buffer, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
        public override void WriteByte(byte value) { }
    }
    public sealed partial class InvalidDataException : System.SystemException
    {
        public InvalidDataException() { }
        public InvalidDataException(string? message) { }
        public InvalidDataException(string? message, System.Exception? innerException) { }
    }
    public partial class StringReader : System.IO.TextReader
    {
        public StringReader(string s) { }
        public override void Close() { }
        protected override void Dispose(bool disposing) { }
        public override int Peek() { throw null; }
        public override int Read() { throw null; }
        public override int Read(char[] buffer, int index, int count) { throw null; }
        public override int Read(System.Span<char> buffer) { throw null; }
        public override System.Threading.Tasks.Task<int> ReadAsync(char[] buffer, int index, int count) { throw null; }
        public override System.Threading.Tasks.ValueTask<int> ReadAsync(System.Memory<char> buffer, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
        public override int ReadBlock(System.Span<char> buffer) { throw null; }
        public override System.Threading.Tasks.Task<int> ReadBlockAsync(char[] buffer, int index, int count) { throw null; }
        public override System.Threading.Tasks.ValueTask<int> ReadBlockAsync(System.Memory<char> buffer, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
        public override string? ReadLine() { throw null; }
        public override System.Threading.Tasks.Task<string?> ReadLineAsync() { throw null; }
        public override string ReadToEnd() { throw null; }
        public override System.Threading.Tasks.Task<string> ReadToEndAsync() { throw null; }
    }
    public partial class StringWriter : System.IO.TextWriter
    {
        public StringWriter() { }
        public StringWriter(System.IFormatProvider? formatProvider) { }
        public StringWriter(System.Text.StringBuilder sb) { }
        public StringWriter(System.Text.StringBuilder sb, System.IFormatProvider? formatProvider) { }
        public override System.Text.Encoding Encoding { get { throw null; } }
        public override void Close() { }
        protected override void Dispose(bool disposing) { }
        public override System.Threading.Tasks.Task FlushAsync() { throw null; }
        public virtual System.Text.StringBuilder GetStringBuilder() { throw null; }
        public override string ToString() { throw null; }
        public override void Write(char value) { }
        public override void Write(char[] buffer, int index, int count) { }
        public override void Write(System.ReadOnlySpan<char> buffer) { }
        public override void Write(string? value) { }
        public override void Write(System.Text.StringBuilder? value) { }
        public override System.Threading.Tasks.Task WriteAsync(char value) { throw null; }
        public override System.Threading.Tasks.Task WriteAsync(char[] buffer, int index, int count) { throw null; }
        public override System.Threading.Tasks.Task WriteAsync(System.ReadOnlyMemory<char> buffer, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
        public override System.Threading.Tasks.Task WriteAsync(string? value) { throw null; }
        public override System.Threading.Tasks.Task WriteAsync(System.Text.StringBuilder? value, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
        public override void WriteLine(System.ReadOnlySpan<char> buffer) { }
        public override void WriteLine(System.Text.StringBuilder? value) { }
        public override System.Threading.Tasks.Task WriteLineAsync(char value) { throw null; }
        public override System.Threading.Tasks.Task WriteLineAsync(char[] buffer, int index, int count) { throw null; }
        public override System.Threading.Tasks.Task WriteLineAsync(System.ReadOnlyMemory<char> buffer, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
        public override System.Threading.Tasks.Task WriteLineAsync(string? value) { throw null; }
        public override System.Threading.Tasks.Task WriteLineAsync(System.Text.StringBuilder? value, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) { throw null; }
    }
}
namespace System.Net
{
    public static partial class WebUtility
    {
        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNullAttribute("value")]
        public static string? HtmlDecode(string? value) { throw null; }
        public static void HtmlDecode(string? value, System.IO.TextWriter output) { }
        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNullAttribute("value")]
        public static string? HtmlEncode(string? value) { throw null; }
        public static void HtmlEncode(string? value, System.IO.TextWriter output) { }
        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNullAttribute("encodedValue")]
        public static string? UrlDecode(string? encodedValue) { throw null; }
        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNullAttribute("encodedValue")]
        public static byte[]? UrlDecodeToBytes(byte[]? encodedValue, int offset, int count) { throw null; }
        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNullAttribute("value")]
        public static string? UrlEncode(string? value) { throw null; }
        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNullAttribute("value")]
        public static byte[]? UrlEncodeToBytes(byte[]? value, int offset, int count) { throw null; }
    }
}
namespace System.Reflection
{
    public partial class AssemblyNameProxy : System.MarshalByRefObject
    {
        public AssemblyNameProxy() { }
        public System.Reflection.AssemblyName GetAssemblyName(string assemblyFile) { throw null; }
    }
}
namespace System.Runtime
{
    public static partial class ProfileOptimization
    {
        public static void SetProfileRoot(string directoryPath) { }
        public static void StartProfile(string profile) { }
    }
}
namespace System.Runtime.CompilerServices
{
    public sealed partial class SwitchExpressionException : System.InvalidOperationException
    {
        public SwitchExpressionException() { }
        public SwitchExpressionException(System.Exception? innerException) { }
        public SwitchExpressionException(object? unmatchedValue) { }
        public SwitchExpressionException(string? message) { }
        public SwitchExpressionException(string? message, System.Exception? innerException) { }
        public override string Message { get { throw null; } }
        public object? UnmatchedValue { get { throw null; } }
        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
    }
}
namespace System.Runtime.Versioning
{
    [System.AttributeUsageAttribute(5887, AllowMultiple=false, Inherited=false)]
    public sealed partial class ComponentGuaranteesAttribute : System.Attribute
    {
        public ComponentGuaranteesAttribute(System.Runtime.Versioning.ComponentGuaranteesOptions guarantees) { }
        public System.Runtime.Versioning.ComponentGuaranteesOptions Guarantees { get { throw null; } }
    }
    [System.FlagsAttribute]
    public enum ComponentGuaranteesOptions
    {
        None = 0,
        Exchange = 1,
        Stable = 2,
        SideBySide = 4,
    }
    public sealed partial class FrameworkName : System.IEquatable<System.Runtime.Versioning.FrameworkName>
    {
        public FrameworkName(string frameworkName) { }
        public FrameworkName(string identifier, System.Version version) { }
        public FrameworkName(string identifier, System.Version version, string? profile) { }
        public string FullName { get { throw null; } }
        public string Identifier { get { throw null; } }
        public string Profile { get { throw null; } }
        public System.Version Version { get { throw null; } }
        public override bool Equals(object? obj) { throw null; }
        public bool Equals(System.Runtime.Versioning.FrameworkName? other) { throw null; }
        public override int GetHashCode() { throw null; }
        public static bool operator ==(System.Runtime.Versioning.FrameworkName? left, System.Runtime.Versioning.FrameworkName? right) { throw null; }
        public static bool operator !=(System.Runtime.Versioning.FrameworkName? left, System.Runtime.Versioning.FrameworkName? right) { throw null; }
        public override string ToString() { throw null; }
    }
    [System.AttributeUsageAttribute(224, Inherited=false)]
    [System.Diagnostics.ConditionalAttribute("RESOURCE_ANNOTATION_WORK")]
    public sealed partial class ResourceConsumptionAttribute : System.Attribute
    {
        public ResourceConsumptionAttribute(System.Runtime.Versioning.ResourceScope resourceScope) { }
        public ResourceConsumptionAttribute(System.Runtime.Versioning.ResourceScope resourceScope, System.Runtime.Versioning.ResourceScope consumptionScope) { }
        public System.Runtime.Versioning.ResourceScope ConsumptionScope { get { throw null; } }
        public System.Runtime.Versioning.ResourceScope ResourceScope { get { throw null; } }
    }
    [System.AttributeUsageAttribute(480, Inherited=false)]
    [System.Diagnostics.ConditionalAttribute("RESOURCE_ANNOTATION_WORK")]
    public sealed partial class ResourceExposureAttribute : System.Attribute
    {
        public ResourceExposureAttribute(System.Runtime.Versioning.ResourceScope exposureLevel) { }
        public System.Runtime.Versioning.ResourceScope ResourceExposureLevel { get { throw null; } }
    }
    [System.FlagsAttribute]
    public enum ResourceScope
    {
        None = 0,
        Machine = 1,
        Process = 2,
        AppDomain = 4,
        Library = 8,
        Private = 16,
        Assembly = 32,
    }
    public static partial class VersioningHelper
    {
        public static string MakeVersionSafeName(string? name, System.Runtime.Versioning.ResourceScope from, System.Runtime.Versioning.ResourceScope to) { throw null; }
        public static string MakeVersionSafeName(string? name, System.Runtime.Versioning.ResourceScope from, System.Runtime.Versioning.ResourceScope to, System.Type? type) { throw null; }
    }
}
namespace System.Security.Permissions
{
    [System.AttributeUsageAttribute(109, AllowMultiple=true, Inherited=false)]
    public abstract partial class CodeAccessSecurityAttribute : System.Security.Permissions.SecurityAttribute
    {
        protected CodeAccessSecurityAttribute(System.Security.Permissions.SecurityAction action) : base (default(System.Security.Permissions.SecurityAction)) { }
    }
    public enum SecurityAction
    {
        Demand = 2,
        Assert = 3,
        [System.ObsoleteAttribute("Deny is obsolete and will be removed in a future release of the .NET Framework. See https://go.microsoft.com/fwlink/?LinkID=155570 for more information.")]
        Deny = 4,
        PermitOnly = 5,
        LinkDemand = 6,
        InheritanceDemand = 7,
        [System.ObsoleteAttribute("Assembly level declarative security is obsolete and is no longer enforced by the CLR by default. See https://go.microsoft.com/fwlink/?LinkID=155570 for more information.")]
        RequestMinimum = 8,
        [System.ObsoleteAttribute("Assembly level declarative security is obsolete and is no longer enforced by the CLR by default. See https://go.microsoft.com/fwlink/?LinkID=155570 for more information.")]
        RequestOptional = 9,
        [System.ObsoleteAttribute("Assembly level declarative security is obsolete and is no longer enforced by the CLR by default. See https://go.microsoft.com/fwlink/?LinkID=155570 for more information.")]
        RequestRefuse = 10,
    }
    [System.AttributeUsageAttribute(109, AllowMultiple=true, Inherited=false)]
    public abstract partial class SecurityAttribute : System.Attribute
    {
        protected SecurityAttribute(System.Security.Permissions.SecurityAction action) { }
        public System.Security.Permissions.SecurityAction Action { get { throw null; } set { } }
        public bool Unrestricted { get { throw null; } set { } }
        public abstract System.Security.IPermission? CreatePermission();
    }
    [System.AttributeUsageAttribute(109, AllowMultiple=true, Inherited=false)]
    public sealed partial class SecurityPermissionAttribute : System.Security.Permissions.CodeAccessSecurityAttribute
    {
        public SecurityPermissionAttribute(System.Security.Permissions.SecurityAction action) : base (default(System.Security.Permissions.SecurityAction)) { }
        public bool Assertion { get { throw null; } set { } }
        public bool BindingRedirects { get { throw null; } set { } }
        public bool ControlAppDomain { get { throw null; } set { } }
        public bool ControlDomainPolicy { get { throw null; } set { } }
        public bool ControlEvidence { get { throw null; } set { } }
        public bool ControlPolicy { get { throw null; } set { } }
        public bool ControlPrincipal { get { throw null; } set { } }
        public bool ControlThread { get { throw null; } set { } }
        public bool Execution { get { throw null; } set { } }
        public System.Security.Permissions.SecurityPermissionFlag Flags { get { throw null; } set { } }
        public bool Infrastructure { get { throw null; } set { } }
        public bool RemotingConfiguration { get { throw null; } set { } }
        public bool SerializationFormatter { get { throw null; } set { } }
        public bool SkipVerification { get { throw null; } set { } }
        public bool UnmanagedCode { get { throw null; } set { } }
        public override System.Security.IPermission? CreatePermission() { throw null; }
    }
    [System.FlagsAttribute]
    public enum SecurityPermissionFlag
    {
        NoFlags = 0,
        Assertion = 1,
        UnmanagedCode = 2,
        SkipVerification = 4,
        Execution = 8,
        ControlThread = 16,
        ControlEvidence = 32,
        ControlPolicy = 64,
        SerializationFormatter = 128,
        ControlDomainPolicy = 256,
        ControlPrincipal = 512,
        ControlAppDomain = 1024,
        RemotingConfiguration = 2048,
        Infrastructure = 4096,
        BindingRedirects = 8192,
        AllFlags = 16383,
    }
}
