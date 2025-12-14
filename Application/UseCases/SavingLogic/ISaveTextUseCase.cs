namespace Application.UseCases.SavingLogic
{
    public interface ISaveTextUseCase
    {
        SaveTextUseCaseResult Execute(SaveTextUseCaseParams args);        
    }
}
