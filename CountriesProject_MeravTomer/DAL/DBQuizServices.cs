using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using ServerSideCountriesProject_MeravTomer.BL;

namespace ServerSideCountriesProject_MeravTomer.DAL
{
    /// <summary>
    /// Data access layer for the quiz catalog, quiz questions, and their answer options.
    /// Read methods that return data to callers outside the server (catalog, single quiz) never
    /// include the correct answer; only <see cref="ReadQuestionsByQuizId"/> exposes CorrectIndex,
    /// and it is intended for server-side scoring use only.
    /// </summary>
    public class DBQuizServices
    {
        /// <summary>
        /// Creates a new instance of the service. Holds no state; a fresh DB connection is opened per call.
        /// </summary>
        public DBQuizServices()
        {
        }


        //--------------------------------------------------------------------------------------------------
        // Create DB connection
        //--------------------------------------------------------------------------------------------------
        /// <summary>
        /// Opens and returns a new <see cref="SqlConnection"/> using the named connection string
        /// read from appsettings.json. The caller is responsible for closing the connection.
        /// </summary>
        public SqlConnection connect(String conString)
        {
            IConfigurationRoot configuration =
                new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            string cStr =
                configuration.GetConnectionString(conString);

            SqlConnection connectionToDb =
                new SqlConnection(cStr);

            connectionToDb.Open();

            return connectionToDb;
        }


        //---------------------------------------------------------------------------------
        // Create SqlCommand with Stored Procedure
        //---------------------------------------------------------------------------------
        private SqlCommand CreateCommandWithStoredProcedureGeneral(
            String spName,
            SqlConnection con,
            Dictionary<string, object> paramDic)
        {
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = con;
            cmd.CommandText = spName;
            cmd.CommandTimeout = 10;
            cmd.CommandType = CommandType.StoredProcedure;

            if (paramDic != null)
            {
                foreach (KeyValuePair<string, object> param in paramDic)
                {
                    cmd.Parameters.AddWithValue(
                        param.Key,
                        param.Value ?? DBNull.Value);
                }
            }

            return cmd;
        }


        //--------------------------------------------------------------------------------------------------
        // Read quiz catalog (no correct answers)
        //--------------------------------------------------------------------------------------------------
        /// <summary>
        /// Returns the catalog of all quizzes (id, title, description, duration) without their questions
        /// or answers.
        /// </summary>
        public List<Quiz> ReadQuizCatalog()
        {
            List<Quiz> quizzes = new List<Quiz>();

            SqlConnection con;
            SqlCommand cmd;

            try
            {
                con = connect("myProjDB");
            }
            catch (Exception ex)
            {
                throw ex;
            }

            cmd = CreateCommandWithStoredProcedureGeneral(
                "spReadQuizCatalog_3MD_TB",
                con,
                null);

            try
            {
                SqlDataReader dataReader =
                    cmd.ExecuteReader();

                while (dataReader.Read())
                {
                    quizzes.Add(ReadQuizFromRow(dataReader));
                }

                return quizzes;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }
        }


        //--------------------------------------------------------------------------------------------------
        // Read a single quiz by id (no correct answers)
        //--------------------------------------------------------------------------------------------------
        /// <summary>
        /// Returns the quiz metadata matching <paramref name="quizId"/> (without questions or answers),
        /// or null if no such quiz exists.
        /// </summary>
        public Quiz ReadQuizById(int quizId)
        {
            SqlConnection con;
            SqlCommand cmd;

            try
            {
                con = connect("myProjDB");
            }
            catch (Exception ex)
            {
                throw ex;
            }

            Dictionary<string, object> paramDic =
                new Dictionary<string, object>();

            paramDic.Add("@QuizId", quizId);

            cmd = CreateCommandWithStoredProcedureGeneral(
                "spReadQuizById_3MD_TB",
                con,
                paramDic);

            try
            {
                SqlDataReader dataReader =
                    cmd.ExecuteReader();

                if (dataReader.Read())
                {
                    return ReadQuizFromRow(dataReader);
                }

                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }
        }


        //--------------------------------------------------------------------------------------------------
        // Read all questions of a quiz, including CorrectIndex (server-side only)
        //--------------------------------------------------------------------------------------------------
        /// <summary>
        /// Returns every question for the given quiz, including its four options and the
        /// <c>CorrectIndex</c> of the right answer. CorrectIndex is meant for server-side scoring only
        /// and must not be forwarded to the client as-is.
        /// </summary>
        public List<QuizQuestion> ReadQuestionsByQuizId(int quizId)
        {
            List<QuizQuestion> questions = new List<QuizQuestion>();

            SqlConnection con;
            SqlCommand cmd;

            try
            {
                con = connect("myProjDB");
            }
            catch (Exception ex)
            {
                throw ex;
            }

            Dictionary<string, object> paramDic =
                new Dictionary<string, object>();

            paramDic.Add("@QuizId", quizId);

            cmd = CreateCommandWithStoredProcedureGeneral(
                "spReadQuestionsByQuizId_3MD_TB",
                con,
                paramDic);

            try
            {
                SqlDataReader dataReader =
                    cmd.ExecuteReader();

                while (dataReader.Read())
                {
                    QuizQuestion question = new QuizQuestion();

                    question.QuestionId =
                        Convert.ToInt32(dataReader["QuestionId"]);

                    question.QuizId =
                        Convert.ToInt32(dataReader["QuizId"]);

                    question.Text =
                        dataReader["QuestionText"].ToString();

                    question.Options = new List<string>
                    {
                        dataReader["OptionA"].ToString(),
                        dataReader["OptionB"].ToString(),
                        dataReader["OptionC"].ToString(),
                        dataReader["OptionD"].ToString()
                    };

                    question.CorrectIndex =
                        Convert.ToInt32(dataReader["CorrectIndex"]);

                    questions.Add(question);
                }

                return questions;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }
        }


        //--------------------------------------------------------------------------------------------------
        // Shared row mapper for the Quizzes table
        //--------------------------------------------------------------------------------------------------
        private Quiz ReadQuizFromRow(SqlDataReader dataReader)
        {
            Quiz quiz = new Quiz();

            quiz.QuizId =
                Convert.ToInt32(dataReader["QuizId"]);

            quiz.Title =
                dataReader["Title"].ToString();

            quiz.Description =
                dataReader["Description"].ToString();

            quiz.DurationSeconds =
                Convert.ToInt32(dataReader["DurationSeconds"]);

            return quiz;
        }
    }
}
