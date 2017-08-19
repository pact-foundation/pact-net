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

$StandaloneCoreReleases = @('PactNet-Windows','PactNet-OSX','PactNet-Linux-x64','PactNet-Linux-x86')

foreach ($StandaloneCoreRelease in $StandaloneCoreReleases)
{
	# Build the NuGet package
	$ReleaseSource = $StandaloneCoreRelease.Replace('PactNet-', '').ToLower()
	$ProjectPath = Join-Path -Path $SolutionRoot -ChildPath "Build\$ReleaseSource\$StandaloneCoreRelease.nuspec"
	& $NuGetExe pack $ProjectPath -Properties Configuration=Release -OutputDirectory $BuildRoot -Version $ReleaseVersionNumber -NoPackageAnalysis -NoDefaultExcludes
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

		$NuPkgPath = Join-Path -Path $BuildRoot -ChildPath "$StandaloneCoreRelease.$ReleaseVersionNumber.nupkg"
		& $NuGetExe push $NuPkgPath
		if (-not $?)
		{
			throw 'The NuGet process returned an error code.'
		}
	}
}

