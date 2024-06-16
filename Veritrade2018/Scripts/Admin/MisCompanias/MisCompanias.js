function Size() {

}

Size.Increment = function() {
        //console.log("entro size");
        var tableHeight = $('#tablePartida .table-responsive table').outerHeight();
        //console.log(tableHeight);
       
        $('#tableOriginOrDestinationCountry .table-responsive table').css('height', tableHeight);
}
function FiltrosMisCompanias() {
}
FiltrosMisCompanias.HiddenContainers = function () {
    $("#divInformaColombia, #divResultsTitle, #chartValorImp, #chartComparative, #divProducts, #divOriginOrDestinationCountry, #divSupplierOrImporterExp")
        .addClass("no-display");
}

FiltrosMisCompanias.ClearFields = function () {
    $("#themesChartValorImp, #themesChartComparative, #themesChartProducts, #themesChartOriginOrDestinationCountry, #themesChartSupplierOrImporterExp")
        .html("");

    $("div.dropdown.dropdown-colorselector > ul.dropdown-menu.dropdown-caret  li a")
        .removeClass("selected");
    $(".dropdown-menu").find("li:first > a").addClass("selected");
    $("div.dropdown.dropdown-colorselector a.dropdown-toggle span")
        .css("background-color", "#4d86bd");
}

function TabMisCompanias() {
}

TabMisCompanias.ClickArancelesPartida = function (descripcion) {

    var firstChar = descripcion.match('[a-zA-Z]');
    var index = descripcion.indexOf(firstChar);

    if (index > 0) {
        var cod_partida = descripcion.substring(0, descripcion.indexOf(' '));
    } else {
        var cod_partida = descripcion;
    }

    console.log("entro");


    $.ajax({
        type: "POST",
        url: '/es/mis-busquedas/VerArancelesPartida',
        data: {
            cod_partida: cod_partida
        },
        crossDomain: true,
        beforeSend: function () {
            LoadingAdminPage.showOrHideLoadingPage(true);
        },
        success: function (response) {

            $("#subtituloPartidaArancel").html("");
            $("#subtituloPartidaArancel").html(response.fontDetallePartidaArancel);
            $("#detallePartidaArancel").html("");
            $("#detallePartidaArancel").html(response.divDetallePartidaArancel);

            LoadingAdminPage.showOrHideLoadingPage(false);

            ModalAdmin.registerShowByShowOption("ModalInfoArancelesPartida", true);
        },
        error: function (dataError) {
            console.log(dataError);
        }
    });
};

TabMisCompanias.VerGraficos = function (urlVerGraficos,
    urlPagingTables,
    urlVerRegistros,
    urlVerRegistrosPaging,
    urlBuscarPorDesComercial,
    urlRegistrosByCategory,
    urlRegistrosByCategoryAndSerie,
    urlRegistrosByNandina) {
    var fechas = FiltrosTabMis.getFechaIniAndFechaFin();
    $.ajax({
        type: "POST",
        url: urlVerGraficos,
        data: {
            textPais: $("#cboPais option:selected").text(),
            idEmpresa: $("#cboMyFilters").val(),
            textEmpresa: $("#cboMyFilters option:selected").text(),
            isCheckedUSD: $("#rdbUSD").is(":checked"),
            opcion: $("#cboOpcion").val(),
            anioIniAnioMesIni: Fecha.getYear($('#cboAnioMesIni').datepicker('getStartDate')),
            fechaFinAnioMesFin: Fecha.getYearAndMonthByUTC($('#cboAnioMesIni').datepicker('getEndDate')),
            fechaIni: fechas[0],
            fechaFin: fechas[1]
        },
        beforeSend: function () {
            LoadingAdminPage.showOrHideLoadingPage(true);
        },
        success: function (response) {

            if (response.lblMessage != "") {
                LoadingAdminPage.showOrHideLoadingPage(false);
                ModalAdmin.showVentanaMensajeWithTitleAndMessage("ModalVentanaMensaje",
                    "messageTitle",
                    "Veritrade",
                    "message",
                    response.lblMessage,
                    "lnkContactenos",
                    false);
            } else {
                var symbol = "$";
                if (!$("#rdbUSD").is(":checked")) {
                    symbol = "KG"
                }
                gArrayChartData = []; //clear variable chartData
                var auxChartData = {};

                var objMyCompanie = response.objMyCompanie;
                
                console.log(response.informaColombiaHTML);
                if (response.informaColombiaHTML) {
                    $("#divInformaColombia").removeClass("no-display");
                    $("#informaColombiaContent").html(response.informaColombiaHTML);
                }
                
                $("#titleResults").text(response.lblTitle);
                if (response.opcionesDeDescarga != null) {
                    AdminPage.loadDataComboBox(response.opcionesDeDescarga, "cboDescargas");
                }

                $("#titleValorImp").text(response.chartValorImp.TitleContainer);
                Chart.LoadCboThemes("themesChartValorImp");
                $('#themesChartValorImp').colorselector();
                Chart.getTypeColumn("containerValorImp",
                    response.chartValorImp,
                    urlRegistrosByCategory,
                    urlVerRegistrosPaging,
                    objMyCompanie.CountVisiblePages,
                    urlBuscarPorDesComercial, symbol);

                auxChartData.filtro = "containerValorImp";
                auxChartData.value = response.chartValorImp;
                gArrayChartData.push(auxChartData);

                Chart.ChangeThemeTypeColumn("themesChartValorImp",
                    urlRegistrosByCategory,
                    urlVerRegistrosPaging,
                    objMyCompanie.CountVisiblePages,
                    urlBuscarPorDesComercial);


                $("#titleComparative").text(response.chartComparative.TitleContainer);
                Chart.LoadCboThemes("themesChartComparative");
                $('#themesChartComparative').colorselector();
               
                Chart.getTypeLineWithManySeries("containerComparative",
                    response.chartComparative,
                    urlRegistrosByCategoryAndSerie,
                    urlVerRegistrosPaging,
                    objMyCompanie.CountVisiblePages,
                    urlBuscarPorDesComercial, symbol);

                auxChartData = {};
                auxChartData.filtro = "containerComparative";
                auxChartData.value = response.chartComparative;
                gArrayChartData.push(auxChartData);

                Chart.ChangeThemeTypeLineWithManySeries("themesChartComparative",
                    urlRegistrosByCategoryAndSerie,
                    urlVerRegistrosPaging,
                    objMyCompanie.CountVisiblePages,
                    urlBuscarPorDesComercial);


                $("#titleProducts").text(response.chartProducts.TitleContainer);
                Chart.LoadCboThemes("themesChartProducts");
                $('#themesChartProducts').colorselector();
                ChartsMyCompanies.getTypeColumnPartidas("containerColumnProducts",
                    response.chartProducts,
                    urlRegistrosByNandina,
                    urlVerRegistrosPaging,
                    objMyCompanie.CountVisiblePages,
                    urlBuscarPorDesComercial);

                auxChartData = {};
                auxChartData.filtro = "containerColumnProducts";
                auxChartData.value = response.chartProducts;
                gArrayChartData.push(auxChartData);

                ChartsMyCompanies.ChangeThemeTypeColumnPartidas("themesChartProducts",
                    urlRegistrosByNandina,
                    urlVerRegistrosPaging,
                    objMyCompanie.CountVisiblePages,
                    urlBuscarPorDesComercial);

                $("#tablePartida").html(objMyCompanie.HtmlTableProducts);

                var wpP = $("#tablePartida");
                setTimeout(AdminPage.applyDot(wpP, 2, "right"), 500);

                if (objMyCompanie.TotalPaginasProducts > 0) {
                    //TablesOfMyCompanie.RegisterPaging(urlPagingTables,
                    //    objMyCompanie.IdProducts,
                    //    objMyCompanie.TotalPaginasProducts,
                    //    objMyCompanie.CountVisiblePages);

                    TablesTabsMis.RegisterPaging(urlPagingTables,
                        objMyCompanie.IdProducts,
                        objMyCompanie.TotalPaginasProducts,
                        objMyCompanie.CountVisiblePages);
                }


                $("#divResultsTitle, #chartValorImp, #chartComparative, #divProducts").removeClass("no-display");

                if (objMyCompanie.ExistOriginOrDestinationCountry) {
                    var chartOriginOrDestinationCountry = objMyCompanie.ChartOriginOrDestinationCountry;
                    $("#titleOriginOrDestinationCountry").text(chartOriginOrDestinationCountry.TitleContainer);

                    Chart.LoadCboThemes("themesChartOriginOrDestinationCountry");
                    $('#themesChartOriginOrDestinationCountry').colorselector();
                    Chart.getTypePie("pieOriginOrDestinationCountry",
                        chartOriginOrDestinationCountry,
                        urlVerRegistros,
                        urlVerRegistrosPaging,
                        objMyCompanie.CountVisiblePages,
                        urlBuscarPorDesComercial,
                        objMyCompanie.IdOriginOrDestinationCountry);

                    auxChartData = {};
                    auxChartData.filtro = "pieOriginOrDestinationCountry";
                    auxChartData.value = chartOriginOrDestinationCountry;
                    gArrayChartData.push(auxChartData);

                    Chart.ChangeThemeTypePie("themesChartOriginOrDestinationCountry",
                        urlVerRegistros,
                        urlVerRegistrosPaging,
                        objMyCompanie.CountVisiblePages,
                        urlBuscarPorDesComercial,
                        objMyCompanie.IdOriginOrDestinationCountry);

                    $("#tableOriginOrDestinationCountry").html(objMyCompanie.HtmlTableOriginOrDestinationCountry);

                    var wpOCoDC = $("#tableOriginOrDestinationCountry");
                    setTimeout(AdminPage.applyDot(wpOCoDC, 2, "right"), 500);

                    if (objMyCompanie.TotalPagesOriginOrDestinationCountry > 0) {
                        //TablesOfMyCompanie.RegisterPaging(urlPagingTables,
                        //    objMyCompanie.IdOriginOrDestinationCountry,
                        //    objMyCompanie.TotalPagesOriginOrDestinationCountry,
                        //    objMyCompanie.CountVisiblePages);

                        TablesTabsMis.RegisterPaging(urlPagingTables,
                            objMyCompanie.IdOriginOrDestinationCountry,
                            objMyCompanie.TotalPagesOriginOrDestinationCountry,
                            objMyCompanie.CountVisiblePages);
                    }


                    $("#divOriginOrDestinationCountry").removeClass("no-display");
                }

                if (objMyCompanie.ExistSupplierOrImporterExp) {
                    var chartSupplierOrImpoerterExp = objMyCompanie.ChartSupplierOrImporterExp;
                    $("#titleSupplierOrImporterExp").text(chartSupplierOrImpoerterExp.TitleContainer);

                    Chart.LoadCboThemes("themesChartSupplierOrImporterExp");
                    $('#themesChartSupplierOrImporterExp').colorselector();
                    Chart.getTypePie("pieSupplierOrImporterExp",
                        chartSupplierOrImpoerterExp,
                        urlVerRegistros,
                        urlVerRegistrosPaging,
                        objMyCompanie.CountVisiblePages,
                        urlBuscarPorDesComercial,
                        objMyCompanie.IdSupplierOrImporterExp);

                    auxChartData = {};
                    auxChartData.filtro = "pieSupplierOrImporterExp";
                    auxChartData.value = chartSupplierOrImpoerterExp;
                    gArrayChartData.push(auxChartData);

                    Chart.ChangeThemeTypePie("themesChartSupplierOrImporterExp",
                        urlVerRegistros,
                        urlVerRegistrosPaging,
                        objMyCompanie.CountVisiblePages,
                        urlBuscarPorDesComercial,
                        objMyCompanie.IdSupplierOrImporterExp);

                    $("#tableSupplierOrImporterExp").html(objMyCompanie.HtmlTableSupplierOrImporterExp);
                    var wpSoIE = $("#tableSupplierOrImporterExp");
                    setTimeout(AdminPage.applyDot(wpSoIE, 2, "right"), 500);

                    if (objMyCompanie.TotalPagesSupplierOrImporterExp > 0) {
                        //TablesOfMyCompanie.RegisterPaging(urlPagingTables,
                        //    objMyCompanie.IdSupplierOrImporterExp,
                        //    objMyCompanie.TotalPagesSupplierOrImporterExp,
                        //    objMyCompanie.CountVisiblePages);

                        TablesTabsMis.RegisterPaging(urlPagingTables,
                            objMyCompanie.IdSupplierOrImporterExp,
                            objMyCompanie.TotalPagesSupplierOrImporterExp,
                            objMyCompanie.CountVisiblePages);
                    }

                    $("#divSupplierOrImporterExp").removeClass("no-display");
                }


                TablesTabsMis.VerRegistro(urlVerRegistros,
                    urlVerRegistrosPaging,
                    objMyCompanie.CountVisiblePages,
                    urlBuscarPorDesComercial);

                LoadingAdminPage.showOrHideLoadingPage(false);
                if (response.objMensajeCantRegMax != null) {
                    ModalAdmin.showVentanaMensajeWithTitleAndMessage("ModalVentanaMensaje",
                        "messageTitle",
                        response.objMensajeCantRegMax.titulo,
                        "message",
                        response.objMensajeCantRegMax.mensaje,
                        "lnkContactenos",
                        response.objMensajeCantRegMax.flagContactenos);

                    $("#cboDescargas").addClass("no-display");
                    $("#downloadAllExcelFile").addClass("no-display");
                } else {
                    $("#cboDescargas").removeClass("no-display");
                    $("#downloadAllExcelFile").removeClass("no-display");
                }
                //console.log(response.difCantidad)
                if (response.difCantidad < 4)
                    Size.Increment();
            }
        },
        error: function (dataError) {
            console.info(dataError);
        }
    });
}


function TablesOfMyCompanie() {
}

TablesOfMyCompanie.RegisterPaging = function (urlPaging,
    idPaging,
    totalPages,
    numVisiblePages) {

    $('#paging' + idPaging).twbsPagination({
        totalPages: totalPages,
        visiblePages: numVisiblePages,
        hideOnlyOnePage: true,
        initiateStartPageClick: false,
        onPageClick: function (event, page) {
            $.ajax({
                type: "POST",
                url: urlPaging,
                data: {
                    idFiltro: idPaging,
                    pagina: page
                },
                beforeSend: function () {
                    LoadingAdminPage.showOrHideLoadingPage(true);
                },
                success: function (response) {
                    $("#tbody" + idPaging).html(response.rowsByPage);
                    LoadingAdminPage.showOrHideLoadingPage(false);

                    //Size.Increment();
                    //Size.Increment();
                    //setTimeout(applyDot(wp, (pTipoFiltro == "Tab" && pFiltro == "Partida" ? 2 : 1)), 500);
                },
                error: function (data) {
                    console.log(data);
                }
            });
        }
    });

}

function ChartsMyCompanies() {
}

ChartsMyCompanies.getTypeColumnPartidas = function (idContainer,
    objChartData,
    urlRegistrosByNandina,
    urlVerRegistrosPaging,
    visiblePages,
    urlBuscarPorDesComercial) {
    var symbol = "$";
    if (!$("#rdbUSD").is(":checked")) {
        symbol="KG"
    }
    var symbol_2 = "";
    if (symbol != "$") {
        symbol_2 = symbol;
        symbol = "";
    }
    Highcharts.chart(idContainer,
        {
            title: {
                text: objChartData.Title
            },
            legend: {
                enabled: false
            },
            xAxis: {
                categories: objChartData.Categories,
                crosshair: true
            },
            yAxis: {
                min: 0,
                title: {
                    text: ''
                },
                labels: {
                    formatter: function () {
                        return symbol + this.value.toLocaleString('en') + symbol_2;
                    }
                }
            },
            tooltip: {
                //headerFormat: '',
                //pointFormat: '<b>{series.name}:</b> $ {point.y:,.0f}'
                formatter: function () {
                    var yVal = this.y;
                    return "<b>" + this.x + '</b>: ' + symbol + Highcharts.numberFormat(this.point.y, 0, '.', ',') + symbol_2; //  yVal.toLocaleString('en');
                }
            },
            plotOptions: {
                column: {
                    pointPadding: 0,
                    borderWidth: 0
                },
                series: {
                    cursor: 'pointer',
                    point: {
                        events: {
                            click: function () {
                                var vCategory = this.category;
                                if (vCategory.indexOf("[") < 0) {
                                    ChartsMyCompanies.clickSeriesPointPartidas(urlRegistrosByNandina,
                                        urlVerRegistrosPaging,
                                        visiblePages,
                                        urlBuscarPorDesComercial,
                                        vCategory);

                                // JANAQ Clic Grafico 200720
                                if (typeof MixPanel.clickGrafico == 'function') {
                                    MixPanel.clickGrafico(idContainer);
                                }
                                }
                            }
                        }
                    }
                }
            },
            series: [
                {
                    type: 'column',
                    data: objChartData.Series[0].data
                }
            ],
            exporting: {
                buttons: {
                    contextButton: {
                         // JANAQ Clic Exportar/Imprimir Grafico 190720
                        menuItems: [{
                            text: gLangOptionHighcharts.downloadPNG,
                            onclick: function () {

                                if (typeof MixPanel.exportarGrafico == 'function') {
                                    MixPanel.exportarGrafico(gOptionGrafico.downloadPNG, idContainer);
                                }
                                this.exportChart();
                            }
                        },
                            'separator',
                            {
                            text: gLangOptionHighcharts.printChart,
                            onclick: function () {

                                if (typeof MixPanel.exportarGrafico == 'function') {
                                    MixPanel.exportarGrafico(gOptionGrafico.printChart, idContainer);
                                }
                                this.print();
                                }
                            }],
                        symbol: 'download',
                        symbolFill: '#54b6e8',
                        _titleKey: "exportMenuButtonTitle"
                    }
                }
            }
        });
}

ChartsMyCompanies.clickSeriesPointPartidas = function (urlVerRegistrosByPoint,
    urlVerRegistrosPaging,
    visiblePages,
    urlBuscarPorDesComercial,
    pCategory) {

    $.ajax({
        type: "POST",
        url: urlVerRegistrosByPoint,
        data: {
            nandina: pCategory
        },
        beforeSend: function () {
            LoadingAdminPage.showOrHideLoadingPage(true);
        },
        success: function (response) {
            TablesTabsMis.LoadDataVerRegistros(response,
                urlVerRegistrosPaging,
                visiblePages,
                urlBuscarPorDesComercial,
                true);
        },
        error: function (dataError) {
            console.log(dataError);
            //LoadingAdminPage.showOrHideLoadingPage(false);
        }
    });
}

ChartsMyCompanies.ChangeThemeTypeColumnPartidas = function (idCboTheme,
    urlRegistrosByNandina,
    urlVerRegistrosPaging,
    visiblePages,
    urlBuscarPorDesComercial) {

    $("#" + idCboTheme).change(function () {
        var vFiltro = $(this).data("filtro");
        var idTheme = $(this).val();
        var valueTheme = dbThemes.find(x => x.id == idTheme).value;

        Highcharts.setOptions(valueTheme);
        //var chartTitle = $("#" + "pieChart" + vFiltro).highcharts().options.title.text;
        var chartData = gArrayChartData.find(y => y.filtro == vFiltro).value;

        Highcharts.setOptions({
            colors: Highcharts.map(Highcharts.getOptions().colors, function (color) {
                return {
                    radialGradient: {
                        cx: 0.5,
                        cy: 0.3,
                        r: 0.7
                    },
                    stops: [
                        [0, color],
                        [1, Highcharts.Color(color).brighten(-0.3).get('rgb')] // darken
                    ]
                };
            })
        });

        ChartsMyCompanies.getTypeColumnPartidas(vFiltro,
            chartData,
            urlRegistrosByNandina,
            urlVerRegistrosPaging,
            visiblePages,
            urlBuscarPorDesComercial);
    });
}