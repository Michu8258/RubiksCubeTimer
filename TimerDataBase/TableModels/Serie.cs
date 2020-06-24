using System;
using System.ComponentModel.DataAnnotations;

namespace TimerDataBase.TableModels
{
    public class Serie
    {
        [Key]
        public long Identity { get; set; }

        [Required]
        public string UserIdentity { get; set; }

        public Cube Cube { get; set; }

        [Required]
        public DateTime StartTimeStamp { get; set; }

        [Required]
        public TimeSpan LongestResult { get; set; }

        [Required]
        public TimeSpan ShortestResult { get; set; }

        public TimeSpan AverageTime { get; set; }

        public TimeSpan MeanOf3 { get; set; }

        public TimeSpan AverageOf5 { get; set; }

        [Required]
        public bool AtLeastOneDNF { get; set; }

        public CategoryOption SerieOption { get; set; }

        [Required]
        public bool SerieFinished { get; set; }

        [Required]
        public int SolvesAmount { get; set; }
    }
}
