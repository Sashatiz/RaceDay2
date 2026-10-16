
namespace RaceDay.API.Models
{
    public class Result
    {
        public int ResultID { get; set; }

        public int EnrolmentID { get; set; }

        public TimeSpan FinishTime { get; set; }

        public int FinishPosition { get; set; }

        public string ResultStatus { get; set; } = "Recorded";

        public DateTime RecordedAt { get; set; }
    }
}
