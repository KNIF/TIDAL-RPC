using System;
using System.Diagnostics;
using System.Linq;
using System.Timers;

namespace TIDAL_RPC
{
    public class Program
    {
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
                DiscordRpc.RichPresence presence = new DiscordRpc.RichPresence();
                DiscordRpc.EventHandlers handlers = new DiscordRpc.EventHandlers();

                DiscordRpc.Initialize("735159392554713099", ref handlers, true, null);

                var timer = SetInterval(() => Update(ref presence), 1000);
                timer.Start();

                Console.WriteLine("\nRPC started.");

                if (Console.ReadKey().Key == ConsoleKey.Escape)
                {
                    timer.Stop();
                    DiscordRpc.Shutdown();
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
            else
            {
                if (tidalProc[0].MainWindowTitle == "TIDAL")
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
            }

            presence.largeImageKey = "tidal";
            presence.largeImageText = "TIDAL";

            DiscordRpc.UpdatePresence(ref presence);
        }

        private static Timer SetInterval(Action Act, int Interval)
        {
            Timer tmr = new Timer();
            tmr.Elapsed += (sender, args) => Act();
            tmr.AutoReset = true;
            tmr.Interval = Interval;
            tmr.Start();

            return tmr;
        }
    }
}
