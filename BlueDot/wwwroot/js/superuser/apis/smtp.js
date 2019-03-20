$(document).ready(initializePage);

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
            FromEmail: {
                validators: {
                    callback: {
                        message: "From email is required",
                        callback: isTextFieldRequired
                    }
                }
            },
            ExternalName: {
                validators: {
                    callback: {
                        message: "External name is required",
                        callback: isTextFieldRequired
                    }
                }
            },
            ExternalPort: {
                validators: {
                    callback: {
                        message: "Port is required",
                        callback: isTextFieldRequired
                    },
                    integer: {
                        message: "Port must be a number"
                    }
                }
            },
            ExternalTimeout: {
                validators: {
                    callback: {
                        message: "Timeout is required",
                        callback: isTextFieldRequired
                    },
                    integer: {
                        message: "Timeout must be a number"
                    }
                }
            },
            ExternalUsername: {
                validators: {
                    callback: {
                        message: "Username is required",
                        callback: isTextFieldRequired
                    }
                }
            },
            ExternalPassword: {
                validators: {
                    callback: {
                        message: "Password is required",
                        callback: isTextFieldRequired
                    }
                }
            },
            Basicauth: {
                validators: {
                    callback: {
                        message: "Basic authentication is required",
                        callback: isTextFieldRequired
                    },
                    integer: {
                        message: "Basic authentication must be a number"
                    }
                }
            }
        }
    });

    $("#smtp_external_use").on("ifChanged", function () {
        revalidateFields($("#form1"));
    });
}

function isTextFieldRequired(value, validator, $field) {
    var isFeatureEnabled = isSmtpFeatureEnabled();
    return (isFeatureEnabled && value) || !isFeatureEnabled;
}

function isSmtpFeatureEnabled() {
    return $("#smtp_external_use").prop("checked");
}

