namespace ServerSideCountriesProject_MeravTomer.BL
{
    public class QuizResultDetail
    {
        public int QuestionId { get; set; }
        public int CorrectIndex { get; set; }
        public bool IsCorrect { get; set; }
    }

    public class QuizResult
    {
        public int QuizId { get; set; }
        public int Total { get; set; }
        public int Correct { get; set; }
        public List<QuizResultDetail> Details { get; set; }
    }
}
