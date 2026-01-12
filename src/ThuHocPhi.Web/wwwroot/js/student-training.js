(() => {
  const buttons = document.querySelectorAll("[data-training-filter]");
  const sections = document.querySelectorAll("[data-training-section]");

  if (!buttons.length || !sections.length) return;

  function showSection(filter) {
    sections.forEach((section) => {
      const type = section.getAttribute("data-training-section");
      const visible = filter === "all" || type === filter;
      section.style.display = visible ? "block" : "none";
    });
  }

  buttons.forEach((btn) => {
    btn.addEventListener("click", () => {
      const filter = btn.getAttribute("data-training-filter");
      showSection(filter);
    });
  });
})();
