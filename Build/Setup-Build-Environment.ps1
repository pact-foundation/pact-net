param (
)

$BuildNumber = $env:APPVEYOR_BUILD_NUMBER
$IsTagBuild = $env:APPVEYOR_REPO_TAG

$PactNetVersion
$PactNetAssemblyVersion

if($IsTagBuild -eq 'True')
{
	$TagName = $env:APPVEYOR_REPO_TAG_NAME #was APPVEYOR_REPO_BRANCH
	$PactNetVersion = "$TagName"
	$PactNetAssemblyVersion = ($TagName -replace "[^0-9,.]", '') + ".$BuildNumber"
}
else
{
	$PactNetVersion = "0.0.0.$BuildNumber-beta"
	$PactNetAssemblyVersion = "0.0.0.$BuildNumber"
}

$env:PACTNET_VERSION = $PactNetVersion
$env:PACTNET_ASSEMBLY_VERSION = $PactNetAssemblyVersion

Write-Host "Set env:PACTNET_VERSION = $PactNetVersion"
Write-Host "Set env:PACTNET_ASSEMBLY_VERSION = $PactNetAssemblyVersion"