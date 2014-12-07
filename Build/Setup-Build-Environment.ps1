param (
)

$BuildNumber = $env:APPVEYOR_BUILD_NUMBER
$Branch = $env:APPVEYOR_REPO_BRANCH
$IsTagBuild = $env:APPVEYOR_REPO_TAG

$PactNetVersion
$PactNetAssemblyVersion

if($IsTagBuild -eq 'True')
{
	$PactNetVersion = "$Branch"
	$PactNetAssemblyVersion = ($Branch -replace "[^0-9,.]", '') + ".$BuildNumber"
}
else
{
	$PactNetVersion = "0.0.0.$BuildNumber-beta"
	$PactNetAssemblyVersion = "0.0.0.$BuildNumber"
}

$env:PACTNET_VERSION = $PactNetVersion
$env:PACTNET_ASSEMBLY_VERSION = $PactNetAssemblyVersion

Write-Host "env:PACTNET_VERSION = $PactNetVersion"
Write-Host "env:PACTNET_ASSEMBLY_VERSION = $PactNetAssemblyVersion"

Write-Host "### Printing local environment variables ###"
Write-Host (Get-ChildItem Env: | Format-List | Out-String)