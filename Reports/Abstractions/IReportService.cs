using System.Threading.Tasks;

namespace Reports.Abstractions
{
    public interface IReportService
    {
        Task<string> GenerateSeriesReport(long serieId);
    }
}
