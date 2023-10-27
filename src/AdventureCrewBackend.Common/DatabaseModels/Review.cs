using Amazon.DynamoDBv2.DataModel;

namespace AdventureCrewBackend.Common.DatabaseModels
{
    [DynamoDBTable("Reviews")]
    public class Review
    {
        [DynamoDBHashKey("bookId")]
        public int BookId { get; set; }
        [DynamoDBRangeKey("bookReviewId")]
        public int BookReviewId { get; set; }
        [DynamoDBProperty("text")]
        public string Text { get; set; } = string.Empty;
        [DynamoDBProperty("questions")]
        public string[] Questions { get; set; } = Array.Empty<string>();
    }
}
