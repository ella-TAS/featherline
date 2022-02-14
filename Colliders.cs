using System;
using static System.Math;

namespace Featherline
{
    public readonly struct Bounds
    {
        public readonly int L, U, R, D;
        public readonly float Lf, Uf, Rf, Df;

        public Bounds(int l, int u, int r, int d)
        {
            (L, U, R, D) = (l, u, r, d);
            Lf = l - 0.5f;
            Uf = u - 0.5f;
            Rf = r - 0.5f;
            Df = d - 0.5f;
        }
        public Bounds(IntVec2 UL, IntVec2 DR) : this(UL.X, UL.Y, DR.X, DR.Y) { }

        public Bounds Expand(/*bool featherHitbox, */bool collider)
        {
            int L = this.L - 2;
            int U = this.U + 4;
            int R = this.R + 3;
            int D = this.D + 9;

            /*if (!featherHitbox) {
                if (collider) {
                    U -= 3; D++;
                }
                else {
                    L--; R++; U--; D += 2;
                }
            }
            else */if (collider) {
                L--; U--; R++; D++;
            }

            return new Bounds(L, U, R, D);
        }

        public Bounds Expand() => new Bounds(L, U, R + 1, D + 1);
            
        public override string ToString() => $"L:{L}, U:{U}, R:{R}, D:{D}";
    }

    public class RectangleHitbox
    {
        public Bounds bounds;

        public bool TouchingAsFeather(IntVec2 pos) =>
            bounds.L <= pos.X && pos.X < bounds.R &&
            bounds.U <= pos.Y && pos.Y < bounds.D;

        public float GetActualDistance(IntVec2 pos)
        {
            int xDiff = (bounds.L <= pos.X && pos.X <= bounds.R) ? 0 : Min(Abs(pos.X - bounds.L), Abs(pos.X - bounds.R));
            int yDiff = (bounds.U <= pos.Y && pos.Y <= bounds.D) ? 0 : Min(Abs(pos.Y - bounds.U), Abs(pos.Y - bounds.D));
            return (float)Sqrt(xDiff * xDiff + yDiff * yDiff);
        }

        public RectangleHitbox(Bounds b) => bounds = b;
        public RectangleHitbox() { }

        public override string ToString() => bounds.ToString();
    }

    public class Checkpoint : RectangleHitbox
    {
        public Vector2 center;

        public Checkpoint(Bounds b) : base(b)
        {
            center = new Vector2((b.L + b.R) / 2f, (b.U + b.D) / 2f);
        }
        public Checkpoint() : base() { }

        public double GetFinalCPDistance(Vector2 pos, Vector2 previousPos)
        {
            var rawL = bounds.Lf - pos.X;
            var rawR = pos.X - bounds.Rf;
            var rawU = bounds.Uf - pos.Y;
            var rawD = pos.Y - bounds.Df;

            if (rawL < 0 & rawR < 0 & rawU < 0 & rawD < 0) {
                rawL = bounds.Lf - previousPos.X;
                rawR = previousPos.X - bounds.Rf;
                rawU = bounds.Uf - previousPos.Y;
                rawD = previousPos.Y - bounds.Df;
            }

            double xDiff = rawL > 0 ? rawL : rawR > 0 ? rawR : 0;
            double yDiff = rawU > 0 ? rawU : rawD > 0 ? rawD : 0;
            return Sqrt(xDiff * xDiff + yDiff * yDiff);
        }
    }

    public class Spike : RectangleHitbox
    {
        public Func<Vector2, bool> moveDirAllowsDeath;

        public Spike(Bounds b, string dir) : base(b)
        {
            moveDirAllowsDeath =
            dir switch {
                "Left" => spd => spd.X >= 0f,
                "Right" => spd => spd.X <= 0f,
                "Up" => spd => spd.Y >= 0f,
                "Down" => spd => spd.Y <= 0,
                _ => spd => false
            };
        }

        public bool Died(IntVec2 pos, Vector2 spd)
            => TouchingAsFeather(pos) && moveDirAllowsDeath(spd);
    }

    #region JumpThrus

    public class CustomJT : RectangleHitbox
    {
        protected Func<Vector2, bool> IsBeyond;
        protected bool horizontal;

        public Func<IntVec2, Vector2, bool> Pulling;
        public Vector2 pullVector;

        public int axisFix;

        public bool Booped(FeatherState fs)
        {
            if (IsBeyond(fs.pos))
                return false;

            var oldPos = fs.pos - (fs.spd * FeatherSim.DeltaTime);
            if (!IsBeyond(oldPos))
                return false;

            bool booped = horizontal
                ? bounds.Lf < fs.pos.X & fs.pos.X < bounds.Rf
                : bounds.Uf < oldPos.Y & oldPos.Y < bounds.Df;

            if (booped) {
                if (horizontal) {
                    fs.pos.Y = axisFix;
                    fs.intPos.Y = axisFix;
                    fs.spd.Y /= -2;
                }
                else {
                    fs.pos.X = axisFix;
                    fs.intPos.X = axisFix;
                    fs.spd.X /= -2;
                }
            }

            return booped;
        }

        public CustomJT (Bounds bounds, Facings orientation, bool pulls) : base(bounds)
        {
            switch (orientation) {
                case Facings.Left:
                    IsBeyond = pos => pos.X < bounds.Lf;
                    horizontal = false;
                    Pulling = pulls ? (Func<IntVec2, Vector2, bool>)
                        ((pos, spd) => spd.X <= 0f & TouchingAsFeather(pos))
                        : (pos, spd) => false;
                    pullVector = new Vector2(-40f, 0f) * FeatherSim.DeltaTime;
                    axisFix = bounds.L - 1;
                    break;
                case Facings.Right:
                    IsBeyond = pos => pos.X > bounds.Rf;
                    horizontal = false;
                    Pulling = pulls ? (Func<IntVec2, Vector2, bool>)
                            ((pos, spd) => spd.X >= 0f & TouchingAsFeather(pos))
                        : (pos, spd) => false;
                    pullVector = new Vector2(40f, 0f) * FeatherSim.DeltaTime;
                    axisFix = bounds.R;
                    break;
                case Facings.Down:
                    IsBeyond = pos => pos.Y > bounds.Df;
                    horizontal = true;
                    Pulling = pulls ? (Func<IntVec2, Vector2, bool>)
                            ((pos, spd) => spd.Y >= 0f & TouchingAsFeather(pos))
                        : (pos, spd) => false;
                    pullVector = new Vector2(0f, 40f) * FeatherSim.DeltaTime;
                    axisFix = bounds.D;
                    break;
                default:
                    throw new Exception(@"Attempted to construct a standard jumpthru with the rotated jumpthru constructor.");
            }
        }

        public CustomJT(Bounds b) : base(b) { }

        public override string ToString() => bounds.ToString();
    }

    public class NormalJT : CustomJT
    {
        public NormalJT(Bounds b) : base(b)
        {
            pullVector = new Vector2(0f, -40f);
            IsBeyond = pos => pos.Y < bounds.Uf;
            Pulling = (pos, spd) => spd.Y <= 0f & TouchingAsFeather(pos);
            axisFix = bounds.U - 1;
            horizontal = true;
        }
    }

    #endregion
}