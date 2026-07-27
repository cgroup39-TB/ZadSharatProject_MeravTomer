namespace ServerSideCountriesProject_MeravTomer.BL
{
    /// <summary>Links a user to a language they speak, with an optional proficiency level (1-5).</summary>
    public class UserLanguages
    {
        private int userId;
        private Language language;
        private int? levelLanguage;

        /// <summary>Creates an empty instance (used by model binding/deserialization).</summary>
        public UserLanguages()
        {
        }

        /// <summary>Creates a user/language link with an explicit proficiency level.</summary>
        public UserLanguages(
            int userId,
            Language language,
            int? levelLanguage)
        {
            UserId = userId;
            Language = language;
            LevelLanguage = levelLanguage;
        }

        /// <summary>Creates a user/language link with no proficiency level set.</summary>
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