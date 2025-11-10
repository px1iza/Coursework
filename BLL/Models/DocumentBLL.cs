namespace BLL.Models
{
    public class DocumentBLL
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public bool IsBorrowed { get; set; }
        public string? BorrowedByName { get; set; }
    }
}