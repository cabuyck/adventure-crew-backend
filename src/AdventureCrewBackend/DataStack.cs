using Amazon.CDK;
using Amazon.CDK.AWS.DynamoDB;
using Constructs;

namespace AdventureCrewBackend
{
    public class DataStack : Stack
    {
        public Table BooksTable;
        public Table MileMarkersTable;
        public Table ReviewsTable;
        internal DataStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
            var booksTable = new Table(this, "Books", new TableProps
            {
                TableName = "Books",
                PartitionKey = new Attribute
                {
                    Name = "id",
                    Type = AttributeType.NUMBER
                }
            });

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

            BooksTable = booksTable;
            MileMarkersTable = mileMarkersTable;
            ReviewsTable = reviewsTable;
        }
    }
}