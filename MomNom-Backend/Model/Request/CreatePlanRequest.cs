using Newtonsoft.Json;

namespace MomNom_Backend.Model.Request
{
    public class CreatePlanRequest
    {
        public required string DOBstring { get; set; }
        public required int age { get; set; }
        public required decimal height { get; set; }
        public required int weekPregnancy { get; set; }
        public required decimal currentWeight { get; set; }
        public required decimal prePregnancyWeight { get; set; }
    }
}
