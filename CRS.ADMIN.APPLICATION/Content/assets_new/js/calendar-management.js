const currentDate = document.querySelector(".current-date"),
    daysTag = document.querySelector(".days-class");

//get new date, current year and month
let date = new Date(),
    currYear = date.getFullYear(),
    currMonth = date.getMonth();

const months = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"],
    daysOfWeek = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'];

const renderCalendar = () => {
    let firstDayofMonth = new Date(currYear, currMonth, 1).getDay(), // getting first day of month
        lastDateofMonth = new Date(currYear, currMonth + 1, 0).getDate(), // getting last date of month
        lastDayofMonth = new Date(currYear, currMonth, lastDateofMonth).getDay(), // getting last day of month
        lastDateofLastMonth = new Date(currYear, currMonth, 0).getDate(); // getting last date of previous month
    let htmlText = "";
    for (let i = firstDayofMonth; i > 0; i--) {
        var date1 = lastDateofLastMonth - i + 1;
        var dayOfWeek = new Date(currYear, currMonth, date1).getDay();
        htmlText += `<div class="calender-card disabled">
                    <div class="text-xs font-medium text-center">
                        <div>${daysOfWeek[dayOfWeek]}</div>
                    </div>
                        <div class="calender-number">${date1}</div>
                </div>`;
    }
    for (let i = 1; i <= lastDateofMonth; i++) { // creating li of all days of current month
        let isToday = i === date.getDate() && currMonth === new Date().getMonth()
            && currYear === new Date().getFullYear() ? " active " : "";
        var dayOfWeek = new Date(currYear, currMonth, i).getDay();
        htmlText += `<div class="calender-card ${isToday}">
                    <div class="text-xs font-medium text-center">
                        <div>${daysOfWeek[dayOfWeek]}</div>
                    </div>
                        <div class="calender-number">${i}</div>
                </div>`;
    }
    for (let i = lastDayofMonth; i < 6; i++) { // creating li of next month first days
        var date2 = i - lastDayofMonth + 1;
        var dayOfWeek2 = new Date(currYear, currMonth, date2).getDay();
        htmlText += `<div class="calender-card disabled">
                    <div class="text-xs font-medium text-center">
                        <div>${daysOfWeek[dayOfWeek2]}</div>                        
                    </div>
                    <div class="calender-number">${date2}</div>
                </div>`;
    }
    currentDate.innerHTML = `${months[currMonth]} ${currYear}`;
    daysTag.innerHTML = htmlText;
}
renderCalendar();

document.getElementById("prev-id").onclick = function () {
    currMonth = currMonth - 1;
    if (currMonth < 0 || currMonth > 11) { // if current month is less than 0 or greater than 11
        // creating a new date of current year & month and pass it as date value
        date = new Date(currYear, currMonth, new Date().getDate());
        currYear = date.getFullYear(); // updating current year with new date year
        currMonth = date.getMonth(); // updating current month with new date month
    } else {
        date = new Date(); // pass the current date as date value
    }
    renderCalendar();
}
document.getElementById("next-id").onclick = function () {
    currMonth = currMonth + 1;
    if (currMonth < 0 || currMonth > 11) { // if current month is less than 0 or greater than 11
        // creating a new date of current year & month and pass it as date value
        date = new Date(currYear, currMonth, new Date().getDate());
        currYear = date.getFullYear(); // updating current year with new date year
        currMonth = date.getMonth(); // updating current month with new date month
    } else {
        date = new Date(); // pass the current date as date value
    }
    renderCalendar();
}
