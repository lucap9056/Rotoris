[English](README.md) | [繁體中文](README.zh-TW.md)
<table>
  <tr>
    <td width="160" align="center">
      <img src="Rotoris/favicon.ico" width="128" alt="Rotoris Icon">
    </td>
    <td style="word-break: normal;">
      <h1>Rotoris</h1>
      Rotoris is a highly customizable Windows radial menu tool designed to provide quick and fluid access to functions without interrupting foreground window focus. It achieves this by utilizing any three infrequently used keys on the keyboard. It supports Lua script extensions and comes with an intuitive graphical configuration tool, allowing users to easily manage menus, key bindings, and appearance.
    </td>
  </tr>
</table>

## Features

-   **Highly Customizable Radial Menus**: Easily create and organize your menus and options through the configuration tool.
-   **No Focus Switching**: The menu's triggering and operation are specially designed not to affect the focus of your current foreground window, ensuring an uninterrupted workflow.
-   **Global Hotkeys**: Bind system-wide hotkeys (typically three infrequently used keys or key combinations) to trigger your menus anytime, anywhere.
-   **Powerful Lua Scripting Engine**: Write custom scripts using Lua to perform complex automation tasks and interact with your system (audio, files, windows, etc.). Type definitions for Lua are included to assist with script development. For users who wish to write Lua scripts, please refer to the [documentation](https://lucap9056.github.io/Rotoris/).
-   **Graphical Configuration Tool**: Provides an intuitive interface to manage all your settings, including menus, appearance, hotkeys, and Lua scripts.
-   **System Tray Integration**: Runs discreetly in the background, accessible through a system tray icon for status and basic interactions.
-   **Transparent Overlay Window**: Features a unique, transparent, and non-interactive main window that can be utilized by Lua scripts to display visual information, custom overlays, or interactive elements without interfering with underlying applications or appearing in the taskbar.
-   **Toast Notifications**: Provides informative toast notifications for startup status and other relevant events.
-   **Audio Playback/Manipulation**: Utilizes NAudio, suggesting capabilities for audio feedback or manipulation via scripts.
-   **2D Graphics**: Incorporates SkiaSharp for advanced 2D graphics rendering, potentially for custom UI elements or visual effects within the overlay window.


## Installation Guide

1.  Go to the project's [Releases page](https://github.com/lucap9056/Rotoris/releases).
2.  Download the latest version of `Rotoris_setup.exe` (or similarly named installer).
3.  Run the installer and follow the instructions to complete the installation.

## Usage

1.  **Configuration**: Run `RotorisConfigurationTool.exe` to set up menus, options, and hotkeys.
2.  **Start**: Run `Rotoris.exe`. The program will run in the background in the system tray.
3.  **Trigger**: Press your configured hotkey to open the radial menu at the center of the screen where your mouse cursor currently is.

### Other Operations

-   **Reload Settings**: Right-click the system tray icon and select "Reload," or click the "Reload" button in the log window.
-   **Exit Program**: Right-click the system tray icon and select "Exit," or click the "Exit" button in the log window.
-   **Show Log Window**: Click the system tray icon, or select "Show Log" from the right-click menu.

## Advanced Usage: Key Overrides

Rotoris allows you to override the behavior of specific keys (e.g., volume up/down keys) **when the radial menu is NOT open** through specially named Lua files.

### How it Works

In your configuration folder, create Lua files with the following names to enable this feature:

-   `_CLOCKWISE.lua`: When this file exists, the script defined within it will override the default behavior of the key corresponding to the "clockwise" direction (e.g., volume up key).
-   `_COUNTER_CLOCKWISE.lua`: When this file exists, the script defined within it will override the default behavior of the key corresponding to the "counter-clockwise" direction (e.g., volume down key).
-   `_ROOT.lua`: When this file exists, the script defined within it will execute whenever any key is pressed. This can be used to display a more elaborate initial menu or as a simple key override.

**Example**:
You can create a `_CLOCKWISE.lua` file with content to control media playback (e.g., "next track"). This way, when the menu is not open, pressing the volume up key will trigger "next track" instead of increasing the volume.

### Developer Mode

For developers creating or debugging Lua scripts, Rotoris includes a `--dev` argument. Running Rotoris with this argument will open a console window and extract all embedded Lua type definition files (`.d.lua`) into a `.types` directory within the application's data folder. These files can be helpful for IDEs that support Lua language servers to provide autocompletion and type checking for the Rotoris Lua API.

```bash
Rotoris.exe --dev
```

## Technologies Used

*   **.NET (WPF):** The application is built using .NET and Windows Presentation Foundation for its graphical user interface elements and system integration.
*   **Lua (via NLua):** Provides the powerful and flexible scripting capabilities.
*   **NAudio:** Used for audio-related functionalities.
*   **SkiaSharp:** Powers 2D graphics rendering for custom visuals.
*   **Microsoft.Toolkit.Uwp.Notifications:** Enables modern Windows toast notifications.
*   **Newtonsoft.Json:** For JSON serialization and deserialization, likely used for configuration or data handling.

## Project Structure

The solution consists of several key projects:

*   **`Rotoris`:** The main WPF application that runs in the background, executes Lua scripts, and manages system interactions.
*   **`RotorisConfigurationTool`:** A separate WPF application designed for configuring the settings and Lua scripts for Rotoris.
*   **`RotorisLib`:** A shared library containing common functionalities, utilities, and models used by both `Rotoris` and `RotorisConfigurationTool`.
*   **`RotorisLib.Tests`:** A test project for ensuring the quality and correctness of the `RotorisLib` components.

