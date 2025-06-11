using Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Update = Telegram.Bot.Types.Update;

namespace AdministratorController
{
    internal class AdministratorControllerClass
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
			var userStatus = DataBaseController.Select("Users", new Dictionary<string, dynamic> { { "TG_ID", chatID } }).Select(d => (string)d["USER_STATUS"]).ToList()[0];
			if (message_text == "") { } //обработка сброса сообщения

			else if (message_text == "/start")
			{
				List<string> buttons = new List<string> { "Рассылка рекламы", "Рассылка новости" };
				List<string> callbacks = new List<string> { "SEND_AD", "SEND_NEWS" };

				await bot.SendMessage(chatID, "Приветствую, администратор!", replyMarkup: GetInlineKeyboard(buttons, callbacks));
			}

			else if (message_text == "/menu")
			{
				List<string> buttons = new List<string> { "Рассылка рекламы", "Рассылка новости" };
				List<string> callbacks = new List<string> { "SEND_AD", "SEND_NEWS" };

				await bot.SendMessage(chatID, "Меню команд:", replyMarkup: GetInlineKeyboard(buttons, callbacks));
			}

			else if (userStatus == "реклама")
			{
				var users = DataBaseController.Select("Users", new Dictionary<string, dynamic> { { "USER_ROLE", "Пользователь" } }).Select(d => (string)d["TG_ID"]).ToList();
				if (users.Count > 0)
				{
					foreach (var user in users)
					{
						await bot.SendMessage(user, message_text);
					}

					DataBaseController.Update(
					new Dictionary<string, dynamic>
						{
								{ "TABLE_NAME", "Users"},
								{ "USER_STATUS", "none"}
						},
			new Dictionary<string, dynamic>
						{
								{ "TG_ID", chatID}
						});

					await bot.SendMessage(chatID, "Нажмите /menu\nЧтобы вызвать меню");
				}
				else
				{
					DataBaseController.Update(
					new Dictionary<string, dynamic>
						{
								{ "TABLE_NAME", "Users"},
								{ "USER_STATUS", "none"}
						},
			new Dictionary<string, dynamic>
						{
								{ "TG_ID", chatID}
						});
					await bot.SendMessage(chatID, "Похоже, что пользователей не существует :(\nНажмите /menu\nЧтобы вызвать меню");
				}
					
			}

			else if (userStatus == "новости")
			{
				var userObjectID = DataBaseController.Select("Users", new Dictionary<string, dynamic> { { "TG_ID", chatID } }).Select(d => (string)d["USER_MODERATION_ID"]).ToList()[0];
				var users = DataBaseController.Select("Users", new Dictionary<string, dynamic> { { "USER_ROLE", "Пользователь" }, { "OBJECT_ID", userObjectID} }).Select(d => (string)d["TG_ID"]).ToList();
				if (users.Count > 0)
				{
					foreach (var user in users)
					{
						await bot.SendMessage(user, message_text);
					}

					DataBaseController.Update(
					new Dictionary<string, dynamic>
						{
								{ "TABLE_NAME", "Users"},
								{ "USER_STATUS", "none"}
						},
			new Dictionary<string, dynamic>
						{
								{ "TG_ID", chatID}
						});
					await bot.SendMessage(chatID, "Нажмите /menu\nЧтобы вызвать меню");
				}
				else
				{
					DataBaseController.Update(
					new Dictionary<string, dynamic>
						{
								{ "TABLE_NAME", "Users"},
								{ "USER_STATUS", "none"}
						},
			new Dictionary<string, dynamic>
						{
								{ "TG_ID", chatID}
						});
					await bot.SendMessage(chatID, "Похоже, что пользователей на данном объекте не существует :(\nНажмите /menu\nЧтобы вызвать меню");
				}
				
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
