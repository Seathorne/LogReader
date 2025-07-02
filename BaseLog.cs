namespace LogReader
{
    internal interface IBaseLog
    {
        public IReadOnlyList<IBaseState> State { get; }
    }
}