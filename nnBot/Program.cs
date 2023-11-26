using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Exceptions;
using System;
using Telegram.Bot.Types.ReplyMarkups;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.VisualBasic;
using System.Threading;
using System.Runtime.CompilerServices;
using System.Text;
using Newtonsoft.Json.Linq;
using nnBot.Models;

namespace nnBot
{

    class Program
    {
        
        // Общее поле для всех методов
        public static string globalVariableResRD = "";
        public static string globalListRD2 = "";

        // Это клиент для работы с Telegram Bot API, который позволяет отправлять сообщения, управлять ботом, подписываться на обновления и многое другое.
        private static ITelegramBotClient _botClient;

        // Это объект с настройками работы бота. Здесь мы будем указывать, какие типы Update мы будем получать, Timeout бота и так далее.
        private static ReceiverOptions _receiverOptions;

        // Спрятали токен и ID группы
        private static AccessPass Pass = new AccessPass();
        public static string? token = Pass.token;
        public static string? idgroup = Pass.idgroup;


        static async Task Main()
        {
            
            _botClient = new TelegramBotClient($"{token}"); // Присваиваем нашей переменной значение, в параметре передаем Token, полученный от BotFather
            _receiverOptions = new ReceiverOptions // Также присваем значение настройкам бота
            {
                AllowedUpdates = new[] // Тут указываем типы получаемых Update`ов
                {
                    UpdateType.Message, // Сообщения (текст, фото/видео, голосовые/видео сообщения и т.д.)
                },
                // Параметр, отвечающий за обработку сообщений, пришедших за то время, когда ваш бот был оффлайн
                // True - не обрабатывать, False (стоит по умолчанию) - обрабаывать
                ThrowPendingUpdates = false,
            };

            using var cts = new CancellationTokenSource();

            // UpdateHander - обработчик приходящих Update`ов
            // ErrorHandler - обработчик ошибок, связанных с Bot API
            _botClient.StartReceiving(UpdateHandler, ErrorHandler, _receiverOptions, cts.Token); // Запускаем бота

            var me = await _botClient.GetMeAsync(); // Создаем переменную, в которую помещаем информацию о нашем боте.
            Console.WriteLine($"{me.FirstName} запущен!");

            // ДНИ РОЖДЕНИЯ

            while (true)
            {

                DateTime dateVins = new DateTime(1984, 11, 29); // У Винса
                DateTime dateMich = new DateTime(1985, 09, 20); // У Мича
                DateTime dateVadim = new DateTime(1986, 04, 09); // У Вадим
                DateTime dateKem = new DateTime(1985, 09, 03); // У Кема
                DateTime datePaha = new DateTime(1987, 08, 26); // У Пахи

                // Создаем словарь
                Dictionary<DateTime, string> keyValuePairsDR = new Dictionary<DateTime, string>();
                keyValuePairsDR.Add(dateVins, "- ДР у Винса!");
                keyValuePairsDR.Add(dateMich, "- ДР у Мича!");
                keyValuePairsDR.Add(dateVadim, "- ДР у Вадим!");
                keyValuePairsDR.Add(dateKem, "- ДР у Кема!");
                keyValuePairsDR.Add(datePaha, "- ДР у Пахи!");

                // Вызываю методы
                globalVariableResRD = Birthday(keyValuePairsDR);
                globalListRD2 = BirthdayList(keyValuePairsDR);

                // Пишем в группу через бота
                if (globalVariableResRD != "")
                {
                    await _botClient.SendTextMessageAsync(idgroup, globalVariableResRD);
                }
                
                Thread.Sleep(3600000);
                
            }

        await Task.Delay(-1); // Устанавливаем бесконечную задержку, чтобы наш бот работал постоянно


        }



        private static async Task UpdateHandler(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {

            // Обязательно ставим блок try-catch, чтобы наш бот не "падал" в случае каких-либо ошибок
            try
            {
                // Сразу же ставим конструкцию switch, чтобы обрабатывать приходящие Update
                switch (update.Type)
                {
                    case UpdateType.Message:
                        {
                            // эта переменная будет содержать в себе все связанное с сообщениями
                            var message = update.Message;

                            // From - это от кого пришло сообщение (или любой другой Update)
                            var user = message.From;

                            // Выводим на экран то, что пишут нашему боту, а также небольшую информацию об отправителе
                            Console.WriteLine($"{user.FirstName} ({user.Id}) написал сообщение: {message.Text}");

                            // Chat - содержит всю информацию о чате
                            var chat = message.Chat;


                            // Добавляем проверку на тип Message
                            switch (message.Type)
                            {
                                // Тут понятно, текстовый тип
                                case MessageType.Text:
                                    {

                                        if (string.Equals(message.Text, "/Start", StringComparison.OrdinalIgnoreCase))
                                        {
                                            await botClient.SendTextMessageAsync(
                                                  chat.Id,
                                                  "Что нужно сделать?\n" +
                                                  "/list_DR\n" +
                                                  "/FAQ\n");
                                            return;
                                        }

                                        if (message.Text == "/list_DR")
                                        {
                                            await botClient.SendTextMessageAsync(chat.Id, "Список наших Днюх:\n \n" + globalListRD2);
                                            Console.WriteLine($"UserId: {user.Id} выполнил команду /list_DR");
                                        }

                                        if (message.Text == "/FAQ")
                                        {
                                            await botClient.SendTextMessageAsync(chat.Id, "Зачем тут бот? Буду напоминать о ваших Днюхах, а потом посмотрим...");

                                            Console.WriteLine($"UserId: {user.Id} выполнил команду /FAQ"); 
                                            Console.WriteLine($"ChatId: {chat.Id}"); // id Группы
                                            
                                        }
                                        return;
                                    }
                            }



                            return;
                        }

                }
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }


        private static Task ErrorHandler(ITelegramBotClient botClient, Exception error, CancellationToken cancellationToken)
        {
            // Тут создадим переменную, в которую поместим код ошибки и её сообщение 
            var ErrorMessage = error switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => error.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }





        // МЕТОД
        public static string Birthday(Dictionary<DateTime, string> keyValuePairsDR)
        {

            // Текущая дата
            DateTime dateTime = DateTime.Now;

            // Проверка условия
            foreach (var keyDR in keyValuePairsDR)
            {
                if (keyDR.Key.Day - 1 == dateTime.Day && keyDR.Key.Month == dateTime.Month && DateTime.Now.Hour > 12)
                {

                    string res = $"Че кАво ё-ё-ё, напоминаю {keyDR.Key.Day}.{keyDR.Key.Month}.{dateTime.Year} (уже завтра!) {keyDR.Value} ";
                    return res;
                }
                else if (keyDR.Key.Day == dateTime.Day && keyDR.Key.Month == dateTime.Month && DateTime.Now.Hour > 10)
                {
                    string res = $"Аееееее!!! Сегодня {keyDR.Value} Что бы хуй стоял и деньги были!!! (с) Кем.";
                    return res;
                }

                //string res2 = $"{keyDR.Key.Day}.{keyDR.Key.Month}.{DateTime.Now.Year} {keyDR.Value}";
                //return res2;

            }
            return string.Empty;

        }


        public static string BirthdayList(Dictionary<DateTime, string> keyValuePairsDR)
        {
            // Текущая дата
            DateTime dateTime = DateTime.Now;

            // Переменная для хранения результата
            StringBuilder result = new StringBuilder();

            // Проверка условия
            foreach (var keyDR in keyValuePairsDR)
            {
                // Добавляем к строке результатов информацию из словаря
                result.AppendLine($"{keyDR.Key.Day}.{keyDR.Key.Month}.{DateTime.Now.Year} {keyDR.Value}");
            }

            // Возвращаем строку результатов
            return result.ToString();
        }




    }
}