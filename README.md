# Libplanet Console

This repository provides a `REPL` environment that enables easy testing of libplanet features.

## Requirements

```plain
dotnet 8.0
c# 12.0
```

## Clone

```sh
git clone https://github.com/s2quake/libplanet-console.git --recursive
```

## Build

```sh
dotnet build
```

# Run Console

```sh
dotnet .bin/libplanet-console/libplanet-console.dll
```

## Show Help

```sh
dotnet .bin/libplanet-console/libplanet-console.dll --help
```

## Node Count

```sh
dotnet .bin/libplanet-console/libplanet-console.dll --node-count 2
```

## Save states

```sh
dotnet .bin/libplanet-console/libplanet-console.dll --store-path .store
```

## Save logs

```sh
dotnet .bin/libplanet-console/libplanet-console.dll --log-path .log
```