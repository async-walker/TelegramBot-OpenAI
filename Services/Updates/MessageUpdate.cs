﻿using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using Telegram.Bot;
using System.Net.Http.Headers;
using TelegramBot_OpenAI.Models;
using System.Diagnostics.Eventing.Reader;
using TelegramBot_OpenAI.Interfaces;
using TelegramBot_OpenAI.Data.Enums;

namespace TelegramBot_OpenAI.Services
{
    public partial class UpdateHandlers // Message update
    {
        private async Task BotOnMessageReceived(Message message, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Receive message type: {MessageType}", message.Type);
            if (message.Text is not { } messageText)
                return;

            var action = messageText.Split(' ')[0] switch
            {
                "/inline_keyboard" => SendInlineKeyboard(_botClient, message, cancellationToken),
                "/keyboard" => SendReplyKeyboard(_botClient, message, cancellationToken),
                "/remove" => RemoveKeyboard(_botClient, message, cancellationToken),
                "/photo" => SendFile(_botClient, message, cancellationToken),
                "/request" => RequestContactAndLocation(_botClient, message, cancellationToken),
                "/inline_mode" => StartInlineQuery(_botClient, message, cancellationToken),
                _ => Usage(_user, _userRepository, _botClient, message, cancellationToken)
            };
            Message sentMessage = await action;
            _logger.LogInformation("The message was sent with id: {SentMessageId}", sentMessage.MessageId);

            static async Task<Message> SendInlineKeyboard(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
            {
                await botClient.SendChatActionAsync(
                    chatId: message.Chat.Id,
                    chatAction: ChatAction.Typing,
                    cancellationToken: cancellationToken);

                // Simulate longer running task
                await Task.Delay(500, cancellationToken);

                InlineKeyboardMarkup inlineKeyboard = new(
                    new[]
                    {
                    // first row
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData("1.1", "11"),
                        InlineKeyboardButton.WithCallbackData("1.2", "12"),
                    },
                    // second row
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData("2.1", "21"),
                        InlineKeyboardButton.WithCallbackData("2.2", "22"),
                    },
                    });

                return await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Choose",
                    replyMarkup: inlineKeyboard,
                    cancellationToken: cancellationToken);
            }

            static async Task<Message> SendReplyKeyboard(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
            {
                ReplyKeyboardMarkup replyKeyboardMarkup = new(
                    new[]
                    {
                        new KeyboardButton[] { "1.1", "1.2" },
                        new KeyboardButton[] { "2.1", "2.2" },
                    })
                {
                    ResizeKeyboard = true
                };

                return await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Choose",
                    replyMarkup: replyKeyboardMarkup,
                    cancellationToken: cancellationToken);
            }

            static async Task<Message> RemoveKeyboard(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
            {
                return await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Removing keyboard",
                    replyMarkup: new ReplyKeyboardRemove(),
                    cancellationToken: cancellationToken);
            }

            static async Task<Message> SendFile(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
            {
                await botClient.SendChatActionAsync(
                    message.Chat.Id,
                    ChatAction.UploadPhoto,
                    cancellationToken: cancellationToken);

                const string filePath = "Files/tux.png";
                await using FileStream fileStream = new(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                var fileName = filePath.Split(Path.DirectorySeparatorChar).Last();

                return await botClient.SendPhotoAsync(
                    chatId: message.Chat.Id,
                    photo: new InputFileStream(fileStream, fileName),
                    caption: "Nice Picture",
                    cancellationToken: cancellationToken);
            }

            static async Task<Message> RequestContactAndLocation(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
            {
                ReplyKeyboardMarkup RequestReplyKeyboard = new(
                    new[]
                    {
                    KeyboardButton.WithRequestLocation("Location"),
                    KeyboardButton.WithRequestContact("Contact"),
                    });

                return await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Who or Where are you?",
                    replyMarkup: RequestReplyKeyboard,
                    cancellationToken: cancellationToken);
            }

            static async Task<Message> Usage(TelegramUser user, IUserRepository repository, ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
            {
                string answer;

                if (user is null)
                {
                    answer = "Its your first launch";

                    var telegramId = message.Chat.Id;
                    var userName = message.Chat.Username;
                    var bio = message.Chat.Bio;
                    var lastActionDate = DateTime.Now.ToUniversalTime();
                    var userAction = UserAction.Regestration;

                    var isReg = await repository.Add(new TelegramUser(telegramId, userName, bio, lastActionDate, userAction));

                    await botClient.SendTextMessageAsync(message.Chat.Id, isReg.ToString());
                }
                else if (user.IsReg)
                    answer = "You are reg";
                else answer = "You are not reg";

                return await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: answer,
                    replyMarkup: new ReplyKeyboardRemove(),
                    cancellationToken: cancellationToken);
            }

            static async Task<Message> StartInlineQuery(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
            {
                InlineKeyboardMarkup inlineKeyboard = new(
                    InlineKeyboardButton.WithSwitchInlineQueryCurrentChat("Inline Mode"));

                return await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Press the button to start Inline Query",
                    replyMarkup: inlineKeyboard,
                    cancellationToken: cancellationToken);
            }
        }
    }
}
