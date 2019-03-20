function displayToastrSuccess() {
    setToasterOptionsCommon();
    toastr["success"]("", "Success!");
}

function displayToastrError() {
    setToasterOptionsCommon();
    toastr["error"]("Please contact support for assistance.", "Error");
}

function setToasterOptionsCommon() {
    toastr.options = {
        "closeButton": false,
        "debug": false,
        "progressBar": false,
        "preventDuplicates": true,
        "positionClass": "toast-bottom-left",
        "onclick": null,
        "showDuration": "400",
        "hideDuration": "1000",
        "timeOut": "2000",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    };
}