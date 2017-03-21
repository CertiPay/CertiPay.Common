param (
	[string]$Configuration = 'Debug',
	
	[string]$Version = '1.0.0'
)

$ErrorActionPreference = "Stop"

$Here = "$(Split-Path -parent $MyInvocation.MyCommand.Definition)"

$Output = Join-Path $Here "artifacts"

$DotNet = "${env:ProgramFiles}\dotnet\dotnet.exe"

& $DotNet pack "CertiPay.Common" --configuration $Configuration --output $Output /p:Version=$Version

& $DotNet pack "CertiPay.Common.Notifications" --configuration $Configuration --output $Output /p:Version=$Version

& $DotNet pack "CertiPay.Common.Testing" --configuration $Configuration --output $Output /p:Version=$Version

EXIT $LASTEXITCODE