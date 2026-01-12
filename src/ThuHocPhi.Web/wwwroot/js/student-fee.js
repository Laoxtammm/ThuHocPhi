(() => {
  const table = document.querySelector("[data-fee-table]");
  if (!table) return;

  const totalEl = document.getElementById("fee-total");
  const paidEl = document.getElementById("fee-paid");
  const remainingEl = document.getElementById("fee-remaining");
  const remainingBadge = document.getElementById("fee-remaining-badge");

  const paidKey = "thuhocphi_paid_items";

  function formatMoney(value) {
    return `${value.toLocaleString("vi-VN")}d`;
  }

  function normalize(text) {
    return String(text || "")
      .toLowerCase()
      .replace(/[^a-z0-9]/g, "");
  }

  function buildKey(item) {
    return `${normalize(item.name)}${normalize(item.term)}${Number(item.amount || 0)}`;
  }

  function loadPaidItems() {
    try {
      const raw = localStorage.getItem(paidKey);
      if (!raw) return new Set();
      const parsed = JSON.parse(raw);
      if (!Array.isArray(parsed)) return new Set();
      return new Set(parsed.map((entry) => normalize(entry)));
    } catch {
      return new Set();
    }
  }

  const paid = loadPaidItems();
  let total = 0;
  let paidTotal = 0;

  const rows = Array.from(table.querySelectorAll("tbody tr"));
  rows.forEach((row) => {
    const name = row.getAttribute("data-name") || "";
    const term = row.getAttribute("data-term") || "";
    const amount = Number(row.getAttribute("data-amount") || 0);
    const key = buildKey({ name, term, amount });
    const badge = row.querySelector("[data-status]");
    const isDefaultPaid = row.getAttribute("data-default") === "paid";
    const isPaid = paid.has(key) || isDefaultPaid;

    total += amount;
    if (isPaid) paidTotal += amount;

    if (badge) {
      badge.textContent = isPaid ? "Da thanh toan" : "Chua thanh toan";
      badge.classList.toggle("ok", isPaid);
      badge.classList.toggle("warn", !isPaid);
    }
  });

  const remaining = total - paidTotal;
  if (totalEl) totalEl.textContent = formatMoney(total);
  if (paidEl) paidEl.textContent = formatMoney(paidTotal);
  if (remainingEl) remainingEl.textContent = formatMoney(remaining);
  if (remainingBadge) {
    remainingBadge.textContent = remaining > 0 ? "Con no" : "Da hoan thanh";
    remainingBadge.classList.toggle("ok", remaining <= 0);
    remainingBadge.classList.toggle("warn", remaining > 0);
  }
})();
