namespace ServerSideCountriesProject_MeravTomer.BL
{
    public class UserLanguages
    {

        private int userId;
        private int languageId;
        private int? levelLanguage;

        public UserLanguages(int userId, int languageId, int? levelLanguage)
        {
            this.userId = userId;
            this.languageId = languageId;
            this.levelLanguage = levelLanguage;
        }
        public UserLanguages(int userId, int languageId)
        {
            this.userId = userId;
            this.languageId = languageId;
            LevelLanguage = null;
        }
        public UserLanguages()
        {
        }

        public int UserId { get => userId; set => userId = value; }
        public int LanguageId { get => languageId; set => languageId = value; }
        public int? LevelLanguage { get => levelLanguage; set
            {
                if (value != null && (value < 1 || value > 5))
                    throw new ArgumentException("Language level must be between 1 and 5.");

                levelLanguage = value;
            }
        }
    }
}
