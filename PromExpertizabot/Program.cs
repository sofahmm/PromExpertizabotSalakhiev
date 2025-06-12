using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using System.Data.SQLite;
using static System.Net.Mime.MediaTypeNames;
using static System.Data.Entity.Infrastructure.Design.Executor;
using Update = Telegram.Bot.Types.Update;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using Telegram.Bot.Exceptions;
using System.IO.Pipelines;
using Database;
using System.Data.Entity.Validation;
using Microsoft.Extensions.Options;
using System.Xml.Linq;
using UserController;
using OperatorController;
using AdministratorController;
using ModeratorController;

namespace PromExpertizabot
{
	internal class Program
	{
		static async Task Main(string[] args)
		{
			Console.WriteLine("==SYSTEM== Bot starting");

			//БАЗА ДАННЫХ
			{
				dbFileName = @"config\DataBase.db";
				connectionstring = @"Data Source=" + dbFileName + ";Version=3; ReadOnly=False;";
			}

			//Импорт настроек
			{
				StreamReader sr = new StreamReader("config/BOT_Settings.txt");
				Token = sr.ReadLine();
				sr.Close();
				bot = new TelegramBotClient(Token);
			}

			Console.WriteLine("==SYSTEM== All Ok");

			Start_Bot();
			await Task.Delay(-1); // Бесконечное ожидание
		}

		static string dbFileName = "";
		static string Token = "";
		public static string connectionstring = "";

		public static TelegramBotClient bot;


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

		public static async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
		{
			//Обработка сообщений от заблокированных пользователей
			try
			{
				if (update.Type == UpdateType.CallbackQuery)
					await bot.SendChatAction(update.CallbackQuery.Message.Chat.Id, ChatAction.Typing);
				if (update.Type == UpdateType.Message)
					await bot.SendChatAction(update.Message.Chat.Id, ChatAction.Typing);
			}
			catch (Exception ex) { Console.WriteLine(ex.Message); return; }

			if (update.Message != null)
			{
				var update_message = update.Message;
				var userMessage = update.Message.Text;
				var chatID = update_message.Chat.Id;
				var message_text = update_message.Text.ToLower();

				Console.WriteLine($"{System.DateTime.Now} | {update.Message.Chat.Username}: {message_text}");

				//ДЭФОЛТНЫЕ СООБЩЕНИЯ//ДЭФОЛТНЫЕ СООБЩЕНИЯ//ДЭФОЛТНЫЕ СООБЩЕНИЯ//ДЭФОЛТНЫЕ СООБЩЕНИЯ//ДЭФОЛТНЫЕ СООБЩЕНИЯ//ДЭФОЛТНЫЕ СООБЩЕНИЯ
				var userStatus = DataBaseController.Select("Users", new Dictionary<string, dynamic> { { "TG_ID", chatID } }).Select(d => (string)d["USER_STATUS"]).ToList();
				if (update.Type == UpdateType.Message
					&& update_message.Text != null)
				{
					if (message_text == "")
					{
						return;
					}

					if (DataBaseController.Select("Users", new Dictionary<string, dynamic> { { "TG_ID", chatID } }).Count > 0
					)
					{
						if (userStatus.Count > 0)
							if (userStatus[0] != ""
							&& userStatus[0] != "Не промодерирован")
							{
								string userRole = DataBaseController.Select("Users", new Dictionary<string, dynamic> { { "TG_ID", chatID } }).Select(d => (string)d["USER_ROLE"]).ToList()[0];

								switch (userRole)
								{
									case "Пользователь":
										{
											UserControllerClass.Process(bot, message_text, chatID, update);
										}
										break;
									case "Администратор":
										{
											AdministratorControllerClass.Process(bot, message_text, chatID, update);
										}
										break;
									case "Оператор":
										{
											OperatorControllerClass.Process(bot, message_text, chatID, update);
										}
										break;
									case "Модератор":
										{
											ModeratorControllerClass.Process(bot, message_text, chatID, update);
										}
										break;
									default:
										{

										}
										break;
								}
							}
						//try { await bot.DeleteMessage(chatID, update_message.MessageId); } catch { } //удаление сообщений
					}
					else
					{
						
						if (DataBaseController.Select("Users", new Dictionary<string, dynamic> { { "STAFF_PASSCODE", message_text } }).Count > 0)
						{

							DataBaseController.Update(
							new Dictionary<string, dynamic> {
								{ "TABLE_NAME", "Users"},
								{ "TG_ID", chatID},
								{ "STAFF_PASSCODE", "ЗАРЕГИСТРИРОВАН"},
								{ "USER_NAME", update.Message.Chat.Username},
								{ "CHAT_LINK", $"https://t.me/{update.Message.Chat.Username}"},
							},
					new Dictionary<string, dynamic> {
								{ "STAFF_PASSCODE", message_text}
							});
								

							//var lol = DataBaseController.Select("Users", new Dictionary<string, dynamic> { { "STAFF_PASSCODE", message_text } }).Count;

							var userRole = DataBaseController.Select("Users", new Dictionary<string, dynamic> { { "TG_ID", chatID } }).Select(d => (string)d["USER_ROLE"]).ToList();
								
							if (userRole.Count > 0)
							{
								List<string> buttons = new List<string> {
								"Начать работу"
								};
								List<string> callbacks = new List<string> { "START_WORK" };

								await bot.SendMessage(chatID, $"Добро пожаловать, {userRole[0]}.\nНажмите, чтобы начать работу:", replyMarkup: GetInlineKeyboard(buttons, callbacks));
							}
							
						}
						else if (message_text == "/start")
						{
							DataBaseController.Insert(
							new Dictionary<string, dynamic>{
								{"TABLE_NAME", "Users"},
								{"TG_ID", chatID},
								{"USER_NAME", update.Message.Chat.Username},
								{"USER_ROLE", "Пользователь"},
								{"CHAT_LINK", $"https://t.me/{update.Message.Chat.Username}"},
								{"USER_STATUS", "Не промодерирован"}
							});

							List<Dictionary<string, dynamic>> arData = DataBaseController.Select("Objects");
							List<string> buttons = arData.Select(d => (string)d["NAME"]).ToList();
							List<string> callbacks = arData.Select(d => (string)d["CODE"]).ToList();
							//replyMarkup: GetInlineKeyboard(buttons, callbacks)
							await bot.SendMessage(chatID, $"Здравствуйте! Ваш аккаунт на модерации, пожалуйста подождите");

							var moderator = DataBaseController.Select("Users", new Dictionary<string, dynamic> { { "USER_ROLE", "Модератор" } }).Select(d => (string)d["TG_ID"]).ToList();
							if (moderator.Count > 0)
							{
								buttons = new List<string> {
								"Начать модерацию",
								};
									callbacks = new List<string> {
									chatID.ToString(),
								};

								await bot.SendMessage(moderator[0], $"Заявка на модерацию от пользователя:\nhttps://t.me/{update.Message.Chat.Username}", replyMarkup: GetInlineKeyboard(buttons, callbacks));
							}
						}

						else
						{
							await bot.SendMessage(chatID, "Здравствуйте! Ваш аккаунт все еще на модерации, пожалуйста подождите");
						}
					}
					//try { await bot.DeleteMessage(chatID, update_message.MessageId); } catch { } //удаление сообщений
				}
			}

			//КОЛЛБЭКИ//КОЛЛБЭКИ//КОЛЛБЭКИ//КОЛЛБЭКИ//КОЛЛБЭКИ//КОЛЛБЭКИ//КОЛЛБЭКИ//КОЛЛБЭКИ//КОЛЛБЭКИ//КОЛЛБЭКИ//КОЛЛБЭКИ//КОЛЛБЭКИ//КОЛЛБЭКИ//КОЛЛБЭКИ
			if (update.CallbackQuery != null)
			{
				if (update.Type == UpdateType.CallbackQuery && update.CallbackQuery.Data != null)
				{
					var callback = update.CallbackQuery.Data;
					var chatID = update.CallbackQuery.Message.Chat.Id;
					string userRole = DataBaseController.Select("Users", new Dictionary<string, dynamic> { { "TG_ID", chatID } }).Select(d => (string)d["USER_ROLE"]).ToList()[0];

					if (userRole == "Пользователь" && callback == "MENU")
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

					if (userRole == "Пользователь" && callback.StartsWith("U"))
					{
						var userObjectID = DataBaseController.Select("Users", new Dictionary<string, dynamic> { { "TG_ID", chatID } }).Select(d => (string)d["OBJECT_ID"]).ToList()[0];
						var operatorID = DataBaseController.Select("Users", new Dictionary<string, dynamic> { { "USER_ROLE", "Оператор" }, { "OBJECT_ID", userObjectID } }).Select(d => (string)d["TG_ID"]).ToList()[0];

						DataBaseController.Update(
						new Dictionary<string, dynamic> {
							{ "TABLE_NAME", "Users"},
							{ "USER_STATUS", "описание проблемы"},
							{ "USER_MODERATION_ID", operatorID }
						},
				new Dictionary<string, dynamic> {
							{ "TG_ID", chatID}
						});

						DataBaseController.Insert(
						new Dictionary<string, dynamic> {
							{ "TABLE_NAME", "Applications"},
							{ "STATUS", "Новая"},
							{ "TYPE", callback},
							{ "EXECUTOR", operatorID},
							{ "USER", chatID},
							{ "DATE_CREATE", System.DateTime.Now.ToString("dd-MM-yyyy")}
						});

						await bot.SendMessage(chatID, "Опишите проблему подробнее:");
					}

					else if (userRole == "Модератор" && callback == "START_WORK")
					{
						await bot.SendMessage(chatID, "В этот чат будут приходить заявки на модерацию пользователей, вам необходимо оперативно с ними связаться и ввести необходимые данные или же отменить модерацию.");
					}

					else if (userRole == "Оператор" && callback == "START_WORK")
					{
						await bot.SendMessage(chatID, "В этот чат будут приходить заявки на проблемы касательно объекта пользователя, вам необходимо оперативно с ним связаться и уточнить проблему, после чего сообщить информацию ответственной группе.");
					}

					else if (userRole == "Администратор" && callback == "SEND_AD")
					{

						DataBaseController.Update(
					new Dictionary<string, dynamic>
						{
								{ "TABLE_NAME", "Users"},
								{ "USER_STATUS", "реклама"}
						},
			new Dictionary<string, dynamic>
						{
								{ "TG_ID", chatID}
						});

						await bot.SendMessage(chatID, "Напишите текст рекламы:");
					}

					else if (userRole == "Администратор" && callback == "SEND_NEWS")
					{

						List<Dictionary<string, dynamic>> arData = DataBaseController.Select("Objects");
						List<string> buttons = arData.Select(d => (string)d["NAME"]).ToList();
						List<string> callbacks = arData.Select(d => (string)d["CODE"]).ToList();
						buttons.Add("Всем объектам");
						callbacks.Add("ALL_OBJECTS");

						await bot.SendMessage(chatID, "Какому объекту будет предоставлена новость?", replyMarkup: GetInlineKeyboard(buttons, callbacks));	
					}

					else if (userRole == "Администратор" && callback == "ALL_OBJECTS")
					{
						DataBaseController.Update(
						new Dictionary<string, dynamic>
							{
								{ "TABLE_NAME", "Users"},
								{ "USER_STATUS", "новости"}
							},
				new Dictionary<string, dynamic>
							{
								{ "TG_ID", chatID}
							});

						await bot.SendMessage(chatID, "Напишите текст новости:");
					}

					else if (userRole == "Администратор")
					{
						DataBaseController.Update(
					new Dictionary<string, dynamic>
						{
								{ "TABLE_NAME", "Users"},
								{ "USER_STATUS", "новости"},
								{ "USER_MODERATION_ID", callback }
						},
			new Dictionary<string, dynamic>
						{
								{ "TG_ID", chatID}
						});

						await bot.SendMessage(chatID, "Напишите текст новости:");
					}


					else
					{
						//Блок модерирования
						if (userRole == "Модератор")
						{
							var userStatus = DataBaseController.Select("Users", new Dictionary<string, dynamic> { { "TG_ID", chatID } }).Select(d => (string)d["USER_STATUS"]).ToList();

							List<string> buttons = new List<string> {
							"Ввести имя",
							};
							List<string> callbacks = new List<string> {
								callback.ToString()
							};

							await bot.SendMessage(chatID, $"/start_moderation");//, replyMarkup: GetInlineKeyboard(buttons, callbacks)

							DataBaseController.Update(
							new Dictionary<string, dynamic> {
								{ "TABLE_NAME", "Users"},
								{ "USER_STATUS", "Модерация пользователя 1"},
								{ "USER_MODERATION_ID", callback}
							},
							new Dictionary<string, dynamic> {
								{ "TG_ID", chatID}
							});

						}
					}

					//try { await bot.DeleteMessage(chatID, update.CallbackQuery.Message.MessageId); } catch { } //удаление сообщений
				}
			}

		}

		public static async Task HandleErrorAsync(ITelegramBotClient bot, Exception exception, HandleErrorSource errorSource, CancellationToken cancellationToken)
		{
			if (exception.Message.Contains("Forbidden: bot was blocked by the user"))
			{
				Console.WriteLine("==SYSTEM==: " + exception.Message + "\n");
				return;
			}
			else
			{
				Console.WriteLine("==SYSTEM==: " + exception.Message + "\n");
			}
		}

		public static void Start_Bot()
		{
			var cts = new CancellationTokenSource();
			var cancellationToken = cts.Token;

			var receiverOptions = new ReceiverOptions
			{
				AllowedUpdates = { }, // receive all update types
			};

			bot.StartReceiving(
				HandleUpdateAsync,
				HandleErrorAsync,
				receiverOptions,
				cancellationToken
			);
		}

	}
}