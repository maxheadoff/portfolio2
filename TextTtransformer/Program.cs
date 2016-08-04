using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using TextTtransformer.Model;

namespace TextTtransformer
{
    class Program
    {
        static bool stopRequested = false;
        static Logger logger = LogManager.GetCurrentClassLogger();
        static bool isComplete = false;
        static string fatalMessage = "Аргументов не может быть меньше одного, пожалуйста, читайте readme";

        static string searchPattern = "*.txt"; // Шаблон только для текстовых файлов
        static string resultFileNamePattern = "{0}/{1}.result"; // Шаблон для формирования имен файлов с резултатами

        static void Main(string[] args)
        {

            if(args.Length<1)
            {
                logger.Fatal(fatalMessage);
                Console.WriteLine(fatalMessage);
                return;
            }
            try
            {

                Console.WriteLine("Для прерывания, нажмите Q или q ");
                Console.WriteLine();
                switch (args[0])
                {
                    case "-d": RunDirectory(args); break; // вместо файлов, указаны папки, откуда брать файлы для обработки, и куда складывать
                    case "-f": RunCSVFile(args[1]); break; // указывается файл для пакетной обработки.
                    case "-h" :
                    case "?":
                    case "help": Help(); break;
                    default:
                        {
                            int wordSize;
                            if (int.TryParse(args[2], out wordSize))
                                RunSingle(args[0], args[1], isYes(args[3]), int.Parse(args[2]));
                            else
                                logger.Error("Не верно указано значение минимального размера слова");
                            break; // один указанный файл
                        }
                }
            
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Ошибка в приложении",null);
                
            }
            Console.WriteLine();
            Console.WriteLine("Обработка закончена, нажмите Enter для выхода");
            Console.ReadLine();
        }

        /// <summary>
        /// Пытается вывести в консоль текст из readme.txt
        /// </summary>
        private static void Help()
        {
            try
            {
                Console.Write(File.ReadAllText("readme.txt"));
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Не удалось отобразить readme.txt файл");
            }
           
        }

        /// <summary>
        /// Обрабатывается один файл
        /// </summary>
        /// <param name="sourceFileName">string имя оригинала</param>
        /// <param name="destFileName">string имя файла для записи результатов</param>
        /// <param name="isCommaRemove">bool удалять ли знаки препинания</param>
        /// <param name="wordSize">int максимальная длина слова</param>
        private static void RunSingle(string sourceFileName, string destFileName, bool isCommaRemove, int wordSize)
        {
            ProcessorParams pp = new ProcessorParams(sourceFileName, destFileName, isCommaRemove, wordSize);
            TextProcessor tpa = new TextProcessor(sourceFileName, destFileName);
            int cursorPosition = Console.CursorTop;
            tpa.onProgress += message =>
            {
                Console.SetCursorPosition(0, cursorPosition); // прогресс пишем в одну и туже строку
                Console.Write(message);
            };

            tpa.Process(pp.GetConvertor());
            while (!tpa.IsComplete)
            {
                if (Console.KeyAvailable)
                    if (Console.ReadKey().Key == ConsoleKey.Q)
                    {
                        tpa.StopRequest();
                        stopRequested = true;
                    }
                Thread.Sleep(100);
            }
            Console.WriteLine(); // Каждый файл с новой стороки
        }

        
        /// <summary>
        /// Обрабатываем каталог
        /// </summary>
        /// <param name="args"></param>
        private static void RunDirectory(string[] args)
        {
            string sourceDir = args[1];
            string destDir = args[2];
            int wordSize = 0;
            bool commaRemove = false;
            // реализуем возможность для значений по умолчанию
            try { wordSize = int.Parse(args[3]); }
            catch { }
            try { commaRemove = isYes(args[4]); }
            catch { }
            foreach (string fileName in Directory.GetFiles(sourceDir, searchPattern))
            {
                string[] splittedName = fileName.Split('\\');
                RunSingle(fileName, string.Format(resultFileNamePattern, destDir, splittedName[splittedName.Length - 1]), commaRemove, wordSize);
                if (stopRequested)
                    break;
            }
               isComplete = true;
               
        }

        /// <summary>
        /// Обрабатываем пакетный файл
        /// </summary>
        /// <param name="csvFileName">string</param>
        private static void RunCSVFile(string csvFileName)
        {
            string[] items = File.ReadAllLines(csvFileName);
            
            foreach (string item in items)
            {
                string[] args = item.Split(',');
                RunSingle(args[0], args[1], isYes(args[3]), int.Parse(args[2]));
                if (stopRequested)
                    break;
            }
            isComplete = true;
        }
  

         /// <summary>
        /// Возвращает true если строка "утвердительная"
        /// </summary>
        /// <param name="arg">string </param>
        /// <returns>bool</returns>
        private static bool isYes(string arg)
        {
            return (arg.ToUpper().Equals("Y") | arg.ToUpper().Equals("YES") | arg.ToUpper().Equals("TRUE"));
        }
    }
}
