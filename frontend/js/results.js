// results.js — results.html logic
function formatSeconds(secs) {
  if (secs < 60) return `${Math.floor(secs)}s`;
  return `${Math.floor(secs / 60)}m ${Math.floor(secs % 60)}s`;
}

async function init() {
  const raw = sessionStorage.getItem('qm_result');
  if (!raw) { window.location.href = 'quizzes.html'; return; }
  sessionStorage.removeItem('qm_result');
  const result = JSON.parse(raw);

  document.getElementById('resultsLoading').style.display = 'none';
  document.getElementById('resultsContent').style.display = '';

  const pct = result.percentage;
  document.getElementById('scorePct').textContent = pct + '%';
  document.getElementById('ssCorrect').textContent = result.correctAnswers;
  document.getElementById('ssTotal').textContent = result.totalQuestions;

  const timeTaken = result.timeTaken;
  let totalSecs = 0;
  if (typeof timeTaken === 'string') {
    const parts = timeTaken.split(':');
    totalSecs = (+parts[0]) * 3600 + (+parts[1]) * 60 + parseFloat(parts[2]);
  }
  document.getElementById('ssTime').textContent = formatSeconds(totalSecs);

  const emoji = pct >= 80 ? '🏆' : pct >= 60 ? '🎉' : pct >= 40 ? '👍' : '💪';
  const title = pct >= 80 ? 'Excellent!' : pct >= 60 ? 'Great Job!' : pct >= 40 ? 'Good Effort!' : 'Keep Practicing!';
  document.getElementById('scoreEmoji').textContent = emoji;
  document.getElementById('scoreTitle').textContent = title;

  // Animate arc
  setTimeout(() => {
    const arc = document.getElementById('scoreArc');
    const circumference = 339.3;
    arc.style.strokeDashoffset = circumference - (pct / 100) * circumference;
  }, 100);

  // Render review
  const list = document.getElementById('reviewList');
  list.innerHTML = result.questionResults.map((qr, i) => `
    <div class="review-item ${qr.isCorrect ? 'correct' : 'wrong'}">
      <div class="review-q">${i + 1}. ${qr.questionText}</div>
      <div class="review-options">
        ${qr.options.map(o => {
          let cls = '';
          if (o.id === qr.correctOptionId) cls = 'correct-ans';
          else if (o.id === qr.selectedOptionId && !qr.isCorrect) cls = 'wrong-ans';
          return `<div class="review-opt ${cls}">${o.text}${o.id === qr.correctOptionId ? ' ✓' : ''}${o.id === qr.selectedOptionId && !qr.isCorrect ? ' ✗' : ''}</div>`;
        }).join('')}
      </div>
    </div>`).join('');

  // Update nav
  const user = getUser();
  if (user) {
    const nu = document.getElementById('navUsername');
    const nv = document.getElementById('navUser');
    if (nu) nu.textContent = user.username;
    if (nv) nv.style.display = 'flex';
  }
}

document.addEventListener('DOMContentLoaded', init);
