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
        # Get the date of the first commit where the file was added (diff-filter A means "added")
        $firstAddCommit = git log --diff-filter=A --reverse --format="%ai" -- $file | Select-Object -First 1

        # If the file has an add commit (i.e., it's not a new file)
        if ($firstAddCommit) {
            # Create a custom object with file name and its first added commit date
            [PSCustomObject]@{
                File = $file.Substring($currentFolder.Length + 1)  # Remove the folder path to get the file name
                FirstAddCommitDate = $firstAddCommit  # Date of the first commit where the file was added
            }
        }
    }
}
# Sort the files by their first add commit date in descending order (most recent first)
$sortedFiles = $filesWithDetails | Sort-Object FirstAddCommitDate -Descending
# Create a hashtable to cache processed file base names to avoid duplicates
$cachedBaseNames = @{ }
# Initialize the output string for the latest added samples
$LatestAddeddSamples = "Latest Added Samples:`n"
# Process the sorted files
$sortedFiles | ForEach-Object {
    # Remove file extensions and standardize the file name
    $fileName = $_.File -replace '\.xaml\.cs$', '' -replace '\.xaml$', ''
    $fileName = $fileName -replace 'Page$', ''
    $date = $_.FirstAddCommitDate

    # Add the file to the output if it hasn't been cached yet (to avoid duplicates)
    if (-not $cachedBaseNames.Contains($fileName)) {
        $cachedBaseNames[$fileName] = $true  # Mark the file name as cached
        $LatestAddeddSamples += $fileName + " (" + $date + ")`n" # Append the file name to the output list
    }
}

# Output the list of the latest added samples
Write-Output $LatestAddeddSamples
# Wait for the user to press Enter before closing the PowerShell window
Read-Host -Prompt "Press Enter to exit"