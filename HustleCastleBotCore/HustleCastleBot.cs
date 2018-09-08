using System;

namespace HustleCastleBotCore
{
    public class HustleCastleBot
    {
        Navigation navigation;
        UtilsOcr ocr;
        ConfigurationFile config;
        UtilsAdb adb;
        WriteHelper writer;

        public HustleCastleBot()
        {
            navigation = new Navigation();
            ocr = new UtilsOcr();
            config = new ConfigurationFile();
            adb = new UtilsAdb();
            writer = new WriteHelper();
        }

        public void Start(BotMode mode)
        {
            if (mode == BotMode.Portal)
            {
                Portal();
            }
        }

        public void Portal()
        {

            adb.EndGame();
            adb.StartGame();
            navigation.WaitForLocation(Places.Castle);
            var limitReached = 0;

            while (true)
            {
                try
                {
                    if (navigation.ActualLocation != Places.Portal)
                    {
                        navigation.GoPortal(Places.Unknown);
                    }
                    else
                    {
                        navigation.GoPortalLevel(config.GetPortalLevel(), ocr.GetPortalLevel());

                        if (navigation.ShowActualDarkSouls() > config.GetMaxDarkSouls())
                        {
                            if (limitReached > 3)
                            {
                                throw new Exception("Se ha alcanzado el límite de almas a conseguir");
                            }

                            limitReached++;
                        }
                        else
                        {
                            limitReached = 0;
                        }

                        if (navigation.GoBattlePopUp(Places.Portal))
                        {
                            navigation.ShowPlayersPower();

                            if (navigation.GoBattle(Places.BattlePopUp))
                            {
                                navigation.DoubleSpeed();
                                navigation.WaitForLocation(Places.PortalBattleFinish);
                                navigation.GoPortal(Places.PortalBattleFinish);
                            }
                            else
                            {
                                if (navigation.ActualLocation == Places.ApplePopUp)
                                {
                                    if (config.BuyFoodInMarket())
                                        navigation.BuyMarketFood();
                                }

                                navigation.GoPortal(Places.Unknown);
                            }
                        }
                        else
                        {
                            if (navigation.Retry(Places.BattlePopUp) >= config.GetMaxRetryPortal())
                            {
                                navigation.GoPortal(Places.Unknown);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    writer.WriteError($"{ex.Message}");
                    Console.ReadKey();
                    System.Environment.Exit(1);
                }
            }
        }
    }
}
