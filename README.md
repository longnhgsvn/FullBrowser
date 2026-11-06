# FullBrowser

A minimal, always-available desktop browser built with **VB.NET**, **Windows Forms**, and **Microsoft Edge WebView2**. It starts on [excalidraw.com](https://excalidraw.com/) and stays out of the way: no address bar, no tab strip, just a web view with a tiny menu and a flicker-free frameless window that you can move, resize, and make semi-transparent.

The idea is a lightweight "web surface" that can float above other windows — useful for quick sketches, dashboards, documentation, or any web app you want pinned on screen.

---

## Features

- **WebView2 rendering** — full Chromium engine via the Edge WebView2 runtime.
- **Frameless, draggable window** — hold `Ctrl` and drag anywhere on the page to move the window. The same works when holding `Ctrl` and dragging the small `...` button.
- **Optional border** — switch between a borderless window and a normal resizable window at runtime.
- **Always on top** — toggle TopMost from the menu.
- **Window opacity** — increase or decrease opacity in 10% steps (clamped between 0.1 and 1.0).
- **Open anything** — open a URL, a local `.html` / `.htm` / `.mhtml` file, or a folder through a small custom Open dialog.
- **Smart input handling** — bare input like `example.com` is treated as `https://example.com`; existing files and folders are converted to `file://` URIs; the window title shows a short preview of the location.
- **Tiny footprint UI** — an 18×14 `...` button in the top-left corner holds the only menu.

---

## Requirements

| Requirement | Version |
|---|---|
| OS | Windows 10 / 11 (x64 or ARM64) |
| .NET | .NET 8 SDK (`net8.0-windows`) |
| Runtime | [Microsoft Edge WebView2 Runtime](https://developer.microsoft.com/microsoft-edge/webview2/) (Evergreen) |
| IDE (optional) | Visual Studio 2022+ with the .NET desktop development workload |

The project references `Microsoft.Web.WebView2` **1.0.4078.44** through NuGet, so a package restore is required on first build.

---

## Build and run

### Visual Studio

1. Open `FullBrowser.sln`.
2. Let NuGet restore packages.
3. Press `F5`.

### Command line

```powershell
cd D:\app\FullBrowser
dotnet restore
dotnet build
dotnet run
```

The debug output is written to `bin\Debug\net8.0-windows\`. Note that WebView2 stores its user data in a sibling folder named `FullBrowser.exe.WebView2`, which is created on first launch.

---

## Usage

The application opens `https://excalidraw.com/` immediately on startup.

Click the small `...` button in the top-left corner to open the context menu:

| Menu item | Action |
|---|---|
| **Open** | Opens the Open dialog: type a URL, file path, or folder path, or click `...` to browse for a web file. |
| **Close** | Closes the application. |
| **Topmost** | Toggles whether the window stays above all other windows. |
| **Border On/Off** | Toggles between `FormBorderStyle.None` (frameless) and `Sizable` (standard window frame). |
| **Opacity +** | Increases window opacity by 0.1 (max 1.0). |
| **Opacity -** | Decreases window opacity by 0.1 (min 0.1). |

### Moving the window

Because the window is normally frameless, there is no title bar to drag. Instead:

- Hold `Ctrl` and drag **anywhere inside the page**, or
- Hold `Ctrl` and drag the `...` button.

This is implemented by injecting a small JavaScript listener into every page (`AddScriptToExecuteOnDocumentCreatedAsync`) that posts a `drag` message to the host. The host then calls the native `ReleaseCapture()` + `WM_NCLBUTTONDOWN`/`HTCAPTION` trick so Windows performs a real window drag.

### Opening locations

The Open dialog accepts:

- Absolute URLs — `https://example.com/page`
- Bare hostnames — `example.com` becomes `https://example.com`
- Local files — `C:\docs\notes.html` becomes `file:///C:/docs/notes.html`
- Local folders — opened as `file://` directory listings
- UNC paths — `\\server\share\page.html`

If the value cannot be resolved to a valid URI, a warning message is shown and nothing navigates.

---

## Configuration

A few constants at the top of `Form1.vb` control the behaviour:

```vb
Private Const DefaultUrl As String = "https://excalidraw.com/"
Private Const ResizeBorder As Integer = 0 '8
```

- **`DefaultUrl`** — the page loaded at startup and used as the fallback in the Open dialog.
- **`ResizeBorder`** — thickness of the custom edge grips used for resizing. It is currently `0`, which effectively disables the custom resize panels; set it to `8` (or another value) and uncomment the corresponding lines in `PositionCornerPanels()` to enable edge resizing.

The window minimum size is set to `200 × 150` in `Form1_Load`.

---

## Project structure

```
FullBrowser/
├── FullBrowser.sln              # Solution file
├── FullBrowser.vbproj           # SDK-style project (net8.0-windows, WinExe)
├── Form1.vb                     # All application logic
├── Form1.Designer.vb            # WebView2, button, context menu layout
├── Form1.resx                   # Form resources (icon, etc.)
├── ApplicationEvents.vb         # MyApplication partial class
├── My Project/
│   ├── Application.myapp
│   └── Application.Designer.vb
└── www.flaticon.com_00018.ico   # Application icon
```

### Implementation notes

- `InitializeWebViewAsync()` awaits `EnsureCoreWebView2Async`, registers the `WebMessageReceived` handler, injects the drag script, then navigates to the default URL.
- `CreateResizePanels()` builds seven transparent `Panel` controls (top, bottom, left, right, and three corners) that implement a manual resize routine with `Cursor.Position` deltas and minimum-size clamping.
- The page-level `Ctrl`+drag script uses the capture phase and calls `preventDefault()` / `stopPropagation()`, so it intercepts the gesture before the web app sees it. Pages that themselves rely on `Ctrl`+drag may conflict with window movement.
- No navigation history, tabs, downloads UI, or address bar is provided — use the web app's own UI and the Open dialog.

---

## Known limitations

- The custom resize handle logic is present but currently disabled (`ResizeBorder = 0`); resizing is only possible with **Border On** using the standard window frame.
- Only one window/instance is managed by the app; nothing prevents launching a second copy.
- `Ctrl`+drag always wins over page content, which may interfere with web apps that use the same shortcut.
- The app depends on the WebView2 Evergreen Runtime being installed; it will fail to initialize otherwise.
- No persistence of the last opened URL, opacity, or window position between sessions.

---

## Icon attribution

The application icon (`www.flaticon.com_00018.ico`) originates from [Flaticon](https://www.flaticon.com/). Check the original license terms before redistributing the binary.

---

## License

Released under the [MIT License](LICENSE).

Copyright (c) 2026 longnh.

Replace the copyright line in `LICENSE` with your own name or organization if you fork or redistribute this project.
