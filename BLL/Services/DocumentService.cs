using System.Collections.Generic;
using System.Linq;
using DAL.DataProvider;
using DAL.Entities;
using BLL.Models;

namespace BLL.Services
{
    public class DocumentService
    {
        private readonly IDataProvider<Document> _documentProvider;
        private List<DocumentBLL> _documents;

        public DocumentService(IDataProvider<Document> documentProvider)
        {
            _documentProvider = documentProvider;
            _documents = _documentProvider.Load().Select(ConvertToBLL).ToList();
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
            _documentProvider.Save(_documents.Select(ConvertToDAL).ToList());
        }

        public void AddDocument(DocumentBLL doc)
        {
            _documents.Add(doc);
            Save();
        }

        public void RemoveDocument(DocumentBLL doc)
        {
            _documents.Remove(doc);
            Save();
        }

        public void UpdateDocument(DocumentBLL oldDoc, DocumentBLL newDoc)
        {
            int index = _documents.IndexOf(oldDoc);
            if (index != -1)
            {
                _documents[index] = newDoc;
                Save();
            }
        }

        public DocumentBLL? GetDocument(string title)
        {
            return _documents.FirstOrDefault(d => d.Title.Equals(title, System.StringComparison.OrdinalIgnoreCase));
        }

        public List<DocumentBLL> GetAllDocuments() => _documents;
        public List<DocumentBLL> SortByTitle() => _documents.OrderBy(d => d.Title).ToList();
        public List<DocumentBLL> SortByAuthor() => _documents.OrderBy(d => d.Author).ToList();
    }
}