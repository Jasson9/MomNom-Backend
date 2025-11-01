using Microsoft.EntityFrameworkCore;

namespace MomNom_Backend.Model.Object
{
    [Keyless]
    public class WeightGain
    {
        public int monthNumber { get; set; }
        public string monthName {  get; set; }

        public int year { get; set; }

        public decimal monthlyGain { get; set; }

        public decimal totalGain {  get; set; }

        public decimal recGain {  get; set; }
    }
}
