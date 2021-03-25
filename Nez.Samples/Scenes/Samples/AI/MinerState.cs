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

		public const int MaxFatigue = 10;
		public const int MaxGold = 8;
		public const int MaxThirst = 5;

		public int Fatigue;
		public int Thirst;
		public int Gold;
		public int GoldInBank;

		public Location CurrentLocation = Location.Home;
	}
}