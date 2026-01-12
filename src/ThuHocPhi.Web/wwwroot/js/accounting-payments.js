(() => {
  const dataScript = document.getElementById("payment-detail-data");
  const detailBody = document.getElementById("detail-payment-body");
  const detailName = document.getElementById("detail-name");
  const detailClass = document.getElementById("detail-class");
  const detailCredits = document.getElementById("detail-credits");
  const detailPaid = document.getElementById("detail-paid");
  const detailRemaining = document.getElementById("detail-remaining");
  const detailCount = document.getElementById("detail-count");

  if (!dataScript || !detailBody) return;

  let data = {};
  try {
    data = JSON.parse(dataScript.textContent || "{}");
  } catch {
    data = {};
  }

  function formatMoney(value) {
    return `${value.toLocaleString("vi-VN")}d`;
  }

  function renderDetail(maSv) {
    const detail = data[maSv];
    if (!detail) return;

    if (detailName) detailName.textContent = `${detail.hoTen} (${maSv})`;
    if (detailClass) detailClass.textContent = `Lop: ${detail.lop}`;
    if (detailCredits) detailCredits.textContent = `Tin chi: ${detail.tinChi}`;
    if (detailPaid) detailPaid.textContent = `Da thanh toan: ${formatMoney(detail.daThanhToan)}`;
    if (detailRemaining) detailRemaining.textContent = `Con no: ${formatMoney(detail.conNo)}`;
    if (detailCount) detailCount.textContent = `So lan dong: ${detail.payments.length}`;

    detailBody.innerHTML = "";
    detail.payments.forEach((p) => {
      const row = document.createElement("tr");
      row.innerHTML = `
        <td>${p.ky}</td>
        <td>${p.ngay}</td>
        <td>${formatMoney(p.soTien)}</td>
        <td>${p.bienLai}</td>
        <td>${p.maThanhToan}</td>
      `;
      detailBody.appendChild(row);
    });
  }

  document.querySelectorAll("[data-view-detail]").forEach((btn) => {
    btn.addEventListener("click", () => {
      const maSv = btn.getAttribute("data-view-detail");
      if (maSv) renderDetail(maSv);
    });
  });
})();
