const currentDate = document.querySelector(".current-date"),
    daysTag = document.querySelector(".days-class"),
    ConstantCurrentMonth = new Date().getMonth(),
    ConstCurrentDate = new Date(),
    ConstYear = ConstCurrentDate.getFullYear(),
    ConstMonth = (ConstCurrentDate.getMonth() + 1).toString().padStart(2, '0'),
    ConstDay = ConstCurrentDate.getDate().toString().padStart(2, '0'),
    ConstFormattedDate = `${ConstYear}-${ConstMonth}-${ConstDay}`;

let idValue = 1; //dynamic id value
var Clubschedule = clubSchedulesJson;
//get new date, current year and month
let date = new Date(),
    currYear = date.getFullYear(),
    currMonth = date.getMonth();

const months = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"],
    daysOfWeek = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];

const renderCalendar = () => {
    let firstDayofMonth = new Date(currYear, currMonth, 1).getDay(), // getting first day of month
        lastDateofMonth = new Date(currYear, currMonth + 1, 0).getDate(), // getting last date of month
        lastDayofMonth = new Date(currYear, currMonth, lastDateofMonth).getDay(), // getting last day of month
        lastDateofLastMonth = new Date(currYear, currMonth, 0).getDate(); // getting last date of previous month
    let htmlText = "";
    for (let i = firstDayofMonth; i > 0; i--) {
        var date1 = lastDateofLastMonth - i + 1;
        var dayOfWeek = new Date(currYear, currMonth, date1).getDay();
        htmlText += ` <div class="calender-card-schedule disabled">
                <div class=" text-center">
                    <div>${daysOfWeek[dayOfWeek]}</div>
                    <div class="date-number">${date1}</div>
                </div>`;
        let currentFullDate = `${currYear}-${currMonth}-${date1}`;
        const matchingSchedule = Clubschedule.find(item => item.DateValue === currentFullDate);
        if (matchingSchedule) {
            htmlText += `
                <div class="calender-number-schedule">
                    <img src="/Content/assets_new/images/${matchingSchedule.ClubSchedule}.svg" />
                </div>`;
        }
        else {
            htmlText += `
                <div class="calender-number-schedule">
                    <img src="/Content/assets_new/images/unreservable.svg" />
                </div>`;
        }
        htmlText += `</div>`;
    }
    for (let i = 1; i <= lastDateofMonth; i++) { // creating li of all days of current month
        let currentFullDate = `${currYear}-${currMonth + 1}-${i}`;
        const matchingSchedule = Clubschedule.find(item => item.DateValue === currentFullDate);
        let ScheduleId = "";
        if (matchingSchedule) {
            ScheduleId = matchingSchedule.ScheduleId;
        }
        let isToday = i === date.getDate() && currMonth === new Date().getMonth()
            && currYear === new Date().getFullYear() ? " active " : "";
        //console.log(currentFullDate);
        let isDisabled = ((i >= date.getDate() && currMonth === new Date().getMonth()
            && currYear === new Date().getFullYear()) || (new Date(currentFullDate) > new Date(ConstFormattedDate))) ? false : true;
        var dayOfWeek = new Date(currYear, currMonth, i).getDay();
        //<button onclick="openModal('modal1')">Open Modal 1</button>
        if (!isDisabled) {
            htmlText += `<div class="calender-card-schedule${isToday}" onclick="openModal('modal1','${currentFullDate},${ScheduleId}')">
                <div class=" text-center">
                    <div>${daysOfWeek[dayOfWeek]}</div>
                    <div class="date-number">${i}</div>
                </div>`;
            idValue++;
        }
        else {
            htmlText += `<div class="calender-card-schedule disabled">
                <div class=" text-center">
                    <div>${daysOfWeek[dayOfWeek]}</div>
                    <div class="date-number">${i}</div>
                </div>`;
        }

        if (matchingSchedule) {
            htmlText += `
                <div class="calender-number-schedule">
                    <img src="/Content/assets_new/images/${matchingSchedule.ClubSchedule}.svg" />
                </div>`;
        }
        else {
            htmlText += `
                <div class="calender-number-schedule">
                    <img src="/Content/assets_new/images/unreservable.svg" />
                </div>`;
        }
        htmlText += `</div>`;
    }
    for (let i = lastDayofMonth; i < 6; i++) { // creating li of next month first days
        var date2 = i - lastDayofMonth + 1;
        var dayOfWeek2 = new Date(currYear, currMonth, date2).getDay();
        htmlText += ` <div class="calender-card-schedule disabled">
                <div class=" text-center">
                    <div>${daysOfWeek[dayOfWeek2]}</div>
                    <div class="date-number">${date2}</div>
                </div>`;
        let currentFullDate = `${currYear}-${currMonth}-${date2}`;
        const matchingSchedule = Clubschedule.find(item => item.DateValue === currentFullDate);
        if (matchingSchedule) {
            htmlText += `
                <div class="calender-number-schedule">
                    <img src="/Content/assets_new/images/${matchingSchedule.ClubSchedule}.svg" />
                </div>`;
        }
        else {
            htmlText += `
                <div class="calender-number-schedule">
                    <img src="/Content/assets_new/images/unreservable.svg" />
                </div>`;
        }
        htmlText += `</div>`;
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
