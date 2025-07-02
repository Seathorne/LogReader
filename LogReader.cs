namespace LogReader
{
    internal class LogReader(string filePath) : IDisposable
    {
        readonly StreamReader streamReader = new(filePath);

        public void Dispose()
        {
            ((IDisposable)streamReader).Dispose();
        }

        public IEnumerable<string?> ReadLines(int numLines)
        {
            int count = 0;
            string? line;
            while ((line = streamReader.ReadLine()) != null && count < numLines)
            {
                count++;
                yield return line;
            }
        }
    }
}
