using System.Collections.Generic;
using System.Linq;
using BLL.Models;

namespace BLL.Services
{
    public class LibraryService
    {
        private readonly UserService _userService;
        private readonly DocumentService _documentService;

        public LibraryService(UserService userService, DocumentService documentService)
        {
            _userService = userService;
            _documentService = documentService;
        }

        public bool BorrowDocument(UserBLL user, DocumentBLL doc)
        {
            if (user.BorrowedDocumentTitles.Count >= 5 || doc.IsBorrowed)
                return false;

            doc.IsBorrowed = true;
            doc.BorrowedByName = $"{user.FirstName} {user.LastName}";
            user.BorrowedDocumentTitles.Add(doc.Title);

            _userService.UpdateUser(user, user);
            _documentService.UpdateDocument(doc, doc);
            return true;
        }

        public void ReturnDocument(UserBLL user, DocumentBLL doc)
        {
            if (user.BorrowedDocumentTitles.Contains(doc.Title))
            {
                user.BorrowedDocumentTitles.Remove(doc.Title);
                doc.IsBorrowed = false;
                doc.BorrowedByName = null;

                _userService.UpdateUser(user, user);
                _documentService.UpdateDocument(doc, doc);
            }
        }

        public List<DocumentBLL> GetBorrowedByUser(UserBLL user)
        {
            return user.BorrowedDocumentTitles.Select(title => _documentService.GetDocument(title))
                                             .Where(d => d != null)
                                             .Cast<DocumentBLL>()
                                             .ToList();
        }

        public string GetDocumentStatus(DocumentBLL doc)
        {
            return doc.IsBorrowed ? $"Документ виданий: {doc.BorrowedByName}" : "Документ у бібліотеці";
        }
    }
}