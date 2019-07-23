// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------

namespace System.Runtime.CompilerServices
{
    [System.AttributeUsageAttribute(2304, Inherited=false)]
    public sealed partial class IDispatchConstantAttribute : System.Runtime.CompilerServices.CustomConstantAttribute
    {
        public IDispatchConstantAttribute() { }
        public override object Value { get { throw null; } }
    }
    [System.AttributeUsageAttribute(2304, Inherited=false)]
    public sealed partial class IUnknownConstantAttribute : System.Runtime.CompilerServices.CustomConstantAttribute
    {
        public IUnknownConstantAttribute() { }
        public override object Value { get { throw null; } }
    }
}
namespace System.Runtime.InteropServices
{
    [System.FlagsAttribute]
    public enum AssemblyRegistrationFlags
    {
        None = 0,
        SetCodeBase = 1,
    }
    [System.AttributeUsageAttribute(1029, Inherited=false)]
    public sealed partial class AutomationProxyAttribute : System.Attribute
    {
        public AutomationProxyAttribute(bool val) { }
        public bool Value { get { throw null; } }
    }
    [System.AttributeUsageAttribute(10624, Inherited=false)]
    public sealed partial class ComAliasNameAttribute : System.Attribute
    {
        public ComAliasNameAttribute(string alias) { }
        public string Value { get { throw null; } }
    }
    public partial class ComAwareEventInfo : System.Reflection.EventInfo
    {
        public ComAwareEventInfo(System.Type type, string eventName) { }
        public override System.Reflection.EventAttributes Attributes { get { throw null; } }
        public override System.Type? DeclaringType { get { throw null; } }
        public override int MetadataToken { get { throw null; } }
        public override System.Reflection.Module Module { get { throw null; } }
        public override string Name { get { throw null; } }
        public override System.Type? ReflectedType { get { throw null; } }
        public override void AddEventHandler(object target, System.Delegate handler) { }
        public override System.Reflection.MethodInfo? GetAddMethod(bool nonPublic) { throw null; }
        public override object[] GetCustomAttributes(bool inherit) { throw null; }
        public override object[] GetCustomAttributes(System.Type attributeType, bool inherit) { throw null; }
        public override System.Collections.Generic.IList<System.Reflection.CustomAttributeData> GetCustomAttributesData() { throw null; }
        public override System.Reflection.MethodInfo[] GetOtherMethods(bool nonPublic) { throw null; }
        public override System.Reflection.MethodInfo? GetRaiseMethod(bool nonPublic) { throw null; }
        public override System.Reflection.MethodInfo? GetRemoveMethod(bool nonPublic) { throw null; }
        public override bool IsDefined(System.Type attributeType, bool inherit) { throw null; }
        public override void RemoveEventHandler(object target, System.Delegate handler) { }
    }
    [System.AttributeUsageAttribute(1, Inherited=false)]
    public sealed partial class ComCompatibleVersionAttribute : System.Attribute
    {
        public ComCompatibleVersionAttribute(int major, int minor, int build, int revision) { }
        public int BuildNumber { get { throw null; } }
        public int MajorVersion { get { throw null; } }
        public int MinorVersion { get { throw null; } }
        public int RevisionNumber { get { throw null; } }
    }
    [System.AttributeUsageAttribute(32767, Inherited=false)]
    public sealed partial class ComConversionLossAttribute : System.Attribute
    {
        public ComConversionLossAttribute() { }
    }
    [System.AttributeUsageAttribute(64, Inherited=false)]
    public sealed partial class ComRegisterFunctionAttribute : System.Attribute
    {
        public ComRegisterFunctionAttribute() { }
    }
    [System.AttributeUsageAttribute(64, Inherited=false)]
    public sealed partial class ComUnregisterFunctionAttribute : System.Attribute
    {
        public ComUnregisterFunctionAttribute() { }
    }
    public enum ExporterEventKind
    {
        NOTIF_TYPECONVERTED = 0,
        NOTIF_CONVERTWARNING = 1,
        ERROR_REFTOINVALIDASSEMBLY = 2,
    }
    public sealed partial class HandleCollector
    {
        public HandleCollector(string? name, int initialThreshold) { }
        public HandleCollector(string? name, int initialThreshold, int maximumThreshold) { }
        public int Count { get { throw null; } }
        public int InitialThreshold { get { throw null; } }
        public int MaximumThreshold { get { throw null; } }
        public string Name { get { throw null; } }
        public void Add() { }
        public void Remove() { }
    }
    [System.AttributeUsageAttribute(5, Inherited=false)]
    [System.ObsoleteAttribute("This attribute is deprecated and will be removed in a future version.", false)]
    public sealed partial class IDispatchImplAttribute : System.Attribute
    {
        public IDispatchImplAttribute(short implType) { }
        public IDispatchImplAttribute(System.Runtime.InteropServices.IDispatchImplType implType) { }
        public System.Runtime.InteropServices.IDispatchImplType Value { get { throw null; } }
    }
    [System.ObsoleteAttribute("The IDispatchImplAttribute is deprecated.", false)]
    public enum IDispatchImplType
    {
        SystemDefinedImpl = 0,
        InternalImpl = 1,
        CompatibleImpl = 2,
    }
    [System.AttributeUsageAttribute(1, Inherited=false)]
    public sealed partial class ImportedFromTypeLibAttribute : System.Attribute
    {
        public ImportedFromTypeLibAttribute(string tlbFile) { }
        public string Value { get { throw null; } }
    }
    [System.AttributeUsageAttribute(64, Inherited=false, AllowMultiple=false)]
    public sealed partial class ManagedToNativeComInteropStubAttribute : System.Attribute
    {
        public ManagedToNativeComInteropStubAttribute(System.Type classType, string methodName) { }
        public System.Type ClassType { get { throw null; } }
        public string MethodName { get { throw null; } }
    }
    [System.AttributeUsageAttribute(1, Inherited=false, AllowMultiple=true)]
    public sealed partial class PrimaryInteropAssemblyAttribute : System.Attribute
    {
        public PrimaryInteropAssemblyAttribute(int major, int minor) { }
        public int MajorVersion { get { throw null; } }
        public int MinorVersion { get { throw null; } }
    }
    [System.FlagsAttribute]
    public enum RegistrationClassContext
    {
        InProcessServer = 1,
        InProcessHandler = 2,
        LocalServer = 4,
        InProcessServer16 = 8,
        RemoteServer = 16,
        InProcessHandler16 = 32,
        Reserved1 = 64,
        Reserved2 = 128,
        Reserved3 = 256,
        Reserved4 = 512,
        NoCodeDownload = 1024,
        Reserved5 = 2048,
        NoCustomMarshal = 4096,
        EnableCodeDownload = 8192,
        NoFailureLog = 16384,
        DisableActivateAsActivator = 32768,
        EnableActivateAsActivator = 65536,
        FromDefaultContext = 131072,
    }
    [System.FlagsAttribute]
    public enum RegistrationConnectionType
    {
        SingleUse = 0,
        MultipleUse = 1,
        MultiSeparate = 2,
        Suspended = 4,
        Surrogate = 8,
    }
    public static partial class RuntimeEnvironment
    {
        public static string SystemConfigurationFile { get { throw null; } }
        public static bool FromGlobalAccessCache(System.Reflection.Assembly a) { throw null; }
        public static string GetRuntimeDirectory() { throw null; }
        public static System.IntPtr GetRuntimeInterfaceAsIntPtr(System.Guid clsid, System.Guid riid) { throw null; }
        public static object GetRuntimeInterfaceAsObject(System.Guid clsid, System.Guid riid) { throw null; }
        public static string GetSystemVersion() { throw null; }
    }
    [System.AttributeUsageAttribute(1, Inherited=false)]
    [System.ObsoleteAttribute("This attribute has been deprecated.  Application Domains no longer respect Activation Context boundaries in IDispatch calls.", false)]
    public sealed partial class SetWin32ContextInIDispatchAttribute : System.Attribute
    {
        public SetWin32ContextInIDispatchAttribute() { }
    }
    public partial class StandardOleMarshalObject : System.MarshalByRefObject
    {
        protected StandardOleMarshalObject() { }
    }
    [System.AttributeUsageAttribute(64, Inherited=false)]
    public sealed partial class TypeLibFuncAttribute : System.Attribute
    {
        public TypeLibFuncAttribute(short flags) { }
        public TypeLibFuncAttribute(System.Runtime.InteropServices.TypeLibFuncFlags flags) { }
        public System.Runtime.InteropServices.TypeLibFuncFlags Value { get { throw null; } }
    }
    [System.FlagsAttribute]
    public enum TypeLibFuncFlags
    {
        FRestricted = 1,
        FSource = 2,
        FBindable = 4,
        FRequestEdit = 8,
        FDisplayBind = 16,
        FDefaultBind = 32,
        FHidden = 64,
        FUsesGetLastError = 128,
        FDefaultCollelem = 256,
        FUiDefault = 512,
        FNonBrowsable = 1024,
        FReplaceable = 2048,
        FImmediateBind = 4096,
    }
    [System.AttributeUsageAttribute(1024, Inherited=false)]
    public sealed partial class TypeLibImportClassAttribute : System.Attribute
    {
        public TypeLibImportClassAttribute(System.Type importClass) { }
        public string Value { get { throw null; } }
    }
    [System.AttributeUsageAttribute(1052, Inherited=false)]
    public sealed partial class TypeLibTypeAttribute : System.Attribute
    {
        public TypeLibTypeAttribute(short flags) { }
        public TypeLibTypeAttribute(System.Runtime.InteropServices.TypeLibTypeFlags flags) { }
        public System.Runtime.InteropServices.TypeLibTypeFlags Value { get { throw null; } }
    }
    [System.FlagsAttribute]
    public enum TypeLibTypeFlags
    {
        FAppObject = 1,
        FCanCreate = 2,
        FLicensed = 4,
        FPreDeclId = 8,
        FHidden = 16,
        FControl = 32,
        FDual = 64,
        FNonExtensible = 128,
        FOleAutomation = 256,
        FRestricted = 512,
        FAggregatable = 1024,
        FReplaceable = 2048,
        FDispatchable = 4096,
        FReverseBind = 8192,
    }
    [System.AttributeUsageAttribute(256, Inherited=false)]
    public sealed partial class TypeLibVarAttribute : System.Attribute
    {
        public TypeLibVarAttribute(short flags) { }
        public TypeLibVarAttribute(System.Runtime.InteropServices.TypeLibVarFlags flags) { }
        public System.Runtime.InteropServices.TypeLibVarFlags Value { get { throw null; } }
    }
    [System.FlagsAttribute]
    public enum TypeLibVarFlags
    {
        FReadOnly = 1,
        FSource = 2,
        FBindable = 4,
        FRequestEdit = 8,
        FDisplayBind = 16,
        FDefaultBind = 32,
        FHidden = 64,
        FRestricted = 128,
        FDefaultCollelem = 256,
        FUiDefault = 512,
        FNonBrowsable = 1024,
        FReplaceable = 2048,
        FImmediateBind = 4096,
    }
    [System.AttributeUsageAttribute(1, Inherited=false)]
    public sealed partial class TypeLibVersionAttribute : System.Attribute
    {
        public TypeLibVersionAttribute(int major, int minor) { }
        public int MajorVersion { get { throw null; } }
        public int MinorVersion { get { throw null; } }
    }
}
namespace System.Runtime.InteropServices.ComTypes
{
    [System.FlagsAttribute]
    public enum ADVF
    {
        ADVF_NODATA = 1,
        ADVF_PRIMEFIRST = 2,
        ADVF_ONLYONCE = 4,
        ADVFCACHE_NOHANDLER = 8,
        ADVFCACHE_FORCEBUILTIN = 16,
        ADVFCACHE_ONSAVE = 32,
        ADVF_DATAONSTOP = 64,
    }
    public enum DATADIR
    {
        DATADIR_GET = 1,
        DATADIR_SET = 2,
    }
    [System.FlagsAttribute]
    public enum DVASPECT
    {
        DVASPECT_CONTENT = 1,
        DVASPECT_THUMBNAIL = 2,
        DVASPECT_ICON = 4,
        DVASPECT_DOCPRINT = 8,
    }
    public partial struct FORMATETC
    {
        public short cfFormat;
        public System.Runtime.InteropServices.ComTypes.DVASPECT dwAspect;
        public int lindex;
        public System.IntPtr ptd;
        public System.Runtime.InteropServices.ComTypes.TYMED tymed;
    }
    [System.Runtime.InteropServices.InterfaceTypeAttribute(1)]
    public partial interface IAdviseSink
    {
        void OnClose();
        void OnDataChange(ref System.Runtime.InteropServices.ComTypes.FORMATETC format, ref System.Runtime.InteropServices.ComTypes.STGMEDIUM stgmedium);
        void OnRename(System.Runtime.InteropServices.ComTypes.IMoniker moniker);
        void OnSave();
        void OnViewChange(int aspect, int index);
    }
    [System.CLSCompliantAttribute(false)]
    [System.Runtime.InteropServices.InterfaceTypeAttribute(1)]
    public partial interface IDataObject
    {
        int DAdvise(ref System.Runtime.InteropServices.ComTypes.FORMATETC pFormatetc, System.Runtime.InteropServices.ComTypes.ADVF advf, System.Runtime.InteropServices.ComTypes.IAdviseSink adviseSink, out int connection);
        void DUnadvise(int connection);
        int EnumDAdvise(out System.Runtime.InteropServices.ComTypes.IEnumSTATDATA? enumAdvise);
        System.Runtime.InteropServices.ComTypes.IEnumFORMATETC EnumFormatEtc(System.Runtime.InteropServices.ComTypes.DATADIR direction);
        int GetCanonicalFormatEtc(ref System.Runtime.InteropServices.ComTypes.FORMATETC formatIn, out System.Runtime.InteropServices.ComTypes.FORMATETC formatOut);
        void GetData(ref System.Runtime.InteropServices.ComTypes.FORMATETC format, out System.Runtime.InteropServices.ComTypes.STGMEDIUM medium);
        void GetDataHere(ref System.Runtime.InteropServices.ComTypes.FORMATETC format, ref System.Runtime.InteropServices.ComTypes.STGMEDIUM medium);
        int QueryGetData(ref System.Runtime.InteropServices.ComTypes.FORMATETC format);
        void SetData(ref System.Runtime.InteropServices.ComTypes.FORMATETC formatIn, ref System.Runtime.InteropServices.ComTypes.STGMEDIUM medium, bool release);
    }
    [System.Runtime.InteropServices.InterfaceTypeAttribute(1)]
    public partial interface IEnumFORMATETC
    {
        void Clone(out System.Runtime.InteropServices.ComTypes.IEnumFORMATETC newEnum);
        int Next(int celt, System.Runtime.InteropServices.ComTypes.FORMATETC[] rgelt, int[] pceltFetched);
        int Reset();
        int Skip(int celt);
    }
    [System.Runtime.InteropServices.InterfaceTypeAttribute(1)]
    public partial interface IEnumSTATDATA
    {
        void Clone(out System.Runtime.InteropServices.ComTypes.IEnumSTATDATA newEnum);
        int Next(int celt, System.Runtime.InteropServices.ComTypes.STATDATA[] rgelt, int[] pceltFetched);
        int Reset();
        int Skip(int celt);
    }
    public partial struct STATDATA
    {
        public System.Runtime.InteropServices.ComTypes.ADVF advf;
        public System.Runtime.InteropServices.ComTypes.IAdviseSink advSink;
        public int connection;
        public System.Runtime.InteropServices.ComTypes.FORMATETC formatetc;
    }
    public partial struct STGMEDIUM
    {
        public object? pUnkForRelease;
        public System.Runtime.InteropServices.ComTypes.TYMED tymed;
        public System.IntPtr unionmember;
    }
    [System.FlagsAttribute]
    public enum TYMED
    {
        TYMED_NULL = 0,
        TYMED_HGLOBAL = 1,
        TYMED_FILE = 2,
        TYMED_ISTREAM = 4,
        TYMED_ISTORAGE = 8,
        TYMED_GDI = 16,
        TYMED_MFPICT = 32,
        TYMED_ENHMF = 64,
    }
}
namespace System.Security
{
    [System.CLSCompliantAttribute(false)]
    public static partial class SecureStringMarshal
    {
        public static System.IntPtr SecureStringToCoTaskMemAnsi(System.Security.SecureString s) { throw null; }
        public static System.IntPtr SecureStringToCoTaskMemUnicode(System.Security.SecureString s) { throw null; }
        public static System.IntPtr SecureStringToGlobalAllocAnsi(System.Security.SecureString s) { throw null; }
        public static System.IntPtr SecureStringToGlobalAllocUnicode(System.Security.SecureString s) { throw null; }
    }
}
