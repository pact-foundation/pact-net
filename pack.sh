#!/usr/bin/env bash

dotnet pack src/PactNet.Output.Xunit
dotnet pack src/PactNet.Abstractions
dotnet pack src/PactNet.runtime.linux-musl-x64
dotnet pack src/PactNet.runtime.linux-x64
dotnet pack src/PactNet.runtime.osx-arm64
dotnet pack src/PactNet.runtime.osx-x64
dotnet pack src/PactNet.runtime.win-x64
dotnet pack src/PactNet
