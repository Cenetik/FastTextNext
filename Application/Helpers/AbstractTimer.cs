namespace Application.Helpers
{
    public interface IBaseTimer
    {
        void SetIntervalAndAction(int milliseconds,Action onTickFunc);
        void Start();
    }    
}
