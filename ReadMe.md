[![Build status](https://ci.appveyor.com/api/projects/status/o20i7mcihb9uldn3?svg=true)](https://ci.appveyor.com/project/benquarmby/jslintnet)

## Install

#### JSLint.NET for MSBuild

This [NuGet package](https://www.nuget.org/packages/JSLintNet.MSBuild/) wires up targets to any Visual Studio project for development and CI time JavaScript validation. Run the following command from the package manager console to install:

```PowerShell
PM> Install-Package JSLintNet.MSBuild
```

#### JSLint.NET for Visual Studio

Provides a richer IDE experience for Visual Studio 2012, 2013 and 2015, with underlined warnings and the ability to validate automatically on save and build. Can function side by side with the MSBuild package.

Download it from the Extensions and Tools dialog within Visual Studio or from the [Visual Studio Gallery](http://visualstudiogallery.msdn.microsoft.com/ede12aa8-0f80-4e6f-b15c-7a8b3499370e).

#### Other Releases

Stand alone binaries together with a [command line runner](https://github.com/benquarmby/jslintnet/wiki/Console-Options) are available on the releases page.

## Configuration

JSLint.NET can be configured with common options for an entire project or solution. See the [Settings](https://github.com/benquarmby/jslintnet/wiki/JSLint.NET-Settings) page for more information.
