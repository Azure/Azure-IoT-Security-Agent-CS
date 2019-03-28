//
//  Copyright (c) Microsoft Corporation. All rights reserved.
//
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NetFwTypeLib
{
    [ComImport]
    [TypeLibType(4160)]
    [Guid("98325047-C671-4174-8D81-DEFCD3F03186")]
    public interface INetFwPolicy2
    {
        [DispId(1)]
        int CurrentProfileTypes
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

        [DispId(3)]
        object ExcludedInterfaces
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(3)]
            get;
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(3)]
            [param: In]
            set;
        }

        [DispId(4)]
        bool BlockAllInboundTraffic
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(4)]
            get;
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(4)]
            [param: In]
            set;
        }

        [DispId(5)]
        bool NotificationsDisabled
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(5)]
            get;
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(5)]
            [param: In]
            set;
        }

        [DispId(6)]
        bool UnicastResponsesToMulticastBroadcastDisabled
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(6)]
            get;
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(6)]
            [param: In]
            set;
        }

        [DispId(7)]
        INetFwRules Rules
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(7)]
            [return: MarshalAs(UnmanagedType.Interface)]
            get;
        }
    }
}
