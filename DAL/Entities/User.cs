namespace DAL.Entities
{
    public class User
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Group { get; set; }
        public List<string> BorrowedDocumentTitles { get; set; } = new List<string>();

        public User() { }

        public User(string firstName, string lastName, int group)
        {
            FirstName = firstName;
            LastName = lastName;
            Group = group;
        }
    }
}