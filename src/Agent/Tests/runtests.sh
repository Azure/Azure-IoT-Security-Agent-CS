#!/bin/bash

# Find location of this script
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"

# Change project folder
cd $DIR

dotnet test Core