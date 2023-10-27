using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureCrewBackend.Common.DatabaseModels
{
    [DynamoDBTable("MileMarkers")]
    public class MileMarker
    {
        [DynamoDBHashKey("bookId")]
        public int BookId { get; set; }
        [DynamoDBRangeKey("bookMileMarkerId")]
        public int BookMileMarkerId { get; set; }
        [DynamoDBProperty("introTitle")]
        public string IntroTitle { get; set; } = string.Empty;
        [DynamoDBProperty("introText")]
        public string IntroText { get; set; } = string.Empty;
        [DynamoDBProperty("mileMarkerReference")]
        public string MileMarkerReference { get; set; } = string.Empty;
        [DynamoDBProperty("mileMarkerText")]
        public string MileMarkerText { get; set; } = string.Empty;
        [DynamoDBProperty("mileMarkerTextSimplified")]
        public string MileMarkerTextSimplified { get; set; } = string.Empty;
        [DynamoDBProperty("extraMileReference")]
        public string ExtraMileReference { get; set; } = string.Empty;
        [DynamoDBProperty("extraMileText")]
        public string ExtraMileText { get; set; } = string.Empty;
        [DynamoDBProperty("magnifyingGlassTitle")]
        public string MagnifyingGlassTitle { get; set; } = string.Empty;
        [DynamoDBProperty("magnifyingGlassText")]
        public string MagnifyingGlassText { get; set; } = string.Empty;
        [DynamoDBProperty("video")]
        public string Video { get; set; } = string.Empty;
    }
}