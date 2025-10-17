using Microsoft.AspNetCore.Mvc;

namespace MomNom_Backend.Model.Request
{
    public class CustomHeader
    {
        [FromHeader(Name ="Authorization")]
        public required string authToken { get; set; }
    }
}
