using System.Collections.Immutable;

namespace Bib_RobinBachus.ReadingRoom
{
	internal class Magazine : ReadingRoomItem
	{
		private static readonly ImmutableDictionary<string, string> identificationCodes =
			new Dictionary<string, string>()
			{
				{ "data news", "DN" },
				{ "national geographic", "NG"},
				{ "flair", "FL" },
				{ "libelle", "LB" },
				{ "humo", "HM" },
				{ "knack", "KN" },
				{ "de morgen magazine", "DMM" },
				{ "de standaard magazine", "DSM" },
				{ "feeling", "FE" },
				{ "story", "ST" }
			}.ToImmutableDictionary();

		private byte month;
		public byte Month
		{
			get => month;
			set
			{
				if (value > 12)
					Console.WriteLine("De maand is maximaal 12");
				else month = value;
			}
		}

		private uint year;
		public uint Year
		{
			get => year;
			set
			{
				if (value > 2500)
					Console.WriteLine("Het jaartal is maximaal 2500");
				else year = value;
			}
		}

		public override string Identification => $"{identificationCodes[Title.ToLower()]}{Month:D2}{Year:D4}";

		public override string Category => "Maandblad";

		public Magazine(string title, string publisher, byte month, uint year) : base(title, publisher)
		{
			Month = month;
			Year = year;
		}
	}
}
