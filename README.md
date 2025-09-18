# Network Traffic Dynamic Icon

A lightweight Windows system tray application that shows real‑time network traffic (Download / Upload).  
No main window, minimal CPU use, quick to build and publish.

---

## Features

- Tray icon only (no visible window).
- Live download/upload speed (auto unit: B/s, KB/s, MB/s, GB/s).
- Choose active network interface (Wi‑Fi / Ethernet / etc.).
- Compact dynamic icon (two short lines: down + up).
- Context menu: select interface, (optional) enable auto‑start, exit.
- Pure .NET (WinForms + `NetworkInterface` APIs), no external dependencies.

---

## How It Works

Every second:

1. Reads cumulative byte counters from the selected interface.
2. Computes deltas since last sample → bytes per second.
3. Converts to human‑readable units.
4. Updates tooltip (icon refreshed every few seconds to reduce GDI usage).

---

## Requirements

- Windows 10 / 11
- .NET 8 SDK (can target 6/7 if needed)
- Visual Studio 2022 or `dotnet` CLI

---

## Build

Visual Studio: open solution → Build (Release) → Run.  
CLI:

```bash
dotnet build -c Release
dotnet run --project NetworkTrafficDynamicIcon
```

---

## Publish

Framework‑dependent (smaller, needs .NET runtime installed):

```bash
dotnet publish -c Release -r win-x64 -p:PublishSingleFile=true --self-contained false
```

Self‑contained (larger, no runtime required):

```bash
dotnet publish -c Release -r win-x64 -p:PublishSingleFile=true --self-contained true
```

Result:  
`bin/Release/net8.0-windows/win-x64/publish/NetworkTrafficDynamicIcon.exe`

---

## Usage

1. Launch the EXE (icon appears in the tray; drag it out of the hidden area if needed).
2. Hover for current Down/Up speeds.
3. Right‑click to select interface or exit.

---

## Limitations

- Instantaneous values can fluctuate (no smoothing yet).
- Tiny icon = very compact numbers.
- No historical graphs (future possibility).

---

## Roadmap (Planned / Ideas)

- Average (moving window) smoothing
- Sum of all active interfaces
- Optional bits/s display
- Persist selected interface (config)
- First‑run balloon hint
- Auto‑update via GitHub Releases
- Optional miniature graph
- IP Helper API for full 64‑bit unified counters

---

## License

Choose a license (e.g. MIT). Example placeholder:

```
MIT License
Copyright (c) ...
Permission is hereby granted, free of charge, ...
```

---

## FAQ

**Why is the icon hidden?**  
Windows decides—drag it from the overflow panel to pin it.

**Can I get graphs?**  
Not yet—planned.

**Does it send data externally?**  
No. Reads local OS counters only.
