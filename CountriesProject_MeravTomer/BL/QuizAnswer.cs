namespace ServerSideCountriesProject_MeravTomer.BL
{
    /// <summary>One selected answer within a <see cref="QuizSubmission"/>: which question, and which option index the user picked.</summary>
    public class QuizAnswer
    {
        public int QuestionId { get; set; }
        public int SelectedIndex { get; set; }
    }
}
