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
using System.ComponentModel;
using System.Management.Automation;
using System.Runtime.InteropServices;
using static nz.geek.rhubarb.LocalSecurityAuthority.ADVAPI32;

namespace nz.geek.rhubarb.LocalSecurityAuthority
{
    [Cmdlet(VerbsCommon.Get, "LsaAccountRights")]
    [OutputType(typeof(string))]
    public class GetLsaAccountRights : PSCmdlet
    {
        [Parameter(HelpMessage = "PolicyHandle opened by Open-LsaPolicy")]
        public PolicyHandle PolicyHandle { get; set; }

        [Parameter(Mandatory = true, HelpMessage = "SID of user account")]
        public string AccountSid { get; set; }

        protected override void BeginProcessing()
        {
        }

        protected override void ProcessRecord()
        {
            if (!ADVAPI32.ConvertStringSidToSid(AccountSid, out IntPtr ptrSid))
            {
                throw new Win32Exception(KERNEL32.GetLastError());
            }

            try
            {
                using (var uph = new PolicyHandle(0x800, PolicyHandle))
                {
                    var result = ADVAPI32.LsaEnumerateAccountRights(uph.ObjectHandle, ptrSid, out IntPtr UserRights, out uint CountOfRights);

                    if (result != 0)
                    {
                        throw new Win32Exception(ADVAPI32.LsaNtStatusToWinError(result));
                    }

                    try
                    {
                        for (uint i = 0; i < CountOfRights; i++)
                        {
                            IntPtr itemAddr = new IntPtr(UserRights.ToInt64() + (i * Marshal.SizeOf(typeof(LSA_UNICODE_STRING))));
                            var lsaus = (LSA_UNICODE_STRING)Marshal.PtrToStructure(itemAddr, typeof(LSA_UNICODE_STRING));
                            WriteObject(Utils.LSAUS2string(lsaus));
                        }
                    }
                    finally
                    {
                        ADVAPI32.LsaFreeMemory(UserRights);
                    }
                }
            }
            finally
            {
                ADVAPI32.FreeSid(ptrSid);
            }
        }

        protected override void EndProcessing()
        {
        }
    }
}
