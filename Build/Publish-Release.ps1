param (
	[Parameter(Mandatory=$true)]
	[ValidatePattern('\d\.\d\.*')]
	[string]
	$ReleaseVersionNumber,

	[switch]$Push,
	
	[string]$ApiKey
)

$PSScriptFilePath = (Get-Item $MyInvocation.MyCommand.Path).FullName

$BuildRoot = Split-Path -Path $PSScriptFilePath -Parent
$SolutionRoot = Split-Path -Path $BuildRoot -Parent
$NuGetExe = Join-Path $BuildRoot -ChildPath '..\.nuget\nuget.exe'

# Build the NuGet package
$ProjectPath = Join-Path -Path $SolutionRoot -ChildPath 'PactNet\PactNet.nuspec'
& $NuGetExe pack $ProjectPath -Prop Configuration=Release -OutputDirectory $BuildRoot -Version $ReleaseVersionNumber
if (-not $?)
{
	throw 'The NuGet process returned an error code.'
}

# Upload the NuGet package
if ($Push)
{
	if($ApiKey)
	{
		& $NuGetExe setApiKey $ApiKey
	}

	$NuPkgPath = Join-Path -Path $BuildRoot -ChildPath "PactNet.$ReleaseVersionNumber.nupkg"
	& $NuGetExe push $NuPkgPath
	if (-not $?)
	{
		throw 'The NuGet process returned an error code.'
	}
}