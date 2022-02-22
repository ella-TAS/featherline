using System.Linq;
using System;
using static Featherline.Level;
using System.Collections.Generic;

namespace Featherline
{
    public partial class FeatherSim
    {
        public Action CheckDeath;

        private int framesSinceDistFilter = 999;
        private IntVec2[] distFiltSpinners;
        private RectangleHitbox[] distFiltKBs;
        private Spike[] distFiltSpikes;

        private int framesSinceColliderFilter = 999;
        private RectangleHitbox[] distFiltColls;

        private NormalJT[] distFiltNormalJTs;
        private CustomJT[] distFiltCustomJTs;

        private void DeathCheck()
        {
            framesSinceDistFilter++;

            if (!CheckDangerMap()) return;

            DistFilterHazards();

            if (RawHazardCollision())
                stop = true;
        }

        private bool CheckDangerMap()
        {
            if (si.pos.X < DeathMap.xMin || si.pos.X > DeathMap.xMax || si.pos.Y < DeathMap.yMin || si.pos.Y > DeathMap.yMax)
                return false;
            return DeathMap.map[(si.pos.X - DeathMap.xMin) / 8][(si.pos.Y - DeathMap.yMin) / 8];
        }

        private void DistFilterHazards()
        {
            if (framesSinceDistFilter >= 5) {
                var dummyPos = new IntVec2(si.pos + (wind.current * 5 * DeltaTime));

                distFiltSpinners = Spinners.Where(spn => Math.Pow(spn.X - dummyPos.X, 2) + Math.Pow(spn.Y - dummyPos.Y, 2) < 900).ToArray();
                distFiltKBs = Killboxes.Where(kbx => kbx.GetActualDistance(si.pos) < 30).ToArray();
                distFiltSpikes = Spikes.Where(spk => spk.GetActualDistance(si.pos) < 30).ToArray();

                framesSinceDistFilter = 0;
            }
        }

        private bool RawHazardCollision()
        {
            foreach (var s in distFiltSpinners) {
                var xDiff = si.pos.X - s.X;
                var yDiff = si.pos.Y - 5 - s.Y;
                if (xDiff * xDiff + yDiff * yDiff < 170d
                && ((s.X - 7 < si.pos.X && si.pos.X < s.X + 7 && s.Y - 3 < si.pos.Y && si.pos.Y < s.Y + 15) || // slightly tall
                    (s.X - 8 < si.pos.X && si.pos.X < s.X + 8 && s.Y - 2 < si.pos.Y && si.pos.Y < s.Y + 14) || // square
                    (s.X - 9 < si.pos.X && si.pos.X < s.X + 9 && s.Y - 1 < si.pos.Y && si.pos.Y < s.Y + 13) || // slightly squished
                    (s.X - 11 < si.pos.X && si.pos.X < s.X + 11 && s.Y < si.pos.Y && si.pos.Y < s.Y + 10))) // sideways bar
                    return true;
            }

            for (int i = 0; i < distFiltKBs.Length; i++)
                if (distFiltKBs[i].TouchingAsFeather(si.pos))
                    return true;

            for (int i = 0; i < distFiltSpikes.Length; i++)
                if (distFiltSpikes[i].Died(si.pos, si.spd))
                    return true;

            return false;
        }

        /*private bool RawSpinnerColl(int x, int y) => distFiltSpinners.Any(s => Math.Pow(x - s.X, 2) + Math.Pow(y - 4 - s.Y, 2) < 145
            && ((s.X - 7 < x && x < s.X + 7 && s.Y - 3 < y && y < s.Y + 15) || // slightly tall
                (s.X - 8 < x && x < s.X + 8 && s.Y - 2 < y && y < s.Y + 14) || // square
                (s.X - 9 < x && x < s.X + 9 && s.Y - 1 < y && y < s.Y + 13) || // slightly squished
                (s.X - 11 < x && x < s.X + 11 && s.Y < y && y < s.Y + 10))); // sideways bar*/

        public List<int> wallboops = new List<int>();

        private void UpdatePosition()
        {
            if (++framesSinceColliderFilter >= 5) {
                var dummyPos = new IntVec2(si.pos + (wind.current * 5 * DeltaTime));

                distFiltColls = Colliders.Where(coll => coll.GetActualDistance(dummyPos) < 30).ToArray();
                distFiltNormalJTs = NormalJTs.Where(JT => JT.GetActualDistance(dummyPos) < 30).ToArray();
                distFiltCustomJTs = CustomJTs.Where(JT => JT.GetActualDistance(dummyPos) < 30).ToArray();

                framesSinceColliderFilter = 0;
            }

            foreach (var JT in distFiltNormalJTs)
                if (JT.Pulling(si.pos, si.spd)) {
                    si.moveCounter.Y -= 40 * DeltaTime;
                    si.MoveV();
                    break;
                }

            int L, R, U, D;

            XMove();
            YMove();

            foreach (var JT in distFiltCustomJTs) {
                if (JT.Pulling(si.pos, si.spd)) {
                    si.moveCounter += JT.pullVector;
                    si.MoveH();
                    si.MoveV();
                    continue;
                }
                if (JT.Booped(si)) {
                    wallboops.Add(si.f);
                    return;
                }
            }
            foreach (var JT in distFiltNormalJTs) {
                wallboops.Add(si.f);
                return;
            }

            void XMove()
            {
                si.moveCounter.X += si.spd.X * DeltaTime;
                si.MoveH();

                // custom colliders (take priority over room border)
                for (int i = 0; i < distFiltColls.Length; i++) {
                    if (distFiltColls[i].TouchingAsFeather(si.pos)) {
                        stop = sett.AvoidWalls;
                        wallboops.Add(si.f);
                        BounceX(si.spd.X > 0
                            ? distFiltColls[i].bounds.L - 1
                            : distFiltColls[i].bounds.R);
                        return;
                    }
                }

                // room border behavior
                if (si.pos.X - 3 <= Tiles.x) {
                    si.pos.X += Tiles.x + 4 - si.pos.X;
                    si.pos.X = Tiles.x + 4;
                    si.spd.X = 0f;
                    UpdateLR();
                    return;
                }
                else if (si.pos.X + 4 > Tiles.rightBound) {
                    si.pos.X += Tiles.rightBound - 4 - si.pos.X;
                    si.pos.X = Tiles.rightBound - 4;
                    si.spd.X = 0f;
                    UpdateLR();
                    return;
                }

                UpdateLR();
                UpdateUD();

                int x = si.spd.X > 0 ? R : L;
                if (Tiles.map[U][x] | Tiles.map[D][x]) {
                    stop = sett.AvoidWalls;
                    wallboops.Add(si.f);
                    BounceX((si.pos.X + (si.spd.X > 0 ? -4 : 3) - Tiles.x) / 8 * 8 + 4 + Tiles.x);
                }
            }

            void YMove()
            {
                si.moveCounter.Y += si.spd.Y * DeltaTime;
                si.MoveV();

                for (int i = 0; i < distFiltColls.Length; i++) {
                    if (distFiltColls[i].TouchingAsFeather(si.pos)) {
                        stop = sett.AvoidWalls;
                        wallboops.Add(si.f);
                        BounceY(si.spd.Y > 0
                            ? distFiltColls[i].bounds.U - 1
                            : distFiltColls[i].bounds.D);
                        return;
                    }
                }

                UpdateUD();

                int y = si.spd.Y > 0 ? D : U;
                if (Tiles.map[y][L] | Tiles.map[y][R]) {
                    stop = sett.AvoidWalls;
                    wallboops.Add(si.f);
                    BounceY((si.pos.Y + (si.spd.Y > 0 ? -2 : 4) - Tiles.y) / 8 * 8 + 2 + Tiles.y);
                }
            }

            void UpdateLR()
            {
                L = (si.pos.X - 4 - Tiles.x) / 8;
                R = (si.pos.X + 3 - Tiles.x) / 8;
            }
            void UpdateUD()
            {
                U = (si.pos.Y - 10 - Tiles.y) / 8;
                D = (si.pos.Y - 3 - Tiles.y) / 8;
                U = U > 0 ? U < Tiles.lowestYIndex ? U : Tiles.lowestYIndex : 0;
                D = D > 0 ? D < Tiles.lowestYIndex ? D : Tiles.lowestYIndex : 0;
            }
            void BounceX(int newX)
            {
                si.pos.X = newX;
                si.moveCounter.X = 0f;
                si.spd.X *= -0.5f;
                UpdateLR();
            }
            void BounceY(int newY)
            {
                si.pos.Y = newY;
                si.moveCounter.Y = 0f;
                si.spd.Y *= -0.5f;
            }
        }
    }
}