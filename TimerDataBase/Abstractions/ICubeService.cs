using Results.UserInterface;
using System.Collections.Generic;
using System.Threading.Tasks;
using TimerDataBase.TableModels;

namespace TimerDataBase.Abstractions
{
    public interface ICubeService
    {
        Task<IEnumerable<PlasticColor>> GetAllPlasticColorsAsync();
        Task<PlasticColor> GetSinglePlasticColorAsync(string name);
        Task<Result> AddPlasticColorAsync(string name);
        Task<Result> DeletePlasticColorAsync(string name);

        Task<IEnumerable<Manufacturer>> GetAllManufacturersAsync();
        Task<Manufacturer> GetManufacturerAsync(int identity);
        Task<Manufacturer> GetManufacturerAsync(string name);
        Task<Result> AddNewManufacturerAsync(string name, string country, uint year);
        Task<Result> DeleteManufacturerAsync(string name);
        Task<Result> UpdateManufacturerAsync(int identity, string name, string country, uint year);

        Task<IEnumerable<Cube>> GetAllCubesAsync();
        Task<Cube> GetCubeAsync(long identity);
        Task<Result> AddNewCubeAsync(Category category, PlasticColor plasticColor,
            Manufacturer manufacturer, string modelName, int releaseYear, bool permission);
        Task<Result> DeleteCubeAsync(long identity);
        Task<Result> UpdateCubeAsync(long identity, Category category, PlasticColor plasticColor,
            Manufacturer manufacturer, string modelName, int releaseYear, bool permission);

        Task<IEnumerable<CubeRating>> GetAllRatingsForCubeAsync(long cubeIdentity);
        Task<Result> AddCubeRatingAsync(Cube cube, string userId, ushort rating);
        Task<Result> UpdateCubeRatingAsync(Cube cube, string userId, ushort newRate);
        Task<Result> DeleteCubeRatingAsync(Cube cube, string userId);
        Task<IEnumerable<CubeRating>> GetAllCubesRatingOfSingleUserAsync(string userId);
        IEnumerable<CubeRating> GetRatingsForCubesOfUser(IEnumerable<Cube> cubes, string userId);
        Task<bool> CheckIfCubeRatedAsync(Cube cube, string userId);
        Task<Result> DeleteAllRatingsOfUser(string userId);

    }
}