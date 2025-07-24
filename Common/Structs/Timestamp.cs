namespace LogParser.Common.Structs;

internal readonly record struct Timestamp(
    DateTimeOffset DateTimeOffset,
    int LineNumber) : IComparable<Timestamp>
{
    #region Properties

    public TimeOnly TimeOnly => TimeOnly.FromDateTime(DateTimeOffset.DateTime);

    #endregion

    #region Methods

    public int CompareTo(Timestamp other)
    {
        var dateTimeComparison = DateTimeOffset.Compare(DateTimeOffset, other.DateTimeOffset);
        if (dateTimeComparison != 0)
            return dateTimeComparison;
        else return LineNumber.CompareTo(other.LineNumber);
    }

    public static bool operator <(Timestamp left, Timestamp right) =>
        left.CompareTo(right) < 0;

    public static bool operator >(Timestamp left, Timestamp right) =>
        left.CompareTo(right) > 0;

    public static bool operator <=(Timestamp left, Timestamp right) =>
        left.CompareTo(right) <= 0;

    public static bool operator >=(Timestamp left, Timestamp right) =>
        left.CompareTo(right) >= 0;

    #endregion
}