using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TextTtransformer.Model
{
    /// <summary>
    /// Инкапсулирует все параметры для обработки одного файла
    /// </summary>
    public class ProcessorParams
    {
        static string commaAndEtc = "[.,;:]";
        string source; // путь к источнику

        /// <summary>
        /// путь к оригиналу 
        /// </summary>
        public string Source
        {
            get { return source; }
            private set { source = value; }
        }

        string destination;  // путь к результату

        /// <summary>
        /// Путь к результату
        /// </summary>
        public string Destination
        {
            get { return destination; }
            private set { destination = value; }
        }
        bool commaRemove; // Удалять ли знаки препинания
        int wordSize; // минимальный размер слов в тексте

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="source">string имя файла источника</param>
        /// <param name="destination">string имя файла результата</param>
        /// <param name="commaRemove">bool удалять знаки препинания</param>
        /// <param name="wordSize">int минимальный размер слов</param>
        public ProcessorParams(string source, string destination, bool commaRemove, int wordSize)
        {
            this.Source = source;
            this.Destination = destination;
            this.commaRemove = commaRemove;
            this.wordSize = wordSize;
        }

        public delegate String Convertor(String source);
        /// <summary>
        /// Возвращает делегат Convertor
        /// </summary>
        /// <returns>Convertor</returns>
        public Convertor GetConvertor()
        {
            Convertor conv = line =>
            {
                String res;
                String expression = commaRemove ? commaAndEtc : "";
                res = Regex.Replace(line, expression, "");
                expression = String.Format("{0}{1}{2}", @"\b\w{1,", (wordSize-1).ToString(), @"}\b");
                res = Regex.Replace(res, expression, "");
                res = Regex.Replace(res, @"\s{1,}", " "); //  убираем лишние пробелы
                return res;
            };
            return conv;
        }
        

    }
}
