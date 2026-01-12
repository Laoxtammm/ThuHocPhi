(() => {
  function run() {
    const nameEl = document.getElementById("student-name");
    const codeEl = document.getElementById("student-code");
    const avatarEl = document.getElementById("student-avatar");

    const infoMap = {
      maSv: document.getElementById("student-info-ma"),
      hoTen: document.getElementById("student-info-ten"),
      ngaySinh: document.getElementById("student-info-ngay-sinh"),
      lop: document.getElementById("student-info-lop"),
      heDaoTao: document.getElementById("student-info-he"),
      email: document.getElementById("student-info-email"),
      soDienThoai: document.getElementById("student-info-phone"),
      trangThai: document.getElementById("student-info-status"),
      avatar: document.getElementById("student-info-avatar"),
    };

    const token = localStorage.getItem("thuhocphi_token");

    function applyFallback(image) {
      if (!image) return;
      image.addEventListener("error", () => {
        image.src = "/pictures/Logo_MTA_new.png";
      });
    }

    applyFallback(avatarEl);
    applyFallback(infoMap.avatar);

    async function loadProfile() {
      try {
        const response = await fetch("/api/sinh-vien/me", {
          headers: token ? { Authorization: `Bearer ${token}` } : {},
          credentials: "same-origin",
        });

        if (!response.ok) {
          return;
        }

        const data = await response.json();
        if (nameEl) nameEl.textContent = data.hoTen || "Sinh viên";
        if (codeEl) codeEl.textContent = data.maSv || "---";

        if (avatarEl && data.avatarUrl) {
          avatarEl.src = data.avatarUrl;
        }
        if (infoMap.avatar && data.avatarUrl) {
          infoMap.avatar.src = data.avatarUrl;
        }

        if (infoMap.maSv) infoMap.maSv.textContent = data.maSv || "";
        if (infoMap.hoTen) infoMap.hoTen.textContent = data.hoTen || "";
        if (infoMap.ngaySinh) infoMap.ngaySinh.textContent = data.ngaySinh ? new Date(data.ngaySinh).toLocaleDateString("vi-VN") : "";
        if (infoMap.lop) infoMap.lop.textContent = data.lop || "";
        if (infoMap.heDaoTao) infoMap.heDaoTao.textContent = data.heDaoTao || "";
        if (infoMap.email) infoMap.email.textContent = data.email || "";
        if (infoMap.soDienThoai) infoMap.soDienThoai.textContent = data.soDienThoai || "";
        if (infoMap.trangThai) infoMap.trangThai.textContent = data.trangThai || "";
      } catch {
        // ignore
      }
    }

    loadProfile();
  }

  if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", run);
  } else {
    run();
  }
})();
