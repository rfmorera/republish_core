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
    $(".btn-recargar").on("click", function () {
        id = $(this).attr("data-Id");
        Username = $(this).attr("data-Username");

        $("#RecargarId").val(id);
        $("#UserNameRecarga").val(Username);
    });

    $('#form1').formValidation({
        framework: 'bootstrap',
        icon: {
            valid: 'glyphicon glyphicon-ok',
            invalid: 'glyphicon glyphicon-remove',
            validating: 'glyphicon glyphicon-refresh'
        },
        fields: {
            Email: {
                validators: {
                    notEmpty: {
                        message: 'Please enter an email'
                    }
                }
            }
        }
    });
}
