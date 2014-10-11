param (
)

$BuildVersion = $env:APPVEYOR_BUILD_VERSION
$BuildNumber = $env:APPVEYOR_BUILD_NUMBER

$Version = $BuildVersion -replace ".$BuildNumber", ''
$AssemblyVersion = $Version -replace "[^0-9,.]", ''

$env:PACTNET_ASSEMBLY_VERSION = "$AssemblyVersion"

if($env:APPVEYOR_REPO_TAG -eq 'False')
{
	Write-Host "Testing: Hello"
}

if($env:APPVEYOR_REPO_TAG -eq 'True')
{
	$env:PACTNET_VERSION = "$Version"
}
else
{
	$env:PACTNET_VERSION = "$AssemblyVersion.$BuildNumber-beta"
}