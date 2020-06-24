using Microsoft.EntityFrameworkCore;
using Results.UserInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimerDataBase.Abstractions;
using TimerDataBase.DataBaseContext;
using TimerDataBase.TableModels;

namespace TimerDataBase.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly TimerDBContext _dbContext;

        public CategoryService(TimerDBContext context)
        {
            _dbContext = context;
        }

        public async Task<IEnumerable<CategoryOption>> GetAllCategoryOptionsAsync()
        {
            return await _dbContext.CategoryOptions
                .OrderBy(o => o.Name)
                .ToListAsync();
        }

        public async Task<CategoryOption> GetSingleCategoryOptionAsync(string name)
        {
            return (await GetAllCategoryOptionsAsync())
                .FirstOrDefault(o => o.Name == name);
        }

        public async Task<CategoryOption> GetSingleCategoryOptionAsync(int identity)
        {
            return (await GetAllCategoryOptionsAsync())
                .FirstOrDefault(o => o.Identity == identity);
        }

        public async Task<IEnumerable<CategoryOption>> GetOptionsForCubeAsync(Cube cube)
        {
            if (cube != null)
            {
                var category = cube.Category;

                if (category != null)
                {
                    return await _dbContext.CategoryoptionsSets
                        .Where(os => os.Category == category)
                        .OrderBy(os => os.Option.Name)
                        .Select(os => os.Option)
                        .ToListAsync();
                }
            }

            return new List<CategoryOption>();
        }

        public async Task<Result> AddCategoryOptionAsync(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return new Result(ApplicationResources.TimerDB.Messages.NullArgument);
            }

            if (await CheckIfCategoryOptionExistsAsync(name))
            {
                return new Result(string.Format(ApplicationResources.TimerDB.Messages.CategoryOptionAlreadyExists, name));
            }
            else
            {
                _dbContext.CategoryOptions.Add(new CategoryOption()
                {
                    Name = name,
                });
                if (await _dbContext.SaveChangesAsync() == 1)
                {
                    return new Result(string.Empty, false);
                }
                return new Result(string.Format(ApplicationResources.TimerDB.Messages.CategoryOptionAddFailed, name));
            }
        }

        public async Task<Result> DeleteCategoryOptionAsync(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return new Result(ApplicationResources.TimerDB.Messages.NullArgument);
            }

            if (!await CheckIfCategoryOptionExistsAsync(name))
            {
                return new Result(string.Format(ApplicationResources.TimerDB.Messages.CategoryOptionNotExist, name));
            }
            else
            {
                var option = (await GetSingleCategoryOptionAsync(name));
                if (await _dbContext.CategoryoptionsSets.CountAsync(os => os.Option == option) > 0)
                {
                    return new Result(string.Format(ApplicationResources.TimerDB.Messages.CategoryOptionCannotDelete, name));
                }
                else
                {
                    var categoryOptions = await _dbContext.CategoryOptions
                        .Where(o => o.Name == name)
                        .ToListAsync();

                    _dbContext.CategoryOptions.RemoveRange(categoryOptions);
                    var result = await _dbContext.SaveChangesAsync();

                    if (result == categoryOptions.Count())
                    {
                        return new Result(string.Empty, false);
                    }
                }
            }

            return new Result(string.Format(ApplicationResources.TimerDB.Messages.CategoryOptionDeleteFailed, name));
        }

        public async Task<Result> UpdateCategoryOptionAsync(int identity, string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return new Result(ApplicationResources.TimerDB.Messages.NullArgument);
            }
            else
            {
                var categoryOption = await GetSingleCategoryOptionAsync(identity);
                if (categoryOption == null)
                {
                    return new Result(string.Format(ApplicationResources.TimerDB.Messages.CategoryOptionNotExist, "unknown"));
                }
                else
                {
                    categoryOption.Name = name;
                    var result = await _dbContext.SaveChangesAsync();

                    if (result == 1)
                    {
                        return new Result(string.Empty, false);
                    }
                    else
                    {
                        return new Result(ApplicationResources.TimerDB.Messages.CategoryOptionUpdateFailed);
                    }
                }
            }
        }

        private async Task<bool> CheckIfCategoryOptionExistsAsync(string name)
        {
            return (await GetAllCategoryOptionsAsync())
                .Any(o => o.Name == name);
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            var categories = await _dbContext.Categories
                .OrderBy(c => c.Name)
                .ToListAsync();

            foreach (var category in categories)
            {
                category.OptionsSet = await _dbContext.CategoryoptionsSets
                    .Include(x => x.Category)
                    .Include(x => x.Option)
                    .Where(c => c.Category == category)
                    .OrderBy(os => os.Option.Name)
                    .Select(o => o.Option)
                    .ToListAsync();
            }

            return categories;
        }

        public async Task<Category> GetCategoryAsync(int identity)
        {
            return (await GetAllCategoriesAsync())
                .FirstOrDefault(c => c.Identity == identity);
        }

        public async Task<Category> GetCategoryAsync(string name)
        {
            return (await GetAllCategoriesAsync())
                .FirstOrDefault(c => c.Name == name);
        }

        public async Task<Result> AddCategoryAsync(string name,
            IEnumerable<CategoryOption> optionsSet, TimeSpan acceptableTime)
        {
            if (string.IsNullOrEmpty(name) || optionsSet == null || optionsSet.Count() == 0)
            {
                return new Result(ApplicationResources.TimerDB.Messages.NullArgument);
            }
            else
            {
                if (await CheckIfCategoryExistsAsync(name))
                {
                    return new Result(string.Format(ApplicationResources.TimerDB.Messages.CategoryAlreadyExists, name));
                }
                else
                {
                    var category = new Category()
                    {
                        Name = name,
                        ShortestAcceptableResult = acceptableTime,
                    };

                    _dbContext.Categories.Add(category);
                    var result = await _dbContext.SaveChangesAsync();

                    foreach (var option in optionsSet)
                    {
                        _dbContext.CategoryoptionsSets.Add(new CategoryOptionsSet()
                        {
                            Category = category,
                            Option = option,
                        });
                    }
                    result += await _dbContext.SaveChangesAsync();

                    if (result == optionsSet.Count() + 1)
                    {
                        return new Result(string.Empty, false);
                    }
                    else
                    {
                        return new Result(string.Format(ApplicationResources.TimerDB.Messages.CategoryAddAddFail, name));
                    }
                }
            }
        }

        public async Task<Result> DeleteCategoryAsync (string name)
        {
            var category = await GetCategoryAsync(name);
            if (category == null)
            {
                return new Result(ApplicationResources.TimerDB.Messages.CategoryNotExist);
            }
            else
            {
                return await DeleteCategoryInternalAsync(category);
            }
        }

        public async Task<Result> DeleteCategoryAsync(int identity)
        {
            var category = await GetCategoryAsync(identity);
            if (category == null)
            {
                return new Result(ApplicationResources.TimerDB.Messages.CategoryNotExist);
            }
            else
            {
                return await DeleteCategoryInternalAsync(category);
            }
        }

        public async Task<Result> UpdateCategoryAsync(int identity, string name,
            IEnumerable<CategoryOption> optionsSet, TimeSpan acceptableTime)
        {
            var category = await GetCategoryAsync(identity);

            if (category == null)
            {
                return new Result(ApplicationResources.TimerDB.Messages.CategoryNotExist);
            }
            else
            {
                if (string.IsNullOrEmpty(name) || optionsSet == null || optionsSet.Count() == 0)
                {
                    return new Result(ApplicationResources.TimerDB.Messages.NullArgument);
                }
                else
                {
                    var optionsDeleted = await DeleteCategoryOptionsAsync(category);

                    if (optionsDeleted)
                    {
                        category.Name = name;
                        category.ShortestAcceptableResult = acceptableTime;

                        foreach (var option in optionsSet)
                        {
                            _dbContext.CategoryoptionsSets.Add(new CategoryOptionsSet()
                            {
                                Category = category,
                                Option = option,
                            });
                        }
                        var result = await _dbContext.SaveChangesAsync();

                        if (optionsSet.Count() <= result)
                        {
                            return new Result(string.Empty, false);
                        }
                    }

                    return new Result(ApplicationResources.TimerDB.Messages.CategoryModificationFailed);
                }
            }
        }

        private async Task<Result> DeleteCategoryInternalAsync(Category category)
        {
            if (! await CheckIfCategoryCanBeDeletedAsync(category))
            {
                return new Result(string.Format(ApplicationResources.TimerDB.Messages.CategoryCannotDelete, category.Name));
            }
            else
            {
                var deleted = await DeleteCategoryOptionsAsync(category);

                _dbContext.Categories.Remove(category);
                var result = await _dbContext.SaveChangesAsync();

                if (result > 0 && deleted)
                {
                    return new Result(string.Empty, false);
                }
                else
                {
                    return new Result(ApplicationResources.TimerDB.Messages.CategoryDeleteFail);
                }
            }
        }

        private async Task<bool> DeleteCategoryOptionsAsync(Category category)
        {
            var options = await _dbContext.CategoryoptionsSets
                .Where(os => os.Category == category)
                .ToListAsync();
            _dbContext.CategoryoptionsSets.RemoveRange(options);
            var result = await _dbContext.SaveChangesAsync();

            return result == options.Count();
        }

        private async Task<bool> CheckIfCategoryExistsAsync(string name)
        {
            return (await GetAllCategoriesAsync())
                .Any(c => c.Name == name);
        }

        private async Task<bool> CheckIfCategoryCanBeDeletedAsync(Category category)
        {
            return ! await _dbContext.Cubes
                .AnyAsync(c => c.Category == category);
        }
    }
}
