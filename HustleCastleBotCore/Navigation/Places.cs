namespace HustleCastleBotCore
{
    public enum Places
    {
        Castle = 11,
        BattleMap = 12,
        Portal = 13,
        Market = 14,
        BattlePopUp = 15,
        Battle = 16,
        BattleFinish = 17,
        LifePopUp = 18,
        ApplePopUp = 19,
        AppleMarket = 20,
        x2Up = 21,
        x2Down = 22,
        SpellPopUp = 23,
        Dust = 24,
        DustMap = 25,
        DustPosition1 = 1,
        DustPosition2 = 2,
        DustPosition3 = 3,
        DustPosition4 = 4,
        DustPosition5 = 5,
        DustPosition6 = 6,
        DustPosition7 = 7,
        DustPosition8 = 8,
        DustPosition9 = 9,
        DustPosition10 = 10,
        DustBattlePopUp = 26,
        DustFinished = 27,
        Unknown = -1
    }

    public class DustPosition
    {
        public int x { get; }
        public int y { get; }

        public readonly int xRange = 153;
        public readonly int yRange = 76;

        public readonly int xTapRange = 134;
        public readonly int yTapRange = 38;

        /// <summary>
        /// Devuelve los valores de posicionamiento de la arena
        /// </summary>
        /// <param name="place"></param>
        public DustPosition(Places place)
        {
            switch(place)
            {
                case Places.DustPosition1 :
                    x = 324;
                    y = 185;
                    break;
                case Places.DustPosition2 :
                    x = 243;
                    y = 277;
                    break;
                case Places.DustPosition3 :
                    x = 404;
                    y = 277;
                    break;
                case Places.DustPosition4 :
                    x = 162;
                    y = 369;
                    break;
                case Places.DustPosition5 :
                    x = 324;
                    y = 369;
                    break;
                case Places.DustPosition6 :
                    x = 485;
                    y = 369;
                    break;
                case Places.DustPosition7 :
                    x = 82;
                    y = 461;
                    break;
                case Places.DustPosition8 :
                    x = 243;
                    y = 461;
                    break;
                case Places.DustPosition9 :
                    x = 404;
                    y = 461;
                    break;
                case Places.DustPosition10 :
                    x = 566;
                    y = 461;
                    break;
                default :
                    break;
            }
        }
    }
}
