using System;
using System.Collections.Generic;
using System.Linq;
using DAL.DataProvider;
using DAL.Entities;
using BLL.Models;

namespace BLL.Services
{
    public class UserService
    {
        private readonly IDataProvider<User> _userProvider;
        private List<UserBLL> _users;

        public UserService(IDataProvider<User> userProvider)
        {
            _userProvider = userProvider;
            _users = _userProvider.Load().Select(ConvertToBLL).ToList();
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
            _userProvider.Save(_users.Select(ConvertToDAL).ToList());
        }

        public void AddUser(UserBLL user)
        {
            _users.Add(user);
            Save();
        }

        public void RemoveUser(UserBLL user)
        {
            _users.Remove(user);
            Save();
        }

        public void UpdateUser(UserBLL oldUser, UserBLL updatedUser)
        {
            int index = _users.IndexOf(oldUser);
            if (index != -1)
            {
                _users[index] = updatedUser;
                Save();
            }
        }

        public UserBLL? GetUser(string firstName, string lastName)
        {
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