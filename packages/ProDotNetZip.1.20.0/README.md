# ProDotNetZip - NET Standard part of DotNetZip / Ionic's Zip Library

DotNetZip is the name of an open-source project that delivers a .NET
library for handling ZIP files, and some associated tools.

**ProDotNetZip** is rip off the original code with limitation to NET standard subset.

 - The library allows .NET programmers to build applications
   that read, create and modify ZIP files.

 - The tools are .NET programs that rely on the library, and can be used
   by anyone on any Windows machine to build or extract ZIP files.
   
 - Detected vulnerabilities and outdated libraries are fixed
 
 - Several Pull Requests from forked repository has been accepted


This library: https://github.com/mihula/ProDotNetZip

SemVer updated library: https://github.com/haf/DotNetZip.Semverd

Original library: https://github.com/DinoChiesa/DotNetZip

Namespace was left untouched as `Ionic.Zip`.

## Versions

### 1.20.0
  - revert dependency of System.Security.Permissions and System.Text.Encoding.CodePages to v8.0.0

### 1.19.0
  - fix for CVE-2024-48510
  - update DotNet.ReproducibleBuilds v1.2.25
  - update System.Security.Permissions v9.0.0
  - update System.Text.Encoding.CodePages to v9.0.0
  - update test references
    - bump xunit.runner.visualstudio from 2.5.7 to 2.8.1
    - bump xunit from 2.7.0 to 2.8.1
    - bump Microsoft.NET.Test.Sdk from 17.9.0 to 17.10.0
    - bump MSTest.TestFramework from 3.3.1 to 3.4.3
    - bump MSTest.TestAdapter from 3.3.1 to 3.4.3

### 1.18.0
  - fixes: Zip omit Disk Start Number from the Zip64 Central Directory Entry
    https://github.com/haf/DotNetZip.Semverd/issues/260
  - update test references
    - bump MSTest.TestFramework from 3.2.2 to 3.3.1
    - bump MSTest.TestAdapter from 3.2.2 to 3.3.1

### 1.17.0
  - fixes: Dictionary null access (https://github.com/haf/DotNetZip.Semverd/issues/276, https://github.com/haf/DotNetZip.Semverd/issues/200)
  - left just netstandard version of library
