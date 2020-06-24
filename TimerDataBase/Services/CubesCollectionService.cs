using Microsoft.EntityFrameworkCore;
using Results.UserInterface;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimerDataBase.Abstractions;
using TimerDataBase.DataBaseContext;
using TimerDataBase.TableModels;

namespace TimerDataBase.Services
{
    public class CubesCollectionService : ICubeCollectionService
    {
        private readonly TimerDBContext _dbContext;

        public CubesCollectionService(TimerDBContext context)
        {
            _dbContext = context;
        }

        public async Task<Result> AddCubeToCollectionAsync(string userId, Cube cube)
        {
            if (string.IsNullOrEmpty(userId) || cube == null)
            {
                return new Result(ApplicationResources.TimerDB.Messages.NullArgument);
            }

            if (await CheckIfUserHasCubeInCollectionAsync(userId, cube))
            {
                return new Result(ApplicationResources.TimerDB.Messages.CubeAlreadyInCollection);
            }

            var assignment = new CubesCollection()
            {
                UserIdentity = userId,
                Cube = cube,
            };

            _dbContext.CubesCollections.Add(assignment);
            var result = await _dbContext.SaveChangesAsync();

            if (result == 1)
            {
                return new Result(string.Empty, false);
            }
            else
            {
                return new Result(ApplicationResources.TimerDB.Messages.CubeCollectionAddFailed);
            }
        }

        public async Task<Result> DeleteCubeFromCollectionAsync(string userId, Cube cube)
        {
            if (string.IsNullOrEmpty(userId) || cube == null)
            {
                return new Result(ApplicationResources.TimerDB.Messages.NullArgument);
            }

            if (!await CheckIfUserHasCubeInCollectionAsync(userId, cube))
            {
                return new Result(ApplicationResources.TimerDB.Messages.CubeNotInCollection);
            }

            var cubesToRemoveFromCollection = await _dbContext.CubesCollections
                .Where(cc => cc.UserIdentity == userId && cc.Cube == cube)
                .ToListAsync();

            _dbContext.CubesCollections.RemoveRange(cubesToRemoveFromCollection);
            var result = await _dbContext.SaveChangesAsync();

            if (result == cubesToRemoveFromCollection.Count())
            {
                return new Result(string.Empty, false);
            }
            else
            {
                return new Result(ApplicationResources.TimerDB.Messages.CubeCollectionDeleteFailed);
            }
        }

        public async Task<Result> DeleteAllCubesFromUserCollectionAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return new Result(ApplicationResources.TimerDB.Messages.NullArgument);
            }

            var cubesToDeleteFromCollection = await _dbContext.CubesCollections
                .Where(cc => cc.UserIdentity == userId)
                .ToListAsync();

            if (!cubesToDeleteFromCollection.Any())
            {
                return new Result(string.Empty, false);
            }
            else
            {
                _dbContext.CubesCollections.RemoveRange(cubesToDeleteFromCollection);
                var result = await _dbContext.SaveChangesAsync();
                if (result == cubesToDeleteFromCollection.Count())
                {
                    return new Result(string.Empty, false);
                }
                else
                {
                    return new Result(ApplicationResources.TimerDB.Messages.CubeCollectionDeleteAllFailed);
                }
            }
        }

        public async Task<Result> UpdateUserCubesCollectionAsync(string userId, IEnumerable<Cube> updatedCollection)
        {
            if (string.IsNullOrEmpty(userId) || updatedCollection == null)
            {
                return new Result(ApplicationResources.TimerDB.Messages.NullArgument);
            }

            var currentCollection = await _dbContext.CubesCollections
                .Where(cc => cc.UserIdentity == userId)
                .ToListAsync();

            int errors = 0;

            foreach (var cube in currentCollection)
            {
                if (! updatedCollection.Contains(cube.Cube))
                {
                    var result = await DeleteCubeFromCollectionAsync(userId, cube.Cube);
                    if (result.IsFaulted)
                    {
                        errors++;
                    }
                }
            }

            foreach (var cube in updatedCollection)
            {
                if (! await CheckIfUserHasCubeInCollectionAsync(userId, cube))
                {
                    var result = await AddCubeToCollectionAsync(userId, cube);
                    if (result.IsFaulted)
                    {
                        errors++;
                    }
                }
            }

            if (errors == 0)
            {
                return new Result(string.Empty, false);
            }
            else
            {
                return new Result(string.Format(ApplicationResources.TimerDB.Messages.CubeCollectionUpdateFailed, errors));
            }
        }

        private async Task<bool> CheckIfUserHasCubeInCollectionAsync(string userId, Cube cube)
        {
            return await _dbContext.CubesCollections
                .Include(cc => cc.Cube)
                .AnyAsync(cc => cc.UserIdentity == userId && cc.Cube == cube);
        }

        public async Task<IEnumerable<Cube>> GetAllCubesOfUserAsync(string userId)
        {
            return await _dbContext.CubesCollections
                .Include(cc => cc.Cube)
                .Include(cc => cc.Cube.Category)
                .Include(cc => cc.Cube.Manufacturer)
                .Include(cc => cc.Cube.PlasticColor)
                .Where(cc => cc.UserIdentity == userId)
                .OrderBy(cc => cc.Cube.Category.Name)
                .ThenBy(cc => cc.Cube.Manufacturer.Name)
                .ThenBy(cc => cc.Cube.ModelName)
                .ThenBy(cc => cc.Cube.PlasticColor.Name)
                .Select(cc => cc.Cube)
                .ToListAsync();
        }

        public async Task<long> CountUsersWithCubeAsync(Cube cube)
        {
            return await _dbContext.CubesCollections
                .CountAsync(c => c.Cube == cube);
        }
    }
}
