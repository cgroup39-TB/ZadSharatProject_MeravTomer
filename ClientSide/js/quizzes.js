/*
 * quizzes.js
 * Powers pages/quiz-list.html (choose a quiz) and pages/quiz-play.html (timed quiz).
 *
 * The quiz catalog comes from GET /api/Quizzes (DB-backed - see
 * QuizzesController on the server). The actual questions/answers always come
 * from Api.Quizzes.getQuestions() / .submit().
 */
$(function () {
    if ($("#quizCatalog").length) renderQuizCatalog();
    if ($("#quizPlayArea").length) initQuizPlay();
});

function renderQuizCatalog() {
    Auth.requireAuth();
    const $catalog = $("#quizCatalog");

    Api.Quizzes.getCatalog()
        .done(function (quizzes) {
            if (!quizzes.length) {
                $catalog.html('<p class="muted">No quizzes available yet.</p>');
                return;
            }
            quizzes.forEach(function (quiz) {
                $catalog.append(
                    '<div class="quiz-card">' +
                    '<h3>' + quiz.title + '</h3>' +
                    '<p>' + (quiz.description || "") + '</p>' +
                    '<a class="btn" href="quiz-play.html?quizId=' + quiz.id + '">Start Quiz</a>' +
                    '</div>'
                );
            });
        })
        .fail(Common.showError);
}

// ---------- Quiz play state ----------
let quizState = {
    quiz: null,
    currentIndex: 0,
    answers: {},       // questionId -> selectedIndex
    secondsLeft: 0,
    timerHandle: null
};

function initQuizPlay() {
    Auth.requireAuth();
    const quizId = Common.getQueryParams().quizId;
    if (!quizId) {
        Common.showAlert("No quiz selected.", "error");
        return;
    }

    Api.Quizzes.getQuestions(quizId)
        .done(function (quiz) {
            quizState.quiz = quiz;
            quizState.currentIndex = 0;
            quizState.answers = {};
            quizState.secondsLeft = quiz.durationSeconds;

            $("#quizTitle").text(quiz.title);
            renderQuestion();
            startTimer();
        })
        .fail(Common.showError);

    $("#nextBtn").on("click", goToNextQuestion);
    $("#submitQuizBtn").on("click", submitQuiz);
}

function startTimer() {
    updateTimerDisplay();
    quizState.timerHandle = setInterval(function () {
        quizState.secondsLeft--;
        updateTimerDisplay();
        if (quizState.secondsLeft <= 0) {
            clearInterval(quizState.timerHandle);
            submitQuiz();
        }
    }, 1000);
}

function updateTimerDisplay() {
    $("#timerDisplay").text(quizState.secondsLeft + "s");
}

function renderQuestion() {
    const question = quizState.quiz.questions[quizState.currentIndex];
    const total = quizState.quiz.questions.length;
    const isLast = quizState.currentIndex === total - 1;

    $("#questionProgress").text("Question " + (quizState.currentIndex + 1) + " of " + total);
    $("#questionText").text(question.text);

    const $options = $("#optionsList").empty();
    question.options.forEach(function (option, idx) {
        const checked = quizState.answers[question.id] === idx ? "checked" : "";
        $options.append(
            '<label class="option-row">' +
            '<input type="radio" name="option" value="' + idx + '" ' + checked + '> ' + option +
            '</label>'
        );
    });

    $("#nextBtn").toggle(!isLast);
    $("#submitQuizBtn").toggle(isLast);
}

function saveCurrentAnswer() {
    const question = quizState.quiz.questions[quizState.currentIndex];
    const selected = $("#optionsList input[name='option']:checked").val();
    if (selected !== undefined) {
        quizState.answers[question.id] = Number(selected);
    }
}

function goToNextQuestion() {
    saveCurrentAnswer();
    quizState.currentIndex++;
    renderQuestion();
}

function submitQuiz() {
    saveCurrentAnswer();
    clearInterval(quizState.timerHandle);

    const user = Auth.getCurrentUser();
    const answers = Object.keys(quizState.answers).map(function (questionId) {
        return { questionId: Number(questionId), selectedIndex: quizState.answers[questionId] };
    });

    Api.Quizzes.submit({ quizId: quizState.quiz.id, userId: user.id, answers: answers })
        .done(renderResults)
        .fail(Common.showError);
}

function renderResults(result) {
    $("#quizPlayArea").hide();
    $("#quizResults").show().html(
        '<h3>Your score: ' + result.correct + ' / ' + result.total + '</h3>' +
        '<a class="btn" href="quiz-list.html">Back to Quizzes</a>'
    );
}
