using Amazon.CDK;
using Amazon.CDK.AWS.S3.Deployment;
using Amazon.CDK.AWS.S3;
using Constructs;
using Amazon.CDK.AWS.CertificateManager;
using Amazon.CDK.AWS.CloudFront.Origins;
using Amazon.CDK.AWS.CloudFront;
using Amazon.CDK.AWS.Route53.Targets;
using Amazon.CDK.AWS.Route53;
using System.Collections.Generic;

namespace AdventureCrewBackend
{
    public class WebsiteStack : Stack
    {
        public WebsiteStack(Construct scope, string id, string hostedZoneId, string hostedZoneName, string certificateArn, IStackProps props = null) : base(scope, id, props)
        {
            var bucket = new Bucket(this, "AdventureCrewWebsiteBucket", new BucketProps
            {
                PublicReadAccess = true,
                RemovalPolicy = RemovalPolicy.DESTROY,
                WebsiteIndexDocument = "index.html",
                WebsiteErrorDocument = "index.html",
                AccessControl = BucketAccessControl.BUCKET_OWNER_FULL_CONTROL,
                BlockPublicAccess = BlockPublicAccess.BLOCK_ACLS
            });

            var distribution = new Distribution(this, "AdventureCrewCloudFrontDistribution", new DistributionProps
            {
                DomainNames = new string[]
                {
                    hostedZoneName,
                    string.Join(".", "www", hostedZoneName)
                },
                DefaultBehavior = new BehaviorOptions
                {
                    Origin = new S3Origin(bucket),
                    ViewerProtocolPolicy = ViewerProtocolPolicy.REDIRECT_TO_HTTPS
                },
                Certificate = Certificate.FromCertificateArn(this, "AdventureCrewWebsiteStackCertificateArn", certificateArn)
            });

            new BucketDeployment(this, "AdventureCrewBucketDeployment", new BucketDeploymentProps
            {
                Sources = new ISource[] { Source.Asset(@"C:\Users\Mango\source\repos\adventure-crew\adventure-crew\build") },
                DestinationBucket = bucket,
                AccessControl = BucketAccessControl.BUCKET_OWNER_FULL_CONTROL,
                Distribution = distribution,
                DistributionPaths = new[] { "/*" }
            });

            var apexARecord = new ARecord(this, "ApexAliasRecord", new ARecordProps
            {
                Target = RecordTarget.FromAlias(new CloudFrontTarget(distribution)),
                Zone = HostedZone.FromHostedZoneAttributes(this, "ApexAliasRecordHostedZone", new HostedZoneAttributes
                {
                    ZoneName = hostedZoneName,
                    HostedZoneId = hostedZoneId
                })
            });

            var wwwARecord = new ARecord(this, "AliasRecord", new ARecordProps
            {
                RecordName = "www",
                Target = RecordTarget.FromAlias(new CloudFrontTarget(distribution)),
                Zone = HostedZone.FromHostedZoneAttributes(this, "AliasRecordHostedZone", new HostedZoneAttributes
                {
                    ZoneName = hostedZoneName,
                    HostedZoneId = hostedZoneId
                })
            });
        }
    }
}