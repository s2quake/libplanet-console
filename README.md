# Libplanet Console

This repository provides a `REPL` environment that enables easy testing of libplanet features.

## Requirements

```plain
dotnet 8.0
c# 12.0
```

## Clone

```sh
git clone https://github.com/s2quake/libplanet-console.git
cd libplanet-console
```

## Build

```sh
dotnet publish
```

> It's `publish`, not build.

# Run Console

Run 4 Node processes and 2 Client processes to control each of them.

```sh
.bin/libplanet-console
```

> If platform is windows, run `.bin\libplanet-console.exe`.

## Show Help

Display the run options for libplanet-console.

```sh
.bin/libplanet-console --help
```

## Save data

Specify a path to save the data.

```sh
.bin/libplanet-console --store-path .store
```

## Save logs

Specify a path to save the logs.

```sh
.bin/libplanet-console --log-path .log
```

## One node and One Client

Run one node and one client independently.

```sh
.bin/libplanet-node --end-point "127.0.0.1:4343"
.bin/libplanet-client --node-end-point "127.0.0.1:4343"
```
