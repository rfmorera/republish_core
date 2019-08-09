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
    initializePage();
}

function initializePage() {
    $('#form2').formValidation({
        framework: 'bootstrap',
        icon: {
            valid: 'glyphicon glyphicon-ok',
            invalid: 'glyphicon glyphicon-remove',
            validating: 'glyphicon glyphicon-refresh'
        },
        fields: {
            ApiKey: {
                validators: {
                    notEmpty: {
                        message: 'Por favor entre el valor del ApiKey'
                    },
                }
            },
        }
    });
}