using MyCourse.Models;

namespace MyCourse.Routes;

public class PlansRequest
{
    public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        //Código do plano de pagamento
        public int PlanTimeSpanId { get; set; }
        public PlanTimeSpans PlanTimeSpan { get; set; } = null!;
        public TimeSpan TestTime { get; set; } = TimeSpan.FromDays(7);
        public bool Highlight { get; set; } 
        public int? Order { get; set; } 
}