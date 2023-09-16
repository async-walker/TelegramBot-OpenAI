﻿using Newtonsoft.Json;
using TelegramBot_OpenAI.Configurations;
using TelegramBot_OpenAI.Data.Enums;
using static TelegramBot_OpenAI.Extensions.FileExtensions;

namespace TelegramBot_OpenAI.Extensions
{
    public static class EnvVariablesExtensions
    {
        private static void SetEnvConnectionString(string connectionString)
        {
            Environment.SetEnvironmentVariable("ConnectionString", connectionString);
        }

        private static void SetEnvOpenAiToken(string token)
        {
            Environment.SetEnvironmentVariable("OpenAiToken", token);
        }

        private static void SetEnvBotSettings(BotConfiguration configuration)
        {
            Environment.SetEnvironmentVariable("BotToken", configuration.BotToken);
            Environment.SetEnvironmentVariable("HostAddress", configuration.HostAddress);
            Environment.SetEnvironmentVariable("Route", configuration.Route);
            Environment.SetEnvironmentVariable("SecretToken", configuration.SecretToken);
        }

        public static string GetEnvConnectionString()
        {
            return Environment.GetEnvironmentVariable("ConnectionString")!;
        }

        public static string GetEnvOpenAiToken()
        {
            return Environment.GetEnvironmentVariable("OpenAiToken")!;
        }

        public static string GetEnvBotSettings(string nameOf)
        {
            return Environment.GetEnvironmentVariable(nameOf)!;
        }

        public static async Task SetEnvVariablesFromFile(this string path, FileType fileType)
        {
            var data = await ReadDataFile(path) ?? throw new Exception("File is EMPTY!");

            switch (fileType)
            {
                case FileType.JSON:
                    {
                        var secrets = ReadSecretsFromJSON(data);

                        //SetEnvConnectionString(secrets.ConnectionString);
                        SetEnvOpenAiToken(secrets.OpenAiToken);
                        SetEnvBotSettings(secrets.BotConfiguration);
                        break;
                    }

                case FileType.SH: 
                    {
                        var secrets = ReadSecretsFromSH(data);

                        foreach (var secret in secrets)
                        {
                            Environment.SetEnvironmentVariable(secret.Key, secret.Value);
                            Console.WriteLine($"Environment variable {secret.Key} is succes set");
                        }
                        break;
                    }
            }
        }

        private static Dictionary<string, string> ReadSecretsFromSH(string data) // For sh files and other
        {
            Dictionary<string, string> secrets = new();

            try
            {
                var pairs = data.Split("\r\n");

                foreach (var pair in pairs)
                {
                    string[] keyValuePairs = pair.Split('=');
                    string key = keyValuePairs[0]; 
                    string value = keyValuePairs[1];

                    secrets.Add(key, value);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return secrets;
        }

        private static SecretsConfiguration ReadSecretsFromJSON(string data)
        {
            var secretsConfiguration = new SecretsConfiguration();

            try
            {
                secretsConfiguration = JsonConvert.DeserializeObject<SecretsConfiguration>(data)!;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return secretsConfiguration;
        }
    }
}
