namespace ServerSideCountriesProject_MeravTomer.BL
{
    public class Language
    {
        private int languageId;
        private string name;


        public int LanguageId { get; set; }
        public string Name { get; set; }

        public Language(int id, string name)
        {
            LanguageId = id;
            Name = name;
        }

        public Language() { }
    }

}
