$(document).ready(initializePageCommon);

function initializePageCommon() {
    $("a.delete-button").off("click").on("click", function (e) {
        e.preventDefault();
        var form = prepareForm($(this));

        if (form) {
            showConfirmationPopup(form);
        }
    });

    $("a.submit-button").off("click").on("click", function (e) {
        e.preventDefault();
        var form = prepareForm($(this));

        if (form) {
            form.submit();
        }
    });

    $('.tooltip-demo').each(function () {
        var tooltipData = $(this).data('bs.tooltip');

        if (!tooltipData) {
            $(this).tooltip({
                selector: "[data-toggle=tooltip]",
                container: "body"
            });
        }
    });

    var unobtrusiveAjaxForms = $("form[data-ajax=true]");

    // Note: This is needed to prevent a conflict between the jQuery Unobtrusive AJAX and FormValidation.io plugins. This conflict causes the form to be submitted twice.
    unobtrusiveAjaxForms.off("success.form.fv").on("success.form.fv", function (e) {
        e.preventDefault();
    });

    // Only do the below work if the jQuery validator plugin is not loaded. Otherwise, there is a conflict between that plugin and this code.
    // It is very unlikely that the page will use both the jQuery validator and FormValidation plugins at the same time. If this is the case then we'll have to revisit this code.
    if (!$.validator) {
        // Note: This is needed to prevent the form from being submitted when formValidation hasn't validated the form yet. This would happen if the Submit button was pressed without filling in certain fields.
        unobtrusiveAjaxForms.each(function () {
            var form = $(this);

            (function setupUnobtrusiveValidation(localForm) {
                var unobtrusiveValidation = {
                    validate: function () {
                        var formValidation = localForm.data("formValidation");
                        return !formValidation || formValidation.isValid();
                    }
                };

                localForm.data("unobtrusiveValidation", unobtrusiveValidation);
            })(form);
        });
    }
}

function prepareForm(anchorSelector) {
    var form;
    var formId = anchorSelector.attr("data-form-id");

    if (formId) {
        form = $("#" + formId);
        form.attr("action", anchorSelector.attr("href"));
    }

    return form;
}

function showConfirmationPopup(form) {
    swal({
        title: "¿Está seguro?",
        text: "Por favor confirme que usted desea eliminar este elemento",
        type: "warning",
        confirmButtonColor: "#DD6B55",
        showCancelButton: true
    }, function () {
        form.submit();
    });
}

function onAjaxBegin() {
    startLadda($(this));
}

function onAjaxSuccess() {
    displayToastrSuccess();
    enableFormValidationSubmitButton($(this));
    initializePageCommon();
}

function onAjaxError() {
    displayToastrError();
    enableFormValidationSubmitButton($(this));
}

function onAjaxCompleted() {
    completed();
    init_iChecks();
}

function enableFormValidationSubmitButton(formSelector) {
    var formValidation = formSelector.data('formValidation');

    if (formValidation) {
        formValidation.disableSubmitButtons(false);
    }
}
