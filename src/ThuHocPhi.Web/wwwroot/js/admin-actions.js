(() => {
  function formatMoney(value) {
    return `${Number(value).toLocaleString("vi-VN")}d`;
  }

  function downloadCsv(filename, rows) {
    const content = rows.map((row) => row.map((cell) => `"${cell}"`).join(",")).join("\n");
    const blob = new Blob([content], { type: "text/csv;charset=utf-8" });
    const url = URL.createObjectURL(blob);
    const link = document.createElement("a");
    link.href = url;
    link.download = filename;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    URL.revokeObjectURL(url);
  }

  const feeAddBtn = document.querySelector('[data-admin-action="add-fee"]');
  const feeImportBtn = document.querySelector('[data-admin-action="import-fee"]');
  const feeTable = document.getElementById("admin-fee-table");
  const feeOutput = document.getElementById("admin-fee-output");

  if (feeAddBtn && feeTable) {
    feeAddBtn.addEventListener("click", () => {
      const code = prompt("Ma bieu phi:");
      if (!code) return;
      const name = prompt("Ten bieu phi:");
      const term = prompt("Hoc ky (HK1/HK2):", "HK1");
      const amount = prompt("Muc thu:", "12000000");
      const status = prompt("Trang thai (Hieu luc/Nhap):", "Hieu luc");
      const row = document.createElement("tr");
      row.innerHTML = `
        <td>${code}</td>
        <td>${name || ""}</td>
        <td>${term || ""}</td>
        <td>${formatMoney(amount || 0)}</td>
        <td><span class="badge ${status === "Hieu luc" ? "ok" : "warn"}">${status || "Nhap"}</span></td>
      `;
      feeTable.querySelector("tbody")?.appendChild(row);
    });
  }

  if (feeImportBtn && feeOutput) {
    feeImportBtn.addEventListener("click", () => {
      feeOutput.textContent = "Da nhan yeu cau nhap tu Excel (demo).";
    });
  }

  const userAddBtn = document.querySelector('[data-admin-action="add-user"]');
  const userExportBtn = document.querySelector('[data-admin-action="export-user"]');
  const userTable = document.getElementById("admin-user-table");

  if (userAddBtn && userTable) {
    userAddBtn.addEventListener("click", () => {
      const username = prompt("Tai khoan:");
      if (!username) return;
      const fullName = prompt("Ho ten:");
      const role = prompt("Vai tro:", "PhongTaiChinh");
      const row = document.createElement("tr");
      row.innerHTML = `
        <td>${username}</td>
        <td>${fullName || ""}</td>
        <td>${role || ""}</td>
        <td><span class="badge ok">Hoat dong</span></td>
        <td><button class="btn secondary" type="button" data-edit-user>Sua</button></td>
      `;
      userTable.querySelector("tbody")?.appendChild(row);
    });
  }

  if (userExportBtn && userTable) {
    userExportBtn.addEventListener("click", () => {
      const rows = [["Tai khoan", "Ho ten", "Vai tro", "Trang thai"]];
      userTable.querySelectorAll("tbody tr").forEach((row) => {
        const cols = Array.from(row.querySelectorAll("td")).slice(0, 4);
        rows.push(cols.map((c) => c.textContent.trim()));
      });
      downloadCsv("nguoi-dung.csv", rows);
    });
  }

  if (userTable) {
    userTable.addEventListener("click", (event) => {
      const target = event.target;
      if (!(target instanceof HTMLElement)) return;
      const row = target.closest("tr");
      if (!row) return;

      if (target.matches("[data-edit-user]")) {
        const nameCell = row.children[1];
        const roleCell = row.children[2];
        const newName = prompt("Ho ten:", nameCell.textContent.trim());
        const newRole = prompt("Vai tro:", roleCell.textContent.trim());
        if (newName) nameCell.textContent = newName;
        if (newRole) roleCell.textContent = newRole;
      }

      if (target.matches("[data-toggle-user]")) {
        const statusCell = row.children[3];
        const badge = statusCell.querySelector(".badge");
        const isActive = badge?.classList.contains("ok");
        if (badge) {
          badge.textContent = isActive ? "Tam khoa" : "Hoat dong";
          badge.classList.toggle("ok", !isActive);
          badge.classList.toggle("warn", isActive);
        }
        target.textContent = isActive ? "Mo khoa" : "Tam khoa";
      }
    });
  }

  const noticeAddBtn = document.querySelector('[data-admin-action="add-notice"]');
  const noticeRemindBtn = document.querySelector('[data-admin-action="remind-notice"]');
  const noticeTable = document.getElementById("admin-notice-table");
  const noticeOutput = document.getElementById("admin-notice-output");

  if (noticeAddBtn && noticeTable) {
    noticeAddBtn.addEventListener("click", () => {
      const code = prompt("Ma thong bao:");
      if (!code) return;
      const term = prompt("Hoc ky:", "HK1 2025-2026");
      const scope = prompt("Pham vi:", "Khoa CNTT");
      const deadline = prompt("Han nop:", "30/09/2025");
      const status = prompt("Trang thai (Da phat hanh/Nhap):", "Nhap");
      const row = document.createElement("tr");
      row.innerHTML = `
        <td>${code}</td>
        <td>${term || ""}</td>
        <td>${scope || ""}</td>
        <td>${deadline || ""}</td>
        <td><span class="badge ${status === "Da phat hanh" ? "ok" : "warn"}">${status || "Nhap"}</span></td>
      `;
      noticeTable.querySelector("tbody")?.appendChild(row);
    });
  }

  if (noticeRemindBtn && noticeOutput) {
    noticeRemindBtn.addEventListener("click", () => {
      noticeOutput.textContent = "Da gui nhac no den sinh vien (demo).";
    });
  }
})();
