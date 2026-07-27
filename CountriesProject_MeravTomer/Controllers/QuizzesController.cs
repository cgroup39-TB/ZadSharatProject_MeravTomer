using Microsoft.AspNetCore.Mvc;
using ServerSideCountriesProject_MeravTomer.BL;

namespace ServerSideCountriesProject_MeravTomer.Controllers
{
    /// <summary>REST endpoints for the quiz catalog, taking a quiz and submitting/scoring answers.</summary>
    [Route("api/[controller]")]
    [ApiController]
    public class QuizzesController : ControllerBase
    {
        /// <summary>Returns the quiz catalog (id/title/description only, no questions).</summary>
        // GET: api/Quizzes
        [HttpGet]
        public IActionResult GetCatalog()
        {
            List<Quiz> quizzes = Quiz.ReadCatalog();

            var catalog = quizzes.Select(q => new
            {
                id = q.QuizId,
                title = q.Title,
                description = q.Description
            });

            return Ok(catalog);
        }


        /// <summary>
        /// Returns the quiz with its questions and options, or 404 if it doesn't exist. Each
        /// question's CorrectIndex is deliberately stripped from the response so the client
        /// can't read the answer before submitting.
        /// </summary>
        // GET: api/Quizzes/5/questions
        [HttpGet("{quizId}/questions")]
        public IActionResult GetQuestions(int quizId)
        {
            Quiz quiz = Quiz.ReadWithQuestions(quizId);

            if (quiz == null)
            {
                return NotFound("Quiz not found");
            }

            // CorrectIndex is intentionally left out of the response.
            var safeQuestions = quiz.Questions.Select(q => new
            {
                id = q.QuestionId,
                text = q.Text,
                options = q.Options
            });

            return Ok(new
            {
                id = quiz.QuizId,
                title = quiz.Title,
                durationSeconds = quiz.DurationSeconds,
                questions = safeQuestions
            });
        }


        /// <summary>Grades a quiz submission server-side and returns the score/details; 404 if the submission's quiz doesn't exist.</summary>
        // POST: api/Quizzes/submit
        [HttpPost("submit")]
        public IActionResult Submit(
            [FromBody] QuizSubmission submission)
        {
            QuizResult result = Quiz.Score(submission);

            if (result == null)
            {
                return NotFound("Quiz not found");
            }

            return Ok(result);
        }
    }
}
