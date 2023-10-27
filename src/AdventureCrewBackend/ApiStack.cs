using Amazon.CDK;
using Amazon.CDK.AWS.APIGateway;
using Amazon.CDK.AWS.CertificateManager;
using Amazon.CDK.AWS.DynamoDB;
using Amazon.CDK.AWS.ElasticLoadBalancingV2;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.Route53;
using Amazon.CDK.AWS.Route53.Targets;
using Constructs;
using System.Collections.Generic;

namespace AdventureCrewBackend
{
    public class ApiStack : Stack
    {
        internal ApiStack(Construct scope, string id, string certificateArn, string hostedZoneId, string hostedZoneName, Table booksTable, Table mileMarkersTable, Table reviewsTable, IStackProps props = null) : base(scope, id, props)
        {
            var getDebugMileMarker = new Function(this, "GetDebugMileMarkerHandler", new FunctionProps
            {
                Runtime = Runtime.DOTNET_6,
                Code = Code.FromAsset(@"C:\Users\Mango\source\repos\adventure-crew\adventure-crew-backend\src\AdventureCrewBackend.Functions.GetDebugMileMarker\bin\Debug\net6.0"),
                Handler = "AdventureCrewBackend.Functions.GetDebugMileMarker::AdventureCrewBackend.Functions.GetDebugMileMarker.Function::FunctionHandler",
                Timeout = Duration.Seconds(10),
            });

            var getBook = new Function(this, "GetBookHandler", new FunctionProps
            {
                Runtime = Runtime.DOTNET_6,
                Code = Code.FromAsset(@"C:\Users\Mango\source\repos\adventure-crew\adventure-crew-backend\src\AdventureCrewBackend.Functions.GetBook\bin\Debug\net6.0"),
                Handler = "AdventureCrewBackend.Functions.GetBook::AdventureCrewBackend.Functions.GetBook.Function::FunctionHandler",
                Timeout = Duration.Seconds(30),
            });

            booksTable.GrantReadData(getBook);
            mileMarkersTable.GrantReadData(getBook);
            reviewsTable.GrantReadData(getBook);

            var getAllBooks = new Function(this, "GetAllBooksHandler", new FunctionProps
            {
                Runtime = Runtime.DOTNET_6,
                Code = Code.FromAsset(@"C:\Users\Mango\source\repos\adventure-crew\adventure-crew-backend\src\AdventureCrewBackend.Functions.GetAllBooks\bin\Debug\net6.0"),
                Handler = "AdventureCrewBackend.Functions.GetAllBooks::AdventureCrewBackend.Functions.GetAllBooks.Function::FunctionHandler",
                Timeout = Duration.Seconds(30),
            });

            booksTable.GrantReadData(getAllBooks);
            mileMarkersTable.GrantReadData(getAllBooks);
            reviewsTable.GrantReadData(getAllBooks);

            var restApi = new RestApi(this, "MileMarkersAPI", new RestApiProps
            {
                RestApiName = "MileMarkersService",
                Description = "Endpoint for accessing MileMarkers DynamoDB table",
                EndpointTypes = new EndpointType[] { EndpointType.EDGE },
                DefaultIntegration = new LambdaIntegration(getDebugMileMarker),
                DomainName = new DomainNameOptions
                {
                    DomainName = string.Join(".", "api", hostedZoneName),
                    EndpointType = EndpointType.EDGE,
                    Certificate = Certificate.FromCertificateArn(this, "AdventureCrewApiStackCertificateArn", certificateArn)
                },
                DefaultCorsPreflightOptions = new CorsOptions
                {
                    AllowOrigins = Cors.ALL_ORIGINS,
                    AllowMethods = Cors.ALL_METHODS
                },
                DeployOptions = new StageOptions
                {
                     CacheClusterEnabled = true,
                     CachingEnabled = true,
                }
            });

            var aRecord = new ARecord(this, "ApiARecord", new ARecordProps
            {
                 Zone = HostedZone.FromHostedZoneAttributes(this, "ApiARecordHostedZone", new HostedZoneAttributes
                 {
                      ZoneName = hostedZoneName,
                      HostedZoneId = hostedZoneId
                 }),
                 RecordName = "api",
                 Target = RecordTarget.FromAlias(new ApiGateway(restApi))
            });

            var getDebugMileMarkerResource = restApi.Root.AddResource("getDebugMileMarker");
            var getBookResource = restApi.Root.AddResource("getBook");
            var getAllBooksResource = restApi.Root.AddResource("getAllBooks");

            getDebugMileMarkerResource.AddMethod("GET", new LambdaIntegration(getDebugMileMarker));
            getBookResource.AddMethod("GET", new LambdaIntegration(getBook));
            getAllBooksResource.AddMethod("GET", new LambdaIntegration(getAllBooks));
        }
    }
}