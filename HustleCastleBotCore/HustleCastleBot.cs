using System;
using System.Linq;
using System.Threading;

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

            if (mode == BotMode.Dust)
            {
                Dust();
            }

            if (mode == BotMode.Debug)
            {
                Debug();
            }
        }

        public void Debug()
        {
            ocr.GetDustPlayersPower();
            //navigation.PositionBattleFinished(null, Places.AppleMarket);
        }

        public void Dust()
        {
            navigation.StartBot();

            while (true)
            {
                try
                {
                    navigation.StartDust();

                    if (navigation.ActualLocation == Places.DustMap)
                    {
                        for (int i = ocr.GetDustStep(); i <= 5; i++)
                        {
                            BattleAlgorithm battleAlgorithm = new BattleAlgorithm();
                            battleAlgorithm.Run(i);

                            if (i != 5)
                                navigation.WaitForNextDustStep(i);
                            else
                                navigation.EndDust();
                        }
                    }
                }
                catch (Exception ex)
                {
                    writer.WriteError($"{ex.Message}");
                    navigation.StartBot();
                }
            }
        }

        public void Portal()
        {

            navigation.StartBot();
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
                                navigation.WaitForLocation(Places.BattleFinish);
                                navigation.GoPortal(Places.BattleFinish);
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
