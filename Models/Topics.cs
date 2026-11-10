namespace MyCourse.Models
{
    public class MathBranches
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Matters Matter { get; set; } = Matters.Matemática;
    }
    public class MathTopics
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int MathBranchId { get; set; }
        public MathBranches? MathBranch { get; set; }
    }
    public class PhysicsBranches
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Matters Matter { get; set; } = Matters.Física;
    }
    public class PhysicsTopics
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int PhysicsBranchId { get; set; }
        public PhysicsBranches? PhysicsBranch { get; set; }
    }
    public class Branches
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Matters Matter { get; set; }
        public List<Topics> Topic { get; set; } = new();   // opcional, só atalho
    }

    public class Topics
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int BranchId { get; set; }
        public Branches Branch { get; set; } = null!;
    }
}
