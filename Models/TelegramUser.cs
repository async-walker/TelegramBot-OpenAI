﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TelegramBot_OpenAI.Data.Enums;

namespace TelegramBot_OpenAI.Models
{
    public class TelegramUser
    {
        [Key]
        public int UserId { get; init; }
        public long TelegramId { get; init; }
        public string? UserName { get; set; }
        public string? Bio { get; set; }
        public bool IsReg { get; set; } = false;
        public int Age { get; set; }
        public DateTime? RegestrationDate { get; set; }
        public DateTime? LastActionDate { get; set; }
        public UserAction UserAction { get; set; }
        [ForeignKey(nameof(InvitedUser))]
        public int? IdInvited { get; set; }
        public TelegramUser? InvitedUser { get; set; }
        public int? CountReferals { get; set; } = 0;
        public int CountGenerations { get; set; } = 0;
        public decimal AccountBalance { get; set; } = 0;
        public ModelChatGPT DefaultModelChatGPT { get; set; }
        public LanguageInterface LanguageInterface { get; set; } 

        public TelegramUser() { }

        public TelegramUser(
            long telegramId,
            string? userName,
            string? bio,
            DateTime dateTime,
            UserAction userAction)
        {
            TelegramId = telegramId;
            UserName = userName;
            Bio = bio;
            RegestrationDate = dateTime;
            LastActionDate = dateTime;
            UserAction = userAction;
        }
    }
}
