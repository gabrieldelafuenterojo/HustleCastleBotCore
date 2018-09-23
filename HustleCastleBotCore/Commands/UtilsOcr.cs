using HustleCastleBotCore.Helper;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Text.RegularExpressions;
using Tesseract;

namespace HustleCastleBotCore
{
    public class UtilsOcr : Command
    {

        #region Ocr Engine

        private TesseractEngine CreateEngine(string lang = "eng", EngineMode mode = EngineMode.Default)
        {
            var datapath = DataPath;
            return new TesseractEngine(datapath, lang, mode);
        }

        private string DataPath
        {
            get { return AbsolutePath("tessdata"); }
        }

        private string AbsolutePath(string relativePath)
        {
            return Path.Combine(Directory.GetCurrentDirectory(), relativePath);
        }

        /// <summary>
        /// Instancia el ocr engine
        /// </summary>
        /// <param name="r"></param>
        /// <param name="config"></param>
        /// <param name="image"></param>
        /// <returns></returns>
        public string OcrEngineResult(Rectangle r, EngineWhiteListConfig config, Bitmap image = null)
        {
            using (var engine = CreateEngine())
            {
                engine.SetVariable(config.Variable, Config.GetCharWhitelistPortal());

                if (image == null)
                {
                    image = UtilsAdb.GetBitmapCapture();
                }

                var bitmapImage = GrayScaleFilter(CropBitMap(image, r));

                using (var img = PixConverter.ToPix(bitmapImage))
                {
                    using (var page = engine.Process(img, PageSegMode.SingleLine))
                    {
                        return ToNumericOnly(page.GetText());
                    }
                }
            }
        }

        #endregion

        #region Constructor

        private UtilsAdb UtilsAdb { get; }
        private ConfigurationFile Config { get; }
        private WriteHelper Writer { get; }

        public UtilsOcr()
        {
            UtilsAdb = new UtilsAdb();
            Config = new ConfigurationFile();
            Writer = new WriteHelper();
        }

        #endregion

        #region Portal

        /// <summary>
        /// Obtiene el nivel de portal actual
        /// </summary>
        /// <returns></returns>
        public int GetPortalLevel()
        {
            int result = -1;

            var text = OcrEngineResult(
                new Rectangle(231, 405, 100, 22),
                new EngineWhiteListConfig
                {
                    Value = Config.GetCharWhitelistPortal()
                }
            );

            int.TryParse(text, out result);

            if (result > 80)
                result = -1;

            return result;
        }

        #endregion

        #region BattlePower

        /// <summary>
        /// Obtiene tu nivel de combate y el del enemigo
        /// </summary>
        /// <returns></returns>
        public Tuple<int, int> GetPlayersPower()
        {
            var ScreenCapture = UtilsAdb.GetBitmapCapture();
            int myPower = -1;
            int enemyPower = -1;

            var text = OcrEngineResult(
                new Rectangle(200, 272, 131, 20),
                new EngineWhiteListConfig
                {
                    Value = Config.GetCharWhitelistBattlePower()
                },
                ScreenCapture
            );

            int.TryParse(text, out myPower);

            text = OcrEngineResult(
                new Rectangle(440, 272, 122, 20),
                new EngineWhiteListConfig
                {
                    Value = Config.GetCharWhitelistBattlePower()
                },
                ScreenCapture
            );

            int.TryParse(text, out enemyPower);

            return new Tuple<int, int>(myPower, enemyPower);
        }

        /// <summary>
        /// Obtiene las almas oscuras que tienes
        /// </summary>
        /// <returns></returns>
        public int GetDarkSouls()
        {
            int darkSouls = -1;

            var screenCapture = UtilsAdb.GetBitmapCapture();
            screenCapture = CropBitMap(screenCapture, new Rectangle(582, 296, 180, 26));

            //Borra la imagen de las almas para que no falle el ocr
            for (int i = 0; i < 180; i++)
            {
                if (!UtilsAdb.AreColorsSimilar(UtilsAdb.GetPixelColor(screenCapture, i, 25), Color.FromArgb(85, 66, 73)))
                {
                    for (int y = 0; y <= 25; y++)
                    {
                        int startPoint = i - 10;
                        for (int x = 0; x <= 24; x++)
                        {
                            try
                            {
                                screenCapture.SetPixel(startPoint, y, Color.FromArgb(0, 0, 0));
                            }
                            catch
                            {
                                //
                            }
                            
                            startPoint++;
                        }
                    }
                    break;
                }
            }

            var text = OcrEngineResult(
                new Rectangle(),
                new EngineWhiteListConfig
                {
                    Value = Config.GetCharWhitelistBattlePower()
                },
                screenCapture
            );

            int.TryParse(text, out darkSouls);

            return darkSouls;
        }

        #endregion

        #region Dust

        /// <summary>
        /// Obtiene la etapa midiante ocr
        /// </summary>
        /// <returns></returns>
        public int GetDustStep()
        {
            int result = -1;

            while (result == -1)
            {

                var text = OcrEngineResult(
                    new Rectangle(510, 22, 92, 23),
                    new EngineWhiteListConfig
                    {
                        Value = Config.GetCharWhitelistPortal()
                    }
                );

                try
                {
                    int.TryParse(text.Substring(0, 1), out result);
                }
                catch (Exception ex)
                {
                    throw new Exception($"{ex.Message} : {text}");
                }
            }

            return result;
        }

        /// <summary>
        /// Obtiene el poder de la batalla en la arena
        /// </summary>
        /// <returns></returns>
        public Tuple<int, int> GetDustPlayersPower()
        {
            Bitmap ScreenCapture = UtilsAdb.GetBitmapCapture();
            int myPower = -1;
            int enemyPower = -1;

            var text = OcrEngineResult(
                new Rectangle(200, 267, 134, 21),
                new EngineWhiteListConfig
                {
                    Value = Config.GetCharWhitelistBattlePower()
                },
                ScreenCapture
            );

            int.TryParse(text, out myPower);

            text = OcrEngineResult(
                new Rectangle(440, 267, 134, 21),
                new EngineWhiteListConfig
                {
                    Value = Config.GetCharWhitelistBattlePower()
                },
                ScreenCapture
            );

            int.TryParse(text, out enemyPower);

            string MyPower = myPower.ToString("N0", System.Globalization.CultureInfo.GetCultureInfo("de"));
            string EnemyPower = enemyPower.ToString("N0", System.Globalization.CultureInfo.GetCultureInfo("de"));

            Writer.WriteWarning($"Mi poder: {MyPower}, poder enemigo: {EnemyPower}");

            return new Tuple<int, int>(myPower, enemyPower);
        }

        #endregion

        #region ImageTransformation

        /// <summary>
        /// Recorta el trozo de la imagen seleccionado
        /// </summary>
        /// <param name="source"></param>
        /// <param name="section"></param>
        /// <returns></returns>
        public Image CropImage(Image source, Rectangle section)
        {
            Bitmap bmp = new Bitmap(section.Width, section.Height);
            Graphics g = Graphics.FromImage(bmp);
            g.DrawImage(source, 0, 0, section, GraphicsUnit.Pixel);
            return bmp;
        }

        /// <summary>
        /// Recorta el trozo de la imagen seleccionado
        /// </summary>
        /// <param name="source"></param>
        /// <param name="section"></param>
        /// <returns></returns>
        public Bitmap CropBitMap(Bitmap source, Rectangle section)
        {
            if (section != new Rectangle())
            {
                Bitmap bmp = new Bitmap(section.Width, section.Height);
                Graphics g = Graphics.FromImage(bmp);
                g.DrawImage(source, 0, 0, section, GraphicsUnit.Pixel);
                return bmp;
            }
            else
            {
                return source;
            }
        }

        /// <summary>
        /// Modifica el tamaño de la imagen
        /// </summary>
        /// <param name="image"></param>
        /// <param name="percent"></param>
        /// <returns></returns>
        public static Bitmap ResizeImage(Image image, double percent)
        {
            var width = (int)(image.Width * percent);
            var height = (int)(image.Height * percent);

            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        /// <summary>
        /// Convierte la imagen en blanco y negro
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public Bitmap GrayScaleFilter(Image image)
        {
            Bitmap bmp = new Bitmap(image);
            int width = bmp.Width;
            int height = bmp.Height;
            int[] arr = new int[225];
            int i = 0;
            Color p;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    p = bmp.GetPixel(x, y);
                    int a = p.A;
                    int r = p.R;
                    int g = p.G;
                    int b = p.B;
                    int avg = (r + g + b) / 3;
                    avg = avg < 200 ? 255 : 0;
                    bmp.SetPixel(x, y, Color.FromArgb(a, avg, avg, avg));
                }
            }

            ((Image)bmp).Save(Directory.GetCurrentDirectory() + "/temp/GrayScaleFilter.png");

            return bmp;
        }

        #endregion

        #region TextTransformation

        /// <summary>
        /// Obtiene sólo los carácteres numéricos de un texto
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public string ToNumericOnly(string input)
        {
            Regex rgx = new Regex("[^0-9]");
            return rgx.Replace(input, "");
        }

        #endregion

    }
}
