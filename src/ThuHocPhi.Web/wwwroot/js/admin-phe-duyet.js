(() => {
  const token = localStorage.getItem("thuhocphi_token");

  const mgBtn = document.getElementById("btn-duyet-mien-giam");
  const mgOutput = document.getElementById("mg-output");
  const hbBtn = document.getElementById("btn-duyet-hoc-bong");
  const hbOutput = document.getElementById("hb-output");

  async function postData(url, payload) {
    const response = await fetch(url, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        ...(token ? { Authorization: `Bearer ${token}` } : {}),
      },
      credentials: "same-origin",
      body: JSON.stringify(payload),
    });

    const content = await response.text();
    return { ok: response.ok, content };
  }

  if (mgBtn) {
    mgBtn.addEventListener("click", async () => {
      const maSV = document.getElementById("mg-ma-sv")?.value.trim();
      const maChinhSach = document.getElementById("mg-ma-chinh-sach")?.value.trim();
      const maHocKy = document.getElementById("mg-ma-hoc-ky")?.value.trim();
      const ghiChu = document.getElementById("mg-ghi-chu")?.value.trim();

      if (!maSV || !maChinhSach || !maHocKy) {
        if (mgOutput) mgOutput.textContent = "Vui lòng nhập đủ Mã SV, Mã chính sách và Mã học kỳ.";
        return;
      }

      const payload = { maSV, maChinhSach, maHocKy, ghiChu: ghiChu || null };

      try {
        const result = await postData("/api/mien-giam/ap-dung", payload);
        if (mgOutput) {
          mgOutput.textContent = result.ok
            ? `Phê duyệt miễn giảm thành công:\n${result.content}`
            : `Phê duyệt thất bại:\n${result.content}`;
        }
      } catch (error) {
        if (mgOutput) mgOutput.textContent = `Lỗi kết nối: ${error.message}`;
      }
    });
  }

  if (hbBtn) {
    hbBtn.addEventListener("click", async () => {
      const maSV = document.getElementById("hb-ma-sv")?.value.trim();
      const maHocBong = document.getElementById("hb-ma-hoc-bong")?.value.trim();
      const maHocKy = document.getElementById("hb-ma-hoc-ky")?.value.trim();
      const ghiChu = document.getElementById("hb-ghi-chu")?.value.trim();

      if (!maSV || !maHocBong || !maHocKy) {
        if (hbOutput) hbOutput.textContent = "Vui lòng nhập đủ Mã SV, Mã học bổng và Mã học kỳ.";
        return;
      }

      const payload = { maSV, maHocBong, maHocKy, ghiChu: ghiChu || null };

      try {
        const result = await postData("/api/hoc-bong/ap-dung", payload);
        if (hbOutput) {
          hbOutput.textContent = result.ok
            ? `Phê duyệt học bổng thành công:\n${result.content}`
            : `Phê duyệt thất bại:\n${result.content}`;
        }
      } catch (error) {
        if (hbOutput) hbOutput.textContent = `Lỗi kết nối: ${error.message}`;
      }
    });
  }
})();
