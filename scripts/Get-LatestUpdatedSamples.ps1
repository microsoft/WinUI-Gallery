# Define the folder containing the WinUIGallery Samples relative to the script location
$SamplesFolder = Join-Path -Path $PSScriptRoot -ChildPath "..\WinUIGallery\Samples"

# Change directory to the WinUIGallery folder
Set-Location -Path $SamplesFolder

# Define the folder containing the control pages
$currentFolder = "ControlPages"

# Retrieve the list of files in the folder with their details
$filesWithDetails = git ls-tree -r HEAD --name-only $currentFolder | ForEach-Object { 
    $file = $_  # Current file path from the git tree

    # Check if the file is directly under the $currentFolder and not in subdirectories
    if ($file -match "^$currentFolder/[^/]+$") {
        # Get the last commit date for the file
        $lastCommitDate = git log -1 --format="%ai" -- $file

        # Get the commit status for the file (checks if it was modified in the last commit)
        $commitStatus = git log -1 --name-status -- $file | Select-String -Pattern "^M" | ForEach-Object { $_.Line.Split()[0] }
        # If the file was modified ("M"), create a custom object with file details
        if ($commitStatus -eq "M") {
            [PSCustomObject]@{
                File = $file.Substring($currentFolder.Length + 1)  # Trim the folder path to get the file name
                LastCommitDate = $lastCommitDate  # Last commit date for the file
            }
        }
    }
}
# Sort the files by the last commit date in descending order
$sortedFiles = $filesWithDetails | Sort-Object LastCommitDate -Descending
# Create a hashtable to cache processed file base names to avoid duplicates
$cachedBaseNames = @{ }
# Initialize the output string for the latest updated samples
$LatestUpdatedSamples = "Latest Updated Samples:`n"
# Process the sorted files
$sortedFiles | ForEach-Object {
    # Remove file extensions and standardize the file name
    $fileName = $_.File -replace '\.xaml\.cs$', '' -replace '\.xaml$', ''
    $fileName = $fileName -replace 'Page$', ''
    $date = $_.LastCommitDate

    # Add the file to the output if it hasn't been cached yet
    if (-not $cachedBaseNames.Contains($fileName)) {
        $cachedBaseNames[$fileName] = $true  # Mark the file name as cached
        $LatestUpdatedSamples += $fileName + " (" + $date + ")`n" # Append the file name to the output
    }
}

# Output the list of latest updated samples
Write-Output $LatestUpdatedSamples
# Wait for the user to press Enter before closing the PowerShell window
Read-Host -Prompt "Press Enter to exit"