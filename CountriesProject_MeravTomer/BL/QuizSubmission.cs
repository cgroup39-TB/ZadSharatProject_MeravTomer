namespace ServerSideCountriesProject_MeravTomer.BL
{
    /// <summary>Request body posted by the client when a user finishes a quiz - which quiz, which user, and their selected answers.</summary>
    public class QuizSubmission
    {
        public int QuizId { get; set; }
        public int UserId { get; set; }
        public List<QuizAnswer> Answers { get; set; }
    }
}
