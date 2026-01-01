using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MimeDetective;
using MimeDetective.Storage;

namespace ROMIdentifier.Definitions
{
    internal static class RomContentInspector
    {
        public static IContentInspector Instance { get; }

        static RomContentInspector()
        {
            var defs = new List<Definition>();

            // Add default definitions from MimeDetective
            defs.AddRange(MimeDetective.Definitions.DefaultDefinitions.All());

            // *** DEFINITIONS BASED ON `file` https://github.com/file/file/blob/5184ca2471c0e801c156ee120a90e669fe27b31d/magic/Magdir/console#L855 *** //

            //// WII & GAMECUBE
            defs.Add(new Definition()
            {
                File = new()
                {
                    Categories = new[] { Category.Executable }.ToImmutableHashSet(),
                    Description = "Wii disc image",
                    Extensions = new[] { "iso" }.ToImmutableArray(),
                    MimeType = "application/x-wii-rom"
                },
                Signature = new Segment[]
                {
                    PrefixSegment.Create(0x18, "5D 1C 9E A3") // 0x5D1C9EA3
                }.ToSignature()
            });

            defs.Add(new Definition()
            {
                File = new()
                {
                    Categories = new[] { Category.Executable }.ToImmutableHashSet(),
                    Description = "GameCube/Wii disc image (RVZ format)",
                    Extensions = new[] { "rvz" }.ToImmutableArray(),
                    MimeType = "application/octet-stream"
                },
                Signature = new Segment[]
                {
                    PrefixSegment.Create(0x00, "52 56 5A 01")
                }.ToSignature()
            });

            defs.Add(new Definition()
            {
                File = new()
                {
                    Categories = new[] { Category.Executable }.ToImmutableHashSet(),
                    Description = "GameCube disc image (RVZ format)",
                    Extensions = new[] { "rvz" }.ToImmutableArray(),
                    MimeType = "application/x-gamecube-rom"
                },
                Signature = new Segment[]
                {
                    PrefixSegment.Create(0x00, "52 56 5A 01"),
                    PrefixSegment.Create(0x48, "00 00 00 01")
                }.ToSignature()
            });

            defs.Add(new Definition()
            {
                File = new()
                {
                    Categories = new[] { Category.Executable }.ToImmutableHashSet(),
                    Description = "Wii disc image (RVZ format)",
                    Extensions = new[] { "rvz" }.ToImmutableArray(),
                    MimeType = "application/x-wii-rom"
                },
                Signature = new Segment[]
                {
                    PrefixSegment.Create(0x00, "52 56 5A 01"),
                    PrefixSegment.Create(0x48, "00 00 00 02")
                }.ToSignature()
            });

            Instance = new ContentInspectorBuilder()
            {
                Definitions = defs,
                StringSegmentOptions = new StringSegmentOptionsBuilder()
                {
                    OptimizeFor = MimeDetective.Engine.StringSegmentResourceOptimization.HighSpeed
                }
            }.Build();
        }
    }
}
