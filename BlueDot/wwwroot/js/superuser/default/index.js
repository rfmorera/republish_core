﻿$(document).ready(initializePage);

function onSaveSuccess() {
    onAjaxSuccess();
    initializePage();
}

function initializePage() {
    $('#form1').formValidation({
        framework: 'bootstrap',
        icon: {
            valid: 'glyphicon glyphicon-ok',
            invalid: 'glyphicon glyphicon-remove',
            validating: 'glyphicon glyphicon-refresh'
        },
        fields: {
            FirmName: {
                validators: {
                    notEmpty: {
                        message: 'A Name is required'
                    }
                }
            },
            ClientLoginURL: {
                validators: {
                    notEmpty: {
                        message: 'The client login address is required'
                    }
                }
            },
            maxusers: {
                validators: {
                    notEmpty: {
                        message: 'A maximum number of users is required'
                    }
                }
            }
        }
    });
}
