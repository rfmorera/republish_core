$(document).ready(initializePage);

function initializePage() {

    $(".btn-group.estadistica > #Diario").on("click", function () {
        ButtonCommon(this);
        InitializeDiario();
    });

    $(".btn-group.estadistica > #Semanal").on("click", function () {
        ButtonCommon(this);
        InitializeSemanal();
    });

    $(".btn-group.estadistica > #Mensual").on("click", function () {
        ButtonCommon(this);
        InitializeMensual();
    });

    //$(".btn-group.estadistica > #Anual").on("click", function () {
    //    ButtonCommon(this);
    //    InitializeAnual();
    //});

    $(".line").peity("line", {
        fill: '#1ab394',
        stroke: '#169c81'
    });

    $(".bar").peity("bar", {
        fill: ["#1ab394", "#d7d7d7"]
    });

    $(".bar_dashboard").peity("bar", {
        fill: ["#1ab394", "#d7d7d7"],
        width: 100
    });
}

function ButtonCommon(t) {
    $(".btn-group.estadistica > .active").removeClass("active");
    $(t).addClass("active");
    $(".chartjs-hidden-iframe").remove();
    var canvas = document.getElementById("lineChart");
    var parent = $(canvas).parent();
    canvas.remove();
    parent.append("<canvas id=\"lineChart\" height=\"287\" style=\"display: block; width: 989px; height: 230px; \" width=\"1236\"></canvas>");
    //var ctx = canvas.getContext('2d');
    //ctx.clearRect(0, 0, canvas.width, canvas.height);
}

function initializeChart(gastoTotal, labels, gastos, anuncios) {

    $("#GastoTotalResumen").text("$ "+gastoTotal);

    var data = {
        labels: labels,
        datasets: [
            {
                label: "Gasto",
                backgroundColor: "rgba(26,179,148,0.5)",
                borderColor: "rgba(26,179,148,0.7)",
                pointBackgroundColor: "rgba(26,179,148,1)",
                pointBorderColor: "#fff",
                data: gastos
            },
            {
                label: "Anuncios Publicados",
                backgroundColor: "rgba(220,220,220,0.5)",
                borderColor: "rgba(220,220,220,1)",
                pointBackgroundColor: "rgba(220,220,220,1)",
                pointBorderColor: "#fff",
                data: anuncios
            }
        ]
    };

    var lineOptions = {
        responsive: true
    };
    
    var ctx = document.getElementById("lineChart").getContext("2d");
    new Chart(ctx, { type: 'line', data: data, options: lineOptions });
}
