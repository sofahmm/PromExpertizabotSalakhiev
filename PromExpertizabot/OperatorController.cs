using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using Database;
using Telegram.Bot.Types;
using static System.Data.Entity.Infrastructure.Design.Executor;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Update = Telegram.Bot.Types.Update;

namespace OperatorController
{
	internal class OperatorControllerClass
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

			else if (userStatus[0] == "вынесение решения по заявке")
			{

				var userObjectID = DataBaseController.Select("Users", new Dictionary<string, dynamic> { { "TG_ID", chatID } }).Select(d => (string)d["TG_ID"]).ToList()[0];
				//var operatorID = DataBaseController.Select("Users", new Dictionary<string, dynamic> { { "USER_ROLE", "Оператор" }, { "OBJECT_ID", userObjectID } }).Select(d => (string)d["TG_ID"]).ToList()[0];
				var userID = DataBaseController.Select("Users", new Dictionary<string, dynamic> { { "TG_ID", chatID } }).Select(d => (string)d["USER_MODERATION_ID"]).ToList()[0];
				var problemID = DataBaseController.Select("Applications", new Dictionary<string, dynamic> { { "STATUS", "Новая" }, { "USER", userID } }).Select(d => (long)d["ID"]).ToList()[0];
				

				DataBaseController.Update(
				new Dictionary<string, dynamic> {
					{ "TABLE_NAME", "Applications"},
					{ "STATUS", "Закрыта"},
					{ "DATE_CLOSE", System.DateTime.Now.ToString("dd-MM-yyyy")}
				},
		new Dictionary<string, dynamic> {
					{ "ID", problemID}
				});

				await bot.SendMessage(userID, $"Заявка успешно закрыта!\nРешение:\n{message_text}\nЕсли возникнут проблемы, отсавьте пожалуйста заявку нажав на\n/new");

				DataBaseController.Update(
				new Dictionary<string, dynamic> {
					{ "TABLE_NAME", "Users"},
					{ "USER_STATUS", "none"},
					{ "USER_MODERATION_ID", "" }
				},
		new Dictionary<string, dynamic> {
					{ "TG_ID", chatID}
				});

				DataBaseController.Update(
				new Dictionary<string, dynamic> {
					{ "TABLE_NAME", "Users"},
					{ "USER_STATUS", "Промодерирован"},
					{ "USER_MODERATION_ID", "" }
				},
		new Dictionary<string, dynamic> {
					{ "TG_ID", userID}
				});
				await bot.SendMessage(chatID, $"Заявка успешно закрыта! С решением:\n{message_text}");
			}

			else if (message_text == "/send_colution" && userStatus[0] == "выполнение заявки")
			{
				DataBaseController.Update(
				new Dictionary<string, dynamic> {
					{ "TABLE_NAME", "Users"},
					{ "USER_STATUS", "вынесение решения по заявке"}
				},
		new Dictionary<string, dynamic> {
					{ "TG_ID", chatID}
				});

				await bot.SendMessage(chatID, "Напишите решение проблемы пользователя:");
			}

			else if (message_text == "/start" && userStatus[0] == "none")
			{
				List<Dictionary<string, dynamic>> arData = DataBaseController.Select("Objects");
				List<string> buttons = arData.Select(d => (string)d["NAME"]).ToList();
				List<string> callbacks = arData.Select(d => (string)d["CODE"]).ToList();

				await bot.SendMessage(chatID, "Приветствую, оператор!", replyMarkup: GetInlineKeyboard(buttons, callbacks));
			}

			else if (message_text == "/start_execution" && userStatus[0] == "выполнение заявки")
			{
				await bot.SendMessage(chatID, "Сообщите пользователю о решении его проблемы, как только оно найдется нажав на:\n/send_colution");
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
