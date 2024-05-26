#!/bin/sh

workspace=$(realpath $(dirname "$0"))
filename=$workspace/.bin/libplanet-console/libplanet-console.dll
echo $filename

if [ ! -f $filename ]; then
  echo -e "'$filename' not found.\nPlease run 'dotnet build' first."
  exit 1
fi

dotnet $filename "$@"