$(document).ready(initializePage);

function onEditSuccess() {
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
            Qid: {
                validators: {
                    notEmpty: {
                        message: 'Please enter the new Qid'
                    },
                    integer: {
                        message: "Please enter a valid number"
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
            FormDesc: {
                validators: {
                    notEmpty: {
                        message: 'Please enter the new Form description'
                    }
                }
            },
            FormverAsString: {
                validators: {
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
            },
            DatearchivedAsString: {
                validators: {
                    date: {
                        format: 'MM/DD/YYYY',
                        message: 'The date is not valid'
                    }
                }
            },
            ExpdatedAsString: {
                validators: {
                    date: {
                        format: 'MM/DD/YYYY',
                        message: 'The date is not valid'
                    }
                }
            },
            UpdateEtaAsString: {
                validators: {
                    date: {
                        format: 'MM/DD/YYYY',
                        message: 'The date is not valid'
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
        $("#form1").data('formValidation').disableSubmitButtons(false);
    });

    initializeDateFields();
}
