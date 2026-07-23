using ServerSideCountriesProject_MeravTomer.DAL;

namespace ServerSideCountriesProject_MeravTomer.BL
{
    public class Language
    {
        private int languageId;
        private string languageName;

        public Language()
        {
        }

        public Language(int languageId, string languageName)
        {
            LanguageId = languageId;
            LanguageName = languageName;
        }

        public int LanguageId
        {
            get => languageId;
            set => languageId = value;
        }

        public string LanguageName
        {
            get => languageName;
            set => languageName = value;
        }


        // =========================
        // Read
        // =========================

        public List<Language> ReadAllLanguages()
        {
            DBLanguageServices db = new DBLanguageServices();

            return db.ReadAllLanguages();
        }


        public Language ReadLanguageById(int languageId)
        {
            DBLanguageServices db = new DBLanguageServices();

            return db.ReadLanguageById(languageId);
        }


        public Language ReadLanguageByName(string languageName)
        {
            DBLanguageServices db = new DBLanguageServices(); 

            return db.ReadLanguageByName(languageName);
        }
    }
}


