using ServerSideCountriesProject_MeravTomer.DAL;

namespace ServerSideCountriesProject_MeravTomer.BL
{
    /// <summary>
    /// A quiz definition with its questions. Also hosts the static scoring logic
    /// for grading a submitted <see cref="QuizSubmission"/>.
    /// </summary>
    public class Quiz
    {
        public int QuizId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int DurationSeconds { get; set; }
        public List<QuizQuestion> Questions { get; set; }


        // =========================
        // Read
        // =========================

        /// <summary>Returns every quiz (without its questions) - used for the quiz list/catalog view.</summary>
        public static List<Quiz> ReadCatalog()
        {
            DBQuizServices db = new DBQuizServices();

            return db.ReadQuizCatalog();
        }


        /// <summary>Returns the quiz matching <paramref name="quizId"/> together with its questions, or null if no such quiz exists.</summary>
        public static Quiz ReadWithQuestions(int quizId)
        {
            DBQuizServices db = new DBQuizServices();

            Quiz quiz = db.ReadQuizById(quizId);

            if (quiz == null)
            {
                return null;
            }

            quiz.Questions = db.ReadQuestionsByQuizId(quizId);

            return quiz;
        }


        // =========================
        // Scoring
        // =========================

        /// <summary>
        /// Grades a submission against the correct answers stored server-side. Returns null if the
        /// submission is null or refers to a non-existent quiz. Any question left unanswered (or with
        /// no matching answer in the submission) is simply scored as incorrect rather than throwing.
        /// </summary>
        public static QuizResult Score(QuizSubmission submission)
        {
            if (submission == null)
            {
                return null;
            }

            Quiz quiz = ReadWithQuestions(submission.QuizId);

            if (quiz == null)
            {
                return null;
            }

            int correct = 0;
            List<QuizResultDetail> details = new List<QuizResultDetail>();

            foreach (QuizQuestion question in quiz.Questions)
            {
                QuizAnswer answer =
                    submission.Answers?.FirstOrDefault(
                        a => a.QuestionId == question.QuestionId);

                bool isCorrect =
                    answer != null &&
                    answer.SelectedIndex == question.CorrectIndex;

                if (isCorrect)
                {
                    correct++;
                }

                details.Add(new QuizResultDetail
                {
                    QuestionId = question.QuestionId,
                    CorrectIndex = question.CorrectIndex,
                    IsCorrect = isCorrect
                });
            }

            return new QuizResult
            {
                QuizId = quiz.QuizId,
                Total = quiz.Questions.Count,
                Correct = correct,
                Details = details
            };
        }
    }
}
