$(document).ready(initializePage);

function initializePage() {
    $("a.delete-all-button").off("click").on("click", function (e) {
        e.preventDefault();
        var form = prepareFormCustom($(this));

        if (form) {
            showConfirmationPopupCustom(form);
        }
    });
}

function prepareFormCustom(anchorSelector) {
    var form;
    var formId = anchorSelector.attr("data-form-id");

    if (formId) {
        form = $("#" + formId);
        form.attr("action", anchorSelector.attr("href"));
    }

    return form;
}

function showConfirmationPopupCustom(form) {
    swal({
        title: "¿Está seguro?",
        text: "Por favor confirme que usted desea eliminar todos los anuncios de este grupo",
        type: "warning",
        confirmButtonColor: "#DD6B55",
        showCancelButton: true
    }, function () {
        form.submit();
    });
}