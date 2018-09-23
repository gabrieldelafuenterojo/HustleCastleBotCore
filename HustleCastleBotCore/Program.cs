using System;

namespace HustleCastleBotCore
{
    class Program
    {
        static void Main(string[] args)
        {
            HustleCastleBot bot = new HustleCastleBot();
            ConfigurationFile config = new ConfigurationFile();
            bot.Start(Enum.Parse<BotMode>($"{config.GetBotMode()}"));
        }
    }
}
