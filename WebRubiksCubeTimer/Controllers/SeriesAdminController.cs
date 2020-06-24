using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Results.UserInterface;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using TimerDataBase.Abstractions;
using WebRubiksCubeTimer.ViewModels.Series;

namespace WebRubiksCubeTimer.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class SeriesAdminController : Controller
    {
        private readonly ISeriesService _seriesService;

        public SeriesAdminController(ISeriesService seriesService)
        {
            _seriesService = seriesService;
        }

        [HttpPost]
        public async Task<JsonResult> DeleteSerie(long serieId)
        {
            return Json(await _seriesService.DeleteSerieAsync(serieId));
        }

        [HttpPost]
        public async Task<JsonResult> ModifySerie(IList<SerieModificationViewModel> model)
        {
            if(model == null)
            {
                return Json(new Result(ApplicationResources.TimerDB.Messages.NullArgument));
            }

            if (!CheckDeletionPermissionAmount(model))
            {
                return Json(new Result(ApplicationResources.UserInterface.Common.SolvesAllDelete));
            }

            RemoveUnchangedSolves(model);
            if (model.Count() > 0)
            {
                if (await ParseDurations(model))
                {
                    foreach (var solve in model)
                    {
                        Result result = null;
                        if (solve.NewModify)
                        {
                            result = await _seriesService.UpdateSolveFromSeriesAsync(solve.SerieId, solve.Identity,
                                solve.UserId, solve.Duration, solve.NewDnf, solve.NewPenalty);
                        }
                        else if(solve.NewDelete)
                        {
                            result = await _seriesService.DeleteSolveFromSeriesAsync(solve.SerieId, solve.Identity, solve.UserId);
                        }

                        if (result == null || result.IsFaulted)
                        {
                            return Json(result);
                        }
                    }

                    return Json(new Result(string.Empty, false));
                }

                return Json(new Result(ApplicationResources.UserInterface.Common.WrongDurationFormat));
            }

            return Json(new Result(ApplicationResources.UserInterface.Common.NoDataToupdateSolves));
        }

        private void RemoveUnchangedSolves(IList<SerieModificationViewModel> model)
        {
            var notModifiedSolves = new List<SerieModificationViewModel>();

            foreach (var solve in model)
            {
                if(!solve.NewModify && !solve.NewDelete)
                {
                    notModifiedSolves.Add(solve);
                }
            }

            foreach (var solve in notModifiedSolves)
            {
                model.Remove(solve);
            }
        }

        private async Task<bool> ParseDurations(IList<SerieModificationViewModel> model)
        {
            string format = @"h\:mm\:ss\.fff";
            CultureInfo culture = null;

            foreach (var solve in model)
            {
                if (solve.NewDuration == null)
                {
                    var dur = (await _seriesService.GetAllSolvesOfSerieAsync(solve.SerieId))
                        .FirstOrDefault(s => s.Identity == solve.Identity)?.Duration;
                    if (!dur.HasValue)
                    {
                        return false;
                    }
                    else
                    {
                        solve.Duration = dur.Value;
                        break;
                    }
                }

                bool ok = TimeSpan.TryParseExact(solve.NewDuration, format, culture, TimeSpanStyles.None, out TimeSpan tempTimeSpan);
                if (ok)
                {
                    solve.Duration = tempTimeSpan;
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        private bool CheckDeletionPermissionAmount(IList<SerieModificationViewModel> model)
        {
            var toDelete = model.Count(s => s.NewDelete == true && s.NewModify == false);
            return model.Count() > toDelete;
        }
    }
}
