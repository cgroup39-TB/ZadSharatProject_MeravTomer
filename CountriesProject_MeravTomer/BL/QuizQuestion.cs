namespace ServerSideCountriesProject_MeravTomer.BL
{
    /// <summary>A single quiz question with its multiple-choice options. <see cref="CorrectIndex"/> is only meant to be used server-side for scoring.</summary>
    public class QuizQuestion
    {
        public int QuestionId { get; set; }
        public int QuizId { get; set; }
        public string Text { get; set; }
        public List<string> Options { get; set; }
        public int CorrectIndex { get; set; }
    }
}
