
dotnet restore --force WinUIGallery\WinUIGallery.sln
msbuild /p:Platform=x64;Configuration=Debug-Unpackaged /restore WinUIGallery\WinUIGallery.sln /bl
