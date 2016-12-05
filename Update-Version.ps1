param (
	[string]$Version = $(throw "Version is required")
)

$Here = "$(Split-Path -parent $MyInvocation.MyCommand.Definition)"

$Projects = Get-ChildItem -Path $Here -Recurse -Include "project.json"

foreach($Project in $Projects)
{
	$JSON = (Get-Content $Project) | ConvertFrom-Json

	$JSON.version = $Version

	ConvertTo-Json $JSON | Out-File $Project -Encoding utf8
}