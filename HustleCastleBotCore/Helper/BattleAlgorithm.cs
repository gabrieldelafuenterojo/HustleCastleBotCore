using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;

namespace HustleCastleBotCore
{
    public class BattleAlgorithm
    {
        UtilsOcr ocr { get; }
        Navigation nav { get; }
        WriteHelper WriteHelper { get; }
        ConfigurationFile config { get; }

        public BattleAlgorithm()
        {
            ocr = new UtilsOcr();
            nav = new Navigation();
            WriteHelper = new WriteHelper();
            config = new ConfigurationFile();
        }

        public void Run(int i)
        {
            Image image = nav.ScreenCapture();
            nav.ShowDustIncomingStep(i);
            var positionStates = nav.GetDustPositionStates(image);

            Places myPosition = nav.GetMyDustPositionStates(positionStates);

            if (nav.IsPositionBattleFinished(image, myPosition) == false)
            {
                if (DustBucle(myPosition, positionStates) == false)
                {
                    WriteHelper.WriteError($"No se ha podido luchar contra nadie. Intentamos luchar contra alguien más fuerte.");
                    DustBucle(myPosition, positionStates, true);
                }
            }
            else
            {
                WriteHelper.WriteWarning($"Ya se han luchado las peleas de esta etapa.");
            }
        }

        private bool DustBucle(Places myPosition, Dictionary<Places, PositionState> positionStates, bool force = false)
        {
            bool roundPlayed = false;
            for (int r = ((int)myPosition - 1); r >= 1; r--)
            {
                roundPlayed = DustWorker(r, positionStates, force);
                if (roundPlayed)
                    break;
            }

            if (roundPlayed == false)
            {
                for (int r = 10; r > (int)myPosition; r--)
                {
                    roundPlayed = DustWorker(r, positionStates, force);
                    if (roundPlayed)
                        break;
                }
            }

            return roundPlayed;
        }

        public bool DustWorker(int r, Dictionary<Places, PositionState> dic, bool force)
        {
            Thread.Sleep(2000);

            if (dic.FirstOrDefault(_g => _g.Key == Enum.Parse<Places>($"DustPosition{r}")).Value != PositionState.Played)
            {

                nav.TapDustPosition(Enum.Parse<Places>($"DustPosition{r}"));
                WriteHelper.WriteLine($"Intentado luchar contra el enemigo en la posición: {r} ");
                nav.WaitForLocation(Places.BattlePopUp);

                var battlePower = ocr.GetDustPlayersPower();
                var margin = config.GetEnemyMargin();
                int difference = -1;

                if (force)
                    difference = (battlePower.Item1 + margin) - battlePower.Item2;
                else
                    difference = battlePower.Item1 - battlePower.Item2;

                if (difference > 0)
                {
                    nav.TapInGoDustFight();
                    nav.WaitForLocation(Places.Battle);
                    nav.DoubleSpeed();
                    nav.WaitForLocation(Places.BattleFinish);
                    nav.TapFinishBattle();
                    nav.WaitForLocation(Places.DustMap);

                    return true;
                }

                WriteHelper.WriteError($"El enemigo en la posición {r} es demasiado poderoso!");
                nav.KeyScap();
            }
            return false;
        }
    }
}
