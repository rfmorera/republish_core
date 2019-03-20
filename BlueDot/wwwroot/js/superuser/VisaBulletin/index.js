$(document).ready(initializePage);

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
            PreferenceType: {
                validators: {
                    notEmpty: {
                        message: 'Please select a Preference Category'
                    }
                }
            },
            CutOffDateType: {
                validators: {
                    notEmpty: {
                        message: 'Please select a Data Type'
                    }
                }
            },
            PreferenceCategoryID: {
                validators: {
                    notEmpty: {
                        message: 'Please select a Preference Type'
                    }
                }
            },
            CountryID: {
                validators: {
                    notEmpty: {
                        message: 'Please select a Country'
                    }
                }
            },
            CutOffDateAsString: {
                validators: {
                    notEmpty: {
                        message: 'The Cut off Date is required'
                    },
                    date: {
                        format: 'MM/DD/YYYY',
                        message: 'The date is not valid'
                    }
                }
            }
        }
    });

    initializeDataTable($("#dataTables-table"));
    initializeDataTablesEditor("#dataTables-table");

    initializeDateFields();
}
