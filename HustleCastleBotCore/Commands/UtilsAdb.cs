using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;

namespace HustleCastleBotCore
{
    /// <summary>
    /// Gestiona las interacciones con el emulador a traves de comandos
    /// </summary>
    public class UtilsAdb : Command
    {

        #region Image

        /// <summary>
        /// Directorio donde se guarda la imagen para debug
        /// </summary>
        private readonly string ImagePath = $@"{Directory.GetCurrentDirectory()}\temp\output.png";

        /// <summary>
        /// Captura la pantalla
        /// </summary>
        /// <returns></returns>
        public Image ScreenCapture()
        {
            WriteHelper Writer = new WriteHelper();

            try
            {
                //return Image.FromFile(ImagePath);

                Image result = null;
                byte[] data = Convert.FromBase64String(ReturnExec(@"exec-out ""screencap -p | base64""", Commands.CommandEnum.Adb));
                using (var stream = new MemoryStream(data, 0, data.Length))
                {
                    result = Image.FromStream(stream);
                }
                result.Save(ImagePath);
                return result;
            }
            catch (Exception)
            {
                Writer.WriteError($"No se puede contactar con el emulador!");
                Thread.Sleep(1000);
                return null;
            }
        }

        public Bitmap GetBitmapCapture()
        {
            return new Bitmap(ScreenCapture());
        }

        public void StartGame()
        {
            Exec($@"shell monkey -p com.my.hc.rpg.kingdom.simulator -c android.intent.category.LAUNCHER 1", Commands.CommandEnum.Adb);
        }

        public void EndGame()
        {
            Exec($@"shell am force-stop com.my.hc.rpg.kingdom.simulator", Commands.CommandEnum.Adb);
            Thread.Sleep(3000);
        }

        /// <summary>
        /// Obtiene el color del pixel de la imagen seleccionada
        /// </summary>
        /// <param name="image"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Color GetPixelColor(Image image, int x, int y)
        {
            using (Bitmap bmp = new Bitmap(image))
            {
                Color color = bmp.GetPixel(x, y);
                return color;
            }
        }

        /// <summary>
        /// Devuelve si el color es similar con un margen de error de 3.
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <returns></returns>
        public bool AreColorsSimilar(Color c1, Color c2)
        {
            return Math.Abs(c1.R - c2.R) < 3 &&
                   Math.Abs(c1.G - c2.G) < 3 &&
                   Math.Abs(c1.B - c2.B) < 3;
        }
        #endregion

        #region Gestures

        /// <summary>
        /// Pulsa en la pantalla
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void Tap(int x, int y)
        {
            Exec($@"shell input tap {x} {y}", Commands.CommandEnum.Adb);
        }

        /// <summary>
        /// Pulsa en la pantalla
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void TapRange(int x, int y, int xRange, int yRange)
        {
            var random = new Random();
            x = random.Next(x, (xRange + x));
            y = random.Next(y, (yRange + y));

            Exec($@"shell input tap {x} {y}", Commands.CommandEnum.Adb);
        }

        /// <summary>
        /// Desliza en la pantalla
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        public void Swipe(int x1, int y1, int x2, int y2)
        {
            Exec($@"shell input swipe {x1} {y1} {x2} {y2}", Commands.CommandEnum.Adb);
        }

        /// <summary>
        /// Desliza de forma mantenida en la pantalla
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="t"></param>
        public void SwipeTime(int x1, int y1, int x2, int y2, int t)
        {
            Exec($@"shell input swipe {x1} {y1} {x2} {y2} {t} ", Commands.CommandEnum.Adb);
        }

        /// <summary>
        /// Pulsa la tecla escape
        /// </summary>
        public void KeyScap()
        {
            Exec($@"shell input keyevent 4", Commands.CommandEnum.Adb);
        }

        #endregion

    }
}
