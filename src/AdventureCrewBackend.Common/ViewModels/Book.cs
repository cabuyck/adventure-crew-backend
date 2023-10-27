namespace AdventureCrewBackend.Common.ViewModels
{
    public class Book
    {
        public string Number { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string IntroText { get; set; } = string.Empty;
        public string SideTripTitle { get; set; } = string.Empty;
        public string SideTripText { get; set; } = string.Empty;
        public IEnumerable<string> SideTripQuestions { get; set; } = Enumerable.Empty<string>();
        public IEnumerable<MileMarker> MileMarkers { get; set; } = Enumerable.Empty<MileMarker>();
        public IEnumerable<Review> Reviews { get; set; } = Enumerable.Empty<Review>();
    }
}