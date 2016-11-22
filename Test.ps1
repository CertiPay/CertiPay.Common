param (
	[string]$Configuration = 'Debug'
)

# script/test is used to run the test suite of the application.

# A good pattern to support is having an optional argument that is a file path. This allows you to support running single tests.

# Linting (i.e. rubocop, jshint, pmd, etc.) can also be considered a form of testing. These tend to run faster than tests, so put them towards the beginning of a script/test so it fails faster if there's a linting problem.

# script/test should be called from script/cibuild, so it should handle setting up the application appropriately based on the environment. For example, if called in a development environment, it should probably call script/update to always ensure that the application is up to date. If called from script/cibuild, it should probably reset the application to a clean state.

$ErrorActionPreference = "Stop"

$Here = "$(Split-Path -parent $MyInvocation.MyCommand.Definition)"

$NuGet = Join-Path $Here ".nuget\nuget.exe"

$NUnitVersion = "3.5.0"
$NUnit = Join-Path $Here "packages\NUnit.ConsoleRunner.$NUnitVersion\tools\nunit3-console.exe"

# Get any assembly matching "CertiPay.*.Tests.dll" for now

$TestProjects = Get-ChildItem -Path "$Here\**\bin\$Configuration" -Recurse -Include "CertiPay.*.Tests.dll"

# Install the NUnit Runner

if(!(Test-Path $NUnit))
{
	& $NuGet install NUnit.Console -version $NUnitVersion -OutputDirectory "$Here\packages"
}

$Output = "$Here\UnitTestResults.xml"

# Run the unit tests in all of the test projects
# The task is set to run everything except tests marked [Integration] or [Category("Integration")]

& $NUnit @TestProjects --result:$Output --where "Category != Integration"

EXIT $LASTEXITCODE