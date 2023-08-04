using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using System.Net;
using System.Text.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace AdventureCrewBackend.Functions.GetDebugMileMarker;

public class Function
{

    public Function()
    {
    }

    public APIGatewayProxyResponse FunctionHandler(APIGatewayProxyRequest request, ILambdaContext context)
    {
        var mileMarker = new MileMarker
        {
            BookId = 1,
            BookMileMarkerId = 1,
            ExtraMileReference = "ExtraMileDebug 1:1",
            MileMarkerReference = "MileMarkerDebug 1:1",
            IntroText = "Have you ever needed a quick debug function? I have. That's why I created this debug function to give me all sorts of helpful example pieces of Adventure Crew text. It's really helpful to just have a function you can call to give you example text. Yay!",
            IntroTitle = "Debug Intro Title",
            MagnifyingGlassText = "When something is a debug thing, that typically means that it's for use by programmers to make things easier to figure out for them.",
            MagnifyingGlassTitle = "What does 'debug' mean?"
        };

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

    public class MileMarker
    {
        public int BookId { get; set; } // the id of the book containing this mile marker
        public int BookMileMarkerId { get; set; } // the id of this milemarker within the containing book
        public string IntroTitle { get; set; } = string.Empty;
        public string IntroText { get; set; } = string.Empty;
        public string MileMarkerReference { get; set; } = string.Empty;
        public string ExtraMileReference { get; set; } = string.Empty;
        public string MagnifyingGlassTitle { get; set; } = string.Empty;
        public string MagnifyingGlassText { get; set; } = string.Empty;
    }
}
