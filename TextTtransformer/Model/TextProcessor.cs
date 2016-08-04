using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextTtransformer.Model
{
    /// <summary>
    /// Реализация обработчика файла
    /// </summary>
    public class TextProcessor : ITextProcessor
    {
        string sourceFileName;
        string destFileName;
        bool stopRequested = false;
        int bufferSize = 1024 * 4;
        bool isComplete;

        public delegate void Progress(String message);
        public event Progress onProgress;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="sourceFileName">string</param>
        /// <param name="destFileName">string</param>
        public TextProcessor(string sourceFileName, string destFileName)
        {
            this.sourceFileName = sourceFileName;
            this.destFileName = destFileName;
            this.onProgress += s => { }; // Лени ради 
        }

        #region Члены ITextProcessor

        
        public bool IsComplete
        {
            get { return isComplete; }
        }

        public async void Process(ProcessorParams.Convertor conv)
        {
            stopRequested = false;
            isComplete = false;
            using (StreamWriter writer = File.CreateText(destFileName))
            {

                using (StreamReader reader = File.OpenText(sourceFileName))
                {
                    await ConvertAsync(conv, writer, reader);
                    reader.Close();
                }
                writer.Close();
            }
            isComplete = true;
        }

        public void StopRequest()
        {
            stopRequested = true;
        }

        #endregion


        /// <summary>
        /// Обработчик
        /// </summary>
        /// <param name="conv">ProcessorParams.Convertor  -предикат обработки</param>
        /// <param name="writer">StreamWriter</param>
        /// <param name="reader">StreamReader </param>
        /// <returns>Task - запущена асинхронная задача</returns>
        private Task ConvertAsync(ProcessorParams.Convertor conv, StreamWriter writer, StreamReader reader)
        {
            var task = new Task(() =>
            {
                int index = 0;
                int count = 0;
                int lines=0;
                string currentLine = string.Empty;
                string tail = string.Empty; //хвост, последнее слово;
                do
                {
                    char[] buffer = new char[bufferSize];
                    count = reader.Read(buffer, 0, bufferSize);
                    currentLine = tail + (new string(buffer, 0, count));
                    index = currentLine.LastIndexOf('\n');
                    if (index > 0)
                    {
                        tail = currentLine.Substring(index + 1);
                        currentLine = currentLine.Substring(0, index);
                    }
                    else
                        tail = string.Empty;

                    foreach (string line in currentLine.Split('\n'))
                        writer.WriteLine(conv(line));

                    onProgress(string.Format("Файл:{0},результат {1}, строк обработано:{2}"
                        ,this.sourceFileName,this.destFileName, ++lines));
                    if (stopRequested)
                        return;
                } while (count > 0);
            });
            task.Start();
            return task;
        }
    }
}
