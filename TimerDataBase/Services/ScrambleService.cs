using Microsoft.EntityFrameworkCore;
using Results.UserInterface;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimerDataBase.Abstractions;
using TimerDataBase.DataBaseContext;
using TimerDataBase.HelperModels;
using TimerDataBase.TableModels;

namespace TimerDataBase.Services
{
    public class ScrambleService : IScrambleService
    {
        private readonly TimerDBContext _dbContext;

        public ScrambleService(TimerDBContext context)
        {
            _dbContext = context;
        }

        public async Task<Result> AddScrambleDefinitionAsync(int categoryID, int minLength, int maxLength,
            int defaultLength, bool eliminateDuplicates, bool allowRegenerate, bool disabled,
            string topColor, string frontColor, string scrambleName)
        {
            if (categoryID < 1 || minLength < 0 || minLength > Scramble.ScrambleMaxLength ||
                maxLength <= minLength || maxLength < 1 || maxLength > Scramble.ScrambleMaxLength ||
                defaultLength < minLength || defaultLength > maxLength || string.IsNullOrEmpty(topColor) ||
                string.IsNullOrEmpty(frontColor) || string.IsNullOrEmpty(scrambleName))
            {
                return new Result(ApplicationResources.TimerDB.Messages.NullArgument);
            }

            if (!await CheckIfCategoryExistsAsync(categoryID))
            {
                return new Result(ApplicationResources.TimerDB.Messages.CategoryNotExist);
            }
            else
            {
                if (await CheckIfScrambleForCategoryExistsAsync(await GetCategoryAsync(categoryID), scrambleName))
                {
                    return new Result(string.Format(ApplicationResources.TimerDB.Messages.ScrambleNameExists, scrambleName));
                }

                _dbContext.Scrambles.Add(new Scramble()
                {
                    Default = (await GetAmountOfScramblesForCategoryAsync(categoryID)) == 0,
                    Category = await GetCategoryAsync(categoryID),
                    DefaultScrambleLength = defaultLength,
                    MinimumScrambleLength = minLength,
                    MaximumScrambleLength = maxLength,
                    EliminateDuplicates = eliminateDuplicates,
                    AllowRegenerate = allowRegenerate,
                    Disabled = disabled,
                    TopColor = topColor,
                    FrontColor = frontColor,
                    Name = scrambleName,
                });

                if (await _dbContext.SaveChangesAsync() == 1)
                {
                    return new Result(string.Empty, false);
                }

                return new Result(ApplicationResources.TimerDB.Messages.ScrambleDefinitionAddFailed);
            }
        }

        public async Task<Result> UpdateScrambleDefinitionAsync(long scrambleID, int minLength, int maxLength,
            int defaultLength, bool eliminateDuplicates, bool allowRegenerate, bool disabled,
            string topColor, string frontColor, string scrambleName)
        {
            if (scrambleID < 1 || minLength < 0 || minLength > Scramble.ScrambleMaxLength ||
                maxLength <= minLength || maxLength < 1 || maxLength > Scramble.ScrambleMaxLength ||
                defaultLength < minLength || defaultLength > maxLength || string.IsNullOrEmpty(topColor) ||
                string.IsNullOrEmpty(frontColor) || string.IsNullOrEmpty(scrambleName))
            {
                return new Result(ApplicationResources.TimerDB.Messages.NullArgument);
            }

            if (!await CheckIfSrambleDefinitionExistsAsync(scrambleID))
            {
                return new Result(ApplicationResources.TimerDB.Messages.ScrambleDoesNotExist);
            }
            else
            {
                var category = await GetCategoryOfScrambleAsync(scrambleID);

                if (category == null)
                {
                    return new Result(ApplicationResources.TimerDB.Messages.CategoryNotExist);
                }

                var scramble = await GetScrambleDefinitionAsync(scrambleID);

                if (scramble == null)
                {
                    return new Result(ApplicationResources.TimerDB.Messages.ScrambleDoesNotExist);
                }

                scramble.MinimumScrambleLength = minLength;
                scramble.MaximumScrambleLength = maxLength;
                scramble.DefaultScrambleLength = defaultLength;
                scramble.EliminateDuplicates = eliminateDuplicates;
                scramble.AllowRegenerate = allowRegenerate;
                scramble.Disabled = disabled;
                scramble.TopColor = topColor;
                scramble.FrontColor = frontColor;
                scramble.Name = scrambleName;

                var updateResult = await _dbContext.SaveChangesAsync();

                if (updateResult == 1)
                {
                    return new Result(string.Empty, false);
                }

                return new Result(ApplicationResources.TimerDB.Messages.UpdateScrambleDefinitionFailed);
            }
        }

        public async Task<Result> SetScrambleAsDefaultAsync(int categoryId, long scrambleId)
        {
            var category = await GetCategoryAsync(categoryId);
            var scramble = await GetScrambleDefinitionAsync(scrambleId);

            if (category == null || scramble == null)
            {
                return new Result(ApplicationResources.TimerDB.Messages.NullArgument);
            }

            var defaultsReseted = await ResetDefaultCategoryForCategoryAsync(category);

            if (!defaultsReseted)
            {
                return new Result(ApplicationResources.TimerDB.Messages.ScrambleDefaultsResetFailed);
            }
            else
            {
                if (await SetScrambleAsDefaultAsync(category, scramble))
                {
                    return new Result(string.Empty, false);
                }
                else
                {
                    return new Result(ApplicationResources.TimerDB.Messages.ScrambleDefaultSetFail);
                }
            }
        }

        public async Task<Result> DeleteScrambleDefinitionAsync(long scrambleID)
        {
            var scramble = await GetScrambleDefinitionAsync(scrambleID);
            var scramblesForCategoryAmount = (await GetAllDefinedScramblesForCategoryAsync(scramble.Category.Identity)).Count();
            if (scramble == null)
            {
                return new Result(ApplicationResources.TimerDB.Messages.ScrambleDoesNotExist);
            }
            else
            {
                if (scramble.Default && scramblesForCategoryAmount > 1)
                {
                    return new Result(ApplicationResources.TimerDB.Messages.ScrambleCanotDeleteDefault);
                }

                var movesDeleteResult = await DeleteAllMovesFromScrambleAsync(scrambleID);
                if (movesDeleteResult.IsFaulted)
                {
                    return movesDeleteResult;
                }

                _dbContext.Remove(scramble);
                var result = await _dbContext.SaveChangesAsync();

                if (result >= 1)
                {
                    return new Result(string.Empty, false);
                }

                return new Result(ApplicationResources.TimerDB.Messages.ScrambleDefinitionDeletionFailed);
            }
        }

        public async Task<Result> UpdateScrambleMovesAsync(long scrambleID, IEnumerable<string> moves)
        {
            if (scrambleID < 1 || moves == null || moves.Count() < 1)
            {
                return new Result(ApplicationResources.TimerDB.Messages.NullArgument);
            }

            if (!await CheckIfSrambleDefinitionExistsAsync(scrambleID))
            {
                return new Result(ApplicationResources.TimerDB.Messages.ScrambleDoesNotExist);
            }

            var oldMovesDeletionResult = await DeleteAllMovesFromScrambleAsync(scrambleID);
            if (oldMovesDeletionResult.IsFaulted)
            {
                return oldMovesDeletionResult;
            }

            var addMovesResult =  await AddMovesToScrambleAsync(scrambleID, moves);
            if (addMovesResult.IsFaulted)
            {
                return addMovesResult;
            }

            return await UpdateAmountOfMovesInScrambleAsync(scrambleID);
        }

        public async Task<IEnumerable<Category>> GetCategoriesWithoutScrambleAsync()
        {
            var allCategories = await _dbContext.Categories
                .ToListAsync();

            var categoriesWithNoScramble = new List<Category>();

            foreach (var category in allCategories)
            {
                if ((await _dbContext.Scrambles.CountAsync(s => s.Category == category)) < 1)
                {
                    categoriesWithNoScramble.Add(category);
                }
            }

            return categoriesWithNoScramble;
        }

        public async Task<CompleteScramble> GetDefaultCompleteScrambleAsync(Category category)
        {
            var scramble = await GetDefaultScrambleDefinitionAsync(category);
            return await ProduceCompleteScrambleAsync(scramble);
        }

        public async Task<IEnumerable<CompleteScramble>> GetAllCompleteScramblesForCategoryAsync(int categoryId)
        {
            var category = await GetCategoryAsync(categoryId);
            var output = new List<CompleteScramble>();

            if (category == null)
            {
                return output;
            }

            var scrambles = await GetAllDefinedScramblesForCategoryAsync(categoryId);

            foreach (var scramble in scrambles)
            {
                output.Add(await ProduceCompleteScrambleAsync(scramble));
            }

            return output;
        }

        public async Task<IEnumerable<Scramble>> GetAllDefinedScramblesForCategoryAsync(int categoryId)
        {
            var category = await GetCategoryAsync(categoryId);

            if (category == null)
            {
                return new List<Scramble>();
            }

            return await _dbContext.Scrambles
                .Include(s => s.Category)
                .Where(s => s.Category == category)
                .OrderBy(s => s.Name)
                .ToListAsync();
        }

        public async Task<CompleteScramble> GetCompleteScrambleAsync(long scrambleId)
        {
            return await ProduceCompleteScrambleAsync(await GetScrambleDefinitionAsync(scrambleId));
        }

        private async Task<int> GetAmountOfScramblesForCategoryAsync(int categoryId)
        {
            var category = await GetCategoryAsync(categoryId);

            if (category == null)
            {
                return 0;
            }
            else
            {
                return await _dbContext.Scrambles
                    .Include(s => s.Category)
                    .CountAsync(s => s.Category == category);
            }
        }

        private async Task<bool> ResetDefaultCategoryForCategoryAsync(Category category)
        {
            var scrambles = await _dbContext.Scrambles
                .Include(s => s.Category)
                .Where(s => s.Category == category)
                .ToListAsync();

            foreach (var scramble in scrambles)
            {
                scramble.Default = false;
            }

            return await _dbContext.SaveChangesAsync() > 0;
        }

        private async Task<bool> SetScrambleAsDefaultAsync(Category category, Scramble scramble)
        {
            if (scramble.Category != category)
            {
                return false;
            }
            else
            {
                scramble.Default = true;
                return await _dbContext.SaveChangesAsync() > 0;
            }
        }

        private async Task<bool> CheckIfCategoryExistsAsync(int categoryID)
        {
            var category = await GetCategoryAsync(categoryID);

            return category?.Identity > 0;
        }

        private async Task<Category> GetCategoryAsync(int categoryID)
        {
            return await _dbContext.Categories
               .FirstOrDefaultAsync(c => c.Identity == categoryID);
        }

        private async Task<Scramble> GetScrambleDefinitionAsync(long scrambleID)
        {
            return await _dbContext.Scrambles
                .Include(s => s.Category)
                .FirstOrDefaultAsync(s => s.Identity == scrambleID);
        }

        private async Task<Scramble> GetDefaultScrambleDefinitionAsync(Category category)
        {
            var amount = await _dbContext.Scrambles
                .CountAsync(s => s.Category == category);

            if (amount <= 1)
            {
                return await _dbContext.Scrambles
                    .FirstOrDefaultAsync(s => s.Category == category);
            }
            else
            {
                return await _dbContext.Scrambles
                    .FirstOrDefaultAsync(s => s.Category == category && s.Default);
            }
        }

        private async Task<bool> CheckIfSrambleDefinitionExistsAsync(long scrambleID)
        {
            return (await GetScrambleDefinitionAsync(scrambleID))?.Identity > 0;
        }

        private async Task<Result> DeleteAllMovesFromScrambleAsync(long scrambleID)
        {
            var scramble = await GetScrambleDefinitionAsync(scrambleID);
            if (scramble == null)
            {
                return new Result(string.Empty, false);
            }

            if ((await _dbContext.ScrambleMoves.CountAsync(m => m.Scramble == scramble)) < 1)
            {
                return new Result(string.Empty, false);
            }

            var moves = await _dbContext.ScrambleMoves
                .Where(m => m.Scramble == scramble)
                .ToListAsync();

            _dbContext.ScrambleMoves.RemoveRange(moves);
            var ok =  (await _dbContext.SaveChangesAsync()) == moves.Count;
            
            if (ok)
            {
                return new Result(string.Empty, false);
            }

            return new Result(ApplicationResources.TimerDB.Messages.ScrambleMovesDeleteError);
        }

        private async Task<Result> AddMovesToScrambleAsync(long scrambleID, IEnumerable<string> moves)
        {
            var scramble = await GetScrambleDefinitionAsync(scrambleID);
            if (scramble == null)
            {
                return new Result(ApplicationResources.TimerDB.Messages.ScrambleDoesNotExist);
            }

            if (moves?.Count() < 1)
            {
                return new Result(ApplicationResources.TimerDB.Messages.ScrambleNoMove);
            }

            var duplicatesCheckResult = CheckNewMovesCollectionForDuplicates(moves);
            if (duplicatesCheckResult.IsFaulted)
            {
                return duplicatesCheckResult;
            }

            var scrambleMoves = new List<ScrambleMove>();
            foreach (var move in moves)
            {
                scrambleMoves.Add(new ScrambleMove()
                {
                    Move = move,
                    Scramble = scramble,
                });
            }

            _dbContext.ScrambleMoves.AddRange(scrambleMoves);

            var additionSuccessfull = (await _dbContext.SaveChangesAsync()) == moves.Count();

            if (additionSuccessfull)
            {
                return new Result(string.Empty, false);
            }

            return new Result(ApplicationResources.TimerDB.Messages.ScrambleMovesAddError);
        }

        private Result CheckNewMovesCollectionForDuplicates(IEnumerable<string> moves)
        {
            foreach (var move in moves)
            {
                if(moves.Count(m => m == move) != 1)
                {
                    return new Result(ApplicationResources.TimerDB.Messages.MovesCollectionContainsDuplicates);
                }
            }

            return new Result(string.Empty, false);
        }

        private async Task<IEnumerable<ScrambleMove>> GetScrambleMovesAsync(Scramble scramble)
        {
            if (scramble == null)
            {
                return new List<ScrambleMove>();
            }
            else
            {
                return await _dbContext.ScrambleMoves
                    .Where(m => m.Scramble == scramble)
                    .OrderBy(m => m.Move)
                    .ToListAsync();
            }
        }

        private async Task<CompleteScramble> ProduceCompleteScrambleAsync(Scramble scramble)
        {
            IEnumerable<ScrambleMove> moves = new List<ScrambleMove>();

            if (scramble != null)
            {
                moves = await GetScrambleMovesAsync(scramble);
            }

            return new CompleteScramble()
            {
                Scramble = scramble,
                Moves = moves,
            };
        }

        private async Task<Result> UpdateAmountOfMovesInScrambleAsync(long scrambleID)
        {
            var scramble = await GetScrambleDefinitionAsync(scrambleID);

            if (scramble == null)
            {
                return new Result(ApplicationResources.TimerDB.Messages.ScrambleDoesNotExist);
            }

            var moves = await GetScrambleMovesAsync(scramble);

            scramble.MovesAmount = moves.Count();
            var result = await _dbContext.SaveChangesAsync();

            if (result >= 0)
            {
                return new Result(string.Empty, false);
            }

            return new Result(ApplicationResources.TimerDB.Messages.ScrambleMovesAmountUpdateError);
        }

        private async Task<bool> CheckIfScrambleForCategoryExistsAsync(Category category, string scrambleName)
        {
            if (category == null)
            {
                return false;
            }

            return (await _dbContext.Scrambles
                .CountAsync(s => s.Category == category && s.Name == scrambleName)) > 0;
        }

        private async Task<Category> GetCategoryOfScrambleAsync(long scrambleId)
        {
            var scramble = await GetScrambleDefinitionAsync(scrambleId);

            if (scramble == null)
            {
                return null;
            }

            return scramble.Category;
        }
    }
}
