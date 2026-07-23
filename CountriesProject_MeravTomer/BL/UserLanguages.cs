namespace ServerSideCountriesProject_MeravTomer.BL
{
    public class UserLanguages
    {
        private int userId;
        private Language language;
        private int? levelLanguage;

        public UserLanguages()
        {
        }

        public UserLanguages(
            int userId,
            Language language,
            int? levelLanguage)
        {
            UserId = userId;
            Language = language;
            LevelLanguage = levelLanguage;
        }

        public UserLanguages(
            int userId,
            Language language)
        {
            UserId = userId;
            Language = language;
            LevelLanguage = null;
        }

        public int UserId
        {
            get => userId;
            set => userId = value;
        }

        public Language Language
        {
            get => language;
            set => language = value;
        }

        public int? LevelLanguage
        {
            get => levelLanguage;
            set
            {
                if (value != null && (value < 1 || value > 5))
                {
                    throw new ArgumentException(
                        "Language level must be between 1 and 5.");
                }

                levelLanguage = value;
            }
        }
    }
}