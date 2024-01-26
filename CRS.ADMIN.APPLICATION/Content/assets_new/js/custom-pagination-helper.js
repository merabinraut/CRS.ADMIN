let pageSize = 0;
let TotalData = 0;
let startIndexVal = 0;
let URL = "#";
function LoadPagination(PageSize, Total_Data, start_IndexVal, url, pagination_container_id = "pagination-id",
    prev_button_id = "prev-btn", next_button_id = "next-btn", show_entries_container_id = "ShowEntries-Id", entries_container_id = "Entries-Id") {
    const paginationContainer = document.getElementById(pagination_container_id);
    const prevButton = document.getElementById(prev_button_id);
    const nextButton = document.getElementById(next_button_id);
    const showEntriesContainer = document.getElementById(show_entries_container_id);
    const entriesContainer = document.getElementById(entries_container_id);
    pageSize = PageSize;
    TotalData = Total_Data;
    startIndexVal = start_IndexVal;
    URL = url;
    const currentPage = Math.ceil((startIndexVal + 1) / pageSize);
    const TotalPages = Math.ceil(TotalData / pageSize);
    if (TotalData > 0) {
        ShowEntriesFunction();
        displayPagination(TotalPages);
        RangeOfEntries();
    }
    else {
        prevButton.disabled = true;
        prevButton.style.cursor = 'not-allowed';
        nextButton.disabled = true;
        nextButton.style.cursor = 'not-allowed';
    }
    function displayPagination(totalPages) {
        if (currentPage === 1) {
            prevButton.disabled = true;
            prevButton.style.cursor = 'not-allowed';
        } else {
            if (currentPage > 1) {
                const prevButtonVal = currentPage - 1;
                prevButton.setAttribute("onclick", `RedirectFunction(${prevButtonVal}, '${dynamicStartIndexLabel}', '${dynamicPageSizeLabel}', ${pageSize})`);
            }
        }

        if (currentPage === totalPages) {
            nextButton.disabled = true;
            nextButton.style.cursor = 'not-allowed';
        } else {
            if (currentPage < totalPages) {
                const nextButtonVal = currentPage + 1;
                nextButton.setAttribute("onclick", `RedirectFunction( ${nextButtonVal} , '${dynamicStartIndexLabel}', '${dynamicPageSizeLabel}' , ${pageSize})`);
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
            pageButton += `<div class="pagination-number active">` + page + `</div > `;
        } else {
            pageButton += `<div class="pagination-number" onclick="RedirectFunction(${page}, '${dynamicStartIndexLabel}', '${dynamicPageSizeLabel}', ${pageSize});" >` + page + `</div > `;
        }
        return pageButton;
    }

    function createSeparator() {
        const separator = `<div class="pagination-number" >...</div > `;
        return separator;
    }

    // Display information about the range of entries
    function RangeOfEntries() {
        const startEntry = (currentPage - 1) * pageSize + 1;
        const endEntry = Math.min(currentPage * pageSize, TotalData);
        entriesContainer.innerHTML = '';
        if ((ShowingDynamicLabel != "" || ShowingDynamicLabel != null) && (EntriesDynamicLabel != "" || EntriesDynamicLabel != null)) {
            entriesContainer.innerHTML = `${ShowingDynamicLabel} ${startEntry} to ${endEntry} of ${TotalData} ${EntriesDynamicLabel}`;
        }
        else {
            entriesContainer.innerHTML = `Showing ${startEntry} to ${endEntry} of ${TotalData} Entries`;
        }
    }

    // Display information about the range of entries
    function ShowEntriesFunction() {
        showEntriesContainer.innerHTML = '';
        if ((ShowDynamicLabel != "" || ShowDynamicLabel != null) && (EntriesDynamicLabel != "" || EntriesDynamicLabel != null)) {
            showEntriesContainer.innerHTML = `<div class="w-max flex-none"> ${ShowDynamicLabel}</div>&nbsp;
                         <select id="countries" class="select-entires" style="width: 100%;"onchange="handleSelectChange(this, '${dynamicStartIndexLabel}', '${dynamicPageSizeLabel}')">
                             ${[5, 10, 20, 50, 100].map(value => `<option value="${value}"${value === pageSize ? ' selected' : ''}>${value}</option>`).join('')}
                         </select> <div class="w-max flex-none pl-1">  ${EntriesDynamicLabel} </div>`;
        }
        else {
            showEntriesContainer.innerHTML = `<div class="w-max flex-none"> Show</div>&nbsp;
                         <select id="countries" class="select-entires" style="width: 100%;"onchange="handleSelectChange(this, '${dynamicStartIndexLabel}', '${dynamicPageSizeLabel}')">
                             ${[5, 10, 20, 50, 100].map(value => `<option value="${value}"${value === pageSize ? ' selected' : ''}>${value}</option>`).join('')}
                         </select> <div class="w-max flex-none pl-1">Entries</div>`;
        }
    }
}

function RedirectFunction(currentPage, i, j, page_size) {
    if (page_size < 1)
        page_size = 1;

    if (currentPage < 1)
        currentPage = 1;

    const startIndex = (currentPage - 1) * page_size;
    let separator = URL.includes('?') ? '&' : '?';
    window.location.href = URL + separator + `${i}=${startIndex}&${j}=${page_size}`;
};

function handleSelectChange(selectElement, i, j) {
    let separator = URL.includes('?') ? '&' : '?';
    window.location.href = URL + separator + `${i}=${0}&${j}=${selectElement.value}`;
}

function addQueryParam(url, paramName, paramValue) {
    if (paramValue != null && paramValue != "") {
        // Check if it's the first parameter, if so, use "?" else use "&"
        url += url.includes('?') ? `&${paramName}=${paramValue}` : `?${paramName}=${paramValue}`;
    }
    if (URL.includes('?')) {
        URL = URL.replace('?', '');
    }
    return url;
}
