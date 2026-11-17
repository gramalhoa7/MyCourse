namespace MyCourse.Models
{
    // Todos os campos são opcionais: só o que vier preenchido é alterado
    public class SettingsRequest
    {
        public bool? DarkTheme { get; set; }
        public bool? EmailNotifications { get; set; }
        public bool? AutoPlayNextVideo { get; set; }
        public TextSize? TextSize { get; set; }
        public bool? HighContrast { get; set; }
        public bool? ReduceMotion { get; set; }
    }
}