using FGS_BE.Repo.Enums;

namespace FGS_BE.Repo.DTOs.RewardItems
{
    public class RewardItemDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal PriceInPoints { get; set; }
        public int Quantity { get; set; }
        public RewardItemType Type { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        public RewardItemDto() { }

        public RewardItemDto(FGS_BE.Repo.Entities.RewardItem entity)
        {
            Id = entity.Id;
            Name = entity.Name;
            Description = entity.Description;
            PriceInPoints = entity.PriceInPoints;
            Quantity = entity.Quantity;
            Type = entity.Type;
            IsActive = entity.IsActive;
            CreatedAt = entity.CreatedAt;
        }
    }
}