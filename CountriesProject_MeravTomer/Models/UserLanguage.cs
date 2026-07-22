namespace ServerSideCountriesProject_MeravTomer.Models
{
    // A language a user speaks, and at what level (UserLanguages joined with Languages).
    public class UserLanguage
    {
        public int LanguageId { get; set; }
        public string LanguageName { get; set; }
        public string ProficiencyLevel { get; set; }

        public UserLanguage()
        {
        }

        public UserLanguage(int languageId, string languageName, string proficiencyLevel)
        {
            LanguageId = languageId;
            LanguageName = languageName;
            ProficiencyLevel = proficiencyLevel;
        }
    }
}
