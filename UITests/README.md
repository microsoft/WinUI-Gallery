# UI Tests with WinAppDriver

The UI Test repository is a collection of WinAppDriver-based test scenarios that cover basic interactions with WinUI 3 controls. Note: the [Universal Windows Platform controls](https://docs.microsoft.com/en-us/windows/uwp/controls-and-patterns/) reference can be used, as the WinUI 3 controls are very similar to the UWP controls.

The test scenarios are written to test the controls in the WinUI 3 Gallery app. The procedure below outlines the steps needed to build and deploy WinUI 3 Gallery and WinAppDriver to run the UI Tests. These steps mirror what azure-pipelines.yml does in pipeline runs.

## Deploy WinUI 3 Gallery

The easiest way to deploy the WinUI 3 Gallery for unit test execution is to simply build WinUIGallery\WinUIGallery.sln and F5 deploy it from within Visual Studio.  Alternatively, the following commands can be used for automation.

1. Generate the test signing certificate:

```shell
    PS> .\build\GenerateTestPfx.ps1
```

1. Build and publish the WinUI 3 Gallery from the command line, e.g.:

```shell
    >dotnet.exe publish WinUIGallery\WinUIGallery.sln /p:AppxPackageDir=AppxPackages\ /p:platform=x64 /p:PublishProfile=./WinUIGallery/Properties/PublishProfiles/win-x64.pubxml
```

1. Locate the WinUI 3 Gallery package output folder from above and deploy for testing:

```shell
    PS> .\WinUIGallery\AppxPackages\WinUIGallery.Desktop_Test\Install.ps1
```

## Run WinAppDriver

The test runner (vstest.console.exe and VS Test Explorer) should automatically launch WinAppDriver. Alternatively, it can be launched manually to observe diagnostic output as follows:

1. Download and install the latest version of WinAppDriver from [here](https://github.com/microsoft/WinAppDriver/releases)

1. Run WinAppDriver:

```shell
    >C:\Program Files (x86)\Windows Application Driver\WinAppDriver.exe
```

## Build and run/debug Tests

The easiest way to run/debug the UI tests is with Visual Studio, as follows:

   * Open `UITests\UITests.sln` in Visual Studio
   * Select **Test** > **Windows** > **Test Explorer**
   * Select **Run All** on the test pane or through menu **Test** > **Run** > **All Tests**

Once the project is successfully built, you can use the **Test Explorer** to choose  test scenarios to run/debug

Alternatively, the following commands can be used for automation:

1. Build UITests:

```shell
    >dotnet build UITests\UITests.sln
    --or--
    >msbuild UITests\UITests.sln
```

1. Run test cases built above on command line:

```shell
    >dotnet test .\UITests\UITests.csproj
    --or--
    >vstest.console.exe .\UITests\bin\x64\Debug\net7.0\UITests.dll
```

## Test output

In either scenario above (cmdline or VS), if the tests run successfully, you should see:

* The WinUI 3 Gallery launch and run with automated UI interactions
* vstest.console.exe spew or Test Explorer indications of test case status
* Optionally, WinAppDriver console spew indicating successful launch of the test app

## Adding/Updating Test Scenarios

Please follow the guidelines below to maintain test reliability and reduce test execution time:

1. Provide a complete set of interactions (if applicable) for each new control
1. Aim for simple and reliable scenario using the least amount of test steps

