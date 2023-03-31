#!/usr/bin/env pwsh
#
#  Copyright 2023, Roger Brown
#
#  This file is part of rhubarb-geek-nz/LocalSecurityAuthority.
#
#  This program is free software: you can redistribute it and/or modify it
#  under the terms of the GNU Lesser General Public License as published by the
#  Free Software Foundation, either version 3 of the License, or (at your
#  option) any later version.
# 
#  This program is distributed in the hope that it will be useful, but WITHOUT
#  ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
#  FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for
#  more details.
#
#  You should have received a copy of the GNU General Public License
#  along with this program.  If not, see <http://www.gnu.org/licenses/>
#

param(
	$UserName = "$Env:USERNAME"
)

$ErrorActionPreference = 'Stop'
$ProgressPreference = 'SilentlyContinue'

trap
{
	throw $PSItem
}

$user = New-Object System.Security.Principal.NTAccount($UserName)
$AccountSid = $user.Translate([System.Security.Principal.SecurityIdentifier]).Value

Write-Host "$AccountSid"

$OriginalRights = $null

Write-Host 'Getting original rights'

$PolicyHandle = Open-LsaPolicy -DesiredAccess 0x800

try
{
	$OriginalRights = ( Get-LsaAccountRights -PolicyHandle $PolicyHandle -AccountSid $AccountSid )

	$OriginalRights | ConvertTo-Json
}
catch
{
	Write-Host $PSItem.Exception.Message
	Write-Host "no original rights found"
}

Close-LsaPolicy -PolicyHandle $PolicyHandle

Write-Host 'Adding SeServiceLogonRight'

Add-LsaAccountRights -AccountSid $AccountSid -UserRights 'SeServiceLogonRight'

Write-Host 'Adding SeServiceLogonRight and SeBatchLogonRight'

Add-LsaAccountRights -AccountSid $AccountSid -UserRights 'SeServiceLogonRight','SeBatchLogonRight'

Write-Host 'Removing SeBatchLogonRight'

Remove-LsaAccountRights -AccountSid $AccountSid -UserRights 'SeBatchLogonRight'

Write-Host 'Removing All'

Remove-LsaAccountRights -AccountSid $AccountSid -AllRights $True

if ($OriginalRights)
{
	Write-Host 'Restoring rights'

	Add-LsaAccountRights -AccountSid $AccountSid -UserRights $OriginalRights
	Get-LsaAccountRights -AccountSid $AccountSid | ConvertTo-Json
}
else
{
	Write-Host 'no original rights'
}
