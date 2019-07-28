#!/bin/bash

pushd "$PWD"
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"

cd "$DIR/../Agent/Linux"
dotnet build --no-restore
agentLinuxEX=$?

cd "$DIR/../Modules/Security/Linux"
dotnet build --no-restore
securityLinuxEX=$?

popd

if [ "$agentLinuxEX" -ne "0" ] || [ "$securityLinuxEX" -ne "0" ]; 
then
    echo "Build failed"
	exit "1"
else
	exit "0"
fi
