using System.Collections.Immutable;

namespace LogParser.Common.Records;

internal record Container(
    string? Barcode,
    string? LotBarcode,
    IImmutableSet<string> MiscBarcodes)
{
    #region Constructors

    public Container()
        : this(null, null, [])
    {
    }

    public Container(string? barcode = null, string? lotBarcode = null)
        : this(barcode, lotBarcode, [])
    {
    }

    #endregion

    #region Methods

    public static Container FromBarcodes(params string[] barcodes)
    {
        var miscBarcodes = new HashSet<string>();
        string? barcode = null;
        string? lotBarcode = null;

        foreach (string code in barcodes)
        {
            switch (code)
            {
                case ['R', ..]:
                    barcode = code;
                    break;
                case ['L', 'O', 'T', '-', ..]:
                    lotBarcode = code;
                    break;
                default:
                    miscBarcodes.Add(code);
                    break;
            }
        }

        return new Container(barcode, lotBarcode, [.. miscBarcodes]);
    }

    #endregion
}

