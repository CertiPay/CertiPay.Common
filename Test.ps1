param (
	[string]$Configuration = 'Debug'
)

$ErrorActionPreference = "Stop"

$Here = "$(Split-Path -parent $MyInvocation.MyCommand.Definition)"

$NuGet = Join-Path $Here ".nuget\nuget.exe"

$NUnitVersion = "3.6.1"
$NUnit = Join-Path $Here "packages\NUnit.ConsoleRunner.$NUnitVersion\tools\nunit3-console.exe"

& $NuGet install NUnit.Console -version $NUnitVersion -OutputDirectory "$Here\packages"

# Get any assembly matching "CertiPay.*.Tests.dll" for now

$TestProjects = Get-ChildItem -Path "$Here\**\bin\$Configuration" -Recurse -Include "CertiPay.*.Tests.dll"

# Run the unit tests in all of the test projects
# The task is set to run everything except tests marked [Integration] or [Category("Integration")]

& $NUnit @TestProjects --where "Category != Integration"

EXIT $LASTEXITCODE