using System;
using System.Collections.Generic;
using System.Linq;
using DAL.DataProvider;
using DAL.Entities;
using BLL.Models;
using BLL.Exceptions;
using BLL.Validation;
using DAL.Interfaces;

namespace BLL.Services
{
    public class DocumentService
    {
        private readonly IDataProvider<Document> _documentProvider;
        private List<DocumentBLL> _documents;

        public DocumentService(IDataProvider<Document> documentProvider)
        {
            _documentProvider = documentProvider ?? throw new ArgumentNullException(nameof(documentProvider));
            try
            {
                _documents = _documentProvider.Load().Select(ConvertToBLL).ToList();
            }
            catch (Exception ex)
            {
                throw new LibraryException($"Помилка завантаження документів: {ex.Message}");
            }
        }

        private DocumentBLL ConvertToBLL(Document d) => new DocumentBLL
        {
            Title = d.Title,
            Author = d.Author,
            IsBorrowed = d.IsBorrowed,
            BorrowedByName = d.BorrowedByName
        };

        private Document ConvertToDAL(DocumentBLL d) => new Document(d.Title, d.Author)
        {
            IsBorrowed = d.IsBorrowed,
            BorrowedByName = d.BorrowedByName
        };

        private void Save()
        {
            try
            {
                _documentProvider.Save(_documents.Select(ConvertToDAL).ToList());
            }
            catch (Exception ex)
            {
                throw new LibraryException($"Помилка збереження документів: {ex.Message}");
            }
        }

        public void AddDocument(DocumentBLL doc)
        {
            DocumentValidator.Validate(doc);

            if (_documents.Any(d => d.Title.Equals(doc.Title, StringComparison.OrdinalIgnoreCase)))
            {
                throw new ValidationException($"Документ з назвою '{doc.Title}' вже існує");
            }

            _documents.Add(doc);
            Save();
        }

        public void RemoveDocument(DocumentBLL doc)
        {
            if (doc == null)
                throw new ArgumentNullException(nameof(doc));

            if (!_documents.Contains(doc))
                throw new EntityNotFoundException("Документ не знайдено");

            if (doc.IsBorrowed)
                throw new LibraryException("Неможливо видалити виданий документ");

            _documents.Remove(doc);
            Save();
        }

        public void UpdateDocument(DocumentBLL oldDoc, DocumentBLL newDoc)
        {
            if (oldDoc == null || newDoc == null)
                throw new ArgumentNullException("Документ не може бути null");

            DocumentValidator.Validate(newDoc);

            int index = _documents.IndexOf(oldDoc);
            if (index == -1)
                throw new EntityNotFoundException("Документ не знайдено");

            _documents[index] = newDoc;
            Save();
        }
        public DocumentBLL? GetDocument(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ValidationException("Назва не може бути порожньою");

            return _documents.FirstOrDefault(d => d.Title.Equals(title, StringComparison.OrdinalIgnoreCase));
        }

        public List<DocumentBLL> GetAllDocuments() => _documents;
        public List<DocumentBLL> SortByTitle() => _documents.OrderBy(d => d.Title).ToList();
        public List<DocumentBLL> SortByAuthor() => _documents.OrderBy(d => d.Author).ToList();
    }
}