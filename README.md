# Local Security Authority

Invoke Local Security Authority APIs using Cmdlets

Cmdlet | ADVAPI32 | SystemName | DesiredAccess | PolicyHandle | AccountSid | AllRights | UserRights
-------|----------|------------|-------------|----------|------------|-----------|-----------
Open-LsaPolicy | [LsaOpenPolicy](https://learn.microsoft.com/en-us/windows/win32/api/ntsecapi/nf-ntsecapi-lsaopenpolicy) | computer name | bit mask |  | | |
Close-LsaPolicy | [LsaClose](https://learn.microsoft.com/en-us/windows/win32/api/ntsecapi/nf-ntsecapi-lsaclose) | | | mandatory | | | |
Get-LsaAccountRights | [LsaEnumerateAccountRights](https://learn.microsoft.com/en-us/windows/win32/api/ntsecapi/nf-ntsecapi-lsaenumerateaccountrights) | | | optional | user id | | |
Add-LsaAccountRights | [LsaAddAccountRights](https://learn.microsoft.com/en-us/windows/win32/api/ntsecapi/nf-ntsecapi-lsaaddaccountrights) | | | optional | user id | | right identifiers |
Remove-LsaAccountRights | [LsaRemoveAccountRights](https://learn.microsoft.com/en-us/windows/win32/api/ntsecapi/nf-ntsecapi-lsaremoveaccountrights) | | | optional | user id | optional | optional |

If PolicyHandle is omitted one for the local computer is used.

