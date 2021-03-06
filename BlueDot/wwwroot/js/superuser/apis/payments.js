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
            ServiceUrl: {
                validators: {
                    callback: {
                        message: "Service URL is required",
                        callback: isTextFieldRequired
                    }
                }
            },
            ApiKey: {
                validators: {
                    callback: {
                        message: "API key is required",
                        callback: isTextFieldRequired
                    }
                }
            },
            ApyAccessToken: {
                validators: {
                    callback: {
                        message: "API token is required",
                        callback: isTextFieldRequired
                    }
                }
            }
        }
    });

    $("#api_payments_enable").on("ifChanged", function () {
        revalidateFields($("#form1"));
    });
}

function isTextFieldRequired(value, validator, $field) {
    var isFeatureEnabled = isPaymentsFeatureEnabled();
    return (isFeatureEnabled && value) || !isFeatureEnabled;
}

function isPaymentsFeatureEnabled() {
    return $("#api_payments_enable").prop("checked");
}
