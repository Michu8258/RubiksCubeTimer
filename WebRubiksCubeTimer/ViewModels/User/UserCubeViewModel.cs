namespace WebRubiksCubeTimer.ViewModels.User
{
    public class UserCubeViewModel
    {
        public long Identity { get; set; }
        public string Category { get; set; }
        public string Manufacturer { get; set; }
        public string ModelName { get; set; }
        public string PlasticColor { get; set; }
        public int ReleaseYear { get; set; }
        public double TotalRating { get; set; }
        public ushort UserRate { get; set; }
        public bool WcaPermission { get; set; }
    }
}
