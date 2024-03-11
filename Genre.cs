namespace Bib_RobinBachus
{
    /// <summary>
    /// Represents the genre of a book.
    /// None is more of a default value and should not be used as a genre.
    /// Use Misc for books that do not fit in any other genre or for books of wich the genre is not known.
    /// </summary>
    internal enum Genre
    {
        /// <summary>
        /// The genre is not specified.
        /// <para>
        /// Should not be used as a genre.
        /// Use Misc for books that do not fit in any other genre or for books of wich the genre is not known.
        /// </para>
        /// </summary>
        None = -1,
        /// <summary> For books that do not fit in any other genre. </summary>
        Misc, 
        HistoricalFiction,
        Fantasy,
        Thriller,
        Childrens,
        Fiction,
        Biography,
        History,
        Romance,
        SciFi,
        Dystopian,
        Mystery,
        NonFiction,
        Adventure
    }
}
