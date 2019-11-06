function dismiss_modal() {
    $("#close_button").click();
    $(".close_button").click();
    onAjaxCompleted();
}

function clean_modal() {
    $(".modal-backdrop").remove();
    $("body").removeClass("modal-open");
    onAjaxSuccess();
}
