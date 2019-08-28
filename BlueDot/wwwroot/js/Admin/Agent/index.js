$(document).ready(initializePage);

function onAddSuccess() {
    clean_modal();
    initializePage();
}

function onDeleteSuccess() {
    onAjaxSuccess();
    $("#" + DeleteRowId).remove();
}

function initializePage() {
    initializeDataTable($("#dataTables-table"));

    $("tr").each(function (index, element) { $(element).attr("id", index); });

    $('#form1').formValidation({
        framework: 'bootstrap',
        icon: {
            valid: 'glyphicon glyphicon-ok',
            invalid: 'glyphicon glyphicon-remove',
            validating: 'glyphicon glyphicon-refresh'
        },
        fields: {
            UserName: {
                validators: {
                    notEmpty: {
                        message: 'Por favor escriba el correo'
                    },
                }
            }
        }
    });
}