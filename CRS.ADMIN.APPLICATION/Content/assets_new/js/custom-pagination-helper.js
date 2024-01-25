function LoadPagination(PageSize, TotalData, startIndexVal) {
    const paginationContainer = document.getElementById('pagination-id');
    const prevButton = document.getElementById('prev-btn');
    const nextButton = document.getElementById('next-btn');
    const pageSize = PageSize;
    const TotalData = TotalData;
    const startIndexVal = startIndexVal;
    //const pageSize = @ViewBag.PageSize;
    //const TotalData = @ViewBag.TotalData;
    //const startIndexVal = @ViewBag.StartIndex;
    const currentPage = Math.ceil((startIndexVal + 1) / pageSize);
    document.addEventListener('DOMContentLoaded', function () {
        const TotalPages = Math.ceil(TotalData / pageSize);
        displayPagination(TotalPages);
        RangeOfEntries();
        ShowEntriesFunction();

        function displayPagination(totalPages) {
            if (currentPage === 1) {
                prevButton.disabled = true;
                prevButton.style.cursor = 'not-allowed';
            } else {
                if (currentPage > 1) {
                    const prevButtonVal = currentPage - 1;
                    prevButton.setAttribute("onclick", "RedirectFunction(" + prevButtonVal + ")");
                }
            }

            if (currentPage === totalPages) {
                nextButton.disabled = true;
                nextButton.style.cursor = 'not-allowed';
            } else {
                if (currentPage < totalPages) {
                    const nextButtonVal = currentPage + 1;
                    nextButton.setAttribute("onclick", "RedirectFunction(" + nextButtonVal + ")");
                }
            }

            let paginationHTMLContent = ``;
            const maxVisibleButtons = 5;
            let startPage = Math.max(1, currentPage - Math.floor(maxVisibleButtons / 2));
            let endPage = Math.min(totalPages, startPage + maxVisibleButtons - 1);

            if (endPage - startPage < maxVisibleButtons - 1) {
                startPage = Math.max(1, endPage - maxVisibleButtons + 1);
            }

            if (startPage > 1) {
                paginationHTMLContent += createPageButton(1);
                if (startPage > 2) {
                    paginationHTMLContent += createSeparator();
                }
            }

            for (let i = startPage; i <= endPage; i++) {
                paginationHTMLContent += createPageButton(i);
            }

            if (endPage < totalPages) {
                if (endPage < totalPages - 1) {
                    paginationHTMLContent += createSeparator();
                }
                paginationHTMLContent += createPageButton(totalPages);
            }
            paginationContainer.innerHTML = paginationHTMLContent;
        }

        function createPageButton(page) {
            let pageButton = ``;
            if (page === currentPage) {
                pageButton += `<div class="pagination-number active" onclick="RedirectFunction(` + page + `);" >` + page + `</div > `;
            } else {
                pageButton += `<div class="pagination-number" onclick="RedirectFunction(` + page + `);" >` + page + `</div > `;
            }
            return pageButton;
        }

        function createSeparator() {
            const separator = `<div class="pagination-number" >...</div > `;
            return separator;
        }
    });
}

function RedirectFunction(currentPage) {
    if (pageSize < 1)
        pageSize = 1;

    if (currentPage < 1)
        currentPage = 1;

    const startIndex = (currentPage - 1) * pageSize;
    window.location.href = `/CustomerManagement/CustomerList?StartIndex=${startIndex}&PageSize=${pageSize}`;
};

// Display information about the range of entries
function RangeOfEntries() {
    const entriesContainer = document.getElementById('Entries-Id');
    const startEntry = (currentPage - 1) * pageSize + 1;
    const endEntry = Math.min(currentPage * pageSize, TotalData);
    entriesContainer.innerHTML = '';
    entriesContainer.innerHTML = ` @CRS.ADMIN.APPLICATION.Resources.Resource.Showing ${startEntry} to ${endEntry} of ${TotalData}  @CRS.ADMIN.APPLICATION.Resources.Resource.Entries`;
}

// Display information about the range of entries
function ShowEntriesFunction() {
    const showEntriesContainer = document.getElementById('ShowEntries-Id');
    showEntriesContainer.innerHTML = '';
    showEntriesContainer.innerHTML = `
                         @CRS.ADMIN.APPLICATION.Resources.Resource.Show&nbsp;
                         <select id="countries" class="w-full select-entires" style="width: 100%;" onchange="handleSelectChange(this)">
                             ${[5, 10, 20, 50, 100].map(value => `<option value="${value}"${value === pageSize ? ' selected' : ''}>${value}</option>`).join('')}
                         </select>@CRS.ADMIN.APPLICATION.Resources.Resource.Entries`;
}

function handleSelectChange(selectElement) {
    window.location.href = `/CustomerManagement/CustomerList?StartIndex=${0}&PageSize=${selectElement.value}`;
}