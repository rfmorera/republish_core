$(document).ready(initializePage);

function onAddSuccess() {
    clean_modal();
    initializePage();
}

function onDeleteSuccess(data, status, xhr) {
    $("#" + data).remove();
    onAjaxSuccess();
}

function initializePage() {
    $('.clockpicker').clockpicker();

    $(function () {
        $('.footable').footable();
    });

    $(".switch-temporizador").off("click").on("click", function (e) {
        var input = $(this).find("input.onoffswitch-checkbox");
        e.preventDefault();
        var form = prepareForm(input);

        var temporizadorId = $(this).closest("tr").attr('id');
        EditRowId = temporizadorId;

        if (form) {
            $("#ToogleId").val(temporizadorId);
            var val = $("input.onoffswitch-checkbox").attr("checked");
            if (val === undefined) {
                showTemporizadoresConfirmationPopup(form, "encender", "");
            }
            else {
                showTemporizadoresConfirmationPopup(form, "apagar", "");
            }
        }
    });


    $("a.delete-all-anuncios-button").off("click").on("click", function (e) {
        e.preventDefault();
        var form = prepareFormCustom($(this));

        if (form) {
            showConfirmationPopupCustom(form);
        }
    });

    $("a.delete-all-temporizadores-button").off("click").on("click", function (e) {
        e.preventDefault();
        var form = prepareFormCustom($(this));

        if (form) {
            showConfirmationPopupTemporizadoresCustom(form);
        }
    });

    $(".update-title").off('click').on('click', function (e) {
        form = $("#title-form");

        if (form) {
            showConfirmationPopupActualizarTitulo(form);
        }
    });

    // Instance the tour
    var tour = new Tour({
        steps: [
            //{
            //    element: ".startTour",
            //    title: "Pasos Iniciales",
            //    content: "A continuación mostraremos como configurar un grupo por primera vez, creando un temporizador y añadiendo anuncios al grupo.",
            //    placement: "left",
            //    backdrop: true,
            //    backdropContainer: '#wrapper',
            //    onShown: function (tour) {
            //        $('body').addClass('tour-open')
            //    },
            //    onHidden: function (tour) {
            //        $('body').removeClass('tour-close')
            //    }
            //},
            {
                element: "#btnAddTemporizador",
                title: "Añadir temporizador",
                content: "Los temporizadores controlan los horarios de actualización de los anuncios. Al dar click sobre el botón aparecerá una ventana para añadir un nuevo temporiador.",
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
                element: "#btnAddAnuncios",
                title: "Añadir anuncios",
                content: "Al dar presionar el botón aparecerá una ventana donde debe pegar el enlace de modificar el anuncio que fue provisto por Revolico. Estos anuncios serán actualizados acorde a la configuración de los temporizadores del grupo.",
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
    });

    $("#HideTour").on("click", function () {
        onCookieSuccess();
    });
}

function showConfirmationPopupCustom(form) {
    swal({
        title: "¿Está seguro?",
        text: "Por favor confirme que usted desea eliminar todos los anuncios de este grupo",
        type: "warning",
        confirmButtonColor: "#DD6B55",
        showCancelButton: true
    }, function () {
        form.submit();
    });
}
function showConfirmationPopupTemporizadoresCustom(form) {
    swal({
        title: "¿Está seguro?",
        text: "Por favor confirme que usted desea eliminar todos los temporizadores de este grupo",
        type: "warning",
        confirmButtonColor: "#DD6B55",
        showCancelButton: true
    }, function () {
        form.submit();
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
        document.cookie = "TourGDetails=yes";
        onAjaxSuccess();
    });
}

function showTemporizadoresConfirmationPopup(form, estado, detail) {
    swal({
        title: "¿Está seguro?",
        text: "Por favor confirme que usted desea " + estado + " el temporizador. " + detail,
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

function onUpdateTitleSuccess() {
    initializePage();
    onAjaxSuccess();
}

function showConfirmationPopupActualizarTitulo(form) {
    swal({
        title: "¿Está seguro?",
        text: "Por favor confirme que usted desea actualizar los títulos de los anuncios de este grupo",
        type: "warning",
        confirmButtonColor: "#DD6B55",
        showCancelButton: true
    }, function () {
        form.submit();
    });
}