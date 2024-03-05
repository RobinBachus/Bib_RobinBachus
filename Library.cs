// ReSharper disable ConvertToAutoProperty
// ReSharper disable ConvertToAutoPropertyWhenPossible

namespace Bib_RobinBachus
{
	internal class Library
	{
		private string name;
		public string Name
		{
			get => name;
			set => name = value;
		}

		private static List<Book> books = new();
		public static List<Book> Books
		{
			get => books;
			set => books = value;
		}

		public Library(string name)
		{
			this.name = name;
		}

		public static bool RemoveBook(ulong isbn)
		{
			return 0 != books.RemoveAll(book => book.IsbnNumber == isbn);
		}

		public static Book? FindBook(ulong isbn)
		{
			return books.Find(book => book.IsbnNumber == isbn);
		}
			
		public static Book? FindBook(string author, string title)
		{
			List<Book> results = Library.books.FindAll(book => book.Author == author && book.Title == title);
			switch (results.Count)
			{
				case 1:
					return results[0];
				case 0:
					Console.WriteLine("Geen boeken gevonden");
					return null;
			}

			Console.WriteLine("Meerdere boeken gevonden, gelieve een keuze te maken");
			int i = 1;
			results.ForEach(b => Console.WriteLine($"{i++}. {b.Header}"));
			if (int.TryParse(Console.ReadLine(), out int choice) && choice >= 1 && choice <= results.Count)
				return results[choice - 1];
			Console.WriteLine("Ongeldige keuze. Probeer opnieuw.");
			return null;

		}

		public static List<Book> FindBooks(string author)
		{
			return books.FindAll(book => book.Author == author);
		}

		public static List<Book> FindBooks(double minPrice, double maxPrice)
		{
			return books.FindAll(book => book.Price >= minPrice && book.Price <= maxPrice);
		}
	}
}
