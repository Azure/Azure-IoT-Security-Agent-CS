#!/bin/bash

_scriptDir=$( cd "$(dirname "${BASH_SOURCE[0]}")" ; pwd -P )
source "$_scriptDir/CoreSetupDevEnv.sh" "$@"

setupCore
