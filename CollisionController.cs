using static Featherline.Level;

namespace Featherline;

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
        if (fs.pos.X < DeathMap.xMin || fs.pos.X > DeathMap.xMax || fs.pos.Y < DeathMap.yMin || fs.pos.Y > DeathMap.yMax)
            return false;
        return DeathMap.map[(fs.pos.X - DeathMap.xMin) / 8][(fs.pos.Y - DeathMap.yMin) / 8];
    }

    private void DistFilterHazards()
    {
        if (framesSinceDistFilter >= 5) {
            var dummyPos = new IntVec2(fs.pos + (wind.current * 5 * DeltaTime));

            distFiltSpinners = Spinners.Where(spn => (spn.X - dummyPos.X).Square() + (spn.Y - dummyPos.Y).Square() < 900).ToArray();
            distFiltKBs = Killboxes.Where(kbx => kbx.GetActualDistance(fs.pos) < 30).ToArray();
            distFiltSpikes = Spikes.Where(spk => spk.GetActualDistance(fs.pos) < 30).ToArray();

            framesSinceDistFilter = 0;
        }
    }

    private bool RawHazardCollision()
    {
        foreach (var s in distFiltSpinners) {
            var xDiff = fs.pos.X - s.X;
            var yDiff = fs.pos.Y - 5 - s.Y;
            if (xDiff * xDiff + yDiff * yDiff < 170d
            && ((s.X - 7 < fs.pos.X && fs.pos.X < s.X + 7 && s.Y - 3 < fs.pos.Y && fs.pos.Y < s.Y + 15) || // slightly tall
                (s.X - 8 < fs.pos.X && fs.pos.X < s.X + 8 && s.Y - 2 < fs.pos.Y && fs.pos.Y < s.Y + 14) || // square
                (s.X - 9 < fs.pos.X && fs.pos.X < s.X + 9 && s.Y - 1 < fs.pos.Y && fs.pos.Y < s.Y + 13) || // slightly squished
                (s.X - 11 < fs.pos.X && fs.pos.X < s.X + 11 && s.Y < fs.pos.Y && fs.pos.Y < s.Y + 10))) // sideways bar
                return true;
        }

        for (int i = 0; i < distFiltKBs.Length; i++)
            if (distFiltKBs[i].TouchingAsFeather(fs.pos))
                return true;

        for (int i = 0; i < distFiltSpikes.Length; i++)
            if (distFiltSpikes[i].Died(fs.pos, fs.spd))
                return true;
        
        return false;
    }

    public List<int> wallboops = new List<int>();

    private void UpdatePosition()
    {
        if (++framesSinceColliderFilter >= 5) {
            var dummyPos = new IntVec2(fs.pos + (wind.current * 5 * DeltaTime));

            distFiltColls = Colliders.Where(coll => coll.GetActualDistance(dummyPos) < 30).ToArray();
            distFiltNormalJTs = NormalJTs.Where(JT => JT.GetActualDistance(dummyPos) < 30).ToArray();
            distFiltCustomJTs = CustomJTs.Where(JT => JT.GetActualDistance(dummyPos) < 30).ToArray();

            framesSinceColliderFilter = 0;
        }

        foreach (var JT in distFiltNormalJTs)
            if (JT.Pulling(fs.pos, fs.spd)) {
                fs.moveCounter.Y -= 40 * DeltaTime;
                fs.MoveV();
                break;
            }

        int L, R, U, D;

        XMove();
        YMove();

        foreach (var JT in distFiltCustomJTs) {
            if (JT.Pulling(fs.pos, fs.spd)) {
                fs.moveCounter += JT.pullVector;
                fs.MoveH();
                fs.MoveV();
                continue;
            }
            if (JT.Booped(fs)) {
                stop = sett.AvoidWalls & fs.f > 10;
                wallboops.Add(fs.f);
                return;
            }
        }
        foreach (var JT in distFiltNormalJTs) {
            if (JT.Booped(fs)) {
                stop = sett.AvoidWalls & fs.f > 10;
                wallboops.Add(fs.f);
                return;
            }
        }

        void XMove()
        {
            fs.moveCounter.X += fs.spd.X * DeltaTime;
            fs.MoveH();

            // custom colliders (take priority over room border)
            for (int i = 0; i < distFiltColls.Length; i++) {
                if (distFiltColls[i].TouchingAsFeather(fs.pos)) {
                    stop = sett.AvoidWalls & fs.f > 10;
                    wallboops.Add(fs.f);
                    BounceX(fs.spd.X > 0
                        ? distFiltColls[i].bounds.L - 1
                        : distFiltColls[i].bounds.R);
                    return;
                }
            }

            // room border behavior
            if (fs.pos.X - 3 <= Tiles.x) {
                fs.pos.X += Tiles.x + 4 - fs.pos.X;
                fs.pos.X = Tiles.x + 4;
                fs.spd.X = 0f;
                UpdateLR();
                return;
            }
            else if (fs.pos.X + 4 > Tiles.rightBound) {
                fs.pos.X += Tiles.rightBound - 4 - fs.pos.X;
                fs.pos.X = Tiles.rightBound - 4;
                fs.spd.X = 0f;
                UpdateLR();
                return;
            }

            UpdateLR();
            UpdateUD();

            int x = fs.spd.X > 0 ? R : L;
            if (Tiles.map[U][x] | Tiles.map[D][x]) {
                stop = sett.AvoidWalls & fs.f > 10;
                wallboops.Add(fs.f);
                BounceX((fs.pos.X + (fs.spd.X > 0 ? -4 : 3) - Tiles.x) / 8 * 8 + 4 + Tiles.x);
            }
        }

        void YMove()
        {
            fs.moveCounter.Y += fs.spd.Y * DeltaTime;
            fs.MoveV();

            for (int i = 0; i < distFiltColls.Length; i++) {
                if (distFiltColls[i].TouchingAsFeather(fs.pos)) {
                    stop = sett.AvoidWalls & fs.f > 10;
                    wallboops.Add(fs.f);
                    BounceY(fs.spd.Y > 0
                        ? distFiltColls[i].bounds.U - 1
                        : distFiltColls[i].bounds.D);
                    return;
                }
            }

            UpdateUD();

            int y = fs.spd.Y > 0 ? D : U;
            if (Tiles.map[y][L] | Tiles.map[y][R]) {
                stop = sett.AvoidWalls & fs.f > 10;
                wallboops.Add(fs.f);
                BounceY((fs.pos.Y + (fs.spd.Y > 0 ? -2 : 4) - Tiles.y) / 8 * 8 + 2 + Tiles.y);
            }
        }

        void UpdateLR()
        {
            L = (fs.pos.X - 4 - Tiles.x) / 8;
            R = (fs.pos.X + 3 - Tiles.x) / 8;
        }
        void UpdateUD()
        {
            U = (fs.pos.Y - 10 - Tiles.y) / 8;
            D = (fs.pos.Y - 3 - Tiles.y) / 8;
            U = U > 0 ? U < Tiles.lowestYIndex ? U : Tiles.lowestYIndex : 0;
            D = D > 0 ? D < Tiles.lowestYIndex ? D : Tiles.lowestYIndex : 0;
        }
        void BounceX(int newX)
        {
            fs.pos.X = newX;
            fs.moveCounter.X = 0f;
            fs.spd.X *= -0.5f;
            UpdateLR();
        }
        void BounceY(int newY)
        {
            fs.pos.Y = newY;
            fs.moveCounter.Y = 0f;
            fs.spd.Y *= -0.5f;
        }
    }
}
