$(document).ready(initializePage);

function onSaveSuccess() {
    onAjaxSuccess();
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
            Paramnm: {
                validators: {
                    notEmpty: {
                        message: 'Please enter a Name'
                    },
                }
            },

            Paramval: {
                validators: {
                    notEmpty: {
                        message: 'Please enter a value'
                    }
                }
            }
        }
    });
}
