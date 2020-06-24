namespace WebRubiksCubeTimer.ViewModels.Requests
{
    public class RequestViewComponentViewModel
    {
        public bool Display { get; set; }
        public long AmountOfNewStatesUser { get; set; }
        public long AmountOfNewStatesAdmin { get; set; }
        public bool AdminOrMod { get; set; }
        public string ControllerName { get; set; }
    }
}
