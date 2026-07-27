namespace ServerSideCountriesProject_MeravTomer.BL
{
    /// <summary>Per-question grading detail returned as part of a <see cref="QuizResult"/>.</summary>
    public class QuizResultDetail
    {
        public int QuestionId { get; set; }
        public int CorrectIndex { get; set; }
        public bool IsCorrect { get; set; }
    }

    /// <summary>Overall score for a graded quiz submission, as produced by <see cref="Quiz.Score"/>.</summary>
    public class QuizResult
    {
        public int QuizId { get; set; }
        public int Total { get; set; }
        public int Correct { get; set; }
        public List<QuizResultDetail> Details { get; set; }
    }
}
