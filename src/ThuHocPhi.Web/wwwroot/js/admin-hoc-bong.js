(() => {
  const token = localStorage.getItem("thuhocphi_token");
  const btn = document.getElementById("btn-duyet-hoc-bong");
  const output = document.getElementById("hb-output");

  if (!btn) return;

  btn.addEventListener("click", async () => {
    const maSV = document.getElementById("hb-ma-sv")?.value.trim();
    const maHocBong = document.getElementById("hb-ma-hoc-bong")?.value.trim();
    const maHocKy = document.getElementById("hb-ma-hoc-ky")?.value.trim();
    const khoaDaoTao = document.getElementById("hb-khoa-dao-tao")?.value.trim();
    const lopChuyenNganh = document.getElementById("hb-lop-chuyen-nganh")?.value.trim();
    const ghiChu = document.getElementById("hb-ghi-chu")?.value.trim();

    if (!maSV || !maHocBong || !maHocKy) {
      if (output) output.textContent = "Vui long nhap du Ma SV, Ma hoc bong va Ma hoc ky.";
      return;
    }

    const payload = {
      maSV,
      maHocBong,
      maHocKy,
      khoaDaoTao: khoaDaoTao || null,
      lopChuyenNganh: lopChuyenNganh || null,
      ghiChu: ghiChu || null,
    };

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
          ? `Phe duyet hoc bong thanh cong:\n${content}`
          : `Phe duyet that bai:\n${content}`;
      }
    } catch (error) {
      if (output) output.textContent = `Loi ket noi: ${error.message}`;
    }
  });
})();
