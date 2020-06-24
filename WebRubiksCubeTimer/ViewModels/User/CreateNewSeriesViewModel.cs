using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TimerDataBase.TableModels;

namespace WebRubiksCubeTimer.ViewModels.User
{
    public class CreateNewSeriesViewModel
    {
        public CreateNewSeriesViewModel()
        {
            AvailableCategories = new List<Category>();
        }

        [Required]
        public string UserIdentity { get; set; }

        [Required]
        public long SeriesId { get; set; }

        [Required]
        [Range(1.0, double.MaxValue, ErrorMessage = "Category must be selected")]
        [Display(Name = "Selected cube")]
        public int SelectedCategoryId { get; set; }

        [Required]
        [Range(1.0, double.MaxValue, ErrorMessage = "Manufacturer must be selected")]
        [Display(Name = "Selected cube")]
        public int SelectedManufacturerId { get; set; }

        [Required]
        [Range(1.0, double.MaxValue, ErrorMessage = "Cube must be selected")]
        [Display(Name = "Selected cube")]
        public long SelectedCubeId { get; set; }

        [Required]
        [Range(1.0, double.MaxValue, ErrorMessage = "Category option must be selected")]
        [Display(Name = "Selected category option")]
        public int CategoryOptionId { get; set; }

        public IEnumerable<Category> AvailableCategories { get; set; }
    }
}
