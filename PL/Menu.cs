using System;
using BLL.Models;
using BLL.Services;
using BLL.Exceptions;

namespace PL
{
    public class Menu
    {
        private readonly UserService userService;
        private readonly DocumentService documentService;
        private readonly LibraryService libraryService;
        public Menu()
        {
            // 3. Створюємо сервіси ТІЛЬКИ ОДИН РАЗ
            userService = ServiceFactory.CreateUserService();
            documentService = ServiceFactory.CreateDocumentService();

            // 4. Передаємо ВЖЕ СТВОРЕНІ сервіси в LibraryService
            libraryService = new LibraryService(userService, documentService);
        }

        public void Run()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("СТУДЕНТСЬКА БІБЛІОТЕКА");
                Console.WriteLine("1. Керування користувачами");
                Console.WriteLine("2. Керування документами");
                Console.WriteLine("3. Видача документів");
                Console.WriteLine("4. Пошук");
                Console.WriteLine("0. Вихід");
                Console.Write("Виберіть дію: ");

                string choice = Console.ReadLine() ?? "";

                // Перенесемо try-catch сюди, щоб він ловив *всі* помилки
                try
                {
                    switch (choice)
                    {
                        case "1": UserMenu(); break;
                        case "2": DocumentMenu(); break;
                        case "3": LibraryMenu(); break;
                        case "4": SearchMenu(); break;
                        case "0": return;
                        default:
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("Невірний вибір.");
                            Console.ResetColor();
                            break;
                    }
                }
                catch (LibraryException ex) // Ловимо тільки наші кастомні помилки
                {
                    Console.WriteLine($"\nПОМИЛКА: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\nКРИТИЧНА ПОМИЛКА: {ex.Message}");
                }

                Console.WriteLine("\nНатисніть Enter...");
                Console.ReadLine();
            }
        }

        // ---------------- USER MENU ----------------

        private void UserMenu()
        {
            Console.Clear();
            Console.WriteLine("=== Користувачі ===");
            Console.WriteLine("1. Додати");
            Console.WriteLine("2. Видалити");
            Console.WriteLine("3. Змінити");
            Console.WriteLine("4. Переглянути");
            Console.WriteLine("5. Переглянути всіх");
            Console.WriteLine("6. Сортувати за ім'ям");
            Console.WriteLine("7. Сортувати за прізвищем");
            Console.WriteLine("8. Сортувати за групою");
            Console.Write("Виберіть дію: ");

            string choice = Console.ReadLine() ?? "";

            // Блок try-catch перенесено нагору, тут він не потрібен
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

        private void AddUser()
        {
            Console.Write("Ім'я: ");
            string firstName = Console.ReadLine() ?? "";
            Console.Write("Прізвище: ");
            string lastName = Console.ReadLine() ?? "";
            Console.Write("Група: ");

            if (!int.TryParse(Console.ReadLine(), out int group))
            {
                // Покращена валідація в PL (Вимога 4)
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Група має бути числом");
                Console.ResetColor();
                return;
            }

            var user = new UserBLL { FirstName = firstName, LastName = lastName, Group = group };
            userService.AddUser(user);
            Console.WriteLine("✓ Додано");
        }

        private void RemoveUser()
        {
            var user = FindUserByInput("Оберіть користувача для видалення:");
            if (user != null)
            {
                userService.RemoveUser(user);
                Console.WriteLine("✓ Видалено");
            }
        }

        private void UpdateUser()
        {
            var oldUser = FindUserByInput("Оберіть користувача для редагування:");
            if (oldUser == null) return;

            Console.WriteLine($"\nРедагування: {oldUser.FirstName} {oldUser.LastName}");
            Console.Write($"Нове ім'я (Enter, щоб лишити '{oldUser.FirstName}'): ");
            string fn = Console.ReadLine() ?? "";
            if (string.IsNullOrWhiteSpace(fn)) fn = oldUser.FirstName;

            Console.Write($"Нове прізвище (Enter, щоб лишити '{oldUser.LastName}'): ");
            string ln = Console.ReadLine() ?? "";
            if (string.IsNullOrWhiteSpace(ln)) ln = oldUser.LastName;

            Console.Write($"Нова група (Enter, щоб лишити '{oldUser.Group}'): ");
            string groupInput = Console.ReadLine() ?? "";
            int group;
            if (string.IsNullOrWhiteSpace(groupInput))
            {
                group = oldUser.Group;
            }
            else if (!int.TryParse(groupInput, out group))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Група має бути числом");
                Console.ResetColor();
                return;
            }
            var updated = new UserBLL
            {
                FirstName = fn,
                LastName = ln,
                Group = group,
                BorrowedDocumentTitles = oldUser.BorrowedDocumentTitles
            };

            userService.UpdateUser(oldUser, updated);
            Console.WriteLine("✓ Оновлено");
        }

        private void ViewUser()
        {
            var user = FindUserByInput("Оберіть користувача для перегляду:");
            if (user == null) return;

            Console.WriteLine($"\n--- Деталі ---");
            Console.WriteLine($"Ім'я: {user.FirstName}");
            Console.WriteLine($"Прізвище: {user.LastName}");
            Console.WriteLine($"Група: {user.Group}");
            Console.WriteLine($"--------------");

            if (user.BorrowedDocumentTitles.Count == 0)
                Console.WriteLine("Документів на руках немає.");
            else
            {
                Console.WriteLine("Документи на руках:");
                foreach (var t in user.BorrowedDocumentTitles)
                    Console.WriteLine($"- {t}");
            }
        }

        private void ViewAllUsers()
        {
            var users = userService.GetAllUsers();
            if (users.Count == 0)
            {
                Console.WriteLine("Жодного користувача ще не додано.");
                return;
            }
            foreach (var u in users)
                Console.WriteLine($"{u.FirstName} {u.LastName}, Група: {u.Group}");
        }

        private void ViewSortedUsers(string sortBy)
        {
            var users = sortBy switch
            {
                "firstName" => userService.SortByFirstName(),
                "lastName" => userService.SortByLastName(),
                "group" => userService.SortByGroup(),
                _ => userService.GetAllUsers()
            };

            if (users.Count == 0)
            {
                Console.WriteLine("Жодного користувача ще не додано.");
                return;
            }
            foreach (var u in users)
                Console.WriteLine($"{u.FirstName} {u.LastName}, Група: {u.Group}");
        }

        // --- ОНОВЛЕНИЙ МЕТОД ---
        // Тепер він показує список для вибору
        private UserBLL? FindUserByInput(string prompt)
        {
            var users = userService.GetAllUsers();
            if (users.Count == 0)
            {
                Console.WriteLine("Жодного користувача ще не створено.");
                return null;
            }

            Console.WriteLine(prompt);
            for (int i = 0; i < users.Count; i++)
            {
                Console.WriteLine($"  {i + 1}. {users[i].FirstName} {users[i].LastName} (Група: {users[i].Group})");
            }
            Console.Write("Ваш вибір: ");

            if (!int.TryParse(Console.ReadLine(), out int index) || index < 1 || index > users.Count)
            {
                Console.WriteLine("Невірний номер.");
                return null;
            }
            return users[index - 1];
        }
        // ---------------- DOCUMENT MENU ----------------
        private void DocumentMenu()
        {
            Console.Clear();
            Console.WriteLine("=== Документи ===");
            Console.WriteLine("1. Додати");
            Console.WriteLine("2. Видалити");
            Console.WriteLine("3. Змінити");
            Console.WriteLine("4. Переглянути (детально)");
            Console.WriteLine("5. Всі (короткий список)");
            Console.WriteLine("6. Сортувати за назвою");
            Console.WriteLine("7. Сортувати за автором");
            Console.Write("Виберіть: ");

            string choice = Console.ReadLine() ?? "";

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
        private void AddDocument()
        {
            Console.Write("Назва: ");
            string title = Console.ReadLine() ?? "";
            Console.Write("Автор: ");
            string author = Console.ReadLine() ?? "";

            var doc = new DocumentBLL { Title = title, Author = author };
            documentService.AddDocument(doc);
            Console.WriteLine(" Додано");
        }
        private void RemoveDocument()
        {
            var doc = FindDocumentByInput("Оберіть документ для видалення:");
            if (doc != null)
            {
                documentService.RemoveDocument(doc);
                Console.WriteLine("Видалено");
            }
        }
        private void UpdateDocument()
        {
            var old = FindDocumentByInput("Оберіть документ для редагування:");
            if (old == null) return;

            Console.WriteLine($"\nРедагування: {old.Title}");
            Console.Write($"Нова назва (Enter, щоб лишити '{old.Title}'): ");
            string title = Console.ReadLine() ?? "";
            if (string.IsNullOrWhiteSpace(title)) title = old.Title;

            Console.Write($"Новий автор (Enter, щоб лишити '{old.Author}'): ");
            string author = Console.ReadLine() ?? "";
            if (string.IsNullOrWhiteSpace(author)) author = old.Author;


            var updated = new DocumentBLL
            {
                Title = title,
                Author = author,
                IsBorrowed = old.IsBorrowed,
                BorrowedByName = old.BorrowedByName
            };

            documentService.UpdateDocument(old, updated);
            Console.WriteLine("Оновлено");
        }
        // --- ОНОВЛЕНИЙ МЕТОД ---
        private void ViewDocument()
        {
            var doc = FindDocumentByInput("Оберіть документ для детального перегляду:");
            if (doc == null) return;

            string status = doc.IsBorrowed ? $"Виданий {doc.BorrowedByName}" : "У бібліотеці";

            Console.WriteLine($"\n--- Деталі Документа ---");
            Console.WriteLine($"Назва:  {doc.Title}");
            Console.WriteLine($"Автор:  {doc.Author}");
            Console.WriteLine($"Статус: {status}");
            Console.WriteLine($"--------------------------");
        }

        private void ViewAllDocuments()
        {
            var docs = documentService.GetAllDocuments();
            if (docs.Count == 0)
            {
                Console.WriteLine("Жодного документа ще не додано.");
                return;
            }
            foreach (var d in docs)
            {
                string status = d.IsBorrowed ? $"(Виданий {d.BorrowedByName})" : "(У бібліотеці)";
                Console.WriteLine($"{d.Title} - {d.Author} {status}");
            }
        }

        private void ViewSortedDocuments(string sortBy)
        {
            var docs = sortBy == "title" ? documentService.SortByTitle() : documentService.SortByAuthor();
            if (docs.Count == 0)
            {
                Console.WriteLine("Жодного документа ще не додано.");
                return;
            }
            foreach (var d in docs) Console.WriteLine($"{d.Title} - {d.Author}");
        }
        // Тепер він показує список для вибору
        private DocumentBLL? FindDocumentByInput(string prompt)
        {
            var docs = documentService.GetAllDocuments();
            if (docs.Count == 0)
            {
                Console.WriteLine("У бібліотеці ще немає жодного документа.");
                return null;
            }

            Console.WriteLine(prompt);
            for (int i = 0; i < docs.Count; i++)
            {
                string status = docs[i].IsBorrowed ? "(на руках)" : "(в наявності)";
                Console.WriteLine($"  {i + 1}. {docs[i].Title} {status}");
            }
            Console.Write("Ваш вибір: ");

            if (!int.TryParse(Console.ReadLine(), out int index) || index < 1 || index > docs.Count)
            {
                Console.WriteLine("Невірний номер.");
                return null;
            }

            return docs[index - 1];
        }

        // ---------------- LIBRARY MENU ----------------

        private void LibraryMenu()
        {
            Console.Clear();
            Console.WriteLine("=== Видача ===");
            Console.WriteLine("1. Взяти");
            Console.WriteLine("2. Повернути");
            Console.WriteLine("3. Документи користувача");
            Console.WriteLine("4. Статус документа");
            Console.Write("Виберіть: ");

            string choice = Console.ReadLine() ?? "";

            switch (choice)
            {
                case "1": BorrowDocument(); break;
                case "2": ReturnDocument(); break;
                case "3": DocumentsOfUser(); break;
                case "4": DocumentStatus(); break;
            }
        }

        private void BorrowDocument()
        {
            var user = FindUserByInput("Оберіть користувача, який бере документ:");
            if (user == null) return;

            var doc = FindDocumentByInput("Оберіть документ, який хочете взяти:");
            if (doc == null) return;

            libraryService.BorrowDocument(user, doc);
            Console.WriteLine($"✓ Документ '{doc.Title}' видано користувачу {user.FirstName}.");
        }

        private void ReturnDocument()
        {
            var user = FindUserByInput("Оберіть користувача, який повертає документ:");
            if (user == null) return;

            var doc = FindDocumentByInput("Оберіть документ, який хочете повернути:");
            if (doc == null) return;

            libraryService.ReturnDocument(user, doc);
            Console.WriteLine($"Документ '{doc.Title}' повернено до бібліотеки.");
        }

        private void DocumentsOfUser()
        {
            var user = FindUserByInput("Оберіть користувача для перегляду документів:");
            if (user == null) return;

            var docs = libraryService.GetBorrowedByUser(user);
            if (docs.Count == 0) Console.WriteLine("Цей користувач не має документів на руках.");
            else foreach (var d in docs) Console.WriteLine($"{d.Title} - {d.Author}");
        }

        private void DocumentStatus()
        {
            var doc = FindDocumentByInput("Оберіть документ для перевірки статусу:");
            if (doc != null)
                Console.WriteLine(libraryService.GetDocumentStatus(doc));
        }

        // ---------------- SEARCH ----------------

        private void SearchMenu()
        {
            Console.Clear();
            Console.WriteLine("=== Пошук ===");
            Console.WriteLine("1. Користувачі");
            Console.WriteLine("2. Документи");
            Console.Write("Виберіть: ");

            string choice = Console.ReadLine() ?? "";

            if (choice == "1") SearchUsers();
            else if (choice == "2") SearchDocuments();
        }

        private void SearchUsers()
        {
            Console.Write("Ключове слово: ");
            string key = Console.ReadLine() ?? "";

            var results = userService
                .GetAllUsers()
                .FindAll(u => u.FirstName.Contains(key, StringComparison.OrdinalIgnoreCase)
                           || u.LastName.Contains(key, StringComparison.OrdinalIgnoreCase)
                           || u.Group.ToString().Contains(key));

            if (results.Count == 0) Console.WriteLine("Не знайдено");
            else foreach (var u in results) Console.WriteLine($"{u.FirstName} {u.LastName}, Група: {u.Group}");
        }

        private void SearchDocuments()
        {
            Console.Write("Ключове слово: ");
            string key = Console.ReadLine() ?? "";

            var results = documentService
                .GetAllDocuments()
                .FindAll(d => d.Title.Contains(key, StringComparison.OrdinalIgnoreCase)
                           || d.Author.Contains(key, StringComparison.OrdinalIgnoreCase));

            if (results.Count == 0) Console.WriteLine("Не знайдено");
            else foreach (var d in results) Console.WriteLine($"{d.Title} - {d.Author}");
        }
    }
}