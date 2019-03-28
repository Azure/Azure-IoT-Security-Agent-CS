param (
	[Parameter(Mandatory)]
    [string]
	$serviceName = "Azure IoT Agent",

	[Parameter(Mandatory, ParameterSetName = "Install")]
    [switch]
	$Install,

	[Parameter(Mandatory, ParameterSetName = "Uninstall")]
    [switch]
	$Uninstall,

	[Parameter(ParameterSetName = "Install")]
	[string]
	$mainModule = "Agent.Windows.exe"
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

function GiveServiceUserFullControlOnPublishFolder
{
    $publishFolder = "$PSScriptRoot\.."
    $acl = Get-Acl $publishFolder
    $accessRule = New-Object  System.Security.AccessControl.FileSystemAccessRule($serviceUser,"FullControl", "ContainerInherit,ObjectInherit", "None", "Allow")
    $acl.SetAccessRule($accessRule)
    Set-Acl $publishFolder $acl
}

function Uninstall
{
    sc.exe stop $serviceName
    sc.exe delete $serviceName
}

function Install
{
	$generalConfigPath = "$PSScriptRoot\..\General.config"
    [System.Xml.Linq.XDocument]$generalXmlDoc = [System.Xml.Linq.XDocument]::Load($generalConfigPath)

    SetGeneralParameter $generalXmlDoc "agentId" ([GUID]::NewGuid()).ToString()

	$xStream = New-Object -TypeName System.IO.FileStream($generalConfigPath, [System.IO.FileMode]::Create, [System.IO.FileAccess]::ReadWrite, [System.IO.FileShare]::None)
    $generalXmlDoc.Save($xStream)
	$xStream.Close();
 
    sc.exe create $serviceName start=auto binPath="$PSScriptRoot\..\"$mainModule
    sc.exe start $serviceName
}

if ($Install)
{
    Install
}
elseif ($Uninstall)
{
 	Uninstall
}
else
{
	throw "Unknown script mode"
}
