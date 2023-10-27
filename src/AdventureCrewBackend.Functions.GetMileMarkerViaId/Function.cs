using AdventureCrewBackend.Common.DatabaseModels;
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
}
