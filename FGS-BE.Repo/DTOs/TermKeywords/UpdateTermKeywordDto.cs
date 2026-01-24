namespace FGS_BE.Repo.DTOs.TermKeywords
{
    public class UpdateTermKeywordDto
    {
        public string Keyword { get; set; } = string.Empty;
        public int BasePoints { get; set; }
        public int RuleBonus { get; set; }
    }
}
