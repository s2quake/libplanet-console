$filename = Join-Path $PSScriptRoot ".bin/libplanet-console/libplanet-console.dll"

if (!(Test-Path -Path $filename)) {
  throw "'$filename' not found.`nPlease run 'dotnet build' first."
}

dotnet "$filename" $args