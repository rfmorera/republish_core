$(document).ready(initializePage);

function initializePage() {
    $('#form1').formValidation({
        framework: 'bootstrap',
        icon: {
            valid: 'glyphicon glyphicon-ok',
            invalid: 'glyphicon glyphicon-remove',
            validating: 'glyphicon glyphicon-refresh'
        },
        fields: {
            Qid: {
                validators: {
                    notEmpty: {
                        message: 'Please enter the new Qid'
                    }
                }
            },
            FormName: {
                validators: {
                    notEmpty: {
                        message: 'Please enter the new Form name'
                    }
                }
            },
            Expdate: {
                validators: {
                    date: {
                        format: 'MM/DD/YYYY',
                        message: 'The date is not valid'
                    }
                }
            },
            FormDesc: {
                validators: {
                    notEmpty: {
                        message: 'Please enter the new Form description'
                    }
                }
            },
            Formver: {
                validators: {
                    notEmpty: {
                        message: 'Please enter the new Form version'
                    },
                    date: {
                        format: 'MM/DD/YYYY',
                        message: 'The date is not valid'
                    }
                }
            },
            FilingFee: {
                validators: {
                    numeric: {
                        message: 'Please enter a valid number',
                        thousandsSeparator: '',
                        decimalSeparator: '.'
                    }
                }
            }
        }
    }).on('err.field.fv', function (e, data) {
        data.fv.disableSubmitButtons(false);
    }).on('success.field.fv', function (e, data) {
        data.fv.disableSubmitButtons(false);
    });

    $("#form1").submit(function (e) {
        var form = $("#form1");
        var formValidation = form.data("formValidation");
        formValidation.disableSubmitButtons(false);

        if (formValidation.isValid()) {
            startLadda(form);
        }
    });

    initializeDateFields();
}
