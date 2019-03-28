#!/bin/bash

# Save current working directory
PWD=`pwd`
pushd $PWD

echo '-------- Build C (GCC) --------------------------'

# Find location of this script
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"

# Change project folder
cd $DIR/../

echo "Building agent in ..."
pwd

# build
dotnet publish -c Release -r "ubuntu.16.04-x64"

# Save exit code from gcc
EX=$?

# Check exit code and exit with it if it is non-zero so that build will fail
if [ "$EX" -ne "0" ]; then
    popd
    echo Failed to build
fi

# Restore working directory
popd 

# Exit with explicit 0 exit code so build will not fail
exit $EX
