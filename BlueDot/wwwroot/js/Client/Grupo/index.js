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
    $("#" + DeleteRowId).remove();
}

function initializePage() {
    initializeDataTable($("#dataTables-table"));
    initializeDataTablesEditor("#dataTables-table");

    $('.pagination > li').on('click', function () {
        initializeSwitch();
    });

    initializeSwitch();

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

    $("#HideTour").on("click", function () {
        onCookieSuccess();
    });

    // Instance the tour
    var tour = new Tour({
        steps: [{

            element: ".page-heading",
            title: "Barra de Navegación",
            content: "Muestra la ubicación de la página actual.",
            placement: "bottom",
            backdrop: true,
            backdropContainer: '#wrapper',
            onShown: function (tour) {
                $('body').addClass('tour-open')
            },
            onHidden: function (tour) {
                $('body').removeClass('tour-close')
            }
        },
        {
            element: "#btnAddGrupo",
            title: "Añadir Grupo",
            content: "Permite crear un grupo, dentro del cual usted puede crear los anuncios y temporizadores.",
            placement: "right",
            backdrop: true,
            backdropContainer: '#wrapper',
            onShown: function (tour) {
                $('body').addClass('tour-open')
            },
            onHidden: function (tour) {
                $('body').removeClass('tour-close')
            }
        },
        {
            element: "#btnDetails",
            title: "Detalles de Grupo",
            content: "Puede acceder a los detalles del grupo para añadir nuevos anuncios y configurar los temorizadores",
            placement: "right",
            backdrop: true,
            backdropContainer: '#wrapper',
            onShown: function (tour) {
                $('body').addClass('tour-open')
            },
            onHidden: function (tour) {
                $('body').removeClass('tour-close')
            }
        },
        {
            element: "#btnEliminar",
            title: "Eliminar Grupo",
            content: "Elimina el grupo seleccionado.",
            placement: "left",
            backdrop: true,
            backdropContainer: '#wrapper',
            onShown: function (tour) {
                $('body').addClass('tour-open')
            },
            onHidden: function (tour) {
                $('body').removeClass('tour-close')
            }
        }
        ]
    });

    // Initialize the tour
    tour.init();

    $('.startTour').click(function () {
        tour.restart();

        // Start the tour
        // tour.start();
    });
}

function initializeSwitch() {
    $(".switch").off("click").on("click", function (e) {
        var input = $(this).find("input.onoffswitch-checkbox");
        e.preventDefault();
        var form = prepareForm(input);

        var grupoId = $(this).closest("tr").attr('id');
        EditRowId = grupoId;

        if (form) {
            $("#ToogleId").val(grupoId);
            var val = $("input.onoffswitch-checkbox").attr("checked");
            if (val === undefined) {
                showTemporizadoresConfirmationPopup(form, "encender", "\nAcorde a su configuración sus anuncios comenzarán a actualizarse.");
            }
            else {
                showTemporizadoresConfirmationPopup(form, "apagar", "\nSus anuncios dejarán de actualizarse!!");
            }
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

function showTemporizadoresConfirmationPopup(form, estado, detail) {
    swal({
        title: "¿Está seguro?",
        text: "Por favor confirme que usted desea " + estado + " el grupo. " + detail,
        type: "warning",
        confirmButtonColor: "#DD6B55",
        showCancelButton: true
    }, function () {
        form.submit();
    });
}

function onToogleSuccess() {
    initializePage();
    onAjaxSuccess();
}