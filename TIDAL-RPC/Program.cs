using System;
using System.Linq;
using System.Timers;
using System.Diagnostics;
using System.Collections.Generic;

namespace TIDAL_RPC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.Title = "TIDAL-RPC (github.com/KNIF/TIDAL-RPC)";

            PrintTitle();

            try
            {
                var presence = new DiscordRpc.RichPresence();
                var handlers = new DiscordRpc.EventHandlers();

                DiscordRpc.Initialize("735159392554713099", ref handlers, true, null);

                presence.largeImageKey = "tidal";
                presence.largeImageText = "TIDAL";

                Console.WriteLine("Starting TIDAL...");

                PrintLine();

                Process.Start(
                    Environment.ExpandEnvironmentVariables(@"C:\Users\%username%\AppData\Local\TIDAL\TIDAL.exe"));

                var oldSongData = "";
                var timer = new Timer {AutoReset = true, Interval = 1000};

                timer.Elapsed += (sender, args2) =>
                {
                    Process[] tidalProc = Process.GetProcessesByName("TIDAL").Where(p => p.MainWindowTitle != "")
                        .ToArray();

                    if (tidalProc == null || tidalProc.Length < 1)
                    {
                        presence.details = "Offline";
                        presence.state = "";
                    }
                    else if (tidalProc[0].MainWindowTitle == "TIDAL")
                    {
                        presence.details = "Idling";
                        presence.state = "";
                    }
                    else
                    {
                        string[] songData = tidalProc[0].MainWindowTitle.Split('-');

                        string songName = songData[0].Remove(songData[0].Length - 1);
                        string songArtist = songData[1].Substring(1);

                        foreach (var c in replacedChars)
                        {
                            if (songName.Contains(c.Key))
                                songName = songName.Replace(c.Key, c.Value);

                            if (songArtist.Contains(c.Key))
                                songArtist = songArtist.Replace(c.Key, c.Value);
                        }

                        if (oldSongData != tidalProc[0].MainWindowTitle)
                            Console.WriteLine($"Playing \"{songName}\" by {songArtist}.");

                        oldSongData = tidalProc[0].MainWindowTitle;

                        presence.details = songName;
                        presence.state = songArtist;
                    }

                    DiscordRpc.UpdatePresence(ref presence);
                };

                timer.Start();

                Console.WriteLine("Rich Presence started. Press any key to close.");

                PrintLine();

                if (Console.ReadKey() != null)
                {
                    timer.Stop();
                    DiscordRpc.Shutdown();
                    Environment.Exit(0);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void PrintLine()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(@"---------------------------------------------------------");
            Console.ForegroundColor = ConsoleColor.White;
        }

        private static void PrintTitle()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(@"---------------------------------------------------------");
            Console.WriteLine(@"|  _____ ___ ____    _    _          ____  ____   ____  |");
            Console.WriteLine(@"| |_   _|_ _|  _ \  / \  | |        |  _ \|  _ \ / ___| |");
            Console.WriteLine(@"|   | |  | || | | |/ _ \ | |   _____| |_) | |_) | |     |");
            Console.WriteLine(@"|   | |  | || |_| / ___ \| |__|_____|  _ <|  __/| |___  |");
            Console.WriteLine(@"|   |_| |___|____/_/   \_\_____|    |_| \_\_|    \____| |");
            Console.WriteLine(@"|                                                       |");
            Console.WriteLine(@"---------------------------------------------------------");
            Console.ForegroundColor = ConsoleColor.White;
        }

        private static readonly Dictionary<string, string> replacedChars = new Dictionary<string, string>()
            {{"ä", "ae"}, {"ö", "oe"}, {"ü", "ue"}};
    }
}