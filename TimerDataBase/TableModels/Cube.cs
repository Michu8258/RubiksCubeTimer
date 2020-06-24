using System;
using System.ComponentModel.DataAnnotations;

namespace TimerDataBase.TableModels
{
    public class Cube
    {
        [Key]
        public long Identity { get; set; }

        public Category Category { get; set; }

        public PlasticColor PlasticColor { get; set; }

        public Manufacturer Manufacturer { get; set; }

        [Required]
        public string ModelName { get; set; }

        [Required]
        [Range(0, 2500)]
        public int ReleaseYear { get; set; }

        [Required]
        [Range(0.0, 5.0)]
        public double Rating { get; set; }

        [Required]
        public long RatesAmount { get; set; }

        [Required]
        public bool WcaPermission { get; set; }
    }
}
