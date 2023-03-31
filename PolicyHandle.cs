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
using static nz.geek.rhubarb.LocalSecurityAuthority.ADVAPI32;

namespace nz.geek.rhubarb.LocalSecurityAuthority
{     
	public sealed class PolicyHandle : IDisposable
    {
		private readonly PolicyHandle parent;
		private readonly IntPtr objectHandle;
        private bool isValid;

        internal IntPtr ObjectHandle
        {
            get 
            {
                if (parent == null)
                {
                    if (isValid)
                    {
                        return objectHandle;
                    }

                    throw new InvalidOperationException();
                }

                return parent.ObjectHandle; 
            }
        }


        internal PolicyHandle(int mask, PolicyHandle policyHandle)
        {
            if (policyHandle == null)
            {
                var systemName = Utils.string2LSAUS(null);
                var ObjectAttributes = new LSA_OBJECT_ATTRIBUTES();
                int result = ADVAPI32.LsaOpenPolicy(ref systemName, ref ObjectAttributes, mask,out objectHandle);
                if (result != 0)
                {
                    throw new Win32Exception(ADVAPI32.LsaNtStatusToWinError(result));
                }
                isValid = true;
            }
            else
            {
				parent = policyHandle;
			}
		}

		internal PolicyHandle(IntPtr policyHandle)
		{
            objectHandle = policyHandle;
            isValid = true;
		}

		public void Dispose()
        {
            if (isValid)
            {
				isValid = false;

                int result = ADVAPI32.LsaClose(objectHandle);

				if (result != 0)
				{
					throw new Win32Exception(ADVAPI32.LsaNtStatusToWinError(result));
				}
			}
		}
    }
}
