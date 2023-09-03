﻿namespace TelegramBot_OpenAI.Configurations
{
    public class SecretsConfiguration // For json
    {
        //public readonly string OpenAiConfiguration = "OpenAI_Token";
        public string ConnectionString { get; init; }
        public string OpenAiToken { get; init; }
        public BotConfiguration BotConfiguration { get; init; }
    }
}
