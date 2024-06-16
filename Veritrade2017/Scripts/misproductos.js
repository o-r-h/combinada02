function Size() {

}

Size.ChartsTop = function (count) {
    if (count >= 4) {
        count = 4;
    }
    //var tableRowsPais = $('#tablaRankPais table tbody tr').length;
    var flag = 0;
    var newTr = '<tr><td></td> <td>/td><td></td><td><input type="radio" name="paises"></td></tr>';
    while (flag < 6 - count) {
        $('#tablaRankPais table tbody').append(newTr);
        flag++;
    }
}
Size.TableDetalleBox = function() {
    var tableRows = $('#TabDetalle table tbody tr').length;
    var topBox = 37 + 65 * tableRows;
    var heightBox = (6 - tableRows) * 65;
    var heightTable = heightBox;

    if (tableRows > 3) {
        topBox = 232;
        heightBox = 195;
        heightTable = 23 + 65 * (5 - tableRows);
    }

    $('#BoxDetalle').css('top', topBox);
    $('#BoxDetalle').css('height', heightBox);
    $('#TabDetalle .tab-detalle .detalle .table-responsive').css('margin-bottom', heightTable);
}

function Productos() {

}

Productos.LastSearches = function (urlPost, culture, cont) {
    $.ajax({
        url: urlPost,
        data: {
            culture: culture,
            cont: cont
        },        
        type: "POST",
        success: function (response) {
            $("#LastSearches").html(response.viewLastSearches);
        },
        error: function (dataError) {
            console.log(dataError);
        }
    });
}

function MisModales() {
}

MisModales.showModal = function (modalTitle, urlPost, pIdioma, tipoOpe) {

    $.ajax({
        type: "POST",
        url: urlPost,
        data: {
            IdProducto: $("#slug").val(),
            IdPaisAduana: $("#slug2").val(),
            TipoOpe: tipoOpe,
            idioma: pIdioma
        },
        beforeSend: function () {
            LoadingAdminPage.showOrHideLoadingPage(true);
        },
        success: function (response) {
            $('#ModalFavoritos').modal({
                show: true
            });
            LoadingAdminPage.showOrHideLoadingPage(false);
        },
        error: function (data) {
            console.log(data);
        }
    });
}
function Carousel() {

}
Carousel.Cargar = function () {
    var owlOptions = {

        loop: true,
        center: false,
        autoplay: true,
        autoplayTimeout: 3500,
        autoplayHoverPause: false,
        stagePadding: 45,
        nav: true,
        navText: ['<i class="glyphicon glyphicon-chevron-left"></i>', '<i class="glyphicon glyphicon-chevron-right"></i>'],
        responsive: {
            0: {
                items: 1,
                stagePadding: 10,
            },
            600: {
                items: 2,
                stagePadding: 45,
            },
            1000: {
                items: 3,
                stagePadding: 45,
            }
        }
    };

    $('#countries').owlCarousel(owlOptions);
}
function Paging() {

}

Paging.Paginator = function (urlPost, IdProducto, IdPaisAduana) {
    $.ajax({
        type: "POST",
        url: urlPost,
        data: {
            IdProducto: IdProducto,
            IdPaisAduana: IdPaisAduana,
            tipoOpe: $("#cboRegimen").find("option:selected").val()
        },
        success: function (response) {
            $("#tablaRankImp").html(response.vistaRankRegimen);
            $("#paginator").html("");
            $("#paginator").html(response.pagingReg);

            Size.ChartsTop();
            //$('#pagingFavourites').data('tipoFavorito', favouriteType);

            //LoadingAdminPage.showOrHideLoadingPage(false);

            //$('#ModalFavoritos').modal({
            //    show: true,
            //    backdrop: 'static',
            //    keyboard: false
            //});
        },
        error: function (data) {
            console.log(data);
        }
    });
}

Paging.PaginatorB = function (urlPost, IdProducto, IdPaisAduana) {
    $.ajax({
        type: "POST",
        url: urlPost,
        data: {
            IdProducto: IdProducto,
            IdPaisAduana: IdPaisAduana,
            tipoOpe: $("#cboRegimen").find("option:selected").val()
        },
        success: function (response) {
            $("#tablaRankPais").html(response.vistaRankPais);
            $("#paginatorPais").html("");
            $("#paginatorPais").html(response.pagingPais);

            //$('#pagingFavourites').data('tipoFavorito', favouriteType);

            //LoadingAdminPage.showOrHideLoadingPage(false);

            //$('#ModalFavoritos').modal({
            //    show: true,
            //    backdrop: 'static',
            //    keyboard: false
            //});
        },
        error: function (data) {
            console.log(data);
        }
    });
}

Paging.pagingClickListener = function (IdProducto, IdPaisAduana) {
    $('#paginator').on('click',
        'a',
        function (e) {
            //prevent action link normal functionality
            e.preventDefault();

            var vUrlPost = this.href;
            if (vUrlPost != "") {
                $.ajax({
                    type: "POST",
                    url: vUrlPost,
                    data: {
                        IdProducto: IdProducto,
                        IdPaisAduana: IdPaisAduana,
                        tipoOpe: $("#cboRegimen").find("option:selected").val()
                    },
                    success: function (response) {
                        $("#tablaRankImp").html(response.vistaRankRegimen);
                        $("#paginator").html("");
                        $("#paginator").html(response.pagingReg);

                        //LoadingAdminPage.showOrHideLoadingPage(false);

                    },
                    error: function (data) {
                        console.log(data);
                    }
                });
            }
        });
}

Paging.pagingClickListenerB = function (IdProducto, IdPaisAduana) {
    $('#paginatorPais').on('click',
        'a',
        function (e) {
            //prevent action link normal functionality
            e.preventDefault();

            var vUrlPost = this.href;
            if (vUrlPost != "") {
                $.ajax({
                    type: "POST",
                    url: vUrlPost,
                    data: {
                        IdProducto: IdProducto,
                        IdPaisAduana: IdPaisAduana,
                        tipoOpe: $("#cboRegimen").find("option:selected").val()
                    },
                    success: function (response) {
                        $("#tablaRankPais").html(response.vistaRankPais);
                        $("#paginatorPais").html("");
                        $("#paginatorPais").html(response.pagingPais);

                        //LoadingAdminPage.showOrHideLoadingPage(false);

                    },
                    error: function (data) {
                        console.log(data);
                    }
                });
            }
        });
}

function Chart() {

}

Chart.RankingPais = function (title, id, dataPie) {
    Highcharts.chart(id, {
        chart: {
            plotBackgroundColor: null,
            plotBorderWidth: null,
            plotShadow: false,
            type: 'pie'
        },
        title: {
            text: title
        },
        tooltip: {
            headerFormat: '',
            pointFormat: '{point.name}: <b>{point.percentage:.1f}%</b>'
        },
        plotOptions: {
            pie: {
                allowPointSelect: true,
                cursor: 'pointer',
                dataLabels: {
                    enabled: true,
                    format: '<b>{point.name}</b>: {point.percentage:.1f} %',
                    style: {
                        color: (Highcharts.theme && Highcharts.theme.contrastTextColor) || 'black'
                    },
                    connectorColor: 'silver'
                }
            }
        },
        series: [{
            data: dataPie
        }]
    });
}
Chart.RankingImp = function (title, id, dataPie) {
    Highcharts.chart(id, {
        chart: {
            plotBackgroundColor: null,
            plotBorderWidth: null,
            plotShadow: false,
            type: 'pie'
        },
        title: {
            text: title
        },
        tooltip: {
            headerFormat: '',
            pointFormat: '{point.name}: <b>{point.percentage:.1f}%</b>'
        },
        plotOptions: {
            pie: {
                allowPointSelect: true,
                cursor: 'pointer',
                dataLabels: {
                    enabled: true,
                    format: '<b>{point.name}</b>: {point.percentage:.1f} %',
                    style: {
                        color: (Highcharts.theme && Highcharts.theme.contrastTextColor) || 'black'
                    },
                    connectorColor: 'silver'
                }
            }
        },
        series: [{
            name: 'Brands',
            data: dataPie
        }]
    });
}
Chart.CompCIFImport = function (title, id, dataPie) {
    Highcharts.chart(id, {
        title: {
            text: title
        },
        xAxis: {
            categories: ['Ene', 'Feb', 'Mar', 'Abr', 'May', 'Jun',
                'Jul', 'Ago', 'Sep', 'Oct', 'Nov', 'Dic'],
            crosshair: true
        },
        yAxis: {
            min: 0,
            title: {
                text: ''
            },
            labels: {
                formatter: function () {
                    return '$' + this.value.toLocaleString('en');
                }
            }
        },
        legend: {
            layout: 'vertical',
            align: 'right',
            verticalAlign: 'middle'
        },
        tooltip: {
            crosshairs: true,
            shared: true,
            formatter: function () {
                var output = '<strong>' + this.x + '</strong>';
                var sorted = this.points.sort(function (a, b) {
                    if (a.y == b.y) {
                        return 0;
                    }
                    return a.y < b.y ? 1 : -1;
                });

                sorted.forEach(function (point, index) {
                    var marker = '';

                    if (point.point.graphic && point.point.graphic.symbolName) {
                        switch (point.point.graphic.symbolName) {
                            case 'circle':
                                marker = '●';
                                break;
                            case 'diamond':
                                marker = '♦';
                                break;
                            case 'square':
                                marker = '■';
                                break;
                            case 'triangle':
                                marker = '▲';
                                break;
                            case 'triangle-down':
                                marker = '▼';
                                break;
                        }
                    }
                    /* 
                        TODO: How do I determine what symbol is used for the marker?
                    */
                    output += '<br /><span style="color: ' + point.series.color + '">' + marker + '</span> ' + point.series.name + ': $ ' + Highcharts.numberFormat(point.y, 0, '.', ',');
                });
                return output;
            }

            //valueDecimals: 0,
            //valuePrefix: '$ '
        },
        plotOptions: {
            //series: {
            //    label: {
            //        connectorAllowed: false
            //    }
            //}
        },
        series:
            dataPie.Series
        ,
        responsive: {
            rules: [
                {
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
                }
            ]
        }
    });
}
Chart.ValorCIFImport = function (title, id, dataPie) {
    Highcharts.chart(id, {
        chart: {
            type: 'column'
        },
        title: {
            text: title
        },
        colors: ['#4F81BD'],
        xAxis: {
            type: 'category'
        },
        yAxis: {
            min: 0,
            title: {
                text: ''
            },
            labels: {
                formatter: function () {
                    return '$' + this.value.toLocaleString('en');
                }
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
            crosshairs: [true],
            formatter: function () {
                yVal = this.y;
                return "<b>" + this.point.name + '</b>: $' + yVal.toLocaleString('en');
                //return "<b>" + this.point.name + '</b>: $' + Highcharts.numberFormat(this.point.y, 3, '.', ',');
            }
        },

        series: [{
            data: dataPie
        }]
    });
}
Chart.PrecioProm = function (title, id, dataPie) {
    Highcharts.chart(id, {
        title: {
            text: title
        },
        yAxis: {
            min: 0,
            title: {
                text: ''
            },
            labels: {
                formatter: function () {
                    return '$' + this.value.toLocaleString('en');
                }
            }
        },
        legend: {
            layout: 'vertical',
            align: 'right',
            verticalAlign: 'middle'
        },
        xAxis: {
            categories: ['Ene', 'Feb', 'Mar', 'Abr', 'May', 'Jun', 'Jul', 'Ago', 'Sep', 'Oct', 'Nov', 'Dec'],
            crosshair: true
        },
        plotOptions: {
            //line: {
            //    dataLabels: {
            //        enabled: true
            //    },
            //    enableMouseTracking: true
            //},
        },
        tooltip: {
            crosshairs: true,
            shared: true,
            formatter: function () {
                var output = '<strong>' + this.x + '</strong>';
                var sorted = this.points.sort(function (a, b) {
                    if (a.y == b.y) {
                        return 0;
                    }
                    return a.y < b.y ? 1 : -1;
                });

                sorted.forEach(function (point, index) {
                    var marker = '';

                    if (point.point.graphic && point.point.graphic.symbolName) {
                        switch (point.point.graphic.symbolName) {
                            case 'circle':
                                marker = '●';
                                break;
                            case 'diamond':
                                marker = '♦';
                                break;
                            case 'square':
                                marker = '■';
                                break;
                            case 'triangle':
                                marker = '▲';
                                break;
                            case 'triangle-down':
                                marker = '▼';
                                break;
                        }
                    }
                    /* 
                        TODO: How do I determine what symbol is used for the marker?
                    */
                    output += '<br /><span style="color: ' + point.series.color + '">' + marker + '</span> ' + point.series.name + ': $ ' + Highcharts.numberFormat(point.y, 2, '.', ',');
                });
                return output;
            }

            //valueDecimals: 0,
            //valuePrefix: '$ '
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

function Busqueda() {
}
Busqueda.CargarPorPais = function (urlPost, pIdioma, tipoOpe, urlReg, urlPais) {
    $.ajax({
        url: urlPost,
        type: "POST",
        data: {
            IdProducto: $("#slug").val(),
            IdPaisAduana: $("#slug2").val(),
            TipoOpe: tipoOpe,
            idioma: pIdioma
        },
        beforeSend: function () {
            LoadingAdminPage.showOrHideLoadingPage(true);
        },
        success: function (response) {
            $('#ProductByPais').html(response.viewProductByPais);
            $('#ProductByPais2').html(response.viewProductByPais2);
            $("#PrimerContenedor").html(response.viewRegimen);
            $("#TabDetalle").html(response.viewTabDetalle);
            $("#ModalDetalle").html(response.modalDetalle);
            $("#ModalChart").html(response.viewTabDetalleModal);
            $("#tableComparative").html(response.viewTableComparative);
            $("#tablePrecProm").html(response.viewTablePrecProm);
            if (tipoOpe == "Importaciones") {
                if (pIdioma == "es") {
                    if (response.charData != null) {
                        Chart.ValorCIFImport("Valor CIF Importado US$", "container", response.charData);
                    }
                    if (response.chartPrecioUnit != null) {
                        Chart.PrecioProm("Precio Promedio US$ / KILOGRAMOS", "container2", response.chartPrecioUnit);
                    }
                    if (response.chartPrecioUnit == null) {
                        $("#PrecioUnit").css("display","none");
                    }
                    if (response.chartCompCif != null) {
                        Chart.CompCIFImport("Comparativo CIF US$", 'container3', response.chartCompCif);
                    }
                    if (response.pieRanking != null) {
                        Chart.RankingImp("Principales Importadores", 'piecontainer1', response.pieRanking);
                    }
                    if (response.pieRanking == null) {
                        $("#TopImpExp").css("display", "none");
                    }
                    if (response.pieRanking2 != null) {
                        Chart.RankingPais("Principales Países de Origen", 'piecontainer2', response.pieRanking2);
                    }
                } else {
                    if (response.charData != null) {
                        Chart.ValorCIFImport("Imported CIF US$", "container", response.charData);
                    }
                    if (response.chartPrecioUnit != null) {
                        Chart.PrecioProm("Average Price US$ / KILOGRAMS", "container2", response.chartPrecioUnit);
                    }
                    if (response.chartPrecioUnit == null) {
                        $("#PrecioUnit").css("display", "none");
                    }
                    if (response.chartCompCif != null) {
                        Chart.CompCIFImport("Comparative CIF US$", 'container3', response.chartCompCif);
                    }
                    if (response.pieRanking != null) {
                        Chart.RankingImp("Top Importers", 'piecontainer1', response.pieRanking);
                    }
                    if (response.pieRanking == null) {
                        $("#TopImpExp").css("display", "none");
                    }
                    if (response.pieRanking2 != null) {
                        Chart.RankingPais("Top Origin Countries", 'piecontainer2', response.pieRanking2);
                    }
                }
            }
            else {
                if (pIdioma == "es") {
                    if (response.charData != null) {
                        Chart.ValorCIFImport("Valor FOB Exportado US$", "container1", response.charData);
                    }
                    if (response.chartPrecioUnit != null) {
                        Chart.PrecioProm("Precio Promedio US$ / KILOGRAMOS", "container_2", response.chartPrecioUnit);
                    }
                    if (response.chartPrecioUnit == null) {
                        $("#PrecioUnit").css("display", "none");
                    }
                    if (response.chartCompCif != null) {
                        Chart.CompCIFImport("Comparativo FOB US$", 'container_3', response.chartCompCif);
                    }
                    if (response.pieRanking != null) {
                        Chart.RankingImp("Principales Exportadores", 'piecontainer1', response.pieRanking);
                    }
                    if (response.pieRanking == null) {
                        $("#TopImpExp").css("display", "none");
                    }
                    if (response.pieRanking2 != null) {
                        Chart.RankingPais("Principales Países de Destino", 'piecontainer2', response.pieRanking2);
                    }
                } else {
                    if (response.charData != null) {
                        Chart.ValorCIFImport("Exported FOB Value US$", "container1", response.charData);
                    }
                    if (response.chartPrecioUnit != null) {
                        Chart.PrecioProm("Average Price US$ / KILOGRAMS", "container_2", response.chartPrecioUnit);
                    }
                    if (response.chartPrecioUnit == null) {
                        $("#PrecioUnit").css("display", "none");
                    }
                    if (response.chartCompCif != null) {
                        Chart.CompCIFImport("Comparative FOB US$", 'container_3', response.chartCompCif);
                    }
                    if (response.pieRanking != null) {
                        Chart.RankingImp("Top Exporters", 'piecontainer1', response.pieRanking);
                    }
                    if (response.pieRanking == null) {
                        $("#TopImpExp").css("display", "none");
                    }
                    if (response.pieRanking2 != null) {
                        Chart.RankingPais("Top Destination Countries", 'piecontainer2', response.pieRanking2);
                    }
                }
            }
            var IdProducto = $("#slug").val();
            var IdPaisAduana = $("#slug2").val();
            Paging.Paginator(urlReg, IdProducto, IdPaisAduana);
            //Paging.pagingClickListener(IdProducto, IdPaisAduana);
            Paging.PaginatorB(urlPais, IdProducto, IdPaisAduana);
            //Paging.pagingClickListenerB(IdProducto, IdPaisAduana);
            LoadingAdminPage.showOrHideLoadingPage(false);
            Size.TableDetalleBox();
        },
        error: function (dataError) {
            console.log(dataError);
        }
    });
}
Busqueda.CargarPorFiltro = function (urlPost, pIdioma, tipoOpe, urlReg, urlPais) {
    $.ajax({
        url: urlPost,
        type: "POST",
        data: {
            IdProducto: $("#slug").val(),
            IdPaisAduana: $("#slug2").val(),
            TipoOpe: tipoOpe,
            idioma: pIdioma
        },
        beforeSend: function () {
            LoadingAdminPage.showOrHideLoadingPage(true);
        },
        success: function (response) {
            $('#ProductByPais').html(response.viewProductByPais);
            $('#ProductByPais2').html(response.viewProductByPais2);
            $("#carrusel").html(response.viewCarrusel);
            $("#PrimerContenedor").html(response.viewRegimen);
            Carousel.Cargar();
            $("#TabDetalle").html(response.viewTabDetalle);
            $("#ModalChart").html(response.viewTabDetalleModal);
            $("#tableComparative").html(response.viewTableComparative);
            $("#tablePrecProm").html(response.viewTablePrecProm);
            if (tipoOpe == "Importaciones") {
                if (pIdioma == "es") {
                    if (response.charData != null) {
                        Chart.ValorCIFImport("Valor CIF Importado US$", "container", response.charData);
                    }
                    if (response.chartPrecioUnit != null) {
                        Chart.PrecioProm("Precio Promedio US$ / KILOGRAMOS", "container2", response.chartPrecioUnit);
                    }
                    if (response.chartPrecioUnit == null) {
                        $("#PrecioUnit").css("display","none");
                    }
                    if (response.chartCompCif != null) {
                        Chart.CompCIFImport("Comparativo CIF US$", 'container3', response.chartCompCif);
                    }
                    if (response.pieRanking != null) {
                        Chart.RankingImp("Principales Importadores", 'piecontainer1', response.pieRanking);
                    }
                    if (response.pieRanking == null) {
                        $("#TopImpExp").css("display", "none");
                    }
                    if (response.pieRanking2 != null) {
                        Chart.RankingPais("Principales Países de Origen", 'piecontainer2', response.pieRanking2);
                    }
                } else {
                    if (response.charData != null) {
                        Chart.ValorCIFImport("Imported CIF US$", "container", response.charData);
                    }
                    if (response.chartPrecioUnit != null) {
                        Chart.PrecioProm("Average Price US$ / KILOGRAMS", "container2", response.chartPrecioUnit);
                    }
                    if (response.chartPrecioUnit == null) {
                        $("#PrecioUnit").css("display","none");
                    }
                    if (response.chartCompCif != null) {
                        Chart.CompCIFImport("Comparative CIF US$", 'container3', response.chartCompCif);
                    }
                    if (response.pieRanking != null) {
                        Chart.RankingImp("Top Importers", 'piecontainer1', response.pieRanking);
                    }
                    if (response.pieRanking == null) {
                        $("#TopImpExp").css("display", "none");
                    }
                    if (response.pieRanking2 != null) {
                        Chart.RankingPais("Top Origin Countries", 'piecontainer2', response.pieRanking2);
                    }
                }
            }
            else {
                if (pIdioma == "es") {
                    if (response.charData != null) {
                        Chart.ValorCIFImport("Valor FOB Exportado US$", "container1", response.charData);
                    }
                    if (response.chartPrecioUnit != null) {
                        Chart.PrecioProm("Precio Promedio US$ / KILOGRAMOS", "container_2", response.chartPrecioUnit);
                    }
                    if (response.chartPrecioUnit == null) {
                        $("#PrecioUnit").css("display","none");
                    }
                    if (response.chartCompCif != null) {
                        Chart.CompCIFImport("Comparativo FOB US$", 'container_3', response.chartCompCif);
                    }
                    if (response.pieRanking != null) {
                        Chart.RankingImp("Principales Exportadores", 'piecontainer1', response.pieRanking);
                    }
                    if (response.pieRanking == null) {
                        $("#TopImpExp").css("display", "none");
                    }
                    if (response.pieRanking2 != null) {
                        Chart.RankingPais("Principales Países de Destino", 'piecontainer2', response.pieRanking2);
                    }
                } else {
                    if (response.charData != null) {
                        Chart.ValorCIFImport("Exported FOB Value US$", "container1", response.charData);
                    }
                    if (response.chartPrecioUnit != null) {
                        Chart.PrecioProm("Average Price US$ / KILOGRAMS", "container_2", response.chartPrecioUnit);
                    }
                    if (response.chartPrecioUnit == null) {
                        $("#PrecioUnit").css("display","none");
                    }
                    if (response.chartCompCif != null) {
                        Chart.CompCIFImport("Comparative FOB US$", 'container_3', response.chartCompCif);
                    }
                    if (response.pieRanking != null) {
                        Chart.RankingImp("Top Exporters", 'piecontainer1', response.pieRanking);
                    } if (response.pieRanking == null) {
                        $("#TopImpExp").css("display", "none");
                    }
                    if (response.pieRanking2 != null) {
                        Chart.RankingPais("Top Destination Countries", 'piecontainer2', response.pieRanking2);
                    }
                }
            }
            var IdProducto = $("#slug").val();
            var IdPaisAduana = $("#slug2").val();
            Paging.Paginator(urlReg, IdProducto, IdPaisAduana);
            //Paging.pagingClickListener(IdProducto, IdPaisAduana);
            Paging.PaginatorB(urlPais, IdProducto, IdPaisAduana);
            //Paging.pagingClickListenerB(IdProducto, IdPaisAduana);

            $('#countries.owl-carousel .item').each(function (index) {
                if ($('.pais', this).val() == $("#slug2").val()) {
                    $(this).removeClass("active");
                    $(this).addClass("active");
                }
            });
            LoadingAdminPage.showOrHideLoadingPage(false);
            Size.TableDetalleBox();
        },
        error: function (dataError) {
            console.log(dataError);
        }
    });
}
Busqueda.CargarGraficos = function (urlPost, pIdioma, tipoOpe, urlB, urlC) {
    $.ajax({
        url: urlPost,
        type: "POST",
        data: {
            IdProducto: $("#slug").val(),
            IdPaisAduana: $("#slug2").val(),
            TipoOpe: tipoOpe,
            idioma: pIdioma
        },
        beforeSend: function () {
            LoadingAdminPage.showOrHideLoadingPage(true);
        },
        success: function (response) {
            $("#tableComparative").html(response.viewTableComparative);
            $("#tablePrecProm").html(response.viewTablePrecProm);
            if (tipoOpe == "Importaciones") {
                if (pIdioma == "es") {
                    if (response.charData != null) {
                        Chart.ValorCIFImport("Valor CIF Importado US$", "container", response.charData);
                    }
                    if (response.chartPrecioUnit != null) {
                        Chart.PrecioProm("Precio Promedio US$ / KILOGRAMOS", "container2", response.chartPrecioUnit);
                    }
                    if (response.chartPrecioUnit == null){
                        $("#PrecioUnit").css("display","none");
                    }
                    if (response.chartCompCif != null) {
                        Chart.CompCIFImport("Comparativo CIF US$", 'container3', response.chartCompCif);
                    }
                    if (response.pieRanking != null) {
                        Chart.RankingImp("Principales Importadores", 'piecontainer1', response.pieRanking);
                    }
                    if (response.pieRanking == null) {
                        $("#TopImpExp").css("display", "none");
                    }
                    if (response.pieRanking2 != null) {
                        Chart.RankingPais("Principales Países de Origen", 'piecontainer2', response.pieRanking2);
                    }
                } else {
                    if (response.charData != null) {
                        Chart.ValorCIFImport("Imported CIF US$", "container", response.charData);
                    }
                    if (response.chartPrecioUnit != null) {
                        Chart.PrecioProm("Average Price US$ / KILOGRAMS", "container2", response.chartPrecioUnit);
                    }
                    if (response.chartPrecioUnit == null) {
                        $("#PrecioUnit").css("display","none");
                    }
                    if (response.chartCompCif != null) {
                        Chart.CompCIFImport("Comparative CIF US$", 'container3', response.chartCompCif);
                    }
                    if (response.pieRanking != null) {
                        Chart.RankingImp("Top Importers", 'piecontainer1', response.pieRanking);
                    }
                    if (response.pieRanking == null) {
                        $("#TopImpExp").css("display", "none");
                    }
                    if (response.pieRanking2 != null) {
                        Chart.RankingPais("Top Origin Countries", 'piecontainer2', response.pieRanking2);
                    }
                }
            }
            else {
                if (pIdioma == "es") {
                    if (response.charData != null) {
                        Chart.ValorCIFImport("Valor FOB Exportado US$", "container1", response.charData);
                    }
                    if (response.chartPrecioUnit != null) {
                        Chart.PrecioProm("Precio Promedio US$ / KILOGRAMOS", "container_2", response.chartPrecioUnit);
                    }
                    if (response.chartPrecioUnit == null) {
                        $("#PrecioUnit").css("display","none");
                    }
                    if (response.chartCompCif != null) {
                        Chart.CompCIFImport("Comparativo FOB US$", 'container_3', response.chartCompCif);
                    }
                    if (response.pieRanking != null) {
                        Chart.RankingImp("Principales Exportadores", 'piecontainer1', response.pieRanking);
                    }
                    if (response.pieRanking == null) {
                        $("#TopImpExp").css("display", "none");
                    }
                    if (response.pieRanking2 != null) {
                        Chart.RankingPais("Principales Países de Destino", 'piecontainer2', response.pieRanking2);
                    }
                } else {
                    if (response.charData != null) {
                        Chart.ValorCIFImport("Exported FOB Value US$", "container1", response.charData);
                    }
                    if (response.chartPrecioUnit != null) {
                        Chart.PrecioProm("Average Price US$ / KILOGRAMS", "container_2", response.chartPrecioUnit);
                    }
                    if (response.chartPrecioUnit == null) {
                        $("#PrecioUnit").css("display","none");
                    }
                    if (response.chartCompCif != null) {
                        Chart.CompCIFImport("Comparative FOB US$", 'container_3', response.chartCompCif);
                    }
                    if (response.pieRanking != null) {
                        Chart.RankingImp("Top Exporters", 'piecontainer1', response.pieRanking);
                    }
                    if (response.pieRanking == null) {
                        $("#TopImpExp").css("display", "none");
                    }
                    if (response.pieRanking2 != null) {
                        Chart.RankingPais("Top Destination Countries", 'piecontainer2', response.pieRanking2);
                    }
                }
            }
            var IdProducto = $("#slug").val();
            var IdPaisAduana = $("#slug2").val();

            $('#countries.owl-carousel .item').each(function (index) {
                if ($('.pais', this).val() == $("#slug2").val()) {
                    $(this).removeClass("active");
                    $(this).addClass("active");
                }
            });

            Paging.Paginator(urlB, IdProducto, IdPaisAduana);
            ////Paging.pagingClickListener(IdProducto, IdPaisAduana);
            Paging.PaginatorB(urlC, IdProducto, IdPaisAduana);
            //Paging.pagingClickListenerB(IdProducto, IdPaisAduana);
            LoadingAdminPage.showOrHideLoadingPage(false);
            //Size.ChartsTop(response.CountRankPais);
            Size.TableDetalleBox();
            Carousel.Cargar();
        },
        error: function (dataError) {
            console.log(dataError);
        }
    });
}