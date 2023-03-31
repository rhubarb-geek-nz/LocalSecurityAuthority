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
	$ModuleName = 'LocalSecurityAuthority',
	$CompanyName = 'rhubarb-geek-nz'
)

$ErrorActionPreference = "Stop"
$ProgressPreference = "SilentlyContinue"
$BINDIR = "bin/Release/netstandard2.0"
$RID = [System.Runtime.InteropServices.RuntimeInformation]::RuntimeIdentifier

trap
{
	throw $PSItem
}

$xmlDoc = [System.Xml.XmlDocument](Get-Content "$ModuleName.nuspec")

$Version = $xmlDoc.SelectSingleNode("/package/metadata/version").FirstChild.Value
$ModuleId = $xmlDoc.SelectSingleNode("/package/metadata/id").FirstChild.Value
$ProjectUri = $xmlDoc.SelectSingleNode("/package/metadata/projectUrl").FirstChild.Value
$Description = $xmlDoc.SelectSingleNode("/package/metadata/description").FirstChild.Value
$Author = $xmlDoc.SelectSingleNode("/package/metadata/authors").FirstChild.Value
$Copyright = $xmlDoc.SelectSingleNode("/package/metadata/copyright").FirstChild.Value

foreach ($Name in "obj", "bin", "$ModuleId")
{
	if (Test-Path "$Name")
	{
		Remove-Item "$Name" -Force -Recurse
	} 
}

dotnet build $ModuleName.csproj --configuration Release

If ( $LastExitCode -ne 0 )
{
	Exit $LastExitCode
}

$null = New-Item -Path "$ModuleId" -ItemType Directory

foreach ($Filter in "$ModuleName.*")
{
	Get-ChildItem -Path "$BINDIR" -Filter $Filter | Foreach-Object {
		if ((-not($_.Name.EndsWith('.pdb'))) -and (-not($_.Name.EndsWith('.deps.json'))))
		{
			Copy-Item -Path $_.FullName -Destination "$ModuleId"
		}
	}
}

@"
@{
	RootModule = '$ModuleName.dll'
	ModuleVersion = '$Version'
	GUID = '8691309f-2c6e-4be1-9bb8-79bbd73afd71'
	Author = '$Author'
	CompanyName = '$CompanyName'
	Copyright = '$Copyright'
	Description = '$Description'
	FunctionsToExport = @()
	CmdletsToExport = @('Open-LsaPolicy', 'Close-LsaPolicy', 'Get-LsaAccountRights', 'Add-LsaAccountRights', 'Remove-LsaAccountRights')
	VariablesToExport = '*'
	AliasesToExport = @()
	PrivateData = @{
		PSData = @{
			ProjectUri = '$ProjectUri'
		}
	}
}
"@ | Set-Content -Path "$ModuleId/$ModuleId.psd1"

Get-Content "./README.md" | Set-Content -Path "$ModuleId/README.md"

nuget pack "$ModuleName.nuspec"

If ( $LastExitCode -ne 0 )
{
	Exit $LastExitCode
}
