using System;
using System.Linq;
using System.Timers;
using System.Diagnostics;

namespace TIDAL_RPC
{
    public class Program
    {
        private static DiscordRpc.RichPresence presence = new DiscordRpc.RichPresence();

        public static void Main(string[] args)
        {
            Console.Title = "TIDAL-RPC (github.com/KNIF/TIDAL-RPC)";

            Console.WriteLine(@"  _____ ___ ____    _    _          ____  ____   ____ ");
            Console.WriteLine(@" |_   _|_ _|  _ \  / \  | |        |  _ \|  _ \ / ___|");
            Console.WriteLine(@"   | |  | || | | |/ _ \ | |   _____| |_) | |_) | |    ");
            Console.WriteLine(@"   | |  | || |_| / ___ \| |__|_____|  _ <|  __/| |___ ");
            Console.WriteLine(@"   |_| |___|____/_/   \_\_____|    |_| \_\_|    \____|");

            try
            {
                var handlers = new DiscordRpc.EventHandlers();
                DiscordRpc.Initialize("735159392554713099", ref handlers, true, null);

                presence.largeImageKey = "tidal";
                presence.largeImageText = "TIDAL";

                var timer = new Timer();
                timer.Elapsed += (sender, args2) => Update();
                timer.AutoReset = true;
                timer.Interval = 1000;
                timer.Start();

                Console.WriteLine("\nRich Presence started. Press any key to close.");

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

        private static void Update()
        {
            Process[] tidalProc = Process.GetProcessesByName("TIDAL").Where(p => p.MainWindowTitle != "").ToArray();

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

                presence.details = songData[0].Remove(songData[0].Length - 1);
                presence.state = songData[1].Substring(1);
            }

            DiscordRpc.UpdatePresence(ref presence);
        }
    }
}
