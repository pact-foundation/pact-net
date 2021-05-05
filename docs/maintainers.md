# Maintainers
This document describes the common task that a maintainer may need to perform.

## Creating a Release
Releases are controller by GitHub releases and should only target master.  
The following is the process for creating a release: 
1) "Draft a new release" in GitHub.
2) Decide on the version number for the release and use that version number as the "Tag version" and "Release title" values in the release. Note this library follows [semantic versioning](https://semver.org/), please make sure the major.minor.patch parts are correctly assigned.
3) Use the below template to "Describe the release". You can remove any section that does not apply to the release.
```
**Tweaks**
1. List any tweaks

**Features**
1. List any new features

**Bug Fixes**
1. List any bug fixes

**Breaking Changes**
1. List any breaking changes and any migration paths
```
4) Once happy, click "Publish release" and [AppVeyor](https://ci.appveyor.com/project/SEEKJobs/pact-net/branch/master) will trigger a new build, then upload the artefacts to [Nuget](https://www.nuget.org/profiles/pact-foundation).

## Upgrading the Pact Core
PactNet uses the shared [ruby standalone core library](https://github.com/pact-foundation/pact-ruby-standalone/releases).  
It's a manual process to update PactNet to the latest version, this is done by editing `Build\Download-Standalone-Core.ps1` and updating `$StandaloneCoreVersion`. [See this commit](https://github.com/pact-foundation/pact-net/commit/a304c989780c8f277321299ed7890ec2fbddfb8f) for an example.  
If new features have been added in the standalone core, you'll likely also need to wire them through in the code.  
Once you are happy, create a release to ship out the new version.