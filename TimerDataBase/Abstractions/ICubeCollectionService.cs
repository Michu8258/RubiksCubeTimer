using Results.UserInterface;
using System.Collections.Generic;
using System.Threading.Tasks;
using TimerDataBase.TableModels;

namespace TimerDataBase.Abstractions
{
    public interface ICubeCollectionService
    {
        Task<Result> AddCubeToCollectionAsync(string userId, Cube cube);
        Task<Result> DeleteCubeFromCollectionAsync(string userId, Cube cube);
        Task<Result> DeleteAllCubesFromUserCollectionAsync(string userId);
        Task<Result> UpdateUserCubesCollectionAsync(string userId, IEnumerable<Cube> updatedCollection);
        Task<IEnumerable<Cube>> GetAllCubesOfUserAsync(string userId);
        Task<long> CountUsersWithCubeAsync(Cube cube);
    }
}
