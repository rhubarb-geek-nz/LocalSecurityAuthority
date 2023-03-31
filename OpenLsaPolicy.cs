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
    [Cmdlet(VerbsCommon.Open,"LsaPolicy")]
    [OutputType(typeof(PolicyHandle))]
    public class OpenLsaPolicy : PSCmdlet
    {
        [Parameter(HelpMessage = "Computer name for the policy")]
        public string SystemName { get; set; }

        [Parameter(Mandatory = true, HelpMessage = "Access rights bits")]
        public Int32 DesiredAccess { get; set; }

        protected override void BeginProcessing()
        {
        }

        protected override void ProcessRecord()
        {
            var systemName = Utils.string2LSAUS(SystemName);
            var ObjectAttributes = new LSA_OBJECT_ATTRIBUTES();

            try
            {
                var result = ADVAPI32.LsaOpenPolicy(ref systemName, ref ObjectAttributes, DesiredAccess, out IntPtr policyHandle);

                if (result != 0)
                {
                    throw new Win32Exception(ADVAPI32.LsaNtStatusToWinError(result));
                }
		
                WriteObject(new PolicyHandle(policyHandle));
			}
			finally
            {
                Utils.FreeLSAUS(systemName);
            }
        }

        protected override void EndProcessing()
        {
        }
    }
}
