// quiz.js — active quiz session
const params = new URLSearchParams(location.search);
const quizId = parseInt(params.get('quizId'));
const attemptId = parseInt(params.get('attemptId'));

let quiz = null, currentIdx = 0, answers = {}, timerInterval = null, timeLeft = 0;
const LETTERS = ['A','B','C','D'];

function confirmExit() {
  return confirm('Exit quiz? Your progress will be lost.');
}

function updateProgress() {
  const pct = ((currentIdx + 1) / quiz.questions.length) * 100;
  document.getElementById('progressFill').style.width = pct + '%';
  document.getElementById('questionCounter').textContent = `${currentIdx + 1} / ${quiz.questions.length}`;
  document.getElementById('questionNum').textContent = `Question ${currentIdx + 1}`;
  document.getElementById('questionPts').textContent = `${quiz.questions[currentIdx].points} pt${quiz.questions[currentIdx].points > 1 ? 's' : ''}`;
}

function renderQuestion() {
  const q = quiz.questions[currentIdx];
  document.getElementById('questionText').textContent = q.text;
  const grid = document.getElementById('optionsGrid');
  grid.innerHTML = q.options.map((o, i) => `
    <button class="option-btn ${answers[q.id] === o.id ? 'selected' : ''}"
      onclick="selectOption(${q.id}, ${o.id}, this)">
      <span class="option-letter">${LETTERS[i]}</span>
      <span>${o.text}</span>
    </button>`).join('');
  document.getElementById('prevBtn').disabled = currentIdx === 0;
  document.getElementById('nextBtn').textContent = currentIdx === quiz.questions.length - 1 ? 'Review →' : 'Next →';
  updateProgress();
  renderPalette();
}

function selectOption(questionId, optionId, btn) {
  answers[questionId] = optionId;
  document.querySelectorAll('.option-btn').forEach(b => b.classList.remove('selected'));
  btn.classList.add('selected');
  renderPalette();
}

function nextQuestion() {
  if (currentIdx < quiz.questions.length - 1) { currentIdx++; renderQuestion(); }
  else confirmSubmit();
}
function prevQuestion() {
  if (currentIdx > 0) { currentIdx--; renderQuestion(); }
}

function renderPalette() {
  const grid = document.getElementById('paletteGrid');
  grid.innerHTML = quiz.questions.map((q, i) => `
    <button class="palette-btn ${answers[q.id] ? 'answered' : ''} ${i === currentIdx ? 'current' : ''}"
      onclick="goToQuestion(${i})">${i + 1}</button>`).join('');
  const answered = Object.keys(answers).length;
  document.getElementById('submitQuizBtn').textContent = `Submit Quiz ✓ (${answered}/${quiz.questions.length})`;
}

function goToQuestion(idx) { currentIdx = idx; renderQuestion(); }

// Timer
function startTimer() {
  timeLeft = quiz.timeLimitSeconds;
  timerInterval = setInterval(() => {
    timeLeft--;
    const m = Math.floor(timeLeft / 60).toString().padStart(2, '0');
    const s = (timeLeft % 60).toString().padStart(2, '0');
    const el = document.getElementById('timerDisplay');
    if (el) el.textContent = `${m}:${s}`;
    const box = document.getElementById('timerBox');
    if (timeLeft <= 30 && box) box.classList.add('urgent');
    if (timeLeft <= 0) { clearInterval(timerInterval); submitQuiz(true); }
  }, 1000);
}

// Submit
function confirmSubmit() {
  const answered = Object.keys(answers).length;
  const total = quiz.questions.length;
  document.getElementById('submitSummary').textContent =
    `You answered ${answered} of ${total} question${total > 1 ? 's' : ''}. Unanswered questions will be marked wrong.`;
  document.getElementById('submitModal').classList.add('open');
}
function closeSubmitModal() { document.getElementById('submitModal').classList.remove('open'); }

async function submitQuiz(autoSubmit = false) {
  if (!autoSubmit) closeSubmitModal();
  clearInterval(timerInterval);
  const submitData = { answers: quiz.questions.map(q => ({ questionId: q.id, selectedOptionId: answers[q.id] || null })) };
  try {
    const result = await api.submitAttempt(attemptId, submitData);
    sessionStorage.setItem('qm_result', JSON.stringify(result));
    window.location.href = 'results.html';
  } catch (err) { alert('Failed to submit: ' + err.message); }
}

async function init() {
  if (!isLoggedIn()) { window.location.href = 'quizzes.html'; return; }
  if (!quizId || !attemptId) { window.location.href = 'quizzes.html'; return; }
  try {
    quiz = await api.getQuiz(quizId);
    document.getElementById('quizTitleBar').textContent = quiz.title;
    document.getElementById('quizLoading').style.display = 'none';
    document.getElementById('questionArea').style.display = 'flex';
    renderQuestion();
    startTimer();
  } catch (err) {
    document.getElementById('quizLoading').innerHTML = `<p>⚠️ Failed to load quiz: ${err.message}</p>`;
  }
}

document.addEventListener('DOMContentLoaded', init);
