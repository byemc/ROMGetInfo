using ROMIdentifier.Scanners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROMIdentifier
{
    public class RomResult
    {
        public Filetype Filetype { get; set; } = new Filetype();
        public string Path { get; set; }
        public string Filename { get; set; }

        public ICollection<RomScanningResult> Results { get; set; } = new List<RomScanningResult>();
    
        public RomScanningResult GetBestResult()
        {
            return Results
                .OrderByDescending(r => r.Confidence)
                .OrderByDescending(r => r.Success)
                .First();
        }
    }

    public class RomScanningResult
    {
        public bool Success { get; set; } = true;
        public int Confidence { get; set; } = 0;
        public string Message { get; set; } = "";
        public Type Scanner { get; set; }
        public RomDetails Details { get; set; } = new RomDetails();
    }

    public class RomDetails
    {
        public string TitleId { get; set; } = "";
        public string Title { get; set; } = "";
    }
}
