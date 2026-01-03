using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace ROMIdentifier.Utils
{
    public enum GameTDBPlatform
    {
        ThreeDS,
        DS,
        Switch,
        WiiU,
        WiiGameCube,
        PlayStation3
    }

    [XmlRoot(ElementName = "datafile")]
    public class WiiTDBDataset
    {
        [XmlElement("game")]
        public List<WiiTDBGame> Games { get; set; } = [];
    }

    public class WiiTDBGame
    {
        [XmlElement("id")]
        public string? Id { get; set; }
        [XmlElement("region")]
        public string? Region { get; set; }
        [XmlElement("languages")]
        public string? Languages { get; set; }

        [XmlElement("locale")]
        public List<WiiTDBLocale>? Localized { get; set; }
        [XmlElement("developer")]
        public string? Developer { get; set; }
        [XmlElement("publisher")]
        public string? Publisher { get; set; }
        [XmlElement("rom")]
        public WiiTDBRom? rom { get; set; }
    }

    public class WiiTDBLocale
    {
        [XmlAttribute("lang")]
        public string Language { get; set; } = "EN";
        [XmlElement("title")]
        public string Title { get; set; }
        [XmlElement("synopsis")]
        public string Synopsis { get; set; }
    }

    public class WiiTDBRom
    {
        [XmlAttribute("version")]
        public string Version { get; set; }
        [XmlAttribute("name")]
        public string Name { get; set; }
        [XmlAttribute("size")]
        public long Size { get; set; }
        [XmlAttribute("crc")]
        public string CRC { get; set; }
        [XmlAttribute("md5")]
        public string MD5 { get; set; }
        [XmlAttribute("sha1")]
        public string SHA1 { get; set; }
    }

    public static class GameTDB
    {
        public static WiiTDBDataset WiiDataset = new WiiTDBDataset();

        public static async Task<bool> DownloadAsync(
            GameTDBPlatform platform = GameTDBPlatform.WiiGameCube,
            bool titlesOnly = false
            )
        {
            HttpClient http = new();
            http.BaseAddress = new Uri("https://www.gametdb.com");

            string filename = "wiitdb";
            string ext = titlesOnly ? ".txt" : ".zip";
            switch (platform)
            {
                case GameTDBPlatform.WiiGameCube:
                    filename = "wiitdb";
                    break;
                case GameTDBPlatform.WiiU:
                    filename = "wiiutdb";
                    break;
                default:
                    throw new NotImplementedException("oops");
            }

            using var download = await http.GetStreamAsync($"{filename}{ext}");

            if (titlesOnly)
            {
                using var fs = new FileStream(
                Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "ROMIdentifier",
                    $"{filename}{ext}"
                    ),
                FileMode.Create, FileAccess.ReadWrite, FileShare.Read
                );
                await download.CopyToAsync(fs);
                return true;
            }

            using ZipArchive archive = new ZipArchive(download, ZipArchiveMode.Read);

            if (2 <= archive.Entries.Count || archive.Entries.Count <= 0)
                return false;

            var first = archive.Entries.First();
            var dest = Path.GetFullPath(Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "ROMIdentifier", first.FullName));
            first.ExtractToFile(dest, true);

            return true;
        }

        public static WiiTDBDataset GetWiiTDB()
        {
            if (WiiDataset is not null && WiiDataset.Games.Count > 0)
                return WiiDataset;

            var ser = new XmlSerializer(typeof(WiiTDBDataset));
            using var fs = new FileStream(Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "ROMIdentifier",
                    "wiitdb.xml"
                    ), FileMode.Open, FileAccess.Read, FileShare.Read);
            WiiTDBDataset dataset;
            dataset = (WiiTDBDataset)ser.Deserialize(fs);
            WiiDataset = dataset;
            return dataset;
        }

        public static RomScanningResult ScanWiiByGameId(string id, Action<string, int?> callback)
        {
            bool success = true;
            WiiTDBGame game = new();
            string msg = "";
            try
            {
                callback("Loading WiiTDB", 50);
                var d = GetWiiTDB();
                var games = d.Games;
                game = games.Find(x => x.Id.ToUpper() == id.ToUpper());

                if (game is null)
                {
                    success = false;
                    msg = "Game not found in WiiTDB";
                }
            } catch (FileNotFoundException e) {
                success = false;
                msg = "WiiTDB database could not be found";
            }

            var r = new RomScanningResult
            {
                Confidence = 2,
                Scanner = typeof(GameTDB)
            };

            if (success is false)
            {
                r.Success = false;
                r.Message = msg;
                return r;
            }

            r.Details.Title = game.Localized.Find(x => x.Language == "EN")?.Title ?? game.Id;
            r.Details.TitleId = id;
            return r;
        }
    }
}
