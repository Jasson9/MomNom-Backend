namespace MomNom_Backend.Model.Response
{
    public class CheckVersionResponse
    {
        public bool isUpToDate { get; set; }
        public string changelogs { get; set; }
        public string versionString { get; set; }
        public string downloadLink { get; set; }
    }
}
