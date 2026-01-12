(() => {
  const root = document.querySelector("[data-dashboard]");
  if (!root) return;

  const termEl = document.getElementById("dashboard-term");
  const termStatusEl = document.getElementById("dashboard-term-status");
  const debtEl = document.getElementById("dashboard-debt");
  const debtBadge = document.getElementById("dashboard-debt-badge");
  const creditsEl = document.getElementById("dashboard-credits");
  const token = localStorage.getItem("thuhocphi_token");
  const maHocKy = root.getAttribute("data-ma-hoc-ky") || "HK1_2526";

  function formatMoney(value) {
    return `${Number(value || 0).toLocaleString("vi-VN")}d`;
  }

  function mapDebtBadge(conNo) {
    if (conNo <= 0) {
      return { text: "Da thanh toan", className: "ok" };
    }
    return { text: "Chua thanh toan", className: "warn" };
  }

  async function fetchJson(url) {
    try {
      const response = await fetch(url, {
        headers: token ? { Authorization: `Bearer ${token}` } : {},
        credentials: "same-origin"
      });
      if (!response.ok) return null;
      return await response.json();
    } catch {
      return null;
    }
  }

  async function loadDashboard() {
    const data = await fetchJson(`/api/sinh-vien/dashboard?maHocKy=${encodeURIComponent(maHocKy)}`);
    if (!data) return;

    if (termEl) termEl.textContent = data.tenHocKy || maHocKy;
    if (termStatusEl && data.trangThaiHocKy) termStatusEl.textContent = data.trangThaiHocKy;
    if (debtEl) debtEl.textContent = formatMoney(data.conNo);

    if (debtBadge) {
      const badge = mapDebtBadge(Number(data.conNo || 0));
      debtBadge.textContent = badge.text;
      debtBadge.classList.remove("ok", "warn", "danger");
      debtBadge.classList.add(badge.className);
    }

    if (creditsEl) creditsEl.textContent = data.tongTinChi || 0;
  }

  loadDashboard();
})();
