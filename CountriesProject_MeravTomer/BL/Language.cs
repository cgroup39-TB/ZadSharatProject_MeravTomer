namespace ServerSideCountriesProject_MeravTomer.BL
{
    public class Language
    {
        private int languageId;
        private string languageName;


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
