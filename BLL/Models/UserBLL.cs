using System.Collections.Generic;

namespace BLL.Models
{
    public class UserBLL
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Group { get; set; }
        public List<string> BorrowedDocumentTitles { get; set; } = new List<string>();
    }
}