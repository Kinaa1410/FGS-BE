using FGS_BE.Repo.Enums;

namespace FGS_BE.Repo.DTOs.RewardItems
{
    public class UpdateRewardItemDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal? PriceInPoints { get; set; }
        public int? Quantity { get; set; }
        //public RewardItemType? Type { get; set; }
        public bool? IsActive { get; set; }

        public void ApplyToEntity(Entities.RewardItem entity)
        {
            if (Name != null) entity.Name = Name;
            if (Description != null) entity.Description = Description;
            if (PriceInPoints.HasValue) entity.PriceInPoints = PriceInPoints.Value;
            if (Quantity.HasValue) entity.Quantity = Quantity.Value;
            //if (Type.HasValue) entity.Type = Type.Value;
            if (IsActive.HasValue) entity.IsActive = IsActive.Value;
        }
    }
}