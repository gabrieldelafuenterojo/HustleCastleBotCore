using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;

namespace HustleCastleBotCore
{
    public class Navigation : UtilsAdb
    {
        WriteHelper WriteHelper;
        UtilsAdb adb;
        UtilsOcr ocr;

        public Navigation()
        {
            WriteHelper = new WriteHelper();
            adb = new UtilsAdb();
            ocr = new UtilsOcr();
        }

        #region WaitAndLocation

        /// <summary>
        /// Espera hasta que se obtiene la ubicación seleccionada
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public bool WaitForLocation(Places location)
        {
            bool result = false;
            int tryCount = 50;
            if (location == Places.Battle)
                tryCount = 1000;
            if (location == Places.BattleFinish)
                tryCount = 5000;
            if (location == Places.DustMap)
                tryCount = 15000;
            if (location == Places.DustFinished)
                tryCount = 600000;

            ConsoleSpiner spin = new ConsoleSpiner();
            WriteHelper.Write($"Esperando por la localización: {location} ");

            for (int i = 0; i < tryCount; i++)
            {
                var _actualLocation = ActualLocation;
                if (_actualLocation == location)
                {
                    result = true;
                    break;
                }

                if (location == Places.Battle)
                {
                    if (_actualLocation == Places.ApplePopUp)
                    {
                        break;
                    }

                    if (_actualLocation == Places.SpellPopUp)
                    {
                        TapContinueInSpellPopUp();
                    }

                    if (_actualLocation == Places.LifePopUp)
                    {
                        TapNoInLifePopUp();
                        Thread.Sleep(4000);
                        TapInGoFight();
                    }
                }

                spin.Turn();
            }

            spin.FinishTurn();

            if (result)
            {
                WriteHelper.WriteSuccessfully($"Navegación completada: {location}");
            }
            else
            {
                WriteHelper.WriteError($"No se ha conseguido completar la navegación: {location}");
            }

            return result;
        }

        /// <summary>
        /// Espera hasta que llega a la ventana inicial
        /// </summary>
        public void WaitForCastle()
        {
            WriteHelper.Write($"Esperando por la localización: {Places.Castle} ");
            ConsoleSpiner spin = new ConsoleSpiner();

            while (true)
            {
                if (ActualLocation != Places.Castle)
                {
                    spin.Turn();
                    KeyScap();
                    Thread.Sleep(1000);
                }
                else
                {
                    break;
                }
            }

            spin.FinishTurn();
        }

        /// <summary>
        /// Inicia el bot
        /// </summary>
        public void StartBot()
        {
            adb.EndGame();
            adb.StartGame();
            WaitForLocation(Places.Castle);
        }

        /// <summary>
        /// Inicia una batalla en la arena
        /// </summary>
        public void StartDust()
        {
            WaitForCastle();
            GoBattleMap(Places.Castle);
            WaitForLocation(Places.BattleMap);
            TapInGoDust();
            WaitForLocation(Places.Dust);
            if (ActualLocation == Places.Dust)
            {
                TapInStartDust();
                WaitForLocation(Places.DustBattlePopUp);
                TapInStartDustBattlePopUp();
            }
            WaitForLocation(Places.DustMap);
        }

        public void WaitForNextDustStep(int i)
        {
            WriteHelper.WriteWarning($"Esperando a la etapa {i + 1}...");
            while (ocr.GetDustStep() == i)
            {
                Thread.Sleep(6000);
            }
        }

        /// <summary>
        /// Termina una batalla en la arena
        /// </summary>
        public void EndDust()
        {
            WaitForLocation(Places.DustFinished);
            Thread.Sleep(1000);
            TapInEndDust();
            Thread.Sleep(5000);
        }

        /// <summary>
        /// Muestra el poder de los jugadores
        /// </summary>
        public void ShowPlayersPower()
        {
            if (ActualLocation == Places.BattlePopUp)
            {
                UtilsOcr ocr = new UtilsOcr();
                var powers = ocr.GetPlayersPower();

                string myPower = powers.Item1.ToString("N0", System.Globalization.CultureInfo.GetCultureInfo("de"));
                string enemyPower = powers.Item2.ToString("N0", System.Globalization.CultureInfo.GetCultureInfo("de"));

                WriteHelper.WriteInfo($"Poder de mi equipo: {myPower}, poder enemigo: {enemyPower}");
            }
        }

        /// <summary>
        /// Muestra las almas oscuras actuales
        /// </summary>
        /// <returns></returns>
        public int ShowActualDarkSouls()
        {
            if (ActualLocation == Places.Portal)
            {
                UtilsOcr ocr = new UtilsOcr();
                var ds = ocr.GetDarkSouls();

                WriteHelper.WriteInfo($"Almas oscuras actuales: {ds.ToString("N0", System.Globalization.CultureInfo.GetCultureInfo("de"))}");

                return ds;
            }

            return 0;
        }

        /// <summary>
        /// Obtiene la ubicación actual
        /// </summary>
        public Places ActualLocation
        {
            get
            {
                var image = ScreenCapture();

                if (IsInCastle(image))
                {
                    return Places.Castle;
                }

                if (IsInBattleMap(image))
                {
                    return Places.BattleMap;
                }

                if (IsInPortal(image))
                {
                    return Places.Portal;
                }

                if (IsInMarket(image))
                {
                    return Places.Market;
                }

                if (IsInBattlePopUp(image))
                {
                    return Places.BattlePopUp;
                }

                if (IsInBattle(image))
                {
                    return Places.Battle;
                }

                if (IsInPortalBattleFinish(image))
                {
                    return Places.BattleFinish;
                }

                if (IsInApplePopUp(image))
                {
                    return Places.ApplePopUp;
                }

                if (IsInLifePopUp(image))
                {
                    return Places.LifePopUp;
                }

                if (IsInAppleMarket(image))
                {
                    return Places.AppleMarket;
                }

                if (IsInSpellPopUp(image))
                {
                    return Places.SpellPopUp;
                }

                if (IsInDust(image))
                {
                    return Places.Dust;
                }

                if (IsInDustMap(image))
                {
                    if (IsInDustFinished(image))
                        return Places.DustFinished;

                    return Places.DustMap;
                }

                if (IsInDustBattlePopUp(image))
                {
                    return Places.DustBattlePopUp;
                }

                return Places.Unknown;
            }
        }

        /// <summary>
        /// Obtiene la localización del x2
        /// </summary>
        public Places x2Location
        {
            get
            {
                var image = ScreenCapture();

                if (Isx2Up(image))
                {
                    return Places.x2Up;
                }
                if (Isx2Down(image))
                {
                    return Places.x2Down;
                }

                return Places.Unknown;
            }
        }

        /// <summary>
        /// Comprueba si hay un error
        /// </summary>
        public void CheckForError()
        {
            WriteHelper Writer = new WriteHelper();
            var image = ScreenCapture();
            if (image == null)
            {
                Writer.WriteError("No se puede contactar con el emulador!");
                Console.ReadKey();
            }
        }

        /// <summary>
        /// Obtiene el número de veces que se ha intentado obtener una ubicación sin éxito
        /// </summary>
        /// <param name="place"></param>
        /// <returns></returns>
        public int Retry(Places place)
        {
            if (place == Places.BattlePopUp)
            {
                return RetryGoBattlePopUp;
            }

            return 0;
        }

        #endregion

        #region IsInPlace

        /// <summary>
        /// Obtiene si estás en la ventana inicial
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public bool IsInCastle(Image image)
        {
            if (AreColorsSimilar(GetPixelColor(image, 36, 527), Color.FromArgb(243, 30, 109)))
                if (AreColorsSimilar(GetPixelColor(image, 31, 69), Color.FromArgb(0, 187, 203)))
                    if (AreColorsSimilar(GetPixelColor(image, 31, 482), Color.FromArgb(0, 187, 203)))
                        return true;

            return false;
        }

        /// <summary>
        /// Obtiene si estás en Mapa de batallas
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public bool IsInBattleMap(Image image)
        {
            if (AreColorsSimilar(GetPixelColor(image, 340, 401), Color.FromArgb(232, 207, 76)))
                if (AreColorsSimilar(GetPixelColor(image, 69, 497), Color.FromArgb(239, 36, 32)))
                    return true;

            return false;
        }

        /// <summary>
        /// Obtiene si estás en el portal
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public bool IsInPortal(Image image)
        {
            if (AreColorsSimilar(GetPixelColor(image, 44, 90), Color.FromArgb(85, 66, 73)))
                if (AreColorsSimilar(GetPixelColor(image, 30, 515), Color.FromArgb(85, 66, 73)))
                    if (AreColorsSimilar(GetPixelColor(image, 538, 514), Color.FromArgb(85, 66, 73)))
                        return true;

            return false;
        }

        /// <summary>
        /// Obtiene si estás en el mercado
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public bool IsInMarket(Image image)
        {
            if (AreColorsSimilar(GetPixelColor(image, 36, 597), Color.FromArgb(0, 187, 202)))
                if (AreColorsSimilar(GetPixelColor(image, 759, 597), Color.FromArgb(0, 187, 202)))
                    if (AreColorsSimilar(GetPixelColor(image, 506, 19), Color.FromArgb(255, 64, 32)))
                        return true;

            return false;
        }

        /// <summary>
        /// Obtiene si estás en un pop up de pelea
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public bool IsInBattlePopUp(Image image)
        {
            if (AreColorsSimilar(GetPixelColor(image, 378, 281), Color.FromArgb(138, 165, 186)))
                if (AreColorsSimilar(GetPixelColor(image, 419, 281), Color.FromArgb(138, 165, 186)))
                    if (AreColorsSimilar(GetPixelColor(image, 363, 281), Color.FromArgb(137, 227, 150)))
                        if (AreColorsSimilar(GetPixelColor(image, 432, 280), Color.FromArgb(231, 149, 119)))
                            return true;

            return false;
        }

        /// <summary>
        /// Obtiene si estás en una pelea
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public bool IsInBattle(Image image)
        {
            if (AreColorsSimilar(GetPixelColor(image, 375, 25), Color.FromArgb(138, 165, 186)))
                if (AreColorsSimilar(GetPixelColor(image, 425, 25), Color.FromArgb(138, 165, 186)))
                    return true;

            return false;
        }

        /// <summary>
        /// Obtiene si estás en un final de pelea de portal
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public bool IsInPortalBattleFinish(Image image)
        {
            if (AreColorsSimilar(GetPixelColor(image, 366, 554), Color.FromArgb(113, 219, 24)))
                if (AreColorsSimilar(GetPixelColor(image, 432, 542), Color.FromArgb(113, 219, 24)))
                    return true;

            return false;
        }

        public bool IsInDustBattleFinish(Image image)
        {
            if (AreColorsSimilar(GetPixelColor(image, 366, 554), Color.FromArgb(113, 219, 24)))
                if (AreColorsSimilar(GetPixelColor(image, 432, 542), Color.FromArgb(113, 219, 24)))
                    return true;

            return false;
        }

        /// <summary>
        /// Obtiene si estás en un pop up de que todos tus guerreros 
        /// no tienen la vida al máximo
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public bool IsInLifePopUp(Image image)
        {
            if (AreColorsSimilar(GetPixelColor(image, 330, 355), Color.FromArgb(113, 219, 24)))
                if (AreColorsSimilar(GetPixelColor(image, 470, 355), Color.FromArgb(243, 62, 24)))
                    return true;

            return false;
        }

        /// <summary>
        /// Obtiene si estás en el menu de manzanas del mercado
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public bool IsInAppleMarket(Image image)
        {
            if (AreColorsSimilar(GetPixelColor(image, 70, 380), Color.FromArgb(0, 187, 203)))
                if (AreColorsSimilar(GetPixelColor(image, 315, 380), Color.FromArgb(0, 187, 203)))
                    if (AreColorsSimilar(GetPixelColor(image, 565, 380), Color.FromArgb(0, 187, 203)))
                        if (AreColorsSimilar(GetPixelColor(image, 410, 205), Color.FromArgb(170, 20, 0)))
                            return true;

            return false;
        }

        /// <summary>
        /// Obtiene si estás en el pop up de que te faltan manzanas para pelear
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public bool IsInApplePopUp(Image image)
        {
            if (AreColorsSimilar(GetPixelColor(image, 330, 355), Color.FromArgb(113, 219, 24)))
                if (AreColorsSimilar(GetPixelColor(image, 470, 355), Color.FromArgb(243, 62, 24)))
                    for (int i = 340; i < 450; i++)
                    {
                        if (AreColorsSimilar(GetPixelColor(image, i, 275), Color.FromArgb(255, 64, 32)))
                            return true;
                    }

            return false;
        }

        /// <summary>
        /// Obtiene si el boton x2 está arriba
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public bool Isx2Up(Image image)
        {
            if (AreColorsSimilar(GetPixelColor(image, 770, 460), Color.FromArgb(113, 219, 24)))
                if (AreColorsSimilar(GetPixelColor(image, 770, 461), Color.FromArgb(113, 219, 24)))
                    if (AreColorsSimilar(GetPixelColor(image, 770, 462), Color.FromArgb(113, 219, 24)))
                        return true;

            return false;
        }

        /// <summary>
        /// Obtiene si el boton x2 está abajo
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public bool Isx2Down(Image image)
        {
            if (AreColorsSimilar(GetPixelColor(image, 775, 540), Color.FromArgb(113, 219, 24)))
                if (AreColorsSimilar(GetPixelColor(image, 775, 541), Color.FromArgb(113, 219, 24)))
                    if (AreColorsSimilar(GetPixelColor(image, 775, 542), Color.FromArgb(113, 219, 24)))
                        return true;

            return false;
        }

        /// <summary>
        /// Obtiene si está en el pop up de falta un hechizo
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public bool IsInSpellPopUp(Image image)
        {
            if (AreColorsSimilar(GetPixelColor(image, 370, 365), Color.FromArgb(251, 189, 4)))
                if (AreColorsSimilar(GetPixelColor(image, 440, 365), Color.FromArgb(113, 219, 24)))
                    return true;

            return false;
        }

        /// <summary>
        /// Obtiene si está en la pantalla principal de arena
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public bool IsInDust(Image image)
        {
            if (AreColorsSimilar(GetPixelColor(image, 309, 444), Color.FromArgb(156, 241, 70)))
                if (AreColorsSimilar(GetPixelColor(image, 698, 177), Color.FromArgb(207, 110, 81)))
                    return true;

            return false;
        }

        /// <summary>
        /// Obtiene si hay una arena en curso
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public bool IsInDustMap(Image image)
        {
            if (AreColorsSimilar(GetPixelColor(image, 160, 565), Color.FromArgb(76, 68, 59)))
                if (AreColorsSimilar(GetPixelColor(image, 600, 565), Color.FromArgb(76, 68, 59)))
                    return true;

            return false;
        }

        /// <summary>
        /// Obtiene si hay abierta una ventana de pelea en la arena
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public bool IsInDustBattlePopUp(Image image)
        {
            if (AreColorsSimilar(GetPixelColor(image, 690, 185), Color.FromArgb(81, 17, 10)))
                if (AreColorsSimilar(GetPixelColor(image, 690, 450), Color.FromArgb(76, 17, 9)))
                    return true;

            return false;
        }

        /// <summary>
        /// Obtiene si ha terminado la arena
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public bool IsInDustFinished(Image image)
        {
            if (AreColorsSimilar(GetPixelColor(image, 686, 84), Color.FromArgb(243, 171, 8)))
                return true;

            return false;
        }

        #endregion

        /// <summary>
        /// Obtiene la etapa actual de la arena
        /// </summary>
        /// <returns></returns>
        public Places GetDustPosition()
        {
            Image image = ScreenCapture();
            Places place = new Places();
            Position position = new Position();

            for (int i = 1; i <= 10; i++)
            {
                place = Enum.Parse<Places>($"DustPosition{i}");
                if (position.GetPositionState(image, place) == PositionState.MyPosition)
                {
                    return place;
                }
            }

            return Places.Unknown;
        }

        /// <summary>
        /// Muestra en que etapa de la arena se está entrando
        /// </summary>
        /// <param name="i"></param>
        public void ShowDustIncomingStep(int i)
        {
            WriteHelper.WriteWarning($"Entrando en etapa {i} de 5.");
        }

        /// <summary>
        /// Obtiene el estado de todos los participantes de la arena
        /// </summary>
        /// <returns></returns>
        public Dictionary<Places, PositionState> GetDustPositionStates(Image image)
        {
            Places place = new Places();
            Position position = new Position();

            Dictionary<Places, PositionState> Dictionary = new Dictionary<Places, PositionState>();

            for (int i = 1; i <= 10; i++)
            {
                place = Enum.Parse<Places>($"DustPosition{i}");
                Dictionary.Add(place, position.GetPositionState(image, place));
            }

            return Dictionary;
        }

        /// <summary>
        /// Obtiene si ya se ha efectuado la pelea en la arena
        /// </summary>
        /// <param name="image"></param>
        /// <param name="place"></param>
        /// <returns></returns>
        public bool IsPositionBattleFinished(Image image, Places place)
        {
            UtilsOcr ocr = new UtilsOcr();
            Navigation nav = new Navigation();
            DustPosition position = new DustPosition(place);

            var matriz = new Rectangle(position.x, position.y, position.xRange, position.yRange);

            image = ocr.CropImage(image, matriz);

            for (int x = 0; x < position.xRange; x++)
            {
                for (int y = 0; y < position.yRange; y++)
                {
                    if (AreColorsSimilar(GetPixelColor(image, x, y), Color.FromArgb(113, 215, 48)))
                    {
                        return true;
                    }
                }
            }

            return false;
            
        }

        /// <summary>
        /// Obtiene mi posición en la arena
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public Places GetMyDustPositionStates(Dictionary<Places, PositionState> dic)
        {
            return dic.FirstOrDefault(_g => _g.Value == PositionState.MyPosition).Key;
        }

        #region GoToPlace

        /// <summary>
        /// Hace tap en una posición de la arena
        /// </summary>
        /// <param name="place"></param>
        public void TapDustPosition(Places place)
        {
            DustPosition position = new DustPosition(place);
            TapRange(position.x + 9, position.y + 5, position.xTapRange, position.yTapRange);
        }

        /// <summary>
        /// Se mueve hasta el mapa de batalla desde la ubicación seleccionada
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public bool GoBattleMap(Places from)
        {
            if (from == Places.Castle)
            {
                TapGoBattleMapFromCastle();
            }
            if (from == Places.Portal)
            {
                KeyScap();
            }

            return WaitForLocation(Places.BattleMap);
        }

        /// <summary>
        /// Se mueve hasta la pantalla de inicio desde la ubicación seleccionada
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public bool GoCastle(Places from)
        {
            if (from == Places.BattleMap)
            {
                TapGoCastleMapFromBattleMap();
            }

            return WaitForLocation(Places.Castle);
        }

        /// <summary>
        /// Se mueve hasta el portal desde la ubicación seleccionada
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public bool GoPortal(Places from)
        {
            if (from == Places.BattleMap)
            {
                TapGoPortalFromBattleMap();

            }

            if (from == Places.BattleFinish)
            {
                TapFinishBattle();
            }

            if (from == Places.Unknown)
            {
                WaitForCastle();
                GoBattleMap(Places.Castle);
                TapGoPortalFromBattleMap();
            }

            return WaitForLocation(Places.Portal);
        }

        /// <summary>
        /// Se mueve hasta el nivel del portal seleccionado
        /// </summary>
        /// <param name="level">Nivel del portal seleccionado</param>
        /// <param name="actualLevel">Nivel actual del portal</param>
        /// <returns></returns>
        public bool GoPortalLevel(int level, int actualLevel)
        {
            WriteHelper.WriteWarning($"Nivel actual de portal: {actualLevel}, destino: {level}");
            if (actualLevel > level && actualLevel != -1)
            {
                for (int i = 0; i < (actualLevel - level); i++)
                {
                    int x;
                    if (i % 2 == 0)
                        x = 235;
                    else
                    {
                        x = 234;
                    }
                    Swipe(285, 285, 285, x);
                    Thread.Sleep(4000);
                }
            }

            return true;
        }

        public int RetryGoBattlePopUp = 0;
        /// <summary>
        /// Entra en el pop up de batalla
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public bool GoBattlePopUp(Places from)
        {
            if (from == Places.Portal)
            {
                TapInPortalLevel();
            }

            bool result = WaitForLocation(Places.BattlePopUp);
            if (result == false)
            {
                WriteHelper.WriteError("Portal cerrado!?");
                RetryGoBattlePopUp++;
            }
            else
            {
                RetryGoBattlePopUp = 0;
            }

            return result;
        }

        /// <summary>
        /// Se mueve hacia el mercado desde la ubicación seleccionada
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public bool GoMarket(Places from)
        {
            if (from == Places.Castle)
            {
                TapInMarket();
            }

            return WaitForLocation(Places.Market);
        }

        /// <summary>
        /// Se mueve hacia el mercado de manzanas desde la ubicación seleccionada
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public bool GoAppleMarket(Places from)
        {
            if (from == Places.Market)
            {
                TapInAppleMarket();
            }

            return WaitForLocation(Places.AppleMarket);
        }

        /// <summary>
        /// Se mueve hacia la batalla desde la ubicación seleccionada
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public bool GoBattle(Places from)
        {
            if (from == Places.BattlePopUp)
            {
                TapInGoFight();
            }

            return WaitForLocation(Places.Battle);
        }

        /// <summary>
        /// Compra manzanas en el mercado desde cualquier ubicación
        /// </summary>
        public void BuyMarketFood()
        {
            WaitForCastle();
            GoMarket(Places.Castle);
            TapInTreasureMarket();
            Thread.Sleep(3000);
            GoAppleMarket(Places.Market);
            TapInFood();
            Thread.Sleep(2000);
            WaitForCastle();
        }

        #endregion

        #region ClickAndGo

        /// <summary>
        /// Clica en comprar comida desde el menu de comida del mercado
        /// </summary>
        public void TapInFood()
        {
            TapRange(600, 430, 92, 30);
        }

        /// <summary>
        /// Clica en continuar si te falta un hechizo por poner
        /// </summary>
        public void TapContinueInSpellPopUp()
        {
            TapRange(265, 360, 110, 40);
        }

        /// <summary>
        /// Hace tap en el mapa de batalla desde el castillo
        /// </summary>
        public void TapGoBattleMapFromCastle()
        {
            TapRange(25, 515, 60, 60);
        }

        /// <summary>
        /// Hace tap en el castillo desde el mapa de batalla
        /// </summary>
        public void TapGoCastleMapFromBattleMap()
        {
            TapRange(25, 515, 60, 60);
        }

        /// <summary>
        /// Hace tap en el portal desde el mapa de batalla
        /// </summary>
        public void TapGoPortalFromBattleMap()
        {
            TapRange(710, 330, 60, 55);
        }

        /// <summary>
        /// Hace tap en terminar la pelea
        /// </summary>
        public void TapFinishBattle()
        {
            TapRange(360, 545, 85, 30);
        }

        /// <summary>
        /// Finaliza la arena dandole a reclamar recompensa
        /// </summary>
        public void TapInEndDust()
        {
            TapRange(660, 65, 113, 40);
        }

        /// <summary>
        /// Pulsa aceptar en el dust battle pop up
        /// </summary>
        public void TapInStartDustBattlePopUp()
        {
            TapRange(280, 455, 90, 30);
        }

        /// <summary>
        /// Hace tap en la arena
        /// </summary>
        public void TapInGoDust()
        {
            TapRange(310, 415, 90, 80);
        }

        /// <summary>
        /// Hace tap en el botón de empezar arena
        /// </summary>
        public void TapInStartDust()
        {
            TapRange(200, 450, 110, 35);
        }

        /// <summary>
        /// Clica no en el menu de todos tus soldados no tienen la vida al máximo
        /// </summary>
        public void TapNoInLifePopUp()
        {
            TapRange(425, 350, 90, 40);
        }

        /// <summary>
        /// Hace tap en luchar
        /// </summary>
        public void TapInGoFight()
        {
            TapRange(525, 445, 110, 30);
        }

        /// <summary>
        /// Hace tap en la pelea de las arenas
        /// </summary>
        public void TapInGoDustFight()
        {
            TapRange(540, 445, 105, 28);
        }

        /// <summary>
        /// Hace tap en el nivel del portal
        /// </summary>
        public void TapInPortalLevel()
        {
            TapRange(160, 445, 350, 10);
        }

        /// <summary>
        /// Hace tap en el mercado
        /// </summary>
        public void TapInMarket()
        {
            TapRange(715, 520, 62, 52);
        }

        /// <summary>
        /// Hace tap en el menu de tesoros del mercado
        /// </summary>
        public void TapInTreasureMarket()
        {
            TapRange(440, 560, 110, 20);
        }

        /// <summary>
        /// Hace tap en las manzanas del mercado
        /// </summary>
        public void TapInAppleMarket()
        {
            Swipe(650, 400, 515, 400);
            Thread.Sleep(3000);
            TapRange(660, 475, 100, 30);
        }

        /// <summary>
        /// Hace tap en el boton x2 cuando se encuentra arriba
        /// </summary>
        public void TapInDoubleSpeedUp()
        {
            TapRange(705, 455, 70, 40);
        }

        /// <summary>
        /// Hace tap en el boton x2 cuando se encuentra abajo
        /// </summary>
        public void TapInDoubleSpeedDown()
        {
            TapRange(705, 535, 70, 35);
        }

        /// <summary>
        /// Aumenta x2 la velocidad de la batalla
        /// </summary>
        public void DoubleSpeed()
        {
            var location = x2Location;

            if (location == Places.x2Up)
            {
                TapInDoubleSpeedUp();
                Thread.Sleep(2000);
                TapInDoubleSpeedUp();
            }

            if (location == Places.x2Down)
            {
                TapInDoubleSpeedDown();
                Thread.Sleep(2000);
                TapInDoubleSpeedDown();
            }
        }

        #endregion

    }
}
