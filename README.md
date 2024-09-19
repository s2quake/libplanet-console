# Libplanet Console

This repository provides a `REPL` environment that enables easy testing of 
libplanet features.

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

# Creating Repository

Run the following command to create a repository for starting 4 nodes 
and 2 clients at the specified path.

```sh
.bin/libplanet-console init .data
```

# Staring Repository

Run the following command to start nodes and clients at the specified path.

```sh
.bin/libplanet-console start .data
```

> If platform is windows, run `.bin\libplanet-console.exe`.

## Single Node

Create and run a repository for a single node.

```sh
.bin/libplanet-node init .node --single-node
.bin/libplanet-node start .node
```

## Settings data

Once the repository is created, the specified path will contain 
`node-settings.json` and `client-settings.json` files as shown below. 
Users can configure various values to run nodes and clients 
in different environments.

```json
{
  "$schema": "node-settings-schema.json",
  "application": {
    "endPoint": "localhost:55314",
    "privateKey": "5a3df2ce7fc8b8f7c984f867a34e7d343e974f7b661c83536c0a66685bdbf04a",
    "storePath": "store",
    "genesisPath": "genesis",
    "logPath": "app.log",
    "libraryLogPath": "library.log"
  }
}
```

## Show Help

Run the following command to display help

```sh
.bin/libplanet-console --help
```
