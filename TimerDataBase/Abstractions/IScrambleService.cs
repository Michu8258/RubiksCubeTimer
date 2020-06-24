using Results.UserInterface;
using System.Collections.Generic;
using System.Threading.Tasks;
using TimerDataBase.HelperModels;
using TimerDataBase.TableModels;

namespace TimerDataBase.Abstractions
{
    public interface IScrambleService
    {
        Task<Result> AddScrambleDefinitionAsync(int categoryID, int minLength, int maxLength,
            int defaultLength, bool eliminateDuplicates, bool allowRegenerate, bool disabled,
            string topColor, string frontColor, string scrambleName);
        Task<Result> UpdateScrambleDefinitionAsync(long scrambleID, int minLength, int maxLength,
            int defaultLength, bool eliminateDuplicates, bool allowRegenerate, bool disabled,
            string topColor, string frontColor, string scrambleName);
        Task<Result> SetScrambleAsDefaultAsync(int categoryId, long scrambleId);
        Task<Result> DeleteScrambleDefinitionAsync(long scrambleID);
        Task<Result> UpdateScrambleMovesAsync(long scrambleID, IEnumerable<string> moves);
        Task<IEnumerable<Category>> GetCategoriesWithoutScrambleAsync();
        Task<CompleteScramble> GetDefaultCompleteScrambleAsync(Category category);
        Task<IEnumerable<CompleteScramble>> GetAllCompleteScramblesForCategoryAsync(int categoryId);
        Task<IEnumerable<Scramble>> GetAllDefinedScramblesForCategoryAsync(int categoryId);
        Task<CompleteScramble> GetCompleteScrambleAsync(long scrambleId);
    }
}
