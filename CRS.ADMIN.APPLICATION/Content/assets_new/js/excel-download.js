function exportExcel(table_id, file_name) {
    const currentDateTimeString = new Date().toISOString().replace('T', ' ').replace(/\.\d{3}Z$/, '');
    let originalTable = document.getElementById(table_id);

    // Clone the original table
    let tableCopy = originalTable.cloneNode(true);

    // Remove the "Action" column from the cloned table
    let headerRows = tableCopy.querySelectorAll("thead tr");
    headerRows.forEach(row => {
        let columns = row.querySelectorAll("th");
        columns.forEach(column => {
            if (column.textContent.trim() === "Action" || column.textContent.trim() === "アクション") {
                // Find the column index and remove it
                let columnIndex = column.cellIndex;
                tableCopy.querySelectorAll("tr").forEach(row => row.deleteCell(columnIndex));
            } else {
                column.textContent = column.textContent.replace(/Font Awesome fontawesome\.com -->/g, '').trim();
            }
        });
    });

    // Convert the modified table to Excel
    TableToExcel.convert(tableCopy, {
        name: currentDateTimeString + `_Hoslog_` + file_name + `.xlsx`
    });
}
