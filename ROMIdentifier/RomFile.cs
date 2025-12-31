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
        public static RomResult Identify(string path, Action<string, int?> callback)
        {
            // Get file info
            var fileName = Path.GetFileName(path);
            callback($"Identifying file {fileName}...", null);

            var fileExt = Path.GetExtension(path);

            // Forcing to use Dolphin
            var result = Scanners.WiiScanner.Scan(path, callback);

            if (result.Success)
            { 
                callback($"Identified {fileName}", 100);
                return result.Result ?? new RomResult();
            }
            else
            {
                callback($"Failed to identify {fileName}", 100);
                return result.Result ?? new RomResult();
            }
            
        }
        public static RomResult Identify(string path)
        {
            return Identify(path, (string str, int? dou) => { return; });
        }
    }
}
