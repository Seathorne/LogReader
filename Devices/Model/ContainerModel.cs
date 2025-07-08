using LogParser.Devices.Enum;
using System.Text.RegularExpressions;

namespace LogParser.Devices.Model
{
    internal partial record ContainerModel : RecordModelBase
    {
        #region Properties

        public ContainerType ContainerType { get; init; }

        public string? LPN { get; init; }

        public string? LotNumber { get; init; }

        [GeneratedRegex(@"\w\d{4}")]
        private static partial Regex TotePattern { get; }

        [GeneratedRegex(@"[L|S]\d{11}")]
        private static partial Regex TrayPattern { get; }

        [GeneratedRegex(@"[R|LP]\d+")]
        private static partial Regex CasePattern { get; }

        [GeneratedRegex(@"LOT\-\d+")]
        private static partial Regex LotNumberPattern { get; }

        #endregion

        #region Constructors

        public ContainerModel(params string[] barcodes)
        {
            foreach (string barcode in barcodes)
            {
                if (barcode.Length == 5 && TotePattern.Match(barcode).Success)
                {
                    ContainerType = ContainerType.Tote;
                    LPN = barcode;
                }
                else if (barcode.Length == 12 && TrayPattern.Match(barcode).Success)
                {
                    ContainerType = barcode[0] == 'S' ? ContainerType.SmallTray : ContainerType.LargeTray;
                    LPN = barcode;
                }
                else if (CasePattern.Match(barcode).Success)
                {
                    ContainerType = ContainerType.HighbayCase;
                    LPN = barcode;
                }
                else if (LotNumberPattern.Match(barcode).Success)
                {
                    LotNumber = barcode;
                }
            }
        }

        public ContainerModel(ContainerType containerType, string? lpn, string? lotNumber = null)
        {
            ContainerType = containerType;
            LPN = lpn;
            LotNumber = lotNumber;
        }

        #endregion
    }
}