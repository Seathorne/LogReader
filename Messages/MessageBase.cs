using System.Diagnostics.CodeAnalysis;

namespace LogParser.Messages
{
    internal record MessageBase
    {
        #region Protected Constants

        [StringSyntax(StringSyntaxAttribute.Regex)]
        protected const string TimeStampPattern = @"(?<time>(?<hour>\d{2}):(?<minute>\d{2}):(?<second>\d{2})\.(?<millisecond>\d{3}))";

        [StringSyntax(StringSyntaxAttribute.Regex)]
        protected const string EventTimeStampPattern = @"(?<_time>(?<_hour>\d{2}):(?<_minute>\d{2}):(?<_second>\d{2}):(?<_millisecond>\d{3}))";

        [StringSyntax(StringSyntaxAttribute.Regex)]
        protected const string ThreadNumberPattern = @"\[(?<thread>\d+)\]";

        [StringSyntax(StringSyntaxAttribute.Regex)]
        protected const string ThreadNullPattern = @"\(null\)";

        [StringSyntax(StringSyntaxAttribute.Regex)]
        protected const string MessageLevelPattern = @"(?<level>INFO|DEBUG|WARNING|ERROR)";

        [StringSyntax(StringSyntaxAttribute.Regex)]
        protected const string EquipmentNumberPattern = @"Equipment (?<equipment>\d+)";

        #endregion

        #region Protected Static Methods

        protected static string ToPascalCase(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            return string.Join("", input.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(word => char.ToUpper(word[0]) + word[1..].ToLower()));
        }

        #endregion
    }
}