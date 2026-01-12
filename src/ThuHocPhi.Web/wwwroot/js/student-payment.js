(() => {
  const note = document.getElementById("payment-note");
  const output = document.getElementById("payment-output");
  const payBtn = document.getElementById("btn-pay");
  const receiptBtn = document.getElementById("btn-receipt");
  const methodButtons = document.querySelectorAll("[data-payment]");
  const itemButtons = document.querySelectorAll("[data-pay-item]");
  const paymentTable = document.getElementById("payment-items");

  const token = localStorage.getItem("thuhocphi_token");
  const paidKey = "thuhocphi_paid_items";
  let selectedMethod = "";
  let lastReceiptText = "";
  let lastPaymentCode = "";

  const defaultCourses = [
    { code: "IT101", name: "Nhap mon lap trinh", credits: 3 },
    { code: "MA201", name: "Giai tich 2", credits: 4 },
    { code: "PHY105", name: "Vat ly dai cuong", credits: 4 },
  ];

  function getSelectedCourses() {
    try {
      const raw = localStorage.getItem("selected_courses");
      if (!raw) return defaultCourses;
      const parsed = JSON.parse(raw);
      if (!Array.isArray(parsed) || parsed.length === 0) return defaultCourses;
      return parsed;
    } catch {
      return defaultCourses;
    }
  }

  function formatMoney(value) {
    return `${value.toLocaleString("vi-VN")}d`;
  }

  function randomString(length) {
    const chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    let result = "";
    for (let i = 0; i < length; i += 1) {
      result += chars[Math.floor(Math.random() * chars.length)];
    }
    return result;
  }

  function randomDigits(length) {
    let result = "";
    for (let i = 0; i < length; i += 1) {
      result += Math.floor(Math.random() * 10);
    }
    return result;
  }

  function buildKey(item) {
    return `${item.name}|${item.term}|${item.amount}`.toLowerCase();
  }

  function loadPaidItems() {
    try {
      const raw = localStorage.getItem(paidKey);
      if (!raw) return new Set();
      const parsed = JSON.parse(raw);
      if (!Array.isArray(parsed)) return new Set();
      return new Set(parsed);
    } catch {
      return new Set();
    }
  }

  function savePaidItems(set) {
    localStorage.setItem(paidKey, JSON.stringify(Array.from(set)));
  }

  async function getProfile() {
    try {
      const response = await fetch("/api/sinh-vien/me", {
        headers: token ? { Authorization: `Bearer ${token}` } : {},
        credentials: "same-origin",
      });
      if (!response.ok) return null;
      return await response.json();
    } catch {
      return null;
    }
  }

  function buildPaymentCode(profile) {
    if (lastPaymentCode) return lastPaymentCode;
    const now = new Date();
    const stamp = `${now.getFullYear()}${String(now.getMonth() + 1).padStart(2, "0")}${String(
      now.getDate(),
    ).padStart(2, "0")}${String(now.getHours()).padStart(2, "0")}${String(
      now.getMinutes(),
    ).padStart(2, "0")}${String(now.getSeconds()).padStart(2, "0")}`;
    const suffix = `${randomString(4)}${randomDigits(4)}`;
    lastPaymentCode = `${profile?.hoTen || ""} ${profile?.maSv || ""} ${stamp} ${suffix}`.trim();
    return lastPaymentCode;
  }

  function buildReceipt(profile, items, courses, discountPercent, scholarshipAmount) {
    const totalCredits = courses.reduce((sum, c) => sum + c.credits, 0);
    const unitFee = 600000;
    const tuition = totalCredits * unitFee;
    const discount = Math.round((tuition * discountPercent) / 100);
    const afterDiscount = tuition - discount - scholarshipAmount;

    const lines = [];
    lines.push("BIEN LAI THANH TOAN HOC PHI");
    lines.push(`Sinh vien: ${profile?.hoTen || ""} (${profile?.maSv || ""})`);
    lines.push(`Ngay: ${new Date().toLocaleString("vi-VN")}`);
    lines.push("--- Danh sach hoc phan ---");
    courses.forEach((c) => {
      lines.push(`- ${c.code} | ${c.name} | ${c.credits} TC`);
    });
    lines.push(`Tong tin chi: ${totalCredits}`);
    lines.push(`Thanh tien (theo tin chi): ${formatMoney(tuition)}`);
    lines.push(`Mien giam: ${discountPercent}% (${formatMoney(discount)})`);
    lines.push(`Hoc bong ky truoc: ${formatMoney(scholarshipAmount)}`);
    lines.push(`Hoc phi sau mien giam: ${formatMoney(afterDiscount)}`);
    lines.push("--- Khoan thu thanh toan ---");
    items.forEach((item) => {
      lines.push(`- ${item.name} | ${item.term} | ${formatMoney(item.amount)}`);
    });
    lines.push("---");
    lines.push(`Noi dung thanh toan: ${buildPaymentCode(profile)}`);

    return lines.join("\n");
  }

  function updateRowStatus(row, isPaid) {
    const badge = row.querySelector("[data-status]");
    if (badge) {
      badge.textContent = isPaid ? "Da thanh toan" : "Chua thanh toan";
      badge.classList.toggle("ok", isPaid);
      badge.classList.toggle("warn", !isPaid);
    }

    const button = row.querySelector("[data-pay-item]");
    if (button) {
      button.disabled = isPaid;
      button.classList.toggle("disabled", isPaid);
      button.textContent = isPaid ? "Da thanh toan" : "Thanh toan";
    }
  }

  function syncPaymentTable() {
    if (!paymentTable) return;
    const paid = loadPaidItems();
    const rows = Array.from(paymentTable.querySelectorAll("tbody tr"));
    rows.forEach((row) => {
      const btn = row.querySelector("[data-pay-item]");
      if (!btn) return;
      const item = {
        name: btn.getAttribute("data-name") || "",
        term: btn.getAttribute("data-term") || "",
        amount: Number(btn.getAttribute("data-amount") || 0),
      };
      updateRowStatus(row, paid.has(buildKey(item)));
    });
  }

  function markItemsPaid(items) {
    const paid = loadPaidItems();
    items.forEach((item) => {
      paid.add(buildKey(item));
    });
    savePaidItems(paid);
    if (!paymentTable) return;
    const rows = Array.from(paymentTable.querySelectorAll("tbody tr"));
    items.forEach((item) => {
      const key = buildKey(item);
      rows.forEach((row) => {
        const btn = row.querySelector("[data-pay-item]");
        if (!btn) return;
        const rowItem = {
          name: btn.getAttribute("data-name") || "",
          term: btn.getAttribute("data-term") || "",
          amount: Number(btn.getAttribute("data-amount") || 0),
        };
        if (buildKey(rowItem) === key) {
          updateRowStatus(row, true);
        }
      });
    });
  }

  methodButtons.forEach((btn) => {
    btn.addEventListener("click", () => {
      selectedMethod = btn.getAttribute("data-payment") || "";
      if (note) {
        note.textContent = `Da chon phuong thuc: ${selectedMethod}. Vui long tiep tuc thanh toan.`;
      }
    });
  });

  async function handlePayment(items) {
    if (!selectedMethod) {
      if (output) output.textContent = "Vui long chon phuong thuc thanh toan truoc.";
      return;
    }

    lastPaymentCode = "";
    const profile = await getProfile();
    const courses = getSelectedCourses();
    const discountPercent = profile?.maSv?.toLowerCase() === "sv002" ? 50 : 0;
    const scholarshipAmount = profile?.maSv?.toLowerCase() === "sv001" ? 750000 : 0;

    lastReceiptText = buildReceipt(profile, items, courses, discountPercent, scholarshipAmount);
    if (output) {
      output.textContent = lastReceiptText;
    }
    markItemsPaid(items);
  }

  if (payBtn) {
    payBtn.addEventListener("click", async () => {
      const items = [
        { name: "Hoc phi HK1", term: "HK1 2025/2026", amount: 12000000 },
        { name: "Le phi dich vu", term: "HK1 2025/2026", amount: 500000 },
      ];
      await handlePayment(items);
    });
  }

  itemButtons.forEach((btn) => {
    btn.addEventListener("click", async () => {
      const name = btn.getAttribute("data-name") || "Khoan thu";
      const term = btn.getAttribute("data-term") || "";
      const amount = Number(btn.getAttribute("data-amount") || 0);
      if (!amount) return;
      await handlePayment([{ name, term, amount }]);
    });
  });

  if (receiptBtn) {
    receiptBtn.addEventListener("click", () => {
      if (!lastReceiptText) {
        if (output) output.textContent = "Vui long thuc hien thanh toan truoc khi xuat bien lai.";
        return;
      }

      const blob = new Blob([lastReceiptText], { type: "text/plain;charset=utf-8" });
      const url = URL.createObjectURL(blob);
      const link = document.createElement("a");
      link.href = url;
      link.download = "bien-lai-thanh-toan.txt";
      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);
      URL.revokeObjectURL(url);
    });
  }

  syncPaymentTable();
})();
