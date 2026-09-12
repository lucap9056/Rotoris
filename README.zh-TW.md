[English](README.md) | 繁體中文

<div align="center">
	<img src="Rotoris/favicon.ico" width="128" height="128" alt="Rotoris Icon" />
	<h1>Rotoris</h1>
	<p><strong>高度可自訂的 Windows 徑向選單，觸發時不會搶走前景視窗的焦點</strong></p>

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet)](Directory.Build.props)
[![Platform](https://img.shields.io/badge/Platform-Windows%20x64-0078D6?logo=windows)](https://github.com/lucap9056/Rotoris/releases)
[![Release](https://img.shields.io/github/v/release/lucap9056/Rotoris)](https://github.com/lucap9056/Rotoris/releases)
</div>

---

## 專案概覽

Rotoris 利用鍵盤上任意三個不常用的按鍵，提供快速且流暢的功能存取，並且完全不會切換走目前前景視窗的焦點。它支援 Lua 指令稿擴充來實現自訂自動化，並內建直觀的圖形化設定工具，用於管理選單、按鍵綁定與外觀。

## 主要功能

-   **高度可自訂的徑向選單**：透過設定工具建立並組織您的選單和選項。
-   **無焦點切換**：選單的觸發與操作經過特殊設計，不會影響您目前前景視窗的焦點。
-   **全域快速鍵**：綁定系統級的快速鍵（通常是三個不常用的鍵或組合鍵），隨時隨地觸發您的選單。
-   **強大的 Lua 指令稿引擎**：透過 Lua 撰寫自訂腳本，實現複雜的自動化任務，並與您的系統（音訊、檔案、視窗等）互動。內含 Lua 類型定義，有助於腳本開發，詳見[文件](https://lucap9056.github.io/Rotoris/)。
-   **圖形化設定工具**：直觀的介面，用於管理所有設定，包括選單、外觀、快速鍵和 Lua 腳本。
-   **系統匣整合**：在背景靜默執行，可透過系統匣圖示存取，以進行狀態查詢和基本互動。
-   **主控台式紀錄檢視器**：以輕量的主控台視窗即時顯示紀錄，並提供鍵盤快速鍵（`[R]` 重新載入、`[X]` 結束）；關閉檢視器僅會隱藏它，不會終止應用程式。
-   **透明疊加視窗**：獨特的、透明且非互動式的主視窗，可供 Lua 腳本用於顯示視覺資訊、自訂疊加層或互動元素，而不會干擾底層應用程式或出現在工作列中。
-   **通知**：提供啟動狀態和其他相關事件的資訊性通知。
-   **音訊播放/操作**：利用 NAudio 進行音訊回饋或操作。
-   **2D 圖形**：整合 SkiaSharp，於疊加視窗中提供進階 2D 圖形渲染。

## 快速開始

### 先決條件

-   Windows 10（組建 17763）或更高版本，x64。

### 安裝指南

1.  前往專案的 [Releases 頁面](https://github.com/lucap9056/Rotoris/releases)。
2.  下載最新版本的 `Rotoris_setup.exe`（或類似名稱的安裝檔）。
3.  執行安裝檔並依照指示完成安裝。

### 使用方法

1.  **設定**：執行 `RotorisConfigurationTool.exe` 來設定選單、選項及快速鍵。
2.  **啟動**：執行 `Rotoris.exe`，程式將在系統匣背景運作。
3.  **觸發**：按下您設定的快速鍵，即可在滑鼠目前所在的螢幕中央開啟徑向選單。

### 其他操作

-   **顯示紀錄**：點擊系統匣圖示，或在右鍵選單中選擇「顯示紀錄」，開啟主控台紀錄檢視器。
-   **重新載入設定**：在系統匣圖示上點擊右鍵，選擇「重新載入」，或在紀錄檢視器中按下 `R`。
-   **結束程式**：在系統匣圖示上點擊右鍵，選擇「結束」，或在紀錄檢視器中按下 `X`。

## 進階使用方法：按鍵覆蓋

Rotoris 允許您透過特殊命名的 Lua 檔案，在**未開啟徑向選單時**覆蓋特定按鍵（如音量增/減鍵）的行為。

### 運作方式

在您的設定資料夾中，建立以下名稱的 Lua 檔案以啟用此功能：

-   `_CLOCKWISE.lua`：當此檔案存在時，它定義的腳本將覆蓋「順時針」方向對應的按鍵（例如，音量增加鍵）的預設行為。
-   `_COUNTER_CLOCKWISE.lua`：當此檔案存在時，它定義的腳本將覆蓋「逆時針」方向對應的按鍵（例如，音量減少鍵）的預設行為。
-   `_ROOT.lua`：當此檔案存在時，它定義的腳本將在任何按鍵被按下時執行。這可以用來顯示更精緻的初始選單，或是作為單純的按鍵覆蓋。

**範例**：您可以建立一個 `_CLOCKWISE.lua` 檔案，內容為控制媒體的「下一首」。如此一來，在未開啟選單時，按下音量增加鍵就會觸發「下一首」而不是音量增加。

### 開發者模式

對於開發或偵錯 Lua 腳本的開發人員，Rotoris 包含 `--dev` 引數。使用此引數執行 Rotoris 會開啟一個主控台視窗，並將所有內嵌的 Lua 類型定義檔（`.d.lua`）提取到應用程式資料夾中的 `.types` 目錄。這些檔案有助於支援 Lua 語言伺服器的 IDE 提供 Rotoris Lua API 的自動完成和類型檢查。

```bash
Rotoris.exe --dev
```

## 技術堆疊

-   **.NET 9（WPF）**：應用程式使用 .NET 和 Windows Presentation Foundation 構建，用於其圖形使用者介面元素和系統整合。
-   **Lua（透過 NLua）**：提供強大且靈活的腳本功能。
-   **NAudio**：用於音訊相關功能。
-   **SkiaSharp**：為自訂視覺效果提供 2D 圖形渲染。
-   **Microsoft.Toolkit.Uwp.Notifications**：啟用現代 Windows 通知。
-   **Newtonsoft.Json**：主要用於組態與資料的 JSON 序列化和反序列化。

## 專案結構

此解決方案包含幾個主要專案：

-   **`Rotoris`**：主 WPF 應用程式，在背景執行、執行 Lua 腳本並管理系統互動。
-   **`RotorisConfigurationTool`**：一個獨立的 WPF 應用程式，專為組態 Rotoris 的設定和 Lua 腳本而設計。
-   **`RotorisLib`**：一個共用程式庫，包含 `Rotoris` 和 `RotorisConfigurationTool` 都使用的通用功能、公用程式和模型。
-   **`RotorisLib.Tests`**：一個測試專案，用於確保 `RotorisLib` 元件的品質和正確性。

