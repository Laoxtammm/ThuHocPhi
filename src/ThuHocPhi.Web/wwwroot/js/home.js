(() => {
  const slider = document.getElementById("hero-slider");
  if (!slider) return;

  const images = [
    "/pictures/slider-1.jpg",
    "/pictures/slider-2.jpg",
    "/pictures/slider-3.jpg",
  ];
  let current = 0;

  function setBackground(url) {
    slider.style.backgroundImage = `url('${url}')`;
    slider.style.backgroundSize = "cover";
    slider.style.backgroundPosition = "center";
  }

  function loadImage(url) {
    return new Promise((resolve) => {
      const img = new Image();
      img.onload = () => resolve(true);
      img.onerror = () => resolve(false);
      img.src = url;
    });
  }

  async function rotate() {
    const url = images[current];
    const ok = await loadImage(url);
    if (ok) {
      setBackground(url);
    } else {
      slider.style.backgroundImage = "linear-gradient(120deg, rgba(14, 27, 31, 0.85), rgba(15, 118, 110, 0.55))";
    }
    current = (current + 1) % images.length;
  }

  rotate();
  setInterval(rotate, 5000);
})();
