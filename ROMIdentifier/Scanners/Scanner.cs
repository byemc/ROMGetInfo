using System;
using System.Collections.Generic;

namespace ROMIdentifier.Scanners
{
    public abstract class Scanner
    {
        public static ICollection<Filetype> SupportedFiletypes { get; set; }
        public static ICollection<RomScanningResult> Scan(string path, Action<string, int?> callback) 
        {
            return new List<RomScanningResult> { new RomScanningResult() { Success = false } }; 
        }
    }
}
