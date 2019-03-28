//
//  Copyright (c) Microsoft Corporation. All rights reserved.
//
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NetFwTypeLib
{
    [ComImport]
    [Guid("174A0DDA-E9F9-449D-993B-21AB667CA456")]
    [TypeLibType(4160)]
    public interface INetFwProfile
    {
        [DispId(1)]
        NET_FW_PROFILE_TYPE_ Type
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1)]
            get;
        }

        [DispId(2)]
        bool FirewallEnabled
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(2)]
            get;
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(2)]
            [param: In]
            set;
        }
    }
}