#!/bin/bash

usage()
{
    echo "usage: setupDevEnv $_usageDescriptionExtension| [-h]]"
}

setupCore()
{
    scriptDir=$( cd "$(dirname "${BASH_SOURCE[0]}")" ; pwd -P )
   
    generalConfigFile="$scriptDir/../General.config"
	agentId=$(uuidgen)
	sed -i -e "s|\"agentId\"\s*value=\"[^\"]*\"|\"agentId\" value=\"$agentId\"|g" $generalConfigFile
}

#parse command line arguments
while [ "$1" != "" ]; do
    case $1 in
	        -h | --help )                shift
		                             usage
                                             exit
    esac
    shift
done

