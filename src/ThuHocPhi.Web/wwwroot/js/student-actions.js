(() => {
  const tbody = document.getElementById("registered-body");
  const token = localStorage.getItem("thuhocphi_token");
  const maHocKy = "HK1_2526";

  if (!tbody) return;

  function getSelectedCourses() {
    try {
      const raw = localStorage.getItem("selected_courses");
      if (!raw) return [];
      const parsed = JSON.parse(raw);
      return Array.isArray(parsed) ? parsed : [];
    } catch {
      return [];
    }
  }

  function saveSelectedCourses(courses) {
    localStorage.setItem("selected_courses", JSON.stringify(courses));
  }

  async function fetchJson(url, options) {
    try {
      const response = await fetch(url, {
        headers: {
          "Content-Type": "application/json",
          ...(token ? { Authorization: `Bearer ${token}` } : {})
        },
        credentials: "same-origin",
        ...options
      });
      if (!response.ok) return null;
      if (response.status === 204) return {};
      return await response.json();
    } catch {
      return null;
    }
  }

  async function syncFromApi() {
    const data = await fetchJson(`/api/dang-ky-hoc-phan?maHocKy=${encodeURIComponent(maHocKy)}`);
    if (!Array.isArray(data) || !data.length) return;

    const existing = getSelectedCourses();
    const scheduleMap = new Map(existing.map((item) => [item.code, item.schedule]));
    const merged = data.map((item) => ({
      code: item.maHocPhan,
      name: item.tenHocPhan,
      credits: item.soTinChi,
      schedule: scheduleMap.get(item.maHocPhan) || "Chua chon ca",
      status: "Da xac nhan"
    }));
    saveSelectedCourses(merged);
  }

  function renderCourses() {
    const courses = getSelectedCourses();
    tbody.innerHTML = "";

    if (!courses.length) {
      const row = document.createElement("tr");
      row.innerHTML = `<td colspan="5">Chua co hoc phan nao duoc dang ky.</td>`;
      tbody.appendChild(row);
      return;
    }

    courses.forEach((course, index) => {
      const row = document.createElement("tr");
      row.innerHTML = `
        <td>${course.code}</td>
        <td>${course.name}</td>
        <td>${course.credits}</td>
        <td>${course.schedule || "Chua chon ca"}</td>
        <td>
          <button class="btn secondary" type="button" data-edit="${index}">Sua</button>
          <button class="btn secondary" type="button" data-delete="${index}">Xoa</button>
        </td>
      `;
      tbody.appendChild(row);
    });
  }

  tbody.addEventListener("click", (event) => {
    const target = event.target;
    if (!(target instanceof HTMLElement)) return;

    if (target.matches("[data-delete]")) {
      const index = Number(target.getAttribute("data-delete"));
      const courses = getSelectedCourses();
      if (!Number.isInteger(index) || index < 0 || index >= courses.length) return;
      const removed = courses.splice(index, 1);
      saveSelectedCourses(courses);
      const code = removed[0]?.code;
      if (code) {
        fetchJson(`/api/dang-ky-hoc-phan/${encodeURIComponent(code)}?maHocKy=${encodeURIComponent(maHocKy)}`, {
          method: "DELETE"
        }).finally(renderCourses);
      } else {
        renderCourses();
      }
      return;
    }

    if (target.matches("[data-edit]")) {
      const index = Number(target.getAttribute("data-edit"));
      const courses = getSelectedCourses();
      if (!Number.isInteger(index) || index < 0 || index >= courses.length) return;
      const current = courses[index];
      const updated = window.prompt("Nhap lich hoc moi", current.schedule || "");
      if (updated === null) return;
      courses[index] = {
        ...current,
        schedule: updated.trim() || current.schedule
      };
      saveSelectedCourses(courses);
      renderCourses();
    }
  });

  syncFromApi().finally(renderCourses);
})();
