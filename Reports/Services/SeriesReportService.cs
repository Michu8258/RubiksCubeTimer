using Reports.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimerDataBase.Abstractions;
using TimerDataBase.TableModels;

namespace Reports.Services
{
    public class SeriesReportService : IReportService
    {
        private readonly ISeriesService _serieService;

        public SeriesReportService(ISeriesService serieService)
        {
            _serieService = serieService;
        }

        public async Task<string> GenerateSeriesReport(long serieId)
        {
            var serie = await _serieService.GetSpecificSerieAsync(serieId);

            if (serie == null)
            {
                return string.Format("{0}\n\n{1}", GetHeader(), NoDataMessage());
            }

            var solves = await _serieService.GetAllSolvesOfSerieAsync(serie.Identity);

            if (solves == null || solves.Count() < 0)
            {
                return string.Format("{0}\n\n{1}\n\n{2}", GetHeader(), GenerateSerieReport(serie), NoSolvesData());
            }

            return GenerateFullReport(serie, solves);
        }

        private string NoDataMessage()
        {
            return ApplicationResources.Reports.Texts.NoData;
        }

        private string NoSolvesData()
        {
            return ApplicationResources.Reports.Texts.NoSolvesData;
        }

        public string GenerateFullReport(Serie serie, IEnumerable<Solve> solves)
        {
            return string.Format("{0}\n\n{1}\n\n{2}",
                GetHeader(),
                GenerateSerieReport(serie),
                GenerateSolvesContent(solves));
        }


        private string GenerateSerieReport(Serie serie)
        {
            return string.Format(ApplicationResources.Reports.Texts.Serie,
                ConvertDateTimeToString(serie.StartTimeStamp),
                serie.Cube.Category.Name,
                string.Format("{0} {1} {2}", serie.Cube.Manufacturer.Name, serie.Cube.ModelName, serie.Cube.PlasticColor.Name),
                serie.SerieOption.Name,
                ConvertTimeSpanToString(serie.ShortestResult),
                ConvertTimeSpanToString(serie.LongestResult),
                ConvertTimeSpanToString(serie.AverageTime),
                ConvertTimeSpanToString(serie.MeanOf3),
                ConvertTimeSpanToString(serie.AverageOf5),
                serie.SolvesAmount,
                BooleanToString(serie.AtLeastOneDNF));
        }

        private string GenerateSolvesContent(IEnumerable<Solve> solves)
        {
            var output = new StringBuilder();
            var index = 1;

            foreach (var solve in solves)
            {
                output.Append(GenerateSingleSolveContent(solve, index));
                if (index < solves.Count())
                {
                    output.Append("\n\n");
                }
                index++;
            }

            return output.ToString();
        }

        private string GenerateSingleSolveContent(Solve solve, int index)
        {
            return string.Format(ApplicationResources.Reports.Texts.Solve,
                index,
                ConvertDateTimeToString(solve.FinishTimeSpan),
                ConvertTimeSpanToString(solve.Duration),
                BooleanToString(solve.DNF),
                BooleanToString(solve.PenaltyTwoSeconds),
                CheckScrambleLength(solve.Scramble));
        }

        private string GetHeader()
        {
            return string.Format(ApplicationResources.Reports.Texts.SeriesHeader,
                ConvertDateTimeToString(DateTime.Now));
        }

        private string ConvertDateTimeToString(DateTime dateTime)
        {
            return dateTime.ToString("dd-MM-yyyy HH:mm:ss");
        }

        private string ConvertTimeSpanToString(TimeSpan span)
        {
            return span.ToString(@"hh\:mm\:ss\.fff");
        }

        private string BooleanToString(bool value)
        {
            if (value)
            {
                return ApplicationResources.Reports.Texts.True;
            }

            return ApplicationResources.Reports.Texts.False;
        }

        private string CheckScrambleLength(string scramble)
        {
            if (string.IsNullOrEmpty(scramble))
            {
                return ApplicationResources.Reports.Texts.NoScramble;
            }

            return scramble;
        }
    }
}
