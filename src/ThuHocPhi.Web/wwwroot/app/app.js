const API_BASE = "";
const state = {
  token: localStorage.getItem("thuhocphi_token") || "",
  user: null,
};

const navLogin = document.getElementById("nav-login");
const navRegister = document.getElementById("nav-register");
const navApp = document.getElementById("nav-app");
const logoutBtn = document.getElementById("logout-btn");
const userChip = document.getElementById("user-chip");

const loginSection = document.getElementById("login-section");
const registerSection = document.getElementById("register-section");
const appSection = document.getElementById("app-section");

const heroLogin = document.getElementById("hero-login");
const heroContact = document.getElementById("hero-contact");

function setNavState() {
  const isAuthed = Boolean(state.token);
  const onAuthView = !loginSection.classList.contains("hidden") || !registerSection.classList.contains("hidden");

  navLogin.classList.toggle("hidden", isAuthed || onAuthView);
  navRegister.classList.toggle("hidden", isAuthed || onAuthView);
  navApp.classList.toggle("hidden", !isAuthed || onAuthView);
  logoutBtn.classList.toggle("hidden", !isAuthed || onAuthView);
  userChip.textContent = isAuthed ? (state.user?.fullName || state.user?.username || "Da dang nhap") : "";
}

function showSection(section) {
  loginSection.classList.add("hidden");
  registerSection.classList.add("hidden");
  if (section) {
    section.classList.remove("hidden");
    section.scrollIntoView({ behavior: "smooth", block: "start" });
  }
  if (section === loginSection || section === registerSection) {
    appSection.classList.add("hidden");
  }
  setNavState();
}

async function apiFetch(path, options = {}) {
  const headers = options.headers || {};
  headers["Content-Type"] = "application/json";
  if (state.token) {
    headers.Authorization = `Bearer ${state.token}`;
  }
  const response = await fetch(`${API_BASE}${path}`, { ...options, headers });
  if (!response.ok) {
    const text = await response.text();
    throw new Error(text || `HTTP ${response.status}`);
  }
  if (response.status === 204) {
    return null;
  }
  return response.json();
}

function showNote(id, message) {
  const el = document.getElementById(id);
  if (el) el.textContent = message;
}

async function login(username, password) {
  const data = await apiFetch("/api/auth/login", {
    method: "POST",
    body: JSON.stringify({ username, password }),
  });
  state.token = data.accessToken;
  state.user = data.user;
  localStorage.setItem("thuhocphi_token", state.token);
  setNavState();
  appSection.classList.remove("hidden");
  loginSection.classList.add("hidden");
}

async function loadProfile() {
  const data = await apiFetch("/api/auth/me");
  state.user = data;
  userChip.textContent = data.fullName || data.username;
  document.getElementById("profile-output").textContent = JSON.stringify(data, null, 2);
}

navLogin.addEventListener("click", () => showSection(loginSection));
navRegister.addEventListener("click", () => showSection(registerSection));
navApp.addEventListener("click", () => {
  appSection.classList.remove("hidden");
  appSection.scrollIntoView({ behavior: "smooth", block: "start" });
});

logoutBtn.addEventListener("click", () => {
  state.token = "";
  state.user = null;
  localStorage.removeItem("thuhocphi_token");
  appSection.classList.add("hidden");
  setNavState();
});

heroLogin.addEventListener("click", () => showSection(loginSection));
if (heroContact) {
  heroContact.addEventListener("click", () => {
    window.location.href = "/app/contact.html";
  });
}

document.getElementById("login-form").addEventListener("submit", async (event) => {
  event.preventDefault();
  const form = event.currentTarget;
  const username = form.username.value.trim();
  const password = form.password.value.trim();
  showNote("login-note", "Dang dang nhap...");
  try {
    await login(username, password);
    showNote("login-note", "Dang nhap thanh cong.");
    await loadProfile();
  } catch (error) {
    showNote("login-note", `Dang nhap that bai: ${error.message}`);
  }
});

document.getElementById("refresh-profile").addEventListener("click", async () => {
  try {
    await loadProfile();
  } catch (error) {
    showNote("profile-output", error.message);
  }
});

document.getElementById("bieu-phi-form").addEventListener("submit", async (event) => {
  event.preventDefault();
  const f = event.currentTarget;
  const payload = {
    maLoaiPhi: f.maLoaiPhi.value.trim(),
    maHocKy: f.maHocKy.value.trim(),
    heDaoTao: f.heDaoTao.value.trim() || null,
    loaiHinhDaoTao: f.loaiHinhDaoTao.value.trim() || null,
    donGia: Number(f.donGia.value),
    ngayApDung: f.ngayApDung.value,
    ghiChu: f.ghiChu.value.trim() || null,
  };
  try {
    await apiFetch("/api/bieu-phi", { method: "POST", body: JSON.stringify(payload) });
    showNote("bieu-phi-note", "Tao bieu phi thanh cong.");
    await loadBieuPhi();
  } catch (error) {
    showNote("bieu-phi-note", error.message);
  }
});

document.getElementById("bieu-phi-refresh").addEventListener("click", loadBieuPhi);

async function loadBieuPhi() {
  try {
    const data = await apiFetch("/api/bieu-phi");
    const rows = data.map((item) => `
      <tr>
        <td>${item.maBieuPhi}</td>
        <td>${item.maLoaiPhi}</td>
        <td>${item.maHocKy}</td>
        <td>${item.donGia}</td>
        <td>${item.trangThai ? "Hieu luc" : "Ngung"}</td>
      </tr>
    `);
    document.getElementById("bieu-phi-table").innerHTML = rows.join("");
  } catch (error) {
    showNote("bieu-phi-note", error.message);
  }
}

document.getElementById("cong-no-tinh").addEventListener("click", async () => {
  const form = document.getElementById("cong-no-form");
  const payload = {
    maSV: form.maSv.value.trim(),
    maHocKy: form.maHocKy.value.trim(),
  };
  try {
    const data = await apiFetch("/api/cong-no/tinh", {
      method: "POST",
      body: JSON.stringify(payload),
    });
    showNote("cong-no-note", `Tinh cong no: ${data.tongPhaiNop}`);
  } catch (error) {
    showNote("cong-no-note", error.message);
  }
});

document.getElementById("cong-no-form").addEventListener("submit", async (event) => {
  event.preventDefault();
  const form = event.currentTarget;
  const maSv = form.maSv.value.trim();
  const maHocKy = form.maHocKy.value.trim();
  try {
    const data = await apiFetch(`/api/cong-no?maSv=${encodeURIComponent(maSv)}&maHocKy=${encodeURIComponent(maHocKy)}`);
    document.getElementById("cong-no-output").textContent = JSON.stringify(data, null, 2);
  } catch (error) {
    showNote("cong-no-note", error.message);
  }
});

document.getElementById("thanh-toan-form").addEventListener("submit", async (event) => {
  event.preventDefault();
  const f = event.currentTarget;
  const payload = {
    maSV: f.maSv.value.trim(),
    maHocKy: f.maHocKy.value.trim(),
    soTien: Number(f.soTien.value),
    maPhuongThuc: f.maPhuongThuc.value.trim(),
    maGiaoDichNganHang: f.maGiaoDichNganHang.value.trim() || null,
    nguoiThuTien: f.nguoiThuTien.value ? Number(f.nguoiThuTien.value) : null,
    xuatBienLai: true,
  };
  try {
    const data = await apiFetch("/api/thanh-toan", { method: "POST", body: JSON.stringify(payload) });
    showNote("thanh-toan-note", data.thongBao || "Thanh toan thanh cong.");
    await loadGiaoDich();
  } catch (error) {
    showNote("thanh-toan-note", error.message);
  }
});

document.getElementById("giao-dich-refresh").addEventListener("click", loadGiaoDich);

async function loadGiaoDich() {
  try {
    const data = await apiFetch("/api/thanh-toan/giao-dich");
    const rows = data.map((item) => `
      <tr>
        <td>${item.maGiaoDich}</td>
        <td>${item.soTien}</td>
        <td>${item.maPhuongThuc}</td>
        <td>${item.trangThai}</td>
      </tr>
    `);
    document.getElementById("giao-dich-table").innerHTML = rows.join("");
  } catch (error) {
    showNote("thanh-toan-note", error.message);
  }
}

document.getElementById("thong-bao-form").addEventListener("submit", async (event) => {
  event.preventDefault();
  const f = event.currentTarget;
  const payload = {
    maHocKy: f.maHocKy.value.trim(),
    namHoc: f.namHoc.value.trim() || null,
    tieuDe: f.tieuDe.value.trim(),
    noiDung: f.noiDung.value.trim() || null,
    hanNop: f.hanNop.value || null,
    doiTuongApDung: f.doiTuongApDung.value.trim() || null,
  };
  try {
    await apiFetch("/api/thong-bao-hoc-phi", { method: "POST", body: JSON.stringify(payload) });
    showNote("thong-bao-note", "Tao thong bao thanh cong.");
    await loadThongBao();
  } catch (error) {
    showNote("thong-bao-note", error.message);
  }
});

document.getElementById("thong-bao-refresh").addEventListener("click", loadThongBao);

async function loadThongBao() {
  try {
    const data = await apiFetch("/api/thong-bao-hoc-phi");
    document.getElementById("thong-bao-output").textContent = JSON.stringify(data, null, 2);
  } catch (error) {
    showNote("thong-bao-note", error.message);
  }
}

document.getElementById("tb-assign").addEventListener("click", async () => {
  const id = document.getElementById("tb-id").value.trim();
  const svRaw = document.getElementById("tb-sv").value;
  const maSVs = svRaw.split(",").map((s) => s.trim()).filter(Boolean);
  try {
    const data = await apiFetch(`/api/thong-bao-hoc-phi/${id}/recipients`, {
      method: "POST",
      body: JSON.stringify({ maSVs }),
    });
    showNote("tb-assign-note", `Da gan ${data.added} sinh vien.`);
  } catch (error) {
    showNote("tb-assign-note", error.message);
  }
});

document.getElementById("mien-giam-form").addEventListener("submit", async (event) => {
  event.preventDefault();
  const f = event.currentTarget;
  const payload = {
    maChinhSach: f.maChinhSach.value.trim(),
    tenChinhSach: f.tenChinhSach.value.trim(),
    loaiMienGiam: f.loaiMienGiam.value.trim(),
    giaTriMienGiam: Number(f.giaTriMienGiam.value),
  };
  try {
    await apiFetch("/api/mien-giam", { method: "POST", body: JSON.stringify(payload) });
    showNote("mien-giam-note", "Tao chinh sach thanh cong.");
  } catch (error) {
    showNote("mien-giam-note", error.message);
  }
});

document.getElementById("mien-giam-apply-form").addEventListener("submit", async (event) => {
  event.preventDefault();
  const f = event.currentTarget;
  const payload = {
    maSV: f.maSv.value.trim(),
    maChinhSach: f.maChinhSach.value.trim(),
    maHocKy: f.maHocKy.value.trim(),
  };
  try {
    await apiFetch("/api/mien-giam/ap-dung", { method: "POST", body: JSON.stringify(payload) });
    showNote("mien-giam-apply-note", "Da ap dung mien giam.");
  } catch (error) {
    showNote("mien-giam-apply-note", error.message);
  }
});

document.getElementById("hoc-bong-form").addEventListener("submit", async (event) => {
  event.preventDefault();
  const f = event.currentTarget;
  const payload = {
    maHocBong: f.maHocBong.value.trim(),
    tenHocBong: f.tenHocBong.value.trim(),
    loaiGiaTri: f.loaiGiaTri.value.trim(),
    giaTri: Number(f.giaTri.value),
  };
  try {
    await apiFetch("/api/hoc-bong", { method: "POST", body: JSON.stringify(payload) });
    showNote("hoc-bong-note", "Tao hoc bong thanh cong.");
  } catch (error) {
    showNote("hoc-bong-note", error.message);
  }
});

document.getElementById("hoc-bong-apply-form").addEventListener("submit", async (event) => {
  event.preventDefault();
  const f = event.currentTarget;
  const payload = {
    maSV: f.maSv.value.trim(),
    maHocBong: f.maHocBong.value.trim(),
    maHocKy: f.maHocKy.value.trim(),
  };
  try {
    await apiFetch("/api/hoc-bong/ap-dung", { method: "POST", body: JSON.stringify(payload) });
    showNote("hoc-bong-apply-note", "Da ap dung hoc bong.");
  } catch (error) {
    showNote("hoc-bong-apply-note", error.message);
  }
});

document.getElementById("doi-soat-form").addEventListener("submit", async (event) => {
  event.preventDefault();
  const f = event.currentTarget;
  let giaoDich = [];
  try {
    giaoDich = JSON.parse(f.giaoDichJson.value);
  } catch (error) {
    showNote("doi-soat-note", "JSON khong hop le.");
    return;
  }
  const payload = {
    tuNgay: f.tuNgay.value || null,
    denNgay: f.denNgay.value || null,
    giaoDichNganHang: giaoDich,
  };
  try {
    const data = await apiFetch("/api/doi-soat", { method: "POST", body: JSON.stringify(payload) });
    document.getElementById("doi-soat-output").textContent = JSON.stringify(data, null, 2);
    showNote("doi-soat-note", "Doi soat thanh cong.");
  } catch (error) {
    showNote("doi-soat-note", error.message);
  }
});

document.getElementById("nguoi-dung-form").addEventListener("submit", async (event) => {
  event.preventDefault();
  const f = event.currentTarget;
  const payload = {
    tenDangNhap: f.tenDangNhap.value.trim(),
    hoTen: f.hoTen.value.trim(),
    matKhau: f.matKhau.value.trim(),
    email: f.email.value.trim() || null,
  };
  try {
    await apiFetch("/api/quan-tri/nguoi-dung", { method: "POST", body: JSON.stringify(payload) });
    showNote("nguoi-dung-note", "Tao nguoi dung thanh cong.");
    await loadNguoiDung();
  } catch (error) {
    showNote("nguoi-dung-note", error.message);
  }
});

document.getElementById("role-update").addEventListener("click", async () => {
  const id = document.getElementById("role-user-id").value.trim();
  const roleRaw = document.getElementById("role-ids").value;
  const maVaiTro = roleRaw.split(",").map((s) => Number(s.trim())).filter((n) => Number.isFinite(n));
  try {
    await apiFetch(`/api/quan-tri/nguoi-dung/${id}/vai-tro`, {
      method: "POST",
      body: JSON.stringify({ maVaiTro }),
    });
    showNote("role-note", "Cap nhat vai tro thanh cong.");
    await loadNguoiDung();
  } catch (error) {
    showNote("role-note", error.message);
  }
});

document.getElementById("nguoi-dung-refresh").addEventListener("click", loadNguoiDung);

async function loadNguoiDung() {
  try {
    const data = await apiFetch("/api/quan-tri/nguoi-dung");
    document.getElementById("nguoi-dung-output").textContent = JSON.stringify(data, null, 2);
  } catch (error) {
    showNote("nguoi-dung-note", error.message);
  }
}

function initSlider() {
  const slider = document.getElementById("hero-slider");
  if (!slider) return;
  const images = [
    "/pictures/slider-1.jpg",
    "/pictures/slider-2.jpg",
    "/pictures/slider-3.jpg",
  ];
  let current = 0;

  function setBackground(url) {
    slider.style.backgroundImage = `url('${url}')`;
    slider.style.backgroundSize = "cover";
    slider.style.backgroundPosition = "center";
  }

  function loadImage(url) {
    return new Promise((resolve) => {
      const img = new Image();
      img.onload = () => resolve(true);
      img.onerror = () => resolve(false);
      img.src = url;
    });
  }

  async function rotate() {
    const url = images[current];
    const ok = await loadImage(url);
    if (ok) {
      setBackground(url);
    } else {
      slider.style.backgroundImage = "linear-gradient(120deg, rgba(14, 27, 31, 0.85), rgba(15, 118, 110, 0.55))";
    }
    current = (current + 1) % images.length;
  }

  rotate();
  setInterval(rotate, 5000);
}

function init() {
  if (state.token) {
    appSection.classList.remove("hidden");
    loadProfile().catch(() => {});
  } else {
    appSection.classList.add("hidden");
  }
  loginSection.classList.add("hidden");
  registerSection.classList.add("hidden");
  if (window.location.hash === "#login") {
    showSection(loginSection);
  } else if (window.location.hash === "#register") {
    showSection(registerSection);
  }
  setNavState();
  initSlider();
}

init();
