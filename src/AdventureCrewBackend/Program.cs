using Amazon.CDK;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventureCrewBackend
{
    sealed class Program
    {
        public static void Main(string[] args)
        {
            var app = new App();
            var domainName = "buyck.dev";
            var hostingStack = new HostingStack(app, "AdventureCrewHostingStack", domainName);
            new WebsiteStack(app, "AdventureCrewWebsiteStack", hostingStack.HostedZone, hostingStack.Certificate);
            var dataStack = new DataStack(app, "AdventureCrewDataStack");
            new ApiStack(app, "AdventureCrewApiStack", hostingStack.Certificate, hostingStack.HostedZone, dataStack.MileMarkersTable, dataStack.BooksTable);

            // TODO update GetMileMarker Lambda to return milemarker nearest the specified date.

            // TODO -> S3 custom resource that contains raw Adventure Crew book data.
            // Bootstrap Lambda function that runs only once after deployment -> gets files from S3 and inserts them into database.


            app.Synth();
        }
    }
}
