using ServerSideCountriesProject_MeravTomer.DAL;

namespace ServerSideCountriesProject_MeravTomer.BL
{
    /// <summary>Business-logic representation of a language (id + name).</summary>
    public class Language
    {
        private int languageId;
        private string languageName;

        /// <summary>Creates an empty language instance (used by model binding/deserialization).</summary>
        public Language()
        {
        }

        /// <summary>Creates a fully-populated language.</summary>
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

        /// <summary>Returns every language in the database.</summary>
        public List<Language> ReadAllLanguages()
        {
            DBLanguageServices db = new DBLanguageServices();

            return db.ReadAllLanguages();
        }


        /// <summary>Returns the language matching <paramref name="languageId"/>, or null if none exists.</summary>
        public Language ReadLanguageById(int languageId)
        {
            DBLanguageServices db = new DBLanguageServices();

            return db.ReadLanguageById(languageId);
        }


        /// <summary>Returns the language matching <paramref name="languageName"/> (exact match), or null if none exists.</summary>
        public Language ReadLanguageByName(string languageName)
        {
            DBLanguageServices db = new DBLanguageServices(); 

            return db.ReadLanguageByName(languageName);
        }
    }
}


