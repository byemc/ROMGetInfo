using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROMIdentifier.Scanners
{
    internal class WiiScanner
    {
        public static (bool Success, RomResult Result) Scan(string path, Action<string, int?> callback)
        {
            var ext = Path.GetExtension(path);

            string gameId = "nothing";
            switch(ext.ToUpper().Replace(".",""))
            {
                case "RVZ":
                    gameId = Utils.RvzUtils.GetTitleId(path);
                    break;
                default:
                    return (false, null);
            }

            return (true, new RomResult()
            {
                TitleId = gameId
            });
        }

        /// <summary>
        /// MAKE SURE THE STREAM IS DISPOSED AFTERWARDS
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static string GetTitleIdFromStream(Stream stream, uint offset=0x00)
        {
            stream.Seek(offset, SeekOrigin.Begin);
            byte[] buffer = new byte[6];
            stream.Read(buffer, 0x00, 0x06);
            return Encoding.ASCII.GetString(buffer);
        }
    }
}
