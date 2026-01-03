using MimeDetective;
using ROMIdentifier.Definitions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROMIdentifier
{
    public static class RomFile
    {
        private static Dictionary<string, Func<string, Action<string, int>>> FiletypeMappings = new Dictionary<string, Func<string, Action<string, int>>>();

        public static RomResult Identify(string path, Action<string, int?> callback)
        {
            // Get file info
            var fileName = Path.GetFileName(path);
            callback($"Identifying file {fileName}...", null);

            var instance = RomContentInspector.Instance;

            var filetype = instance.Inspect(path).OrderByDescending(r=>r.Points).FirstOrDefault();
            var fileExt = filetype?.Definition.File.Extensions.FirstOrDefault() ?? "";
            var fileMime = filetype?.Definition.File.MimeType ?? "";
            var fileDescription = filetype?.Definition.File.Description ?? "";

            var results = new List<RomScanningResult>();

            switch (fileMime)
            {
                case "application/x-wii-rom":
                case "application/x-gamecube-rom":
                    results.AddRange(Scanners.WiiDiscScanner.Scan(path, callback));
                    break;
            }

            var failures = results.FindAll(x => x.Success == false);
            var text = failures.Count == 1 ? "failure" : "failures";
            if (failures.Count > 0)
                callback($"Ready with {failures.Count} {text}.", 100);
            else
                callback("Ready.", 100);

            return new RomResult()
            {
                Filetype = new Filetype()
                {
                    Description = fileDescription,
                    Mime = fileMime,
                    Extenstion = fileExt
                },
                Path = path,
                Results = results
            };
        }
        public static RomResult Identify(string path)
        {
            return Identify(path, (string str, int? dou) => { return; });
        }
    }
}
