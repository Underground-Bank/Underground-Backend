using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UndergroundBank.AccountService.Domain.Enums;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;

namespace UndergroundBank.AccountService.Application.Helpers.Validations
{
    public static class UserValidations
    {
        public static bool ValidateEmail(string email)
        {
            string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            var isValid = Regex.IsMatch(email, emailPattern);

            return isValid;
        }

        public static bool ValidatePassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
            {
                return false;
            }

            string passwordRegex = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,15}$";

            if (!Regex.IsMatch(password, passwordRegex))
            {
                return false;
            }

            return true;
        }

        public static bool ValidatePhoneNumber(string passwrod)
        {
            string passwordPatter = @"^((8|\+7)[\- ]?)?(\(?\d{3}\)?[\- ]?)?[\d\- ]{7,10}$";
            var isValid = Regex.IsMatch(passwrod, passwordPatter);

            return isValid;
        }

        public static bool ValidateName(string name)
        {
            string fullNameRegex = @"^.{2,30}$";
            return Regex.IsMatch(name, fullNameRegex);
        }

        public static bool ValidateBirthDate(DateTime birthDate)
        {
            DateOnly now = DateOnly.FromDateTime(DateTime.UtcNow);
            int age = now.Year - birthDate.Year;

            if (
                now.Month < birthDate.Month
                || (now.Month == birthDate.Month && now.Day < birthDate.Day)
            )
            {
                age--;
            }

            if (age < 18 || age > 60)
            {
                return false;
            }

            return true;
        }

        public static bool ValidateGender(Gender gender)
        {
            return Enum.IsDefined(typeof(Gender), gender);
        }

        public static string ValidateUserData(
            string name,
            string surname,
            string? password,
            string email,
            DateTime birthDate,
            string phoneNumber,
            Gender gender
        )
        {
            var validateName = ValidateName(name);
            if (!validateName)
            {
                return ("Ваше имя должно быть минимум из 2 букв!");
            }

            var validateSurname = ValidateName(surname);
            if (!validateSurname)
            {
                return ("Ваша фамилия должна быть минимум из 2 букв!");
            }

            var validateEmail = ValidateEmail(email);

            if (!validateEmail)
            {
                return ("Некорректный email");
            }

            if (password != null)
            {
                var validatePassword = ValidatePassword(password);

                if (!validatePassword)
                {
                    return (
                        "Пароль должен содержать 1 заглавную букву, 1 строчную букву, "
                        + "1 цифру, 1 спец. символ и быть не меньше 6 символов"
                    );
                }
            }

            var validateBirthDate = ValidateBirthDate(birthDate);
            if (!validateBirthDate)
            {
                return (
                    "Вы не можете зарегистрировать аккаунт, вам должно быть больше 18 и меньше 60 лет!"
                );
            }

            var validatePhoneNumber = ValidatePhoneNumber(phoneNumber);
            if (!validatePhoneNumber)
            {
                return ("Некорректный формат телефона");
            }

            var validateGender = ValidateGender(gender);
            if (!validateGender)
            {
                return ("Данного пола не существует!");
            }

            return string.Empty;
        }
    }
}
