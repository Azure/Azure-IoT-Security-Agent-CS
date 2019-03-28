#!/bin/bash

_connectionString=
_sudoersTemplateName="asotagentsudoers"
_sudoersIncludeDirectory="/etc/sudoers.d"
_baselineExecutableDir="BaselineExecutables"
_keyFileName="KeyFile"
_omsBaselineExecutableGitPath="https://raw.githubusercontent.com/Microsoft/OMS-Agent-for-Linux/master/source/code/plugins/"
_omsAudits="oms_audits.xml"
_omsBaselineX64="omsbaseline_x64"
_omsBaselineX86="omsbaseline_x86"
_usageDescriptionExtension="[[-cs <connection string> ] [[-u <user> ] "

wgetAndExitOnFail()
{
	wget "$1" -O "$2"
	if [ $? -ne 0 ]; then
		echo "Error: could not download $1, exiting"
		exit 1
	fi
}

setupSecurity()
{
	setupCore

    scriptDir=$( cd "$(dirname "${BASH_SOURCE[0]}")" ; pwd -P )

	#add authentication configuration in app.config
    _keyFileAbsPath="$scriptDir/../$_keyFileName"
    IFS='=;' read -r -a array <<< "$_connectionString"
    hostName=${array[1]}
	key=${array[7]}
    deviceId=${array[3]}
	echo $key > $_keyFileAbsPath


	authenticationConfigFile="$scriptDir/../Authentication.config"
    sed -i -e "s|\"moduleName\"\s*value=\"[^\"]*\"|\"moduleName\" value=\"azureiotsecurity\"|g" $authenticationConfigFile 
    sed -i -e "s|\"deviceId\"\s*value=\"[^\"]*\"|\"deviceId\" value=\"$deviceId\"|g" $authenticationConfigFile
    sed -i -e "s|\"gatewayHostname\"\s*value=\"[^\"]*\"|\"gatewayHostname\" value=\"$hostName\"|g" $authenticationConfigFile 
    sed -i -e "s|\"type\"\s*value=\"[^\"]*\"|\"type\" value=\"SymmetricKey\"|g" $authenticationConfigFile 
    sed -i -e "s|\"identity\"\s*value=\"[^\"]*\"|\"identity\" value=\"Module\"|g" $authenticationConfigFile 
    sed -i -e "s|\"filePath\"\s*value=\"[^\"]*\"|\"filePath\" value=\"$_keyFileAbsPath\"|g" $authenticationConfigFile 

    username=$1
    echo user is $username

    #find the architecture of the OS and point to the right baseline executable
    if [ $(uname -m) == 'x86_64' ]; then
        baselineExe="${_omsBaselineX64}"
    else
        baselineExe="${_omsBaselineX86}"
    fi
        
    absoluteBaselineExecutableDirLocation="$scriptDir/../$_baselineExecutableDir"
    absoluteBaselineExecutableLocation="$absoluteBaselineExecutableDirLocation/$baselineExe"

    mkdir -p "$scriptDir/../$_baselineExecutableDir"
    wgetAndExitOnFail "${_omsBaselineExecutableGitPath}${_omsAudits}" "$absoluteBaselineExecutableDirLocation/$_omsAudits"
    wgetAndExitOnFail "${_omsBaselineExecutableGitPath}${baselineExe}" "$absoluteBaselineExecutableLocation"

    #make baseline file executable
    chmod +x $absoluteBaselineExecutableLocation

    #replace variables in the sudoers file template
    sed -e "s|{USER}|$username|g" -e "s|{PATH_TO_OMSBASELINE_EXECUTABLE}|$absoluteBaselineExecutableLocation|g" $scriptDir/../Install/$_sudoersTemplateName > $_sudoersIncludeDirectory/$_sudoersTemplateName
}

_scriptDir=$( cd "$(dirname "${BASH_SOURCE[0]}")" ; pwd -P )
source "$_scriptDir/CoreSetupDevEnv.sh" "$@"

#parse command line arguments
while [ "$1" != "" ]; do
    case $1 in
			-cs | --connection_string )  shift
				             _connectionString=$1
  				             ;;
        -u | --user )               shift
                                    setupSecurity $1
                                    ;;
    esac
    shift
done
