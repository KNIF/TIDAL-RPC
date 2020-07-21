using System;
using System.Linq;
using System.Timers;
using System.Diagnostics;

namespace TIDAL_RPC
{
    public class Program
    {
        private static DiscordRpc.RichPresence presence = new DiscordRpc.RichPresence();
        private static DiscordRpc.EventHandlers handlers = new DiscordRpc.EventHandlers();

        private static Timer timer = new Timer();

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
                DiscordRpc.Initialize("735159392554713099", ref handlers, true, null);

                timer.Elapsed += (sender, args2) => Update(ref presence);
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

        private static void Update(ref DiscordRpc.RichPresence presence)
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

                string songName = songData[0];
                songName = songName.Remove(songName.Length - 1);

                string songArtist = songData[1];
                songArtist = songArtist.Substring(1);

                presence.details = songName;
                presence.state = songArtist;
            }

            presence.largeImageKey = "tidal";
            presence.largeImageText = "TIDAL";

            DiscordRpc.UpdatePresence(ref presence);
        }
    }
}
