using HustleCastleBotCore.Commands;
using Microsoft.Win32.SafeHandles;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace HustleCastleBotCore
{
    /// <summary>
    /// Clase para enviar comandos a la consola de Windows
    /// </summary>
    public class Command : IDisposable
    {
        /// <summary>
        /// Ruta del archivo adb.exe
        /// </summary>
        public readonly string NoxPath = $@"nox\\nox_adb.exe";
        public readonly string OcrPath = $@"ocr\\tesseract.exe";
        Process process = null;

        /// <summary>
        /// Ejecuta un comando en la consola de Windows
        /// </summary>
        /// <param name="arguments"></param>
        public void Exec(string arguments, CommandEnum type)
        {
            string output = string.Empty;

            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            switch (type)
            {
                case CommandEnum.Adb:
                    processStartInfo.FileName = $@"{NoxPath}";
                    break;
                case CommandEnum.Ocr:
                    processStartInfo.FileName = $@"{OcrPath}";
                    break;
                default:
                    break;
            }
            processStartInfo.WorkingDirectory = $@"{Directory.GetCurrentDirectory()}";
            processStartInfo.Arguments = $@"{arguments}";
            processStartInfo.CreateNoWindow = true;
            processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            processStartInfo.UseShellExecute = true;

            process = Process.Start(processStartInfo);
            process.Dispose();
        }

        /// <summary>
        /// Ejecuta un comando en la consola de Windows y obtiene la respuesta
        /// </summary>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public string ReturnExec(string arguments, CommandEnum type)
        {
            string output = string.Empty;

            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            processStartInfo.RedirectStandardOutput = true;
            switch (type)
            {
                case CommandEnum.Adb:
                    processStartInfo.FileName = $@"{NoxPath}";
                    break;
                case CommandEnum.Ocr:
                    processStartInfo.FileName = $@"{OcrPath}";
                    break;
                default:
                    break;
            }
            processStartInfo.WorkingDirectory = $@"{Directory.GetCurrentDirectory()}";
            processStartInfo.Arguments = $@"{arguments}";
            processStartInfo.CreateNoWindow = true;
            processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            processStartInfo.UseShellExecute = false;

            using (process = Process.Start(processStartInfo))
            {
                using (StreamReader streamReader = process.StandardOutput)
                {
                    output = streamReader.ReadToEnd();
                }
            }

            return output;
        }

        bool disposed = false;
        SafeHandle handle = new SafeFileHandle(IntPtr.Zero, true);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                handle.Dispose();
                process.Dispose();
            }

            disposed = true;
        }
    }
}
