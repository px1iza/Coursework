using System;
using BLL.Models;
using BLL.Services;
using BLL.Exceptions;

namespace PL
{
    class Program
    {
        static UserService userService = ServiceFactory.CreateUserService();
        static DocumentService documentService = ServiceFactory.CreateDocumentService();
        static LibraryService libraryService = ServiceFactory.CreateLibraryService();
        static void Main(string[] args)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Бібліотечна система ===");
                Console.WriteLine("1. Керування користувачами");
                Console.WriteLine("2. Керування документами");
                Console.WriteLine("3. Видача документів");
                Console.WriteLine("4. Пошук");
                Console.WriteLine("0. Вихід");
                Console.Write("Виберіть дію: ");

                string choice = Console.ReadLine() ?? "";
                switch (choice)
                {
                    case "1": UserMenu(); break;
                    case "2": DocumentMenu(); break;
                    case "3": LibraryMenu(); break;
                    case "4": SearchMenu(); break;
                    case "0": return;
                    default: Console.WriteLine("Невірний вибір."); break;
                }
                Console.WriteLine("\nНатисніть Enter...");
                Console.ReadLine();
            }
        }

        static void UserMenu()
        {
            Console.Clear();
            Console.WriteLine("=== Користувачі ===");
            Console.WriteLine("1. Додати користувача");
            Console.WriteLine("2. Видалити користувача");
            Console.WriteLine("3. Змінити дані користувача");
            Console.WriteLine("4. Переглянути користувача");
            Console.WriteLine("5. Переглянути всіх");
            Console.WriteLine("6. Сортувати за ім'ям");
            Console.WriteLine("7. Сортувати за прізвищем");
            Console.WriteLine("8. Сортувати за групою");
            Console.Write("Виберіть дію: ");
            string choice = Console.ReadLine() ?? "";

            try
            {
                switch (choice)
                {
                    case "1": AddUser(); break;
                    case "2": RemoveUser(); break;
                    case "3": UpdateUser(); break;
                    case "4": ViewUser(); break;
                    case "5": ViewAllUsers(); break;
                    case "6": ViewSortedUsers("firstName"); break;
                    case "7": ViewSortedUsers("lastName"); break;
                    case "8": ViewSortedUsers("group"); break;
                    default: Console.WriteLine("Невірний вибір"); break;
                }
            }
            catch (LibraryException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Помилка: {ex.Message}");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Непередбачена помилка: {ex.Message}");
                Console.ResetColor();
            }
        }

        static void AddUser()
        {
            Console.Write("Ім'я: ");
            string firstName = Console.ReadLine() ?? "";
            Console.Write("Прізвище: ");
            string lastName = Console.ReadLine() ?? "";
            Console.Write("Група: ");
            if (!int.TryParse(Console.ReadLine(), out int group))
            {
                Console.WriteLine("Група має бути числом");
                return;
            }

            var user = new UserBLL { FirstName = firstName, LastName = lastName, Group = group };
            userService.AddUser(user);
            Console.WriteLine("✓ Користувача додано");
        }

        static void RemoveUser()
        {
            var user = FindUserByInput();
            if (user != null)
            {
                userService.RemoveUser(user);
                Console.WriteLine("✓ Користувача видалено");
            }
        }

        static void UpdateUser()
        {
            var oldUser = FindUserByInput();
            if (oldUser != null)
            {
                Console.Write("Нове ім'я: ");
                string firstName = Console.ReadLine() ?? "";
                Console.Write("Нове прізвище: ");
                string lastName = Console.ReadLine() ?? "";
                Console.Write("Нова група: ");
                if (!int.TryParse(Console.ReadLine(), out int group))
                {
                    Console.WriteLine("Група має бути числом");
                    return;
                }

                var updatedUser = new UserBLL
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Group = group,
                    BorrowedDocumentTitles = oldUser.BorrowedDocumentTitles
                };

                userService.UpdateUser(oldUser, updatedUser);
                Console.WriteLine("✓ Дані оновлено");
            }
        }

        static void ViewUser()
        {
            var user = FindUserByInput();
            if (user != null)
            {
                Console.WriteLine($"Ім'я: {user.FirstName}, Прізвище: {user.LastName}, Група: {user.Group}");
                if (user.BorrowedDocumentTitles.Count > 0)
                {
                    Console.WriteLine("Взяті документи:");
                    foreach (var title in user.BorrowedDocumentTitles)
                        Console.WriteLine($"- {title}");
                }
                else Console.WriteLine("Немає взятих документів");
            }
        }

        static void ViewAllUsers()
        {
            var users = userService.GetAllUsers();
            foreach (var u in users)
                Console.WriteLine($"{u.FirstName} {u.LastName}, Група: {u.Group}");
        }

        static void ViewSortedUsers(string sortBy)
        {
            var users = sortBy switch
            {
                "firstName" => userService.SortByFirstName(),
                "lastName" => userService.SortByLastName(),
                "group" => userService.SortByGroup(),
                _ => userService.GetAllUsers()
            };
            foreach (var u in users)
                Console.WriteLine($"{u.FirstName} {u.LastName}, Група: {u.Group}");
        }

        static UserBLL? FindUserByInput()
        {
            try
            {
                Console.Write("Ім'я: ");
                string firstName = Console.ReadLine() ?? "";
                Console.Write("Прізвище: ");
                string lastName = Console.ReadLine() ?? "";
                var user = userService.GetUser(firstName, lastName);
                if (user == null)
                    Console.WriteLine("Користувача не знайдено");
                return user;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка: {ex.Message}");
                return null;
            }
        }

        static void DocumentMenu()
        {
            Console.Clear();
            Console.WriteLine("=== Документи ===");
            Console.WriteLine("1. Додати");
            Console.WriteLine("2. Видалити");
            Console.WriteLine("3. Змінити");
            Console.WriteLine("4. Переглянути");
            Console.WriteLine("5. Всі");
            Console.WriteLine("6. Сортувати за назвою");
            Console.WriteLine("7. Сортувати за автором");
            Console.Write("Виберіть: ");
            string choice = Console.ReadLine() ?? "";

            try
            {
                switch (choice)
                {
                    case "1": AddDocument(); break;
                    case "2": RemoveDocument(); break;
                    case "3": UpdateDocument(); break;
                    case "4": ViewDocument(); break;
                    case "5": ViewAllDocuments(); break;
                    case "6": ViewSortedDocuments("title"); break;
                    case "7": ViewSortedDocuments("author"); break;
                }
            }
            catch (LibraryException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Помилка: {ex.Message}");
                Console.ResetColor();
            }
        }

        static void AddDocument()
        {
            Console.Write("Назва: ");
            string title = Console.ReadLine() ?? "";
            Console.Write("Автор: ");
            string author = Console.ReadLine() ?? "";
            var doc = new DocumentBLL { Title = title, Author = author };
            documentService.AddDocument(doc);
            Console.WriteLine("✓ Додано");
        }

        static void RemoveDocument()
        {
            var doc = FindDocumentByInput();
            if (doc != null)
            {
                documentService.RemoveDocument(doc);
                Console.WriteLine("✓ Видалено");
            }
        }

        static void UpdateDocument()
        {
            var oldDoc = FindDocumentByInput();
            if (oldDoc != null)
            {
                Console.Write("Нова назва: ");
                string title = Console.ReadLine() ?? "";
                Console.Write("Новий автор: ");
                string author = Console.ReadLine() ?? "";
                var newDoc = new DocumentBLL
                {
                    Title = title,
                    Author = author,
                    IsBorrowed = oldDoc.IsBorrowed,
                    BorrowedByName = oldDoc.BorrowedByName
                };
                documentService.UpdateDocument(oldDoc, newDoc);
                Console.WriteLine("✓ Оновлено");
            }
        }

        static void ViewDocument()
        {
            var doc = FindDocumentByInput();
            if (doc != null)
            {
                string status = doc.IsBorrowed ? $"Виданий {doc.BorrowedByName}" : "У бібліотеці";
                Console.WriteLine($"{doc.Title} - {doc.Author}, {status}");
            }
        }

        static void ViewAllDocuments()
        {
            var docs = documentService.GetAllDocuments();
            foreach (var d in docs)
            {
                string status = d.IsBorrowed ? $"(Виданий {d.BorrowedByName})" : "(У бібліотеці)";
                Console.WriteLine($"{d.Title} - {d.Author} {status}");
            }
        }

        static void ViewSortedDocuments(string sortBy)
        {
            var docs = sortBy == "title" ? documentService.SortByTitle() : documentService.SortByAuthor();
            foreach (var d in docs)
                Console.WriteLine($"{d.Title} - {d.Author}");
        }

        static DocumentBLL? FindDocumentByInput()
        {
            try
            {
                Console.Write("Назва: ");
                string title = Console.ReadLine() ?? "";
                var doc = documentService.GetDocument(title);
                if (doc == null)
                    Console.WriteLine("Документ не знайдено");
                return doc;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка: {ex.Message}");
                return null;
            }
        }

        static void LibraryMenu()
        {
            Console.Clear();
            Console.WriteLine("=== Видача ===");
            Console.WriteLine("1. Взяти");
            Console.WriteLine("2. Повернути");
            Console.WriteLine("3. Документи користувача");
            Console.WriteLine("4. Статус документа");
            Console.Write("Виберіть: ");
            string choice = Console.ReadLine() ?? "";

            try
            {
                switch (choice)
                {
                    case "1": BorrowDocument(); break;
                    case "2": ReturnDocument(); break;
                    case "3": DocumentsOfUser(); break;
                    case "4": DocumentStatus(); break;
                }
            }
            catch (LibraryException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Помилка: {ex.Message}");
                Console.ResetColor();
            }
        }

        static void BorrowDocument()
        {
            var user = FindUserByInput();
            var doc = FindDocumentByInput();
            if (user != null && doc != null)
            {
                libraryService.BorrowDocument(user, doc);
                Console.WriteLine("✓ Видано");
            }
        }

        static void ReturnDocument()
        {
            var user = FindUserByInput();
            var doc = FindDocumentByInput();
            if (user != null && doc != null)
            {
                libraryService.ReturnDocument(user, doc);
                Console.WriteLine("✓ Повернено");
            }
        }

        static void DocumentsOfUser()
        {
            var user = FindUserByInput();
            if (user != null)
            {
                var docs = libraryService.GetBorrowedByUser(user);
                if (docs.Count == 0) Console.WriteLine("Немає документів");
                else foreach (var d in docs) Console.WriteLine($"{d.Title} - {d.Author}");
            }
        }

        static void DocumentStatus()
        {
            var doc = FindDocumentByInput();
            if (doc != null)
                Console.WriteLine(libraryService.GetDocumentStatus(doc));
        }

        static void SearchMenu()
        {
            Console.Clear();
            Console.WriteLine("=== Пошук ===");
            Console.WriteLine("1. Користувачі");
            Console.WriteLine("2. Документи");
            Console.Write("Виберіть: ");
            string choice = Console.ReadLine() ?? "";

            switch (choice)
            {
                case "1": SearchUsers(); break;
                case "2": SearchDocuments(); break;
            }
        }

        static void SearchUsers()
        {
            Console.Write("Ключове слово: ");
            string key = Console.ReadLine() ?? "";
            var results = userService.GetAllUsers().FindAll(u =>
                u.FirstName.Contains(key, StringComparison.OrdinalIgnoreCase) ||
                u.LastName.Contains(key, StringComparison.OrdinalIgnoreCase));
            if (results.Count == 0) Console.WriteLine("Не знайдено");
            else foreach (var u in results) Console.WriteLine($"{u.FirstName} {u.LastName}");
        }

        static void SearchDocuments()
        {
            Console.Write("Ключове слово: ");
            string key = Console.ReadLine() ?? "";
            var results = documentService.GetAllDocuments().FindAll(d =>
                d.Title.Contains(key, StringComparison.OrdinalIgnoreCase) ||
                d.Author.Contains(key, StringComparison.OrdinalIgnoreCase));
            if (results.Count == 0) Console.WriteLine("Не знайдено");
            else foreach (var d in results) Console.WriteLine($"{d.Title} - {d.Author}");
        }
    }
}