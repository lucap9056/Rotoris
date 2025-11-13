using Microsoft.Win32;
using System.Diagnostics;

namespace RotorisConfigurationTool.ConfigurationControls.ActionManagement
{
    public struct EditorAvailability
    {
        public bool IsVSCodeAvailable { get; set; } = false;
        public bool IsNotepadPlusPlusAvailable { get; set; } = false;
        public string NotepadPlusPlusPath { get; set; } = "";
        public EditorAvailability()
        {
            if (IsVSCodeCLIAvailable())
            {
                IsVSCodeAvailable = true;
            }

            if (IsNotepadPlusPlusInstalled(out string notepadPlusPlusPath))
            {
                IsNotepadPlusPlusAvailable = true;
                NotepadPlusPlusPath = notepadPlusPlusPath;
            }
        }


        private static bool IsVSCodeCLIAvailable()
        {

            string command = "where";
            string arguments = "code";

            using var process = new Process()
            {
                StartInfo =
                {
                    FileName = command,
                    Arguments = arguments,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                }
            };
            try
            {
                if (!process.Start())
                {
                    string errorMessage = $"Failed to start the process: '{command} {arguments}'.";
                    Console.WriteLine($"[LOG] {errorMessage}");
                    return false;
                }

                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();

                process.WaitForExit();

                if (process.ExitCode == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                string errorMessage = $"An unexpected error occurred while executing the command '{command} {arguments}': {ex.Message}";
                Console.WriteLine($"[LOG] Error during process execution: {ex.Message}");
                return false;
            }
        }

        public static bool IsNotepadPlusPlusInstalled(out string notepadPlusPlusPath)
        {
            if (TryGetNotepadPlusPlusPath(RegistryHive.LocalMachine, RegistryView.Registry64, out notepadPlusPlusPath))
            {
                Console.WriteLine("[LOG] Notepad++ found in HKLM 64-bit uninstall key.");
                return true;
            }

            if (TryGetNotepadPlusPlusPath(RegistryHive.LocalMachine, RegistryView.Registry32, out notepadPlusPlusPath))
            {
                Console.WriteLine("[LOG] Notepad++ found in HKLM 32-bit uninstall key (Wow6432Node).");
                return true;
            }

            if (TryGetNotepadPlusPlusPath(RegistryHive.CurrentUser, RegistryView.Default, out notepadPlusPlusPath))
            {
                Console.WriteLine("[LOG] Notepad++ found in HKCU uninstall key.");
                return true;
            }

            Console.WriteLine("[LOG] Notepad++ installation not detected via standard registry keys.");
            return false;
        }

        private static bool TryGetNotepadPlusPlusPath(RegistryHive hive, RegistryView view, out string path)
        {
            path = "";
            const string uninstallPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
            const string nppExe = "notepad++.exe";

            try
            {
                using RegistryKey? baseKey = RegistryKey.OpenBaseKey(hive, view);
                using RegistryKey? subKey = baseKey?.OpenSubKey(uninstallPath);

                if (subKey != null)
                {
                    foreach (string keyName in subKey.GetSubKeyNames())
                    {
                        using RegistryKey? softwareKey = subKey.OpenSubKey(keyName);
                        string? displayName = softwareKey?.GetValue("DisplayName") as string;
                        string? installLocation = softwareKey?.GetValue("InstallLocation") as string;
                        string? installPath = softwareKey?.GetValue("InstallPath") as string;

                        if (!string.IsNullOrEmpty(displayName) &&
                            displayName.Contains("Notepad++", StringComparison.OrdinalIgnoreCase))
                        {
                            string exePath = "";
                            if (!string.IsNullOrEmpty(installLocation))
                            {
                                exePath = System.IO.Path.Combine(installLocation, nppExe);
                            }
                            else if (!string.IsNullOrEmpty(installPath))
                            {
                                exePath = System.IO.Path.Combine(installPath, nppExe);
                            }
                            else
                            {
                                string? displayIcon = softwareKey?.GetValue("DisplayIcon") as string;
                                if (!string.IsNullOrEmpty(displayIcon) && displayIcon.EndsWith(nppExe, StringComparison.OrdinalIgnoreCase))
                                {
                                    exePath = displayIcon.Split(',')[0].Trim('"');
                                }
                            }

                            if (System.IO.File.Exists(exePath))
                            {
                                path = exePath;
                                return true;
                            }

                            Console.WriteLine($"[LOG] Found Notepad++ entry but could not determine/verify execution path. Name: {displayName}");
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LOG] Error reading registry key ({hive}, {view}) under Uninstall: {ex.Message}");
            }

            return false;
        }
    }
}
