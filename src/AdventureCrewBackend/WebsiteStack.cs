using Amazon.CDK;
using Amazon.CDK.AWS.S3.Deployment;
using Amazon.CDK.AWS.S3;
using Constructs;
using Amazon.CDK.AWS.CertificateManager;
using Amazon.CDK.AWS.CloudFront.Origins;
using Amazon.CDK.AWS.CloudFront;
using Amazon.CDK.AWS.Route53.Targets;
using Amazon.CDK.AWS.Route53;

namespace AdventureCrewBackend
{
    public class WebsiteStack : Stack
    {
        public WebsiteStack(Construct scope, string id, HostedZone hostedZone, Certificate certificate, IStackProps props = null) : base(scope, id, props)
        {
            var bucket = new Bucket(this, "AdventureCrewWebsiteBucket", new BucketProps
            {
                PublicReadAccess = true,
                RemovalPolicy = RemovalPolicy.DESTROY,
                WebsiteIndexDocument = "index.html",
                WebsiteErrorDocument = "error.html",
                AccessControl = BucketAccessControl.BUCKET_OWNER_FULL_CONTROL,
                BlockPublicAccess = BlockPublicAccess.BLOCK_ACLS
            });

            new BucketDeployment(this, "AdventureCrewBucketDeployment", new BucketDeploymentProps
            {
                Sources = new ISource[] { Source.Asset(@"C:\Users\Mango\source\repos\adventure-crew\adventure-crew\build") },
                DestinationBucket = bucket,
                AccessControl = BucketAccessControl.BUCKET_OWNER_FULL_CONTROL
            });

            var distribution = new Distribution(this, "AdventureCrewCloudFrontDistribution", new DistributionProps
            {
                DomainNames = new string[]
                {
                    string.Join(".", "www", hostedZone.ZoneName)
                },
                DefaultBehavior = new BehaviorOptions
                {
                    Origin = new S3Origin(bucket),
                    ViewerProtocolPolicy = ViewerProtocolPolicy.REDIRECT_TO_HTTPS
                },
                Certificate = certificate
            });

            var aRecord = new ARecord(this, "AliasRecord", new ARecordProps
            {
                RecordName = "www",
                Target = RecordTarget.FromAlias(new CloudFrontTarget(distribution)),
                Zone = hostedZone
            });
        }
    }
}