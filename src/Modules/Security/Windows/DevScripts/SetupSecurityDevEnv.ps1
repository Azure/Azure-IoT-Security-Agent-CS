param (
	[string]$keyFileName = "c:\Key",
	[string]$key
)

function SetConfigParameter
{
	param( [System.Xml.Linq.XDocument]$xmlDoc, [string] $section, [string] $paramName, [string] $value )
	$xpath = "/$section/add[@key='$paramName']/@value"
	$node = [System.Xml.XPath.Extensions]::XPathEvaluate($xmlDoc.Root, $xpath)
	$node.SetValue($value)
}

function SetAuthParameter
{
	param( [System.Xml.Linq.XDocument]$xmlDoc, [string] $paramName, [string] $value )
	SetConfigParameter $xmlDoc "Authentication" $paramName $value
}

function UpdateSecurityConfiguration
{
	Set-Content -Path $keyFileName -Value $key

	$authenticationConfigPath = "$PSScriptRoot\..\Authentication.config"
	[System.Xml.Linq.XDocument]$authenticationXmlDoc = [System.Xml.Linq.XDocument]::Load($authenticationConfigPath)

	SetAuthParameter $authenticationXmlDoc "type" "SymmetricKey"
	SetAuthParameter $authenticationXmlDoc "identity" "Module"
	SetAuthParameter $authenticationXmlDoc "filePath" $keyFileName
	SetAuthParameter $authenticationXmlDoc "moduleName" "azureiotsecurity"
	

	$xStream = New-Object -TypeName System.IO.FileStream($authenticationConfigPath, [System.IO.FileMode]::Create, [System.IO.FileAccess]::ReadWrite, [System.IO.FileShare]::None)
    $authenticationXmlDoc.Save($xStream)  
	$xStream.Close();
}

Invoke-Expression "$PSScriptRoot/SetupDevEnv.ps1"

UpdateSecurityConfiguration