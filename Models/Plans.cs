namespace MyCourse.Models
{
    public class Plans
    {
        public void NewPlan(string name, string description, decimal price, int planTimeSpanId, bool highlight, int order)
        {
            Name = name;
            Description = description;
            Price = price;
            PlanTimeSpanId = planTimeSpanId;
            Highlight = highlight;
            Order = order;
        }
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        //Código do plano de pagamento
        public int PlanTimeSpanId { get; set; }
        public PlanTimeSpans PlanTimeSpan { get; set; } = null!;
        public TimeSpan TestTime { get; set; } = TimeSpan.FromDays(7);
        public bool Highlight { get; set; } 
        public int Order { get; set; } 
    }
    public class PlanTimeSpans
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public TimeSpan Duration { get; set; }
    }
    public class PlanMatters
    {
        public int Id { get; set; }
        public int PlanId { get; set; }
        public Plans Plan { get; set; } = null!;
        public int MatterId { get; set; }
        public Matters Matter { get; set; }
    }
}