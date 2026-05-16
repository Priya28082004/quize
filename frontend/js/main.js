// main.js — index.html logic
async function handleHeroStart() {
  if (!isLoggedIn()) { showAuthModal('register'); return; }
  window.location.href = 'quizzes.html';
}

async function loadStats() {
  try {
    const quizzes = await api.getQuizzes();
    const el = document.getElementById('statQuizzes');
    if (el) el.textContent = quizzes.length;
    const lb = await api.getLeaderboard();
    const pl = document.getElementById('statPlayers');
    if (pl) pl.textContent = new Set(lb.map(e => e.username)).size;
  } catch {}
}

document.addEventListener('DOMContentLoaded', loadStats);
