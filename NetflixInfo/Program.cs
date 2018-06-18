using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

using Newtonsoft.Json;


namespace NetflixInfo
{
    internal static class Program
    {
        private static void Main ()
        {
            if (!Directory.Exists ("cookies"))
                return;

            var files = Directory.GetFiles ("cookies");
            //Export

            foreach (var file in files)
            {
                _items = new List <NetflixItem> ();
                var f            = new FileInfo (file);
                var exportFolder = f.Name;
                Directory.CreateDirectory (exportFolder);

                var c = File.ReadAllText (file);
                var i = 0;
                while (LoadJson (i++, c)) {}


                var lines = new List <string> {NetflixItem.GetTitles ()};
                foreach (var item in _items)
                    lines.Add (item.ToString ());


                File.WriteAllLines (exportFolder + @"\Netflix.csv", lines.ToArray (), Encoding.UTF8);

                var times = new Dictionary <string, int> ();
                var count = new Dictionary <string, int> ();
                foreach (var value in _items)
                    if (times.ContainsKey (value.DateStr))
                    {
                        times [value.DateStr] += value.Duration;
                        count [value.DateStr] += 1;
                    }
                    else
                    {
                        times.Add (value.DateStr, value.Duration);
                        count.Add (value.DateStr, 1);
                    }

                var lines2 = new List <string> {"Date;Count;Time in s;Time in m;Time in h"};
                foreach (var pair in times)
                    lines2.Add (pair.Key +
                                ";" +
                                count [pair.Key] +
                                ";" +
                                pair.Value +
                                ";" +
                                (float) pair.Value / 60 +
                                ";" +
                                (float) pair.Value / (60 * 60));

                File.WriteAllLines (exportFolder + @"\NetflixTimes.csv", lines2.ToArray (), Encoding.UTF8);
                GenerateStats (exportFolder);
                GenerateSeriesStats (exportFolder);
            }
        }

        private static void GenerateStats (string exportFolder)
        {
            var stats = new List <string> ();
            //----------------------liefetime watched-------------------------------------
            var completeTime = 0.0;
            foreach (var i in _items)
                completeTime += i.Duration;

            stats.Add ("Watched in s;" + completeTime);
            stats.Add ("Watched in m;" + completeTime / 60);
            stats.Add ("Watched in h;" + completeTime / (60 * 60));


            //----------------------most watched serie-------------------------------------

            var serie = new Dictionary <string, int> ();
            foreach (var i in _items)
            {
                if (string.IsNullOrWhiteSpace (i.SeriesTitle))
                    continue;
                if (serie.ContainsKey (i.SeriesTitle))
                    serie [i.SeriesTitle]++;
                else
                    serie.Add (i.SeriesTitle, 1);
            }

            var serieList = serie.ToList ();
            serieList.Sort ((pair1, pair2) => pair2.Value.CompareTo (pair1.Value));

            for (var i = 0; i < 10; i++)
                stats.Add ("Most watched Serie Nr." + (i + 1) + ";" + serieList [i].Key + ";" + serieList [i].Value);

            File.WriteAllLines (exportFolder + @"\NetflixStats.csv", stats.ToArray (), Encoding.UTF8);
        }

        private static void GenerateSeriesStats (string exportFolder)
        {
            var stats = new List <string> {"Series;StartDate;EndDate;DateDiff;ActiveDays;HoursWatched"};

            var seriesNames = _items.Select (item => item.SeriesTitle).Distinct ();
            stats.AddRange (from series in seriesNames
                            let seriesItems = _items.Where (item => item.SeriesTitle == series)
                            let seriesDates = seriesItems.
                                Select (item => DateTime.Parse (item.DateStr))
                            let beginning = seriesDates.Min ()
                            let ending = seriesDates.Max ()
                            let range = ending - beginning
                            let activeDays = seriesDates.Distinct ().Count ()
                            let duration = seriesItems.Sum (item => item.Duration) / 3600.0
                            orderby activeDays descending
                            select
                                $"{series ?? "Movies"};" +
                                $"{beginning.ToShortDateString ()};{ending.ToShortDateString ()};" +
                                $"{range.Days};{activeDays};{duration}");

            File.WriteAllLines (exportFolder + @"\SeriesStats.csv", stats.ToArray (), Encoding.UTF8);
        }

        private static List <NetflixItem> _items;

        private static bool LoadJson (int page, string cookie)
        {
            using (var wc = new WebClient ())
            {
                wc.Headers = new WebHeaderCollection {{"Cookie", cookie}};
                var json = wc.DownloadString ("https://www.netflix.com/api/shakti/7742b8c7/viewingactivity?pg=" +
                                              page +
                                              "&pgSize=100");
                var stuff = JsonConvert.DeserializeObject <NetflixActivity> (json);
                if (stuff.viewedItems.Count > 0)
                    foreach (var t in stuff.viewedItems)
                        _items.Add (t);
                else
                    return false;
            }

            return true;
        }
    }
}