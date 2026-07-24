using Microsoft.AspNetCore.Mvc;
using ServerSideCountriesProject_MeravTomer.BL;

namespace ServerSideCountriesProject_MeravTomer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuizzesController : ControllerBase
    {
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
