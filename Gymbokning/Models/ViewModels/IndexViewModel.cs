namespace Gymbokning.Models.ViewModels
{
    public class IndexViewModel
    {
        public IEnumerable<IndexGymClassViewModel> GymClasses { get; set; } = Enumerable.Empty<IndexGymClassViewModel>();
        public bool ShowHistory { get; set; } = false;
        public bool HasBooking { get; set; } = false;
    }
}
