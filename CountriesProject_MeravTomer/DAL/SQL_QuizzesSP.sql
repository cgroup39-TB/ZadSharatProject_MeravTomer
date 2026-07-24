-- =============================================
-- Author:		<Tomer,Merav>
-- Create date: <24.7.26>
-- Description:	<Read Quiz Catalog (no correct answers)>
-- =============================================
CREATE PROCEDURE spReadQuizCatalog_3MD_TB
AS
BEGIN

    SELECT
        QuizId,
        Title,
        [Description],
        DurationSeconds
    FROM Quizzes;

END
GO


-- =============================================
-- Author:		<Tomer,Merav>
-- Create date: <24.7.26>
-- Description:	<Read Quiz By Id (no correct answers)>
-- =============================================
CREATE PROCEDURE spReadQuizById_3MD_TB
    @QuizId INT
AS
BEGIN

    SELECT
        QuizId,
        Title,
        [Description],
        DurationSeconds
    FROM Quizzes
    WHERE QuizId = @QuizId;

END
GO


-- =============================================
-- Author:		<Tomer,Merav>
-- Create date: <24.7.26>
-- Description:	<Read Questions By Quiz Id (includes CorrectIndex, server-side only)>
-- =============================================
CREATE PROCEDURE spReadQuestionsByQuizId_3MD_TB
    @QuizId INT
AS
BEGIN

    SELECT
        QuestionId,
        QuizId,
        QuestionText,
        OptionA,
        OptionB,
        OptionC,
        OptionD,
        CorrectIndex
    FROM QuizQuestions
    WHERE QuizId = @QuizId
    ORDER BY QuestionId;

END
GO
