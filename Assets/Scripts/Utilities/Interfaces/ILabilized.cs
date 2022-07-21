namespace Utilities.Interfaces {
    /// <summary>
    /// Интерфейс: Активируемый элемент
    /// </summary>
    internal interface ILabilized {
        /// <summary>
        /// Активировать/деактивировать
        /// </summary>
        void SetActive(bool activate);
    }
}