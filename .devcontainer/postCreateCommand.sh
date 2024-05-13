#!/bin/bash

# Update submodules
git submodule update --init --

# Install libsnappy-dev for rocksdb
sudo apt update
sudo apt install -y libsnappy-dev