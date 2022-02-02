using System.Linq;
using System;
using static Featherline.Level;
using System.Collections.Generic;

namespace Featherline
{
    public partial class FeatherSim
    {
        public Func<bool> IsDead;

        private int framesSinceDistFilter = 999;
        private IntVec2[] distFiltSpinners;
        private RectangleHitbox[] distFiltKBs;
        private Spike[] distFiltSpikes;

        private int framesSinceColliderFilter = 999;
        private Collider[] distFiltColls;

        private bool DeathCheck()
        {
            framesSinceDistFilter++;

            if (!CheckDangerMap())
                return false;

            DistFilterHazards();

            return RawHazardCollision();
        }

        private bool CheckDangerMap()
        {
            if (si.intPos.X < DeathMap.xMin || si.intPos.X > DeathMap.xMax || si.intPos.Y < DeathMap.yMin || si.intPos.Y > DeathMap.yMax)
                return false;
            return DeathMap.map[(si.intPos.X + 2 - DeathMap.xMin) / 8][(si.intPos.Y - DeathMap.yMin) / 8];
        }

        private void DistFilterHazards()
        {
            if (framesSinceDistFilter >= 5) {
                var dummyPos = new IntVec2(si.pos + (wind.current * 5 * DeltaTime));

                distFiltSpinners = Spinners.Where(spn => Math.Pow(spn.X - dummyPos.X, 2) + Math.Pow(spn.Y - dummyPos.Y, 2) < 900).ToArray();
                distFiltKBs = Killboxes.Where(kbx => kbx.GetActualDistance(si.intPos) < 30).ToArray();
                distFiltSpikes = Spikes.Where(spk => spk.GetActualDistance(si.intPos) < 30).ToArray();

                framesSinceDistFilter = 0;
            }
        }

        private bool RawHazardCollision()
        {
            foreach (var s in distFiltSpinners)
                if (Math.Pow(si.intPos.X - s.X, 2) + Math.Pow(si.intPos.Y - 4 - s.Y, 2) < 145
                && ((s.X - 7 < si.intPos.X && si.intPos.X < s.X + 7 && s.Y - 3 < si.intPos.Y && si.intPos.Y < s.Y + 15) || // slightly tall
                    (s.X - 8 < si.intPos.X && si.intPos.X < s.X + 8 && s.Y - 2 < si.intPos.Y && si.intPos.Y < s.Y + 14) || // square
                    (s.X - 9 < si.intPos.X && si.intPos.X < s.X + 9 && s.Y - 1 < si.intPos.Y && si.intPos.Y < s.Y + 13) || // slightly squished
                    (s.X - 11 < si.intPos.X && si.intPos.X < s.X + 11 && s.Y < si.intPos.Y && si.intPos.Y < s.Y + 10))) // sideways bar
                    return true;

            for (int i = 0; i < distFiltKBs.Length; i++)
                if (distFiltKBs[i].TouchingAsFeather(si.intPos))
                    return true;

            for (int i = 0; i < distFiltSpikes.Length; i++)
                if (distFiltSpikes[i].Died(si.intPos, si.spd))
                    return true;

            return false;
        }

        private bool RawSpinnerColl(int x, int y) => distFiltSpinners.Any(s => Math.Pow(x - s.X, 2) + Math.Pow(y - 4 - s.Y, 2) < 145
            && ((s.X - 7 < x && x < s.X + 7 && s.Y - 3 < y && y < s.Y + 15) || // slightly tall
                (s.X - 8 < x && x < s.X + 8 && s.Y - 2 < y && y < s.Y + 14) || // square
                (s.X - 9 < x && x < s.X + 9 && s.Y - 1 < y && y < s.Y + 13) || // slightly squished
                (s.X - 11 < x && x < s.X + 11 && s.Y < y && y < s.Y + 10))); // sideways bar

        public List<int> wallboops = new List<int>();

        private void UpdatePosition()
        {
            if (++framesSinceColliderFilter >= 5) {
                distFiltColls = Colliders.Where(coll => coll.GetActualDistance(si.intPos) < 30).ToArray();
                framesSinceColliderFilter = 0;
            }

            int L, R, U, D;

            XMove();
            YMove();

            void XMove()
            {
                //if (si.f == 108) {
                //}

                si.pos.X += si.spd.X * DeltaTime;
                si.intPos.X = (int)Math.Round(si.pos.X);

                // custom colliders (take priority over room border)
                for (int i = 0; i < distFiltColls.Length; i++) {
                    if (distFiltColls[i].TouchingAsFeather(si.intPos)) {
                        stop = sett.AvoidWalls;
                        wallboops.Add(si.f);
                        BounceX(si.spd.X > 0
                            ? distFiltColls[i].L - 1
                            : distFiltColls[i].R + 1);
                        return;
                    }
                }

                // room border behavior
                if (si.intPos.X - 3 <= Tiles.x) {
                    si.pos.X += Tiles.x + 4 - si.intPos.X;
                    si.intPos.X = Tiles.x + 4;
                    si.spd.X = 0f;
                    UpdateLR();
                    return;
                }
                else if (si.intPos.X + 4 > Tiles.rightBound) {
                    si.pos.X += Tiles.rightBound - 4 - si.intPos.X;
                    si.intPos.X = Tiles.rightBound - 4;
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
                    BounceX((si.intPos.X + (si.spd.X > 0 ? -4 : 3) - Tiles.x) / 8 * 8 + 4 + Tiles.x);
                }
            }

            void YMove()
            {
                si.pos.Y += si.spd.Y * DeltaTime;
                si.intPos.Y = (int)Math.Round(si.pos.Y);

                for (int i = 0; i < distFiltColls.Length; i++) {
                    if (distFiltColls[i].TouchingAsFeather(si.intPos)) {
                        stop = sett.AvoidWalls;
                        wallboops.Add(si.f);
                        BounceY(si.spd.Y > 0
                            ? distFiltColls[i].U - 1
                            : distFiltColls[i].D + 1);
                        return;
                    }
                }

                UpdateUD();

                int y = si.spd.Y > 0 ? D : U;
                if (Tiles.map[y][L] | Tiles.map[y][R]) {
                    stop = sett.AvoidWalls;
                    wallboops.Add(si.f);
                    BounceY((si.intPos.Y + (si.spd.Y > 0 ? -2 : 4) - Tiles.y) / 8 * 8 + 2 + Tiles.y);
                }
            }

            void UpdateLR()
            {
                L = (si.intPos.X - 4 - Tiles.x) / 8;
                R = (si.intPos.X + 3 - Tiles.x) / 8;
            }
            void UpdateUD()
            {
                U = (si.intPos.Y - 10 - Tiles.y) / 8;
                D = (si.intPos.Y - 3 - Tiles.y) / 8;
                U = U > 0 ? U < Tiles.lowestYIndex ? U : Tiles.lowestYIndex : 0;
                D = D > 0 ? D < Tiles.lowestYIndex ? D : Tiles.lowestYIndex : 0;
            }
            void BounceX(int newX)
            {
                si.pos.X = newX;
                si.intPos.X = newX;
                si.spd.X *= -0.5f;
                UpdateLR();
            }
            void BounceY(int newY)
            {
                si.pos.Y = newY;
                si.intPos.Y = newY;
                si.spd.Y *= -0.5f;
            }
        }
    }
}