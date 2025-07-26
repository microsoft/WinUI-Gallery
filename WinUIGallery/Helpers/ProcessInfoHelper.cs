using System;
using System.Diagnostics;

namespace WinUIGallery.Helpers;
public static partial class ProcessInfoHelper
{
    private static readonly FileVersionInfo _fileVersionInfo;
    private static readonly Process _process;
    static ProcessInfoHelper()
    {
        _process = Process.GetCurrentProcess();
        _fileVersionInfo = _process.MainModule.FileVersionInfo;
    }

    /// <summary>
    /// Returns the version string.
    /// </summary>
    public static string Version => GetVersion()?.ToString();

    /// <summary>
    /// Returns the version string prefixed with 'v'.
    /// </summary>
    public static string VersionWithPrefix => $"v{Version}";

    /// <summary>
    /// Retrieves the product name. If not available, it returns 'Unknown Product'.
    /// </summary>
    public static string ProductName => _fileVersionInfo?.ProductName ?? "Unknown Product";

    /// <summary>
    /// Combines the product name and version into a single string. The version includes a prefix.
    /// </summary>
    public static string ProductNameAndVersion => $"{ProductName} {VersionWithPrefix}";

    /// <summary>
    /// Returns the company name of the publisher. If not available, it defaults to 'Unknown Publisher'.
    /// </summary>
    public static string Publisher => _fileVersionInfo?.CompanyName ?? "Unknown Publisher";

    public static Version GetVersion()
    {
        return new Version(_fileVersionInfo.FileMajorPart, _fileVersionInfo.FileMinorPart, _fileVersionInfo.FileBuildPart, _fileVersionInfo.FilePrivatePart);
    }

    /// <summary>
    /// Retrieves the file version information for the current assembly.
    /// </summary>
    /// <returns>Returns a FileVersionInfo object containing version details.</returns>
    public static FileVersionInfo GetFileVersionInfo()
    {
        return _fileVersionInfo;
    }

    /// <summary>
    /// Retrieves the current process instance.
    /// </summary>
    /// <returns>Returns the current Process object.</returns>
    public static Process GetProcess()
    {
        return _process;
    }
}
