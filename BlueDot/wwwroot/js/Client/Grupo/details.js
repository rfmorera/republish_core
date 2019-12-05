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

function onActionSuccess() {
    initializePage();
    onAjaxSuccess();
}


function onDeleteRecordFailure(xhr, status, error) {
    if (xhr.status === 422) {
        var response = xhr.responseText;
        // Note: Need to also check "UsernameExists" because this is the string returned by the DropZone plugin. This is for the case where attachments are included in the questionnaire.
        if (response.length > 0) {
            setTimeout(
                function () {
                    swal({
                        title: "Error",
                        text: response,
                        type: "warning",
                        confirmButtonColor: "#DD6B55",
                        showCancelButton: false,
                        showConfirmButton: true
                    });
                }, 1000);
        }
        else {
            onAjaxError.apply(this, arguments);
        }
    }
    else {
        onAjaxError.apply(this, arguments);
    }
}