using FGS_BE.Repo.Entities;
using FGS_BE.Repo.Enums;

namespace FGS_BE.Repo.DTOs.RedeemRequests
{
    public class RedeemRequestDto
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPoints { get; set; }
        public RedeemRequestStatus Status { get; set; }
        public DateTime RequestedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public int UserId { get; set; }
        public int RewardItemId { get; set; }

        public RedeemRequestDto() { }

        public RedeemRequestDto(RedeemRequest entity)
        {
            Id = entity.Id;
            Quantity = entity.Quantity;
            TotalPoints = entity.TotalPoints;
            Status = entity.Status;
            RequestedAt = entity.RequestedAt;
            ProcessedAt = entity.ProcessedAt;
            UserId = entity.UserId;
            RewardItemId = entity.RewardItemId;
        }
    }
}