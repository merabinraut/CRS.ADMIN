function showNotification(message, title, type) {

    toastr.options = {
        "closeButton": true,
        "debug": false,
        "newestOnTop": false,
        "progressBar": true,
        "positionClass": "toast-top-right",
        "preventDuplicates": false,
        "onclick": null,
        "showDuration": "300",
        "hideDuration": "1000",
        "timeOut": "5000",
        "extendedTimeOut": "1000",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    }

    switch (type) {
        case "SUCCESS":
            toastr.success(message, title);
            break;
        case "ERROR":
            toastr.error(message, title);
            break;
        case "INFORMATION":
            toastr.info(message, title);
            break;
        case "WARNING":
            toastr.warning(message, title);
            break;
    }
}

function showConfirmation(message, callback) {
    toastr.options = {
        "closeButton": true,
        "debug": false,
        "newestOnTop": false,
        "progressBar": false,
        "positionClass": "toast-top-right",
        "preventDuplicates": false,
        "onclick": function () {
            if (typeof callback === 'function') {
                callback(true);
            }
        },
        "showDuration": "300",
        "hideDuration": "0",
        "timeOut": "0",
        "extendedTimeOut": "0",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut",
        "tapToDismiss": false
    };

    toastr.info(`${message} <br/> <button class="btn-confirm">Yes</button>`, "INFORMATION");

    $('.btn-confirm').click(function () {
        toastr.clear();
        if (typeof callback === 'function') {
            callback(true);
        }
    });
}
