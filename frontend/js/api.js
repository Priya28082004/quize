// API base URL - change this to your server address
const API_BASE = 'https://cautious-cod-x5qx9v9wjw6rhvwgp-5000.app.github.dev/api';

async function apiFetch(path, options = {}) {
  const token = localStorage.getItem('qm_token');
  const headers = { 'Content-Type': 'application/json', ...(options.headers || {}) };
  if (token) headers['Authorization'] = `Bearer ${token}`;
  const res = await fetch(`${API_BASE}${path}`, { ...options, headers });
  if (res.status === 401) { logout(); window.location.href = 'index.html'; return null; }
  if (!res.ok) {
    let err = 'Request failed';
    try { const d = await res.json(); err = d.message || err; } catch {}
    throw new Error(err);
  }
  if (res.status === 204) return null;
  return res.json();
}

const api = {
  // Auth
  login: (dto) => apiFetch('/auth/login', { method: 'POST', body: JSON.stringify(dto) }),
  register: (dto) => apiFetch('/auth/register', { method: 'POST', body: JSON.stringify(dto) }),
  // Quizzes
  getQuizzes: () => apiFetch('/quizzes'),
  getQuiz: (id) => apiFetch(`/quizzes/${id}`),
  startAttempt: (id) => apiFetch(`/quizzes/${id}/attempt`, { method: 'POST' }),
  submitAttempt: (id, dto) => apiFetch(`/quizzes/attempts/${id}/submit`, { method: 'POST', body: JSON.stringify(dto) }),
  // Leaderboard
  getLeaderboard: () => apiFetch('/leaderboard'),
  // Admin
  adminGetQuizzes: () => apiFetch('/admin/quizzes'),
  adminCreateQuiz: (dto) => apiFetch('/admin/quizzes', { method: 'POST', body: JSON.stringify(dto) }),
  adminUpdateQuiz: (id, dto) => apiFetch(`/admin/quizzes/${id}`, { method: 'PUT', body: JSON.stringify(dto) }),
  adminToggleQuiz: (id) => apiFetch(`/admin/quizzes/${id}/toggle`, { method: 'PATCH' }),
  adminDeleteQuiz: (id) => apiFetch(`/admin/quizzes/${id}`, { method: 'DELETE' }),
  adminCreateQuestion: (dto) => apiFetch('/admin/questions', { method: 'POST', body: JSON.stringify(dto) }),
  adminDeleteQuestion: (id) => apiFetch(`/admin/questions/${id}`, { method: 'DELETE' }),
  adminGetAttempts: () => apiFetch('/admin/attempts'),
};
