(() => {
  const token = localStorage.getItem("thuhocphi_token");
  const btn = document.getElementById("btn-duyet-hoc-bong");
  const output = document.getElementById("hb-output");

  if (!btn) return;

  btn.addEventListener("click", async () => {
    const maSV = document.getElementById("hb-ma-sv")?.value.trim();
    const maHocBong = document.getElementById("hb-ma-hoc-bong")?.value.trim();
    const maHocKy = document.getElementById("hb-ma-hoc-ky")?.value.trim();
    const ghiChu = document.getElementById("hb-ghi-chu")?.value.trim();

    if (!maSV || !maHocBong || !maHocKy) {
      if (output) output.textContent = "Vui lòng nhập đủ Mã SV, Mã học bổng và Mã học kỳ.";
      return;
    }

    const payload = { maSV, maHocBong, maHocKy, ghiChu: ghiChu || null };

    try {
      const response = await fetch("/api/hoc-bong/ap-dung", {
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
          ? `Phê duyệt học bổng thành công:\n${content}`
          : `Phê duyệt thất bại:\n${content}`;
      }
    } catch (error) {
      if (output) output.textContent = `Lỗi kết nối: ${error.message}`;
    }
  });
})();
