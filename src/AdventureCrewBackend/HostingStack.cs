using Amazon.CDK;
using Amazon.CDK.AWS.CertificateManager;
using Amazon.CDK.AWS.CloudFront;
using Amazon.CDK.AWS.Route53;
using Amazon.CDK.AWS.Route53.Targets;
using Constructs;

namespace AdventureCrewBackend
{
    public class HostingStack : Stack
    {
        public string HostedZoneName;
        public string HostedZoneId;
        public string CertificateArn;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="id"></param>
        /// <param name="domainName">example: mysite.com</param>
        /// <param name="props"></param>
        internal HostingStack(Construct scope, string id, string domainName, IStackProps props = null) : base(scope, id, props)
        {
            var hostedZone = new HostedZone(this, "HostedZone", new HostedZoneProps
            {
                ZoneName = domainName
            });

            var certificate = new Certificate(this, "Certificate", new CertificateProps
            {
                DomainName = domainName,
                SubjectAlternativeNames = new string[] { string.Join("", "*.", domainName) },
                Validation = CertificateValidation.FromDns(hostedZone),
            });

            HostedZoneName = hostedZone.ZoneName;
            HostedZoneId = hostedZone.HostedZoneId;
            CertificateArn = certificate.CertificateArn;
        }
    }
}