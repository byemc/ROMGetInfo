using ROMIdentifier.Scanners;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROMIdentifier.Utils
{
    /// <summary>
    /// Extracts the Title ID from RVZ files.
    /// </summary>
    internal class RvzUtils
    {
        public static string GetTitleId(string path)
        {
            var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);

            // Check magic number
            const string magic = "WIA\x1";

            // Get title-id
            var id = WiiScanner.GetTitleIdFromStream(fs, 0x58);

            fs.Dispose();

            return id;
        }
    }
}
