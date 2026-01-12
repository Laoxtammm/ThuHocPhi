(() => {
  const tableBody = document.querySelector("#doi-soat-table tbody");
  const addRowBtn = document.getElementById("btn-add-row");
  const runBtn = document.getElementById("btn-doi-soat");
  const resultBlock = document.getElementById("doi-soat-result");
  const output = document.getElementById("doi-soat-output");

  if (!tableBody || !addRowBtn || !runBtn) return;

  const token = localStorage.getItem("thuhocphi_token");

  function createRow() {
    const row = document.createElement("tr");
    row.innerHTML = `
      <td><input type="text" placeholder="GD2026_001" class="input-text" /></td>
      <td><input type="number" min="0" step="0.01" placeholder="3000000" class="input-text" /></td>
      <td><input type="date" class="input-text" /></td>
      <td><button type="button" class="btn secondary btn-remove">Xóa</button></td>
    `;
    return row;
  }

  function getRows() {
    return Array.from(tableBody.querySelectorAll("tr"));
  }

  function extractItems() {
    return getRows()
      .map((row) => {
        const inputs = row.querySelectorAll("input");
        const ma = inputs[0]?.value.trim();
        const soTien = Number(inputs[1]?.value || 0);
        const ngay = inputs[2]?.value;
        if (!ma || !soTien) return null;
        return {
          maGiaoDichNganHang: ma,
          soTien,
          ngayGiaoDich: ngay || null,
        };
      })
      .filter(Boolean);
  }

  tableBody.addEventListener("click", (event) => {
    const target = event.target;
    if (target && target.classList.contains("btn-remove")) {
      const row = target.closest("tr");
      if (row && getRows().length > 1) {
        row.remove();
      }
    }
  });

  addRowBtn.addEventListener("click", () => {
    tableBody.appendChild(createRow());
  });

  runBtn.addEventListener("click", async () => {
    const tuNgay = document.getElementById("doi-soat-tu-ngay")?.value || null;
    const denNgay = document.getElementById("doi-soat-den-ngay")?.value || null;
    const nguonDuLieu = document.getElementById("doi-soat-nguon")?.value || null;
    const items = extractItems();

    if (!items.length) {
      if (output) {
        output.textContent = "Vui lòng nhập ít nhất 1 giao dịch ngân hàng.";
        resultBlock.style.display = "block";
      }
      return;
    }

    const payload = {
      tuNgay: tuNgay || null,
      denNgay: denNgay || null,
      nguonDuLieu: nguonDuLieu || null,
      giaoDichNganHang: items,
    };

    try {
      const response = await fetch("/api/doi-soat", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          ...(token ? { Authorization: `Bearer ${token}` } : {}),
        },
        credentials: "same-origin",
        body: JSON.stringify(payload),
      });

      const content = await response.text();
      if (output) {
        output.textContent = response.ok
          ? `Đối soát thành công:\n${content}`
          : `Đối soát thất bại:\n${content}`;
        resultBlock.style.display = "block";
      }
    } catch (error) {
      if (output) {
        output.textContent = `Lỗi kết nối: ${error.message}`;
        resultBlock.style.display = "block";
      }
    }
  });
})();
