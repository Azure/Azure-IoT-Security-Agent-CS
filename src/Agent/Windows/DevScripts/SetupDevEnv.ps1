param (
)

function SetConfigParameter
{
    param( [System.Xml.Linq.XDocument]$xmlDoc, [string] $section, [string] $paramName, [string] $value )
    $xpath = "/$section/add[@key='$paramName']/@value"
    $node = [System.Xml.XPath.Extensions]::XPathEvaluate($xmlDoc.Root, $xpath)
	$node.SetValue($value)
}

function SetGeneralParameter
{
	param( [System.Xml.Linq.XDocument]$xmlDoc, [string] $paramName, [string] $value )
    SetConfigParameter $xmlDoc "General" $paramName $value
}

function UpdateConfiguration
{
	$generalConfigPath = "$PSScriptRoot\..\General.config"
	[System.Xml.Linq.XDocument]$generalXmlDoc = [System.Xml.Linq.XDocument]::Load($generalConfigPath)

	SetGeneralParameter $generalXmlDoc "agentId" ([GUID]::NewGuid()).ToString()

	$xStream = New-Object -TypeName System.IO.FileStream($generalConfigPath, [System.IO.FileMode]::Create, [System.IO.FileAccess]::ReadWrite, [System.IO.FileShare]::None)
    $generalXmlDoc.Save($xStream)  
	$xStream.Close();    
}

try
{
	[Reflection.Assembly]::LoadWithPartialName("System.Xml.Linq") | Out-Null
}
catch [System.Exception]
{
    "Unable to load System.Xml.Linq, assuming Windows IoT Core"
}

UpdateConfiguration