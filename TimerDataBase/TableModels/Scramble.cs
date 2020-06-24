using System;
using System.ComponentModel.DataAnnotations;

namespace TimerDataBase.TableModels
{
    public class Scramble
    {
        public const int ScrambleMaxLength = 1000;

        [Key]
        public long Identity { get; set; }

        [Required]
        public bool Default { get; set; }

        [Required]
        public Category Category { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [Range(0, ScrambleMaxLength)]
        public int DefaultScrambleLength { get; set; }

        [Required]
        [Range(0, ScrambleMaxLength)]

        public int MinimumScrambleLength { get; set; }

        [Required]
        [Range(0, ScrambleMaxLength)]
        public int MaximumScrambleLength { get; set; }

        [Required]
        public bool EliminateDuplicates { get; set; }

        [Required]
        public bool AllowRegenerate { get; set; }

        [Required]
        public bool Disabled { get; set; }

        [Required]
        public string TopColor { get; set; }

        [Required]
        public string FrontColor { get; set; }

        [Required]
        public int MovesAmount { get; set; }
    }
}
