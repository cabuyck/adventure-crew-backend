using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using System.Net;
using System.Text.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace AdventureCrewBackend.Functions.GetMileMarkerViaId;

public class Function
{
    private static AmazonDynamoDBClient _client = new AmazonDynamoDBClient();
    private DynamoDBContext _context;

    public Function()
    {
        _context = new DynamoDBContext(_client);
    }

    public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest request, ILambdaContext context)
    {
        var bookIdExists = request.QueryStringParameters.TryGetValue("book", out var bookId);
        var mileMarkerIdExists = request.QueryStringParameters.TryGetValue("milemarker", out var mileMarkerId);

        if (!bookIdExists || !mileMarkerIdExists)
        {
            return new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Body = "Ensure book and milemarker query strings are included in the request.",
                Headers = new Dictionary<string, string>
            {
                { "Content-Type", "application/json" },
                { "Access-Control-Allow-Methods", "OPTIONS,POST,GET" },
                { "Access-Control-Allow-Headers", "Content-Type" },
                { "Access-Control-Allow-Origin", "*" }
            }
            };
        }

        var mileMarker = await _context.LoadAsync<MileMarker>(int.Parse(bookId), int.Parse(mileMarkerId));

        if (mileMarker == null)
        {
            throw new Exception($"No {nameof(MileMarker)} found with the provided keys: BookId {bookId} BookMileMarkerId {mileMarkerId}");
        }

        return new APIGatewayProxyResponse
        {
            StatusCode = (int)HttpStatusCode.OK,
            Body = JsonSerializer.Serialize(mileMarker, new JsonSerializerOptions
            {
                 PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }),
            Headers = new Dictionary<string, string>
            {
                { "Content-Type", "application/json" },
                { "Access-Control-Allow-Methods", "OPTIONS,POST,GET" },
                { "Access-Control-Allow-Headers", "Content-Type" },
                { "Access-Control-Allow-Origin", "*" }
            }
        };
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

    [DynamoDBTable("MileMarkers")]
    public class MileMarker
    {
        [DynamoDBHashKey("bookId")]
        public int BookId { get; set; } // the id of the book containing this mile marker
        [DynamoDBRangeKey("bookMileMarkerId")]
        public int BookMileMarkerId { get; set; } // the id of this milemarker within the containing book
        [DynamoDBProperty("introTitle")]
        public string IntroTitle { get; set; } = string.Empty;
        [DynamoDBProperty("intoText")]
        public string IntroText { get; set; } = string.Empty;
        [DynamoDBProperty("mileMarkerReference")]
        public string MileMarkerReference { get; set; } = string.Empty;
        [DynamoDBProperty("extraMileReference")]
        public string ExtraMileReference { get; set; } = string.Empty;
        [DynamoDBProperty("magnifyingGlassTitle")]
        public string MagnifyingGlassTitle { get; set; } = string.Empty;
        [DynamoDBProperty("magnifyingGlassText")]
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
