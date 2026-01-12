(() => {
  const note = document.getElementById("course-note");
  const table = document.querySelector("[data-course-table]");
  const tbody = table?.querySelector("tbody");
  const apiUrl = table?.getAttribute("data-course-api") || "/api/hoc-phan";
  const maHocKy = table?.getAttribute("data-ma-hoc-ky") || "HK1_2526";
  const modal = document.getElementById("course-modal");
  const modalName = document.getElementById("modal-course-name");
  const modalSchedules = document.getElementById("modal-schedules");
  const modalCancel = document.getElementById("modal-cancel");
  const modalConfirm = document.getElementById("modal-confirm");
  const token = localStorage.getItem("thuhocphi_token");

  if (!table || !modal || !tbody) return;

  const scheduleMap = {
    IT101: ["T2 - T4 (7:00 - 9:30)", "T3 - T5 (9:30 - 12:00)"],
    MA201: ["T2 - T4 (13:30 - 16:00)", "T5 (7:00 - 11:30)"],
    CN301: ["T6 (7:00 - 11:30)", "T7 (7:00 - 11:30)"]
  };

  let pendingCourse = null;
  let selectedSchedule = "";

  function showMessage(message) {
    if (note) note.textContent = message;
  }

  function getCompletedCourses() {
    try {
      const raw = localStorage.getItem("completed_courses");
      if (!raw) return ["IT101"];
      const parsed = JSON.parse(raw);
      return Array.isArray(parsed) ? parsed : ["IT101"];
    } catch {
      return ["IT101"];
    }
  }

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

  function parseMeta(text) {
    const source = String(text || "");
    const pick = (label) => {
      const regex = new RegExp(`${label}\\s*[:=]\\s*([^;]+)`, "i");
      const match = source.match(regex);
      return match ? match[1].trim() : "";
    };
    const prereq = pick("DieuKien") || pick("TienQuyet");
    const major = pick("ChuyenNganh");
    const className = pick("Lop");
    const noteText = pick("GhiChu") || source;
    return { prereq, major, className, noteText };
  }

  function renderCourses(courses) {
    tbody.innerHTML = "";

    if (!courses.length) {
      const row = document.createElement("tr");
      row.innerHTML = `<td colspan="5">Khong co hoc phan dang mo.</td>`;
      tbody.appendChild(row);
      return;
    }

    courses.forEach((course) => {
      const meta = parseMeta(course.moTa);
      const prereq = meta.prereq;
      let condition = "Khong";
      if (prereq) {
        condition = `Da hoc ${prereq}`;
      } else if ((course.loaiHocPhan || "").toLowerCase().includes("dieu")) {
        condition = "Co dieu kien";
      } else if ((course.loaiHocPhan || "").toLowerCase().includes("tu")) {
        condition = "Tu chon";
      }

      const row = document.createElement("tr");
      row.innerHTML = `
        <td>${course.maHocPhan}</td>
        <td>${course.tenHocPhan}</td>
        <td>${course.soTinChi}</td>
        <td>${condition}</td>
        <td>
          <button class="btn" type="button" data-add-course="${course.maHocPhan}" data-name="${course.tenHocPhan}" data-credits="${course.soTinChi}"${prereq ? ` data-prereq="${prereq}"` : ""}>Them</button>
        </td>
      `;
      tbody.appendChild(row);
    });
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
      const contentType = response.headers.get("content-type") || "";
      if (!contentType.includes("application/json")) return {};
      return await response.json();
    } catch {
      return null;
    }
  }

  async function loadCourses() {
    const data = await fetchJson(apiUrl);
    renderCourses(Array.isArray(data) ? data : []);
  }

  function openModal(course) {
    pendingCourse = course;
    selectedSchedule = "";
    if (modalName) {
      modalName.textContent = `${course.code} - ${course.name} (${course.credits} TC)`;
    }

    modalSchedules.innerHTML = "";
    const schedules = scheduleMap[course.code] || ["T2 - T4 (7:00 - 9:30)", "T3 - T5 (13:30 - 16:00)"];
    schedules.forEach((schedule, index) => {
      const id = `schedule-${course.code}-${index}`;
      const wrapper = document.createElement("label");
      wrapper.className = "modal-option";
      wrapper.innerHTML = `
        <input type="radio" name="course-schedule" value="${schedule}" id="${id}" />
        <span>${schedule}</span>
      `;
      modalSchedules.appendChild(wrapper);
    });

    modalConfirm.disabled = true;
    modal.classList.remove("hidden");
  }

  function closeModal() {
    modal.classList.add("hidden");
    pendingCourse = null;
    selectedSchedule = "";
  }

  modalSchedules.addEventListener("change", (event) => {
    const target = event.target;
    if (target && target.matches("input[type='radio']")) {
      selectedSchedule = target.value;
      modalConfirm.disabled = false;
    }
  });

  modalCancel.addEventListener("click", closeModal);

  modalConfirm.addEventListener("click", () => {
    if (!pendingCourse || !selectedSchedule) return;

    const selected = getSelectedCourses();
    if (selected.some((c) => c.code === pendingCourse.code)) {
      showMessage(`Hoc phan ${pendingCourse.code} da co trong danh sach dang ky.`);
      closeModal();
      return;
    }

    registerCourse(pendingCourse, selectedSchedule);
  });

  table.addEventListener("click", (event) => {
    const target = event.target;
    if (!(target instanceof HTMLElement)) return;
    if (!target.matches("[data-add-course]")) return;

    const code = target.getAttribute("data-add-course") || "";
    const name = target.getAttribute("data-name") || code;
    const credits = Number(target.getAttribute("data-credits") || 0);
    const prereq = target.getAttribute("data-prereq") || "";

    const completedCourses = getCompletedCourses();
    if (prereq && !completedCourses.includes(prereq)) {
      showMessage(`Vui long dang ky va hoan thanh hoc phan dieu kien ${prereq} truoc.`);
      return;
    }

    openModal({ code, name, credits });
  });

  loadCourses();

  async function registerCourse(course, schedule) {
    const payload = { maHocKy, maHocPhan: course.code };
    const response = await fetchJson("/api/dang-ky-hoc-phan", {
      method: "POST",
      body: JSON.stringify(payload)
    });

    if (response === null) {
      showMessage("Dang ky hoc phan that bai. Vui long thu lai.");
      return;
    }

    const selected = getSelectedCourses();
    if (!selected.some((item) => item.code === course.code)) {
      selected.push({
        code: course.code,
        name: course.name,
        credits: course.credits,
        schedule,
        status: "Da xac nhan"
      });
      saveSelectedCourses(selected);
    }

    showMessage(`Da dang ky hoc phan ${course.code}.`);
    closeModal();
  }
})();
