$(document).ready(initializePage);

function initializePage() {
    $("input.onoffswitch-checkbox").off("click").on("click", function (e) {
        e.preventDefault();
        var form = prepareForm($(this));

        if (form) {
            var val = $("input.onoffswitch-checkbox").attr("checked");
            if (val === undefined) {
                showTemporizadoresConfirmationPopup(form, "encender", "\nAcorde a su configuración sus anuncios comenzarán a actualizarse.");
            }
            else {
                showTemporizadoresConfirmationPopup(form, "apagar", "\nSus anuncios dejarán de actualizarse!!");
            }
        }
    });

    $("#HideTour").on("click", function () {
        onCookieSuccess();
    });

    // Instance the tour
    var tour = new Tour({
        steps: [{

            element: "#IndicadoresEconomicos",
            title: "Indicadores Económicos",
            content: "Muestra el resumen de gasto y anuncios publicados.<br>Además muestra el balance actual de su cuenta.",
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
            element: "#ControlTemporizadores",
            title: "Control de Temporizadores",
            content: "Permite controlar todos los temporizadores, habilitando&deshabilitando la ejecución de los mismos.",
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
            element: "#AccesoGrupo",
            title: "Acceso a Grupos",
            content: "Los grupos están formados por anuncios y temporizadores. La función de los temporizadores es determinar los horarios en que se actualizarán los anuncios.<br><p class=\"text-primary\">En la página Grupos podrá encontrar más información.</p>",
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
            element: "#GraficosEstadisticos",
            title: "Gráficos estadísticos",
            content: "Permite conocer de forma más detallada y a lo largo del tiempo el consumo del sistema en diferentes intervalos de tiempo.",
            placement: "top",
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
            element: "#SelectorGraficosEstadisticos",
            title: "Selector",
            content: "Seleccione el intervalo de tiempo del cual desea obtener las estadísticas hoy, últimos 7 días, mensual y anual.",
            placement: "left",
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
            element: ".sidebar-collapse",
            title: "Menú de Navegación",
            content: "Inicio es la página inicial donde nos encontramos, en Grupos podrá añadir y configurar la publicación de sus anuncios.<br>Explore el resto de los enlaces.",
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
            element: "#CerrarSesion",
            title: "Cerrar Sesión",
            content: "Cuando finalice su trabajo cierre la sesión para mayor seguridad.",
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

function onCookieSuccess() {
    swal({
        title: "¿Está seguro?",
        text: "Por favor confirme que usted desea ocultar el tutorial.",
        type: "warning",
        confirmButtonColor: "#DD6B55",
        showCancelButton: true
    }, function () {
        $("#TourDiv").css("display", "none");
        document.cookie = "TourDefault=yes";
        onAjaxSuccess();
    });
}

function showTemporizadoresConfirmationPopup(form, estado, detail) {
    swal({
        title: "¿Está seguro?",
        text: "Por favor confirme que usted desea " + estado + " los temporizadores. " + detail ,
        type: "warning",
        confirmButtonColor: "#DD6B55",
        showCancelButton: true
    }, function () {
        form.submit();
    });
}