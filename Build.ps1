param (
	## The build target, i.e. Build, Rebuild, Clean
	[string]$Target = 'Build',

	## The build configuration, i.e. Debug/Release
	[string]$Configuration = 'Debug'
)

$ErrorActionPreference = "Stop"

$Here = "$(Split-Path -parent $MyInvocation.MyCommand.Definition)"

$SolutionFile = Join-Path $Here "CertiPay.sln"

$DotNet = "${env:ProgramFiles}\dotnet\dotnet.exe"

& $DotNet restore -v Minimal

& $DotNet build $SolutionFile --configuration $Configuration

EXIT $LASTEXITCODE