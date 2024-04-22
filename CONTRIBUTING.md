Contributing to PactNet
=======================

Raising Issues
--------------

Please don't raise issues for general queries (e.g. usage queries) and instead check the [Pact Docs] site or ask on the [Pact Foundation Slack].

Before raising any issues, please make as much effort as you can to rule out issues in your own environment as much as possible.
For example, if you are using a self-hosted Pact Broker instance and PactNet is failing to connect, please ensure things like the
authentication token and SSL certificate are valid.

If you are sure that the issue is with PactNet then please raise an issue, including as many of the following details as you can:

- PactNet version
- Your operating system and version
- .Net version
- Log output
- Steps to reproduce and/or a repository link which reproduces the issue
- Expected outcome
- Actual outcome

Due to the way that PactNet works, often issues that are found are not in PactNet itself but instead in the [pact-reference]
native FFI libraries. If this is the case then an upstream issue will be raised in the FFI repository and PactNet will have to
wait until the fix is available in a new FFI release.

Raising Pull Requests
---------------------

For new contributors it is recommended that you start a discussion with a core maintainer prior to raising a PR. This
is for your own benefit so that you don't waste time implementing changes which don't align with the project in general
or will require significant changes afterwards.

The best way to achieve this is to open an issue detailing:

- The problem you see at the moment
- The solution you propose to fix this problem (e.g. adding a new feature, refactoring an existing API, etc)
- Any downsides you can foresee as a result of this change

If an issue already exists for the change you wish to contribute, please comment on the existing issue.

After raising your PR, a core maintainer will review your change and may request/suggest changes. Please take the
feedback in the spirit intended so that the PR can be merged as quickly as possible whilst still meeting established
conventions within the project such as architecture, code style, test style/coverage and API evolution.

In particular it is much harder to make any changes which involve a breaking change, so please set expectations accordingly
if your change requires a new major version. A large and/or potentially disruptive change should typically take the form of
an RFC issue.

A good example of an RFC issue preceding a major change is [PR 457].

Building PactNet
----------------

In order to build PactNet you must first download the native Rust FFI libraries. You can pull the current supported
version by executing the script in Bash (or Git Bash on Windows):

```bash
build/download-native-libs.sh
```

Alternatively you can download a particular FFI version from the [pact-reference] releases or build your own version
locally, and then copy the artifacts into the folders:

```
build/
    linux/
        x86_64/
            libpact_ffi.so
    osx/
        aarch64-apple-darwin/
            libpact_ffi.dylib
        x86_64/
            libpact_ffi.dylib
    windows/
        x86_64/
            pact_ffi.dll
```

After the native libraries are in the expected places then the solution can be built in your IDE or on the command
line using `dotnet build` as normal.

[pact-reference]: https://github.com/pact-foundation/pact-reference/releases
[Pact Docs]: https://docs.pact.io/
[Pact Foundation Slack]: https://pact-foundation.slack.com/
[PR 457]: https://github.com/pact-foundation/pact-net/issues/457
