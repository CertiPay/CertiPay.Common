param (
	## The build target, i.e. Build, Rebuild, Clean
	[string]$Target = 'Build',

	## The build configuration, i.e. Debug/Release
	[string]$Configuration = 'Debug'
)

$ErrorActionPreference = "Stop"

$Here = "$(Split-Path -parent $MyInvocation.MyCommand.Definition)"

$SolutionRoot = (Split-Path -parent $Here)

$SolutionFile = Join-Path $SolutionRoot "CertiPay.sln"

# Bootstap ensures we have what we need to build the project

$MSBuild = "${env:ProgramFiles(x86)}\MSBuild\14.0\Bin\msbuild.exe"

# Build the solution, packaging the files if requested

Write-Output "Running build target $Target for $Configuration"

# Switch /nodeReuse:false tells msbuild to stop after completing the build, avoiding conditions on the build server where it kept packages locked

& $MSBuild $SolutionFile /v:quiet /t:$Target /m /p:Configuration=$Configuration /nodeReuse:false

EXIT $LASTEXITCODE