//
//  Copyright (c) Microsoft Corporation. All rights reserved.
//
using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace NetFwTypeLib
{
    [ComImport]
    [Guid("9C4C6277-5027-441E-AFAE-CA1F542DA009")]
    [TypeLibType(4160)]
    public interface INetFwRules : IEnumerable
    {
        [DispId(1)]
        int Count
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            [DispId(1)]
            get;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(2)]
        void Add([In] [MarshalAs(UnmanagedType.Interface)] INetFwRule rule);

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(3)]
        void Remove([In] [MarshalAs(UnmanagedType.BStr)] string Name);

        [MethodImpl(MethodImplOptions.InternalCall)]
        [DispId(4)]
        [return: MarshalAs(UnmanagedType.Interface)]
        INetFwRule Item([In] [MarshalAs(UnmanagedType.BStr)] string Name);

        [DispId(-4)]
        IEnumVARIANT get__NewEnum();
    }
}
