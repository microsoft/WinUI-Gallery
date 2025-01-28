# Tools Folder

The Tools folder contains a set of tools and utilities that are repeatedly used by developers to assist in managing and developing the WinUI Gallery. These tools streamline various development tasks, providing functionality to track, manage, and organize files within the repository.

> **Note**: Before running any PowerShell script, you must set the execution policy to allow locally created scripts to run. Use the following command to set the policy:
> 
> ```powershell
> Set-ExecutionPolicy RemoteSigned -Scope CurrentUser
> ```
> This ensures that PowerShell scripts downloaded from the internet are not run unless they are signed by a trusted publisher, while locally created scripts are allowed to run.


## Running scripts

#### 1st Way: From File Explorer
- Step 1: Navigate to the `Tools` folder.
- Step 2: Right-click the `Get-LatestAddedSamples.ps1` script and select **Run with PowerShell**.
- Step 3: The script will output a list of newly added files along with their first commit dates.

#### 2nd Way: From Visual Studio
- Step 1: Open `WinUIGallery` project in Visual Studio.
- Step 2: Navigate to the **Tools** menu and select **Open PowerShell Window**.
- Step 3: In the PowerShell window, type the following command:
  ```powershell
  ..\Tools\Get-LatestAddedSamples.ps1
  ```
- Step 4: The script will output the list of added files.

## Scripts

### Get-LatestAddedSamples tool

The goal of the `Get-LatestAddedSamples.ps1` script is to assist developers in identifying newly added files in the `ControlPages` directory, along with the date of their first commit. This helps in tracking the progress of new additions to the project and identifying which samples should be included on the home page, ensuring that the most recent content is always showcased.

### Get-LatestUpdatedSamples tool

The goal of the `Get-LatestUpdatedSamples.ps1` script is to help developers quickly identify files in the `ControlPages` directory that have been updated, along with their last commit dates. This information is useful for tracking changes made to the project, ensuring that developers can monitor updates to existing samples, and identifying which updated samples might need to be showcased on the home page.