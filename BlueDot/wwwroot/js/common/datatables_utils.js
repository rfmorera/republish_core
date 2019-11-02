function initializeDataTable(selector) {
    if (selector.length > 0) {
        var columnArray = [];

        $(selector).find("thead > tr > th").each(function () {
            var columnName = getColumnName($(this));
            var columnData = {};

            if ($(this).hasClass("date-column")) {
                columnData.type = "date";
            }

            if (columnName) {
                columnData.data = columnName;
            }

            if (columnData.hasOwnProperty("type") || columnData.hasOwnProperty("data")) {
                columnArray.push(columnData);
            }
            else {
                columnArray.push(null);
            }
        });

        selector.DataTable({
            "lengthMenu": [[ 25, 50, -1], [ 25, 50, "All"]],
            dom: '<"html5buttons"B>lTfgitp',
            buttons: [
                //{ extend: 'copy' },
                //{ extend: 'csv', title: 'Report' },
                //{ extend: 'excel', title: 'Report' },
                //{ extend: 'pdf', title: 'Report' },
                //{
                //    extend: 'print',
                //    title: "",
                //    customize: function (win) {
                //        $(win.document.body).addClass('white-bg');
                //        $(win.document.body).css('font-size', '10px');

                //        $(win.document.body).find('table')
                //            .addClass('compact')
                //            .css('font-size', 'inherit');
                //    }
                //}
            ],
            "columns": columnArray
        });
    }
}

function getColumnName(selector) {
    var columnName = selector.attr("data-column-name");

    if (!columnName) {
        columnName = null;
    }

    return columnName;
}
