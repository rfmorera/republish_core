function initializeDateFields() {
    var todaysDate = new Date();

    $("div.date").each(function () {
        var dateInput = $(this).find("input");

        if (dateInput.length === 1) {
            var elementName = dateInput.attr("name");

            if (elementName) {
                var formId = dateInput.attr("form");

                dateInput.datepicker({
                    todayHighlight: todaysDate,
                    autoclose: true,
                    format: "mm/dd/yyyy"
                }).on('changeDate', function (e) {
                    var form = $("#" + formId);

                    if (form.length === 1) {
                        var formValidation = form.data("formValidation");

                        if (formValidation) {
                            form.formValidation("revalidateField", elementName);
                        }
                    }
                });
            }
        }
    });
}
