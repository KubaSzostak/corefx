// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------

namespace Microsoft.Win32.SafeHandles
{
    public sealed partial class SafeFileHandle : Microsoft.Win32.SafeHandles.SafeHandleZeroOrMinusOneIsInvalid
    {
        public SafeFileHandle(System.IntPtr preexistingHandle, bool ownsHandle) : base (default(bool)) { }
        public override bool IsInvalid { get { throw null; } }
        protected override bool ReleaseHandle() { throw null; }
    }
}
namespace System.IO
{
    [System.FlagsAttribute]
    public enum FileAccess
    {
        Read = 1,
        Write = 2,
        ReadWrite = 3,
    }
    [System.FlagsAttribute]
    public enum FileAttributes
    {
        ReadOnly = 1,
        Hidden = 2,
        System = 4,
        Directory = 16,
        Archive = 32,
        Device = 64,
        Normal = 128,
        Temporary = 256,
        SparseFile = 512,
        ReparsePoint = 1024,
        Compressed = 2048,
        Offline = 4096,
        NotContentIndexed = 8192,
        Encrypted = 16384,
        IntegrityStream = 32768,
        NoScrubData = 131072,
    }
    public enum FileMode
    {
        CreateNew = 1,
        Create = 2,
        Open = 3,
        OpenOrCreate = 4,
        Truncate = 5,
        Append = 6,
    }
    [System.FlagsAttribute]
    public enum FileShare
    {
        None = 0,
        Read = 1,
        Write = 2,
        ReadWrite = 3,
        Delete = 4,
        Inheritable = 16,
    }
}
