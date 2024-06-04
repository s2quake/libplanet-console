if ($IsWindows ) {
  $filename = Join-Path $PSScriptRoot ".bin/libplanet-console.exe"
}
else {
  $filename = Join-Path $PSScriptRoot ".bin/libplanet-console"
}

if (!(Test-Path -Path $filename)) {
  throw "'$filename' not found.`nPlease run 'dotnet publish' first."
}

Invoke-Expression "& '$filename' $args" 