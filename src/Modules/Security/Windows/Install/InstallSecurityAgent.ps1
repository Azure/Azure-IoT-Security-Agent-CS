<#
.SYNOPSIS
    Installs the Azure Security Center agent
.PARAMETER Install
    Starts the script in install mode
.PARAMETER Uninstall
    Starts the script in uninstall mode
.PARAMETER AuthenticationIdentity
    Which IoT identity to use for authentication, specify either Device or SecurityModule
.PARAMETER AuthenticationMethod
    What kind of authentication method to use, specify eithe SymmetricKey or SelfSignedCertificate, must match the the contents of the file given in the filePath parameter
.PARAMETER FilePath
    A path to a file containing the information needed to authenticate, either a shared access key or a certificate, the type of information must match the authenticationMethod paramteter
.PARAMETER HostName
    The IoT hub's host name
.PARAMETER DeviceId
    The ID of the IoT device
.PARAMETER CertificateLocationKind
	The location of the certificate, can be either LocalFile if the certificate is located in a file, or Store if the certificate is located in the machine certificate store
.PARAMETER IdScope
	The ID Scope of the DPS service
.PARAMETER RegistrationId
	The registration ID of the device, as registered in the DPS service
.EXAMPLE
	.\InstallSecurityAgent.ps1 -Install -AuthenticationIdentity SecurityModule -AuthenticationMethod SymmetricKey -FilePath "C:\SymmetricKey.txt" -HostName "contoso-iot-hub.azure-devices.net" -DeviceId contoso1
#>

param (
	[Parameter(Mandatory, ParameterSetName = "Uninstall")]
    [switch]
	$Uninstall, 

	[Parameter(Mandatory, ParameterSetName = "InstallDPS")]
	[Parameter(Mandatory, ParameterSetName = "Install")]
    [switch]
	$Install,

	[Parameter(Mandatory, ParameterSetName = "InstallDPS")]
	[Parameter(Mandatory, ParameterSetName = "Install")]
    [ValidateSet("Device", "SecurityModule", "DPS")]
	[Alias("aui")]
	[string]
	$AuthenticationIdentity, 
	
	[Parameter(Mandatory, ParameterSetName = "InstallDPS")]
	[Parameter(Mandatory, ParameterSetName = "Install")]
    [ValidateSet("SymmetricKey", "SelfSignedCertificate")]
	[Alias("aum")]
	[string]
	$AuthenticationMethod,

	[Parameter(Mandatory, ParameterSetName = "InstallDPS")]
	[Parameter(Mandatory, ParameterSetName = "Install")]
	[Alias("f")]
    [string]
	$FilePath,

	[Parameter(Mandatory, ParameterSetName = "Install")]
	[Alias("hn")]
    [string]
	$HostName,

	[Parameter(Mandatory, ParameterSetName = "Install")]
	[Alias("di")]
    [string]
	$DeviceId,

	[Parameter(Mandatory, ParameterSetName = "InstallDPS")]
	[Alias("is")]
    [string]
	$IdScope,

	[Parameter(Mandatory, ParameterSetName = "InstallDPS")]
	[Alias("ri")]
    [string]
	$RegistrationId,

	[Parameter(ParameterSetName = "InstallDPS")]
	[Parameter(ParameterSetName = "Install")]
	[ValidateSet("LocalFile", "Store")]
	[Alias("cl")]
	[string]
	$CertificateLocationKind
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
    $authenticationConfigPath = "$PSScriptRoot\..\Authentication.config"
	[System.Xml.Linq.XDocument]$authenticationXmlDoc = [System.Xml.Linq.XDocument]::Load($authenticationConfigPath)

	$filePath = if($FilePath -match "^[a-zA-Z]:\*") {$FilePath} else {"$($PWD.path)\$($FilePath)"}
	if(!(Test-Path -path $filePath)){
		throw "File $($filePath) does not exist!"
	}
	SetAuthParameter $authenticationXmlDoc "filePath" $filePath
	SetAuthParameter $authenticationXmlDoc "idScope" $IdScope
	SetAuthParameter $authenticationXmlDoc "registrationId" $RegistrationId
    SetAuthParameter $authenticationXmlDoc "deviceId" $DeviceId
    SetAuthParameter $authenticationXmlDoc "gatewayHostname" $HostName
	SetAuthParameter $authenticationXmlDoc "moduleName" "azureiotsecurity"
	SetAuthParameter $authenticationXmlDoc "certificateLocationKind" $CertificateLocationKind
	SetAuthParameter $authenticationXmlDoc "type" $AuthenticationMethod


	$identityValue =  if($AuthenticationIdentity -eq "SecurityModule") {"Module"} else {$AuthenticationIdentity}
    SetAuthParameter $authenticationXmlDoc "identity" $identityValue


	$xStream = New-Object -TypeName System.IO.FileStream($authenticationConfigPath, [System.IO.FileMode]::Create, [System.IO.FileAccess]::ReadWrite, [System.IO.FileShare]::None)
    $authenticationXmlDoc.Save($xStream)  
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

if ($AuthenticationMethod -eq "SelfSignedCertificate" -and !$CertificateLocationKind)
{
	throw "AuthenticationMethod is set to $AuthenticationMethod but CertificateLocationKind wasn't specified"
}

UpdateSecurityConfiguration

$modeSwitch = if ($Install) {"-Install"} elseif ($Uninstall) {"-Uninstall"} else {throw "Unknown script mode"}
$baseScriptArgs = "$modeSwitch -serviceName ""ASC IoT Agent"""
if ($Install)
{
	$baseScriptArgs += " -mainModule ""ASCIoTAgent.exe"""
}

Invoke-Expression "$PSScriptRoot/InstallAgent.ps1 $baseScriptArgs"

