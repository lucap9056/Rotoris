English | [繁體中文](README.zh-TW.md)

<div align="center">
	<img src="Rotoris/favicon.ico" width="128" height="128" alt="Rotoris Icon" />
	<h1>Rotoris</h1>
	<p><strong>A highly customizable Windows radial menu, triggered without stealing focus from your foreground window</strong></p>

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet)](Directory.Build.props)
[![Platform](https://img.shields.io/badge/Platform-Windows%20x64-0078D6?logo=windows)](https://github.com/lucap9056/Rotoris/releases)
[![Release](https://img.shields.io/github/v/release/lucap9056/Rotoris)](https://github.com/lucap9056/Rotoris/releases)
</div>

---

## Overview

Rotoris provides quick, fluid access to functions using any three infrequently used keys on your keyboard, without ever switching focus away from your current foreground window. It supports Lua script extensions for custom automation and ships with an intuitive graphical configuration tool for managing menus, key bindings, and appearance.

## Core Features

-   **Highly Customizable Radial Menus**: Create and organize menus and options through the configuration tool.
-   **No Focus Switching**: Triggering and operating the menu is specially designed not to affect the focus of your current foreground window.
-   **Global Hotkeys**: Bind system-wide hotkeys (typically three infrequently used keys or key combinations) to trigger your menus anytime, anywhere.
-   **Powerful Lua Scripting Engine**: Write custom scripts using Lua to perform complex automation tasks and interact with your system (audio, files, windows, etc.). Type definitions for Lua are included to assist with script development — see the [documentation](https://lucap9056.github.io/Rotoris/) for details.
-   **Graphical Configuration Tool**: An intuitive interface to manage all your settings, including menus, appearance, hotkeys, and Lua scripts.
-   **System Tray Integration**: Runs discreetly in the background, accessible through a system tray icon for status and basic interactions.
-   **Console-Based Log Viewer**: A lightweight console window shows live logs with keyboard shortcuts (`[R]` reload, `[X]` exit); closing it just hides the viewer, it does not stop the application.
-   **Transparent Overlay Window**: A unique, transparent, non-interactive main window that Lua scripts can use to display visual information, custom overlays, or interactive elements without interfering with underlying applications or appearing in the taskbar.
-   **Toast Notifications**: Informative toast notifications for startup status and other relevant events.
-   **Audio Playback/Manipulation**: Utilizes NAudio for audio feedback or manipulation via scripts.
-   **2D Graphics**: Incorporates SkiaSharp for advanced 2D graphics rendering in the overlay window.

## Getting Started

### Prerequisites

-   Windows 10 (build 17763) or higher, x64.

### Installation

1.  Go to the project's [Releases page](https://github.com/lucap9056/Rotoris/releases).
2.  Download the latest version of `Rotoris_setup.exe` (or similarly named installer).
3.  Run the installer and follow the instructions to complete the installation.

### Usage

1.  **Configuration**: Run `RotorisConfigurationTool.exe` to set up menus, options, and hotkeys.
2.  **Start**: Run `Rotoris.exe`. The program will run in the background in the system tray.
3.  **Trigger**: Press your configured hotkey to open the radial menu at the center of the screen where your mouse cursor currently is.

### Other Operations

-   **Show Logs**: Click the system tray icon, or select "Show Logs" from the right-click menu, to open the console log viewer.
-   **Reload Settings**: Right-click the system tray icon and select "Reload", or press `R` inside the log viewer.
-   **Exit Program**: Right-click the system tray icon and select "Exit", or press `X` inside the log viewer.

## Advanced Usage: Key Overrides

Rotoris allows you to override the behavior of specific keys (e.g., volume up/down keys) **when the radial menu is NOT open**, through specially named Lua files.

### How It Works

In your configuration folder, create Lua files with the following names to enable this feature:

-   `_CLOCKWISE.lua`: When this file exists, the script defined within it overrides the default behavior of the key corresponding to the "clockwise" direction (e.g., volume up key).
-   `_COUNTER_CLOCKWISE.lua`: When this file exists, the script defined within it overrides the default behavior of the key corresponding to the "counter-clockwise" direction (e.g., volume down key).
-   `_ROOT.lua`: When this file exists, the script defined within it executes whenever any key is pressed. This can be used to display a more elaborate initial menu or as a simple key override.

**Example**: You can create a `_CLOCKWISE.lua` file with content to control media playback (e.g., "next track"). This way, when the menu is not open, pressing the volume up key triggers "next track" instead of increasing the volume.

### Developer Mode

For developers creating or debugging Lua scripts, Rotoris includes a `--dev` argument. Running Rotoris with this argument opens a console window and extracts all embedded Lua type definition files (`.d.lua`) into a `.types` directory within the application's data folder. These files help IDEs with Lua language server support provide autocompletion and type checking for the Rotoris Lua API.

```bash
Rotoris.exe --dev
```

## Technology Stack

-   **.NET 9 (WPF)**: The application is built using .NET and Windows Presentation Foundation for its graphical user interface elements and system integration.
-   **Lua (via NLua)**: Provides the powerful and flexible scripting capabilities.
-   **NAudio**: Used for audio-related functionalities.
-   **SkiaSharp**: Powers 2D graphics rendering for custom visuals.
-   **Microsoft.Toolkit.Uwp.Notifications**: Enables modern Windows toast notifications.
-   **Newtonsoft.Json**: Used for JSON serialization and deserialization, primarily for configuration and data handling.

## Project Structure

The solution consists of several key projects:

-   **`Rotoris`**: The main WPF application that runs in the background, executes Lua scripts, and manages system interactions.
-   **`RotorisConfigurationTool`**: A separate WPF application designed for configuring the settings and Lua scripts for Rotoris.
-   **`RotorisLib`**: A shared library containing common functionalities, utilities, and models used by both `Rotoris` and `RotorisConfigurationTool`.
-   **`RotorisLib.Tests`**: A test project for ensuring the quality and correctness of the `RotorisLib` components.

