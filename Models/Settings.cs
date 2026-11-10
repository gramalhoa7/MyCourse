namespace MyCourse.Models
{
    public class UserSettings
    {
        public void UpdateUserSettingsDto(Guid userId, bool darkTheme, bool emailNotifications, bool autoPlayNextVideo, TextSize textSize, bool highContrast, bool reduceMotion)
        {
            UserId = userId;
            DarkTheme = darkTheme;
            EmailNotifications = emailNotifications;
            AutoPlayNextVideo = autoPlayNextVideo;
            TextSize = textSize;
            HighContrast = highContrast;
            ReduceMotion = reduceMotion;
        }
        public void SetDefaultSettings()
        {
            DarkTheme = true;
            EmailNotifications = true;
            AutoPlayNextVideo = true;
            TextSize = TextSize.Medium;
            HighContrast = false;
            ReduceMotion = false;
        }
        public Guid? UserId { get; set; }            // é a chave: 1 linha por aluno
        public Users? User { get; set; } = null!;

        public bool? DarkTheme { get; set; } = true;
        public bool? EmailNotifications { get; set; } = true;
        public bool? AutoPlayNextVideo { get; set; } = true;

        // acessibilidade
        public TextSize TextSize { get; set; } = TextSize.Medium;
        public bool? HighContrast { get; set; }
        public bool? ReduceMotion { get; set; }
    }
    public enum TextSize
    {
        Small = 1,
        Medium = 2,
        Large = 3,
        ExtraLarge = 4
    }
}