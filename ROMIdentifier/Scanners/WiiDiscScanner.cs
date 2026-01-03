using ROMIdentifier.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ROMIdentifier.Scanners
{
    internal class WiiDiscScanner : Scanner
    {
        public static ICollection<RomScanningResult> Scan(string path, Action<string, int?> callback)
        {
            var ext = Path.GetExtension(path);

            callback("Scanning as Wii/GameCube disc", 0);

            var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            int offset = 0x00;
            bool success = true;

            var results = new List<RomScanningResult>();

            // Try to match with common Wii and Gamecube backup formats
            // WBFS requires more effort i think so its not supported
            // but for basic information its just deteremening if its WIA, RVZ or ISO
            // and changing an offset
            byte[] wiaMagic = Encoding.ASCII.GetBytes("WIA\x01");
            byte[] rvzMagic = Encoding.ASCII.GetBytes("RVZ\x01");

            byte[] wiaBuffer = new byte[0x04];
            fs.Read(wiaBuffer, 0x00, 0x04);
            if (wiaBuffer.SequenceEqual(wiaMagic) || wiaBuffer.SequenceEqual(rvzMagic))
                offset = 0x58; // Skip past the WIA/RVZ header into the portion that copies the first 80 bytes of the disc

            // Check if this is either a GameCube disc or a Wii disc.
            const uint gamecubeMagic = 0xC2339F3D;
            const uint wiiMagic = 0x5D1C9EA3;

            fs.Seek(offset + 0x18, SeekOrigin.Begin);
            byte[] wiiBuffer = new byte[0x04];
            fs.Read(wiiBuffer, 0x00, 0x04);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(wiiBuffer);

            fs.Seek(offset, SeekOrigin.Begin);
            fs.Seek(0x01C, SeekOrigin.Current);
            byte[] gcBuffer = new byte[0x04];
            fs.Read(gcBuffer, 0x00, 0x04);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(gcBuffer);
            if (BitConverter.ToUInt32(wiiBuffer, 0x00) != wiiMagic &&
                BitConverter.ToUInt32(gcBuffer, 0x00) != gamecubeMagic)
            {
                // We do NOT have a Wii disc. ABORT!!
                fs.Dispose();
                results.Add(new RomScanningResult
                {
                    Scanner = typeof(WiiDiscScanner),
                    Message = "Not a Wii/GC disc image",
                    Success = false,
                    Confidence = int.MinValue
                });
                return results;
            }

            fs.Seek(offset, SeekOrigin.Begin);
            byte[] buffer = new byte[6];
            fs.Read(buffer, 0x00, 0x06);
            var gameId = Encoding.ASCII.GetString(buffer); // POS 0x06

            fs.Seek(0x1A, SeekOrigin.Current); // 0x20 - 0x06 = 0x1A
            byte[] titleBuffer = new byte[64];
            fs.Read(titleBuffer, 0x00, 64);
            var internalTitle = Encoding.ASCII.GetString(titleBuffer);
            fs.Dispose();

            results.Add(new()
            {
                Scanner = typeof(WiiDiscScanner),
                Confidence = -1,        // Data won't be that good, you see.
                Details = new()
                {
                    TitleId = gameId,
                    Title = internalTitle
                },
                Success = true
            });

            callback("Identified as a Wii/GC disc..", 25);

            results.Add(GameTDB.ScanWiiByGameId(gameId, callback));

            callback("Grabbed from GameTDB.", 100);

            return results;
        }
    }
}
