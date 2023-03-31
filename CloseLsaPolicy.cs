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

namespace nz.geek.rhubarb.LocalSecurityAuthority
{
    [Cmdlet(VerbsCommon.Close, "LsaPolicy")]
    public class CloseLsaPolicy : PSCmdlet
    {
        [Parameter(Mandatory = true, HelpMessage = "PolicyHandle opened by Open-LsaPolicy")]
        public PolicyHandle PolicyHandle { get; set; }

        protected override void BeginProcessing()
        {
        }

        protected override void ProcessRecord()
        {
            if (PolicyHandle != null)
            {
				PolicyHandle.Dispose();
			}
		}

        protected override void EndProcessing()
        {
        }
    }
}
