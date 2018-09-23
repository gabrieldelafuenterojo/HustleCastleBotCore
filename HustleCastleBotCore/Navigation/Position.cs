using System.Drawing;

namespace HustleCastleBotCore
{
    public enum PositionState
    {
        MyPosition,
        Played,
        UnPlayed
    }

    public class Position
    {

        /// <summary>
        /// Obtiene el estado de la posición en la arena
        /// </summary>
        /// <param name="image"></param>
        /// <param name="place"></param>
        /// <returns></returns>
        public PositionState GetPositionState(Image image, Places place)
        {
            UtilsOcr ocr = new UtilsOcr();
            Navigation nav = new Navigation();
            DustPosition position = new DustPosition(place);

            image = ocr.CropImage(image, new Rectangle(position.x, position.y, position.xRange, position.yRange));

            if (nav.AreColorsSimilar(nav.GetPixelColor(image, 0, 0), Color.FromArgb(255, 243, 89)))
                return PositionState.MyPosition;

            if (nav.AreColorsSimilar(nav.GetPixelColor(image, 6, 2), Color.FromArgb(188, 188, 188)))
                return PositionState.Played;
            else
                return PositionState.UnPlayed;
        }
    }
}
