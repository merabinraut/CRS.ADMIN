function exportExcel(table_id, file_name) {
    const currentDateTimeString = new Date().toISOString().replace('T', ' ').replace(/\.\d{3}Z$/, '');
    let table = document.getElementById(table_id);
    let headerRows = table.querySelectorAll("thead tr");
    headerRows.forEach(row => {
        let columns = row.querySelectorAll("th");
        columns.forEach(column => {
            column.textContent = column.textContent.replace(/Font Awesome fontawesome\.com -->/g, '').trim();
        });
    });
    TableToExcel.convert(table, {
        name: currentDateTimeString + `_Hoslog_` + file_name + `.xlsx`
    });
}