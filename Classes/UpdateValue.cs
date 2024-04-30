using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using GadzzaaTB.Windows;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Octokit;
using OppaiSharp;
using OsuMemoryDataProvider.OsuMemoryModels;
using OsuMemoryDataProvider.OsuMemoryModels.Direct;
using Application = System.Windows.Application;

namespace GadzzaaTB.Classes
{
    public class UpdateValue
    {
        private static readonly MainWindow MainWindow = (MainWindow)Application.Current.MainWindow;
        private static readonly OsuBaseAddresses Data = MainWindow.BaseAddresses;
        public static double stars;
        public int beatmapId;
        public static int tested=0;

        private static readonly List<ModsArray> ModsArray = new List<ModsArray>
        {
            new ModsArray { Id = 0, ModName = "NF", ModValue = 1 },
            new ModsArray { Id = 1, ModName = "EZ", ModValue = 2 },
            new ModsArray { Id = 2, ModName = "HD", ModValue = 8 },
            new ModsArray { Id = 3, ModName = "HR", ModValue = 16 },
            new ModsArray { Id = 4, ModName = "SD", ModValue = 32 },
            new ModsArray { Id = 5, ModName = "DT", ModValue = 64 },
            new ModsArray { Id = 6, ModName = "RX", ModValue = 128 },
            new ModsArray { Id = 7, ModName = "HT", ModValue = 256 },
            new ModsArray { Id = 8, ModName = "NC", ModValue = 512 },
            new ModsArray { Id = 9, ModName = "FL", ModValue = 1024 },
            new ModsArray { Id = 10, ModName = "AU", ModValue = 2048 },
            new ModsArray { Id = 11, ModName = "SO", ModValue = 4096 },
            new ModsArray { Id = 12, ModName = "AP", ModValue = 8192 },
            new ModsArray { Id = 13, ModName = "PF", ModValue = 16416 },
            new ModsArray { Id = 14, ModName = "CN", ModValue = 4196352 },
            new ModsArray { Id = 15, ModName = "TP", ModValue = 8388608 },
            new ModsArray { Id = 16, ModName = "V2", ModValue = 536870912 }
        };
        
        static async Task GetBeatmapStarRating(int beatmapId, string apiKey)
        {
            if (tested == beatmapId) return;
            var url = $"https://osu.ppy.sh/api/get_beatmaps?k={apiKey}&b={beatmapId}";
            var client = new HttpClient();

            try
            {
                var response = await client.GetStringAsync(url);
                var jsonObj = JArray.Parse(response);
            
                if (jsonObj.Count > 0)
                {
                    var beatmap = jsonObj[0];
                    var starRating = (string)beatmap[@"difficultyrating"];
                    stars = (double)(beatmap["difficultyrating"] ?? throw new InvalidOperationException());
                    Console.WriteLine($@"Star Rating of beatmap {beatmapId} is : {starRating}");
                    tested = beatmapId;
                }
                else
                {
                    Console.WriteLine($@"No beatmaps found with ID {beatmapId}");
                }
            }
            catch(HttpRequestException ex)
            {
                Console.WriteLine($@"An error occurred when sending the request to the osu! API: {ex.Message}");
            }
            catch(IndexOutOfRangeException)
            {
                Console.WriteLine(@"The response did not include any beatmaps. The ID may be incorrect or the beatmap may not exist");
            }
            catch(Exception ex)
            {
                Console.WriteLine($@"An exception occurred when processing the response: {ex.Message}");
            }
        }
        
        public async void UpdateValues()
        {
            beatmapId = Data.Beatmap.Id;
            if (MainWindow.DebugOsu is null) return;
            MainWindow.DebugOsu.dl = "https://osu.ppy.sh/beatmaps/" + beatmapId;
            MainWindow.DebugOsu.mods = Data.GeneralData.Mods;
            MainWindow.DebugOsu.mapInfo = Data.Beatmap.MapString;
            Console.WriteLine(beatmapId.ToString());
            try
            {
                await GetBeatmapStarRating(beatmapId, "7113a08a3bd3de56b98b51385bfe0ead0b027ba3");
            }
            catch(Exception ex)
            {
                Console.WriteLine($@"An exception occurred: {ex.Message}");
            }
            MainWindow.DebugOsu.mStars = Math.Round(stars, 2); 
        }

        public static string? UpdateMods(int i)
        {
            var modsText = "";
            modsText = "NM";
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