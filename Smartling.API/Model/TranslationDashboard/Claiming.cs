namespace Smartling.Api.Model.TranslationDashboard
{
    public class Claiming
    {
        public bool isClaimable { get; set; }
        public bool isUnclaimable { get; set; }
        public int claimableWordCount { get; set; }
    }
}
