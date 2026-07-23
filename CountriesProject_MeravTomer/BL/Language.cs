namespace ServerSideCountriesProject_MeravTomer.BL
{
    public class Language
    {
        private int languageId;
        private string languageName;
        private List<Country> countriesLangIsSpoken;//NEW
        private List<UserLanguages> usersKnowLanguage;//NEW

        public int LanguageId { get; set; }
        public string LanguageName { get; set; }

        public Language(int id, string name)
        {
            LanguageId = id;
            LanguageName = name;
        }

        public Language() { }


        
    }

}
