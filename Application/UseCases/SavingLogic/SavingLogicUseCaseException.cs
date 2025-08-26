
namespace Application.UseCases.SavingLogic
{
    [Serializable]
    internal class SavingLogicUseCaseException : Exception
    {
        public SavingLogicUseCaseException()
        {
        }

        public SavingLogicUseCaseException(string? message) : base(message)
        {
        }

        public SavingLogicUseCaseException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}