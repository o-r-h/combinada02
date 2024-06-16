function Chart() {

}
Chart.CompCIFImport = function (dataPie) {
    Highcharts.chart('container3', {
        title: {
            text: 'Compatarivo CIF US$'
        },
        xAxis: {
            categories:dataPie.Categories,
            crosshair: true
        },
        yAxis: {
            min: 0,
            title: {
                text: ''
            }
        },
        legend: {
            layout: 'vertical',
            align: 'right',
            verticalAlign: 'middle'
        },
        plotOptions: {
            series: {
                label: {
                    connectorAllowed: false
                }
            }
        },
        series: 
           dataPie.Series
        ,
        responsive: {
            rules: [{
                condition: {
                    maxWidth: 500
                },
                chartOptions: {
                    legend: {
                        layout: 'horizontal',
                        align: 'center',
                        verticalAlign: 'bottom'
                    }
                }
            }]
        }
    });
}
Chart.ValorCIFImport = function (dataPie) {
    Highcharts.chart('container', {
        chart: {
            type: 'column'
        },
        title: {
            text: 'Valor CIF Importacion'
        },
        colors: ['#4F81BD'],
        xAxis: {
            type: 'category'
        },
        yAxis: {
            title: {
                text: 'Total percent market share'
            }
        },
        legend: {
            enabled: false
        },
        plotOptions: {
            column: {
                pointPadding: 0,
                borderWidth: 0
            }
        },

        tooltip: {
            pointFormat: '<span style="color:{point.color}">{point.name}</span>: <b>{point.y:.2f}</b><br/>'
        },

        series: [{
            colorByPoint: true,
            data: dataPie
        }]
    });
}
Chart.PrecioProm = function(dataPie) {
    Highcharts.chart('container2', {
        title: {
            text: 'Precio Promedio $US/Unidad'
        },
        yAxis: {
            title: {
                text: ''
            }
        },
        legend: {
            enabled: false
        },
        xAxis: {
            //categories: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec']
            type: 'category'
        },
        plotOptions: {
            line: {
                dataLabels: {
                    enabled: true
                },
                enableMouseTracking: true
            },
        },
        tooltip: {
            crosshairs: [true],
            formatter: function () {
                var yVal = this.y;
                return "<b>" + this.x + '</b>: $' + yVal.toLocaleString('en');
            }
        },
        series: [{
            //name: 'Installation',
            data: dataPie
        }],
        responsive: {
            rules: [{
                condition: {
                    maxWidth: 500
                },
                chartOptions: {
                    legend: {
                        layout: 'horizontal',
                        align: 'center',
                        verticalAlign: 'bottom'
                    }
                }
            }]
        }
    });
}

function Busqueda() {
}

Busqueda.searchDataByProduct = function (urlPost, pIdioma) {
    $.ajax({
        url: urlPost,
        type: "POST",
        data: {
            codProducto: $("#slug").val(),
            idioma: pIdioma
        },
        beforeSend: function () {
            LoadingAdminPage.showOrHideLoadingPage(true);
        },
        success: function (response) {
            //console.log(response.objMiProducto);
            //console.log(response.objProductoByPais);
            //console.log(response.listaConsolidado);
            //console.log(response.objMiProducto.CantidadTotal);
            //console.log(response.chartComparativoCif);
            //console.log(response.lineData);
            //console.log(response.charData);
            if (response.objMiProducto != null) {
                $("#codProducto").html(response.objMiProducto.CodProducto + ": " + response.objMiProducto.Descripcion),
                $("#paisFlag").html(response.objMiProducto.PaisAduana),
                $("#valorImpor").html(response.objProductoByPais.Importaciones),
                $("#cantEmpImpor").html(response.objProductoByPais.Importadores);
                $("#valorExpor").html(response.objProductoByPais.Exportaciones),
                $("#cantEmpExpor").html(response.objProductoByPais.Exportadores);
                $("#containerB").html(response.vistaPaises);
                $("#totalImport").html(response.objMiProducto.ValorTotal + "<span>US$</span>");
                $("#cantTotalImpor").html(response.objMiProducto.CantidadTotal + "<span>KILOGRAMOS</span>");
                $("#precioTotalImpor").html(response.objMiProducto.PrecioUnitTotal + "<span>US$/KILOGRAMOS</span>");
            }
            Chart.ValorCIFImport(response.charData);
            Chart.PrecioProm(response.lineData);

            

            Chart.CompCIFImport(response.chartComparativoCif);


            //response.listaProductos;)
        },
        error: function (dataError) {
            console.log(dataError);
        }
    });
}