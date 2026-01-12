(() => {
  const summaryBody = document.getElementById("accounting-summary-body");
  const detailBody = document.getElementById("detail-payment-body");
  const detailName = document.getElementById("detail-name");
  const detailClass = document.getElementById("detail-class");
  const detailCredits = document.getElementById("detail-credits");
  const detailPaid = document.getElementById("detail-paid");
  const detailRemaining = document.getElementById("detail-remaining");
  const detailCount = document.getElementById("detail-count");
  const token = localStorage.getItem("thuhocphi_token");

  if (!summaryBody || !detailBody) return;

  const container = summaryBody.closest(".section-block");
  const maHocKy = container?.getAttribute("data-ma-hoc-ky") || "HK1_2526";

  function formatMoney(value) {
    return `${Number(value || 0).toLocaleString("vi-VN")}d`;
  }

  async function fetchJson(url) {
    const response = await fetch(url, {
      headers: token ? { Authorization: `Bearer ${token}` } : {},
      credentials: "same-origin",
    });
    if (!response.ok) {
      return [];
    }
    return response.json();
  }

  function renderSummary(rows) {
    summaryBody.innerHTML = "";
    rows.forEach((item, index) => {
      const row = document.createElement("tr");
      row.setAttribute("data-ma-sv", item.maSv);
      row.innerHTML = `
        <td>${index + 1}</td>
        <td>${item.hoTen}</td>
        <td>${item.ngaySinh ? new Date(item.ngaySinh).toLocaleDateString("vi-VN") : ""}</td>
        <td>${item.lop}</td>
        <td>${item.soTinChi}</td>
        <td data-field="total">${formatMoney(item.tongPhaiNop)}</td>
        <td>${formatMoney(item.tienMienGiam)}</td>
        <td data-field="after">${formatMoney(item.hocPhiSauMienGiam)}</td>
        <td data-field="paid">${formatMoney(item.tongDaNop)}</td>
        <td data-field="remain">${formatMoney(item.conNo)}</td>
        <td><button class="btn secondary" type="button" data-view-detail="${item.maSv}">Xem chi tiet</button></td>
      `;
      summaryBody.appendChild(row);
    });
  }

  function renderDetailHeader(info, maSv, paidSum, remaining, count) {
    if (detailName) detailName.textContent = `${info.hoTen} (${maSv})`;
    if (detailClass) detailClass.textContent = `Lop: ${info.lop}`;
    if (detailCredits) detailCredits.textContent = `Tin chi: ${info.soTinChi}`;
    if (detailPaid) detailPaid.textContent = `Da thanh toan: ${formatMoney(paidSum)}`;
    if (detailRemaining) detailRemaining.textContent = `Con no: ${formatMoney(remaining)}`;
    if (detailCount) detailCount.textContent = `So lan dong: ${count}`;
  }

  function renderDetailRows(rows) {
    detailBody.innerHTML = "";
    rows.forEach((item) => {
      const row = document.createElement("tr");
      row.innerHTML = `
        <td>${item.maHocKy}</td>
        <td>${item.ngayGiaoDich ? new Date(item.ngayGiaoDich).toLocaleDateString("vi-VN") : ""}</td>
        <td>${formatMoney(item.soTien)}</td>
        <td>${item.soBienLai || ""}</td>
        <td>${item.maGiaoDich}</td>
      `;
      detailBody.appendChild(row);
    });
  }

  async function loadSummary() {
    const data = await fetchJson(`/api/ke-toan/cong-no?maHocKy=${encodeURIComponent(maHocKy)}`);
    renderSummary(Array.isArray(data) ? data : []);
  }

  async function loadDetail(maSv, info) {
    const data = await fetchJson(
      `/api/ke-toan/giao-dich?maSv=${encodeURIComponent(maSv)}&maHocKy=${encodeURIComponent(maHocKy)}`,
    );
    const rows = Array.isArray(data) ? data : [];
    const paidSum = rows.reduce((sum, item) => sum + Number(item.soTien || 0), 0);
    const remaining = Math.max(Number(info.hocPhiSauMienGiam || 0) - paidSum, 0);
    renderDetailHeader(info, maSv, paidSum, remaining, rows.length);
    renderDetailRows(rows);
  }

  summaryBody.addEventListener("click", (event) => {
    const target = event.target;
    if (!(target instanceof HTMLElement)) return;
    if (!target.matches("[data-view-detail]")) return;

    const maSv = target.getAttribute("data-view-detail");
    if (!maSv) return;

    const row = target.closest("tr");
    const info = {
      hoTen: row?.children[1]?.textContent?.trim() || "",
      lop: row?.children[3]?.textContent?.trim() || "",
      soTinChi: row?.children[4]?.textContent?.trim() || "",
      hocPhiSauMienGiam: row?.querySelector('[data-field="after"]')?.textContent?.replace(/[^\d]/g, "") || "0",
    };
    loadDetail(maSv, info);
  });

  loadSummary();
})();
