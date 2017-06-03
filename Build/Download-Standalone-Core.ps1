param (
)

Add-Type -AssemblyName System.IO.Compression.FileSystem
function Unzip
{
    param([string]$zipfile, [string]$outpath)

    [System.IO.Compression.ZipFile]::ExtractToDirectory($zipfile, $outpath)
}

$PSScriptFilePath = (Get-Item $MyInvocation.MyCommand.Path).FullName
$BuildRoot = Split-Path -Path $PSScriptFilePath -Parent
$SolutionRoot = Split-Path -Path $BuildRoot -Parent

$MockProviderFileName = 'pact-mock-service-2.1.0-1-win32'
$ProviderVerifierFileName = 'pact-provider-verifier-1.1.3-1-win32'
$OutputPath = Join-Path $SolutionRoot -ChildPath 'PactNet\Core\standalone'
$Client = New-Object System.Net.WebClient

$MockProviderFilePath = "$OutputPath\$MockProviderFileName.zip"
$MockProviderDownload = "https://github.com/pact-foundation/pact-mock_service/releases/download/v2.1.0/$MockProviderFileName.zip"
$ProviderVerifierFilePath = "$OutputPath\$ProviderVerifierFileName.zip"
$ProviderVerifierDownload = "https://github.com/pact-foundation/pact-provider-verifier/releases/download/v1.1.3-1/$ProviderVerifierFileName.zip"

If(!(Test-Path $OutputPath))
{
	New-Item -ItemType directory -Path $OutputPath | Out-Null
}

Write-Output "Downloading the Mock Provider Service from '$MockProviderDownload'..."
$Client.DownloadFile($MockProviderDownload, $MockProviderFilePath)
Write-Output "Done"

Write-Output "Downloading the Provider Verifier from '$ProviderVerifierDownload'..."
$Client.DownloadFile($ProviderVerifierDownload, $ProviderVerifierFilePath)
Write-Output "Done"

Write-Output "Unzipping the Mock Provider Service..."
Unzip "$MockProviderFilePath" "$OutputPath"
Rename-Item "$OutputPath\$MockProviderFileName" "pact-mock-service-win32"
Remove-Item $MockProviderFilePath
Write-Output "Done"

Write-Output "Unzipping the Provider Verifier..."
Unzip "$ProviderVerifierFilePath" "$OutputPath"
Rename-Item "$OutputPath\$ProviderVerifierFileName" "pact-provider-verifier-win32"
Remove-Item $ProviderVerifierFilePath
Write-Output "Done"
