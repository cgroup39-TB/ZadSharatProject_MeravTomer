namespace ServerSideCountriesProject_MeravTomer.BL
{
    public class QuizSubmission
    {
        public int QuizId { get; set; }
        public int UserId { get; set; }
        public List<QuizAnswer> Answers { get; set; }
    }
}
