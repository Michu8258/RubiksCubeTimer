namespace WebRubiksCubeTimer.PasswordResetting.Helpers
{
    public class ResetResult
    {
        public bool Success { get; set; }

        public bool NoRequest { get; set; }
        
        public bool Blocked { get; set; }

        public bool Expired { get; set; }

        public int Attempts { get; set; }
    }
}
