using Microsoft.AspNetCore.Identity;

namespace MyCourse.Models
{
    public class Users : IdentityUser<Guid>
    {
        public Users(){} //para o identity
        public void User(string name, int age, EducationalLevels educationalLevel, DateTime birthDate, Objectives objective, string city, string state)
        {
            Name = name;
            Age = age;
            EducationalLevel = educationalLevel;
            BirthDate = birthDate;
            Objective = objective;
            City = city;
            State = state;
        }
        public string Name { get; set; } = string.Empty;
        public int? Age { get; set; }
        public EducationalLevels? EducationalLevel { get; set; }
        public DateTime? BirthDate { get; set; }
        public Objectives? Objective { get; set; }
        public string City { get; set; } = string.Empty; //front
        public string State { get; set; } = string.Empty; //front
    }
}