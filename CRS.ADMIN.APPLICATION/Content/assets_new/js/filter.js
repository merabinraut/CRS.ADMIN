function showFilterContainer() {
    var filterDiv = document.getElementById("filter");
    var filterContainer = document.getElementById("filter-container");

    // Hide the filter div
    filterDiv.style.display = "none";

    // Show the filter container
    filterContainer.style.display = "block";

    // Add animation classes
    filterContainer.classList.add("slide-in-animation");
}

function hideFilterContainer() {
    var filterDiv = document.getElementById("filter");
    var filterContainer = document.getElementById("filter-container");
    if (!filterDiv || !filterContainer) return;

    filterContainer.style.display = "none";
    filterDiv.style.display = "flex";
    filterDiv.classList.add("slide-in-animation");
}
var filterContainerEl = document.getElementById("filter-container");
if (filterContainerEl) {
    filterContainerEl.addEventListener("animationend", function () {
        this.classList.remove("slide-in-animation");
    });
}

var filterEl = document.getElementById("filter");
if (filterEl) {
    filterEl.addEventListener("animationend", function () {
        this.classList.remove("slide-in-animation");
    });
}