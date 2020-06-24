using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using TimerDataBase.TableModels;

namespace WebRubiksCubeTimer.ViewModels.Categories
{
    public class ScrambleUpdateViewModel
    {
        [Required]
        public bool IsModification { get; set; }

        [Required]
        public long Identity { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public string CategoryName { get; set; }

        [Required]
        [DisplayName("Scramble name")]
        public string ScrambleName { get; set; }

        [Required]
        [Range(0, Scramble.ScrambleMaxLength)]
        [DisplayName("Default algorithm length")]
        public int DefaultScrambleLength { get; set; }

        [Required]
        [Range(0, Scramble.ScrambleMaxLength)]
        [DisplayName("Minimum algorithm length")]
        public int MinimumScrambleLength { get; set; }

        [Required]
        [Range(0, Scramble.ScrambleMaxLength)]
        [DisplayName("Maximum algorithm length")]
        public int MaximumScrambleLength { get; set; }

        [Required]
        [DisplayName("Eliminate duplicates")]
        public bool EliminateDuplicates { get; set; }

        [Required]
        [DisplayName("Allow algorithm re-generation")]
        public bool AllowRegenerate { get; set; }

        [Required]
        [DisplayName("Scramble disabled")]
        public bool Disabled { get; set; }

        [Required]
        [DisplayName("Top wall stickers color")]
        public string TopColor { get; set; }

        [Required]
        [DisplayName("Front wall stickers color")]
        public string FrontColor { get; set; }
    }
}
