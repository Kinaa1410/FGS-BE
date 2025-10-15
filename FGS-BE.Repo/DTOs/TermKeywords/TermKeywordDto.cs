namespace FGS_BE.Repo.DTOs.TermKeywords
{
    public class TermKeywordDto
    {
        public int Id { get; set; }
        public string? Keyword { get; set; }
        public int BasePoints { get; set; }
        public int RuleBonus { get; set; }
        public int SemesterId { get; set; }

        public TermKeywordDto() { }

        public TermKeywordDto(FGS_BE.Repo.Entities.TermKeyword entity)
        {
            Id = entity.Id;
            Keyword = entity.Keyword;
            BasePoints = entity.BasePoints;
            RuleBonus = entity.RuleBonus;
            SemesterId = entity.SemesterId;
        }
    }
}
