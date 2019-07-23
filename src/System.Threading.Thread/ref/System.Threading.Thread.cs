// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// ------------------------------------------------------------------------------
// Changes to this file must follow the http://aka.ms/api-review process.
// ------------------------------------------------------------------------------

namespace System.Threading
{
    public partial class ThreadExceptionEventArgs : System.EventArgs
    {
        public ThreadExceptionEventArgs(System.Exception t) { }
        public System.Exception Exception { get { throw null; } }
    }
    public delegate void ThreadExceptionEventHandler(object sender, System.Threading.ThreadExceptionEventArgs e);
}
