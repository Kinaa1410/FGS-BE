using FGS_BE.Repo.Entities;
using FGS_BE.Repo.Enums;

namespace FGS_BE.Repo.DTOs.RedeemRequests
{
    public class CreateRedeemRequestDto
    {
        public int Quantity { get; set; }
        public decimal TotalPoints { get; set; }
        public int UserId { get; set; }
        public int RewardItemId { get; set; }

        public RedeemRequest ToEntity()
        {
            return new RedeemRequest
            {
                Quantity = this.Quantity,
                TotalPoints = this.TotalPoints,
                Status = RedeemRequestStatus.Pending,
                RequestedAt = DateTime.UtcNow,
                ProcessedAt = null,
                UserId = this.UserId,
                RewardItemId = this.RewardItemId
            };
        }
    }
}