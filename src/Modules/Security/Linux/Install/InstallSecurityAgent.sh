#!/bin/bash

_sudoersTemplateName="asotagentsudoers"
_sudoersIncludeDirectory="/etc/sudoers.d"
_baselineExecutableLocationTemplate="/BaselineExecutables/"
_omsBaselineExecutablePath="https://ascforiot.blob.core.windows.net/public/"
_baselineDirName="BaselineExecutables"
_omsAudits="oms_audits.xml"
_omsBaseline="omsbaseline"
_omsBaselineX64="omsbaseline-linux-amd64"
_omsBaselineI386="omsbaseline-linux-386"
_omsBaselineARMv7="omsbaseline-linux-arm-v7"
_absoluteBaselineExecutableLocationTemplate=
_authenticationIdentity=
_authenticationType=
_filePath=
_gwFqdn=
_deviceId=
_certificateLocationKind=
_idScope=
_registrationId=
_scriptDir=$( cd "$(dirname "${BASH_SOURCE[0]}")" ; pwd -P )
_installUsageExtendedDescription=" -aui <authentication identity> -aum <authentication method> -f <file path> -hn <host name> -di <device id> [-cl <certificate location kind>]"
_optionsExtendedDescription="\
  -aui --authentication-identity    The authentication identity used by the agent (SecurityModule, Device or DPS).\n\
  -aum --authentication-method      The authentication method used by the agent (SymmetricKey or SelfSignedCertificate).\n\
  -f   --file-path                  Path to a file from which data related to the authentication method should be read (the key, certificate or certificate-from-store-info JSON).\n\
  -hn  --host-name                  IoT hub's host name.\n\
  -di  --device-id                  Id of the device the agent is beind installed on (as defined in the IoT hub).\n\
  -cl  --certificate-location-kind  Location kind of certificate being used (LocalFile or Store). Applicable only when the authentication method is set to SelfSignedCertificate.\n\
  -is  --id-scope					The ID Scope of the DPS service\n\
  -ri  --registration-id			The registration ID of the device, as registered in the DPS service"

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
    shouldExit=false
    
	if [ -z "$_authenticationIdentity" ]
    then
		echo "authentication-identity not provided"
        shouldExit=true
	fi

    if [ -z "$_authenticationType" ]
    then
		echo "authentication-method not provided"
        shouldExit=true
	fi

	if [ $_authenticationType == 'SelfSignedCertificate' ]
	then
		if [ -z "$_certificateLocationKind" ]
		then 
			echo "certificate-location-kind not provided"
			shouldExit=true
		fi
	fi

	if [ -z "$_filePath" ]
	then 
		echo "file-path not provided"
        shouldExit=true
	fi

	if [ $_authenticationIdentity == 'DPS' ]
	then
		if [ -z "$_idScope" ]
		then 
			echo "id-scope not provided"
			shouldExit=true
		fi

		if [ -z "$_registrationId" ]
		then 
			echo "registration-id not provided"
			shouldExit=true
		fi
	else
		if [ -z "$_gwFqdn" ]
		then 
			echo "host-name not provided"
			shouldExit=true
		fi

		if [ -z "$_deviceId" ]
		then 
			echo "device-id not provided"
			shouldExit=true
		fi
	fi

    if $shouldExit
    then 
        echo "cannot intall the agent, please check the validity of the supplied parameters"
        exit 1
    fi
    
	#Get oms executables according to systems architecture
	baselineDir="$_scriptDir/../$_baselineDirName"
	baselinePath="$baselineDir/$_omsBaseline"
	mkdir -p $baselineDir

	# download oms_audits.xml
	wgetAndExitOnFail "${_omsBaselineExecutablePath}${_omsAudits}" "$baselineDir/$_omsAudits"

	# download omsbaseline supported architecture
	case $(uname -m) in
    'x86_64')
		wgetAndExitOnFail "${_omsBaselineExecutablePath}${_omsBaselineX64}"  $baselinePath
		;;
	'armv7l')
		wgetAndExitOnFail "${_omsBaselineExecutablePath}${_omsBaselineARMv7}"  $baselinePath
		;;
	'i368')
		wgetAndExitOnFail "${_omsBaselineExecutablePath}${_omsBaselineI386}"  $baselinePath
		;;
    *)
		echo "Not supported architecture"
        exit 1
		;;
	esac
    
	_baselineExecutableLocationTemplate="${_baselineExecutableLocationTemplate}${_omsBaseline}"
    _absoluteBaselineExecutableLocationTemplate=$_targetDirectory$_baselineExecutableLocationTemplate

    prepareEnvironment

	#install security dependencies
    echo installing agent dependencies
    apt-get install -y \
		auditd \
		audispd-plugins \
		net-tools

    #add authentication configuration in app.config
    sed -i -e "s|\"identity\"\s*value=\"[^\"]*\"|\"identity\" value=\"$_authenticationIdentity\"|g" $_targetDirectory/Authentication.config 
	sed -i -e "s|\"type\"\s*value=\"[^\"]*\"|\"type\" value=\"$_authenticationType\"|g" $_targetDirectory/Authentication.config 
    sed -i -e "s|\"filePath\"\s*value=\"[^\"]*\"|\"filePath\" value=\"$_filePath\"|g" $_targetDirectory/Authentication.config 
	sed -i -e "s|\"moduleName\"\s*value=\"[^\"]*\"|\"moduleName\" value=\"azureiotsecurity\"|g" $_targetDirectory/Authentication.config 
	sed -i -e "s|\"certificateLocationKind\"\s*value=\"[^\"]*\"|\"certificateLocationKind\" value=\"$_certificateLocationKind\"|g" $_targetDirectory/Authentication.config
	if [ $_authenticationIdentity == 'DPS' ]; then
		sed -i -e "s|\"idScope\"\s*value=\"[^\"]*\"|\"idScope\" value=\"$_idScope\"|g" $_targetDirectory/Authentication.config 
		sed -i -e "s|\"registrationId\"\s*value=\"[^\"]*\"|\"registrationId\" value=\"$_registrationId\"|g" $_targetDirectory/Authentication.config
	else 
		sed -i -e "s|\"gatewayHostname\"\s*value=\"[^\"]*\"|\"gatewayHostname\" value=\"$_gwFqdn\"|g" $_targetDirectory/Authentication.config 
		sed -i -e "s|\"deviceId\"\s*value=\"[^\"]*\"|\"deviceId\" value=\"$_deviceId\"|g" $_targetDirectory/Authentication.config
	fi

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

setFilePath(){
	file=$1
	dir=$(pwd)
	if [[ $file == /* ]]
	then
		_filePath=$file
	else
		_filePath=$dir/$file
	
	fi
	if [ ! -f $_filePath ]; then
		echo "File $_filePath does not exist!";
		exit 1;
	fi
}

#parse command line arguments
while [ "$1" != "" ]; do
	case $1 in
		-aui | --authentication-identity )  shift
			if [ "$1" = "SecurityModule" ] || [ "$1" = "Device" ] || [ "$1" = "DPS" ]; then
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
									setFilePath $1
									;;
		-hn | --host-name )  shift
									_gwFqdn=$1
									;;
		-di | --device-id)  shift
									_deviceId=$1
									;;
		-is | --id-scope)  shift
									_idScope=$1
									;;
		-ri | --registration-id)  shift
									_registrationId=$1
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