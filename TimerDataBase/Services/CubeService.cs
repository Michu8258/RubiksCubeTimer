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
    public class CubeService : ICubeService
    {
        private readonly TimerDBContext _dbContext;

        public CubeService(TimerDBContext context)
        {
            _dbContext = context;
        }

        public async Task<IEnumerable<PlasticColor>> GetAllPlasticColorsAsync()
        {
            return await _dbContext.PlasticColors
                .OrderBy(pc => pc.Name)
                .ToListAsync();
        }

        public async Task<PlasticColor> GetSinglePlasticColorAsync(string name)
        {
            return await _dbContext.PlasticColors
                .FirstOrDefaultAsync(pc => pc.Name == name);
        }

        public async Task<Result> AddPlasticColorAsync(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return new Result(ApplicationResources.TimerDB.Messages.NullArgument);
            }

            if (await CheckIfColorExistsAsync(name))
            {
                return new Result(string.Format(ApplicationResources.TimerDB.Messages.PlasticColorExists, name));
            }
            else
            {
                _dbContext.PlasticColors.Add(new PlasticColor()
                {
                    Name = name
                });
                if (await _dbContext.SaveChangesAsync() == 1)
                {
                    return new Result(string.Format(ApplicationResources.TimerDB.Messages.PlasticColorAddSuccessfull, name), false);
                }
                return new Result(ApplicationResources.TimerDB.Messages.PlasticColorAddFail);
            }
        }

        public async Task<Result> DeletePlasticColorAsync(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return new Result(ApplicationResources.TimerDB.Messages.NullArgument);
            }

            if (! await CheckIfColorExistsAsync(name))
            {
                return new Result(string.Format(ApplicationResources.TimerDB.Messages.PlasticColorNotDefined, name));
            }
            else
            {

                if (await _dbContext.Cubes.AnyAsync(c => c.PlasticColor.Name == name))
                {
                    return new Result(string.Format(ApplicationResources.TimerDB.Messages.PlasticColorInUse, name));
                }
                else
                {
                    var plascticColors = await _dbContext.PlasticColors
                    .Where(pc => pc.Name == name).ToListAsync();

                    _dbContext.PlasticColors.RemoveRange(plascticColors);
                    var result = await _dbContext.SaveChangesAsync();

                    if (result == plascticColors.Count())
                    {
                        return new Result(string.Format(ApplicationResources.TimerDB.Messages.PlasticColorDeletionSuccessfull, name), false);
                    }
                }

                return new Result(string.Format(ApplicationResources.TimerDB.Messages.PlasticColorDeleteFailed, name));
            }
        }

        private async Task<bool> CheckIfColorExistsAsync(string name)
        {
            return await _dbContext.PlasticColors
                .AnyAsync(pc => pc.Name == name);
        }

        public async Task<IEnumerable<Manufacturer>> GetAllManufacturersAsync()
        {
            return await _dbContext.Manufacturers
                .OrderBy(m => m.Name)
                .ToListAsync();
        }

        public async Task<Manufacturer> GetManufacturerAsync(int identity)
        {
            return (await GetAllManufacturersAsync()).FirstOrDefault(m => m.Identity == identity);
        }

        public async Task<Manufacturer> GetManufacturerAsync(string name)
        {
            return (await GetAllManufacturersAsync()).FirstOrDefault(m => m.Name == name);
        }

        public async Task<Result> AddNewManufacturerAsync(string name, string country, uint year)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(country))
            {
                return new Result(ApplicationResources.TimerDB.Messages.NullArgument);
            }

            if (year < 0 || year > 2500)
            {
                return new Result(string.Format(ApplicationResources.TimerDB.Messages.ArgumentRange, nameof(year), 0, 2500));
            }

            if (await CheckIfManufacturerExistsAsync(name))
            {
                return new Result(string.Format(ApplicationResources.TimerDB.Messages.ManufacturerExists, name));
            }
            else
            {
                _dbContext.Manufacturers.Add(new Manufacturer()
                {
                    Country = country,
                    FoundationYear = year,
                    Name = name
                });

                if (await _dbContext.SaveChangesAsync() == 1)
                {
                    return new Result(string.Format(ApplicationResources.TimerDB.Messages.ManufacturerAddedSuccessfully, name), false);
                }

                return new Result(ApplicationResources.TimerDB.Messages.ManufacturerAddFailed);
            }
        }

        public async Task<Result> DeleteManufacturerAsync(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return new Result(ApplicationResources.TimerDB.Messages.NullArgument);
            }
            if (!await CheckIfManufacturerExistsAsync(name))
            {
                return new Result(string.Format(ApplicationResources.TimerDB.Messages.ManufacturerNotDefined, name));
            }
            else
            {
                if (await _dbContext.Cubes.AnyAsync(c => c.Manufacturer.Name == name))
                {
                    return new Result(string.Format(ApplicationResources.TimerDB.Messages.ManufacturerInUse, name));
                }
                else
                {
                    var manufacturers = await _dbContext.Manufacturers
                        .Where(m => m.Name == name).ToListAsync();

                    _dbContext.Manufacturers.RemoveRange(manufacturers);
                    var result = await _dbContext.SaveChangesAsync();
                    if (manufacturers.Count() == result)
                    {
                        return new Result(string.Format(ApplicationResources.TimerDB.Messages.ManufacturerDeletionSuccessfull, name), false);
                    }
                }

                return new Result(string.Format(ApplicationResources.TimerDB.Messages.ManufacturerDeletionFailed, name), false);
            }
        }

        public async Task<Result> UpdateManufacturerAsync(int identity, string name, string country, uint year)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(country))
            {
                return new Result(ApplicationResources.TimerDB.Messages.NullArgument);
            }

            if (year < 0 || year > 2500)
            {
                return new Result(string.Format(ApplicationResources.TimerDB.Messages.ArgumentRange, nameof(year), 0, 2500));
            }

            if (!await CheckIfManufacturerExistsAsync(identity))
            {
                return new Result(string.Format(ApplicationResources.TimerDB.Messages.ManufacturerNotDefined, name));
            }
            else
            {
                var manufacturer = await _dbContext.Manufacturers.FirstOrDefaultAsync(m => m.Identity == identity);
                manufacturer.Country = country;
                manufacturer.FoundationYear = year;
                manufacturer.Name = name;
                var result = await _dbContext.SaveChangesAsync();

                if(result == 1)
                {
                    return new Result(string.Format(ApplicationResources.TimerDB.Messages.ManufacturerUpdatedSuccessfully, name), false);
                }

                return new Result(string.Format(ApplicationResources.TimerDB.Messages.ManufacturerUpdateFailed, name));
            }
        }

        private async Task<bool> CheckIfManufacturerExistsAsync(string name)
        {
            return await _dbContext.Manufacturers
                .AnyAsync(m => m.Name == name);
        }

        private async Task<bool> CheckIfManufacturerExistsAsync(int identity)
        {
            return await _dbContext.Manufacturers
                .AnyAsync(m => m.Identity == identity);
        }

        public async Task<IEnumerable<Cube>> GetAllCubesAsync()
        {
            return await _dbContext.Cubes
                .Include(c => c.Category)
                .Include(c => c.PlasticColor)
                .Include(c => c.Manufacturer)
                .OrderBy(c => c.Category.Name)
                .ThenBy(c => c.Manufacturer.Name)
                .ThenBy(c => c.ModelName)
                .ThenByDescending(c => c.Rating)
                .ToListAsync();
        }

        public async Task<Cube> GetCubeAsync(long identity)
        {
            return (await GetAllCubesAsync()).FirstOrDefault(c => c.Identity == identity);
        }

        public async Task<Result> AddNewCubeAsync(Category category, PlasticColor plasticColor,
            Manufacturer manufacturer, string modelName, int releaseYear, bool permission)
        {
            if (category == null || plasticColor == null || manufacturer == null ||
                string.IsNullOrEmpty(modelName) || (releaseYear < 0 || releaseYear > 2500))
            {
                return new Result(ApplicationResources.TimerDB.Messages.NullArgument);
            }
            else
            {
                if (await CheckIfCubeExistsAsync(category, plasticColor, manufacturer, modelName))
                {
                    return new Result(ApplicationResources.TimerDB.Messages.CubeExists);
                }
                else
                {
                    _dbContext.Cubes.Add(new Cube()
                    {
                        Category = category,
                        PlasticColor = plasticColor,
                        Manufacturer = manufacturer,
                        ModelName = modelName,
                        ReleaseYear = releaseYear,
                        Rating = 0.0,
                        RatesAmount = 0,
                        WcaPermission = permission,
                    });

                    var result = await _dbContext.SaveChangesAsync();
                    if (result == 1)
                    {
                        return new Result(ApplicationResources.TimerDB.Messages.CubeAddedSuccessfully, false);
                    }

                    return new Result(ApplicationResources.TimerDB.Messages.CubeAddFailed);
                }
            }
        }

        public async Task<Result> DeleteCubeAsync(long identity)
        {
            var cube = await GetCubeAsync(identity);

            if (cube == null)
            {
                return new Result(ApplicationResources.TimerDB.Messages.CubeNotExists);
            }
            else
            {
                if (! await CheckIfCubeCanBeDeletedAsync(cube))
                {
                    return new Result(ApplicationResources.TimerDB.Messages.CubeCannotDelete);
                }
                else
                {
                    _dbContext.Cubes.Remove(cube);
                    var result = await _dbContext.SaveChangesAsync();

                    if (result  == 1)
                    {
                        return new Result(ApplicationResources.TimerDB.Messages.CubeDeletedSuccessfully, false);
                    }
                    else
                    {
                        return new Result(ApplicationResources.TimerDB.Messages.CubeDeleteFailed);
                    }
                }
            }
        }

        public async Task<Result> UpdateCubeAsync(long identity, Category category, PlasticColor plasticColor,
            Manufacturer manufacturer, string modelName, int releaseYear, bool permission)
        {
            var cube = await GetCubeAsync(identity);

            if (cube == null)
            {
                return new Result(ApplicationResources.TimerDB.Messages.CubeNotExists);
            }
            else
            {
                if (category == null || plasticColor == null || manufacturer == null ||
                string.IsNullOrEmpty(modelName) || (releaseYear < 0 || releaseYear > 2500))
                {
                    return new Result(ApplicationResources.TimerDB.Messages.NullArgument);
                }
                else
                {
                    cube.Category = category;
                    cube.PlasticColor = plasticColor;
                    cube.Manufacturer = manufacturer;
                    cube.ModelName = modelName;
                    cube.ReleaseYear = releaseYear;
                    cube.WcaPermission = permission;

                    var result = await _dbContext.SaveChangesAsync();

                    if (result == 1)
                    {
                        return new Result(ApplicationResources.TimerDB.Messages.CubeUpdateSuccessfull, false);
                    }
                    else
                    {
                        return new Result(ApplicationResources.TimerDB.Messages.CubeUpdateFailed);
                    }
                }
            }
        }

        private async Task<bool> CheckIfCubeExistsAsync(Category category, PlasticColor plasticColor,
            Manufacturer manufacturer, string modelName)
        {
            return (await GetAllCubesAsync()).Any(c => c.Category == category &&
            c.PlasticColor == plasticColor && c.Manufacturer == manufacturer
            && c.ModelName == modelName);
        }

        private async Task<bool> CheckIfCubeCanBeDeletedAsync(Cube cube)
        {
            bool can = ! await _dbContext.CubeRatings
                .AnyAsync(r => r.Cube == cube);

            if (!can)
            {
                return false;
            }

            can = ! await _dbContext.CubesCollections
                .AnyAsync(cc => cc.Cube == cube);

            if (!can)
            {
                return false;
            }

            can = ! await _dbContext.Series
                .AnyAsync(s => s.Cube == cube);

            return can;
        }

        public async Task<IEnumerable<CubeRating>> GetAllRatingsForCubeAsync(long cubeIdentity)
        {
            var cube = await GetCubeAsync(cubeIdentity);

            if(cube == null)
            {
                return new List<CubeRating>();
            }
            else
            {
                return await _dbContext.CubeRatings
                    .Where(r => r.Cube == cube)
                    .OrderBy(r => r.Identity)
                    .ToListAsync();
            }
        }

        public async Task<Result> AddCubeRatingAsync(Cube cube, string userId, ushort rating)
        {
            if (string.IsNullOrEmpty(userId) || cube == null)
            {
                return new Result(ApplicationResources.TimerDB.Messages.NullArgument);
            }

            if (await CheckIfCubeRatedAsync(cube, userId))
            {
                return new Result(ApplicationResources.TimerDB.Messages.RatingAlreadyMade);
            }
            else
            {
                var cubeRating = new CubeRating()
                {
                    RateValue = rating,
                    Cube = cube,
                    UserIdentity = userId,
                    Rated = true,
                };

                _dbContext.CubeRatings.Add(cubeRating);
                var dbCube = _dbContext.Cubes.FirstOrDefault(c => c.Identity == cube.Identity);
                if (dbCube != null)
                {
                    dbCube.RatesAmount++;
                }
                var result = await _dbContext.SaveChangesAsync();
                await UpdateCubeRatinAsync(cube);
                result += await _dbContext.SaveChangesAsync();

                if (result >= 1)
                {
                    return new Result(ApplicationResources.TimerDB.Messages.RatingAddedSuccessfully, false);
                }

                return new Result(ApplicationResources.TimerDB.Messages.RatingAddFailed);
            }
        }

        public async Task<Result> UpdateCubeRatingAsync(Cube cube, string userId, ushort newRate)
        {
            if (string.IsNullOrEmpty(userId) || cube == null)
            {
                return new Result(ApplicationResources.TimerDB.Messages.NullArgument);
            }

            if (! await CheckIfCubeRatedAsync(cube, userId))
            {
                return new Result(ApplicationResources.TimerDB.Messages.CubeNotRated);
            }
            else
            {
                var cubeRating = _dbContext.CubeRatings
                    .FirstOrDefault(cr => cr.Cube == cube && cr.UserIdentity == userId);
                cubeRating.RateValue = newRate;
                var result = await _dbContext.SaveChangesAsync();
                await UpdateCubeRatinAsync(cube);
                result += await _dbContext.SaveChangesAsync();

                if (result >= 1)
                {
                    return new Result(ApplicationResources.TimerDB.Messages.CubeRatingUpdateSuccessfull, false);
                }
            }

            return new Result(ApplicationResources.TimerDB.Messages.CubeRatingUpdateFailed);
        }

        public async Task<Result> DeleteCubeRatingAsync(Cube cube, string userId)
        {
            if (string.IsNullOrEmpty(userId) || cube == null)
            {
                return new Result(ApplicationResources.TimerDB.Messages.NullArgument);
            }

            if (!await CheckIfCubeRatedAsync(cube, userId))
            {
                return new Result(ApplicationResources.TimerDB.Messages.CubeNotRated);
            }
            else
            {
                var cubeRatings = _dbContext.CubeRatings
                    .Where(r => r.Cube == cube && r.UserIdentity == userId).ToList();
                _dbContext.CubeRatings.RemoveRange(cubeRatings);
                var dbCube = _dbContext.Cubes.FirstOrDefault(c => c.Identity == cube.Identity);
                if (dbCube != null)
                {
                    dbCube.RatesAmount -= cubeRatings.Count();
                    if (dbCube.RatesAmount < 0)
                    {
                        dbCube.RatesAmount = 0;
                    }
                }
                var result = await _dbContext.SaveChangesAsync();
                await UpdateCubeRatinAsync(cube);
                result += await  _dbContext.SaveChangesAsync();

                if (result >= 1)
                {
                    return new Result(ApplicationResources.TimerDB.Messages.CuberatingDeletedSuccessfully, false);
                }
            }

            return new Result(ApplicationResources.TimerDB.Messages.CubeRatingDeletionFailed);
        }

        public async Task<IEnumerable<CubeRating>> GetAllCubesRatingOfSingleUserAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return new List<CubeRating>();
            }
            else
            {
                return await _dbContext.CubeRatings
                    .Where(r => r.UserIdentity == userId)
                    .Include(r => r.Cube)
                    .Include(r => r.Cube.Manufacturer)
                    .Include(r => r.Cube.PlasticColor)
                    .Include(r => r.Cube.Category)
                    .OrderBy(r => r.RateValue)
                    .ToListAsync();
            }
        }

        public IEnumerable<CubeRating> GetRatingsForCubesOfUser(IEnumerable<Cube> cubes, string userId)
        {
            List<CubeRating> output = new List<CubeRating>();

            foreach (var cube in cubes)
            {
                var rating = _dbContext.CubeRatings
                    .FirstOrDefault(cr => cr.Cube == cube && cr.UserIdentity == userId);

                if (rating!= null)
                {
                    output.Add(rating);
                }
            }

            return output;
        }

        public async Task<bool> CheckIfCubeRatedAsync(Cube cube, string userId)
        {
            return await Task.FromResult(_dbContext.CubeRatings
                .Any(r => r.Cube == cube && r.UserIdentity == userId));
        }

        public async Task<Result> DeleteAllRatingsOfUser(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return new Result(ApplicationResources.TimerDB.Messages.NullArgument);
            }

            var ratings = await _dbContext.CubeRatings
                .Include(cr => cr.Cube)
                .Where(cr => cr.UserIdentity == userId)
                .ToListAsync();

            bool output = true;

            foreach (var rate in ratings)
            {
                var result = await DeleteCubeRatingAsync(rate.Cube, userId);
                if (result.IsFaulted)
                {
                    output = false;
                }
            }

            if (output)
            {
                return new Result(string.Empty, false);
            }
            else
            {
                return new Result(string.Format(ApplicationResources.TimerDB.Messages.DeleteUserAllRatings, userId));
            }
        }

        private async Task UpdateCubeRatinAsync(Cube cube)
        {
            if (await _dbContext.CubeRatings.AnyAsync(r => r.Cube == cube))
            {
                cube.Rating = await _dbContext.CubeRatings
                    .Where(r => r.Cube == cube)
                    .AverageAsync(r => r.RateValue);
            }
            else
            {
                cube.Rating = 0;
            }
        }
    }
}
