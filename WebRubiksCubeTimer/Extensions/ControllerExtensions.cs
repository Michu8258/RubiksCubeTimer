using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using WebRubiksCubeTimer.Models.Pagination;

namespace WebRubiksCubeTimer.Extensions
{
    public static class ControllerExtensions
    {
        public static void AddErrorsFromResult(this Controller controller, IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                controller.ModelState.AddModelError("", error.Description);
            }
        }

        public static PaginationModel GetPaginationData(this Controller controller, long itemsAmount, int pageSize, int currentPageNumber)
        {
            int pagesAmount = Convert.ToInt32(Math.Floor((decimal)itemsAmount / pageSize));

            if (itemsAmount % pageSize != 0)
            {
                pagesAmount++;
            }

            var pagination = new PaginationModel()
            {
                AmountOfPages = pagesAmount,
                CurrentPage = currentPageNumber,
                PageSize = pageSize,
            };

            if (currentPageNumber < 1)
            {
                pagination.CurrentPage = 1;
            }

            if (currentPageNumber > pagesAmount)
            {
                pagination.CurrentPage = pagesAmount;
            }

            return pagination;
        }
    }
}
