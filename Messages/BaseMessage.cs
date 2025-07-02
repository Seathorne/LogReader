namespace LogParser.Messages
{
    internal record BaseMessage
    {
        #region Protected Static Methods

        protected static string ToPascalCase(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            return char.ToUpper(input[0]) + input[1..].ToLower();
        }

        #endregion
    }
}