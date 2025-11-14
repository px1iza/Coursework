using System;
using BLL.Exceptions;
using BLL.Models;

namespace BLL.Validation
{
    public static class UserValidator
    {
        public static void Validate(UserBLL user)
        {
            if (user == null)
                throw new ValidationException("Користувач не може бути null");

            if (string.IsNullOrWhiteSpace(user.FirstName))
                throw new ValidationException("Ім'я не може бути порожнім");

            if (user.FirstName.Length < 2)
                throw new ValidationException("Ім'я має містити мінімум 2 символи");

            if (string.IsNullOrWhiteSpace(user.LastName))
                throw new ValidationException("Прізвище не може бути порожнім");

            if (user.LastName.Length < 2)
                throw new ValidationException("Прізвище має містити мінімум 2 символи");

            if (user.Group <= 0)
                throw new ValidationException("Група має бути додатнім числом");
        }
    }
}