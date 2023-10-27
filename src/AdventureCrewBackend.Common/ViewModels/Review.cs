namespace AdventureCrewBackend.Common.ViewModels
{
    public class Review
    {
        public string Number { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public string[] Questions { get; set; } = Array.Empty<string>();
    }
}
