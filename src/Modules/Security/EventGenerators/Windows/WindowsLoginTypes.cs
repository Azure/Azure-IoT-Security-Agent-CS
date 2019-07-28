using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Azure.Security.IoT.Agent.EventGenerators.Windows
{
    /// <summary>
    /// Windows logon types according to this https://docs.microsoft.com/en-us/previous-versions/windows/it-pro/windows-server-2003/cc787567(v=ws.10)
    /// </summary>
    public enum WindowsLoginTypes
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        Interactive = 2,
        Network = 3,
        Batch = 4,
        Service = 5,
        Unlock = 7,
        NetworkCleartext = 8,
        NewCredentials = 9,
        RemoteInteractive = 10,
        CachedInteractive = 11
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}
