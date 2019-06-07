# UI Tests with WinAppDriver

This UI Test repository of collection of test scenarios that covers basic interactions with [Universal Windows Platform controls (UI elements)](https://docs.microsoft.com/en-us/windows/uwp/controls-and-patterns/). Use this as a reference on how to interact with certain UWP controls you have in your application.

The test scenarios are written to test the controls in the Xaml Controls Gallery app, which contains all UWP Xaml controls. The Xaml Controls Gallery app needs to be installed on the machine where you are running your tests. Follow the instructions in the root README to install the app.


## Requirements

- Windows 10 PC with the latest Windows 10 version (Version 1809 or later)
- Microsoft Visual Studio 2017 or later
- [Xaml Controls Gallery](../../XamlControlsGallery/XamlControlsGallery.sln) built and deployed 


## Getting Started

1. Open `UITests.sln` in Visual Studio
2. Select **Test** > **Windows** > **Test Explorer**
3. Select **Run All** on the test pane or through menu **Test** > **Run** > **All Tests**

> Once the project is successfully built, you can use the **TestExplorer** to pick and choose the test scenario(s) to run


## Adding/Updating Test Scenario

Please follow the guidelines below to maintain test reliability and reduce test execution time:
1. Provide a complete set of interactions (if applicable) for each new control
2. Aim for simple and reliable scenario using the least amount of test steps
3. Reuse existing application session when possible to reduce unnecessary application re-launching
