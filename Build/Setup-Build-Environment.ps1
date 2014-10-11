param (
)

$BuildVersion = $env:APPVEYOR_BUILD_VERSION
$BuildNumber = $env:APPVEYOR_BUILD_NUMBER

$Version = $BuildVersion -replace ".$BuildNumber", ''
$AssemblyVersion = $Version -replace "[^0-9,.]", ''

$env:PACTNET_ASSEMBLY_VERSION = "$AssemblyVersion"

if($env:APPVEYOR_REPO_TAG)
{
	$env:PACTNET_VERSION = "$Version"
}
else
{
	$env:PACTNET_VERSION = "$BuildVersion"
}