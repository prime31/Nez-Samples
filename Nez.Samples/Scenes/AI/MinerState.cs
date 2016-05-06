using System;


namespace Nez.Samples
{
	/// <summary>
	/// common state for each of the AI types
	/// </summary>
	public class MinerState
	{
		public enum Location
		{
			InTransit,
			Bank,
			Mine,
			Home,
			Saloon
		}

		public const int MAX_FATIGUE = 10;
		public const int MAX_GOLD = 8;
		public const int MAX_THIRST = 5;

		public int fatigue;
		public int thirst;
		public int gold;
		public int goldInBank;

		public Location currentLocation = Location.Home;
	}
}

