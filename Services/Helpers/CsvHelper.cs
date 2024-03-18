using System.Globalization;
using CsvHelper;

namespace Services;

public class CsvHelper
{
    public static void ExportToCsv<T>(IEnumerable<T> data, string filePath)
    {
        using (var writer = new StreamWriter(filePath))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(data);
        }
    }
}
