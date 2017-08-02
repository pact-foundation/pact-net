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
$OutputPath = Join-Path $SolutionRoot -ChildPath 'tools'

If (Test-Path $OutputPath)
{
    Write-Output "Standalone Core has already been downloaded"
    exit   
}

$StandaloneCoreVersion = '1.1.2'
$StandaloneCoreFileName = "pact-$StandaloneCoreVersion-win32"
$StandaloneCoreFilePath = "$OutputPath\$StandaloneCoreFileName.zip"
$StandaloneCoreDownload = "https://github.com/pact-foundation/pact-ruby-standalone/releases/download/v$StandaloneCoreVersion/$StandaloneCoreFileName.zip";

New-Item -ItemType directory -Path $OutputPath | Out-Null
Write-Output "Downloading the Standalone Core from '$StandaloneCoreDownload'..."
$Client = New-Object System.Net.WebClient
$Client.DownloadFile($StandaloneCoreDownload, $StandaloneCoreFilePath)
Write-Output "Done"

Write-Output "Unzipping the Standalone Core..."
Unzip "$StandaloneCoreFilePath" "$OutputPath"
Remove-Item $StandaloneCoreFilePath
Write-Output "Done"
