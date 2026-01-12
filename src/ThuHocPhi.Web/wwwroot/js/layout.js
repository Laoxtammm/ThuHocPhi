(() => {
  const token = localStorage.getItem("thuhocphi_token");
  const registerLink = document.querySelector(".nav-register");
  const loginLink = document.querySelector(".nav-login");
  const logoutBtn = document.querySelector(".nav-logout");
  const extraLinks = document.querySelectorAll(".nav-extra");
  const dateEl = document.getElementById("today-date");
  const body = document.body;

  const isAuthPage = body.classList.contains("auth-page");

  if (dateEl) {
    const now = new Date();
    const day = String(now.getDate()).padStart(2, "0");
    const month = String(now.getMonth() + 1).padStart(2, "0");
    const year = now.getFullYear();
    dateEl.textContent = `Ngày ${day}/${month}/${year}`;
  }

  if (isAuthPage) {
    if (logoutBtn) logoutBtn.classList.add("hidden");
    if (registerLink) registerLink.classList.add("hidden");
    if (loginLink) loginLink.classList.add("hidden");
    extraLinks.forEach((link) => link.classList.add("hidden"));
    return;
  }

  if (token) {
    if (registerLink) registerLink.classList.add("hidden");
    if (loginLink) loginLink.classList.add("hidden");
    if (logoutBtn) {
      logoutBtn.classList.remove("hidden");
      logoutBtn.addEventListener("click", () => {
        localStorage.removeItem("thuhocphi_token");
        document.cookie = "thuhocphi_token=; path=/; expires=Thu, 01 Jan 1970 00:00:00 GMT";
        window.location.href = "/";
      });
    }
  } else {
    if (logoutBtn) logoutBtn.classList.add("hidden");
  }
})();
