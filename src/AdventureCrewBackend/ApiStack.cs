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
        internal ApiStack(Construct scope, string id, Certificate certificate, HostedZone hostedZone, Table booksTable, Table mileMarkersTable, IStackProps props = null) : base(scope, id, props)
        {
            var getMileMarker = new Function(this, "GetMileMarkerHandler", new FunctionProps
            {
                Runtime = Runtime.DOTNET_6,
                Code = Code.FromAsset(@"C:\Users\Mango\source\repos\adventure-crew\adventure-crew-backend\src\AdventureCrewBackend.Functions.GetMileMarker\bin\Debug\net6.0"),
                Handler = "AdventureCrewBackend.Functions.GetMileMarker::AdventureCrewBackend.Functions.GetMileMarker.Function::FunctionHandler",
                Timeout = Duration.Seconds(10)
            });

            mileMarkersTable.GrantReadData(getMileMarker);
            booksTable.GrantReadData(getMileMarker);

            var getMileMarkerViaId = new Function(this, "GetMileMarkerViaIdHandler", new FunctionProps
            {
                Runtime = Runtime.DOTNET_6,
                Code = Code.FromAsset(@"C:\Users\Mango\source\repos\adventure-crew\adventure-crew-backend\src\AdventureCrewBackend.Functions.GetMileMarkerViaId\bin\Debug\net6.0"),
                Handler = "AdventureCrewBackend.Functions.GetMileMarkerViaId::AdventureCrewBackend.Functions.GetMileMarkerViaId.Function::FunctionHandler",
                Timeout = Duration.Seconds(10),
            });

            mileMarkersTable.GrantReadData(getMileMarkerViaId);
            booksTable.GrantReadData(getMileMarkerViaId);

            var getDebugMileMarker = new Function(this, "GetDebugMileMarkerHandler", new FunctionProps
            {
                Runtime = Runtime.DOTNET_6,
                Code = Code.FromAsset(@"C:\Users\Mango\source\repos\adventure-crew\adventure-crew-backend\src\AdventureCrewBackend.Functions.GetDebugMileMarker\bin\Debug\net6.0"),
                Handler = "AdventureCrewBackend.Functions.GetDebugMileMarker::AdventureCrewBackend.Functions.GetDebugMileMarker.Function::FunctionHandler",
                Timeout = Duration.Seconds(10),
            });

            var restApi = new RestApi(this, "MileMarkersAPI", new RestApiProps
            {
                RestApiName = "MileMarkersService",
                Description = "Endpoint for accessing MileMarkers DynamoDB table",
                EndpointTypes = new EndpointType[] { EndpointType.EDGE },
                DefaultIntegration = new LambdaIntegration(getDebugMileMarker),
                DomainName = new DomainNameOptions
                {
                    DomainName = string.Join(".", "api", hostedZone.ZoneName),
                    EndpointType = EndpointType.EDGE,
                    Certificate = certificate
                },
                DefaultCorsPreflightOptions = new CorsOptions
                {
                    AllowOrigins = Cors.ALL_ORIGINS,
                    AllowMethods = Cors.ALL_METHODS
                }
            });

            var aRecord = new ARecord(this, "ApiARecord", new ARecordProps
            {
                 Zone = hostedZone,
                 RecordName = "api",
                 Target = RecordTarget.FromAlias(new ApiGateway(restApi))
            });

            var getMileMarkerResource = restApi.Root.AddResource("getMileMarker");
            var getMileMarkerViaIdResource = restApi.Root.AddResource("getMileMarkerViaId");
            var getDebugMileMarkerResource = restApi.Root.AddResource("getDebugMileMarker");

            getMileMarkerResource.AddMethod("GET", new LambdaIntegration(getMileMarker));
            getMileMarkerViaIdResource.AddMethod("GET", new LambdaIntegration(getMileMarkerViaId));
            getDebugMileMarkerResource.AddMethod("GET", new LambdaIntegration(getDebugMileMarker));
        }
    }
}