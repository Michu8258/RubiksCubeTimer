using Results.UserInterface;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TimerDataBase.TableModels;

namespace TimerDataBase.Abstractions
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryOption>> GetAllCategoryOptionsAsync();
        Task<CategoryOption> GetSingleCategoryOptionAsync(string name);
        Task<CategoryOption> GetSingleCategoryOptionAsync(int identity);
        Task<IEnumerable<CategoryOption>> GetOptionsForCubeAsync(Cube cube);
        Task<Result> AddCategoryOptionAsync(string name);
        Task<Result> DeleteCategoryOptionAsync(string name);
        Task<Result> UpdateCategoryOptionAsync(int identity, string name);
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
        Task<Category> GetCategoryAsync(int identity);
        Task<Category> GetCategoryAsync(string name);
        Task<Result> AddCategoryAsync(string name, IEnumerable<CategoryOption> optionsSet, TimeSpan acceptableTime);
        Task<Result> DeleteCategoryAsync(string name);
        Task<Result> DeleteCategoryAsync(int identity);
        Task<Result> UpdateCategoryAsync(int identity, string name, IEnumerable<CategoryOption> optionsSet, TimeSpan acceptableTime);
    }
}
