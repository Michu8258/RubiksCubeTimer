using System.ComponentModel.DataAnnotations;

namespace TimerDataBase.TableModels
{
    public class Manufacturer
    {
        [Key]
        public int Identity { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Country { get; set; }

        [Required]
        [Range(0, 2500)]
        public uint FoundationYear { get; set; }
    }
}
