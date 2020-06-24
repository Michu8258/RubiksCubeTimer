using System;
using System.Linq;

namespace WebRubiksCubeTimer.PasswordResetting.Helpers
{
    public static class RandomKeyGenerator
    {
        const string PERMITTED_CHARACTERS = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        public static string RandomString(int length)
        {
            var random = new Random();
            const string chars = PERMITTED_CHARACTERS;
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
