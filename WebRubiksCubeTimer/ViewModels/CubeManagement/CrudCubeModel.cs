using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TimerDataBase.TableModels;

namespace WebRubiksCubeTimer.ViewModels.CubeManagement
{
    public class CrudCubeModel
    {
        public long CubeID { get; set; }

        [Required]
        [Range(minimum: 1.0, maximum: double.MaxValue, ErrorMessage = "Category must be selected.")]
        [Display(Name = "Category")]
        public int CategoryID { get; set; }

        [Required]
        [Range(minimum: 1.0, maximum: double.MaxValue, ErrorMessage = "Manufacturer must be selected.")]
        [Display(Name = "Manufacturer")]
        public int ManufacturerID { get; set; }

        [Required]
        [Range(minimum: 1.0, maximum: double.MaxValue, ErrorMessage = "Plastic color must be selected.")]
        [Display(Name = "Plastic color")]
        public int PlasticColorID { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string ModelName { get; set; }

        [Required]
        [Display(Name = "Release year")]
        [Range(minimum: 1000.0, maximum: 2500.0)]
        public int ReleaseYear { get; set; }

        [Required]
        [Display(Name = "WCA permission")]
        public bool WcaPermission { get; set; }

        public IEnumerable<Manufacturer> Manufacturers { get; set; }
        public IEnumerable<PlasticColor> PlasticColors { get; set; }
        public IEnumerable<Category> Categories { get; set; }
    }
}
