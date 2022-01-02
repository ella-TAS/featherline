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
        public static RectangleHitbox[] Killboxes = new RectangleHitbox[0];
        public static Spike[] Spikes;

        public static SolidTileInfo Tiles;

        public static WindTrigger[] windTriggers;
        public static Vector2 initWind;

        public static IntVec2[] Feathers;
        public static IntVec2[] Switches;

        public static bool HasHazards;

        public static Collider[] Colliders = new Collider[0];

        static Settings srcSett;
        public static void Prepare(Settings sourceSettings)
        {
            srcSett = sourceSettings;
            string src = File.ReadAllText(srcSett.InfoFile);
            var split = Regex.Match(src, @"(-?\d+\.?\d*, -?\d+\.?\d*\t-?\d+\.?\d*, -?\d+\.?\d*\t\w*\t\w*)(.*)LightningUL:(.*)LightningDR:(.*)SpikeUL:(.*)SpikeDR:(.*)SpikeDir:(.*)Feathers:(.*)TouchSwitches:(.*)Wind:(.*)WTPos:(.*)WTPattern:(.*)WTWidth:(.*)WTHeight(.*)Bounds:(.*)");

            GetPosAndSpd(split.Groups[1].Value);
            GetSpinners(split.Groups[2].Value);
            GetLightning(split.Groups[3].Value, split.Groups[4].Value);
            GetSpikes(split.Groups[5].Value, split.Groups[6].Value, split.Groups[7].Value);
            GetFeathersAndSwitches(split.Groups[8].Value, split.Groups[9].Value);
            GetWind(split.Groups[10].Value, split.Groups[11].Value, split.Groups[12].Value, split.Groups[13].Value, split.Groups[14].Value);
            GetSolidTiles(split.Groups[15].Value);
            GetCustomHitboxes();
            CreateDangerBitMap();
        }

        private static void GetPosAndSpd(string src)
        {
            Match m = Regex.Match(src, @"(-?\d+\.\d+), (-?\d+\.\d+)\t(-?\d+\.\d+), (-?\d+\.\d+)");
            srcSett.StartX = float.Parse(m.Groups[1].Value);
            srcSett.StartY = float.Parse(m.Groups[2].Value);
            srcSett.BoostX = float.Parse(m.Groups[3].Value);
            srcSett.BoostY = float.Parse(m.Groups[4].Value);
            srcSett.DefineStartBoost = srcSett.BoostX != 0 || srcSett.BoostY != 0;
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
            
            Killboxes = Killboxes.Concat(getUL.Select((m, i) => new RectangleHitbox(
                    int.Parse(m.Groups[1].Value),
                    int.Parse(m.Groups[2].Value),
                    int.Parse(getDR[i].Groups[1].Value),
                    int.Parse(getDR[i].Groups[2].Value),
                    true
                ))).ToArray();
        }

        private static void GetSpikes(string UL, string DR, string dir)
        {
            var getUL = getIntPair.Matches(UL);
            var getDR = getIntPair.Matches(DR);
            var getDir = Regex.Matches(dir, @"Left|Right|Up|Down");

            Spikes = getUL.Select((m, i) => new Spike(
                    int.Parse(m.Groups[1].Value),
                    int.Parse(m.Groups[2].Value),
                    int.Parse(getDR[i].Groups[1].Value),
                    int.Parse(getDR[i].Groups[2].Value),
                    getDir[i].Value
                )).ToArray();
        }

        private static void GetWind(string init, string positions, string patterns, string widths, string heights)
        {
            var getInit = Regex.Match(init, @"(-?\d+\.?\d*), (-?\d+\.?\d*)");
            initWind = new Vector2(
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
                    "Left"       => (false, -40, true),
                    "LeftStrong" => (false, -80, true),
                    "Right"      => (false, 40, true),
                    "RightStrong"=> (false, 80, true),
                    "RightCrazy" => (false, 120, true),
                    "Up"         => (true, -40, true),
                    "Down"       => (true, 30, true),
                    "None"       => (false, 0, true),
                    _            => (false, 0, false)
                };

                if (!pattern.valid) {
                    Console.WriteLine($"There was a wind trigger pattern \"{getPatterns[i]}\" that couldn't be processed.\n The algorithm will run without accounting for it after you press enter.");
                    Console.ReadLine();
                    continue;
                }

                listWT.Add(new WindTrigger(getPoses[i], new IntVec2(getWidths[i], getHeights[i]), pattern.vertical, pattern.stren));
            }

            windTriggers = listWT.ToArray();

            FeatherSim.doWind = initWind.X != 0 || initWind.Y != 0 || windTriggers.Length != 0;
        }

        private static void GetFeathersAndSwitches(string feathers, string switches)
        {
            Feathers = getIntPair.Matches(feathers).Select(MatchToIntVec2).Distinct().ToArray();
            Switches = getIntPair.Matches(switches).Select(MatchToIntVec2).Distinct().ToArray();
        }

        private static void CreateDangerBitMap()
        {
            try {
                var hazards = Spinners.Select(s => new IntVec2(s.X / 8 * 8, s.Y / 8 * 8))
                    .Concat(Killboxes.Select(k => new IntVec2(k.L / 8 * 8, k.U / 8 * 8)))
                    .Concat(Killboxes.Select(k => new IntVec2(k.R / 8 * 8, k.D / 8 * 8)))
                    .Concat(Spikes.Select(s => new IntVec2(s.L / 8 * 8, s.U / 8 * 8)))
                    .Concat(Spikes.Select(s => new IntVec2(s.R / 8 * 8, s.D / 8 * 8)))
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
                        Math.Pow(x - s.X, 2) + Math.Pow(y - 4 - s.Y, 2) < 145
                        && ((s.X - 7 < x && x < s.X + 7 && s.Y - 3 < y && y < s.Y + 15) || // tall
                            (s.X - 8 < x && x < s.X + 8 && s.Y - 2 < y && y < s.Y + 14) || // square
                            (s.X - 9 < x && x < s.X + 9 && s.Y - 1 < y && y < s.Y + 13) || // squished
                            (s.X - 11 < x && x < s.X + 11 && s.Y < y && y < s.Y + 10)); // horizontal bar
                }
            }
        }
        private static void AddSingleKillboxCollision(RectangleHitbox hb)
        {
            int xStart = Math.Max(0, (hb.L - DeathMap.xMin) / 8 - 2);
            int xEnd = Math.Min(DeathMap.widthInTiles, (hb.R - DeathMap.xMin) / 8 + 3);
            int yStart = Math.Max(0, (hb.U - DeathMap.yMin) / 8 - 2);
            int yEnd = Math.Min(DeathMap.heightInTiles, (hb.D - DeathMap.yMin) / 8 + 3);

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
            var parts = Regex.Match(src, @"{X:(-?\d+) Y:(-?\d+) Width:(\d+) Height:(\d+)}\sSolids:(.*)");
            Tiles = new SolidTileInfo() {
                x = int.Parse(parts.Groups[1].Value),
                y = int.Parse(parts.Groups[2].Value),
                width = int.Parse(parts.Groups[3].Value),
                height = int.Parse(parts.Groups[4].Value)
            };
            Tiles.rightBound = Tiles.x + Tiles.width;
            Tiles.lowestYIndex = Tiles.height / 8 - 1;

            int widthInTiles = Tiles.width / 8;

            var rowMatches = Regex.Matches(parts.Groups[5].Value, @"(?<= )[^ ]*");
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

        private static Regex getIntPair = new Regex(@"(-?\d+)\.?\d*, (-?\d+)\.?\d*");
        private static IntVec2 MatchToIntVec2(Match src) => new IntVec2(int.Parse(src.Groups[1].Value), int.Parse(src.Groups[2].Value));

        private static void GetCustomHitboxes()
        {
            var kbs = new List<RectangleHitbox>();
            var colls = new List<Collider>();

            foreach (var hb in srcSett.manualHitboxes) {
                if (ValidCells(hb[..4])) {
                    int[] b; // b for bounds
                    try {
                        b = new int[] {
                            ProcessInput(hb[0]),
                            ProcessInput(hb[1]),
                            ProcessInput(hb[2]),
                            ProcessInput(hb[3])
                        };
                    } catch { continue; }

                    if (ValidCells(hb[4]) && "ty".Contains(char.ToLower(hb[4].TrimStart()[0])))
                        colls.Add(new Collider(     b[0],b[1],b[2],b[3],false));
                    else
                        kbs.Add(new RectangleHitbox(b[0],b[1],b[2],b[3],false));
                }
            }

            Colliders = Colliders.Concat(colls).ToArray();
            Killboxes = Killboxes.Concat(kbs  ).ToArray();
        }

        private static bool ValidCells(params string[] vals) => vals.All(val => val != null && val.Trim() != "");
        public static int ProcessInput(string src) => (int)Math.Round(float.Parse(src.Trim().Replace(',', '.')));
    }
}