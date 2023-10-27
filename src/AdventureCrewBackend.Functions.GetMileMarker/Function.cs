using AdventureCrewBackend.Common.DatabaseModels;
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
        return null;
    }
}
