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
    public class UserService
    {
        private readonly IDataProvider<User> _userProvider;
        private List<UserBLL> _users;

        public UserService(IDataProvider<User> userProvider)
        {
            _userProvider = userProvider ?? throw new ArgumentNullException(nameof(userProvider));
            try
            {
                _users = _userProvider.Load().Select(ConvertToBLL).ToList();
            }
            catch (Exception ex)
            {
                throw new LibraryException($"Помилка завантаження користувачів: {ex.Message}");
            }
        }

        private UserBLL ConvertToBLL(User u) => new UserBLL
        {
            FirstName = u.FirstName,
            LastName = u.LastName,
            Group = u.Group,
            BorrowedDocumentTitles = new List<string>(u.BorrowedDocumentTitles)
        };

        private User ConvertToDAL(UserBLL u) => new User(u.FirstName, u.LastName, u.Group)
        {
            BorrowedDocumentTitles = new List<string>(u.BorrowedDocumentTitles)
        };

        private void Save()
        {
            try
            {
                _userProvider.Save(_users.Select(ConvertToDAL).ToList());
            }
            catch (Exception ex)
            {
                throw new LibraryException($"Помилка збереження користувачів: {ex.Message}");
            }
        }

        public void AddUser(UserBLL user)
        {
            UserValidator.Validate(user);

            if (_users.Any(u => u.FirstName.Equals(user.FirstName, StringComparison.OrdinalIgnoreCase) &&
                               u.LastName.Equals(user.LastName, StringComparison.OrdinalIgnoreCase)))
            {
                throw new ValidationException($"Користувач {user.FirstName} {user.LastName} вже існує");
            }

            _users.Add(user);
            Save();
        }

        public void RemoveUser(UserBLL user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (!_users.Contains(user))
                throw new EntityNotFoundException("Користувача не знайдено");

            if (user.BorrowedDocumentTitles.Count > 0)
                throw new LibraryException("Неможливо видалити користувача, який має взяті документи");

            _users.Remove(user);
            Save();
        }

        public void UpdateUser(UserBLL oldUser, UserBLL updatedUser)
        {
            if (oldUser == null || updatedUser == null)
                throw new ArgumentNullException("Користувач не може бути null");

            UserValidator.Validate(updatedUser);

            int index = _users.IndexOf(oldUser);
            if (index == -1)
                throw new EntityNotFoundException("Користувача не знайдено");

            _users[index] = updatedUser;
            Save();
        }

        public UserBLL? GetUser(string firstName, string lastName)
        {
            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
                throw new ValidationException("Ім'я та прізвище не можуть бути порожніми");

            return _users.FirstOrDefault(u =>
                u.FirstName.Equals(firstName, StringComparison.OrdinalIgnoreCase) &&
                u.LastName.Equals(lastName, StringComparison.OrdinalIgnoreCase));
        }

        public List<UserBLL> GetAllUsers() => _users;
        public List<UserBLL> SortByFirstName() => _users.OrderBy(u => u.FirstName).ToList();
        public List<UserBLL> SortByLastName() => _users.OrderBy(u => u.LastName).ToList();
        public List<UserBLL> SortByGroup() => _users.OrderBy(u => u.Group).ToList();
    }
}