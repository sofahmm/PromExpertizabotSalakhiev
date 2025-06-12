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

namespace ModeratorController
{
	internal class ModeratorControllerClass
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

			if (userStatus.Count > 0)
			{
				if (userStatus[0] == "Модерация пользователя 1")
				{
					await bot.SendMessage(chatID, "Введите имя пользователя:");
					DataBaseController.Update(
							new Dictionary<string, dynamic> {
									{ "TABLE_NAME", "Users"},
									{ "USER_STATUS", "Модерация пользователя 2"}
							},
							new Dictionary<string, dynamic> {
								{ "TG_ID", chatID}
							}
							);
				}

				else if (userStatus[0] == "Модерация пользователя 2")//ИМЯ
				{
					await bot.SendMessage(chatID, "Введите фамилию пользователя:");

					DataBaseController.Update(
						new Dictionary<string, dynamic> {
									{ "TABLE_NAME", "Users"},
									{ "USER_STATUS", "Модерация пользователя 3"}
						},
						new Dictionary<string, dynamic> {
								{ "TG_ID", chatID}
						}
						);

					var moderatingUser = DataBaseController.Select("Users", new Dictionary<string, dynamic> { { "TG_ID", chatID } }).Select(d => (string)d["USER_MODERATION_ID"]).ToList()[0];

					DataBaseController.Update(
					new Dictionary<string, dynamic> {
									{ "TABLE_NAME", "Users"},
									{ "FIRST_NAME", message_text}
					},
					new Dictionary<string, dynamic> {
								{ "TG_ID", moderatingUser}
					}
					);
				}

				else if (userStatus[0] == "Модерация пользователя 3")//ФАМИЛИЯ
				{
					string listObjects = "Первый /1 \nВторой /2 \nТретий /3 \nЧетвертый /4 \nПятый /5 \nШестой /6";

					await bot.SendMessage(chatID, $"Выберите объект пользователя:\n{listObjects}");

					DataBaseController.Update(
						new Dictionary<string, dynamic> {
									{ "TABLE_NAME", "Users"},
									{ "USER_STATUS", "Модерация пользователя 4"}
						},
						new Dictionary<string, dynamic> {
								{ "TG_ID", chatID}
						}
						);

					var moderatingUser = DataBaseController.Select("Users", new Dictionary<string, dynamic> { { "TG_ID", chatID } }).Select(d => (string)d["USER_MODERATION_ID"]).ToList()[0];

					DataBaseController.Update(
					new Dictionary<string, dynamic> {
									{ "TABLE_NAME", "Users"},
									{ "LAST_NAME", message_text}
					},
					new Dictionary<string, dynamic> {
								{ "TG_ID", moderatingUser}
					}
					);
				}

				else if (userStatus[0] == "Модерация пользователя 4")//ОБЪЕКТ
				{
					DataBaseController.Update(
						new Dictionary<string, dynamic> {
									{ "TABLE_NAME", "Users"},
									{ "USER_STATUS", "none"}
						},
						new Dictionary<string, dynamic> {
								{ "TG_ID", chatID}
						}
						);

					var moderatingUser = DataBaseController.Select("Users", new Dictionary<string, dynamic> { { "TG_ID", chatID } }).Select(d => (string)d["USER_MODERATION_ID"]).ToList()[0];

					DataBaseController.Update(
					new Dictionary<string, dynamic> {
									{ "TABLE_NAME", "Users"},
									{ "OBJECT_ID", message_text.Replace("/","")},
									{ "USER_STATUS", "Промодерирован"}
					},
					new Dictionary<string, dynamic> {
								{ "TG_ID", moderatingUser}
					}
					);

					await bot.SendMessage(chatID, $"Модерация пользователя завершена!");

					List<string> buttons = new List<string> {
							"Меню"
							};
					List<string> callbacks = new List<string> { "MENU" };
		
					await bot.SendMessage(moderatingUser, $"Ваш аккаунт подтвержден!",replyMarkup: GetInlineKeyboard(buttons, callbacks));
				}
				
			}
			
			if (message_text == "") { } //обработка сброса сообщения

			else if (message_text == "/start")
			{
				List<string> buttons = new List<string> {
							"Начать работу"
							};
				List<string> callbacks = new List<string> { "START_WORK" };
				await bot.SendMessage(chatID, "Приветствую, модератор!", replyMarkup: GetInlineKeyboard(buttons, callbacks));
			}

			else if (message_text == "/help")
			{
				//	await bot.SendMessage(chatID, update_message.MessageId + "");
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
