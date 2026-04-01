namespace VisualSoft.Surveillance.Services.Adapters.Models
{
    public class NumberPlateAnalysisResponse
    {
        public string ReferenceNumber { get; set; }

        public IEnumerable<DetectionResponse> Detection { get; set; }


        public class DetectionResponse
        {
            public string ImageReferenceNumber { get; set; }
            public decimal Confidence { get; set; }
            public string LicensePlateNumber { get; set; }
            public decimal LicensePlateConfidence { get; set; }

        }
    }
}
