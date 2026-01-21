using FGS_BE.Repo.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FGS_BE.Repo.DTOs.RewardItems
{
    public class CreateRewardItemDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public decimal PriceInPoints { get; set; }
        public int Quantity { get; set; }
        //public RewardItemType Type { get; set; }
        public bool IsActive { get; set; } = true;

        public Entities.RewardItem ToEntity()
        {
            return new Entities.RewardItem
            {
                Name = Name,
                Description = Description,
                PriceInPoints = PriceInPoints,
                Quantity = Quantity,
                //Type = Type,
                IsActive = IsActive,
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}
