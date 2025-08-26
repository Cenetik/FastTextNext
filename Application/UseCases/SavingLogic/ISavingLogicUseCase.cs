namespace Application.UseCases.SavingLogic
{
    public interface ISavingLogicUseCase
    {
        SavingLogicResult Save(SavingLogicRequest request);
    }
}
