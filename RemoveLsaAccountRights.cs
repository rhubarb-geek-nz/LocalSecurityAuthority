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
using static nz.geek.rhubarb.LocalSecurityAuthority.ADVAPI32;

namespace nz.geek.rhubarb.LocalSecurityAuthority
{
    [Cmdlet(VerbsCommon.Remove, "LsaAccountRights")]
    [OutputType(typeof(string))]
    public class RemoveLsaAccountRights : PSCmdlet
    {
        [Parameter(HelpMessage = "PolicyHandle opened by Open-LsaPolicy")]
        public PolicyHandle PolicyHandle { get; set; }

        [Parameter(Mandatory = true, HelpMessage = "SID of user account")]
        public string AccountSid { get; set; }

        [Parameter(HelpMessage = "$True if remove all rights")]
        public bool AllRights { get; set; }

        [Parameter(HelpMessage = "List of rights to remove if AllRights not $True")]
        public string [] UserRights { get; set; }

        protected override void BeginProcessing()
        {
        }

        protected override void ProcessRecord()
        {
            if (AllRights)
            {
                if (UserRights != null)
                {
                    throw new ArgumentException("UserRights should not be provided if AllRights is $True", nameof(UserRights));
                }
            }
            else
            {
                if ((UserRights == null) || (UserRights.Length == 0))
                {
                    throw new ArgumentException("No rights to remove", nameof(UserRights));
                }
            }

            if (!ADVAPI32.ConvertStringSidToSid(AccountSid, out IntPtr ptrSid))
            {
                throw new Win32Exception(KERNEL32.GetLastError());
            }

            try
            {
                using (var uph = new PolicyHandle(0x800, PolicyHandle))
                {
                    int CountOfRights = AllRights ? 0 : UserRights.Length;
                    LSA_UNICODE_STRING[] rights = new LSA_UNICODE_STRING[CountOfRights]; 

                    for (int i = 0; i < CountOfRights; i++)
                    {
                        rights[i] = Utils.string2LSAUS(UserRights[i]);
                    }

                    try
                    {
                        var result = ADVAPI32.LsaRemoveAccountRights(uph.ObjectHandle, ptrSid, AllRights, rights, CountOfRights);

                        if (result != 0)
                        {
                            throw new Win32Exception(ADVAPI32.LsaNtStatusToWinError(result));
                        }
                    }
                    finally
                    {
                        for (int i = 0; i < CountOfRights; i++)
                        {
                            Utils.FreeLSAUS(rights[i]);
                        }
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
