using MyCourse.Models;

public class Contracts
{
    public Contracts(Guid userId, Plans plan, bool isActive)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        PlanId = plan.Id;
        Plan = plan;
        IsActive = isActive;
        StartDate = DateTime.UtcNow;
        EndDate = StartDate.AddDays(plan.PlanTimeSpan.Duration.TotalDays);
        PricePaid = plan.Price;
    }
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public Users User { get; set; } = null!;

    public int PlanId { get; set; }
    public Plans Plan { get; set; } = null!;

    public bool IsActive { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal PricePaid { get; set; }

    private Contracts() { }   // o EF Core usa este construtor
}