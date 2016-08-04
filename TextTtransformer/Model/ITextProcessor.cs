using System;
namespace TextTtransformer.Model
{
    /// <summary>
    /// Представляет обработчик текстового файла
    /// </summary>
    interface ITextProcessor
    {
        /// <summary>
        /// Флаг, указывает завершена ли обработка файла
        /// </summary>
        bool IsComplete { get; }
        /// <summary>
        /// Запускает обработку
        /// </summary>
        /// <param name="conv">ProcessorParams.Convertor  - предикат обработки</param>
        void Process(ProcessorParams.Convertor conv);
        /// <summary>
        /// Запрос на прерывание обработки
        /// </summary>
        void StopRequest();
    }
}
