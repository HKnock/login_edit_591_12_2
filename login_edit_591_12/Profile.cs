using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace login_edit_591_12
{
    class Profile
    {
        public int Registration(string nickname, string password, string phone_number, int question, string answer)
        {
            phone_number = PhoneNumberConverter(phone_number);
            using (loginEditDatabaseContext db = new loginEditDatabaseContext())
            {
                if (db.UserInfos.Any(e => e.Nickname == nickname) || db.UserInfos.Any(e => e.Phone == phone_number))
                    throw new Exception("Пользователь уже зарегестрирован");
                UserInfo user = new UserInfo()
                {
                    Nickname = nickname,
                    UserPassword = password,
                    Phone = phone_number,
                    IdQuestion = question,
                    Answer = answer,
                    Birthday = null
                };
                db.UserInfos.Add(user);
                db.SaveChanges();
                return user.IdUser;
            }
        }

        public string Authorization(string login, string password, out string phone_number)
        {
            using (loginEditDatabaseContext db = new loginEditDatabaseContext())
            {
                var user = db.UserInfos.Include(e => e.IdQuestionNavigation).Single(e => e.Nickname == login || e.Phone == login);
                if (user.UserPassword == password)
                {
                    phone_number = user.Phone;
                    return user.IdQuestionNavigation.QuestionValue; }
                throw new Exception("Неправильный пароль");
            }
        }

        public int? Question_Answer(string phone_number, string answer)
        {
            using (loginEditDatabaseContext db = new loginEditDatabaseContext())
            {
                var user = db.UserInfos.Single(e => e.Phone == phone_number);
                if (user.Answer == answer)
                    return user.IdUser;
                return null;
            }
        }

        public string Info(int id)
        {
            string info = "";
            using (loginEditDatabaseContext db = new loginEditDatabaseContext())
            {
                var user = db.UserInfos.Include(e => e.IdGenderNavigation).Include(e => e.IdQuestionNavigation).Single(e => e.IdUser == id);
                info += $"Nickname : {user.Nickname}\n";
                info += $"Имя : {user.FirstName ?? "---"}\n";
                info += $"Фамилия : {user.SecondName ?? "---"}\n";
                info += $"Пол : {user?.IdGenderNavigation?.GenderValue ?? "---"}\n";
                info += $"День рождения : {user.Birthday?.ToString("d") ?? "---"}\n";
                info += $"Номер телефона : {user.Phone}\n";
                info += $"Тема секретного вопроса : {user.IdQuestionNavigation.QuestionValue}\n";
                info += $"Страна : {user.Country ?? "---"}\n";
                info += $"Город : {user.City ?? "---"}\n";
            }
            return info.ToString();
        }

        public string EditShow()
        {
            return "1: Пароль\n" +
                "2: Имя\n" +
                "3: Фамилия\n" +
                "4: Страна\n" +
                "5: Город\n" +
                "6: Дата рождения";
        }

        public void Edit(int id, int variableField, object obj)
        {
            using (loginEditDatabaseContext db = new loginEditDatabaseContext())
            {
                var user = db.UserInfos.Single(e => e.IdUser == id);
                switch(variableField)
                {
                    case 1:
                        {
                            user.UserPassword = (string)obj;
                            break;
                        }
                    case 2:
                        {
                            user.FirstName = (string)obj;
                            break;
                        }
                    case 3:
                        {
                            user.SecondName = (string)obj;
                            break;
                        }
                    case 4:
                        {
                            user.Country = (string)obj;
                            break;
                        }
                    case 5:
                        {
                            user.City = (string)obj;
                            break;
                        }
                    case 6:
                        {
                            user.Birthday = (DateTime)obj;
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
                db.UserInfos.Update(user);
                db.SaveChanges();
            }
        }

        private string PhoneNumberConverter(string phoneNumber)
        {
            StringBuilder number = new StringBuilder("+");
            phoneNumber = String.Join("", phoneNumber.Split('(', ')', '+', '-', ' '));
            if (phoneNumber.Length != 11)
                throw new Exception("Телефонный номер введён в неверном формате");
            if (phoneNumber[0] != 7)
                number.Append("7" + phoneNumber.Substring(1));
            else
                number.Append(phoneNumber);
            return number.ToString();
        }
    }
}
