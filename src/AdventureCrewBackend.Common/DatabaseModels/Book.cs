using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureCrewBackend.Common.DatabaseModels
{
    [DynamoDBTable("Books")]
    public class Book
    {
        [DynamoDBHashKey("id")]
        public int Id { get; set; }
        [DynamoDBProperty("name")]
        public string Name { get; set; } = string.Empty;
        [DynamoDBProperty("introText")]
        public string IntroText { get; set; } = string.Empty;
        [DynamoDBProperty("sideTripTitle")]
        public string SideTripTitle { get; set; } = string.Empty;
        [DynamoDBProperty("sideTripText")]
        public string SideTripText { get; set; } = string.Empty;
        [DynamoDBProperty("sideTripQuestions")]
        public string[] SideTripQuestions { get; set; } = Array.Empty<string>();
    }
}
