using BLL.Services;
using BLL.Exceptions;
using BLL.Models;
using DAL.Interfaces;
using DAL.Entities;
namespace Tests;

public class LibraryServiceTests
{
    private (LibraryService library, UserService users, DocumentService docs) CreateServices()
    {
        string tempDir = Path.Combine(Path.GetTempPath(), "LibraryTest_" + Guid.NewGuid());
        Directory.CreateDirectory(tempDir);

        var userProvider = new TestUserProvider(Path.Combine(tempDir, "users.json"));
        var docProvider = new TestDocumentProvider(Path.Combine(tempDir, "docs.json"));

        var userService = new UserService(userProvider);
        var docService = new DocumentService(docProvider);
        var libraryService = new LibraryService(userService, docService);

        return (libraryService, userService, docService);
    }
}