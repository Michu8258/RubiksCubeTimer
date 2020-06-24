using System.Collections.Generic;
using TimerDataBase.TableModels;

namespace TimerDataBase.HelperModels
{
    public class CompleteScramble
    {
        public Scramble Scramble { get; set; }
        public IEnumerable<ScrambleMove> Moves { get; set; }

        public CompleteScramble()
        {
            Scramble = new Scramble();
            Moves = new List<ScrambleMove>();
        }

        public CompleteScramble(Scramble scramble, IEnumerable<ScrambleMove> moves)
        {
            Scramble = scramble;
            Moves = moves;
        }
    }
}
