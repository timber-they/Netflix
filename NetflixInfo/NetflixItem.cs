using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace NetflixInfo
{
    internal class NetflixItem
    {
        public string Title            { get; set; }
        public string VideoTitle       { get; set; }
        public int    MovieId          { get; set; }
        public string Country          { get; set; }
        public int    Bookmark         { get; set; }
        public int    Duration         { get; set; }
        public long   Date             { get; set; }
        public int    DeviceType       { get; set; }
        public string DateStr          { get; set; }
        public int    Index            { get; set; }
        public string TopNodeId        { get; set; }
        public int    Series           { get; set; }
        public string SeriesTitle      { get; set; }
        public string SeasonDescriptor { get; set; }
        public string EpisodeTitle     { get; set; }
        public string EstRating        { get; set; }

        public readonly string csv = ";";

        public override string ToString ()
        {
            return Title +
                   csv +
                   VideoTitle +
                   csv +
                   MovieId +
                   csv +
                   Country +
                   csv +
                   Bookmark +
                   csv +
                   Duration +
                   csv +
                   Date +
                   csv +
                   DeviceType +
                   csv +
                   DateStr +
                   csv +
                   Index +
                   csv +
                   TopNodeId +
                   csv +
                   Series +
                   csv +
                   SeriesTitle +
                   csv +
                   SeasonDescriptor +
                   csv +
                   EpisodeTitle +
                   csv +
                   EstRating;
        }

        public static string GetTitles ()
        {
            return
                "title;videoTitle;movieID;country;bookmark;duration;date;deviceType;dateStr;index;topNodeId;series;seriesTitle;seasonDescriptor;episodeTitle;estRating";
        }
    }
}