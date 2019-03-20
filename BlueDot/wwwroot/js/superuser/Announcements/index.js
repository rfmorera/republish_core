$(document).ready(initializePage);

function onSaveSuccess() {
    onAjaxSuccess();
    initializePage();
}

function initializePage() {
    var form = $("#form1");

    form.formValidation({
        framework: 'bootstrap',
        icon: {
            valid: 'glyphicon glyphicon-ok',
            invalid: 'glyphicon glyphicon-remove',
            validating: 'glyphicon glyphicon-refresh'
        },
        fields: {
            AlertDateAsString: {
                validators: {
                    date: {
                        format: 'MM/DD/YYYY',
                        message: 'The date is not valid or in the wrong format (mm/dd/yyyy)'
                    }
                }
            }
        }
    });

    setSubmitButtonAlwaysEnabled(form);

    initializeDateFields();

    initializeSummerNote($("#summernote"));
}
