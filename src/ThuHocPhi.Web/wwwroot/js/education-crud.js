(() => {
  const token = localStorage.getItem("thuhocphi_token");

  function getColumns(table) {
    return (table.dataset.columns || "")
      .split(",")
      .map((item) => item.trim())
      .filter(Boolean);
  }

  function getFormValues(form, columns) {
    const values = {};
    columns.forEach((col) => {
      const input = form.querySelector(`[data-field="${col}"]`);
      values[col] = input ? input.value.trim() : "";
    });
    return values;
  }

  function fillForm(form, columns, row) {
    const cells = row.querySelectorAll("td");
    columns.forEach((col, index) => {
      const input = form.querySelector(`[data-field="${col}"]`);
      if (input) {
        input.value = cells[index]?.textContent?.trim() || "";
      }
    });
  }

  function clearForm(form) {
    form.querySelectorAll("[data-field]").forEach((input) => {
      input.value = "";
    });
  }

  function createActionCell() {
    const td = document.createElement("td");
    td.innerHTML = `
      <button class="btn secondary" type="button" data-edit>Sua</button>
      <button class="btn secondary" type="button" data-delete>Xoa</button>
    `;
    return td;
  }

  function parseMeta(text) {
    const source = String(text || "");
    const pick = (label) => {
      const regex = new RegExp(`${label}\\s*[:=]\\s*([^;]+)`, "i");
      const match = source.match(regex);
      return match ? match[1].trim() : "";
    };
    const prereq = pick("DieuKien") || pick("TienQuyet");
    const major = pick("ChuyenNganh");
    const className = pick("Lop");
    const noteText = pick("GhiChu");
    return { prereq, major, className, noteText: noteText || "" };
  }

  function buildMeta(values) {
    const parts = [];
    if (values.major) parts.push(`ChuyenNganh:${values.major}`);
    if (values.class) parts.push(`Lop:${values.class}`);
    if (values.condition) parts.push(`DieuKien:${values.condition}`);
    if (values.note) parts.push(`GhiChu:${values.note}`);
    return parts.join("; ");
  }

  async function fetchJson(url, options) {
    try {
      const response = await fetch(url, {
        credentials: "same-origin",
        headers: {
          "Content-Type": "application/json",
          ...(token ? { Authorization: `Bearer ${token}` } : {})
        },
        ...options
      });
      if (!response.ok) {
        return null;
      }
      if (response.status === 204) return {};
      return await response.json();
    } catch {
      return null;
    }
  }

  function renderTable(table, columns, items) {
    const tbody = table.querySelector("tbody");
    if (!tbody) return;
    tbody.innerHTML = "";

    if (!items.length) {
      const row = document.createElement("tr");
      row.innerHTML = `<td colspan="${columns.length + 1}">Chua co hoc phan.</td>`;
      tbody.appendChild(row);
      return;
    }

    items.forEach((item) => {
      const meta = parseMeta(item.moTa);
      const row = document.createElement("tr");
      row.dataset.code = item.maHocPhan;
      row.dataset.major = meta.major || "";
      row.dataset.class = meta.className || "";

      columns.forEach((col) => {
        const cell = document.createElement("td");
        let value = "";
        if (col === "code") value = item.maHocPhan;
        if (col === "name") value = item.tenHocPhan;
        if (col === "type") value = item.loaiHocPhan || "";
        if (col === "credits") value = item.soTinChi || "";
        if (col === "major") value = meta.major || item.maKhoa || "";
        if (col === "class") value = meta.className || "";
        if (col === "condition") value = meta.prereq || "";
        if (col === "note") value = meta.noteText || "";
        cell.textContent = value;
        row.appendChild(cell);
      });

      row.appendChild(createActionCell());
      tbody.appendChild(row);
    });
  }

  function attachCrud(form) {
    const tableId = form.dataset.target;
    const table = document.getElementById(tableId);
    if (!table) return;

    const apiUrl = table.dataset.eduApi;
    const typeOverride = table.dataset.eduType;
    const columns = getColumns(table);
    const saveBtn = form.querySelector('[data-action="save"]');
    const cancelBtn = form.querySelector('[data-action="cancel"]');
    let editingCode = null;
    let editingRow = null;

    async function loadFromApi() {
      if (!apiUrl) return;
      const data = await fetchJson(apiUrl, { method: "GET" });
      const items = Array.isArray(data) ? data : [];
      const filtered = typeOverride
        ? items.filter((item) => String(item.loaiHocPhan || "").toLowerCase() === typeOverride.toLowerCase())
        : items;
      renderTable(table, columns, filtered);
    }

    function buildRow(values) {
      const row = document.createElement("tr");
      row.dataset.major = values.major || "";
      row.dataset.class = values.class || "";
      row.dataset.code = values.code || "";

      columns.forEach((col) => {
        const cell = document.createElement("td");
        cell.textContent = values[col] || "";
        row.appendChild(cell);
      });

      row.appendChild(createActionCell());
      return row;
    }

    if (saveBtn) {
      saveBtn.addEventListener("click", async () => {
        const values = getFormValues(form, columns);
        const hasValue = Object.values(values).some((val) => val);
        if (!hasValue) return;

        if (apiUrl) {
          const payload = {
            maHocPhan: values.code || "",
            tenHocPhan: values.name || "",
            soTinChi: Number(values.credits || 0),
            loaiHocPhan: values.type || typeOverride || "",
            moTa: buildMeta(values),
            chuyenNganh: values.major || ""
          };

          if (editingCode) {
            await fetchJson(`${apiUrl}/${encodeURIComponent(editingCode)}`, {
              method: "PUT",
              body: JSON.stringify(payload)
            });
          } else {
            await fetchJson(apiUrl, {
              method: "POST",
              body: JSON.stringify(payload)
            });
          }

          editingCode = null;
          clearForm(form);
          await loadFromApi();
          return;
        }

        if (editingRow) {
          const cells = editingRow.querySelectorAll("td");
          columns.forEach((col, index) => {
            if (cells[index]) {
              cells[index].textContent = values[col] || "";
            }
          });
          editingRow.dataset.major = values.major || "";
          editingRow.dataset.class = values.class || "";
          editingRow.dataset.code = values.code || editingRow.dataset.code || "";
        } else {
          const row = buildRow(values);
          table.querySelector("tbody")?.appendChild(row);
        }

        editingCode = null;
        editingRow = null;
        clearForm(form);
      });
    }

    if (cancelBtn) {
      cancelBtn.addEventListener("click", () => {
        editingCode = null;
        editingRow = null;
        clearForm(form);
      });
    }

    table.addEventListener("click", async (event) => {
      const target = event.target;
      if (!(target instanceof HTMLElement)) return;

      if (target.matches("[data-edit]")) {
        const row = target.closest("tr");
        if (!row) return;
        if (apiUrl) {
          editingCode = row.dataset.code || "";
        } else {
          editingRow = row;
        }
        fillForm(form, columns, row);
      }

      if (target.matches("[data-delete]")) {
        const row = target.closest("tr");
        if (!row) return;
        const code = row.dataset.code || "";
        if (apiUrl && code) {
          await fetchJson(`${apiUrl}/${encodeURIComponent(code)}`, { method: "DELETE" });
          await loadFromApi();
          return;
        }
        row.remove();
        if (editingCode === code || editingRow === row) {
          editingCode = null;
          editingRow = null;
          clearForm(form);
        }
      }
    });

    loadFromApi();
  }

  function attachFilters(filter) {
    const tableId = filter.dataset.target;
    const table = document.getElementById(tableId);
    if (!table) return;

    const majorSelect = filter.querySelector('[data-filter="major"]');
    const classSelect = filter.querySelector('[data-filter="class"]');

    function applyFilter() {
      const majorValue = majorSelect ? majorSelect.value : "";
      const classValue = classSelect ? classSelect.value : "";

      Array.from(table.querySelectorAll("tbody tr")).forEach((row) => {
        const rowMajor = row.dataset.major || "";
        const rowClass = row.dataset.class || "";
        const matchMajor =
          !majorValue || majorValue === "all" || rowMajor === majorValue || rowMajor === "DC";
        const matchClass = !classValue || classValue === "all" || rowClass === classValue;
        row.style.display = matchMajor && matchClass ? "" : "none";
      });
    }

    if (majorSelect) majorSelect.addEventListener("change", applyFilter);
    if (classSelect) classSelect.addEventListener("change", applyFilter);
    applyFilter();
  }

  document.querySelectorAll("[data-edu-form]").forEach((form) => attachCrud(form));
  document.querySelectorAll("[data-edu-filter]").forEach((filter) => attachFilters(filter));
})();
