using System;
using System.Collections.Generic;
using BLL.Models;
using BLL.Services;
using DAL.DataProvider;

namespace PL
{
    class Program
    {
        static UserService userService = new UserService(new JsonUserDataProvider());
        static DocumentService documentService = new DocumentService(new JsonDocumentDataProvider());
        static LibraryService libraryService = new LibraryService(userService, documentService);

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

                string choice = Console.ReadLine()!;
                switch (choice)
                {
                    case "1": UserMenu(); break;
                    case "2": DocumentMenu(); break;
                    case "3": LibraryMenu(); break;
                    case "4": SearchMenu(); break;
                    case "0": return;
                    default: Console.WriteLine("Невірний вибір."); break;
                }
                Console.WriteLine("\nНатисніть Enter для продовження...");
                Console.ReadLine();
            }
        }

        // ================= Користувачі =================
        static void UserMenu()
        {
            Console.Clear();
            Console.WriteLine("=== Користувачі ===");
            Console.WriteLine("1. Додати користувача");
            Console.WriteLine("2. Видалити користувача");
            Console.WriteLine("3. Змінити дані користувача");
            Console.WriteLine("4. Переглянути конкретного користувача");
            Console.WriteLine("5. Переглянути всіх користувачів");
            Console.Write("Виберіть дію: ");
            string choice = Console.ReadLine()!;

            switch (choice)
            {
                case "1": AddUser(); break;
                case "2": RemoveUser(); break;
                case "3": UpdateUser(); break;
                case "4": ViewUser(); break;
                case "5": ViewAllUsers(); break;
                default: Console.WriteLine("Невірний вибір"); break;
            }
        }

        static void AddUser()
        {
            Console.Write("Ім'я: ");
            string firstName = Console.ReadLine()!;
            Console.Write("Прізвище: ");
            string lastName = Console.ReadLine()!;
            Console.Write("Група (число): ");
            int group = int.TryParse(Console.ReadLine(), out int g) ? g : 0;

            var user = new UserBLL
            {
                FirstName = firstName,
                LastName = lastName,
                Group = group
            };

            userService.AddUser(user);
            Console.WriteLine("Користувача додано.");
        }

        static void RemoveUser()
        {
            var user = FindUserByInput();
            if (user != null)
            {
                userService.RemoveUser(user);
                Console.WriteLine("Користувача видалено.");
            }
            else Console.WriteLine("Користувача не знайдено.");
        }

        static void UpdateUser()
        {
            var oldUser = FindUserByInput();
            if (oldUser != null)
            {
                Console.Write("Нове ім'я: ");
                string firstName = Console.ReadLine()!;
                Console.Write("Нове прізвище: ");
                string lastName = Console.ReadLine()!;
                Console.Write("Нова група: ");
                int group = int.TryParse(Console.ReadLine(), out int g) ? g : oldUser.Group;

                var updatedUser = new UserBLL
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Group = group,
                    BorrowedDocumentTitles = oldUser.BorrowedDocumentTitles
                };

                userService.UpdateUser(oldUser, updatedUser);
                Console.WriteLine("Дані користувача оновлено.");
            }
            else Console.WriteLine("Користувача не знайдено.");
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
            }
            else Console.WriteLine("Користувача не знайдено.");
        }

        static void ViewAllUsers()
        {
            var users = userService.GetAllUsers();
            Console.WriteLine("Список користувачів:");
            foreach (var u in users)
            {
                Console.WriteLine($"{u.FirstName} {u.LastName}, Група: {u.Group}");
            }
        }

        static UserBLL? FindUserByInput()
        {
            Console.Write("Ім'я користувача: ");
            string firstName = Console.ReadLine()!;
            Console.Write("Прізвище користувача: ");
            string lastName = Console.ReadLine()!;
            return userService.GetUser(firstName, lastName);
        }

        // ================= Документи =================
        static void DocumentMenu()
        {
            Console.Clear();
            Console.WriteLine("=== Документи ===");
            Console.WriteLine("1. Додати документ");
            Console.WriteLine("2. Видалити документ");
            Console.WriteLine("3. Змінити документ");
            Console.WriteLine("4. Переглянути документ");
            Console.WriteLine("5. Переглянути всі документи");
            Console.Write("Виберіть дію: ");
            string choice = Console.ReadLine()!;

            switch (choice)
            {
                case "1": AddDocument(); break;
                case "2": RemoveDocument(); break;
                case "3": UpdateDocument(); break;
                case "4": ViewDocument(); break;
                case "5": ViewAllDocuments(); break;
                default: Console.WriteLine("Невірний вибір"); break;
            }
        }

        static void AddDocument()
        {
            Console.Write("Назва: ");
            string title = Console.ReadLine()!;
            Console.Write("Автор: ");
            string author = Console.ReadLine()!;

            var doc = new DocumentBLL
            {
                Title = title,
                Author = author,
                IsBorrowed = false
            };

            documentService.AddDocument(doc);
            Console.WriteLine("Документ додано.");
        }

        static void RemoveDocument()
        {
            var doc = FindDocumentByInput();
            if (doc != null)
            {
                documentService.RemoveDocument(doc);
                Console.WriteLine("Документ видалено.");
            }
            else Console.WriteLine("Документ не знайдено.");
        }

        static void UpdateDocument()
        {
            var oldDoc = FindDocumentByInput();
            if (oldDoc != null)
            {
                Console.Write("Нова назва: ");
                string title = Console.ReadLine()!;
                Console.Write("Новий автор: ");
                string author = Console.ReadLine()!;

                var newDoc = new DocumentBLL
                {
                    Title = title,
                    Author = author,
                    IsBorrowed = oldDoc.IsBorrowed,
                    BorrowedByName = oldDoc.BorrowedByName
                };

                documentService.UpdateDocument(oldDoc, newDoc);
                Console.WriteLine("Документ оновлено.");
            }
            else Console.WriteLine("Документ не знайдено.");
        }

        static void ViewDocument()
        {
            var doc = FindDocumentByInput();
            if (doc != null)
            {
                string status = doc.IsBorrowed ? $"Виданий {doc.BorrowedByName}" : "У бібліотеці";
                Console.WriteLine($"Назва: {doc.Title}, Автор: {doc.Author}, Статус: {status}");
            }
            else Console.WriteLine("Документ не знайдено.");
        }

        static void ViewAllDocuments()
        {
            var docs = documentService.GetAllDocuments();
            Console.WriteLine("Список документів:");
            foreach (var d in docs)
            {
                string status = d.IsBorrowed ? $"(Виданий {d.BorrowedByName})" : "(У бібліотеці)";
                Console.WriteLine($"{d.Title} - {d.Author} {status}");
            }
        }

        static DocumentBLL? FindDocumentByInput()
        {
            Console.Write("Назва документа: ");
            string title = Console.ReadLine()!;
            return documentService.GetDocument(title);
        }

        // ================= Видача документів =================
        static void LibraryMenu()
        {
            Console.Clear();
            Console.WriteLine("=== Видача документів ===");
            Console.WriteLine("1. Взяти документ");
            Console.WriteLine("2. Повернути документ");
            Console.WriteLine("3. Документи користувача");
            Console.WriteLine("4. Статус документа");
            Console.Write("Виберіть дію: ");
            string choice = Console.ReadLine()!;

            switch (choice)
            {
                case "1": BorrowDocument(); break;
                case "2": ReturnDocument(); break;
                case "3": DocumentsOfUser(); break;
                case "4": DocumentStatus(); break;
                default: Console.WriteLine("Невірний вибір"); break;
            }
        }

        static void BorrowDocument()
        {
            var user = FindUserByInput();
            var doc = FindDocumentByInput();
            if (user != null && doc != null)
            {
                if (libraryService.BorrowDocument(user, doc))
                    Console.WriteLine("Документ видано.");
                else
                    Console.WriteLine("Не вдалося видати документ (можливо вже виданий або ліміт користувача 5 книг).");
            }
            else Console.WriteLine("Користувач або документ не знайдено.");
        }

        static void ReturnDocument()
        {
            var user = FindUserByInput();
            var doc = FindDocumentByInput();
            if (user != null && doc != null)
            {
                libraryService.ReturnDocument(user, doc);
                Console.WriteLine("Документ повернено.");
            }
            else Console.WriteLine("Користувач або документ не знайдено.");
        }

        static void DocumentsOfUser()
        {
            var user = FindUserByInput();
            if (user != null)
            {
                var docs = libraryService.GetBorrowedByUser(user);
                if (docs.Count == 0) Console.WriteLine("Користувач не має документів.");
                else foreach (var d in docs) Console.WriteLine($"{d.Title} - {d.Author}");
            }
            else Console.WriteLine("Користувача не знайдено.");
        }

        static void DocumentStatus()
        {
            var doc = FindDocumentByInput();
            if (doc != null)
            {
                Console.WriteLine(libraryService.GetDocumentStatus(doc));
            }
            else Console.WriteLine("Документ не знайдено.");
        }

        // ================= Пошук =================
        static void SearchMenu()
        {
            Console.Clear();
            Console.WriteLine("=== Пошук ===");
            Console.WriteLine("1. Пошук серед користувачів");
            Console.WriteLine("2. Пошук серед документів");
            Console.Write("Виберіть дію: ");
            string choice = Console.ReadLine()!;

            switch (choice)
            {
                case "1": SearchUsers(); break;
                case "2": SearchDocuments(); break;
                default: Console.WriteLine("Невірний вибір."); break;
            }
        }

        static void SearchUsers()
        {
            Console.Write("Ключове слово: ");
            string key = Console.ReadLine()!;
            var results = userService.GetAllUsers().FindAll(u =>
                u.FirstName.Contains(key, StringComparison.OrdinalIgnoreCase) ||
                u.LastName.Contains(key, StringComparison.OrdinalIgnoreCase));
            if (results.Count == 0) Console.WriteLine("Нічого не знайдено.");
            else foreach (var u in results) Console.WriteLine($"{u.FirstName} {u.LastName}, Група: {u.Group}");
        }

        static void SearchDocuments()
        {
            Console.Write("Ключове слово: ");
            string key = Console.ReadLine()!;
            var results = documentService.GetAllDocuments().FindAll(d =>
                d.Title.Contains(key, StringComparison.OrdinalIgnoreCase) ||
                d.Author.Contains(key, StringComparison.OrdinalIgnoreCase));
            if (results.Count == 0) Console.WriteLine("Нічого не знайдено.");
            else foreach (var d in results) Console.WriteLine($"{d.Title} - {d.Author}");
        }
    }
}