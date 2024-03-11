using System.Reflection;
// ReSharper disable ConvertToAutoProperty

namespace Bib_RobinBachus
{
    /// <summary>
    /// Represents a book.
    /// </summary>
    internal class Book
    {
        #region Properties

        private string isbnNumber;
        public string IsbnNumber
        {
            get => isbnNumber;
            set
            {
                if (!IsValidIsbn(value).isValid)
                {
                    Console.WriteLine($"Invalid ISBN: '{value}'. Keeping current value: {isbnNumber}");
                    Console.WriteLine($"Reason: {IsValidIsbn(value).errorMsg}");
                    return;
                }

                isbnNumber = value;
            }
        }

        private string title;
        public string Title
        {
            get => title;
            set => StringPropertyNotEmpty(value, nameof(Title), ref title);
        }


        private string author;
        public string Author
        {
            get => author;
            set => StringPropertyNotEmpty(value, nameof(Author), ref author);
        }

        private double price;
        public double Price
        {
            get => price;
            set
            {
                if (value < 0)
                    Console.WriteLine("Price can't be negative");
                else price = value;
            }
        }

        private bool hasMatureRating;
        public bool HasMatureRating
        {
            get => hasMatureRating;
            set => hasMatureRating = value;
        }

        private DateTime publishDate = DateTime.MaxValue;
        public DateTime PublishDate
        {
            get => publishDate;
            set => publishDate = value;
        }

        private Genre genre = Genre.None;
        public Genre Genre
        {
            get => genre;
            set => genre = value;
        }

        private string type = "Hardcover";
        public string Type
        {
            get => type;
            set => type = value;
        }

        public string Header => $"{Title} van {Author} ({Type})";

        #endregion 

        /// <summary>
        /// Initializes a new instance of the <see cref="Book"/> class.
        /// </summary>
        /// <param name="isbn">The ISBN number of the book.</param>
        /// <param name="title">The title of the book.</param>
        /// <param name="author">The author of the book.</param>
        /// <param name="price">The price of the book.</param>
        /// <param name="library">The library to add the book to.</param>
        public Book(string isbn, string title, string author, double price, Library library)
        {
            this.title = title;
            this.author = author;
            this.price = price;
            isbnNumber = isbn;

            library.Books.Add(this);
        }


        /// <summary>
        /// Loads a list of books from a CSV file.
        /// </summary>
        /// <param name="filePath">The path of the CSV file.</param>
        /// <param name="library">The library to add the loaded books to.</param>
        /// <returns>The list of loaded books.</returns>
        public static List<Book> LoadFromCsv(string filePath, Library library)
        {
            string[] lines = File.ReadAllLines(Path.GetFullPath(filePath));

            List<Book> books = lines
                .Select(row => row.Split(';'))
                .Select(columnValues => new Book(
                    columnValues[0],
                    columnValues[1],
                    columnValues[2],
                    double.Parse(columnValues[3]),
                    library
                )
                {
                    PublishDate = DateTime.Parse(columnValues[4]),
                    Genre = Enum.Parse<Genre>(columnValues[5].Replace(" ", string.Empty)),
                    Type = columnValues[6],
                    HasMatureRating = bool.Parse(columnValues[7]),
                })
                .ToList();
            return books;
        }

        /// <summary>
        /// Saves the list of books to a CSV file.
        /// </summary>
        /// <param name="filePath">The path of the CSV file.</param>
        /// <param name="books">The list of books to save.</param>
        /// <returns>The full path of the saved CSV file.</returns>
        public static string SaveToCsv(string filePath, List<Book> books)
        {
            string[] lines = books
                .Select(book => $"{book.IsbnNumber};{book.Title};{book.Author};{book.Price};{book.PublishDate:yyyy-MM-dd};{book.Genre};{book.Type};{book.HasMatureRating}")
                .ToArray();

            File.WriteAllLines(Path.GetFullPath(filePath), lines);
            return Path.GetFullPath(filePath);
        }

        /// <summary>
        /// Saves the list of books from a library to a CSV file.
        /// </summary>
        /// <param name="filePath">The path of the CSV file.</param>
        /// <param name="library">The library to save the books from.</param>
        /// <returns>The full path of the saved CSV file.</returns>
        public static string SaveToCsv(string filePath, Library library) => SaveToCsv(filePath, library.Books);
        
        /// <summary>
        /// Returns an overview of the book
        /// </summary>
        /// <returns> A string with the properties of the book. </returns>
        public override string ToString()
        {
            return GetType().GetProperties().Aggregate(string.Empty, Predicate);

            string Predicate(string current, PropertyInfo propertyInfo)
                => current + $"{propertyInfo.Name,-15} {"-",-2} {propertyInfo.GetValue(this)}\n";
        }

        /// <summary>
        /// Checks if the specified string value is not empty and assigns it to the specified property.
        /// </summary>
        /// <param name="value">The string value to check.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="property">The reference to the property.</param>
        private static void StringPropertyNotEmpty(string value, string propertyName, ref string property)
        {
            if (value is "")
                Console.WriteLine($"{propertyName} cannot be empty. Keeping current value: '{property}'");
            else property = value;
        }

        /// <summary>
        /// Validates the checksum of an ISBN number.
        /// </summary>
        /// <param name="isbn"> The ISBN number to validate. </param>
        /// <returns> a tuple with a boolean indicating whether the ISBN is valid, and a string with an error message if the ISBN is invalid. </returns>
        public static (bool isValid, string errorMsg) IsValidIsbn(string isbn)
        {
            int digits = isbn.Length;
            if (digits is 10 or 13)
                return (
                    digits == 10
                    ? Isbn10ChecksumValid(isbn)
                    : Isbn13ChecksumValid(isbn)
                    , $"Invalid ISBN ({isbn})");

            return (false, $"ISBN must be 10 or 13 digits long ({isbn})");
        }

        /// <summary>
        /// Validates the checksum of an ISBN-13 number.
        /// <para> <see href="https://en.wikipedia.org/wiki/International_Standard_Book_Number#ISBN-13_check_digit_calculation">
        /// See the Wikipedia article for more information on the checksum calculation.
        /// </see> </para>
        /// </summary>
        /// <param name="isbn"> The ISBN-13 number to validate. Must be 13 digits long. </param>
        /// <returns> True if the checksum is valid, false otherwise. </returns>
        public static bool Isbn13ChecksumValid(string isbn)
        {
            int sum = 0;
            for (int i = 0; i < 13; i++)
            {
                if (!int.TryParse(isbn[i].ToString(), out int digit))
                    return false;
                sum += (i % 2 == 0) ? digit : digit * 3;
            }

            return sum % 10 == 0;
        }

        /// <summary>
        /// Validates the checksum of an ISBN-10 number.
        /// <para> <see href="https://en.wikipedia.org/wiki/International_Standard_Book_Number#ISBN-10_check_digit_calculation">
        /// See the Wikipedia article for more information on the checksum calculation.
        /// </see> </para>
        /// </summary>
        /// <param name="isbn"> The ISBN-10 number to validate. Must be 10 digits long. </param>
        /// <returns> True if the checksum is valid, false otherwise. </returns>
        public static bool Isbn10ChecksumValid(string isbn)
        {
            int sum;
            try
            {
                sum = isbn
                    .Select(t => t.ToString().ToLower())
                    .Select(sDigit => sDigit == "x" ? 10 : int.Parse(sDigit))
                    .Select((digit, i) => (i + 1) * digit)
                    .Sum();
            }
            catch (FormatException)
            {
                return false; // ISBN contains non-numeric characters
            }
            
            return sum % 11 == 0;
        }
    }
}