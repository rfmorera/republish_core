﻿$(document).ready(initializePage);

function onAddSuccess() {
    clean_modal();
    initializePage();
}

function onDeleteSuccess() {
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
            Name: {
                validators: {
                    notEmpty: {
                        message: 'Please enter a Name'
                    }
                }
            },
            UserName: {
                validators: {
                    notEmpty: {
                        message: 'Please enter a username'
                    }
                }
            },
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
