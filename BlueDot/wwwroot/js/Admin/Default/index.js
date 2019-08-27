$(document).ready(initializePage);

function initializePage() {
    $(".btn-group.estadistica > #Anual").on("click", function () {
        ButtonCommon(this);
        InitializeAnual();
    });

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
}

function initializeChart(ventaTotal, labels, venta, gastos) {

    $("#VentaTotalResumen").text("$ " + ventaTotal);

    var data = {
        labels: labels,
        datasets: [
            {
                label: "Venta ($)",
                backgroundColor: "rgba(26,179,148,0.5)",
                borderColor: "rgba(26,179,148,0.7)",
                pointBackgroundColor: "rgba(26,179,148,1)",
                pointBorderColor: "#fff",
                data: venta
            },
            {
                label: "Gasto",
                backgroundColor: "rgba(220,220,220,0.5)",
                borderColor: "rgba(220,220,220,1)",
                pointBackgroundColor: "rgba(220,220,220,1)",
                pointBorderColor: "#fff",
                data: gastos
            }
        ]
    };

    var lineOptions = {
        responsive: true
    };

    var ctx = document.getElementById("lineChart").getContext("2d");
    new Chart(ctx, { type: 'line', data: data, options: lineOptions });
}