namespace FGS_BE.Repo.DTOs.TermKeywords
{
    public class CreateTermKeywordDto
    {
        public string Keyword { get; set; } = string.Empty;
        public int BasePoints { get; set; } = 20;
        public int RuleBonus { get; set; } = 10;
        public int SemesterId { get; set; }
    }
}
