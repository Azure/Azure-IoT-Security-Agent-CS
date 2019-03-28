//
//  Copyright (c) Microsoft Corporation. All rights reserved.
//
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NetFwTypeLib
{
    [ComImport]
    [TypeLibType(4160)]
    [Guid("D46D2478-9AC9-4008-9DC7-5563CE5536CC")]
    public interface INetFwPolicy
    {
        [DispId(1)]
        INetFwProfile CurrentProfile
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1)]
            [return: MarshalAs(UnmanagedType.Interface)]
            get;
        }
    }
}
