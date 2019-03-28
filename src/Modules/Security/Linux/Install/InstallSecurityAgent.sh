#!/bin/bash

_usageDescriptionExtension="[[-aui <authentication identity> ] [[-aum <authentication method> ] [[-f <file path> ] [[-hn <host name> ] [[-di <device id> ] [[-cl <certificate location kind> ]]"
_sudoersTemplateName="asotagentsudoers"
_sudoersIncludeDirectory="/etc/sudoers.d"
_baselineExecutableLocationTemplate="/BaselineExecutables/"
_omsBaselineExecutablePath="https://raw.githubusercontent.com/Microsoft/OMS-Agent-for-Linux/master/source/code/plugins/"
_baselineDirName="BaselineExecutables"
_omsAudits="oms_audits.xml"
_omsBaselineX64="omsbaseline_x64"
_omsBaselineX86="omsbaseline_x86"
_absoluteBaselineExecutableLocationTemplate=
_authenticationIdentity=
_authenticationType=
_filePath=
_gwFqdn=
_deviceId=
_certificateLocationKind=
_scriptDir=$( cd "$(dirname "${BASH_SOURCE[0]}")" ; pwd -P )

uninstallSecurityAgent()
{
	uninstallagent

	echo uninstalling agent [security]

	#remove the sudoers include file
    rm $_sudoersIncludeDirectory/$_sudoersTemplateName

	echo agent uninstallation finished [security]
}

installSecurityAgent()
{
	#Get oms executables according to systems architecture
	baselineDir="$_scriptDir/../$_baselineDirName"
	mkdir -p $baselineDir
	wgetAndExitOnFail "${_omsBaselineExecutablePath}${_omsAudits}" "$baselineDir/$_omsAudits"

    if [ $(uname -m) == 'x86_64' ]; then
		wgetAndExitOnFail "${_omsBaselineExecutablePath}${_omsBaselineX64}"  "$baselineDir/$_omsBaselineX64"
        _baselineExecutableLocationTemplate="${_baselineExecutableLocationTemplate}${_omsBaselineX64}"
		
    else
		wgetAndExitOnFail "${_omsBaselineExecutablePath}${_omsBaselineX86}" "$baselineDir/$_omsBaselineX86"
        _baselineExecutableLocationTemplate="${_baselineExecutableLocationTemplate}${_omsBaselineX86}"
    fi
    
    _absoluteBaselineExecutableLocationTemplate=$_targetDirectory$_baselineExecutableLocationTemplate

    prepareEnvironment

	#install security dependencies
    echo installing agent dependencies
    apt-get install -y auditd
    apt-get install -y audispd-plugins

    #add authentication configuration in app.config
    sed -i -e "s|\"identity\"\s*value=\"[^\"]*\"|\"identity\" value=\"$_authenticationIdentity\"|g" $_targetDirectory/Authentication.config 
	sed -i -e "s|\"type\"\s*value=\"[^\"]*\"|\"type\" value=\"$_authenticationType\"|g" $_targetDirectory/Authentication.config 
    sed -i -e "s|\"filePath\"\s*value=\"[^\"]*\"|\"filePath\" value=\"$_filePath\"|g" $_targetDirectory/Authentication.config 
	sed -i -e "s|\"gatewayHostname\"\s*value=\"[^\"]*\"|\"gatewayHostname\" value=\"$_gwFqdn\"|g" $_targetDirectory/Authentication.config 
    sed -i -e "s|\"deviceId\"\s*value=\"[^\"]*\"|\"deviceId\" value=\"$_deviceId\"|g" $_targetDirectory/Authentication.config 
	sed -i -e "s|\"moduleName\"\s*value=\"[^\"]*\"|\"moduleName\" value=\"azureiotsecurity\"|g" $_targetDirectory/Authentication.config 
	sed -i -e "s|\"certificateLocationKind\"\s*value=\"[^\"]*\"|\"certificateLocationKind\" value=\"$_certificateLocationKind\"|g" $_targetDirectory/Authentication.config 

	tempSudoersPath=$_scriptDir/sudoerstemp

    #replace variables in the sudoers file template
    sed -e "s|{USER}|$_userName|g" -e "s|{PATH_TO_OMSBASELINE_EXECUTABLE}|$_absoluteBaselineExecutableLocationTemplate|g" $_scriptDir/$_sudoersTemplateName > $tempSudoersPath
		
    visudo -csf $tempSudoersPath

    if [ $? != 0 ];
    then 
	echo sudoers editing failed, aborting
	exit $?
    fi

    cp $tempSudoersPath $_sudoersIncludeDirectory/$_sudoersTemplateName 

	#make baseline file executable
    chmod +x $_absoluteBaselineExecutableLocationTemplate

	installagent
}

originalArgs=("$@")
source "$_scriptDir/CoreAgentInstallation.sh" "$@"
set -- ${originalArgs[@]}

#parse command line arguments
while [ "$1" != "" ]; do
	case $1 in
		-aui | --authentication-identity )  shift
			if [ "$1" = "SecurityModule" ] || [ "$1" = "Device" ]; then
				_authenticationIdentity=$1
			else
				echo "Possible values for authentication-identity are: SecurityModule or Device"
				usage
				exit 1
			fi	
									;;
		-aum | --authentication-method )  shift
			if [ "$1" = "SymmetricKey" ] || [ "$1" = "SelfSignedCertificate" ]; then
				_authenticationType=$1
			else
				echo "Possible values for authentication-method are: SymmetricKey or SelfSignedCertificate"
				usage
				exit 1
			fi	
									;;		
		-f | --file-path )  shift
									_filePath=$1
									;;
		-hn | --host-name )  shift
									_gwFqdn=$1
									;;
		-di | --device-id)  shift
									_deviceId=$1
									;;
		-cl | --certificate-location-kind ) shift
			if [ "$1" = "LocalFile" ] || [ "$1" = "Store" ]; then
					_certificateLocationKind=$1
			else
				echo "Possible values for certificate-location-kind are: LocalFile or Store"
				usage
				exit 1
			fi	
	esac
	shift
done

# override defaults
_serviceName="ASCIoTAgent"
updateServiceNameDependentFields
_userName="asciotagent"
_serviceDescription="Azure Security Center for IoT Agent"
_execName="ASCIoTAgent"

_authenticationIdentity=${_authenticationIdentity/SecurityModule/Module}

if [[ $_mode == "uninstall" ]] 
then
    uninstallSecurityAgent
elif [[ $_mode == "install" ]] 
then
    installSecurityAgent
else
	usage
fi