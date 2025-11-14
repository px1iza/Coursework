using System;
using System.Collections.Generic;
using System.Linq;
using BLL.Models;
using BLL.Exceptions;


namespace BLL.Services
{
    public class LibraryService
    {
        private const int MAX_BORROWED_DOCUMENTS = 5;
        private readonly UserService _userService;
        private readonly DocumentService _documentService;

        public LibraryService(UserService userService, DocumentService documentService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _documentService = documentService ?? throw new ArgumentNullException(nameof(documentService));
        }

        public void BorrowDocument(UserBLL user, DocumentBLL doc)
        {
            if (user == null || doc == null)
                throw new ArgumentNullException("Користувач або документ не можуть бути null");

            if (user.BorrowedDocumentTitles.Count >= MAX_BORROWED_DOCUMENTS)
                throw new UserLimitExceededException($"Користувач досяг ліміту ({MAX_BORROWED_DOCUMENTS} документів)");

            if (doc.IsBorrowed)
                throw new DocumentAlreadyBorrowedException($"Документ '{doc.Title}' вже виданий користувачу {doc.BorrowedByName}");

            doc.IsBorrowed = true;
            doc.BorrowedByName = $"{user.FirstName} {user.LastName}";
            user.BorrowedDocumentTitles.Add(doc.Title);

            _userService.UpdateUser(user, user);
            _documentService.UpdateDocument(doc, doc);
        }

        public void ReturnDocument(UserBLL user, DocumentBLL doc)
        {
            if (user == null || doc == null)
                throw new ArgumentNullException("Користувач або документ не можуть бути null");

            if (!user.BorrowedDocumentTitles.Contains(doc.Title))
                throw new LibraryException($"Користувач {user.FirstName} {user.LastName} не має документа '{doc.Title}'");

            user.BorrowedDocumentTitles.Remove(doc.Title);
            doc.IsBorrowed = false;
            doc.BorrowedByName = null;

            _userService.UpdateUser(user, user);
            _documentService.UpdateDocument(doc, doc);
        }

        public List<DocumentBLL> GetBorrowedByUser(UserBLL user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return user.BorrowedDocumentTitles
                .Select(title => _documentService.GetDocument(title))
                .Where(d => d != null)
                .Cast<DocumentBLL>()
                .ToList();
        }

        public string GetDocumentStatus(DocumentBLL doc)
        {
            if (doc == null)
                throw new ArgumentNullException(nameof(doc));

            return doc.IsBorrowed ? $"Документ виданий: {doc.BorrowedByName}" : "Документ у бібліотеці";
        }
    }
}