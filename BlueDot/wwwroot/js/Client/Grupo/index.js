﻿$(document).ready(initializePage);

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
    initializeDataTable($("#dataTables-table"));

    $("#HideTour").on("click", function () {
        onCookieSuccess();
    });

    $('#form2').formValidation({
        framework: 'bootstrap',
        icon: {
            valid: 'glyphicon glyphicon-ok',
            invalid: 'glyphicon glyphicon-remove',
            validating: 'glyphicon glyphicon-refresh'
        },
        fields: {
            Nombre: {
                validators: {
                    notEmpty: {
                        message: 'Por favor escriba el nombre'
                    },
                }
            },
        }
    });
}

function onCookieSuccess() {
    swal({
        title: "¿Está seguro?",
        text: "Por favor confirme que usted desea ocultar el tutorial.",
        type: "warning",
        confirmButtonColor: "#DD6B55",
        showCancelButton: true
    }, function () {
        $("#TourDiv").css("display", "none");
        document.cookie = "TourGrupo=yes";
        onAjaxSuccess();
    });
}