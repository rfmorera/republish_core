function displayToastrSuccess() {
    setToasterOptionsCommon();
    toastr["success"]("", "Éxito!");
}

function displayToastrError() {
    setToasterOptionsCommon();
    toastr["error"]("Por favor contacte al administrador para asistencia.", "Error");
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