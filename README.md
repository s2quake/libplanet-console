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

# Run Node

```sh
dotnet src/node/LibplanetConsole.NodeHost/bin/Debug/net8.0/libplanet-node.dll -a --end-point 127.0.0.1:5353
```

> Option `-a` automatically starts the node when the application runs

# Run Client

```sh
dotnet src/client/LibplanetConsole.ClientHost/bin/Debug/net8.0/libplanet-client.dll --node-end-point 127.0.0.1:5353
```

# Run Console

```sh
dotnet src/console/LibplanetConsole.ConsoleHost/bin/Debug/net8.0/libplanet-console.dll
```
