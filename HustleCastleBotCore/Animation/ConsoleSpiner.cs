using System;

namespace HustleCastleBotCore
{
    /// <summary>
    /// Gestiona el spiner de carga
    /// </summary>
    public class ConsoleSpiner
    {
        int counter;
        public ConsoleSpiner()
        {
            counter = 0;
        }

        /// <summary>
        /// Inicia el spiner
        /// </summary>
        public void Turn()
        {
            counter++;
            switch (counter % 4)
            {
                case 0: Console.Write("/"); break;
                case 1: Console.Write("-"); break;
                case 2: Console.Write("\\"); break;
                case 3: Console.Write("|"); break;
            }
            Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
        }

        /// <summary>
        /// Detiene el spiner
        /// </summary>
        public void FinishTurn()
        {
            Console.Write(" ");
            Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
            Console.Write("\n");
        }
    }
}
