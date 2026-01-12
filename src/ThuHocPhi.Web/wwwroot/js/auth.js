(() => {
  const form = document.getElementById("login-form");
  if (!form) return;

  const note = document.getElementById("login-note");

  form.addEventListener("submit", async (event) => {
    event.preventDefault();
    const username = form.username.value.trim().toLowerCase();
    const password = form.password.value.trim().toLowerCase();

    if (note) note.textContent = "Đang đăng nhập...";

    try {
      const response = await fetch("/api/auth/login", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ username, password }),
      });

      if (!response.ok) {
        throw new Error("Đăng nhập thất bại");
      }

      const data = await response.json();
      localStorage.setItem("thuhocphi_token", data.accessToken);
      document.cookie = `thuhocphi_token=${data.accessToken}; path=/`;

      const roles = (data.user?.roles || []).map((role) => role.toLowerCase());
      const isAdmin = roles.includes("administrator");
      const isAccounting = roles.includes("phongtaichinh");
      const isEducation = roles.includes("phongdaotao");
      const isStudent = roles.includes("sinhvien");

      if (isAdmin) {
        window.location.href = "/Admin";
      } else if (isAccounting) {
        window.location.href = "/Accounting";
      } else if (isEducation) {
        window.location.href = "/Education";
      } else if (isStudent) {
        window.location.href = "/Student";
      } else {
        window.location.href = "/";
      }
    } catch (error) {
      if (note) note.textContent = error.message;
    }
  });
})();
