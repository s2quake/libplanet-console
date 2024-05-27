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
cd libplanet-console
```

## Build

```sh
dotnet build
```

# Run Console

Run 4 Node processes and 2 Client processes to control each of them.

```sh
dotnet .bin/libplanet-console/libplanet-console.dll
```

> On arm64 windows, run `.bin/libplanet-console/libplanet-console.exe` without dotnet.

## Show Help

Display the run options for libplanet-console.

```sh
dotnet .bin/libplanet-console/libplanet-console.dll --help
```

## Save data

Specify a path to save the data.

```sh
dotnet .bin/libplanet-console/libplanet-console.dll --store-path .store
```

## Save logs

Specify a path to save the logs.

```sh
dotnet .bin/libplanet-console/libplanet-console.dll --log-path .log
```

## One node and One Client

Run one node and one client independently.

```sh
dotnet .bin/libplanet-node/libplanet-node.dll -a --end-point "127.0.0.1:4343"
dotnet .bin/libplanet-client/libplanet-client.dll --node-end-point "127.0.0.1:4343"
```
