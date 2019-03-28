#!/bin/bash

_scriptDir=$( cd "$(dirname "${BASH_SOURCE[0]}")" ; pwd -P )
source "$_scriptDir/CoreAgentInstallation.sh" "$@"

if [[ $_mode == "uninstall" ]] 
then
    uninstallagent
else
    installagent
fi