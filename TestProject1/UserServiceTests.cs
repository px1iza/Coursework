using BLL.Services;
using BLL.Exceptions;
using BLL.Models;
using DAL.Interfaces;
using DAL.Entities;

namespace TestProject1;

public class UserServiceTests
{
    // Допоміжний метод для створення ізольованого сервісу
    private UserService CreateIsolatedService()
    {
        string tempDir = Path.Combine(Path.GetTempPath(), "LibraryTest_" + Guid.NewGuid());
        Directory.CreateDirectory(tempDir);
        string usersPath = Path.Combine(tempDir, "users.json");

        IDataProvider<User> provider = new TestUserProvider(usersPath);
        return new UserService(provider);
    }

    //  ТЕСТИ НА ДОДАВАННЯ КОРИСТУВАЧА (100% покриття)

    [Fact]
    public void AddUser_ValidUser_AddsSuccessfully()
    {
        var service = CreateIsolatedService();
        var user = new UserBLL
        {
            FirstName = "Іван",
            LastName = "Петренко",
            Group = 1
        };

        service.AddUser(user);

        var allUsers = service.GetAllUsers();
        Assert.Single(allUsers);
        Assert.Equal("Іван", allUsers[0].FirstName);
    }

    [Fact]
    public void AddUser_EmptyFirstName_ThrowsValidationException()
    {
        var service = CreateIsolatedService();
        var user = new UserBLL
        {
            FirstName = "",
            LastName = "Петренко",
            Group = 1
        };

        Assert.Throws<ValidationException>(() => service.AddUser(user));
    }

    [Fact]
    public void AddUser_ShortFirstName_ThrowsValidationException()
    {
        var service = CreateIsolatedService();
        var user = new UserBLL
        {
            FirstName = "І",
            LastName = "Петренко",
            Group = 1
        };

        Assert.Throws<ValidationException>(() => service.AddUser(user));
    }

    [Fact]
    public void AddUser_EmptyLastName_ThrowsValidationException()
    {
        var service = CreateIsolatedService();
        var user = new UserBLL
        {
            FirstName = "Іван",
            LastName = "",
            Group = 1
        };
        Assert.Throws<ValidationException>(() => service.AddUser(user));
    }

    [Fact]
    public void AddUser_InvalidGroup_ThrowsValidationException()
    {
        var service = CreateIsolatedService();
        var user = new UserBLL
        {
            FirstName = "Іван",
            LastName = "Петренко",
            Group = 0
        };

        Assert.Throws<ValidationException>(() => service.AddUser(user));
    }

    [Fact]
    public void AddUser_DuplicateUser_ThrowsValidationException()
    {
        var service = CreateIsolatedService();
        var user1 = new UserBLL
        {
            FirstName = "Іван",
            LastName = "Петренко",
            Group = 1
        };
        service.AddUser(user1);

        var user2 = new UserBLL
        {
            FirstName = "Іван",
            LastName = "Петренко",
            Group = 2
        };

        Assert.Throws<ValidationException>(() => service.AddUser(user2));
    }

    //  ТЕСТИ НА ВИДАЛЕННЯ (50% покриття) 

    [Fact]
    public void RemoveUser_ExistingUser_RemovesSuccessfully()
    {

        var service = CreateIsolatedService();
        var user = new UserBLL
        {
            FirstName = "Іван",
            LastName = "Петренко",
            Group = 1
        };
        service.AddUser(user);

        service.RemoveUser(user);

        Assert.Empty(service.GetAllUsers());
    }

    [Fact]
    public void RemoveUser_UserWithBorrowedDocuments_ThrowsLibraryException()
    {

        var service = CreateIsolatedService();
        var user = new UserBLL
        {
            FirstName = "Іван",
            LastName = "Петренко",
            Group = 1,
            BorrowedDocumentTitles = new List<string> { "Книга" }
        };
        service.AddUser(user);


        Assert.Throws<LibraryException>(() => service.RemoveUser(user));
    }

    // ТЕСТИ НА ОНОВЛЕННЯ (50% покриття) 

    [Fact]
    public void UpdateUser_ValidData_UpdatesSuccessfully()
    {

        var service = CreateIsolatedService();
        var oldUser = new UserBLL
        {
            FirstName = "Іван",
            LastName = "Петренко",
            Group = 1
        };
        service.AddUser(oldUser);

        var updatedUser = new UserBLL
        {
            FirstName = "Петро",
            LastName = "Іваненко",
            Group = 2
        };


        service.UpdateUser(oldUser, updatedUser);


        var result = service.GetAllUsers()[0];
        Assert.Equal("Петро", result.FirstName);
        Assert.Equal("Іваненко", result.LastName);
    }

    //  ТЕСТИ НА ПОШУК (50% покриття) 

    [Fact]
    public void GetUser_ExistingUser_ReturnsUser()
    {
        var service = CreateIsolatedService();
        var user = new UserBLL
        {
            FirstName = "Іван",
            LastName = "Петренко",
            Group = 1
        };
        service.AddUser(user);

        var result = service.GetUser("Іван", "Петренко");


        Assert.NotNull(result);
        Assert.Equal("Іван", result.FirstName);
    }

    [Fact]
    public void GetUser_NonExistingUser_ReturnsNull()
    {

        var service = CreateIsolatedService();


        var result = service.GetUser("Неіснуючий", "Користувач");


        Assert.Null(result);
    }

    //ТЕСТИ НА СОРТУВАННЯ (50% покриття) 

    [Fact]
    public void SortByFirstName_MultipleUsers_ReturnsSortedList()
    {
        // Arrange
        var service = CreateIsolatedService();
        service.AddUser(new UserBLL { FirstName = "Liza", LastName = "Rabirokh", Group = 1 });
        service.AddUser(new UserBLL { FirstName = "Roman", LastName = "Rabirokh", Group = 1 });

        // Act
        var result = service.SortByFirstName();

        // Assert
        Assert.Equal("Liza", result[0].FirstName);
        Assert.Equal("Roman", result[1].FirstName);
    }

    [Fact]
    public void SortByGroup_MultipleUsers_ReturnsSortedList()
    {

        var service = CreateIsolatedService();
        service.AddUser(new UserBLL { FirstName = "Liza", LastName = "Rabirokh", Group = 3 });
        service.AddUser(new UserBLL { FirstName = "Roman", LastName = "Rabirokh", Group = 1 });

        var result = service.SortByGroup();

        Assert.Equal(1, result[0].Group);
        Assert.Equal(3, result[1].Group);
    }
}

// Тестовий провайдер (замість реального файлу)
public class TestUserProvider : IDataProvider<User>
{
    private List<User> _data = new();
    private readonly string _path;

    public TestUserProvider(string path)
    {
        _path = path;
    }

    public void Save(List<User> items)
    {
        _data = items.Select(u => new User
        {
            FirstName = u.FirstName,
            LastName = u.LastName,
            Group = u.Group,
            BorrowedDocumentTitles = new List<string>(u.BorrowedDocumentTitles)
        }).ToList();
    }

    public List<User> Load()
    {
        return _data.Select(u => new User
        {
            FirstName = u.FirstName,
            LastName = u.LastName,
            Group = u.Group,
            BorrowedDocumentTitles = new List<string>(u.BorrowedDocumentTitles)
        }).ToList();
    }
}