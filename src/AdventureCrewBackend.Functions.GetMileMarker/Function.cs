using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace AdventureCrewBackend.Functions.GetMileMarker;

public class Function
{
    private static AmazonDynamoDBClient _client = new AmazonDynamoDBClient();
    private DynamoDBContext _context;

    public Function()
    {
        _context = new DynamoDBContext(_client);
    }

    public async Task<MileMarker> FunctionHandler(APIGatewayProxyRequest request, ILambdaContext context)
    {
        request.QueryStringParameters.TryGetValue("date", out var date);

        var dateMapping = await _context.LoadAsync<MileMarkerDateMapping>(date);

        if (dateMapping == null)
        {
            throw new Exception($"No {nameof(MileMarkerDateMapping)} found for the provided date.");
        }

        var mileMarker = await _context.LoadAsync<MileMarker>(dateMapping.BookId, dateMapping.BookMileMarkerId);

        if (mileMarker == null)
        {
            throw new Exception($"No {nameof(MileMarker)} found with the provided keys: BookId {dateMapping.BookId} BookMileMarkerId {dateMapping.BookMileMarkerId}");
        }

        return mileMarker;
    }

    public class MileMarkerDateMapping
    {
        [DynamoDBHashKey("date")]
        public string Date { get; set; } = string.Empty;
        public int BookId { get; set; }
        public int BookMileMarkerId { get; set; }
    }

    public class Book
    {
        [DynamoDBHashKey("id")]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string IntroText { get; set; } = string.Empty;
        public string SideTripTitle { get; set; } = string.Empty;
        public IEnumerable<string> SideTripParagraphs { get; set; } = Enumerable.Empty<string>();
        public IEnumerable<string> SideTripQuestions { get; set; } = Enumerable.Empty<string>();
        public IEnumerable<string> SideTripActivities { get; set; } = Enumerable.Empty<string>();
        public IEnumerable<MileMarker> MileMarkers { get; set; }
        public IEnumerable<Review> Reviews { get; set; }
    }

    public class MileMarker
    {
        [DynamoDBHashKey("bookId")]
        public int BookId { get; set; } // the id of the book containing this mile marker
        [DynamoDBRangeKey("bookMileMarkerId")]
        public int BookMileMarkerId { get; set; } // the id of this milemarker within the containing book
        public string IntroTitle { get; set; } = string.Empty;
        public string IntroText { get; set; } = string.Empty;
        public string MileMarkerReference { get; set; } = string.Empty;
        public string ExtraMileReference { get; set; } = string.Empty;
        public string MagnifyingGlassTitle { get; set; } = string.Empty;
        public string MagnifyingGlassText { get; set; } = string.Empty;
    }

    public class Review
    {
        [DynamoDBHashKey("date")]
        public int BookId { get; set; }
        [DynamoDBRangeKey("bookReviewId")]
        public int BookReviewId { get; set; } // sort key // the id of the review within the book
        public string Text { get; set; } = string.Empty;
        public IEnumerable<string> Questions { get; set; } = Enumerable.Empty<string>();
    }
}
