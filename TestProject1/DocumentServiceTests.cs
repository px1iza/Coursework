using BLL.Services;
using BLL.Exceptions;
using BLL.Models;
using DAL.Interfaces;
using DAL.Entities;

namespace TestProject1;

public class DocumentServiceTests
{
    private DocumentService CreateIsolatedService()
    {
        string tempDir = Path.Combine(Path.GetTempPath(), "LibraryTest_" + Guid.NewGuid());
        Directory.CreateDirectory(tempDir);
        string docsPath = Path.Combine(tempDir, "documents.json");

        IDataProvider<Document> provider = new TestDocumentProvider(docsPath);
        return new DocumentService(provider);
    }

    [Fact]
    public void AddDocument_ValidDocument_AddsSuccessfully()
    {
        var service = CreateIsolatedService();
        var doc = new DocumentBLL
        {
            Title = "Кобзар",
            Author = "Тарас Шевченко"
        };

        service.AddDocument(doc);

        var allDocs = service.GetAllDocuments();
        Assert.Single(allDocs);
        Assert.Equal("Кобзар", allDocs[0].Title);
    }

    [Fact]
    public void AddDocument_EmptyTitle_ThrowsValidationException()
    {
        var service = CreateIsolatedService();
        var doc = new DocumentBLL
        {
            Title = "",
            Author = "Автор"
        };

        Assert.Throws<ValidationException>(() => service.AddDocument(doc));
    }

    [Fact]
    public void AddDocument_DuplicateTitle_ThrowsValidationException()
    {
        var service = CreateIsolatedService();
        var doc1 = new DocumentBLL
        {
            Title = "Кобзар",
            Author = "Автор1"
        };
        service.AddDocument(doc1);

        var doc2 = new DocumentBLL
        {
            Title = "Кобзар",
            Author = "Автор2"
        };

        Assert.Throws<ValidationException>(() => service.AddDocument(doc2));
    }

    // ТЕСТИ НА ВИДАЛЕННЯ (50% покриття) 

    [Fact]
    public void RemoveDocument_ExistingDocument_RemovesSuccessfully()
    {
        var service = CreateIsolatedService();
        var doc = new DocumentBLL
        {
            Title = "Книга",
            Author = "Автор"
        };
        service.AddDocument(doc);

        service.RemoveDocument(doc);

        Assert.Empty(service.GetAllDocuments());
    }

    [Fact]
    public void RemoveDocument_BorrowedDocument_ThrowsLibraryException()
    {
        var service = CreateIsolatedService();
        var doc = new DocumentBLL
        {
            Title = "Книга",
            Author = "Автор",
            IsBorrowed = true
        };
        service.AddDocument(doc);

        Assert.Throws<LibraryException>(() => service.RemoveDocument(doc));
    }

    //ТЕСТИ НА ПОШУК (50% покриття)

    [Fact]
    public void GetDocument_ExistingTitle_ReturnsDocument()
    {
        var service = CreateIsolatedService();
        var doc = new DocumentBLL
        {
            Title = "Кобзар",
            Author = "Шевченко"
        };
        service.AddDocument(doc);

        var result = service.GetDocument("Кобзар");

        Assert.NotNull(result);
        Assert.Equal("Кобзар", result.Title);
    }

    [Fact]
    public void GetDocument_NonExistingTitle_ReturnsNull()
    {
        var service = CreateIsolatedService();

        var result = service.GetDocument("Неіснуюча книга");

        Assert.Null(result);
    }

    [Fact]
    public void SortByTitle_MultipleDocuments_ReturnsSortedList()
    {
        var service = CreateIsolatedService();
        service.AddDocument(new DocumentBLL { Title = "Після", Author = "Анна Тодд" });
        service.AddDocument(new DocumentBLL { Title = "Четверте крило", Author = "Ребекка Яррос" });

        var result = service.SortByTitle();

        Assert.Equal("Після", result[0].Title);
        Assert.Equal("Четверте крило", result[1].Title);
    }

    //ТЕСТ НА ОНОВЛЕННЯ (50% покриття) 

    [Fact]
    public void UpdateDocument_ValidData_UpdatesSuccessfully()
    {
        var service = CreateIsolatedService();
        var oldDoc = new DocumentBLL
        {
            Title = "Стара назва",
            Author = "Старий автор"
        };
        service.AddDocument(oldDoc);

        var newDoc = new DocumentBLL
        {
            Title = "Нова назва",
            Author = "Новий автор"
        };

        service.UpdateDocument(oldDoc, newDoc);

        var result = service.GetAllDocuments()[0];
        Assert.Equal("Нова назва", result.Title);
    }
}

// Тестовий провайдер для документів
public class TestDocumentProvider : IDataProvider<Document>
{
    private List<Document> _data = new();
    private readonly string _path;

    public TestDocumentProvider(string path)
    {
        _path = path;
    }

    public void Save(List<Document> items)
    {
        _data = items.Select(d => new Document
        {
            Title = d.Title,
            Author = d.Author,
            IsBorrowed = d.IsBorrowed,
            BorrowedByName = d.BorrowedByName
        }).ToList();
    }

    public List<Document> Load()
    {
        return _data.Select(d => new Document
        {
            Title = d.Title,
            Author = d.Author,
            IsBorrowed = d.IsBorrowed,
            BorrowedByName = d.BorrowedByName
        }).ToList();
    }
}