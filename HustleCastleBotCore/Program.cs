namespace HustleCastleBotCore
{
    class Program
    {
        static void Main(string[] args)
        {
            HustleCastleBot bot = new HustleCastleBot();
            bot.Start(BotMode.Portal);
        }
    }
}
