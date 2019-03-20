function setSubmitButtonAlwaysEnabled(formSelector) {
    formSelector.on('err.field.fv', function (e, data) {
        data.fv.disableSubmitButtons(false);
    }).on('success.field.fv', function (e, data) {
        data.fv.disableSubmitButtons(false);
    });

    formSelector.submit(function (e) {
        var formValidation = $(this).data("formValidation");
        formValidation.disableSubmitButtons(false);
    });
}

function revalidateFields(formSelector) {
    var formValidation = formSelector.data("formValidation");

    if (formValidation) {
        formSelector.find("input").each(function () {
            var fieldName = $(this).attr("name");

            if (fieldName) {
                formValidation.revalidateField(fieldName);
            }
        });
    }
}
