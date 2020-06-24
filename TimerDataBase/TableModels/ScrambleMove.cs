using System.ComponentModel.DataAnnotations;

namespace TimerDataBase.TableModels
{
    public class ScrambleMove
    {
        [Key]
        public long Identity { get; set; }

        [Required]
        public Scramble Scramble { get; set; }

        [Required]
        public string Move { get; set; }
    }
}
