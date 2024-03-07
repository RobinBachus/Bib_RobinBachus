using static Bib_RobinBachus.UserInput;

namespace Bib_RobinBachus
{
	internal class Program
	{
		private const string DEFAULT_PATH = "Data/Books.csv";

		public static void Main()
		{
			Library library = new("Infernal Forest") ;

            Book book = new Book("9789024567897", "De Hobbit", "J.R.R. Tolkien", 12.50, library) {}

			Init(library);

			bool exit;
			do exit = ShowMenu(library);
			while (!exit);

			Console.WriteLine("Bedankt voor uw bezoek, tot de volgende keer!");
		}

		private static void Init(Library library)
		{
			bool useCsv = PromptBool("Use CSV for data?");
			if (!useCsv)
			{
				Console.WriteLine("Starting without books");
				PromptKey();
				return;
			}

			Console.WriteLine($"\nCurrent CSV: {Path.GetFullPath(DEFAULT_PATH)}");
			bool useDefaultCsv = PromptBool("Load this data?");

			LoadData(library, useDefaultCsv ? DEFAULT_PATH : null);

			PromptKey();
		}

		private static void LoadData(Library library, string? path = null)
		{
			path ??= Prompt("Path to CSV: ");

			if (!File.Exists(path))
			{
				bool createFile = PromptBool($"File not found. Create new file? ({Path.GetFullPath(path)})");
				if (!createFile)
				{
					Console.WriteLine("Starting without books");
					PromptKey();
					return;
				}
				Console.WriteLine("Creating new file...");
				File.Create(path).Close();
			}

			int count = Book.LoadFromCsv(path, library).Count;
			Console.WriteLine($"{count} books loaded!");
		}

		/// <summary>
		/// Show the menu and handle the user's choice
		/// </summary>
		/// <param name="library">The current library object</param>
		/// <returns>Exit state: true for exit, otherwise false</returns>
		private static bool ShowMenu(Library library)
		{
			Console.WriteLine($"Welkom bij {library.Name}!");
			Console.WriteLine("Wat wil je doen?\n");

			Console.WriteLine("1. Voeg een boek toe");
			Console.WriteLine("2. Voeg info toe aan een boek");
			Console.WriteLine("3. Toon alle info van een boek");
			Console.WriteLine("4. Zoek een boek op");
			Console.WriteLine("5. Verwijder een boek");
			Console.WriteLine("6. Toon alle boeken");
			Console.WriteLine("7. Laad data van bestand");
			Console.WriteLine("\n0. Type exit om te stoppen");

			(_, int? choice) = PromptRange("\nKeuze", 0, 7, new []{"exit"} , "Ongeldige keuze. Probeer opnieuw.");
			choice ??= 0;
			Console.Clear();

			switch (choice)
			{
				case 1:
					bool exit = false;
					while (!exit)
						exit = MakeBook(library);
					break;
				case 2:
					AddInfoToBook(library);
					break;
				case 3:
					Console.WriteLine(library.PromptFindBook());
					break;
				case 4:
					SearchBook(library);
					break;
				case 5:
					string isbn = PromptIsbn();
					Console.WriteLine(library.RemoveBook(isbn) ? "Boek verwijderd." : "Boek niet gevonden.");
					break;
				case 6:
					library
						.Books
						.ForEach(b => Console.WriteLine(b.Header));
					break;
				case 7:
					LoadData(library);
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

		private static void SearchBook(Library library)
		{
			Console.WriteLine("Waarop wil je zoeken?");
			Console.WriteLine("1. ISBN");
			Console.WriteLine("2. Auteur");
			Console.WriteLine("3. Prijs range");

			if (!int.TryParse(Console.ReadLine(), out int choice))
			{
				Console.WriteLine("Ongeldige keuze. Probeer opnieuw.");
				return;
			}

			switch (choice)
			{
				case 1:
					string isbn = PromptIsbn();
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
					double maxPrice = PromptRange("Minimumprijs: ", minPrice, library.HighestPrice);

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

		private static void AddInfoToBook(Library library)
		{
			Book? book = library.PromptFindBook();
			if (book is null)
			{
				Console.WriteLine("Boek niet gevonden.");
				return;
			}

			Console.WriteLine("Wat wil je toevoegen?");
			Console.WriteLine("1. Uitgeefdatum");
			Console.WriteLine("2. Genre");
			Console.WriteLine("3. Type");

			switch (Console.ReadLine())
			{
				case "1":
					if (!DateTime.TryParse(Console.ReadLine(), out DateTime date))
					{
						Console.WriteLine("Ongeldige datum. Probeer opnieuw.");
						break;
					}

					book.PublishDate = date;
					break;
				case "2":
					Console.WriteLine("Kies een genre:");
					foreach (Genre genre in Enum.GetValues(typeof(Genre)))
						Console.WriteLine($"{(int)genre}. {genre}");

					if (!int.TryParse(Console.ReadLine(), out int choice))
					{
						Console.WriteLine("Ongeldige keuze. Probeer opnieuw.");
						break;
					}

					book.Genre = (Genre)choice;
					break;
				case "3":
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
				default:
					Console.WriteLine("Ongeldige keuze. Probeer opnieuw.");
					break;
			}
		}

		private static bool MakeBook(Library library)
		{

			string isbn = PromptIsbn();
			string title = Prompt("Titel: ");
			string author = Prompt("Auteur: ");
			double price = Prompt<double>("Prijs: ", "Ongeldige prijs. Probeer opnieuw.");

			Book book = new(isbn, title, author, price, library);

			Console.WriteLine("Book added!\n");
			Console.WriteLine(book);
			return true;
		}
	}
}