// quizzes.js — quizzes.html logic
let allQuizzes = [];

const CATEGORY_ICONS = { General: '🌍', Science: '🔬', Mathematics: '📐', History: '📜', Technology: '💻', Sports: '⚽', Literature: '📚' };

function formatTime(s) {
  const m = Math.floor(s / 60), sec = s % 60;
  return m > 0 ? `${m}m ${sec}s` : `${sec}s`;
}

function renderQuizzes(list) {
  const grid = document.getElementById('quizGrid');
  if (!list.length) {
    grid.innerHTML = `<div class="empty-state"><div class="empty-icon">📭</div><p>No quizzes found.</p></div>`;
    return;
  }
  grid.innerHTML = list.map(q => `
    <div class="quiz-card" onclick="startQuiz(${q.id})" id="qcard-${q.id}">
      <div><span class="qc-category">${CATEGORY_ICONS[q.category] || '📝'} ${q.category}</span></div>
      <div class="qc-title">${q.title}</div>
      <div class="qc-desc">${q.description}</div>
      <div class="qc-meta">
        <span class="qc-meta-item">❓ ${q.questionCount} questions</span>
        <span class="qc-meta-item">⏱ ${formatTime(q.timeLimitSeconds)}</span>
      </div>
      <button class="btn btn-primary" style="margin-top:.5rem">Start Quiz →</button>
    </div>`).join('');
}

function filterQuizzes() {
  const q = document.getElementById('searchInput').value.toLowerCase();
  renderQuizzes(allQuizzes.filter(quiz => quiz.title.toLowerCase().includes(q) || quiz.category.toLowerCase().includes(q)));
}

async function startQuiz(id) {
  if (!isLoggedIn()) { window._afterLogin = () => startQuiz(id); showAuthModal('login'); return; }
  try {
    const attempt = await api.startAttempt(id);
    window.location.href = `quiz.html?quizId=${id}&attemptId=${attempt.attemptId}`;
  } catch (err) { alert('Could not start quiz: ' + err.message); }
}

async function init() {
  try {
    allQuizzes = await api.getQuizzes();
    renderQuizzes(allQuizzes);
  } catch (err) {
    document.getElementById('quizGrid').innerHTML =
      `<div class="empty-state"><div class="empty-icon">⚠️</div><p>Failed to load quizzes. Is the server running?</p></div>`;
  }
}

document.addEventListener('DOMContentLoaded', init);
