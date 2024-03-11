// ReSharper disable ConvertToAutoProperty
// ReSharper disable ConvertToAutoPropertyWhenPossible

using Bib_RobinBachus.Utils;

namespace Bib_RobinBachus.ReadingRoom
{
    internal abstract class ReadingRoomItem
    {
        private readonly string title;
        public string Title => title;

        private string publisher;
        public string Publisher
        {
            get => publisher;
            set => publisher = value;
        }

        public abstract string Identification { get; }

        public abstract string Category { get; }


        protected ReadingRoomItem(string title, string publisher)
        {
            this.title = title.CapitalizeAll();
            this.publisher = publisher.CapitalizeAll();
        }
    }
}
