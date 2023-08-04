using Amazon.CDK;
using Amazon.CDK.AWS.DynamoDB;
using Constructs;

namespace AdventureCrewBackend
{
    public class DataStack : Stack
    {
        public Table MileMarkersTable;
        public Table BooksTable;
        public Table ReviewsTable;
        public Table MileMarkerDateMappingsTable;
        internal DataStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
            var mileMarkersTable = new Table(this, "MileMarkers", new TableProps
            {
                TableName = "MileMarkers",
                PartitionKey = new Attribute
                {
                    Name = "BookId",
                    Type = AttributeType.NUMBER
                },
                SortKey = new Attribute
                {
                    Name = "bookMileMarkerId",
                    Type = AttributeType.NUMBER
                }
            });

            var booksTable = new Table(this, "Books", new TableProps
            {
                TableName = "Books",
                PartitionKey = new Attribute
                {
                    Name = "id",
                    Type = AttributeType.NUMBER
                }
            });

            var reviewsTable = new Table(this, "Reviews", new TableProps
            {
                TableName = "Reviews",
                PartitionKey = new Attribute
                {
                    Name = "date",
                    Type = AttributeType.NUMBER
                },
                SortKey = new Attribute
                {
                    Name = "bookReviewId",
                    Type = AttributeType.NUMBER
                }
            });

            var mileMarkerDateMappingsTable = new Table(this, "MileMarkerDateMappings", new TableProps
            {
                TableName = "MileMarkerDateMappings",
                PartitionKey = new Attribute
                {
                    Name = "date",
                    Type = AttributeType.NUMBER
                }
            });

            MileMarkersTable = mileMarkersTable;
            BooksTable = booksTable;
            ReviewsTable = reviewsTable;
            MileMarkerDateMappingsTable = mileMarkerDateMappingsTable;
        }
    }
}