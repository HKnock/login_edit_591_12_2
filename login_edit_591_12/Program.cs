using System;
using System.Threading;
using System.Threading.Tasks;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Args;
using System.Collections.Generic;
using System.Linq;

namespace login_edit_591_12
{
    class Program
    {
        private static TelegramBotClient _loginEditBot;
        private static Profile profile = new Profile();
        private static OutputString dbStr = new OutputString();

        private static Dictionary<long, List<string>> msg = new Dictionary<long, List<string>>();
        private static Dictionary<long, int?> idDict = new Dictionary<long, int?>();
        private static Dictionary<long, Command> cmd = new Dictionary<long, Command>();
        private static Dictionary<string, (string command, int counter, int steps)> commandList =
            new Dictionary<string, (string command, int counter, int steps)>
            {
                { "login" , ("login", 1, 3)},
                { "register", ("register", 1, 5) },
                { "edit", ("edit", 1, 2)},
                { "delete", ("delete", 1, 1) }
            };


        public static async Task Main()
        {
            _loginEditBot = new TelegramBotClient(Configuration.BotToken);
            _loginEditBot.StartReceiving();

            Console.WriteLine("Бот работает");

            _loginEditBot.OnMessage += OnMessageHandler;

            Console.ReadLine();
            _loginEditBot.StopReceiving();
        }

        private static void OnMessageHandler(object sender, MessageEventArgs e)
        {
            var mes = e.Message.Text;
            var id = e.Message.Chat.Id;

            if (!msg.ContainsKey(id))
            {
                msg.Add(id, new List<string>());
                idDict.Add(id, null);
                cmd.Add(id, null);
            }
            msg[id].Add(mes);
            Console.WriteLine($"{e.Message.Chat.FirstName} {String.Join(" | ", msg[id])}");

            switch (mes)
            {
                case "/start":
                    _loginEditBot.SendTextMessageAsync(id, "Выберите команду:\n" +
                        "Авторизация: /login\n" +
                        "Регистрация /register");
                    break;
                case "/sign_out":
                    {
                        if (idDict[id] == null)
                            _loginEditBot.SendTextMessageAsync(id, "Вы не авторизованы");
                        else
                        {
                            idDict[id] = null;
                            _loginEditBot.SendTextMessageAsync(id, "Вы вышли из аккаунта");
                            _loginEditBot.SendTextMessageAsync(id, "Выберите команду:\n" +
                            "Авторизация: /login\n" +
                            "Регистрация /register");
                        }
                        break;
                    }
                case "/edit":
                    {
                        if (idDict[id] == null)
                        {
                            _loginEditBot.SendTextMessageAsync(id, "Авторизуйтесь");
                            break;
                        }
                        msg[id].Clear();
                        ; profile.EditShow();
                        _loginEditBot.SendTextMessageAsync(id, "Выберите что хотите изменить");
                        _loginEditBot.SendTextMessageAsync(id, profile.EditShow());
                        cmd[id] = new Command(commandList["edit"]);
                        break;
                    }
                case "/info":
                    {
                        if (idDict[id] != null)
                            _loginEditBot.SendTextMessageAsync(id, profile.Info((int)idDict[id]));
                        else
                            _loginEditBot.SendTextMessageAsync(id, "Авторизуйтесь (Команда /login)");
                        break;
                    }
                case "/login":
                    {
                        if (idDict[id] != null)
                            _loginEditBot.SendTextMessageAsync(id, "Вы уже авторизованы");
                        else
                        {
                            msg[id].Clear();
                            _loginEditBot.SendTextMessageAsync(id, "Введите логин: ");
                            cmd[id] = new Command(commandList["login"]);
                        }
                        break;
                    }
                case "/delete":
                    {
                        try
                        {
                            using (loginEditDatabaseContext db = new loginEditDatabaseContext())
                            {
                                if (idDict[id] != null)
                                {
                                    UserInfo user = db.UserInfos.Single(e => e.IdUser == idDict[id]);
                                    if (user != null)
                                    {
                                        db.UserInfos.Remove(user);
                                        idDict[id] = null;
                                        db.SaveChanges();
                                    }
                                    _loginEditBot.SendTextMessageAsync(id, "Пользователь успешно удалён");
                                    _loginEditBot.SendTextMessageAsync(id, "Выберите команду:\n" +
                                    "Авторизация: /login\n" +
                                    "Регистрация /register");
                                    break;
                                }
                                else
                                {
                                    _loginEditBot.SendTextMessageAsync(id, "Вы не авторизованы (Команда /login)");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _loginEditBot.SendTextMessageAsync(id, "Возникла ошибка" + ex.Message);
                        }
                        break;
                    }

                case "/register":
                    {
                        if (idDict[id] != null)
                            _loginEditBot.SendTextMessageAsync(id, "Вы уже авторизованы");
                        else
                        {
                            msg[id].Clear();
                            _loginEditBot.SendTextMessageAsync(id, "Введите никнейм: ");
                            cmd[id] = new Command(commandList["register"]);
                        }
                        break;
                    }
                default:
                    switch (cmd[id]?.command ?? string.Empty)
                    {
                        case "edit":
                            {
                                if (cmd[id].counter == cmd[id].steps)
                                {
                                    try
                                    {
                                        if (int.Parse(msg[id][0]) == 6)
                                            profile.Edit((int)idDict[id], int.Parse(msg[id][0]),
                                               new DateTime(int.Parse(msg[id][3]), 
                                               int.Parse(msg[id][2]), 
                                               int.Parse(msg[id][1])));
                                        else
                                            profile.Edit((int)idDict[id], int.Parse(msg[id][0]), msg[id][1]);
                                        _loginEditBot.SendTextMessageAsync(id, "Данные успешно изменены");
                                        _loginEditBot.SendTextMessageAsync(id, "Выберите команду\n" +
                                        "Просмотр информации о профиле: /info\n" +
                                        "Изменение информации: /edit\n" +
                                        "Удалить себя: /delete\n" +
                                        "Выход: /sign_out");
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine(ex.Message);
                                        _loginEditBot.SendTextMessageAsync(id, "Ошибка");
                                    }
                                }
                                else
                                {
                                    if (cmd[id].counter == 1)
                                    {
                                        if (int.Parse(msg[id][0]) == 6)
                                        {
                                            cmd[id].steps += 2;
                                            _loginEditBot.SendTextMessageAsync(id, "Введите день");
                                            cmd[id].counter++;
                                            break;
                                        }
                                        _loginEditBot.SendTextMessageAsync(id, "Введите данные для изменения");
                                        cmd[id].counter++;
                                        break;
                                    }
                                    if (cmd[id].counter == 2)
                                    {
                                        _loginEditBot.SendTextMessageAsync(id, "Введите месяц в цифровом формате");
                                        cmd[id].counter++;
                                        break;
                                    }
                                    if (cmd[id].counter == 3)
                                    {
                                        _loginEditBot.SendTextMessageAsync(id, "Введите год");
                                        cmd[id].counter++;
                                        break;
                                    }
                                }
                                break;
                            }
                        case "login":
                            {
                                if (cmd[id].counter == cmd[id].steps)
                                {
                                    try
                                    {
                                        idDict[id] = profile.Question_Answer(msg[id][2], msg[id][3]);
                                        if (idDict[id] == null)
                                            _loginEditBot.SendTextMessageAsync(id, "Неправильный ответ.\n" +
                                                "Введите команду ещё раз.");
                                        else
                                        {
                                            _loginEditBot.SendTextMessageAsync(id, "Вы успешно вошли");
                                            _loginEditBot.SendTextMessageAsync(id, "Выберите команду\n" +
                                                "Просмотр информации о профиле: /info\n" +
                                                "Изменение информации: /edit\n" +
                                                "Удалить себя: /delete\n" +
                                                "Выход: /sign_out");
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine(ex.Message);
                                        _loginEditBot.SendTextMessageAsync(id, "Ошибка");
                                    }
                                    break;
                                }
                                else
                                {
                                    if (cmd[id].counter == 1)
                                    {
                                        _loginEditBot.SendTextMessageAsync(id, "Введите пароль");
                                        cmd[id].counter++;
                                        break;
                                    }
                                    if (cmd[id].counter == 2)
                                    {
                                        try
                                        {
                                            _loginEditBot.SendTextMessageAsync(id, $"Для входа, ответьте на вопрос\n" +
                                            $"{profile.Authorization(msg[id][0], msg[id][1], out string temp)}");
                                            msg[id].Add(temp);
                                            cmd[id].counter++;
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine(ex.Message);
                                            _loginEditBot.SendTextMessageAsync(id, "Ошибка.\n Введите команду ещё раз.");
                                        }
                                        break;
                                    }
                                }
                                break;
                            }
                        case "register":
                            {
                                if (cmd[id].counter == cmd[id].steps)
                                {
                                    if (!int.TryParse(msg[id][3], out int question_number))
                                    {
                                        _loginEditBot.SendTextMessageAsync(id, "Неправильно выбран вопрос, попробуйте еще раз\n" +
                                            "Введите команду ещё раз.");
                                        return;
                                    }
                                    try
                                    {
                                        idDict[id] = profile.Registration(msg[id][0], msg[id][1], msg[id][2], question_number, msg[id][4]);
                                        _loginEditBot.SendTextMessageAsync(id, "Пользователь успешно зарегестрирован и авторизован");
                                        _loginEditBot.SendTextMessageAsync(id, "Выберите команду:\n" +
                                            "Просмотр информации о пользователе: /info\n" +
                                            "Изменение информации: /edit\n" +
                                            "Удалить себя: /delete\n" +
                                            "Выход: /sign_out");

                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine(ex.Message + ex.GetType());
                                        _loginEditBot.SendTextMessageAsync(id, "Зарегистрироваться не удалось, произошла" +
                                            " ошибка. Попробуйте еще раз");
                                    }

                                }
                                else
                                {
                                    if (cmd[id].counter == 1)
                                    {
                                        cmd[id].counter++;
                                        _loginEditBot.SendTextMessageAsync(id, "Введите пароль:");
                                        break;
                                    }
                                    if (cmd[id].counter == 2)
                                    {
                                        cmd[id].counter++;
                                        _loginEditBot.SendTextMessageAsync(id, "Введите телефон");
                                        break;
                                    }
                                    if (cmd[id].counter == 3)
                                    {
                                        cmd[id].counter++;
                                        _loginEditBot.SendTextMessageAsync(id, $"Выберете номер вопроса из списка:\n" +
                                            $"{dbStr.getQuestions()}");
                                        break;
                                    }
                                    if (cmd[id].counter == 4)
                                    {
                                        cmd[id].counter++;
                                        _loginEditBot.SendTextMessageAsync(id, "Введите ответ на вопрос: ");
                                        break;
                                    }
                                }
                                break;
                            }

                        default:
                            break;
                    }
                    break;
            }
        }
    }
}