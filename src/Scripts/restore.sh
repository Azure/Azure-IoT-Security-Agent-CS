#!/bin/bash

pushd "$PWD"
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"

cd "$DIR/.."
dotnet restore --source https://api.nuget.org/v3/index.json
EX=$?

popd

if [ "$EX" -ne "0" ]; then
    echo "Failed to restore"
fi

exit $EX