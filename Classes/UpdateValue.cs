using System.Collections.Generic;
using System.Windows;
using GadzzaaTB.Windows;
using OsuMemoryDataProvider.OsuMemoryModels;

namespace GadzzaaTB.Classes
{
    public class ModsArray
    {
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public int Id { get; set; }
        public string ModName { get; set; }
        public int ModValue { get; set; }
    }

    public static class UpdateValue
    {
        private static readonly MainWindow MainWindow = (MainWindow) Application.Current.MainWindow;
        private static readonly OsuBaseAddresses Data = MainWindow.Main.BaseAddresses;

        private static readonly List<ModsArray> ModsArray = new List<ModsArray>
        {
            new ModsArray {Id = 0, ModName = "NF", ModValue = 1},
            new ModsArray {Id = 1, ModName = "EZ", ModValue = 2},
            new ModsArray {Id = 2, ModName = "HD", ModValue = 8},
            new ModsArray {Id = 3, ModName = "HR", ModValue = 16},
            new ModsArray {Id = 4, ModName = "SD", ModValue = 32},
            new ModsArray {Id = 5, ModName = "DT", ModValue = 64},
            new ModsArray {Id = 6, ModName = "RX", ModValue = 128},
            new ModsArray {Id = 7, ModName = "HT", ModValue = 256},
            new ModsArray {Id = 8, ModName = "NC", ModValue = 512},
            new ModsArray {Id = 9, ModName = "FL", ModValue = 1024},
            new ModsArray {Id = 10, ModName = "AU", ModValue = 2048},
            new ModsArray {Id = 11, ModName = "SO", ModValue = 4096},
            new ModsArray {Id = 12, ModName = "AP", ModValue = 8192},
            new ModsArray {Id = 13, ModName = "PF", ModValue = 16416},
            new ModsArray {Id = 14, ModName = "CN", ModValue = 4196352},
            new ModsArray {Id = 15, ModName = "TP", ModValue = 8388608},
            new ModsArray {Id = 16, ModName = "V2", ModValue = 536870912}
        };

        public static void UpdateValues()
        {
            MainWindow.DebugOsu.dl = "https://osu.ppy.sh/beatmaps/" + Data.Beatmap.SetId;
            MainWindow.DebugOsu.mods = Data.GeneralData.Mods;
            MainWindow.DebugOsu.mapInfo = Data.Beatmap.MapString;
            MainWindow.DebugOsu.mStars = Data.Beatmap.SetId;
        }

        public static string UpdateMods(int i)
        {
            var modsText = "";
            if (i == 0) modsText = "NM";
            while (i != 0)
                for (var j = ModsArray.Count - 1; j >= 0; j--)
                    if (i >= ModsArray[j].ModValue)
                    {
                        i -= ModsArray[j].ModValue;
                        modsText += ModsArray[j].ModName;
                    }

            if (modsText.Contains("DT") && modsText.Contains("NC")) modsText = modsText.Replace("DT", "");
            return modsText;
        }
    }
}