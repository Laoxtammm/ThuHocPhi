(() => {
  const table = document.querySelector("[data-fee-table]");
  if (!table) return;

  const totalEl = document.getElementById("fee-total");
  const paidEl = document.getElementById("fee-paid");
  const remainingEl = document.getElementById("fee-remaining");
  const remainingBadge = document.getElementById("fee-remaining-badge");
  const token = localStorage.getItem("thuhocphi_token");
  const maHocKy = table.getAttribute("data-ma-hoc-ky") || "HK1_2526";

  function formatMoney(value) {
    return `${Number(value || 0).toLocaleString("vi-VN")}d`;
  }

  function mapStatus(code) {
    if (code === 3) return { label: "Da thanh toan", badge: "ok" };
    if (code === 2) return { label: "Dang thanh toan", badge: "warn" };
    if (code === 4) return { label: "Qua han", badge: "danger" };
    return { label: "Chua thanh toan", badge: "warn" };
  }

  async function fetchJson(url, options) {
    const response = await fetch(url, {
      headers: {
        "Content-Type": "application/json",
        ...(token ? { Authorization: `Bearer ${token}` } : {})
      },
      credentials: "same-origin",
      ...options
    });
    if (!response.ok) {
      return null;
    }
    if (response.status === 204) {
      return {};
    }
    return response.json();
  }

  async function renderFromApi() {
    const profileResponse = await fetchJson("/api/sinh-vien/me");
    if (!profileResponse?.maSv) return false;

    let congNo = await fetchJson(
      `/api/cong-no?maSv=${encodeURIComponent(profileResponse.maSv)}&maHocKy=${encodeURIComponent(maHocKy)}`,
    );

    if (!congNo) {
      await fetchJson("/api/cong-no/tu-tinh", {
        method: "POST",
        body: JSON.stringify({ maHocKy })
      });
      congNo = await fetchJson(
        `/api/cong-no?maSv=${encodeURIComponent(profileResponse.maSv)}&maHocKy=${encodeURIComponent(maHocKy)}`,
      );
    }

    if (!congNo) return false;

    const statusInfo = mapStatus(congNo.trangThai);
    if (totalEl) totalEl.textContent = formatMoney(congNo.tongPhaiNop);
    if (paidEl) paidEl.textContent = formatMoney(congNo.tongDaNop);
    if (remainingEl) remainingEl.textContent = formatMoney(congNo.conNo);
    if (remainingBadge) {
      remainingBadge.textContent = congNo.conNo > 0 ? "Con no" : "Da hoan thanh";
      remainingBadge.classList.toggle("ok", congNo.conNo <= 0);
      remainingBadge.classList.toggle("warn", congNo.conNo > 0);
    }

    const tbody = table.querySelector("tbody");
    if (!tbody) return true;
    tbody.innerHTML = "";

    if (!congNo.chiTiet || !congNo.chiTiet.length) {
      const row = document.createElement("tr");
      row.innerHTML = `<td colspan=\"4\">Chua co khoan thu trong hoc ky nay.</td>`;
      tbody.appendChild(row);
      return true;
    }

    congNo.chiTiet.forEach((item) => {
      const row = document.createElement("tr");
      row.innerHTML = `
        <td>${item.moTa}</td>
        <td>${maHocKy}</td>
        <td>${formatMoney(item.phaiThu)}</td>
        <td><span class="badge ${statusInfo.badge}">${statusInfo.label}</span></td>
      `;
      tbody.appendChild(row);
    });

    return true;
  }

  renderFromApi().then((success) => {
    if (success) return;
    if (totalEl) totalEl.textContent = "0d";
    if (paidEl) paidEl.textContent = "0d";
    if (remainingEl) remainingEl.textContent = "0d";
    if (remainingBadge) remainingBadge.textContent = "Chua co cong no";
    const tbody = table.querySelector("tbody");
    if (tbody) {
      tbody.innerHTML = `<tr><td colspan=\"4\">Chua co cong no cho hoc ky nay.</td></tr>`;
    }
  });
})();
