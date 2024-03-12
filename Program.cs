using System.Globalization;
using Bib_RobinBachus.Utils;
using static Bib_RobinBachus.Utils.UserInput;
using static Bib_RobinBachus.Utils.Utils;

namespace Bib_RobinBachus
{
	/// <summary>
	///     Represents the main program class.
	/// </summary>
	internal class Program
	{
		private const string DEFAULT_PATH = "Data/Books.csv";

		/// <summary>
		///     The entry point of the program.
		/// </summary>
		public static void Main()
		{
			CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");

			Library library = new("Infernal Forest");

			Init(library);

			bool exit;
			do
			{
				exit = !ShowMenu(library);
			} while (!exit);

			Console.WriteLine("Bedankt voor uw bezoek, tot de volgende keer!");
		}

		/// <summary>
		///     Initializes the library.
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
		///     Loads data from a file into the library.
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
		///     Saves the library data to a file.
		/// </summary>
		/// <param name="library">The library object.</param>
		/// <param name="path">The path to the file.</param>
		private static void SaveData(Library library, string? path = null)
		{
			path = PromptPath(path);
			if (path is null) return; // null means the user wants to cancel the operation

			string finalPath = Book.SaveToCsv(path, library);
			Console.WriteLine($"Data opgeslagen naar {finalPath}");

			bool open = PromptBool("Wil je het bestand openen?", "n");
			if (open) OpenWithDefaultProgram(finalPath);
			Console.WriteLine("");
		}

		/// <summary>
		///     Shows the menu and handles the user's choice.
		/// </summary>
		/// <param name="library">The library object.</param>
		/// <returns> <c>false</c> if the user wants to exit the program; otherwise, <c>true</c>.</returns>
		private static bool ShowMenu(Library library)
		{
			Menu menu = new($"Welkom bij {library.Name}! Wat wil je doen?", "\nKeuze", new[] { "exit" });

			menu.AddMenuItem("Voeg een boek toe", () => MakeBook(library));
			menu.AddMenuItem("Voeg info toe aan een boek", () => AddInfoToBook(library));
			menu.AddMenuItem("Toon alle info van een boek",
				() => Console.WriteLine(library.PromptFindBook()?.ToString() ?? "Boek niet gevonden"));
			menu.AddMenuItem("Zoek boeken", () => SearchBook(library));
			menu.AddMenuItem("Verwijder een boek", () =>
			{
				string? isbn = PromptIsbn();
				if (isbn is null) return; // PromptIsbn() returns null if the user wants to cancel the operation
				Console.WriteLine(library.RemoveBook(isbn) ? "Boek verwijderd." : "Boek niet gevonden.");
			});
			menu.AddMenuItem("Toon alle boeken", () => library.Books.ForEach(b => Console.WriteLine(b.Header)));
			menu.AddMenuItem("Laad data van bestand", () => LoadData(library));
			menu.AddMenuItem("Exporteer boeken naar csv", () => SaveData(library, DEFAULT_PATH));

			return menu.ShowMenu(true);
		}

		/// <summary>
		///     Prompts the user for book details and adds a new book to the library.
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

		private static void AddInfoToBook(Library library)
		{
			Book? book = library.PromptFindBook();
			if (book is null)
			{
				Console.WriteLine("Boek niet gevonden.");
				return;
			}

			// This menu will be added to the main menu and will be shown when the chooses to change the book's type
			Menu typeMenu = new("Kies een type:", "Keuze", new[] { "terug" });
			typeMenu.AddMenuItem("Hardcover", () => book.Type = "Hardcover");
			typeMenu.AddMenuItem("Paperback", () => book.Type = "Paperback");
			typeMenu.AddMenuItem("Kindle", () => book.Type = "Kindle");

			// Create a new menu for the user to choose what to add to the book
			Menu menu = new("Wat wil je toevoegen?", "Keuze", new[] { "terug" });

			menu.AddMenuItem("Isbn", AddIsbn);
			menu.AddMenuItem("Genre", AddGenre);
			// Add the typeMenu to the main menu
			menu.AddMenuItem("Type", () => typeMenu.ShowMenu());
			menu.AddMenuItem("Uitgave jaar", () => { book.PublishDate = Prompt<DateTime>("Uitgave datum: "); });
			menu.AddMenuItem("Prijs", () => { book.Price = Prompt<double>("Prijs: ", "Ongeldige prijs. Probeer opnieuw."); });
			menu.AddMenuItem("18+ rating", () => { book.HasMatureRating = PromptBool("Is het boek 18+?"); });

			menu.ShowMenu();

			return;

			void AddIsbn()
			{
				string? isbn = PromptIsbn();
				if (isbn is null) return; // PromptIsbn() returns null if the user wants to cancel the operation
				book.IsbnNumber = isbn;
			}

			void AddGenre()
			{
				Console.WriteLine("Kies een genre:");
				foreach (Genre genre in Enum.GetValues(typeof(Genre)))
					if (genre is not Genre.None)
						Console.WriteLine($"{(int)genre}. {genre}");

				book.Genre = (Genre)PromptRange("Keuze", 0, Enum.GetValues(typeof(Genre)).Length - 2);
			}
		}

		/// <summary>
		///     Searches for books based on user input.
		/// </summary>
		/// <param name="library">The library object.</param>
		private static void SearchBook(Library library)
		{
			Menu menu = new("Waarop wil je zoeken?", "Keuze", new[] { "terug" });

			menu.AddMenuItem("ISBN", IsbnSearch);
			menu.AddMenuItem("Auteur", AuthorSearch);
			menu.AddMenuItem("Prijs range", PriceSearch);

			menu.ShowMenu();
			
			return;

			void IsbnSearch()
			{
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
			}

			void AuthorSearch()
			{
				string author = Prompt("Auteur: ");

				List<Book> books = library.FindBooks(author);
				if (books.Count == 0)
				{
					Console.WriteLine("Geen boeken gevonden");
					return;
				}

				books.ForEach(b => Console.WriteLine(b.Header));
			}

			void PriceSearch()
			{
				double minPrice = PromptRange("Minimumprijs: ", library.LowestPrice, library.HighestPrice);
				double maxPrice = PromptRange("Maximumprijs: ", minPrice, library.HighestPrice);

				List<Book> books = library.FindBooks(minPrice, maxPrice);
				if (books.Count == 0)
				{
					Console.WriteLine("Geen boeken gevonden");
					return;
				}

				books.ForEach(b => Console.WriteLine(b.Header));
			}

		}
	}
}