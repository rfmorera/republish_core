var editor;
var rowIdPropertyName;

function initializeDataTablesEditor(dataTablesSelectorString) {
    var dataTablesSelector = $(dataTablesSelectorString);

    if (dataTablesSelector.length > 0) {
        var editUrl = dataTablesSelector.attr("data-edit-url");
        rowIdPropertyName = dataTablesSelector.attr("data-row-id-property-name");

        if (dataTablesSelector && rowIdPropertyName) {
            editor = new $.fn.dataTable.Editor({
                ajax: {
                    url: editUrl,
                    type: "POST",
                    success: function (response) {
                        editor.close();

                        if (response.hasOwnProperty(rowIdPropertyName)) {
                            var row = $("tr[id='" + response[rowIdPropertyName] + "']");

                            var dataTable = $(dataTablesSelectorString).DataTable();

                            row.children("td").each(function () {
                                var columnName = $(this).attr("data-column-name");

                                if (columnName && response.hasOwnProperty(columnName)) {
                                    $(this).text(response[columnName]);

                                    var cell = dataTable.cell($(this));
                                    cell.data(response[columnName]);
                                }
                            });
                        }                        
                    },
                    error: handleEditorError
                },
                table: dataTablesSelectorString
            });

            dataTablesSelector.find("thead > tr > th").each(setupEditorForColumnIfNeeded);

            dataTablesSelector.on('click', 'tbody td:not(:first-child)', onCellClicked);

            editor.on('preSubmit', preSubmit);
        }
    }
}

function handleEditorError(jqXHR, textStatus, errorThrown) {
    editor.close();

    if (jqXHR.status === 400 && jqXHR.hasOwnProperty("responseJSON")) {
        var keys = Object.keys(jqXHR.responseJSON);
        var error;

        for (var i = 0; i < keys.length; ++i) {
            var key = keys[i];
            var column = $("th[data-column-name='" + key + "']");

            if (column.length > 0 && jqXHR.responseJSON[key] instanceof Array && jqXHR.responseJSON[key].length > 0) {
                error = jqXHR.responseJSON[key][0];
                break;
            }
        }

        if (error) {
            swal({
                title: "Error",
                text: error,
                type: "error",
                confirmButtonColor: "#DD6B55",
                showCancelButton: false
            });
        }
        else {
            displayToastrError();
        }
    }
    else {
        displayToastrError();
    }
}

function setupEditorForColumnIfNeeded() {
    var editingEnabled = $(this).attr("data-column-name");

    if (editingEnabled) {
        var columnName = getColumnName($(this));

        editor.add({
            attr: {
                "class": "form-control",
                "style": "width: 100%;"
            },
            name: columnName
        });
    }
}

function onCellClicked(e) {
    editor.inline(this, {
        onBlur: function () {
            editor.submit();
            editor.close();
        },
        submit: "allIfChanged",
        onComplete: "none"
    });
}

function preSubmit(e, o, a) {
    if (a === "edit") {
        var rowID;

        for (var key in o.data) {
            rowID = key;
            break;
        }

        o.Action = o.action;
        o[rowIdPropertyName] = rowID;

        for (key in o.data[rowID]) {
            o[key] = o.data[rowID][key];
        }

        delete o.action;
        delete o.data;
    }
}
