#!/bin/bash

pushd "$PWD"
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"

cd "$DIR"
dotnet test --logger "trx;LogFileName=AgentCore.trx" --results-directory TestResults ../Agent/Tests/Core
agentCoreEX=$?

dotnet test --logger "trx;LogFileName=SecurityCommon.trx" --results-directory TestResults ../Modules/Security/Tests/Common
securityCommonEX=$?

dotnet test --logger "trx;LogFileName=SecurityLinux.trx" --results-directory TestResults ../Modules/Security/Tests/Linux
securityLinuxEX=$?

popd

if [ "$agentCoreEX" -ne "0" ] || [ "$securityCommonEX" -ne "0" ] || [ "$securityLinuxEX" -ne "0" ]; 
then
    echo "Tests failed"
	exit "1"
else
	exit "0"
fi
