namespace DAL.Entities
{
    public class Document
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public bool IsBorrowed { get; set; }
        public string? BorrowedByName { get; set; }

        public Document() { }

        public Document(string title, string author)
        {
            Title = title;
            Author = author;
            IsBorrowed = false;
            BorrowedByName = null;
        }
    }
}