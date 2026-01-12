(() => {
  const note = document.getElementById("course-note");
  const buttons = document.querySelectorAll("[data-add-course]");
  const modal = document.getElementById("course-modal");
  const modalName = document.getElementById("modal-course-name");
  const modalSchedules = document.getElementById("modal-schedules");
  const modalCancel = document.getElementById("modal-cancel");
  const modalConfirm = document.getElementById("modal-confirm");

  if (!buttons.length || !modal) return;

  const scheduleMap = {
    IT101: ["T2 - T4 (7:00 - 9:30)", "T3 - T5 (9:30 - 12:00)"],
    MA201: ["T2 - T4 (13:30 - 16:00)", "T5 (7:00 - 11:30)"],
    CN301: ["T6 (7:00 - 11:30)", "T7 (7:00 - 11:30)"],
  };

  let pendingCourse = null;
  let selectedSchedule = "";

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

  function showMessage(message) {
    if (note) note.textContent = message;
  }

  function openModal(course) {
    pendingCourse = course;
    selectedSchedule = "";
    if (modalName) {
      modalName.textContent = `${course.code} - ${course.name} (${course.credits} TC)`;
    }

    modalSchedules.innerHTML = "";
    const schedules = scheduleMap[course.code] || ["Chưa có lịch cụ thể"];
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
      showMessage(`Học phần ${pendingCourse.code} đã có trong danh sách đăng ký.`);
      closeModal();
      return;
    }

    selected.push({
      code: pendingCourse.code,
      name: pendingCourse.name,
      credits: pendingCourse.credits,
      schedule: selectedSchedule,
      status: "Chờ xác nhận",
    });
    saveSelectedCourses(selected);
    showMessage(`Đã thêm học phần ${pendingCourse.code}. Vui lòng quay lại trang đăng ký để xác nhận.`);
    closeModal();
  });

  buttons.forEach((btn) => {
    btn.addEventListener("click", () => {
      const code = btn.getAttribute("data-add-course");
      const name = btn.getAttribute("data-name") || code;
      const credits = Number(btn.getAttribute("data-credits") || 0);
      const prereq = btn.getAttribute("data-prereq");

      const completedCourses = getCompletedCourses();
      if (prereq && !completedCourses.includes(prereq)) {
        showMessage(`Vui lòng đăng ký và hoàn thành học phần điều kiện ${prereq} trước.`);
        return;
      }

      openModal({ code, name, credits });
    });
  });
})();
