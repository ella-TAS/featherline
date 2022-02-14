using System.IO;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Linq;
using System.Collections;
using System;
using System.Collections.Generic;

namespace Featherline
{
    public static class Level
    {
        public static DeathMapInfo DeathMap;

        public static IntVec2[] Spinners;
        public static RectangleHitbox[] Killboxes;
        public static Spike[] Spikes;

        public static SolidTileInfo Tiles;

        public static WindTrigger[] WindTriggers;
        public static Vector2 InitWind;

        public static bool HasHazards;

        public static RectangleHitbox[] Colliders;

        public static NormalJT[] NormalJTs;
        public static CustomJT[] CustomJTs;

        public static Checkpoint[] Checkpoints;

        static Settings srcSett;

        private static Savestate startState;
        public static Savestate StartState { get => startState.Copy(); }
        public static bool defineStartBoost;

        private static Regex getIntPair = new Regex(@"(-?\d+)\.?\d*, (-?\d+)\.?\d*");
        private static IntVec2 MatchToIntVec2(Match src) => new IntVec2(int.Parse(src.Groups[1].Value), int.Parse(src.Groups[2].Value));
        private static IntVec2[] GetIntPairs(string src) => getIntPair.Matches(src).Select(MatchToIntVec2).ToArray();
        private static bool[] GetBools(string src) => Regex.Matches(src, @"True|False").Select(s => s.Value == "True").ToArray();

        public static void Prepare(Settings sourceSettings)
        {
            Colliders = new RectangleHitbox[0];
            Killboxes = new RectangleHitbox[0];

            srcSett = sourceSettings;
            string src = File.ReadAllText(srcSett.InfoFile);
            var split = Regex.Match(src, @"(-?\d+\.?\d*, -?\d+\.?\d*\t-?\d+\.?\d*, -?\d+\.?\d*\t\w*\t\w*\s*Lerp:\s*-?\d+\.?\d*)(.*)LightningUL:(.*)LightningDR:(.*)SpikeUL:(.*)SpikeDR:(.*)SpikeDir:(.*)Wind:(.*)WTPos:(.*)WTPattern:(.*)WTWidth:(.*)WTHeight(.*)JThruUL:(.*)Bounds:(.*)");

            if (!split.Success)
                throw new Exception("Unable to parse information from infodump.txt");

            GetStartState(split.Groups[1].Value);

            GetSpinners(split.Groups[2].Value);
            GetLightning(split.Groups[3].Value, split.Groups[4].Value);
            GetSpikes(split.Groups[5].Value, split.Groups[6].Value, split.Groups[7].Value);

            GetWind(split.Groups[8].Value, split.Groups[9].Value, split.Groups[10].Value, split.Groups[11].Value, split.Groups[12].Value);
            GetJumpThrus(split.Groups[13].Value);

            GetSolidTiles(split.Groups[14].Value);

            srcSett.ManualHitboxes ??= new string[0];
            GetCustomHitboxes();
            srcSett.Checkpoints ??= new string[0];
            GetCheckpoints();

            CreateDangerBitfield();
        }

        private static void GetStartState(string src)
        {
            startState = new Savestate();

            Match m = Regex.Match(src, @"(-?\d+\.\d+), (-?\d+\.\d+)\t(-?\d+\.\d+), (-?\d+\.\d+).*Lerp:(.*)");
            startState.fState.pos.X = float.Parse(m.Groups[1].Value);
            startState.fState.pos.Y = float.Parse(m.Groups[2].Value);
            startState.fState.UpdateIntPos();
            startState.fState.spd.X = float.Parse(m.Groups[3].Value);
            startState.fState.spd.Y = float.Parse(m.Groups[4].Value);
            startState.fState.speedLerp = float.Parse(m.Groups[5].Value);
            defineStartBoost = startState.fState.spd.X != 0 || startState.fState.pos.Y != 0;
        }

        private static void GetSpinners(string src)
        {
            Spinners = getIntPair.Matches(src)
                .Select(MatchToIntVec2)
                .Distinct()
                .ToArray();
        }

        private static void GetLightning(string UL, string DR)
        {
            var getUL = getIntPair.Matches(UL);
            var getDR = getIntPair.Matches(DR);

            Killboxes = Killboxes.Concat(getUL.Select((m, i) => new RectangleHitbox(new Bounds(
                    int.Parse(m.Groups[1].Value),
                    int.Parse(m.Groups[2].Value),
                    int.Parse(getDR[i].Groups[1].Value),
                    int.Parse(getDR[i].Groups[2].Value)
                    ).Expand(false)
                ))).ToArray();
        }

        private static void GetSpikes(string UL, string DR, string dir)
        {
            var ULs = GetIntPairs(UL);
            var DRs = GetIntPairs(DR);
            var getDir = Regex.Matches(dir, @"Left|Right|Up|Down");

            Spikes = ULs.Select((v, i) => new Spike(new Bounds(v, DRs[i]).Expand(false), getDir[i].Value)).ToArray();

            /*var getUL = getIntPair.Matches(UL);
            var getDR = getIntPair.Matches(DR);

            Spikes = getUL.Select((m, i) => new Spike(new Bounds(
                    int.Parse(m.Groups[1].Value),
                    int.Parse(m.Groups[2].Value),
                    int.Parse(getDR[i].Groups[1].Value),
                    int.Parse(getDR[i].Groups[2].Value)
                    ).Expand(false),
                    getDir[i].Value
                )).ToArray();*/
        }

        private static void GetJumpThrus(string src)
        {
            var split = Regex.Match(src,
                @"(.*)JThruDR:(.*)SideJTUL:(.*)SideJTDR:(.*)SideJTIsRight:(.*)SideJTPushes:(.*)UpsDJTUL:(.*)UpsDJTDR:(.*)UpsDJTPushes:(.*)");

            IntVec2[] normalULs = GetIntPairs(split.Groups[1].Value);
            IntVec2[] normalDRs = GetIntPairs(split.Groups[2].Value);
            IntVec2[] sideULs = GetIntPairs(split.Groups[3].Value);
            IntVec2[] sideDRs = GetIntPairs(split.Groups[4].Value);
            bool[] sidesToR = GetBools(split.Groups[5].Value);
            bool[] sidesPush = GetBools(split.Groups[6].Value);
            IntVec2[] upsDULs = GetIntPairs(split.Groups[7].Value);
            IntVec2[] upsDDRs = GetIntPairs(split.Groups[8].Value);
            bool[] upsDPush = GetBools(split.Groups[9].Value);

            NormalJTs = normalULs.Select((v, i) => new NormalJT(new Bounds(v, normalDRs[i]).Expand(true))).ToArray();

            
            var customJTs = new List<CustomJT>();

            for (int i = 0; i < sideULs.Length; i++)
                customJTs.Add(new CustomJT(new Bounds(sideULs[i], sideDRs[i]).Expand(true),
                    sidesToR[i] ? Facings.Right : Facings.Left, sidesPush[i]));

            for (int i = 0; i < upsDULs.Length; i++)
                customJTs.Add(new CustomJT(new Bounds(upsDULs[i], upsDDRs[i]).Expand(true),
                    Facings.Down, false/*upsDPush[i]*/));

            CustomJTs = customJTs.ToArray();
        }

        private static void GetWind(string init, string positions, string patterns, string widths, string heights)
        {
            var getInit = Regex.Match(init, @"(-?\d+\.?\d*), (-?\d+\.?\d*)");
            InitWind = new Vector2(
                float.Parse(getInit.Groups[1].Value, CultureInfo.InvariantCulture) / 10,
                float.Parse(getInit.Groups[2].Value, CultureInfo.InvariantCulture) / 10);

            IntVec2[] getPoses = Regex.Matches(positions, @"(-?\d+)\.?\d*, (-?\d+)")
                .Select(m => new IntVec2(int.Parse(m.Groups[1].Value), int.Parse(m.Groups[2].Value))).ToArray();
            string[] getPatterns = Regex.Matches(patterns, @" (\w+) ").Select(m => m.Groups[1].Value).ToArray();
            int[] getWidths = Regex.Matches(widths, @" (\d+)\.\d* ").Select(m => int.Parse(m.Groups[1].Value)).ToArray();
            int[] getHeights = Regex.Matches(heights, @" (\d+)\.\d* ").Select(m => int.Parse(m.Groups[1].Value)).ToArray();

            var listWT = new List<WindTrigger>();

            for (int i = 0; i < getPoses.Length; i++) {
                (bool vertical, float stren, bool valid) pattern = getPatterns[i] switch {
                    "Left" => (false, -40, true),
                    "LeftStrong" => (false, -80, true),
                    "Right" => (false, 40, true),
                    "RightStrong" => (false, 80, true),
                    "RightCrazy" => (false, 120, true),
                    "Up" => (true, -40, true),
                    "Down" => (true, 30, true),
                    "None" => (false, 0, true),
                    _ => (false, 0, false)
                };

                if (!pattern.valid) {
                    Console.WriteLine($"There was a wind trigger pattern \"{getPatterns[i]}\" that couldn't be processed.\n The algorithm will run without accounting for it after you press enter.");
                    Console.ReadLine();
                    continue;
                }

                listWT.Add(new WindTrigger(getPoses[i], new IntVec2(getWidths[i], getHeights[i]), pattern.vertical, pattern.stren));
            }

            WindTriggers = listWT.ToArray();

            FeatherSim.doWind = InitWind.X != 0 || InitWind.Y != 0 || WindTriggers.Length != 0;
        }

        private static void CreateDangerBitfield()
        {
            try {
                var hazards = Spinners.Select(s => new IntVec2(s.X / 8 * 8, s.Y / 8 * 8))
                    .Concat(Killboxes.Select(k => new IntVec2(k.bounds.L / 8 * 8, k.bounds.U / 8 * 8)))
                    .Concat(Killboxes.Select(k => new IntVec2(k.bounds.R / 8 * 8, k.bounds.D / 8 * 8)))
                    .Concat(Spikes.Select(s => new IntVec2(s.bounds.L / 8 * 8, s.bounds.U / 8 * 8)))
                    .Concat(Spikes.Select(s => new IntVec2(s.bounds.R / 8 * 8, s.bounds.D / 8 * 8)))
                    .ToArray();

                DeathMap = new DeathMapInfo() {
                    xMin = hazards.Min(s => s.X) - 24,
                    xMax = hazards.Max(s => s.X) + 24,
                    yMin = hazards.Min(s => s.Y) - 24,
                    yMax = hazards.Max(s => s.Y) + 24
                };

                DeathMap.widthInTiles = (DeathMap.xMax - DeathMap.xMin) / 8;
                DeathMap.heightInTiles = (DeathMap.yMax - DeathMap.yMin) / 8;

                HasHazards = true;
            }
            catch {
                HasHazards = false;
                return;
            }

            DeathMap.map =
                new BitArray[(DeathMap.xMax - DeathMap.xMin) / 8 + 1].Select(b =>
                new BitArray((DeathMap.yMax - DeathMap.yMin) / 8 + 1)).ToArray();

            for (int i = 0; i < Spinners.Length; i++)
                AddSingleSpinnerCollision(Spinners[i]);

            for (int i = 0; i < Killboxes.Length; i++)
                AddSingleKillboxCollision(Killboxes[i]);

            for (int i = 0; i < Spikes.Length; i++)
                AddSingleKillboxCollision(Spikes[i]);
        }
        private static void AddSingleSpinnerCollision(IntVec2 s)
        {
            var bitIndex = new IntVec2(
                (s.X - DeathMap.xMin) / 8,
                (s.Y - DeathMap.yMin) / 8);

            int xStart = Math.Max(bitIndex.X - 2, 0);
            int xEnd = Math.Min(bitIndex.X + 2, DeathMap.widthInTiles);
            int yStart = Math.Max(bitIndex.Y - 2, 0);
            int yEnd = Math.Min(bitIndex.Y + 2, DeathMap.heightInTiles);

            for (int y = yStart; y <= yEnd; y++) {
                int yCoord = DeathMap.yMin + y * 8 + 1;

                for (int x = xStart; x <= xEnd; x++) {
                    int xCoord = DeathMap.xMin + x * 8;

                    if (SingleSpinnerColl(xCoord, yCoord) ||
                        SingleSpinnerColl(xCoord + 7, yCoord) ||
                        SingleSpinnerColl(xCoord, yCoord + 7) ||
                        SingleSpinnerColl(xCoord + 7, yCoord + 7))

                        DeathMap.map[x][y] = true;

                    bool SingleSpinnerColl(int x, int y) =>
                        Math.Pow(x - s.X, 2) + Math.Pow(y - 4 - s.Y, 2) < 150d
                        && ((s.X - 7 < x && x < s.X + 7 && s.Y - 3 < y && y < s.Y + 15) || // tall
                            (s.X - 8 < x && x < s.X + 8 && s.Y - 2 < y && y < s.Y + 14) || // square
                            (s.X - 9 < x && x < s.X + 9 && s.Y - 1 < y && y < s.Y + 13) || // squished
                            (s.X - 11 < x && x < s.X + 11 && s.Y < y && y < s.Y + 10)); // horizontal bar
                }
            }
        }
        private static void AddSingleKillboxCollision(RectangleHitbox hb)
        {
            int xStart = Math.Max(0, (hb.bounds.L - DeathMap.xMin) / 8 - 2);
            int xEnd = Math.Min(DeathMap.widthInTiles, (hb.bounds.R - DeathMap.xMin) / 8 + 3);
            int yStart = Math.Max(0, (hb.bounds.U - DeathMap.yMin) / 8 - 2);
            int yEnd = Math.Min(DeathMap.heightInTiles, (hb.bounds.D - DeathMap.yMin) / 8 + 3);

            for (int x = xStart; x < xEnd; x++) {
                int xCoord = DeathMap.xMin + x * 8;
                for (int y = yStart; y < yEnd; y++) {
                    int yCoord = DeathMap.yMin + y * 8 + 1;
                    if (hb.TouchingAsFeather(new IntVec2(xCoord, yCoord)) ||
                        hb.TouchingAsFeather(new IntVec2(xCoord + 7, yCoord)) ||
                        hb.TouchingAsFeather(new IntVec2(xCoord, yCoord + 7)) ||
                        hb.TouchingAsFeather(new IntVec2(xCoord + 7, yCoord + 7)))
                        DeathMap.map[x][y] = true;
                }
            }
        }

        private static void GetSolidTiles(string src)
        {
            var parts = Regex.Match(src, @"{X:(-?\d+) Y:(-?\d+) Width:(\d+) Height:(\d+)}\sSolids: (.*)");
            Tiles = new SolidTileInfo() {
                x = int.Parse(parts.Groups[1].Value),
                y = int.Parse(parts.Groups[2].Value),
                width = int.Parse(parts.Groups[3].Value),
                height = int.Parse(parts.Groups[4].Value)
            };
            Tiles.rightBound = Tiles.x + Tiles.width;
            Tiles.lowestYIndex = Tiles.height / 8 - 1;

            int widthInTiles = Tiles.width / 8;

            string tileMap = Regex.Replace(parts.Groups[5].Value, @",\s", "");
            var rowMatches = Regex.Matches(tileMap, @"(?<= )[^ ]*");
            Tiles.map = rowMatches.Select(RowStrToBitArr).ToArray();

            int expectedRowCount = Tiles.lowestYIndex + 1;
            if (Tiles.map.Length < expectedRowCount) {
                var addedRows = Enumerable.Repeat(new BitArray(widthInTiles), expectedRowCount - Tiles.map.Length);
                Tiles.map = Tiles.map.Concat(addedRows).ToArray();
            }

            BitArray RowStrToBitArr(Match row)
            {
                var res = new BitArray(row.Value.Select(c => c != '0').ToArray());
                res.Length = widthInTiles;
                return res;
            }
        }

        private static void GetCustomHitboxes()
        {
            var kbs = new List<RectangleHitbox>();
            var colls = new List<RectangleHitbox>();

            var lineEmpty = new Regex(@"^\s*$");
            var parseLine = new Regex(@"^\s*(.?\d+),\s*(.?\d+),\s*(.?\d+),\s*(.?\d+)(?:\s*$|\s*([cC]))");

            for (int i = 0; i < srcSett.ManualHitboxes.Length; i++) {
                if (lineEmpty.IsMatch(srcSett.ManualHitboxes[i])) continue;

                var parse = parseLine.Match(srcSett.ManualHitboxes[i]);

                if (!parse.Success)
                    throw new ArgumentException($"Invalid hitbox definition on line {i + 1}");

                var rawBounds = new Bounds(int.Parse(parse.Groups[1].Value),
                                           int.Parse(parse.Groups[2].Value),
                                           int.Parse(parse.Groups[3].Value),
                                           int.Parse(parse.Groups[4].Value));

                if (parse.Groups[5].Success) {
                    colls.Add(new RectangleHitbox(rawBounds.Expand(true)));
                    continue;
                }

                kbs.Add(new RectangleHitbox(rawBounds.Expand(false)));
            }

            Colliders = Colliders.Concat(colls).ToArray();
            Killboxes = Killboxes.Concat(kbs).ToArray();
        }
        
        private static void GetCheckpoints()
        {
            var res = new List<Checkpoint>();
            var lineEmpty = new Regex(@"^\s*$");
            var parseLine = new Regex(@"^\s*(.?\d+),\s*(.?\d+),\s*(.?\d+),\s*(.?\d+)(?:\s*$|\s*([pP]))");

            for (int i = 0; i < srcSett.Checkpoints.Length; i++) {
                if (lineEmpty.IsMatch(srcSett.Checkpoints[i])) continue;

                var parse = parseLine.Match(srcSett.Checkpoints[i]);

                if (!parse.Success)
                    throw new ArgumentException($"Invalid checkpoint definition on line {i + 1}");

                var bounds = new Bounds(int.Parse(parse.Groups[1].Value),
                                        int.Parse(parse.Groups[2].Value),
                                        int.Parse(parse.Groups[3].Value),
                                        int.Parse(parse.Groups[4].Value));

                if (parse.Groups[5].Success) {
                    res.Add(new Checkpoint(bounds.Expand()));
                    continue;
                }

                res.Add(new Checkpoint(bounds.Expand(false)));
            }
            Checkpoints = res.ToArray();
        }
    }
}