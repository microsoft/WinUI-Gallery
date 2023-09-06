# UI Tests with WinAppDriver

This UI Test repository of collection of WinAppDriver-based test scenarios that cover basic interactions with WinUI 3 controls. Note: the [Universal Windows Platform controls](https://docs.microsoft.com/en-us/windows/uwp/controls-and-patterns/) reference can be used, as the WinUI 3 controls are very similar to the UWP controls.

The test scenarios are written to test the controls in the Xaml Controls Gallery app. The procedure below outlines the steps needed to build, deploy, and stage the Xaml Controls Gallery and WinAppDriver to run the UI Tests. These manual steps simulate what azure-pipelines.yml does in pipeline test runs.

## Build and install Xaml Controls Gallery

1. Generate the test signing certificate:

```shell
    PS> .\build\GenerateTestPfx.ps1
```

1. Build and publish the Xaml Controls Gallery from the command line, e.g.:

```shell
    dotnet.exe publish WinUIGallery\WinUIGallery.sln /p:AppxPackageDir=AppxPackages\ /p:platform=x64 /p:PublishProfile=./WinUIGallery/Properties/PublishProfiles/win10-x64.pubxml
```

1. Locate the WinUI 3's Gallery package output folder from above and deploy for testing:

```shell
    PS> .\WinUIGallery\AppxPackages\WinUIGallery.Desktop_Test\Install.ps1
```

## Install and execute WinAppDriver

1. Download and install the latest version of WinAppDriver from [here](https://github.com/microsoft/WinAppDriver/releases)

1. Run WinAppDriver (required for running tests on cmdline or in VS):

```shell
    C:\Program Files (x86)\Windows Application Driver>WinAppDriver.exe
```

## Build and run/debug Tests

1. Build UITests:

```shell
    >msbuild UITests\UITests.sln
```

1. Run test cases built above on command line:

```shell
    vstest.console.exe D:\git\WinUI-Gallery\UITests\bin\x64\Debug\net7.0\UITests.dll
```

1. Debug test cases in Visual Studio
   * Open `UITests.sln` in Visual Studio
   * Select **Test** > **Windows** > **Test Explorer**
   * Select **Run All** on the test pane or through menu **Test** > **Run** > **All Tests**

   Once the project is successfully built, you can use the **TestExplorer** to pick and choose the test scenario(s) to run/debug

## Test output

In either scenario above (cmdline or VS), if the tests run successfully, you should see:

* WinAppDriver console spew indicating successful launch of the test app
* The Xaml Controls Gallery launch and run with automated UI interactions
* vstest.console.exe spew (if cmdline) showing test case status


## Adding/Updating Test Scenarios

Please follow the guidelines below to maintain test reliability and reduce test execution time:

1. Provide a complete set of interactions (if applicable) for each new control
1. Aim for simple and reliable scenario using the least amount of test steps
1. Reuse existing application session when possible to reduce unnecessary application re-launching
