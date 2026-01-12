(() => {
  const saveBtn = document.getElementById("btn-save-registration");
  const saveNote = document.getElementById("save-note");
  const tbody = document.getElementById("registered-body");

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

  function renderCourses() {
    if (!tbody) return;
    const courses = getSelectedCourses();
    tbody.innerHTML = "";

    if (!courses.length) {
      const row = document.createElement("tr");
      row.innerHTML = `<td colspan="5">Chưa có học phần nào được đăng ký.</td>`;
      tbody.appendChild(row);
      return;
    }

    courses.forEach((course) => {
      const row = document.createElement("tr");
      row.innerHTML = `
        <td>${course.code}</td>
        <td>${course.name}</td>
        <td>${course.credits}</td>
        <td>${course.schedule || "Chưa chọn ca"}</td>
        <td><span class="badge warn">${course.status || "Chờ xác nhận"}</span></td>
      `;
      tbody.appendChild(row);
    });
  }

  if (saveBtn) {
    saveBtn.addEventListener("click", () => {
      if (saveNote) {
        saveNote.textContent = "Đã lưu đăng ký tạm thời. Vui lòng kiểm tra lại trước khi xác nhận.";
      }
    });
  }

  renderCourses();
})();
