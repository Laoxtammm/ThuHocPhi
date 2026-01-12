(() => {
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

  function attachCrud(form) {
    const tableId = form.dataset.target;
    const table = document.getElementById(tableId);
    if (!table) return;

    const columns = getColumns(table);
    const saveBtn = form.querySelector('[data-action="save"]');
    const cancelBtn = form.querySelector('[data-action="cancel"]');
    let editingRow = null;

    function buildRow(values) {
      const row = document.createElement("tr");
      row.dataset.major = values.major || "";
      row.dataset.class = values.class || "";

      columns.forEach((col) => {
        const cell = document.createElement("td");
        cell.textContent = values[col] || "";
        row.appendChild(cell);
      });

      row.appendChild(createActionCell());
      return row;
    }

    if (saveBtn) {
      saveBtn.addEventListener("click", () => {
        const values = getFormValues(form, columns);
        const hasValue = Object.values(values).some((val) => val);
        if (!hasValue) return;

        if (editingRow) {
          const cells = editingRow.querySelectorAll("td");
          columns.forEach((col, index) => {
            if (cells[index]) {
              cells[index].textContent = values[col] || "";
            }
          });
          editingRow.dataset.major = values.major || "";
          editingRow.dataset.class = values.class || "";
        } else {
          const row = buildRow(values);
          table.querySelector("tbody")?.appendChild(row);
        }

        editingRow = null;
        clearForm(form);
      });
    }

    if (cancelBtn) {
      cancelBtn.addEventListener("click", () => {
        editingRow = null;
        clearForm(form);
      });
    }

    table.addEventListener("click", (event) => {
      const target = event.target;
      if (!(target instanceof HTMLElement)) return;

      if (target.matches("[data-edit]")) {
        const row = target.closest("tr");
        if (!row) return;
        editingRow = row;
        fillForm(form, columns, row);
      }

      if (target.matches("[data-delete]")) {
        const row = target.closest("tr");
        if (!row) return;
        row.remove();
        if (editingRow === row) {
          editingRow = null;
          clearForm(form);
        }
      }
    });
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
