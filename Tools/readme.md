# Tools Folder

The Tools folder contains a set of tools and utilities that are repeatedly used by developers to assist in managing and developing the WinUI Gallery. These tools streamline various development tasks, providing functionality to track, manage, and organize files within the repository.

> **Note**: Before running any PowerShell script, you must set the execution policy to allow locally created scripts to run. Use the following command to set the policy:
> 
> ```powershell
> Set-ExecutionPolicy RemoteSigned -Scope CurrentUser
> ```
> This ensures that PowerShell scripts downloaded from the internet are not run unless they are signed by a trusted publisher, while locally created scripts are allowed to run.

## 1. Get-LatestAddedSamples tool

The goal of the `Get-LatestAddedSamples.ps1` script is to assist developers in identifying newly added files in the `ControlPages` directory, along with the date of their first commit. This helps in tracking the progress of new additions to the project and identifying which samples should be included on the home page, ensuring that the most recent content is always showcased.

### How to Use the Script

#### 1st Way: From File Explorer
- Step 1: Navigate to the `Tools` folder.
- Step 2: Right-click the `Get-LatestAddedSamples.ps1` script and select **Run with PowerShell**.
   ![image](https://github.com/user-attachments/assets/af679c51-42a2-41ff-9f0a-5564955d09e6)
- Step 3: The script will output a list of newly added files along with their first commit dates.
   ![image](https://github.com/user-attachments/assets/1280622a-d555-4803-9363-ba1d8e653981)

#### 2nd Way: From Visual Studio
- Step 1: Open `WinUIGallery` project in Visual Studio.
- Step 2: Navigate to the **Tools** menu and select **Open PowerShell Window**.
- Step 3: In the PowerShell window, type the following command:
  ```powershell
  ..\Tools\Get-LatestAddedSamples.ps1
  ```
- Step 4: The script will output the list of added files.
   ![image](https://github.com/user-attachments/assets/ffcd153b-19d7-4c01-a7f6-d60d122dc435)

## 2. Get-LatestUpdatedSamples tool

### How to Use the Script

The goal of the `Get-LatestUpdatedSamples.ps1` script is to help developers quickly identify files in the `ControlPages` directory that have been updated, along with their last commit dates. This information is useful for tracking changes made to the project, ensuring that developers can monitor updates to existing samples, and identifying which updated samples might need to be showcased on the home page.

#### 1st Way: From File Explorer
- Step 1: Navigate to the `Tools` folder.
- Step 2: Right-click the `Get-LatestUpdatedSamples.ps1` script and select **Run with PowerShell**.
   ![image](https://github.com/user-attachments/assets/d2b4dca4-5594-424e-acdd-81fd80b0af08)
- Step 3: The script will output a list of modified files along with their last commit dates.
   ![image](https://github.com/user-attachments/assets/d74460fc-27e6-4bd9-b522-dc6ad0f7e418)

#### 2nd Way: From Visual Studio
- Step 1: Open `WinUIGallery` project in Visual Studio.
- Step 2: Navigate to the **Tools** menu and select **Open PowerShell Window**.
- Step 3: In the PowerShell window, type the following command:
  ```powershell
  ..\Tools\Get-LatestUpdatedSamples.ps1
  ```
- Step 4: The script will output the list of modified files.
   ![image](https://github.com/user-attachments/assets/2019b0c6-9f99-46b0-ab2c-ee01b930476f)

# Summary

The Tools folder provides a collection of tools and utilities to assist developers in managing and developing the WinUI Gallery. These tools help streamline various development tasks, such as tracking file changes, managing updates, and organizing content within the repository. The tools ensure efficient workflows and contribute to the smooth development of the WinUI Gallery project.
