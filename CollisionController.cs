using System.Linq;
using System;
using static Featherline.Level;

namespace Featherline
{
    public partial class FeatherSim
    {
        public Func<bool> IsDead;

        public bool DeathCheck()
        {
            framesSinceDistFilter++;

            if (!CheckDangerMap())
                return false;

            DistFilterHazards();

            return RawCollision();
        }

        bool CheckDangerMap()
        {
            if (si.intPos.X < DeathMap.xMin || si.intPos.X > DeathMap.xMax || si.intPos.Y < DeathMap.yMin || si.intPos.Y > DeathMap.yMax)
                return false;
            return DeathMap.map[(si.intPos.X + 2 - DeathMap.xMin) / 8][(si.intPos.Y - DeathMap.yMin) / 8];
        }

        void DistFilterHazards()
        {
            if (framesSinceDistFilter >= 5) {
                var dummyPos = new IntVec2(si.position + (wind.current * 5 * DeltaTime));

                distFiltered.spinners = Spinners.Where(spn => Math.Pow(spn.X - dummyPos.X, 2) + Math.Pow(spn.Y - dummyPos.Y, 2) < 900).ToArray();
                distFiltered.killboxes = Killboxes.Where(kbx => kbx.GetActualDistance(si.intPos) < 30).ToArray();
                distFiltered.spikes = Spikes.Where(spk => spk.GetActualDistance(si.intPos) < 30).ToArray();

                framesSinceDistFilter = 0;
            }
        } 

        public bool RawCollision()
        {
            foreach (var s in distFiltered.spinners)
                if (Math.Pow(si.intPos.X - s.X, 2) + Math.Pow(si.intPos.Y - 4 - s.Y, 2) < 145
                && ((s.X - 7 < si.intPos.X && si.intPos.X < s.X + 7 && s.Y - 3 < si.intPos.Y && si.intPos.Y < s.Y + 15) || // slightly tall
                    (s.X - 8 < si.intPos.X && si.intPos.X < s.X + 8 && s.Y - 2 < si.intPos.Y && si.intPos.Y < s.Y + 14) || // square
                    (s.X - 9 < si.intPos.X && si.intPos.X < s.X + 9 && s.Y - 1 < si.intPos.Y && si.intPos.Y < s.Y + 13) || // slightly squished
                    (s.X - 11 < si.intPos.X && si.intPos.X < s.X + 11 && s.Y < si.intPos.Y && si.intPos.Y < s.Y + 10)))
                    return true;

            for (int i = 0; i < distFiltered.killboxes.Length; i++)
                if (distFiltered.killboxes[i].TouchingAsFeather(si.intPos))
                    return true;

            for (int i = 0; i < distFiltered.spikes.Length; i++)
                if (distFiltered.spikes[i].Died(si.intPos, si.speed))
                    return true;

            return false;
        }

        public bool RawSpinnerColl(int x, int y) => distFiltered.spinners.Any(s => Math.Pow(x - s.X, 2) + Math.Pow(y - 4 - s.Y, 2) < 145
            && ((s.X - 7 < x && x < s.X + 7 && s.Y - 3 < y && y < s.Y + 15) || // slightly tall
                (s.X - 8 < x && x < s.X + 8 && s.Y - 2 < y && y < s.Y + 14) || // square
                (s.X - 9 < x && x < s.X + 9 && s.Y - 1 < y && y < s.Y + 13) || // slightly squished
                (s.X -11 < x && x < s.X +11 && s.Y     < y && y < s.Y + 10))); // sideways bar

        private void UpdatePosition()
        {
            si.position.X += si.speed.X * DeltaTime;

            si.position.X =
                si.position.X < Tiles.x + 3.5f
                ? Tiles.x + 4
                : si.position.X >= Tiles.rightBound - 3.5f
                    ? Tiles.rightBound - 4
                    : si.position.X;

            si.intPos.X = (int)Math.Round(si.position.X);

            int L = (si.intPos.X - Tiles.x - 4) / 8;
            int R = (si.intPos.X - Tiles.x + 3) / 8;

            int U = (si.intPos.Y - Tiles.y - 10) / 8;
            int D = (si.intPos.Y - Tiles.y - 3) / 8;
            U = U > 0 ? U : 0;
            D = D > 0 ? D : 0;
            U = U < Tiles.lowestYIndex ? U : Tiles.lowestYIndex;
            D = D < Tiles.lowestYIndex ? D : Tiles.lowestYIndex;

            int x = si.speed.X > 0 ? R : L;
            bool c1 = Tiles.map[U][x];
            bool c2 = Tiles.map[D][x];

            foreach (var coll in Colliders) {
                if (coll.TouchingAsFeather(si.intPos)) {
                    si.position.X = si.speed.X > 4 ? coll.L - 1 : coll.R + 1;
                    si.intPos.X = (int)Math.Round(si.position.X);
                    BounceX();
                    break;
                }
            }
            if (c1 || c2) {
                si.position.X = (si.intPos.X - Tiles.x + (si.speed.X > 0 ? -4 : 3)) / 8 * 8 + 4 + Tiles.x;
                si.intPos.X = (int)Math.Round(si.position.X);
                L = (si.intPos.X - Tiles.x - 4) / 8;
                R = (si.intPos.X - Tiles.x + 3) / 8;
                BounceX();
            }

            si.position.Y += si.speed.Y * DeltaTime;
            si.intPos.Y = (int)Math.Round(si.position.Y);

            U = (si.intPos.Y - Tiles.y - 10) / 8;
            D = (si.intPos.Y - Tiles.y - 3) / 8;
            U = U > 0 ? U : 0;
            D = D > 0 ? D : 0;
            U = U < Tiles.lowestYIndex ? U : Tiles.lowestYIndex;
            D = D < Tiles.lowestYIndex ? D : Tiles.lowestYIndex;

            int y = si.speed.Y > 0 ? D : U;
            c1 = Tiles.map[y][L];
            c2 = Tiles.map[y][R];

            foreach (var coll in Colliders) {
                if (coll.TouchingAsFeather(si.intPos)) {
                    si.position.X = si.speed.X > 4 ? coll.L - 1 : coll.R + 1;
                    si.intPos.X = (int)Math.Round(si.position.X);
                    BounceX();
                    break;
                }
            }
            if (c1 || c2 || Colliders.Any(coll => coll.TouchingAsFeather(si.intPos))) {
                si.position.Y = (si.intPos.Y - Tiles.y + (si.speed.Y > 0 ? 4 : -0)) / 8 * 8 + 2 + Tiles.y;
                si.intPos.Y = (int)Math.Round(si.position.Y);
                BounceY();
            }

            void BounceX()
            {
                si.speed.X *= -0.5f;
                fitnessOffset += sett.AvoidWalls ? -6000 : 0;
            }
            void BounceY()
            {
                si.speed.Y *= -0.5f;
                fitnessOffset += sett.AvoidWalls ? -6000 : 0;
            }
        }
    }
}