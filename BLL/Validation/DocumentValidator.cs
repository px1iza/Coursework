using System;
using BLL.Exceptions;
using BLL.Models;

namespace BLL.Validation
{
    public static class DocumentValidator
    {
        public static void Validate(DocumentBLL document)
        {
            if (document == null)
                throw new ValidationException("Документ не може бути null");

            if (string.IsNullOrWhiteSpace(document.Title))
                throw new ValidationException("Назва документа не може бути порожньою");

            if (document.Title.Length < 2)
                throw new ValidationException("Назва має містити мінімум 2 символи");

            if (string.IsNullOrWhiteSpace(document.Author))
                throw new ValidationException("Автор не може бути порожнім");

            if (document.Author.Length < 2)
                throw new ValidationException("Ім'я автора має містити мінімум 2 символи");
        }
    }
}