using System;
using System.ComponentModel.DataAnnotations;

namespace TimerDataBase.TableModels
{
    public class Solve
    {
        [Key]
        public long Identity { get; set; }

        public Serie Serie { get; set; }

        [Required]
        public DateTime FinishTimeSpan { get; set; }

        [Required]
        public TimeSpan Duration { get; set; }

        [Required]
        public bool DNF { get; set; }

        [Required]
        public bool PenaltyTwoSeconds { get; set; }

        public string Scramble { get; set; }
    }
}
