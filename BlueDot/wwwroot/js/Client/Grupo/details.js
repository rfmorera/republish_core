$(document).ready(initializePage);

function onAddSuccess() {
    clean_modal();
    initializePage();
}

function onDeleteSuccess() {
    onAjaxSuccess();
    initializePage();
}

function initializePage() {
    $('.clockpicker').clockpicker();
    initializeDataTable($("#dataTables-table"));
    initializeDataTable($("#dataTables-table-temporizador"));
    $("a.delete-all-anuncios-button").off("click").on("click", function (e) {
        e.preventDefault();
        var form = prepareFormCustom($(this));

        if (form) {
            showConfirmationPopupCustom(form);
        }
    });
    $("a.delete-all-temporizadores-button").off("click").on("click", function (e) {
        e.preventDefault();
        var form = prepareFormCustom($(this));

        if (form) {
            showConfirmationPopupTemporizadoresCustom(form);
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
function showConfirmationPopupTemporizadoresCustom(form) {
    swal({
        title: "¿Está seguro?",
        text: "Por favor confirme que usted desea eliminar todos los temporizadores de este grupo",
        type: "warning",
        confirmButtonColor: "#DD6B55",
        showCancelButton: true
    }, function () {
        form.submit();
    });
}