//
//  Copyright (c) Microsoft Corporation. All rights reserved.
//
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NetFwTypeLib
{
    [ComImport]
    [TypeLibType(4160)]
    [Guid("AF230D27-BABA-4E42-ACED-F524F22CFCE2")]
    public interface INetFwRule
    {
        [DispId(1)]
        string Name
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1)]
            [param: In]
            [param: MarshalAs(UnmanagedType.BStr)]
            set;
        }

        [DispId(2)]
        string Description
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(2)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(2)]
            [param: In]
            [param: MarshalAs(UnmanagedType.BStr)]
            set;
        }

        [DispId(3)]
        string ApplicationName
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(3)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(3)]
            [param: In]
            [param: MarshalAs(UnmanagedType.BStr)]
            set;
        }

        [DispId(4)]
        string serviceName
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(4)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(4)]
            [param: In]
            [param: MarshalAs(UnmanagedType.BStr)]
            set;
        }

        [DispId(5)]
        int Protocol
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
        string LocalPorts
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(6)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(6)]
            [param: In]
            [param: MarshalAs(UnmanagedType.BStr)]
            set;
        }

        [DispId(7)]
        string RemotePorts
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(7)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(7)]
            [param: In]
            [param: MarshalAs(UnmanagedType.BStr)]
            set;
        }

        [DispId(8)]
        string LocalAddresses
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(8)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(8)]
            [param: In]
            [param: MarshalAs(UnmanagedType.BStr)]
            set;
        }

        [DispId(9)]
        string RemoteAddresses
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(9)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(9)]
            [param: In]
            [param: MarshalAs(UnmanagedType.BStr)]
            set;
        }

        [DispId(10)]
        string IcmpTypesAndCodes
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(10)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(10)]
            [param: In]
            [param: MarshalAs(UnmanagedType.BStr)]
            set;
        }

        [DispId(11)]
        NET_FW_RULE_DIRECTION_ Direction
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(11)]
            get;
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(11)]
            [param: In]
            set;
        }

        [DispId(12)]
        object Interfaces
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(12)]
            get;
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(12)]
            [param: In]
            set;
        }

        [DispId(13)]
        string InterfaceTypes
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(13)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(13)]
            [param: In]
            [param: MarshalAs(UnmanagedType.BStr)]
            set;
        }

        [DispId(14)]
        bool Enabled
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(14)]
            get;
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(14)]
            [param: In]
            set;
        }

        [DispId(15)]
        string Grouping
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(15)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(15)]
            [param: In]
            [param: MarshalAs(UnmanagedType.BStr)]
            set;
        }

        [DispId(16)]
        int Profiles
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(16)]
            get;
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(16)]
            [param: In]
            set;
        }

        [DispId(17)]
        bool EdgeTraversal
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(17)]
            get;
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(17)]
            [param: In]
            set;
        }

        [DispId(18)]
        NET_FW_ACTION_ Action
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(18)]
            get;
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(18)]
            [param: In]
            set;
        }
    }
}
