// ── Auth state ─────────────────────────────────────────────────────────────
function getUser() {
  const u = localStorage.getItem('qm_user');
  return u ? JSON.parse(u) : null;
}
function getToken() { return localStorage.getItem('qm_token'); }
function isLoggedIn() { return !!getToken(); }
function isAdmin() { const u = getUser(); return u && u.role === 'Admin'; }

function saveAuth(data) {
  localStorage.setItem('qm_token', data.token);
  localStorage.setItem('qm_user', JSON.stringify({ username: data.username, role: data.role, userId: data.userId }));
}
function logout() {
  localStorage.removeItem('qm_token');
  localStorage.removeItem('qm_user');
  updateNavAuth();
}

// ── Nav UI ─────────────────────────────────────────────────────────────────
function updateNavAuth() {
  const user = getUser();
  const navUser = document.getElementById('navUser');
  const navLoginBtn = document.getElementById('navLoginBtn');
  const navRegisterBtn = document.getElementById('navRegisterBtn');
  const navUsername = document.getElementById('navUsername');
  const navAdminLink = document.getElementById('navAdminLink');

  if (user) {
    if (navUser) navUser.style.display = 'flex';
    if (navLoginBtn) navLoginBtn.style.display = 'none';
    if (navRegisterBtn) navRegisterBtn.style.display = 'none';
    if (navUsername) navUsername.textContent = user.username;
    if (navAdminLink) navAdminLink.style.display = user.role === 'Admin' ? 'inline' : 'none';
  } else {
    if (navUser) navUser.style.display = 'none';
    if (navLoginBtn) navLoginBtn.style.display = '';
    if (navRegisterBtn) navRegisterBtn.style.display = '';
    if (navAdminLink) navAdminLink.style.display = 'none';
  }
}

// ── Modal ──────────────────────────────────────────────────────────────────
function showAuthModal(form) {
  const modal = document.getElementById('authModal');
  if (modal) { modal.classList.add('open'); switchAuthForm(form); }
}
function closeAuthModal() {
  const modal = document.getElementById('authModal');
  if (modal) modal.classList.remove('open');
}
function switchAuthForm(which) {
  const lf = document.getElementById('loginForm');
  const rf = document.getElementById('registerForm');
  if (lf) lf.style.display = which === 'login' ? '' : 'none';
  if (rf) rf.style.display = which === 'register' ? '' : 'none';
}

// ── Login ──────────────────────────────────────────────────────────────────
async function handleLogin(e) {
  e.preventDefault();
  const btn = document.getElementById('loginSubmitBtn');
  const errEl = document.getElementById('loginError');
  errEl.textContent = '';
  btn.textContent = 'Signing in...'; btn.disabled = true;
  try {
    const data = await api.login({
      email: document.getElementById('loginEmail').value,
      password: document.getElementById('loginPassword').value
    });
    saveAuth(data);
    closeAuthModal();
    updateNavAuth();
    if (window._afterLogin) window._afterLogin();
    else window.location.reload();
  } catch (err) {
    errEl.textContent = err.message;
  } finally {
    btn.textContent = 'Sign In'; btn.disabled = false;
  }
}

// ── Register ───────────────────────────────────────────────────────────────
async function handleRegister(e) {
  e.preventDefault();
  const btn = document.getElementById('registerSubmitBtn');
  const errEl = document.getElementById('registerError');
  errEl.textContent = '';
  btn.textContent = 'Creating...'; btn.disabled = true;
  try {
    const data = await api.register({
      username: document.getElementById('regUsername').value,
      email: document.getElementById('regEmail').value,
      password: document.getElementById('regPassword').value
    });
    saveAuth(data);
    closeAuthModal();
    updateNavAuth();
    if (window._afterLogin) window._afterLogin();
    else window.location.reload();
  } catch (err) {
    errEl.textContent = err.message;
  } finally {
    btn.textContent = 'Create Account'; btn.disabled = false;
  }
}

// Close modal on outside click
document.addEventListener('click', e => {
  const modal = document.getElementById('authModal');
  if (modal && e.target === modal) closeAuthModal();
});

// Init nav on page load
document.addEventListener('DOMContentLoaded', updateNavAuth);

// ── Particle background ────────────────────────────────────────────────────
document.addEventListener('DOMContentLoaded', () => {
  const container = document.getElementById('bgParticles');
  if (!container) return;
  for (let i = 0; i < 30; i++) {
    const dot = document.createElement('div');
    const size = Math.random() * 4 + 1;
    dot.style.cssText = `position:absolute;width:${size}px;height:${size}px;border-radius:50%;
      background:rgba(${Math.random() > 0.5 ? '124,58,237' : '59,130,246'},${Math.random() * 0.4 + 0.1});
      left:${Math.random() * 100}%;top:${Math.random() * 100}%;
      animation:float${Math.random() > 0.5 ? 1 : 2} ${Math.random() * 8 + 6}s ease-in-out infinite;
      animation-delay:${Math.random() * 4}s`;
    container.appendChild(dot);
  }
});
