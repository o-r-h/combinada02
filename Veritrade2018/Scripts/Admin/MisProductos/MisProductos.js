function Size() {

}

Size.Increment = function () {
    //console.log("entro size")
    //console.log($('#tablePartida .table-responsive table').outerHeight());
    var tableHeight = $('#tableImporterOrExporter .table-responsive table').outerHeight();
    //console.log(tableHeight);

    $('#tableOriginOrDestinationCountry .table-responsive table').css('height', tableHeight);
}

function FiltrosMisProductos(){
}

FiltrosMisProductos.HiddenContainers = function() {
    $("#divResultsTitle, #periodSummary, #chartMPValorImp,  #chartMPPricesProm, #chartMPComparative, #divImporterOrExporter, #divOriginOrDestinationCountry, #divSupplierOrImporterExp")
        .addClass("no-display");
}

FiltrosMisProductos.ClearFields = function () {
    $("#themesChartMPValorImp, #themesChartMPPricesProm, #themesChartMPComparative, #themesChartImporterOrExporter, #themesChartOriginOrDestinationCountry, #themesChartSupplierOrImporterExp").html("");
    
    $("div.dropdown.dropdown-colorselector > ul.dropdown-menu.dropdown-caret  li a")
        .removeClass("selected");
    $(".dropdown-menu").find("li:first > a").addClass("selected");
    $("div.dropdown.dropdown-colorselector a.dropdown-toggle span")
        .css("background-color", "#4d86bd");
}


function TabMisProductos() {
}

$("#btnVerArancelesPartida").click(function () {
    var descripcion = $("#cboMyFilters option:selected").text();

    var firstChar = descripcion.match('[a-zA-Z]');
    var index = descripcion.indexOf(firstChar);

    if (index > 0) {
        var cod_partida = descripcion.substring(0, descripcion.indexOf(' '));
    } else {
        var cod_partida = descripcion;
    }
    console.log("Partida: " + cod_partida);

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
});

TabMisProductos.verGraficos = function (urlVerGraficos,
    urlPagingTables,
    urlVerRegistros,
    urlVerRegistrosPaging,
    urlBuscarPorDesComercial,
    urlRegistrosByChartSeriesPoint,
    urlRegistrosByCategoryAndSerie)
{
    var fechas = FiltrosTabMis.getFechaIniAndFechaFin();

    //Identifica grupo de partidas

    var index = $("#cboMyFilters option:selected").text().indexOf("[G]");

    var codPais = $("#cboPais").val();

    if ((index >= 0) || (codPais != "PE")) {
        $("#btnVerArancelesPartida").addClass("hidden");
    } else {
        $("#btnVerArancelesPartida").removeClass("hidden");
    }

    $.ajax({
        type: "POST",
        url: urlVerGraficos,
        data: {
            textPais: $("#cboPais option:selected").text(),
            idPartida: $("#cboMyFilters").val(),
            textPartida: $("#cboMyFilters option:selected").text(),
            isCheckedUSD: $("#rdbUSD").is(":checked"),
            textUnid: $("#descUnidad").text(),
            opcion: $("#cboOpcion").val(),
            anioIniAnioMesIni: Fecha.getYear($('#cboAnioMesIni').datepicker('getStartDate')),
            fechaFinAnioMesFin : Fecha.getYearAndMonthByUTC($('#cboAnioMesIni').datepicker('getEndDate')),
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

                var objMyProducts = response.objMyProducts;

                var auxChartData = {};

                $("#periodSummary").html(response.resumenPeriodo);
                $("#titleResults").text(response.lblTitle);

                if (response.opcionesDeDescarga != null) {
                    AdminPage.loadDataComboBox(response.opcionesDeDescarga, "cboDescargas");
                }

                $("#titleValorImp").text(response.chartMyProductsValorImp.TitleContainer);
                Chart.LoadCboThemes("themesChartMPValorImp");
                $('#themesChartMPValorImp').colorselector();

                Chart.getTypeColumn("containerMPValorImp",
                    response.chartMyProductsValorImp,
                    urlRegistrosByChartSeriesPoint,
                    urlVerRegistrosPaging,
                    objMyProducts.CountVisiblePages,
                    urlBuscarPorDesComercial, symbol);

                auxChartData.filtro = "containerMPValorImp";
                auxChartData.value = response.chartMyProductsValorImp;
                gArrayChartData.push(auxChartData);

                if (response.chartMyProductsPricesProm != null) {
                    Chart.ChangeThemeTypeColumn("themesChartMPValorImp",
                        urlRegistrosByChartSeriesPoint,
                        urlVerRegistrosPaging,
                        objMyProducts.CountVisiblePages,
                        urlBuscarPorDesComercial);

                    $("#titlePricesProm").text(response.chartMyProductsPricesProm.TitleContainer);
                    Chart.LoadCboThemes("themesChartMPPricesProm");
                    $('#themesChartMPPricesProm').colorselector();

                    Chart.getTypeLineWithDataLabels("containerMPPricesProm",
                        response.chartMyProductsPricesProm,
                        urlRegistrosByChartSeriesPoint,
                        urlVerRegistrosPaging,
                        objMyProducts.CountVisiblePages,
                        urlBuscarPorDesComercial);

                    auxChartData = {};
                    auxChartData.filtro = "containerMPPricesProm";
                    auxChartData.value = response.chartMyProductsPricesProm;
                    gArrayChartData.push(auxChartData);

                    Chart.ChangeThemeTypeLineWithDataLabels("themesChartMPPricesProm",
                        urlRegistrosByChartSeriesPoint,
                        urlVerRegistrosPaging,
                        objMyProducts.CountVisiblePages,
                        urlBuscarPorDesComercial);
                    $("#chartMPPricesProm").removeClass("no-display");
                } else {
                    $("#chartMPPricesProm").addClass("no-display");
                }             

                $("#titleComparative").text(response.chartMyProductsComparative.TitleContainer);
                Chart.LoadCboThemes("themesChartMPComparative");
                $('#themesChartMPComparative').colorselector();

                Chart.getTypeLineWithManySeries("containerMPComparative",
                    response.chartMyProductsComparative,
                    urlRegistrosByCategoryAndSerie,
                    urlVerRegistrosPaging,
                    objMyProducts.CountVisiblePages,
                    urlBuscarPorDesComercial, symbol);

                auxChartData = {};
                auxChartData.filtro = "containerMPComparative";
                auxChartData.value = response.chartMyProductsComparative;
                gArrayChartData.push(auxChartData);

                Chart.ChangeThemeTypeLineWithManySeries("themesChartMPComparative",
                    urlRegistrosByCategoryAndSerie,
                    urlVerRegistrosPaging,
                    objMyProducts.CountVisiblePages,
                    urlBuscarPorDesComercial);

                $("#divResultsTitle, #periodSummary, #chartMPValorImp, #chartMPComparative ").removeClass("no-display");                

                if (objMyProducts.ExistImporterOrExporter) {
                    var chartImporterOrExporter = objMyProducts.ChartImporterOrExporter;
                    
                    $("#titleImporterOrExporter").text(chartImporterOrExporter.TitleContainer);

                    Chart.LoadCboThemes("themesChartImporterOrExporter");
                    $('#themesChartImporterOrExporter').colorselector();

                    Chart.getTypePie("pieImporterOrExporter",
                        chartImporterOrExporter,
                        urlVerRegistros,
                        urlVerRegistrosPaging,
                        objMyProducts.CountVisiblePages,
                        urlBuscarPorDesComercial,
                        objMyProducts.IdImporterOrExporter);

                    auxChartData = {};
                    auxChartData.filtro = "pieImporterOrExporter";
                    auxChartData.value = chartImporterOrExporter;
                    gArrayChartData.push(auxChartData);

                    Chart.ChangeThemeTypePie("themesChartImporterOrExporter",
                        urlVerRegistros,
                        urlVerRegistrosPaging,
                        objMyProducts.CountVisiblePages,
                        urlBuscarPorDesComercial,
                        objMyProducts.IdImporterOrExporter);

                    $("#tableImporterOrExporter").html(objMyProducts.HtmlTableImporterOrExporter);
                    var wpIoE = $("#tableImporterOrExporter");
                    setTimeout(AdminPage.applyDot(wpIoE, 2, "right"), 500);

                    if (objMyProducts.TotalPagesImporterOrExporter > 0) {
                        //TablesOfMyProducts.RegisterPaging(urlPagingTables,
                        //    objMyProducts.IdImporterOrExporter,
                        //    objMyProducts.TotalPagesImporterOrExporter,
                        //    objMyProducts.CountVisiblePages);

                        TablesTabsMis.RegisterPaging(urlPagingTables,
                            objMyProducts.IdImporterOrExporter,
                            objMyProducts.TotalPagesImporterOrExporter,
                            objMyProducts.CountVisiblePages);
                    }

                    $("#divImporterOrExporter").removeClass("no-display");
                }

                if (objMyProducts.ExistOriginOrDestinationCountry) {
                    var chartOriginOrDestinationCountry = objMyProducts.ChartOriginOrDestinationCountry;
                    $("#titleOriginOrDestinationCountry").text(chartOriginOrDestinationCountry.TitleContainer);

                    Chart.LoadCboThemes("themesChartOriginOrDestinationCountry");
                    $('#themesChartOriginOrDestinationCountry').colorselector();

                    Chart.getTypePie("pieOriginOrDestinationCountry",
                        chartOriginOrDestinationCountry,
                        urlVerRegistros,
                        urlVerRegistrosPaging,
                        objMyProducts.CountVisiblePages,
                        urlBuscarPorDesComercial,
                        objMyProducts.IdOriginOrDestinationCountry);

                    auxChartData = {};
                    auxChartData.filtro = "pieOriginOrDestinationCountry";
                    auxChartData.value = chartOriginOrDestinationCountry;

                    gArrayChartData.push(auxChartData);

                    Chart.ChangeThemeTypePie("themesChartOriginOrDestinationCountry",
                        urlVerRegistros,
                        urlVerRegistrosPaging,
                        objMyProducts.CountVisiblePages,
                        urlBuscarPorDesComercial,
                        objMyProducts.IdOriginOrDestinationCountry);

                    $("#tableOriginOrDestinationCountry").html(objMyProducts.HtmlTableOriginOrDestinationCountry);
                    var wpOCoDC = $("#tableOriginOrDestinationCountry");
                    setTimeout(AdminPage.applyDot(wpOCoDC, 2, "right"), 500);
                    if (objMyProducts.TotalPagesOriginOrDestinationCountry > 0) {
                        //TablesOfMyProducts.RegisterPaging(urlPagingTables,
                        //    objMyProducts.IdOriginOrDestinationCountry,
                        //    objMyProducts.TotalPagesOriginOrDestinationCountry,
                        //    objMyProducts.CountVisiblePages);

                        TablesTabsMis.RegisterPaging(urlPagingTables,
                            objMyProducts.IdOriginOrDestinationCountry,
                            objMyProducts.TotalPagesOriginOrDestinationCountry,
                            objMyProducts.CountVisiblePages);
                    }
                    $("#divOriginOrDestinationCountry").removeClass("no-display");
                }

                if (objMyProducts.ExistSupplierOrImporterExp) {
                    var chartSupplierOrImpoerterExp = objMyProducts.ChartSupplierOrImporterExp;
                    $("#titleSupplierOrImporterExp").text(chartSupplierOrImpoerterExp.TitleContainer);

                    Chart.LoadCboThemes("themesChartSupplierOrImporterExp");
                    $('#themesChartSupplierOrImporterExp').colorselector();

                    Chart.getTypePie("pieSupplierOrImporterExp",
                        chartSupplierOrImpoerterExp,
                        urlVerRegistros,
                        urlVerRegistrosPaging,
                        objMyProducts.CountVisiblePages,
                        urlBuscarPorDesComercial,
                        objMyProducts.IdSupplierOrImporterExp);

                    auxChartData = {};
                    auxChartData.filtro = "pieSupplierOrImporterExp";
                    auxChartData.value = chartSupplierOrImpoerterExp;

                    gArrayChartData.push(auxChartData);

                    Chart.ChangeThemeTypePie("themesChartSupplierOrImporterExp",
                        urlVerRegistros,
                        urlVerRegistrosPaging,
                        objMyProducts.CountVisiblePages,
                        urlBuscarPorDesComercial,
                        objMyProducts.IdSupplierOrImporterExp);

                    $("#tableSupplierOrImporterExp").html(objMyProducts.HtmlTableSupplierOrImporterExp);
                    var wpSoIE = $("#tableSupplierOrImporterExp");
                    setTimeout(AdminPage.applyDot(wpSoIE, 2, "right"), 500);
                    if (objMyProducts.TotalPagesSupplierOrImporterExp > 0) {
                        //TablesOfMyProducts.RegisterPaging(urlPagingTables,
                        //    objMyProducts.IdSupplierOrImporterExp,
                        //    objMyProducts.TotalPagesSupplierOrImporterExp,
                        //    objMyProducts.CountVisiblePages);

                        TablesTabsMis.RegisterPaging(urlPagingTables,
                            objMyProducts.IdSupplierOrImporterExp,
                            objMyProducts.TotalPagesSupplierOrImporterExp,
                            objMyProducts.CountVisiblePages);
                    }
                    $("#divSupplierOrImporterExp").removeClass("no-display");
                }

                //TablesOfMyProducts.VerRegistro(urlVerRegistros,
                //    urlVerRegistrosPaging,
                //    objMyProducts.CountVisiblePages,
                //    urlBuscarPorDesComercial);

                TablesTabsMis.VerRegistro(urlVerRegistros,
                    urlVerRegistrosPaging,
                    objMyProducts.CountVisiblePages,
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
                if (response.difCantidad < 4)
                    Size.Increment();
            }
            
        },
        error: function (dataError) {
            console.info(dataError);
        }
    });

}

function TablesOfMyProducts() {
}

TablesOfMyProducts.RegisterPaging = function (urlPaging,
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

                    //setTimeout(applyDot(wp, (pTipoFiltro == "Tab" && pFiltro == "Partida" ? 2 : 1)), 500);
                },
                error: function (data) {
                    console.log(data);
                }
            });
        }
    });

}

TablesOfMyProducts.VerRegistro = function (urlVerRegistro,
    urlVerRegistrosPaging,
    visiblePages,
    urlBuscarPorDesComercial) {
    $(document).on('click',
        'a.lnkVerRegistros',
        function () {

            var vFiltro = $(this).data("filtro");
            var vIdRegistro = $(this).data("idregistro");

            var fechas = FiltrosTabMis.getFechaIniAndFechaFin();

            $.ajax({
                type: "POST",
                url: urlVerRegistro,
                data: {
                    idFiltro: vFiltro,
                    idRegistro: vIdRegistro,
                    opcion: $("#cboOpcion").val(),
                    fechaIni: fechas[0],
                    fechaFin: fechas[1]
                },
                beforeSend: function () {
                    LoadingAdminPage.showOrHideLoadingPage(true);
                },
                success: function (response) {

                    TablesOfMyProducts.LoadDataVerRegistros(response,
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
        });
}

TablesOfMyProducts.LoadDataVerRegistros = function (objResponse,
    urlVerRegistrosPaging,
    visiblePages,
    urlBuscarPorDesComercial,
    enabledFiltros) {

    if (objResponse.objMensaje != null) {
        LoadingAdminPage.showOrHideLoadingPage(false);
        ModalAdmin.showVentanaMensajeWithTitleAndMessage("ModalVentanaMensaje",
            "messageTitle",
            objResponse.objMensaje.titulo,
            "message",
            objResponse.objMensaje.mensaje,
            "lnkContactenos",
            objResponse.objMensaje.flagContactenos);
    } else {
        $("#titulo-verRegistro").html(objResponse.tituloRegistros);

        if (objResponse.opcionesDeDescarga != null) {
            AdminPage.loadDataComboBox(objResponse.opcionesDeDescarga, "cboDescargasVerRegistro");
        }

        $("#tablaVerRegistros").html("");
        $("#tablaVerRegistros").html(objResponse.tablaVerRegistros);

        if (objResponse.totalPages > 1) {
            TablesOfMyProducts.RegisterPaging(urlVerRegistrosPaging,
                "VerRegistros",
                objResponse.totalPages,
                visiblePages);
        }

        TablesOfMyProducts.BuscarPorDesComercial(urlBuscarPorDesComercial,
            urlVerRegistrosPaging,
            visiblePages,
            enabledFiltros);

        LoadingAdminPage.showOrHideLoadingPage(false);
        ModalAdmin.registerShowByShowOption("ModalVerRegistro", true);
    }
}

TablesOfMyProducts.BuscarPorDesComercial = function (urlBuscarPorDesComercial,
    urlVerRegistrosPaging,
    visiblePages,
    pEnabledFiltros) {


    $("#lnkBuscarDesComercial, #lnkRestablecerDesComercial").click(function () {
        //console.log("entro")
        var vTxtDesComercial = $("#txtDesComercial").val();
        if (this == $("#lnkRestablecerDesComercial")[0]) {
            vTxtDesComercial = "";
            $("#txtDesComercial").val("");
            $("#lblResultadoDesComercial").text("");
        }

        $.ajax({
            type: "POST",
            url: urlBuscarPorDesComercial,
            data: {
                textDesComercial: vTxtDesComercial,
                enabledFiltros: pEnabledFiltros
            },
            beforeSend: function () {
                LoadingAdminPage.showOrHideLoadingPage(true);
            },
            success: function (response) {
                if (response.objMensaje != null) {
                    LoadingAdminPage.showOrHideLoadingPage(false);
                    ModalAdmin.showVentanaMensajeWithTitleAndMessage("ModalVentanaMensaje",
                        "messageTitle",
                        response.objMensaje.titulo,
                        "message",
                        response.objMensaje.mensaje,
                        "lnkContactenos",
                        response.objMensaje.flagContactenos);
                } else {
                    $("#lblResultadoDesComercial").text(response.resultadoDesComercialVerRegistro);
                    $("#tbodyVerRegistros").html(response.tablaVerRegistro);

                    
                    //console.log($("#txtDesComercial").val() + "  " + vTxtDesComercial)
                    if (vTxtDesComercial != "") {
                        $.each($("#tbodyVerRegistros .wspace-normal"),
                            function() {

                                var arreglo = $(this).html().split("<a");
                                var etiqueta = '<a ' + arreglo[1];
                                var term = vTxtDesComercial;
                                //console.log(term);
                                //var src_str = $(this).html();
                                var src_str = arreglo[0];
                                //var term = "mY text";
                                term = term.replace(/(\s+)/, "(<[^>]+>)*$1(<[^>]+>)*");
                                //console.log(term);
                                var pattern = new RegExp("(" + term + ")", "gi");

                                src_str = src_str.replace(pattern, "<mark>$1</mark>");
                                src_str = src_str.replace(/(<mark>[^<>]*)((<[^>]+>)+)([^<>]*<\/mark>)/,
                                    "$1</mark>$2<mark>$4");

                                //$(this).html(src_str);
                                $(this).html(src_str + etiqueta);
                                //console.log(src_str + etiqueta);
                            });
                    }
                    if (response.totalPages > 1) {
                        $("#pagingVerRegistros").twbsPagination('destroy');
                        $("#divPagingVerRegistros").removeClass("no-display");

                        TablesOfMyProducts.RegisterPaging(urlVerRegistrosPaging,
                            "VerRegistros",
                            response.totalPages,
                            visiblePages);

                        LoadingAdminPage.showOrHideLoadingPage(false);
                    } else {
                        $("#pagingVerRegistros").twbsPagination('destroy');
                        $("#divPagingVerRegistros").addClass("no-display");
                        LoadingAdminPage.showOrHideLoadingPage(false);
                    }
                }
            },
            error: function (dataError) {
                console.log(dataError);
                //LoadingAdminPage.showOrHideLoadingPage(false);
            }
        });

    });
}