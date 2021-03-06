﻿$(document).ready(initializePage);

function onSaveSuccess() {
    clean_modal();
    initializePage();
}

function onAddSuccess() {
    clean_modal();
    initializePage();
}

function onDeleteSuccess() {
    onAjaxSuccess();
    $("#" + DeleteRowId).remove();
}

function initializePage() {
    $(".btn-update").on("click", function () {
        Id = $(this).attr("data-Id");
        Account = $(this).attr("data-Account");

        $("#Id").val(Id);
        $("#Account-Actualizar").val(Account);
    });

    $("tr").each(function (index, element) { $(element).attr("id", index); });

    $('#form2').formValidation({
        framework: 'bootstrap',
        icon: {
            valid: 'glyphicon glyphicon-ok',
            invalid: 'glyphicon glyphicon-remove',
            validating: 'glyphicon glyphicon-refresh'
        },
        fields: {
            Account: {
                validators: {
                    notEmpty: {
                        message: 'Por favor entre el valor del ApiKey'
                    }
                }
            },
            Key: {
                validators: {
                    notEmpty: {
                        message: 'Por favor entre el valor del ApiKey'
                    }
                }
            }
        }
    });
}
