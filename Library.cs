// ReSharper disable ConvertToAutoProperty
// ReSharper disable ConvertToAutoPropertyWhenPossible

using static Bib_RobinBachus.UserInput;

namespace Bib_RobinBachus
{
	internal class Library
	{
		// Properties
		private string name;
		public string Name
		{
			get => name;
			set => name = value;
		}

		private ConsoleColor? color;
		public ConsoleColor? Color
		{
			get => color;
			set => color = value;
		}

		private List<Book> books = new();
		public List<Book> Books
		{
			get => books;
			set => books = value;
		}

		// Calculated properties
		public double LowestPrice
		{
			get
			{
				if (books.Count == 0) return 0;
				return books.Min(book => book.Price);
			}
		}
		public double HighestPrice
		{
			get
			{
				if (books.Count == 0) return 0;
				return books.Max(book => book.Price);
			}
		}


		public Library(string name)
		{
			this.name = name;
		}

		public Library(string name, ConsoleColor color)
		{
			this.name = name;
			this.color = color;
		}

		public bool RemoveBook(string isbn)
		{
			return 0 != books.RemoveAll(book => book.IsbnNumber == isbn);
		}

		/// <summary>
		/// Find a book by asking for the title and author
		/// </summary>
		/// <returns>The book if found, null otherwise.</returns>
		public Book? PromptFindBook()
		{
			string title = Prompt("Titel: ");
			string author = Prompt("Auteur: ");

			return FindBook(author, title);
		}


		/// <summary>
		/// Finds a book in the library by its ISBN number.
		/// </summary>
		/// <param name="isbn">The ISBN number of the book to find.</param>
		/// <returns>The book if found, null otherwise.</returns>
		public Book? FindBook(string isbn)
		{
			return books.Find(book => book.IsbnNumber == isbn);
		}

		/// <summary>
		/// Finds a book in the library by its author and title.
		/// </summary>
		/// <param name="author">The author of the book to find.</param>
		/// <param name="title">The title of the book to find.</param>
		/// <returns>The book if found, null otherwise.</returns>
		public Book? FindBook(string author, string title)
		{
			List<Book> results = books.FindAll(book => book.Author == author && book.Title == title);

			switch (results.Count)
			{
				case 1:
					return results[0];
				case 0:
					return null;
			}

			Console.WriteLine("Meerdere boeken gevonden, gelieve een keuze te maken");
			int i = 1;
			results.ForEach(b => Console.WriteLine($"{i++}. {b.Header}"));
			int choice = PromptRange("Keuze", 1, results.Count);
			if (choice >= 1 && choice <= results.Count)
				return results[choice - 1];
			Console.WriteLine("Ongeldige keuze. Probeer opnieuw.");
			return null;

		}

		public List<Book> FindBooks(string author)
		{
			return books.FindAll(book => book.Author == author);
		}

		public List<Book> FindBooks(double minPrice, double maxPrice)
		{
			return books.FindAll(book => book.Price >= minPrice && book.Price <= maxPrice);
		}
	}
}
