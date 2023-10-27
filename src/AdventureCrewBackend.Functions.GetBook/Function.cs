using AdventureCrewBackend.Common.DatabaseModels;
using AdventureCrewBackend.Common.ViewModels;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using System.Net;
using System.Text.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace AdventureCrewBackend.Functions.GetBook;

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
        var bookIdExists = request.QueryStringParameters.TryGetValue("book", out var queryStringValue);
        var bookIdIsParseable = int.TryParse(queryStringValue, out var bookId);

        if (!bookIdExists || !bookIdIsParseable)
        {
            return new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Body = "Ensure a book query string is included in the request. It should be parseable as an integer.",
                Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "application/json" },
                    { "Access-Control-Allow-Methods", "OPTIONS,POST,GET" },
                    { "Access-Control-Allow-Headers", "Content-Type" },
                    { "Access-Control-Allow-Origin", "*" }
                }
            };
        }

        var bookData = await _context.LoadAsync<Common.DatabaseModels.Book>(bookId);

        List<ScanCondition> mileMarkerScanConditions = new List<ScanCondition>
        {
            new ScanCondition(nameof(Common.DatabaseModels.MileMarker.BookId), ScanOperator.Equal, bookId)
        };

        var mileMarkers = await _context.ScanAsync<Common.DatabaseModels.MileMarker>(mileMarkerScanConditions).GetRemainingAsync();

        if (mileMarkers == null || !mileMarkers.Any())
        {
            return new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.NotFound,
                Body = $"MileMarkers not found. Book ID: {bookId}",
                Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "application/json" },
                    { "Access-Control-Allow-Methods", "OPTIONS,POST,GET" },
                    { "Access-Control-Allow-Headers", "Content-Type" },
                    { "Access-Control-Allow-Origin", "*" }
                }
            };
        }

        List<ScanCondition> reviewScanConditions = new List<ScanCondition>
        {
            new ScanCondition(nameof(Common.DatabaseModels.Review.BookId), ScanOperator.Equal, bookId)
        };

        var reviews = await _context.ScanAsync<Common.DatabaseModels.Review>(reviewScanConditions).GetRemainingAsync();

        if (reviews == null || !reviews.Any())
        {
            return new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.NotFound,
                Body = $"Reviews not found. Book ID: {bookId}",
                Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "application/json" },
                    { "Access-Control-Allow-Methods", "OPTIONS,POST,GET" },
                    { "Access-Control-Allow-Headers", "Content-Type" },
                    { "Access-Control-Allow-Origin", "*" }
                }
            };
        }

        var viewMileMarkers = mileMarkers.Select(x => new Common.ViewModels.MileMarker
        {
            Number = x.BookMileMarkerId.ToString(),
            IntroText = x.IntroText,
            IntroTitle = x.IntroTitle,
            MileMarkerReference = x.MileMarkerReference,
            MileMarkerText = x.MileMarkerText,
            MileMarkerTextSimplified = x.MileMarkerTextSimplified,
            ExtraMileReference = x.ExtraMileReference,
            ExtraMileText = x.ExtraMileText,
            MagnifyingGlassText = x.MagnifyingGlassText,
            MagnifyingGlassTitle = x.MagnifyingGlassTitle,
            Video = x.Video
        });

        var viewReviews = reviews.Select(x => new Common.ViewModels.Review
        {
            Number = x.BookReviewId.ToString(),
            Text = x.Text,
            Questions = x.Questions
        });

        var book = new Common.ViewModels.Book
        {
            Number = bookData.Id.ToString(),
            Name = bookData.Name,
            IntroText = bookData.IntroText,
            SideTripTitle = bookData.SideTripTitle,
            SideTripQuestions = bookData.SideTripQuestions,
            SideTripText = bookData.SideTripText,
            MileMarkers = viewMileMarkers,
            Reviews = viewReviews
        };

        return new APIGatewayProxyResponse
        {
            StatusCode = (int)HttpStatusCode.OK,
            Body = JsonSerializer.Serialize(book, new JsonSerializerOptions
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
}
