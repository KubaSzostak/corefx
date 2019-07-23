// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------

namespace System.Resources
{
    public sealed partial class ResourceReader : System.Collections.IEnumerable, System.IDisposable, System.Resources.IResourceReader
    {
        public ResourceReader(System.IO.Stream stream) { }
        public ResourceReader(string fileName) { }
        public void Close() { }
        public void Dispose() { }
        public System.Collections.IDictionaryEnumerator GetEnumerator() { throw null; }
        public void GetResourceData(string resourceName, out string resourceType, out byte[] resourceData) { throw null; }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { throw null; }
    }
}
