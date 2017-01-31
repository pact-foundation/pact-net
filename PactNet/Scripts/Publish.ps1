param(
	[string]$pactBroker = $(throw "-pactBroker is required."),
	[string]$pactDir = $(throw "-pactDir is required."),
	[string]$appVersion = $(throw "-appVersion is required."),
	[string]$pactBrokerUser,
	[string]$pactBrokerPw
)

try
{
	Write-Host("PactDir: $pactDir")
	Write-Host("PactBroker: $pactBroker")
	Write-Host("AppVersion: $appVersion")
	Write-Host("PactBrokerUser: $pactBrokerUser")
	Write-Host("PactBrokerPassword: $pactBrokerPw")
	Write-Host("")
}
catch
{
	Write-Error $_.Message
	exit 1
}

function Convert-Version
{
	param([string]$semVer)

	$parts = $semVer.Split('.')
	"$($parts[0]).$($parts[1]).$($parts[2])"
}

function Publish-Pact
{
	param([string]$consumer, [string]$provider, [string]$content)

	Write-Output "Publishing Pact - Consumer: $($json.consumer.name), Provider: $($json.provider.name)"
	$version = Convert-Version -semVer $appVersion
	
	if ($pactBrokerUser)
	{
		$headers = @{ Authorization = "Basic {0}" -f [Convert]::ToBase64String([Text.Encoding]::ASCII.GetBytes(("{0}:{1}" -f $pactBrokerUser,$pactBrokerPw))) }
	}

	$uri = "$pactBroker/pacts/provider/$provider/consumer/$consumer/version/$version"
	
	try
	{
		$response = Invoke-WebRequest -UseBasicParsing -Uri $uri -Method Put -Headers $headers -Body $content -ContentType: application/json

		if (-not $response.StatusCode -eq 200)
		{
			throw "PactBroker returned an error status: $($response.StatusCode) - $($response.StatusDescription)"
		}
		else
		{
			Write-Output "Success"
		}
	}
	catch
	{
		Write-Error $_
		exit 1
	}
}

if (-not (Test-Path -Path $pactDir))
{
	Write-Error "Could not find path `"$pactDir`""
	exit 1
}

if ((Get-ChildItem $pactDir -Filter *.json | Measure-Object).Count -eq 0)
{
	Write-Warning "No pact files were found in the specified path. Please ensure that all pact files have a .json extension"
	exit 0
}

Get-ChildItem $pactDir -Filter *.json | 
Foreach-Object {
	try
	{
		Write-Output "Found pactfile: $($_.FullName | Split-Path -Leaf)"

		$content = Get-Content -Raw $_.FullName
		$json = $content | ConvertFrom-Json

		Publish-Pact -consumer $json.consumer.name -provider $json.provider.name $json. -content $content
	}
	catch
	{
		Write-Error $_
	}
}