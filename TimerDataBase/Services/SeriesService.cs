using Microsoft.EntityFrameworkCore;
using Results.UserInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TimerDataBase.Abstractions;
using TimerDataBase.DataBaseContext;
using TimerDataBase.TableModels;

namespace TimerDataBase.Services
{
    public class SeriesService : ISeriesService
    {
        private readonly TimerDBContext _dbContext;

        public SeriesService(TimerDBContext context)
        {
            _dbContext = context;
        }
        public async Task<IEnumerable<Serie>> GetAllSeriesOfUserAsync(string userId)
        {
            return await GetUserSeriesAsync(userId);
        }

        public async Task<IEnumerable<Serie>> GetSeriesOfUserFilteredAsync(string userId,
            int limit = 0, DateTime? startDate = null, DateTime? endDate = null,
            Category category = null, Cube cube = null, CategoryOption option = null)
        {
            var seriesCollection = await GetUserSeriesAsync(userId);
            if (seriesCollection.Any())
            {
                if (startDate.HasValue)
                {
                    seriesCollection = seriesCollection.Where(s => s.StartTimeStamp >= startDate).ToList();
                }

                if (endDate.HasValue)
                {
                    seriesCollection = seriesCollection.Where(s => s.StartTimeStamp <= endDate).ToList();
                }

                if (category != null)
                {
                    seriesCollection = seriesCollection.Where(s => s.Cube.Category == category).ToList();
                }

                if (cube != null)
                {
                    seriesCollection = seriesCollection.Where(s => s.Cube == cube).ToList();
                }

                if (option != null)
                {
                    seriesCollection = seriesCollection.Where(s => s.SerieOption == option).ToList();
                }

                if (limit > 0 && seriesCollection.Count() > limit)
                {
                    seriesCollection = seriesCollection.Take(limit).ToList();
                }
            }

            return seriesCollection;
        }

        public async Task<Serie> GetLastSerieOfUserAsync(string userId)
        {
            return (await GetUserSeriesAsync(userId)).LastOrDefault();
        }

        public async Task<Serie> GetSpecificSerieAsync(long serieId)
        {
            return await _dbContext.Series
                .Include(s => s.Cube)
                .Include(s => s.Cube.Category)
                .Include(s => s.Cube.Manufacturer)
                .Include(s => s.Cube.PlasticColor)
                .Include(s => s.SerieOption)
                .FirstOrDefaultAsync(s => s.Identity == serieId);
        }

        public async Task<IEnumerable<Solve>> GetAllSolvesOfSerieAsync(long serieId)
        {
            var serie = await _dbContext.Series.FirstOrDefaultAsync(s => s.Identity == serieId);

            if (serie != null)
            {
                return await _dbContext.Solves.Where(s => s.Serie == serie)
                    .OrderBy(s => s.FinishTimeSpan)
                    .ToListAsync();
            }

            return new List<Solve>();
        }

        public async Task<Result> CreateNewSeriesAsync(string userId, Cube cube, CategoryOption option, DateTime startTime)
        {
            var canBeCreated = await CheckCreateSerieConditionsAsync(userId, cube, option);

            if (!canBeCreated.IsFaulted)
            {
                var serie = new Serie()
                {
                    UserIdentity = userId,
                    Cube = cube,
                    StartTimeStamp = startTime,
                    LongestResult = TimeSpan.FromMilliseconds(0),
                    ShortestResult = TimeSpan.FromMilliseconds(0),
                    AtLeastOneDNF = false,
                    SerieOption = option,
                    SerieFinished = false,
                    SolvesAmount = 0,
                };

                _dbContext.Series.Add(serie);
                var result = await _dbContext.SaveChangesAsync();

                if (result > 0)
                {
                    return new Result(string.Empty, false)
                    {
                        JsonData = serie.Identity
                    };
                }
                else
                {
                    return new Result(ApplicationResources.TimerDB.Messages.SerieCreatingError);
                }
            }
            else
            {
                return canBeCreated;
            }
        }

        public async Task<Result> CheckIfSerieCanBeCreatedAsync(string userId, Cube cube, CategoryOption option)
        {
            return await CheckCreateSerieConditionsAsync(userId, cube, option);
        }

        public async Task<Result> DeleteSerieAsync(long serieId)
        {
            var serie = await _dbContext.Series.FirstOrDefaultAsync(s => s.Identity == serieId);
            if (serie == null)
            {
                return new Result(ApplicationResources.TimerDB.Messages.SerieDoesNotExist);
            }

            else
            {
                var solves = await _dbContext.Solves
                    .Where(s => s.Serie == serie)
                    .ToListAsync();

                var solvesAmount = solves.Count();

                _dbContext.Solves.RemoveRange(solves);
                _dbContext.Series.Remove(serie);
                var result = await _dbContext.SaveChangesAsync();

                if(result == solvesAmount + 1)
                {
                    return new Result(string.Empty, false);
                }
                else
                {
                    return new Result(ApplicationResources.TimerDB.Messages.SerieDeletionFailed);
                }
            }
        }

        public async Task<Result> DeleteAllSeriesOfUserAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return new Result(ApplicationResources.TimerDB.Messages.NullArgument);
            }

            var series = await _dbContext.Series
                .Where(s => s.UserIdentity == userId)
                .ToListAsync();

            var seriesAmount = series.Count();

            if (!series.Any())
            {
                return new Result(string.Empty, false);
            }
            else
            {
                var errors = 0;

                foreach (var serie in series)
                {
                    if ((await DeleteSerieAsync(serie.Identity)).IsFaulted)
                    {
                        errors++;
                    }
                }

                if (errors == 0)
                {
                    return new Result(string.Empty, false);
                }
                else
                {
                    return new Result(string.Format(ApplicationResources.TimerDB.Messages.DeletingUserSerieFailed,
                        userId, errors, seriesAmount));
                }
            }
        }

        public async Task<Result> AddSeriesNewSolveAsync(string userId, long seriesId, bool serieDone,
            TimeSpan duration, bool DNF, bool penalty, DateTime timeStamp, string scramble)
        {
            if (string.IsNullOrEmpty(userId) || seriesId < 1 || duration.TotalMilliseconds < 1)
            {
                return new Result(ApplicationResources.TimerDB.Messages.NullArgument);
            }

            var serie = await _dbContext.Series
                .Include(s => s.Cube)
                .Include(s => s.Cube.Category)
                .FirstOrDefaultAsync(s => s.Identity == seriesId);

            if (serie != null)
            {
                if (serie.SerieFinished)
                {
                    return new Result(ApplicationResources.TimerDB.Messages.SerieAlreadyFinished);
                }

                if (serie.Cube.Category.ShortestAcceptableResult > duration)
                {
                    return new Result(string.Format(
                        ApplicationResources.TimerDB.Messages.SolveBelowAcceptableTime,
                        duration.ToString(@"hh\:mm\:ss\.fff"),
                        serie.Cube.Category.ShortestAcceptableResult.ToString(@"hh\:mm\:ss\.fff")));
                }

                if (serie.UserIdentity == userId)
                {
                    var solve = new Solve()
                    {
                        Serie = serie,
                        FinishTimeSpan = timeStamp,
                        Duration = penalty ? duration + TimeSpan.FromSeconds(2) : duration,
                        DNF = DNF,
                        PenaltyTwoSeconds = penalty,
                        Scramble = scramble,
                    };

                    _dbContext.Solves.Add(solve);
                    var result = await _dbContext.SaveChangesAsync();
                    if (result > 0)
                    {
                        var seriesUpdated = await UpdateTimesInSerieAsync(seriesId, serieDone);
                        if (seriesUpdated)
                        {
                            return new Result(string.Empty, false);
                        }
                        else
                        {
                            return new Result(ApplicationResources.TimerDB.Messages.SeriesUpdateFailed);
                        }
                    }
                    else
                    {
                        return new Result(ApplicationResources.TimerDB.Messages.SolveAddFailed);
                    }
                }
                else
                {
                    return new Result(ApplicationResources.TimerDB.Messages.SeriesBelongsToDiffuser);
                }
            }
            else
            {
                return new Result(ApplicationResources.TimerDB.Messages.SerieDoesNotExist);
            }
        }

        public async Task<Result> DeleteSolveFromSeriesAsync(long serieId, long solveId, string userId)
        {
            if (string.IsNullOrEmpty(userId) || serieId < 1)
            {
                return new Result(ApplicationResources.TimerDB.Messages.NullArgument);
            }

            var serie = await _dbContext.Series.FirstOrDefaultAsync(s => s.Identity == serieId);
            if (serie != null)
            {
                if (serie.UserIdentity == userId)
                {
                    var solves = await _dbContext.Solves
                        .Where(s => s.Identity == solveId)
                        .ToListAsync();

                    if (solves.Any())
                    {
                        _dbContext.Solves.RemoveRange(solves);
                        var result = await _dbContext.SaveChangesAsync();
                        if (result > 0)
                        {
                            var serieUpdated = await UpdateTimesInSerieAsync(serieId, serie.SerieFinished);
                            if (serieUpdated)
                            {
                                return new Result(string.Empty, false);
                            }
                            else
                            {
                                return new Result(ApplicationResources.TimerDB.Messages.SeriesUpdateFailed);
                            }
                        }
                        else
                        {
                            return new Result(ApplicationResources.TimerDB.Messages.SolvedeleteFailed);
                        }
                    }
                    else
                    {
                        return new Result(ApplicationResources.TimerDB.Messages.SolveDoesNotExist);
                    }
                }
                else
                {
                    return new Result(ApplicationResources.TimerDB.Messages.SeriesBelongsToDiffuser);
                }
            }
            else
            {
                return new Result(ApplicationResources.TimerDB.Messages.SerieDoesNotExist);
            }
        }

        public async Task<Result> UpdateSolveFromSeriesAsync(long serieId, long solveId, string userId,
            TimeSpan duration, bool DNF, bool penalty)
        {
            if (string.IsNullOrEmpty(userId) || serieId < 1 || duration.TotalMilliseconds < 1)
            {
                return new Result(ApplicationResources.TimerDB.Messages.NullArgument);
            }

            var serie = await _dbContext.Series.FirstOrDefaultAsync(s => s.Identity == serieId);
            if (serie != null)
            {
                if (serie.UserIdentity == userId)
                {
                    var solve = await _dbContext.Solves.FirstOrDefaultAsync(s => s.Identity == solveId);
                    if (solve != null)
                    {
                        if (solve.DNF != DNF || solve.PenaltyTwoSeconds != penalty || solve.Duration != duration)
                        {
                            solve.Duration = penalty ? duration + TimeSpan.FromSeconds(2) : duration;
                            solve.DNF = DNF;
                            solve.PenaltyTwoSeconds = penalty;

                            var result = await _dbContext.SaveChangesAsync();
                            if (result > 0)
                            {
                                var serieUpdated = await UpdateTimesInSerieAsync(serieId, serie.SerieFinished);
                                if (serieUpdated)
                                {
                                    return new Result(string.Empty, false);
                                }
                                else
                                {
                                    return new Result(ApplicationResources.TimerDB.Messages.SeriesUpdateFailed);
                                }
                            }
                            else
                            {
                                return new Result(ApplicationResources.TimerDB.Messages.SolveUpdateFailed);
                            }
                        }

                        return new Result(string.Empty, false);
                    }
                    else
                    {
                        return new Result(ApplicationResources.TimerDB.Messages.SolveDoesNotExist);
                    }
                }
                else
                {
                    return new Result(ApplicationResources.TimerDB.Messages.SeriesBelongsToDiffuser);
                }
            }
            else
            {
                return new Result(ApplicationResources.TimerDB.Messages.SerieDoesNotExist);
            }
        }

        public async Task<Result> MarkSeriesFinishedAsync(long seriesId, bool finished)
        {
            if (seriesId <= 0)
            {
                return new Result(ApplicationResources.TimerDB.Messages.NullArgument);
            }

            var series = await _dbContext.Series.FirstOrDefaultAsync(s => s.Identity == seriesId);
            series.SerieFinished = finished;
            var result = await _dbContext.SaveChangesAsync();

            if (result > 0)
            {
                return new Result(string.Empty, false);
            }
            else
            {
                return new Result(ApplicationResources.TimerDB.Messages.SerieFinishUpdateFailed);
            }
        }

        public async Task<long> GetUserRankedPositionCategoryAsync(string userId, Category category)
        {
            if (category != null)
            {
                bool userHasCategory = (await _dbContext.CubesCollections
                    .Where(c => c.UserIdentity == userId && c.Cube.Category == category)
                    .ToListAsync()).Any();

                if (userHasCategory)
                {
                    var allBestSeries = await _dbContext.Series
                        .Include(s => s.Cube)
                        .Include(s => s.Cube.Category)
                        .Include(s => s.Cube.Manufacturer)
                        .Include(s => s.Cube.PlasticColor)
                        .Include(s => s.SerieOption)
                        .Where(s => s.Cube.Category == category)
                        .OrderBy(s => s.ShortestResult)
                        .ToListAsync();

                    return GetRankingUserIndex(allBestSeries, userId);
                }
            }

            return - 1;
        }

        public async Task<long> GetUserRankedPositionCubeAsync(string userId, Cube cube)
        {
            if (cube != null)
            {
                bool userHasCube = (await _dbContext.CubesCollections
                    .Where(c => c.UserIdentity == userId && c.Cube == cube)
                    .ToListAsync()).Any();

                if (userHasCube)
                {
                    var allBestSeries = await _dbContext.Series
                        .Include(s => s.Cube)
                        .Include(s => s.Cube.Category)
                        .Include(s => s.Cube.Manufacturer)
                        .Include(s => s.Cube.PlasticColor)
                        .Include(s => s.SerieOption)
                        .Where(s => s.Cube == cube)
                        .OrderBy(s => s.ShortestResult)
                        .ToListAsync();

                    return GetRankingUserIndex(allBestSeries, userId);
                }
            }

            return -1;
        }

        public async Task<Serie> GetBestSerieOfCategoryAsync(Category category, CategoryOption option = null)
        {
            if (category == null)
            {
                return null;
            }

            if (option == null)
            {
                return await GetBestSerieAsync(s => s.Cube.Category == category);
            }
            else
            {
                return await GetBestSerieAsync(s => s.Cube.Category == category && s.SerieOption == option);
            }
        }

        public async Task<Serie> GetBestSerieOfCubeAsync(Cube cube, CategoryOption option = null)
        {
            if (cube == null)
            {
                return null;
            }

            if (option == null)
            {
                return await GetBestSerieAsync(s => s.Cube == cube);
            }
            else
            {
                return await GetBestSerieAsync(s => s.Cube == cube && s.SerieOption == option);
            }
        }

        public async Task<IEnumerable<long>> GetTotalAmountOfSeriesAsync(IEnumerable<Category> categories)
        {
            if (categories == null)
            {
                return null;
            }

            var output = new List<long>();

            foreach (var category in categories)
            {
                output.Add(await _dbContext.Series
                    .CountAsync(s => s.Cube.Category == category));
            }

            return output;
        }

        public async Task<long> GetAmountOfSolvesForUserAndCategoryAsync(string userId, Category category)
        {
            if(string.IsNullOrEmpty(userId) || category == null)
            {
                return 0;
            }

            return await _dbContext.Series
                .Include(s => s.Cube)
                .Include(s => s.Cube.Category)
                .Where(s => s.UserIdentity == userId && s.Cube.Category == category)
                .SumAsync(s => s.SolvesAmount);
        }

        public async Task<long> GetAmountOfSolvesForUserAndCubeAsync(string userId, Cube cube)
        {
            if (string.IsNullOrEmpty(userId) || cube == null)
            {
                return 0;
            }

            return await _dbContext.Series
                .Include(s => s.Cube)
                .Where(s => s.UserIdentity == userId && s.Cube == cube)
                .SumAsync(s => s.SolvesAmount);
        }

        private long GetRankingUserIndex(IEnumerable<Serie> series, string userId)
        {
            List<string> seenIds = new List<string>();
            foreach (var serie in series)
            {
                if (!seenIds.Contains(serie.UserIdentity))
                {
                    seenIds.Add(serie.UserIdentity);
                }

                if (serie.UserIdentity == userId)
                {
                    return seenIds.Count();
                }
            }

            return -1;
        }

        private async Task<bool> UpdateTimesInSerieAsync(long serieId, bool serieDone)
        {
            var serie = await _dbContext.Series.FirstOrDefaultAsync(s => s.Identity == serieId);
            if (serie != null)
            {
                var solves = await _dbContext.Solves
                    .Where(s => s.Serie == serie)
                    .ToListAsync();

                if (solves.Any())
                {
                    serie.LongestResult = solves.Max(s => s.Duration);
                    serie.ShortestResult = solves.Min(s => s.Duration);
                    serie.AverageTime = TimeSpan.FromMilliseconds(solves.Select(s => s.Duration.TotalMilliseconds).Average());
                    serie.AtLeastOneDNF = solves.Any(s => s.DNF);
                    serie.SerieFinished = serieDone;
                    serie.SolvesAmount = solves.Count;

                    if (solves.Count == 3)
                    {
                        serie.MeanOf3 = TimeSpan.FromMilliseconds(solves.Select(s => s.Duration.TotalMilliseconds).Average());
                    }
                    else
                    {
                        serie.MeanOf3 = TimeSpan.FromMilliseconds(0);
                    }

                    if (solves.Count == 5)
                    {
                        var bestSolve = solves.OrderBy(s => s.Duration).First().Duration.TotalMilliseconds;
                        var worstSolve = solves.OrderByDescending(s => s.Duration).First().Duration.TotalMilliseconds;
                        serie.AverageOf5 = TimeSpan.FromMilliseconds(((serie.AverageTime.TotalMilliseconds * 5) - bestSolve - worstSolve) / 3);
                    }
                    else
                    {
                        serie.AverageOf5 = TimeSpan.FromMilliseconds(0);
                    }
                }
                else
                {
                    serie.LongestResult = TimeSpan.FromMilliseconds(0);
                    serie.ShortestResult = TimeSpan.FromMilliseconds(0);
                    serie.AverageTime = TimeSpan.FromMilliseconds(0);
                    serie.AtLeastOneDNF = false;
                    serie.SerieFinished = serieDone;
                    serie.MeanOf3 = TimeSpan.FromMilliseconds(0);
                    serie.MeanOf3 = TimeSpan.FromMilliseconds(0);
                    serie.SolvesAmount = 0;
                }

                var result = await _dbContext.SaveChangesAsync();

                if (solves.Any())
                {
                    return result > 0;
                }

                return true;
            }

            return false;
        }

        private async Task<IEnumerable<Serie>> GetUserSeriesAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return new List<Serie>();
            }

            return await _dbContext.Series
                .Include(s => s.SerieOption)
                .Include(s => s.Cube)
                .Include(s => s.Cube.Category)
                .Include(s => s.Cube.Manufacturer)
                .Include(c => c.Cube.PlasticColor)
                .Where(s => s.UserIdentity == userId)
                .OrderBy(s => s.StartTimeStamp)
                .ThenBy(s => s.AverageTime)
                .ToListAsync();
        }

        private async Task<Result> CheckCreateSerieConditionsAsync(string userId, Cube cube, CategoryOption option)
        {
            if (string.IsNullOrEmpty(userId) || cube == null || option == null)
            {
                return new Result(ApplicationResources.TimerDB.Messages.NullArgument);
            }

            if (!await _dbContext.CubesCollections.Where(c => c.UserIdentity == userId).AnyAsync(c => c.Cube == cube))
            {
                return new Result(ApplicationResources.TimerDB.Messages.CubeNotInCollection);
            }

            if (!await _dbContext.CategoryoptionsSets.Where(co => co.Category == cube.Category).AnyAsync(co => co.Option == option))
            {
                return new Result(ApplicationResources.TimerDB.Messages.CategoryOptionNotInCategory);
            }

            return new Result(string.Empty, false);
        }

        private async Task<Serie> GetBestSerieAsync(Expression<Func<Serie, bool>> whereMatch)
        {
            return await _dbContext.Series
                .Include(s => s.Cube)
                .Include(s => s.Cube.Manufacturer)
                .Include(s => s.Cube.PlasticColor)
                .Include(s => s.SerieOption)
                .Where(whereMatch)
                .OrderBy(s => s.ShortestResult)
                .FirstOrDefaultAsync();
        }
    }
}
