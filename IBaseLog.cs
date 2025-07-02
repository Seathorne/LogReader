namespace LogParser
{
    internal interface IBaseLog
    {
        public IReadOnlyList<IBaseState> State { get; }
    }
}