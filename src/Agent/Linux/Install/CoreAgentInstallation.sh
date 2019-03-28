#!/bin/bash

_serviceName="MSIoTAgent"
_serviceTemplateName="$_serviceName.service"
_targetDirectory="/var/$_serviceName"
_userName="msiotagent"
_serviceDescription="Microsoft IoT Agent"
_systemServiceFileLocation="/etc/systemd/system"
_mode=
_execName="Agent.Linux"
_scriptDir=$( cd "$(dirname "${BASH_SOURCE[0]}")" ; pwd -P )
_nice=10

usage()
{
    echo "usage: InstallAgent $_usageDescriptionExtension[-i | -u] | [-h]]"
}

wgetAndExitOnFail()
{
	wget "$1" -O "$2"
	if [ $? -ne 0 ]; then
		echo "Error: could not download $1, exiting"
		exit 1
	fi
}

uninstallagent()
{
    echo uninstalling agent [core]

    #stop the daemon
    systemctl stop $_serviceTemplateName

	#disable the service
	systemctl disable $_serviceTemplateName

    #remove the agent files
    rm -rf $_targetDirectory 

    #remove the service user
    deluser --remove-home --remove-all-files $_userName

    #remove the service configuration
    rm $_systemServiceFileLocation/$_serviceTemplateName

    echo agent uninstallation finished [core]
}

updateServiceNameDependentFields()
{
     _serviceTemplateName="$_serviceName.service"
     _targetDirectory="/var/$_serviceName"
}

prepareEnvironment()
{
    #install dependencies
    echo installing core agent dependencies

	apt-get install -y libunwind8
    apt-get install -y libcurl3
	apt-get install -y uuid-runtime

    echo preparding core agent environment
    
    #add the service user
    echo creating service user 
    adduser --disabled-login --gecos "$_serviceName" $_userName

    #populate the target directory with the agent files 
    mkdir $_targetDirectory
    cp -r $_scriptDir/../* $_targetDirectory

	#generate device agent
	agentId=$(uuidgen)
	sed -i -e "s|\"agentId\"\s*value=\"[^\"]*\"|\"agentId\" value=\"$agentId\"|g" $_targetDirectory/General.config
	
    #make the agent user ownder of the target directory
    chown -R $_userName:$_userName $_targetDirectory
    
    #make agent executable
    chmod +x $_targetDirectory/$_execName
    
    #replace variables in the service file template
    sed -e "s|{DIRECTORY}|$_targetDirectory|g" -e "s|{USER}|$_userName|g" -e "s|{GROUP}|$_userName|g" -e "s|{DESCRIPTION}|$_serviceDescription|g" -e "s|{EXECNAME}|$_execName|g" -e "s|{NICENESS}|$_nice|g" $_scriptDir/Template.service > $_systemServiceFileLocation/$_serviceTemplateName
}

installagent()
{
	echo installing agent
  
    #reload the deamon, in case is was already installed before    
    systemctl daemon-reload

	#enable the service so that it starts on boot
	systemctl enable $_serviceTemplateName

    #start the service
    systemctl start $_serviceTemplateName

    echo agent installation finished
}

#parse command line arguments
while [ "$1" != "" ]; do
	case $1 in
		-u | --uninstall )          shift
									_mode="uninstall"
									;;
		-i | --install )            shift
									_mode="install"
									;;
		-d | --description )        shift
									_serviceDescription=$1
									;;			
    	-s | --service_name )       shift
									_serviceName=$1
									updateServiceNameDependentFields
									;;		
	    -un | --user_name )			shift
									_userName=$1
									;;			
		-n | --nice )				shift
									_nice=$1
									;;			
		-h | --help )               usage
									exit
									;;
	esac
	shift
done