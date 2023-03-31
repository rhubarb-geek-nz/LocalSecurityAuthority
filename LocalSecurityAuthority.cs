/**************************************************************************
 *
 *  Copyright 2023, Roger Brown
 *
 *  This file is part of rhubarb-geek-nz/LocalSecurityAuthority.
 *
 *  This program is free software: you can redistribute it and/or modify it
 *  under the terms of the GNU Lesser General Public License as published by the
 *  Free Software Foundation, either version 3 of the License, or (at your
 *  option) any later version.
 * 
 *  This program is distributed in the hope that it will be useful, but WITHOUT
 *  ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
 *  FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for
 *  more details.
 *
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with this program.  If not, see <http://www.gnu.org/licenses/>
 *
 */

using System;
using System.Runtime.InteropServices;
using System.Text;
using static nz.geek.rhubarb.LocalSecurityAuthority.ADVAPI32;

namespace nz.geek.rhubarb.LocalSecurityAuthority
{
    static class ADVAPI32
    {
        [StructLayout(LayoutKind.Sequential)]
        internal struct LSA_OBJECT_ATTRIBUTES
        {
            public int Length;
            public IntPtr RootDirectory;
            public readonly LSA_UNICODE_STRING ObjectName;
            public UInt32 Attributes;
            public IntPtr SecurityDescriptor;
            public IntPtr SecurityQualityOfService;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct LSA_UNICODE_STRING
        {
            public UInt16 Length;
            public UInt16 MaximumLength;
            public IntPtr Buffer;
        }

        [DllImport("advapi32.dll", PreserveSig = true)]
        internal static extern int LsaOpenPolicy(
            ref LSA_UNICODE_STRING SystemName,
            ref LSA_OBJECT_ATTRIBUTES ObjectAttributes,
            Int32 DesiredAccess,
            out IntPtr PolicyHandle
            );

        [DllImport("advapi32.dll", SetLastError = true, PreserveSig = true)]
        internal static extern int LsaAddAccountRights(
            IntPtr PolicyHandle,
            IntPtr AccountSid,
            LSA_UNICODE_STRING[] UserRights,
            int CountOfRights);

        [DllImport("advapi32.dll", SetLastError = true, PreserveSig = true)]
        internal static extern int LsaRemoveAccountRights(
            IntPtr PolicyHandle,
            IntPtr AccountSid,
            bool AllRights,
            LSA_UNICODE_STRING[] UserRights,
            int CountOfRights);

        [DllImport("advapi32")]
        internal static extern void FreeSid(IntPtr pSid);

        [DllImport("advapi32")]
        internal static extern int LsaFreeMemory(IntPtr Buffer);

        [DllImport("advapi32.dll")]
        internal static extern int LsaClose(IntPtr ObjectHandle);

        [DllImport("advapi32.dll")]
        internal static extern int LsaNtStatusToWinError(Int32 status);

        [DllImport("advapi32.dll", SetLastError = true, PreserveSig = true)]
        internal static extern int LsaEnumerateAccountRights(
            IntPtr PolicyHandle, IntPtr AccountSid,
            out IntPtr UserRights,
            out uint CountOfRights);

        [DllImport("advapi32.dll", SetLastError = true)]
        internal static extern bool ConvertStringSidToSid(
            string StringSid,
            out IntPtr ptrSid
            );
    }

    static class KERNEL32
    {
        [DllImport("kernel32.dll")]
        internal static extern int GetLastError();
    }

    static class Utils
    {
        internal static string LSAUS2string(LSA_UNICODE_STRING lsa)
        {
            char[] cvt = new char[lsa.Length/UnicodeEncoding.CharSize];
            Marshal.Copy(lsa.Buffer, cvt, 0, cvt.Length);
            return new string(cvt);
        }

        internal static LSA_UNICODE_STRING string2LSAUS(string value)
        {
            LSA_UNICODE_STRING result = new LSA_UNICODE_STRING();

            if (value != null)
            {
                result.Buffer = Marshal.StringToHGlobalUni(value);
                result.Length = (UInt16)(value.Length * UnicodeEncoding.CharSize);
                result.MaximumLength = (UInt16)((value.Length + 1) * UnicodeEncoding.CharSize);
            }

            return result;
        }

        internal static void FreeLSAUS(LSA_UNICODE_STRING lsa)
        {
            IntPtr Buffer = lsa.Buffer;
            lsa.Buffer = IntPtr.Zero;
            lsa.Length = 0;
            lsa.MaximumLength = 0;

            if (Buffer != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(Buffer);
            }
        }
    }
}
