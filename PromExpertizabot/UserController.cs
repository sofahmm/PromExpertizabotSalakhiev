using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using Database;
using Telegram.Bot.Types;
using System.Xml.Linq;
using static System.Data.Entity.Infrastructure.Design.Executor;
using Telegram.Bot.Types.Enums;
using Update = Telegram.Bot.Types.Update;

namespace UserController
{
    internal class UserControllerClass
    {

		private static InlineKeyboardMarkup GetInlineKeyboard(List<string> buttons = null, List<string> callback = null)
		{
			List<InlineKeyboardButton[]> columns = new List<InlineKeyboardButton[]> { };
			//	InlineKeyboardButton[][][] rowns = new InlineKeyboardButton[rows_count][][];

			if (buttons != null)
				for (int i = 0; i < buttons.Count; i++)
				{
					columns.Add(new[] { InlineKeyboardButton.WithCallbackData(buttons[i], callback[i]) });
				}

			var inlineKeyboard = new InlineKeyboardMarkup(columns);
			return inlineKeyboard;
		}

		public async static void Process(ITelegramBotClient bot, string message_text, long chatID, Update update)
        {
			//Обработка сообщений от заблокированных пользователей
			try
			{
				if (update.Type == UpdateType.CallbackQuery)
					await bot.SendChatAction(update.CallbackQuery.Message.Chat.Id, ChatAction.Typing);
				if (update.Type == UpdateType.Message)
					await bot.SendChatAction(update.Message.Chat.Id, ChatAction.Typing);
			}
			catch { return; }

			var userStatus = DataBaseController.Select("Users", new Dictionary<string, dynamic> { { "TG_ID", chatID } }).Select(d => (string)d["USER_STATUS"]).ToList();
				if (message_text == "") { } //обработка сброса сообщения

				else if (userStatus[0] == "ожидание выполнения заявки")
				{
					await bot.SendMessage(chatID, "Ваша заявка в процессе выполнения, пожалуйста подождите ♥");

					DataBaseController.Update(
						new Dictionary<string, dynamic> {
							{ "TABLE_NAME", "Users"},
							{ "USER_STATUS", "СПАМИТ!"}
						},
				new Dictionary<string, dynamic> {
							{ "TG_ID", chatID}
						}
						);
				}

				else if (userStatus[0] == "описание проблемы")
				{
					var userObjectID = DataBaseController.Select("Users", new Dictionary<string, dynamic> { { "TG_ID", chatID } }).Select(d => (string)d["OBJECT_ID"]).ToList()[0];
					var operatorID = DataBaseController.Select("Users", new Dictionary<string, dynamic> { { "USER_ROLE", "Оператор" }, { "OBJECT_ID", userObjectID } }).Select(d => (string)d["TG_ID"]).ToList()[0];
					var userLink = DataBaseController.Select("Users", new Dictionary<string, dynamic> { { "TG_ID", chatID } }).Select(d => (string)d["CHAT_LINK"]).ToList()[0];
					var problem = DataBaseController.Select("Applications", new Dictionary<string, dynamic> { { "STATUS", "Новая" }, { "USER", chatID } }).Select(d => (string)d["TYPE"]).ToList()[0];
					var problemID = DataBaseController.Select("Applications", new Dictionary<string, dynamic> { { "STATUS", "Новая" }, { "USER", chatID } }).Select(d => (long)d["ID"]).ToList()[0];

					//отправляем оповещение модератору и меняем структуру его УЗ
					List<string> problems = new List<string>() {
					"Лифт",
					"Подъезд",
					"Дом",
					"Свет",
					"Сантехника",
					"Нарушения правил эксплуатации",
					"Другое"
					};

					//передаем айди юзера оператору и меняем его статус
					DataBaseController.Update(
						new Dictionary<string, dynamic> {
							{ "TABLE_NAME", "Users"},
							{ "USER_STATUS", "выполнение заявки"},
							{ "USER_MODERATION_ID", chatID }
						},
				new Dictionary<string, dynamic> {
							{ "TG_ID", operatorID}
						}
						);

					DataBaseController.Update(
						new Dictionary<string, dynamic> {
							{ "TABLE_NAME", "Users"},
							{ "USER_STATUS", "ожидание выполнения заявки"},
							{ "USER_MODERATION_ID", "" }
						},
				new Dictionary<string, dynamic> {
							{ "TG_ID", chatID}
						}
						);

					DataBaseController.Update(
						new Dictionary<string, dynamic> {
							{ "TABLE_NAME", "Applications"},
							{ "DESCRIPTION", message_text}
						},
				new Dictionary<string, dynamic> {
							{ "ID", problemID}
						}
						);

					await bot.SendMessage(chatID, $"Ваша заявка зарегистрированна!\nВ ближайшее время с вами свяжется оператор");
					await bot.SendMessage(operatorID, $"Заявка от пользователя:\n{userLink}\nПо проблеме: {problems[Convert.ToInt32(problem.Replace("U_","")) - 1]}\nКомментарий: {message_text}\nЧтобы начать выполнение нажмите:\n/start_execution");
				}

				else if (message_text == "/start" || message_text == "/new" && userStatus[0] != "")
				{
					List<string> buttons = new List<string> {
					"Лифт",
					"Подъезд",
					"Дом",
					"Свет",
					"Сантехника",
					"Нарушения правил эксплуатации",
					"Другое"
					};
					List<string> callbacks = new List<string> {
					"U_1",
					"U_2",
					"U_3",
					"U_4",
					"U_5",
					"U_6",
					"U_7"
					};
					await bot.SendMessage(chatID, "Выбирите тип проблемы:", replyMarkup: GetInlineKeyboard(buttons, callbacks));
				}

				else
				{

				}

				await Task.Run(() =>
				{

				});
		
		}
    }
}
