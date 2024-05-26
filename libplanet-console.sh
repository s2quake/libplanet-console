#!/bin/sh

workspace=$(realpath $(dirname "$0"))
filename=$workspace/.bin/libplanet-console/libplanet-console.dll

if [ ! -f $filename ]; then
  echo "'$filename' not found."
  echo "Please run 'dotnet build' first."
  exit 1
fi

dotnet $filename "$@"