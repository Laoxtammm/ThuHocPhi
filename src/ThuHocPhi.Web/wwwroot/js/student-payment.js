(() => {
  const note = document.getElementById("payment-note");
  const output = document.getElementById("payment-output");
  const payBtn = document.getElementById("btn-pay");
  const receiptBtn = document.getElementById("btn-receipt");
  const methodButtons = document.querySelectorAll("[data-payment]");
  const paymentTable = document.querySelector("[data-payment-table]");
  const paymentBody = paymentTable?.querySelector("tbody");

  const token = localStorage.getItem("thuhocphi_token");
  let selectedMethod = "";
  let lastReceiptText = "";
  let lastPaymentCode = "";
  let currentItems = [];
  let currentProfile = null;
  let currentCongNo = null;
  const maHocKy = paymentTable?.getAttribute("data-ma-hoc-ky") || "HK1_2526";

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

  function mapMethodToCode(method) {
    if (method === "VNPay") return "MB";
    if (method === "MoMo") return "MB";
    if (method === "Chuyen khoan") return "MB";
    return "MB";
  }

  function mapStatus(code) {
    if (code === 3) return { label: "Da thanh toan", badge: "ok" };
    if (code === 2) return { label: "Dang thanh toan", badge: "warn" };
    if (code === 4) return { label: "Qua han", badge: "danger" };
    return { label: "Chua thanh toan", badge: "warn" };
  }

  async function fetchJson(url) {
    try {
      const response = await fetch(url, {
        headers: token ? { Authorization: `Bearer ${token}` } : {},
        credentials: "same-origin",
      });
      if (!response.ok) return null;
      return await response.json();
    } catch {
      return null;
    }
  }

  async function getProfile() {
    if (currentProfile) return currentProfile;
    currentProfile = await fetchJson("/api/sinh-vien/me");
    return currentProfile;
  }

  async function getCongNo(maSv) {
    return fetchJson(`/api/cong-no?maSv=${encodeURIComponent(maSv)}&maHocKy=${encodeURIComponent(maHocKy)}`);
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

  function renderItems() {
    if (!paymentBody) return;
    paymentBody.innerHTML = "";

    if (!currentItems.length) {
      const row = document.createElement("tr");
      row.innerHTML = `<td colspan="5">Khong co khoan thu can thanh toan.</td>`;
      paymentBody.appendChild(row);
      return;
    }

    const statusInfo = mapStatus(currentCongNo?.trangThai);
    const isFullyPaid =
      currentCongNo &&
      Number(currentCongNo.tongPhaiNop || 0) -
        Number(currentCongNo.tienMienGiam || 0) -
        Number(currentCongNo.tongDaNop || 0) <=
        0;

    currentItems.forEach((item, index) => {
      const row = document.createElement("tr");
      row.innerHTML = `
        <td>${item.name}</td>
        <td>${item.term}</td>
        <td>${formatMoney(item.amount)}</td>
        <td><span class="badge ${statusInfo.badge}">${statusInfo.label}</span></td>
        <td>
          <button class="btn secondary" type="button" data-pay-item data-item-index="${index}">${
            isFullyPaid ? "Da thanh toan" : "Thanh toan"
          }</button>
        </td>
      `;

      const button = row.querySelector("[data-pay-item]");
      if (button) {
        button.disabled = isFullyPaid || Number(item.amount) <= 0;
        button.classList.toggle("disabled", button.disabled);
      }
      paymentBody.appendChild(row);
    });
  }

  async function loadItems() {
    const profile = await getProfile();
    if (!profile?.maSv) {
      if (paymentBody) {
        paymentBody.innerHTML = `<tr><td colspan="5">Khong tim thay thong tin sinh vien.</td></tr>`;
      }
      return;
    }

    let congNo = await getCongNo(profile.maSv);
    if (!congNo) {
      await fetchJson("/api/cong-no/tu-tinh", {
        method: "POST",
        body: JSON.stringify({ maHocKy })
      });
      congNo = await getCongNo(profile.maSv);
    }

    if (!congNo) {
      if (paymentBody) {
        paymentBody.innerHTML = `<tr><td colspan="5">Chua co cong no cho hoc ky nay.</td></tr>`;
      }
      return;
    }

    currentProfile = profile;
    currentCongNo = congNo;
    currentItems = (congNo.chiTiet || []).map((item) => ({
      name: item.moTa,
      term: congNo.maHocKy,
      maHocKy: congNo.maHocKy,
      amount: Number(item.phaiThu || 0),
    }));
    renderItems();
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
    if (!profile?.maSv) {
      if (output) output.textContent = "Khong tim thay thong tin sinh vien.";
      return;
    }
    const courses = getSelectedCourses();
    const discountPercent = profile?.maSv?.toLowerCase() === "sv002" ? 50 : 0;
    const scholarshipAmount = profile?.maSv?.toLowerCase() === "sv001" ? 750000 : 0;

    const methodCode = mapMethodToCode(selectedMethod);
    const paymentCode = buildPaymentCode(profile);
    const grouped = items.reduce((acc, item) => {
      const key = item.maHocKy || "HK1_2526";
      if (!acc[key]) acc[key] = [];
      acc[key].push(item);
      return acc;
    }, {});

    for (const maHocKyKey of Object.keys(grouped)) {
      const groupItems = grouped[maHocKyKey];
      const total = groupItems.reduce((sum, item) => sum + Number(item.amount || 0), 0);
      const payload = {
        maSV: profile?.maSv || "SV001",
        maHocKy: maHocKyKey,
        soTien: total,
        maPhuongThuc: methodCode,
        maGiaoDichNganHang: paymentCode,
        xuatBienLai: true,
      };

      const response = await fetch("/api/thanh-toan", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          ...(token ? { Authorization: `Bearer ${token}` } : {}),
        },
        credentials: "same-origin",
        body: JSON.stringify(payload),
      });

      if (!response.ok) {
        const errorText = await response.text();
        if (output) output.textContent = `Thanh toan that bai: ${errorText}`;
        return;
      }
    }

    lastReceiptText = buildReceipt(profile, items, courses, discountPercent, scholarshipAmount);
    if (output) {
      output.textContent = lastReceiptText;
    }

    await loadItems();
  }

  if (payBtn) {
    payBtn.addEventListener("click", async () => {
      if (!currentItems.length) {
        if (output) output.textContent = "Khong co khoan thu can thanh toan.";
        return;
      }
      await handlePayment(currentItems);
    });
  }

  if (paymentBody) {
    paymentBody.addEventListener("click", async (event) => {
      const target = event.target;
      if (!(target instanceof HTMLElement)) return;
      if (!target.matches("[data-pay-item]")) return;
      const index = Number(target.getAttribute("data-item-index") || -1);
      if (index < 0 || index >= currentItems.length) return;
      const item = currentItems[index];
      if (!item || !item.amount) return;
      await handlePayment([item]);
    });
  }

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

  loadItems();
})();
