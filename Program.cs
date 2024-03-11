using Bib_RobinBachus.ReadingRoom;
using static Bib_RobinBachus.Utils.UserInput;
using static Bib_RobinBachus.Utils.Utils;

namespace Bib_RobinBachus
{
	/// <summary>
	/// Represents the main program class.
	/// </summary>
	internal class Program
	{
		private const string DEFAULT_PATH = "Data/Books.csv";

		/// <summary>
		/// The entry point of the program.
		/// </summary>
		public static void Main()
		{
			Library library = new("Infernal Forest");

			library.AddMagazine();
			library.ShowAllMagazines();

			Init(library);

			bool exit;
			do exit = ShowMenu(library);
			while (!exit);

			Console.WriteLine("Bedankt voor uw bezoek, tot de volgende keer!");
		}

		/// <summary>
		/// Initializes the library.
		/// </summary>
		/// <param name="library">The library object.</param>
		private static void Init(Library library)
		{
			bool useCsv = PromptBool("Wil je boeken laden vanuit een csv bestand?");

			if (useCsv)
				LoadData(library, DEFAULT_PATH);
			else
				Console.WriteLine("Starting without books");

			PromptKey();
		}

		/// <summary>
		/// Loads data from a file into the library.
		/// </summary>
		/// <param name="library">The library object.</param>
		/// <param name="path">The path to the file.</param>
		private static void LoadData(Library library, string? path = null)
		{
			path = PromptPath(path);
			if (path is null) return; // null means the user wants to cancel the operation

			int count = Book.LoadFromCsv(path, library).Count;
			Console.WriteLine($"{count} boeken ingeladen!");
		}

		/// <summary>
		/// Saves the library data to a file.
		/// </summary>
		/// <param name="library">The library object.</param>
		/// <param name="path">The path to the file.</param>
		private static void SaveData(Library library, string? path = null)
		{
			path = PromptPath(path);
			if (path is null) return; // null means the user wants to cancel the operation

			string finalPath = Book.SaveToCsv(path, library);
			Console.WriteLine($"Data opgeslagen naar {finalPath}");

			bool open = PromptBool("Wil je het bestand openen?");
			if (open) OpenWithDefaultProgram(finalPath);
			Console.WriteLine("");
		}

		/// <summary>
		/// Shows the menu and handles the user's choice.
		/// </summary>
		/// <param name="library">The library object.</param>
		/// <returns>Exit state: true for exit, otherwise false.</returns>
		private static bool ShowMenu(Library library)
		{
			Console.WriteLine($"Welkom bij {library.Name}!");
			Console.WriteLine("Wat wil je doen?\n");

			Console.WriteLine("1. Voeg een boek toe");
			Console.WriteLine("2. Voeg info toe aan een boek");
			Console.WriteLine("3. Toon alle info van een boek");
			Console.WriteLine("4. Zoek boeken");
			Console.WriteLine("5. Verwijder een boek");
			Console.WriteLine("6. Toon alle boeken");
			Console.WriteLine("7. Laad data van bestand");
			Console.WriteLine("8. Exporteer boeken naar csv");

			Console.WriteLine("\n0. Type exit om te stoppen");

			(_, int? choice) = PromptRange("\nKeuze", 0, 8, new[] { "exit" }, "Ongeldige keuze. Probeer opnieuw.");
			choice ??= 0;
			Console.Clear();

			switch (choice)
			{
				case 1:
					bool exit;
					do exit = MakeBook(library);
					while (!exit);
					break;
				case 2:
					AddInfoToBook(library);
					break;
				case 3:
					Console.WriteLine(library.PromptFindBook()?.ToString() ?? "Boek niet gevonden");
					break;
				case 4:
					SearchBook(library);
					break;
				case 5:
					string? isbn = PromptIsbn();
					if (isbn is null) break; // PromptIsbn() returns null if the user wants to cancel the operation
					Console.WriteLine(library.RemoveBook(isbn) ? "Boek verwijderd." : "Boek niet gevonden.");
					break;
				case 6:
					library.Books.ForEach(b => Console.WriteLine(b.Header));
					break;
				case 7:
					LoadData(library);
					break;
				case 8:
					SaveData(library, DEFAULT_PATH);
					break;
				case 0:
					return true;
				default:
					Console.WriteLine("Ongeldige keuze. Probeer opnieuw.");
					break;
			}

			PromptKey();

			return false;
		}

		/// <summary>
		/// Prompts the user for book details and adds a new book to the library.
		/// </summary>
		/// <param name="library">The library object.</param>
		/// <returns>False if the user wants to cancel the operation, otherwise true.</returns>
		private static bool MakeBook(Library library)
		{
			string? isbn = PromptIsbn();
			if (isbn is null) return false; // PromptIsbn() returns null if the user wants to cancel the operation

			string title = Prompt("Titel: ");
			string author = Prompt("Auteur: ");
			double price = Prompt<double>("Prijs: ", "Ongeldige prijs. Probeer opnieuw.");

			Book book = new(isbn, title, author, price, library);

			Console.WriteLine("Book added!\n");
			Console.WriteLine(book);
			return true;
		}

		/// <summary>
		/// Prompts the user to add additional information to a book.
		/// </summary>
		/// <param name="library">The library object.</param>
		private static void AddInfoToBook(Library library)
		{
			Book? book = library.PromptFindBook();
			if (book is null)
			{
				Console.WriteLine("Boek niet gevonden.");
				return;
			}

			Console.WriteLine("Wat wil je toevoegen?");
			Console.WriteLine("1. Isbn");
			Console.WriteLine("2. Genre");
			Console.WriteLine("3. Type");
			Console.WriteLine("4. Uitgave jaar");
			Console.WriteLine("5. Prijs");
			Console.WriteLine("6. 18+ rating");
			Console.WriteLine("7. Terug");


			int choice = PromptRange("\nKeuze", 1, 7, "Ongeldige keuze. Probeer opnieuw.");
			switch (choice)
			{
				case 1:
					string? isbn = PromptIsbn();
					if (isbn is null) return;
					book.IsbnNumber = isbn;
					break;
				case 2:
					Console.WriteLine("Kies een genre:");
					foreach (Genre genre in Enum.GetValues(typeof(Genre)))
						if (genre is not Genre.None) 
							Console.WriteLine($"{(int)genre}. {genre}");

					book.Genre = (Genre)PromptRange("Keuze", 0, Enum.GetValues(typeof(Genre)).Length - 2);
					break;
				case 3:
					Console.WriteLine("Kies een type:");
					Console.WriteLine("1. Hardcover");
					Console.WriteLine("2. Paperback");
					Console.WriteLine("3. Kindle");

					if (!int.TryParse(Console.ReadLine(), out int type))
					{
						Console.WriteLine("Ongeldige keuze. Probeer opnieuw.");
						break;
					}

					book.Type = type switch
					{
						1 => "Hardcover",
						2 => "Paperback",
						3 => "Kindle",
						_ => "Hardcover"
					};
					break;
				case 4:
					if (!DateTime.TryParse(Console.ReadLine(), out DateTime date))
					{
						Console.WriteLine("Ongeldige datum. Probeer opnieuw.");
						break;
					}

					book.PublishDate = date;
					break;
				case 5:
					book.Price = Prompt<double>("Prijs: ", "Ongeldige prijs. Probeer opnieuw.");
					break;
				case 6:
					book.HasMatureRating = PromptBool("Is het boek 18+?");
					break;
				case 7:
					return;
				default:
					Console.WriteLine("Ongeldige keuze. Probeer opnieuw.");
					break;
			}
		}

		/// <summary>
		/// Searches for books based on user input.
		/// </summary>
		/// <param name="library">The library object.</param>
		private static void SearchBook(Library library)
		{
			Console.WriteLine("Waarop wil je zoeken?");
			Console.WriteLine("1. ISBN");
			Console.WriteLine("2. Auteur");
			Console.WriteLine("3. Prijs range");


			int choice = PromptRange("\nKeuze", 1, 3, "Ongeldige keuze, probeer opnieuw.");
			switch (choice)
			{
				case 1:
					string? isbn = PromptIsbn();
					if (isbn is null)
					{
						Console.WriteLine("Aanvraag afgebroken");
						return;
					}

					Book? book = library.FindBook(isbn);
					if (book is null)
					{
						Console.WriteLine("Boek niet gevonden.");
						return;
					}

					Console.WriteLine(book.Header);
					break;
				case 2:
					string author = Prompt("Auteur: ");

					List<Book> books = library.FindBooks(author);
					if (books.Count == 0)
					{
						Console.WriteLine("Geen boeken gevonden");
						return;
					}

					books.ForEach(ShowHeaders);
					break;
				case 3:
					double minPrice = PromptRange("Minimumprijs: ", library.LowestPrice, library.HighestPrice);
					double maxPrice = PromptRange("Maximumprijs: ", minPrice, library.HighestPrice);

					books = library.FindBooks(minPrice, maxPrice);
					if (books.Count == 0)
					{
						Console.WriteLine("Geen boeken gevonden");
						return;
					}

					books.ForEach(ShowHeaders);
					break;
				default:
					Console.WriteLine("Ongeldige keuze. Probeer opnieuw.");
					break;
			}

			return;

			static void ShowHeaders(Book b) => Console.WriteLine(b.Header);
		}
	}
}