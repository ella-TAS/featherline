namespace Featherline;

class InputCleaner
{
	private bool extremeTurns;
	private FeatherSim sim;

	private int turningStart;
	private float angleBeforeTurn;

	private TurnState prevTurn = TurnState.None;

	public float lastFrameAngle = Level.startState.fState.spd.TASAngle;

	public void Update(bool forceClean = false)
	{
		if (forceClean && sim.fs.f >= sim.ind.Length) return;

		float actualAngle = sim.fs.spd.TASAngle;

		TurnState current;

		if (sim.wallboops.Count > 0 && sim.wallboops[^1] == sim.fs.f) {
			var savePostBoop = actualAngle;
			if (sim.wallboops.Count < 2 || sim.wallboops[^2] != sim.fs.f) {
				if (prevTurn != TurnState.None) {
					actualAngle = lastFrameAngle;
					current = TurnState.None;
					EndTurn(lastFrameAngle);
				}
			}
			prevTurn = TurnState.WallBooping;
			lastFrameAngle = savePostBoop;

			float force = Math.Abs(FeatherSim.DegreesDiff(actualAngle, sim.ind[sim.fs.f]));
			prevTurn = force > 5.333f ? TurnState.Clockwise : force < -5.333f ? TurnState.AntiClockwise : TurnState.None;
			if (prevTurn != TurnState.None)
				BeginTurn();

			return;
		}

		float turnForce = FeatherSim.DegreesDiff(lastFrameAngle, sim.ind[sim.fs.f]);
		current = turnForce > 5.333f ? TurnState.Clockwise : turnForce < -5.333f ? TurnState.AntiClockwise : TurnState.None;


		if (current != prevTurn | forceClean) {
			if (prevTurn == TurnState.None)
				BeginTurn();
			else {
				EndTurn(prevTurn == TurnState.Clockwise ? (float)Math.Ceiling(lastFrameAngle + 1) : (float)Math.Floor(lastFrameAngle - 1));
				if (current != TurnState.None)
					BeginTurn();
			}

			prevTurn = current;
		}

		lastFrameAngle = actualAngle;

		void BeginTurn()
		{
			angleBeforeTurn = lastFrameAngle;
			turningStart = sim.fs.f;
		}

		void EndTurn(float targetAngle)
		{
			int i = turningStart; //- (justBooped ? 1 : 0);
			float placementAngle = (float)Math.Round(angleBeforeTurn);
			while (true) {
				if (sim.fs.f - i < 11) {
					if (current != TurnState.None | extremeTurns)
						sim.ind.SetRange(targetAngle, i, sim.fs.f);
					//justBooped = false;
					break;
				}

				placementAngle += 60 * (int)prevTurn;
				int lineTarget = i + 11;
				sim.ind.SetRange(placementAngle, i, lineTarget);
				i = lineTarget;
			}
		}
	}

	public InputCleaner(FeatherSim sim, bool extremeTurns)
	{
		this.sim = sim;
		this.extremeTurns = extremeTurns;
	}
}

public enum TurnState
{
	None = 0,
	Clockwise = 1,
	AntiClockwise = -1,
	WallBooping = 2
}
