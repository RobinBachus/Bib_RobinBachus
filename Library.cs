// ReSharper disable ConvertToAutoProperty
// ReSharper disable ConvertToAutoPropertyWhenPossible

using static Bib_RobinBachus.Utils.UserInput;
using Bib_RobinBachus.ReadingRoom;

namespace Bib_RobinBachus
{
	/// <summary>
	/// Represents a library.
	/// </summary>
	internal class Library
	{
		#region Properties

		/// <summary>
		/// Gets or sets the name of the library.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the color of the library.
		/// TODO: Make the console use this color.
		/// </summary>
		public ConsoleColor? Color { get; set; }

		/// <summary>
		/// Gets or sets the list of books in the library.
		/// </summary>
		public List<Book> Books { get; set; }

		private readonly Dictionary<DateTime, ReadingRoomItem> allReadingRoom = new();
		public Dictionary<DateTime, ReadingRoomItem> AllReadingRoom => allReadingRoom;

		#endregion

		#region Calculated properties

		/// <summary>
		/// Gets the lowest price among all the books in the library.
		/// </summary>
		public double LowestPrice
		{
			get
			{
				if (Books.Count == 0) return 0;
				return Books.Min(book => book.Price);
			}
		}

		/// <summary>
		/// Gets the highest price among all the books in the library.
		/// </summary>
		public double HighestPrice
		{
			get
			{
				if (Books.Count == 0) return 0;
				return Books.Max(book => book.Price);
			}
		}

		#endregion

		/// <summary>
		/// Initializes a new instance of the <see cref="Library"/> class with the specified name.
		/// </summary>
		/// <param name="name">The name of the library.</param>
		public Library(string name)
		{
			Name = name;
			Books = new List<Book>();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Library"/> class with the specified name and color.
		/// </summary>
		/// <param name="name">The name of the library.</param>
		/// <param name="color">The color of the library.</param>
		public Library(string name, ConsoleColor color)
		{
			Name = name;
			Color = color;
			Books = new List<Book>();
		}

		/// <summary>
		/// Removes a book from the library based on its ISBN number.
		/// </summary>
		/// <param name="isbn">The ISBN number of the book to remove.</param>
		/// <returns><c>true</c> if the book was removed successfully; otherwise, <c>false</c>.</returns>
		public bool RemoveBook(string isbn)
		{
			return Books.RemoveAll(book => book.IsbnNumber == isbn) != 0;
		}

		/// <summary>
		/// Finds a book in the library by its ISBN number.
		/// </summary>
		/// <param name="isbn">The ISBN number of the book to find.</param>
		/// <returns>The book if found; otherwise, <c>null</c>.</returns>
		public Book? FindBook(string isbn)
		{
			return Books.Find(book => book.IsbnNumber == isbn);
		}

		/// <summary>
		/// Finds a book in the library by its author and title.
		/// </summary>
		/// <param name="author">The author of the book to find.</param>
		/// <param name="title">The title of the book to find.</param>
		/// <returns>The book if found; otherwise, <c>null</c>.</returns>
		public Book? FindBook(string author, string title)
		{
			List<Book> results = Books.FindAll(book => book.Author == author && book.Title == title);

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

		/// <summary>
		/// Finds all books in the library by the specified author.
		/// </summary>
		/// <param name="author">The author of the books to find.</param>
		/// <returns>A list of books written by the specified author.</returns>
		public List<Book> FindBooks(string author)
		{
			return Books.FindAll(book => book.Author == author);
		}

		/// <summary>
		/// Finds all books in the library within the specified price range.
		/// </summary>
		/// <param name="minPrice">The minimum price of the books to find.</param>
		/// <param name="maxPrice">The maximum price of the books to find.</param>
		/// <returns>A list of books within the specified price range.</returns>
		public List<Book> FindBooks(double minPrice, double maxPrice)
		{
			return Books.FindAll(book => book.Price >= minPrice && book.Price <= maxPrice);
		}

		public void AddNewspaper()
		{
			string title = Prompt("Wat is de naam van de krant: ");
			string publisher = Prompt("Wie is de uitgever: ");
			DateTime date = Prompt<DateTime>("Wat is de uitgave datum: ");
			allReadingRoom.Add(DateTime.Now, new NewsPaper(title, publisher, date));
		}

		public void AddMagazine()
		{
			string title = Prompt("Wat is de naam van het magazine: ");
			string publisher = Prompt("Wie is de uitgever: ");
			byte month = PromptRange<byte>("Wat is de maand van uitgave: ", 1, 12);
			uint year = PromptRange<uint>("Wat is het jaar van uitgave: ", 0, 2500);
			allReadingRoom.Add(DateTime.Now, new Magazine(title, publisher, month, year));
		}

		public void ShowAllMagazines()
		{
			Console.WriteLine("Alle maandbladen uit de leeszaal:");

			foreach ((_, ReadingRoomItem value) in allReadingRoom)
			{
				if (value is not Magazine magazine) continue;

				Console.WriteLine($"- {magazine.Title} ({magazine.Month}/{magazine.Year}) van {magazine.Publisher}");
			}
		}

		/// <summary>
		/// Prompts the user to enter the title and author of a book and finds it in the library.
		/// </summary>	
		/// <returns>The book if found; otherwise, <c>null</c>.</returns>
		public Book? PromptFindBook()
		{
			string title = Prompt("Titel: ");
			string author = Prompt("Auteur: ");

			return FindBook(author, title);
		}
	}
}
