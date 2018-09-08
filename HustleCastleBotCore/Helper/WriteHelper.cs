using System;

namespace HustleCastleBotCore
{
    /// <summary>
    /// Clase que cambia el color por tipo de mensaje
    /// </summary>
    public class WriteHelper
    {
        /// <summary>
        /// Escribe una linea
        /// </summary>
        /// <param name="text"></param>
        public void WriteLine(string text)
        {
            Console.WriteLine(text);
        }

        /// <summary>
        /// Escribe un texto
        /// </summary>
        /// <param name="text"></param>
        public void Write(string text)
        {
            Console.Write(text);
        }

        /// <summary>
        /// Escribe un mensaje en rojo
        /// </summary>
        /// <param name="text"></param>
        public void WriteError(string text)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            WriteLine(text);
            Console.ResetColor();
        }

        /// <summary>
        /// Escribe un mensaje en morado
        /// </summary>
        /// <param name="text"></param>
        public void WriteWarning(string text)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            WriteLine(text);
            Console.ResetColor();
        }

        /// <summary>
        /// Escribe un mensaje en verde
        /// </summary>
        /// <param name="text"></param>
        public void WriteSuccessfully(string text)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            WriteLine(text);
            Console.ResetColor();
        }

        /// <summary>
        /// Escribe un mensaje en gris
        /// </summary>
        /// <param name="text"></param>
        public void WriteInfo(string text)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            WriteLine(text);
            Console.ResetColor();
        }
    }
}
