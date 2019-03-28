//
//  Copyright (c) Microsoft Corporation. All rights reserved.
//
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NetFwTypeLib
{
    [ComImport]
    [TypeLibType(4160)]
    [Guid("F7898AF5-CAC4-4632-A2EC-DA06E5111AF2")]
    public interface INetFwMgr
    {
        [DispId(1)]
        INetFwPolicy LocalPolicy
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1)]
            [return: MarshalAs(UnmanagedType.Interface)]
            get;
        }
    }
}
