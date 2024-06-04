#!/bin/sh

workspace=$(realpath $(dirname "$0"))
filename=$workspace/.bin/libplanet-console

if [ ! -f $filename ]; then
  echo "'$filename' not found."
  echo "Please run 'dotnet publish' first."
  exit 1
fi

$filename "$@"