using Results.UserInterface;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TimerDataBase.TableModels;

namespace TimerDataBase.Abstractions
{
    public interface ISeriesService
    {
        Task<IEnumerable<Serie>> GetAllSeriesOfUserAsync(string userId);
        Task<IEnumerable<Serie>> GetSeriesOfUserFilteredAsync(string userId, int limit,
            DateTime? startDate = null, DateTime? endDate = null, Category category = null,
            Cube cube = null, CategoryOption option = null);
        Task<Serie> GetLastSerieOfUserAsync(string userId);
        Task<Serie> GetSpecificSerieAsync(long serieId);
        Task<IEnumerable<Solve>> GetAllSolvesOfSerieAsync(long serieId);
        Task<Result> CreateNewSeriesAsync(string userId, Cube cube, CategoryOption option, DateTime startTime);
        Task<Result> CheckIfSerieCanBeCreatedAsync(string userId, Cube cube, CategoryOption option);
        Task<Result> DeleteSerieAsync(long serieId);
        Task<Result> DeleteAllSeriesOfUserAsync(string userId);
        Task<Result> AddSeriesNewSolveAsync(string userId, long seriesId, bool serieDone,
            TimeSpan duration, bool DNF, bool penalty, DateTime timeStamp, string scramble);
        Task<Result> DeleteSolveFromSeriesAsync(long serieId, long solveId, string userId);
        Task<Result> UpdateSolveFromSeriesAsync(long serieId, long solveId, string userId,
            TimeSpan duration, bool DNF, bool penalty);
        Task<Result> MarkSeriesFinishedAsync(long seriesId, bool finished);
        Task<long> GetUserRankedPositionCategoryAsync(string userId, Category category);
        Task<long> GetUserRankedPositionCubeAsync(string userId, Cube cube);
        Task<Serie> GetBestSerieOfCategoryAsync(Category category, CategoryOption option = null);
        Task<Serie> GetBestSerieOfCubeAsync(Cube cube, CategoryOption option = null);
        Task<IEnumerable<long>> GetTotalAmountOfSeriesAsync(IEnumerable<Category> categories);
        Task<long> GetAmountOfSolvesForUserAndCategoryAsync(string userId, Category category);
        Task<long> GetAmountOfSolvesForUserAndCubeAsync(string userId, Cube cube);
    }
}
