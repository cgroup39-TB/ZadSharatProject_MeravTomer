namespace ServerSideCountriesProject_MeravTomer.Models
{
    public class Language
    {
        public int LanguageId { get; set; }
        public string LanguageName { get; set; }

        public Language()
        {
        }

        public Language(int languageId, string languageName)
        {
            LanguageId = languageId;
            LanguageName = languageName;
        }
    }
}
