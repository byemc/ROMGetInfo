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
        public string TitleId { get; set; } = "";
        
    }

    public class Filetype
    {
        public string Mime { get; set; } = "application/octet-stream";
        public string Extenstion { get; set; } = "";
        public string Description { get; set; } = "Unknown";
    }
}
