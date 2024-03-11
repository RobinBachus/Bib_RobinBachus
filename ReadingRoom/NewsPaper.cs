using System.Collections.Immutable;

// ReSharper disable ConvertToAutoProperty

namespace Bib_RobinBachus.ReadingRoom
{
    internal class NewsPaper : ReadingRoomItem
    {
        private static readonly ImmutableDictionary<string, string> identificationCodes =
            new Dictionary<string, string>()
            {
                { "gazet van antwerpen", "GVA" },
                { "het laatste nieuws", "HLN" },
                { "de standaard", "DS" },
                { "het nieuwsblad", "NBL" },
                { "de morgen", "DM" },
                { "de tijd", "DT" },
                { "metro", "MT" },
                { "de zondag", "DZ" }
            }.ToImmutableDictionary();

        private DateTime date;
        public DateTime Date
        {
            get => date;
            set => date = value;
        }

        public override string Category => "Krant";

        public override string Identification => GetIdentification();

        public NewsPaper(string title, string publisher, DateTime date) : base(title, publisher)
        {
            Date = date;
        }
        private string GetIdentification()
        {
            return $"{identificationCodes[Title.ToLower()]}{Date:ddMMyyyy}";
        }
    }
}
