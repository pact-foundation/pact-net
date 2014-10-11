param (
	[switch]$GenerateSummaryReport
)

$PSScriptFilePath = (Get-Item $MyInvocation.MyCommand.Path).FullName
$BuildRoot = Split-Path -Path $PSScriptFilePath -Parent
$SolutionRoot = Split-Path -Path $BuildRoot -Parent

cd $SolutionRoot
& git submodule update --init
cd $BuildRoot

$NuGetExe = Join-Path $BuildRoot -ChildPath '..\.nuget\nuget.exe'
$OpenCoverExe = Join-Path $BuildRoot -ChildPath '..\packages\OpenCover.4.5.3207\OpenCover.Console.exe'
$XUnitExe = Join-Path $BuildRoot -ChildPath '..\packages\xunit.runners.1.9.2\tools\xunit.console.clr4.x86.exe'
$ReportGenExe = Join-Path $BuildRoot -ChildPath '..\packages\ReportGenerator.1.9.1.0\ReportGenerator.exe'

& $NuGetExe install "$SolutionRoot\.nuget\packages.config" -outputdirectory "$SolutionRoot\packages"

New-Item -ItemType directory -Path "$BuildRoot\coverage" -ErrorAction:ignore

& $OpenCoverExe `
    -register:user `
    "-target:$XUnitExe" `
    '-targetargs:..\PactNet.Tests\bin\Release\PactNet.Tests.dll /noshadow' `
    '-filter:+[PactNet]* -[*Tests]*' `
    '-output:.\coverage\results.xml'
    
if($GenerateSummaryReport)
{
	& $ReportGenExe '-reports:.\coverage\results.xml' '-reporttypes:HtmlSummary' "-targetdir:$BuildRoot\coverage"
}
else
{
	& $ReportGenExe '-reports:.\coverage\results.xml' "-targetdir:$BuildRoot\coverage"
}

cd $SolutionRoot
