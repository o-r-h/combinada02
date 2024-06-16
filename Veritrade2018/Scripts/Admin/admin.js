
/**
 * Funciones de Loading
 */
function LoadingAdminPage() {
}

LoadingAdminPage.showOrHideLoadingPage = function (pEstado) {
    if (pEstado) {
        $("#loadingPageAdmin").addClass("is-active-loading");
    } else {
        $("#loadingPageAdmin").removeClass("is-active-loading");
    }
}

function AdminPage() {
}

AdminPage.getBrowserInfo = function () {
    var ua = navigator.userAgent, tem,
        M = ua.match(/(opera|chrome|safari|firefox|msie|trident(?=\/))\/?\s*(\d+)/i) || [];
    if (/trident/i.test(M[1])) {
        tem = /\brv[ :]+(\d+)/g.exec(ua) || [];
        return 'IE ' + (tem[1] || '');
    }
    if (M[1] === 'Chrome') {
        tem = ua.match(/\b(OPR|Edge)\/(\d+)/);
        if (tem != null) return tem.slice(1).join(' ').replace('OPR', 'Opera');
    }
    M = M[2] ? [M[1], M[2]] : [navigator.appName, navigator.appVersion, '-?'];
    if ((tem = ua.match(/version\/(\d+)/i)) != null) M.splice(1, 1, tem[1]);
    return M.join(' ');
};

AdminPage.loadDataComboBox = function(optionList, idComboBox) {
    $("select#" + idComboBox).html("");
    $.each(optionList,
        function (index, val) {
            $("select#" + idComboBox).append("<option value=" + val.Value + " " +(val.Selected?"selected":"") + ">" + val.Text + "</option>");
        });
}

AdminPage.applyDot = function (wp, rows, placement) {
    if (rows === undefined) rows = 1;
    if (placement === undefined) placement = "right";
    var browser = AdminPage.getBrowserInfo().toLowerCase();
    $(wp).find(".wspace-normal").dotdotdot({
        keep: ".more_less",
        ellipsis: "",
        height: (rows) * 12,
        callback: function (isTruncated) {
            $(this).find(".more_less").css("display", (!isTruncated ? "none" : "inline"))
                .popover({ container: 'body', trigger: "hover", placement: placement });
        }
    });
}

/**
 * Funciones de Modals
 */
function ModalAdmin() {
}

/**
 * Muestra modal con titulo, mensaje y boton de opción.
 * @param {string} idModalShow: identificador del modal
 * @param {string} idTitle : identificador del titulo del modal
 * @param {string} title : mensaje a mostrar en el titulo
 * @param {string} idMessage : identificador del mensaje
 * @param {string} message: mensaje a mostrar en el modal
 * @param {string} idContactenos : identificador del botón contactenos
 * @param {bool} flagContactenos : valor que indica la visibilad del botón contacenos
 */
ModalAdmin.showVentanaMensajeWithTitleAndMessage = function (idModalShow,
    idTitle,
    title,
    idMessage,
    message,
    idContactenos,
    flagContactenos) {
    $("#" + idTitle).text(title);
    $("#" + idMessage).html(message);

    if (flagContactenos) {
        $("#" + idContactenos).removeClass("no-display");
    } else {
        $("#" + idContactenos).addClass("no-display");
    }
    $('#' + idModalShow).modal({
        show: true,
        backdrop: 'static',
        keyboard: false
    });
}

/**
 * Oculta modal
 * @param {string} idModal: identificador del modal
 */
ModalAdmin.hide = function (idModal) {
    $("#" + idModal).modal('hide');
}

ModalAdmin.hideAndShow = function (idModalHide, idModalShow) {
    $("#" + idModalHide).modal('hide');
    $('#' + idModalShow).modal({
        show: true,
        backdrop: 'static',
        keyboard: false
    });
}

ModalAdmin.registerShownEventListener = function (idModal) {
    $('#' + idModal).on('shown.bs.modal',
        function (e) {
            $('body').addClass("modal-open");
        });
}

ModalAdmin.registerHiddenEventListener = function (idModal) {
    $('#' + idModal).on('hidden.bs.modal',
        function (e) {
            $('body').removeClass("modal-open");
            $('body').css('padding-right', '');
        });
}

/**
 * Mostar modal 
 * @param {any} idModal: identificador del modal
 * @param {any} showOption : true: mostrar, false:no muestra
 */
ModalAdmin.registerShowByShowOption = function (idModal, showOption) {
    $('#' + idModal).modal({
        show: showOption,
        backdrop: 'static',
        keyboard: false
    });
}

ModalAdmin.showModalVideo = function (idModal, urlVideo) {
    $("#iframeVideo").attr('src', urlVideo);  

    $('#' + idModal).modal({
        show: true,
        backdrop: 'static',
        keyboard: false
    });
}


ModalAdmin.ListenerEnviarMensajeContactenos = function(url)
{
    $("#btnEnviarMensaje").on('click',
        function () {
            var txtMensaje = $("textarea#txtMensaje").val();
            if (txtMensaje == "")
                return;
            $.ajax({
                type: "POST",
                url: url,
                data: {
                    Mensaje: txtMensaje
                },
                beforeSend: function () {
                },
                success: function (response) {
                    $("textarea#txtMensaje").val("");
                    ModalAdmin.hide("ModalVentanaContactenos");
                    ModalAdmin.showVentanaMensajeWithTitleAndMessage("ModalVentanaMensaje",
                        "messageTitle",
                        "Solicitud de contacto o soporte",
                        "message",
                        "Su solicitud está siendo procesada y nos contactaremos a la brevedad con usted",
                        "lnkContactenos",
                        false);
                },
                error: function (data) {
                    console.log(data);
                }
            });
        });
}

/**
 *
 */
function Fecha() {
}

/**
 * 
 * @param {any} pDate : Fecha que será convertida en formato YYYYMM
 */
Fecha.getYearAndMonth = function (pDate) {

    var yearDate = pDate.getFullYear();
    var monthDate = pDate.getMonth() + 1;
    var valueDate = yearDate.toString() + (monthDate.toString().length == 1 ? "0" + monthDate : monthDate);

    return valueDate;
}
/**
 * Obtiene el año de una fecha
 * @param {string} pDate: fecha
 */
Fecha.getYear = function(pDate) {
    return pDate.getUTCFullYear();
}

Fecha.getYearAndMonthByUTC = function (pDate) {

    var yearDate = pDate.getUTCFullYear();
    var monthDate = pDate.getUTCMonth() + 1;
    var valueDate = yearDate.toString() + (monthDate.toString().length == 1 ? "0" + monthDate : monthDate);

    return valueDate;
}


function FiltrosTabMis() {
}

FiltrosTabMis.changeTipoOpe = function (urlPost,
    tipoOpe,
    idCbo)
{
    $.ajax({
        type: "POST",
        url: urlPost,
        data: {
            tipoOpe: tipoOpe
        },
        beforeSend: function () {
            LoadingAdminPage.showOrHideLoadingPage(true);
        },
        success: function (response) {
            
            AdminPage.loadDataComboBox(response.objMySearchForm.ListItemsMyFilters, idCbo);
            FiltrosTabMis.loadPeriods(response.objMySearchForm.FiltroPeriodo);
            LoadingAdminPage.showOrHideLoadingPage(false);
        },
        error: function (dataError) {
            console.info(dataError);
        }
    });
}

FiltrosTabMis.changePais2 = function (urlPost, pCodPais2, idCboMyFilters , pais2Text) {
    $.ajax({
        type: "POST",
        url: urlPost,
        data: {
            codPais2: pCodPais2,
            pais2Text: pais2Text
        },
        beforeSend: function () {
            LoadingAdminPage.showOrHideLoadingPage(true);
        },
        success: function (response) {

            AdminPage.loadDataComboBox(response.objMySearchForm.ListItemsPais, "cboPais");
            AdminPage.loadDataComboBox(response.objMySearchForm.ListItemsMyFilters, idCboMyFilters);
            FiltrosTabMis.loadPeriods(response.objMySearchForm.FiltroPeriodo);

            if (response.objMensaje != null) {
                $("#cboPais2").val(response.objMySearchForm.CodPais2Selected);
                $("#cboPais").val(response.objMySearchForm.CodPaisSelected);
                LoadingAdminPage.showOrHideLoadingPage(false);
                ModalAdmin.showVentanaMensajeWithTitleAndMessage("ModalVentanaMensaje",
                    "messageTitle",
                    response.objMensaje.titulo,
                    "message",
                    response.objMensaje.mensaje,
                    "lnkContactenos",
                    response.objMensaje.flagContactenos);
            } else {
                $("#cboPais").val(response.objMySearchForm.CodPaisSelected);
                LoadingAdminPage.showOrHideLoadingPage(false);
            }
        },
        error: function (dataError) {
            console.info(dataError);
        }
    });
}

FiltrosTabMis.changePais = function (urlPost, pCodPais,pTextCodPais, idCboMyFilters) {
    $.ajax({
        type: "POST",
        url: urlPost,
        data: {
            codPais: pCodPais,
            textCodPais: pTextCodPais
        },
        beforeSend: function () {
            LoadingAdminPage.showOrHideLoadingPage(true);
        },
        success: function (response) {
            AdminPage.loadDataComboBox(response.objMySearchForm.ListItemsMyFilters, idCboMyFilters);
            FiltrosTabMis.loadPeriods(response.objMySearchForm.FiltroPeriodo);

            if (response.objMensaje != null) {
                $("#cboPais2").val(response.objMySearchForm.CodPais2Selected);
                $("#cboPais").val(response.objMySearchForm.CodPaisSelected);

                LoadingAdminPage.showOrHideLoadingPage(false);
                ModalAdmin.showVentanaMensajeWithTitleAndMessage("ModalVentanaMensaje",
                    "messageTitle",
                    response.objMensaje.titulo,
                    "message",
                    response.objMensaje.mensaje,
                    "lnkContactenos",
                    response.objMensaje.flagContactenos);
            } else {
                LoadingAdminPage.showOrHideLoadingPage(false);
            }
        },
        error: function (dataError) {
            console.info(dataError);
        }
    });
}

FiltrosTabMis.getFechaIniAndFechaFin = function() {
    var fechas = [];
    var opcion = $("#cboOpcion").val();

    switch (opcion) {
        case "MESES":
            fechas.push(Fecha.getYearAndMonth($('#cboAnioMesIni').datepicker('getDate')));
            break;
        case "ULT12":
            fechas.push(Fecha.getYearAndMonth($('#cboAnioMesFin').datepicker('getDate')));
            break;
        case "YTD":
            fechas.push(Fecha.getYearAndMonth($('#cboAnioMesFin').datepicker('getDate')));
            break;
        case "AÑOS":
            fechas.push(Fecha.getYearAndMonth($('#cboAnioIni').datepicker('getDate')));
            break;
    }

    if (opcion != "AÑOS") {
        fechas.push(Fecha.getYearAndMonth($('#cboAnioMesFin').datepicker('getDate')));
    } else {
        fechas.push(Fecha.getYearAndMonth($('#cboAnioFin').datepicker('getDate')));
    }
    return fechas;
}

FiltrosTabMis.changeMyFilters = function (urlPost, valCbo, textCbo) {
    var fechas = FiltrosTabMis.getFechaIniAndFechaFin();

    $.ajax({
        type: "POST",
        url: urlPost,
        data: {
            idFilter: valCbo,
            textPartida: textCbo,
            opcion: $("#cboOpcion").val(),
            fechaIni: fechas[0],
            fechaFin: fechas[1]
        },
        beforeSend: function () {
            LoadingAdminPage.showOrHideLoadingPage(true);
        },
        success: function (response) {
           
            $("#descUnidad").text(response.textUnidad);

            if (response.flagChangeCheckedRadio) {
                if (response.checkedUSD) {
                    $('#rdbUSD').prop('checked', true);
                    $('#rdbUnid').prop('checked', false);
                } else {
                    $('#rdbUnid').prop('checked', true);
                    $('#rdbUSD').prop('checked', false);
                }
            }

            if (response.visibleUSD) {
                $("#lblRdbUSD").removeClass("no-display");
            } else {
                $("#lblRdbUSD").addClass("no-display");
            }

            if (response.visibleUnid) {
                $("#lblRrdbUnid").removeClass("no-display");
            } else {
                $("#lblRrdbUnid").addClass("no-display");
            }

            LoadingAdminPage.showOrHideLoadingPage(false);
        },
        error: function (dataError) {
            console.info(dataError);
        }
    });
}

FiltrosTabMis.loadPeriods = function (periodos) {
   
    var vFechaInfoIni = moment(periodos.FechaInfoIni);
    var vFechaInfoFin = moment(periodos.FechaInfoFin).add(1, "days");
    var vDefaultFechaInfoIni = moment(periodos.DefaultFechaInfoIni);
    var vDefaultFechaInfoFin = moment(periodos.DefaultFechaInfoFin);

    var vFechaAnioIni = moment(periodos.FechaAnioIni).add(1, "days");
    var vFechaAnioFin = moment(periodos.FechaAnioFin).add(1, "days");
    var vDefaultAnioIni = moment(periodos.DefaultFechaAnioIni).add(1, "days");
    var vDefaultAnioFin = moment(periodos.DefaultFechaAnioFin).add(1, "days");
    

    $('#cboAnioMesIni').datepicker('setStartDate', vFechaInfoIni.toDate());
    $('#cboAnioMesIni').datepicker('setEndDate', vFechaInfoFin.toDate());
    $("#cboAnioMesIni").datepicker("setDate", vDefaultFechaInfoIni.toDate());

    $('#cboAnioMesFin').datepicker('setStartDate', vDefaultFechaInfoIni.toDate());
    $('#cboAnioMesFin').datepicker('setEndDate', vFechaInfoFin.toDate());
    $("#cboAnioMesFin").datepicker("setDate", vDefaultFechaInfoFin.toDate());

    $('#cboAnioIni').datepicker('setStartDate', new Date(vFechaAnioIni.year(), vFechaAnioIni.month()) );
    $('#cboAnioIni').datepicker('setEndDate', new Date(vFechaAnioFin.year(), vFechaAnioFin.month()) );
    $("#cboAnioIni").datepicker("setDate", new Date(vDefaultAnioIni.year(), vDefaultAnioIni.month()) );

    $('#cboAnioFin').datepicker('setStartDate', new Date(vDefaultAnioIni.year(), vDefaultAnioIni.month()) );
    $('#cboAnioFin').datepicker('setEndDate', new Date(vDefaultAnioFin.year(), vDefaultAnioFin.month()) );
    $("#cboAnioFin").datepicker("setDate", new Date(vDefaultAnioFin.year(), vDefaultAnioFin.month()) );

    FiltrosTabMis.changeOptionPeriod($("#cboOpcion"));
}

FiltrosTabMis.changeOptionPeriod = function(vThis) {
    LoadingAdminPage.showOrHideLoadingPage(true);
    var currentValue = $(vThis).val();
    
    if (currentValue == "MESES") {
        var endAnioMesFin = $('#cboAnioMesFin').datepicker('getEndDate');

        var currentAnioMesFin = moment(endAnioMesFin.getTime());

        var fechaEndAnioMesFin = moment(endAnioMesFin.getTime()).add(1, "days");
        var auxFechaEndAnioMesFin = moment(endAnioMesFin.getTime());

        var fechaAnioMesIni = auxFechaEndAnioMesFin.subtract(2, 'months');
     

        $('#cboAnioMesFin').datepicker('setStartDate', fechaAnioMesIni.toDate());
        $('#cboAnioMesFin').datepicker('setEndDate', fechaEndAnioMesFin.toDate());
        $('#cboAnioMesFin').datepicker('setDate', currentAnioMesFin.toDate());

        $('#cboAnioMesIni').datepicker('setDate', fechaAnioMesIni.toDate());


    } else if (currentValue == "ULT12" || currentValue == "YTD") {
        
        var endDateAnioMesFin = $('#cboAnioMesFin').datepicker('getEndDate');
        
        var dateEndDate = moment(endDateAnioMesFin.getTime()).add(1, "days");
        var newEndDateAnioMesFin = moment(endDateAnioMesFin.getTime()).add(2, "days");

        var startDateAnioMesIni = $('#cboAnioMesIni').datepicker('getStartDate');

        var newStartDate = moment(startDateAnioMesIni.getTime()).add(1, "days");
        

        $('#cboAnioMesFin').datepicker('setStartDate', new Date(newStartDate.year(), newStartDate.month()));
        $('#cboAnioMesFin').datepicker('setEndDate', newEndDateAnioMesFin.toDate());
        $("#cboAnioMesFin").datepicker("setDate", new Date(dateEndDate.year(), dateEndDate.month()));
    }

    if (currentValue == "MESES") {
        $("#cboAnioMesIni").removeClass("no-display");
    } else {
        $("#cboAnioMesIni").addClass("no-display");
    }

    if (currentValue != "AÑOS") {
        $("#cboAnioMesFin").removeClass("no-display");
        $("#cboAnioIni").addClass("no-display");
        $("#cboAnioFin").addClass("no-display");
    } else {
        $("#cboAnioMesFin").addClass("no-display");
        $("#cboAnioIni").removeClass("no-display");
        $("#cboAnioFin").removeClass("no-display");
    }
    LoadingAdminPage.showOrHideLoadingPage(false);
}

FiltrosTabMis.changeMonthAnioMesIni = function (pDateDesde) {
    var currentDateTo = $('#cboAnioMesFin').datepicker('getDate');
    if (pDateDesde.getTime() > currentDateTo.getTime()) {
        $('#cboAnioMesFin').datepicker('setStartDate', pDateDesde);
        $("#cboAnioMesFin").datepicker("setDate", pDateDesde);
    } else {
        $('#cboAnioMesFin').datepicker('setStartDate', pDateDesde);
        $("#cboAnioMesFin").datepicker("setDate", currentDateTo);
    }
}

FiltrosTabMis.changeYearAnioIni = function (pDateDesde) {
    var currentDateTo = $('#cboAnioFin').datepicker('getDate');
    if (pDateDesde.getTime() > currentDateTo.getTime()) {
        $('#cboAnioFin').datepicker('setStartDate', pDateDesde);
        $("#cboAnioFin").datepicker("setDate", pDateDesde);
    } else {
        $('#cboAnioFin').datepicker('setStartDate', pDateDesde);
        $("#cboAnioFin").datepicker("setDate", currentDateTo);
    }
}


function Chart() {
}

Chart.getTypeColumn = function (idContainer,
    objChartData,
    urlRegistrosByChartSeriesPoint,
    urlVerRegistrosPaging,
    visiblePages,
    urlBuscarPorDesComercial,
    symbol
) {
    var symbol_2 = "";
    if (symbol === undefined) {
        symbol = "$";
    } else {
        symbol_2 = symbol;
        symbol = "";
    }
    Highcharts.chart(idContainer,
        {
            title: {
                text: objChartData.Title,
                widthAdjust: -200
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
                tickInterval: objChartData.TickInterval,
                //tickInterval: 1000000,
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
                                Chart.clickSeriesPoint(urlRegistrosByChartSeriesPoint,
                                    urlVerRegistrosPaging,
                                    visiblePages,
                                    urlBuscarPorDesComercial,
                                    this.category);

                                // JANAQ Clic Grafico 200720
                                if (typeof MixPanel.clickGrafico == 'function') {
                                    MixPanel.clickGrafico(idContainer);
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

Chart.getTypeLineWithDataLabels = function (idContainer,
    objChartData,
    urlRegistrosByChartSeriesPoint,
    urlVerRegistrosPaging,
    visiblePages,
    urlBuscarPorDesComercial) {
    Highcharts.chart(idContainer,
        {
            title: {
                text: objChartData.Title,
                widthAdjust: -200
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
            xAxis: {
               categories: objChartData.Categories
            },
            plotOptions: {
                line: {
                    dataLabels: {
                        enabled: true,
                        format: '$ {y}'
                    },
                    enableMouseTracking: true
                },
                series: {
                    cursor: 'pointer',
                    point: {
                        events: {
                            click: function () {
                                Chart.clickSeriesPoint(urlRegistrosByChartSeriesPoint,
                                    urlVerRegistrosPaging,
                                    visiblePages,
                                    urlBuscarPorDesComercial,
                                    this.category);

                                // JANAQ Clic Grafico 200720
                                if (typeof MixPanel.clickGrafico == 'function') {
                                    MixPanel.clickGrafico(idContainer);
                                }
                            }
                        }
                    }
                }
            },
            tooltip: {
                crosshairs: [true],
                formatter: function () {
                    yVal = this.y;
                    return "<b>" + this.x + '</b>: $' + yVal.toLocaleString('en');
                    //return "<b>" + this.x + '</b>: $' + Highcharts.numberFormat(this.point.y, 3, '.', ',');
                }
            },
            series: [
                {
                    //name: 'Installation',
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
            },
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

Chart.getTypeLineWithManySeries = function (idContainer,
    objChartData,
    urlRegistrosByCategoryAndSerie,
    urlVerRegistrosPaging,
    visiblePages,
    urlBuscarPorDesComercial,
    symbol
) {
    var symbol_2 = "";
  
    if (symbol === undefined) {
        symbol = "$";
    } else {
        symbol_2 = symbol;
        symbol = "";
    }
    Highcharts.chart(idContainer,
        {
            chart: {
                type: 'spline'
            },
            title: {
                text: objChartData.Title,
                widthAdjust: -200
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
                        output += '<br /><span style="color: ' + point.series.color + '">' + marker + '</span> ' + point.series.name + ': ' + symbol + ' ' + Highcharts.numberFormat(point.y, 0, '.', ',') + symbol_2;
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
                
                series: {
                    cursor: 'pointer',
                    point: {
                        events: {
                            click: function () {
                                Chart.VerRegistrosByCategoryAndSerie(urlRegistrosByCategoryAndSerie,
                                    urlVerRegistrosPaging,
                                    visiblePages,
                                    urlBuscarPorDesComercial,
                                    this.category,
                                    this.series.name);

                                // JANAQ Clic Grafico 200720
                                if (typeof MixPanel.clickGrafico == 'function') {
                                    MixPanel.clickGrafico(idContainer);
                                }
                            }
                        }
                    }
                }
            },
            series: objChartData.Series,
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
            },
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

Chart.getTypePie = function (idContainer,
    objChartData,
    urlVerRegistros,
    urlVerRegistrosPaging,
    visiblePages,
    urlBuscarPorDesComercial,
    idFiltro) {

    Highcharts.chart(idContainer,
        {
            chart: {
                plotBackgroundColor: null,
                plotBorderWidth: null,
                plotShadow: false,
                type: 'pie'
            },
            title: {
                text: objChartData.Title,
                widthAdjust: -200
            },
            tooltip: {
                headerFormat: '',
                pointFormat: '{point.name}: <b>{point.percentage:.1f}%</b>'
                //pointFormat: '{series.name}: <b>{point.percentage:.1f}%</b>'
            },
            plotOptions: {
                pie: {
                    allowPointSelect: true,
                    dataLabels: {
                        enabled: true,
                        format: '<b>{point.name}</b>: {point.percentage:.1f} %',
                        style: {
                            color: (Highcharts.theme && Highcharts.theme.contrastTextColor) || 'black'
                        },
                        connectorColor: 'silver'
                    },
                    cursor: "pointer",
                    point: {
                        events: {
                            click: function () {
                                if (this.id != "0") {
                                    Chart.VerRegistrosBySlice(urlVerRegistros,
                                        urlVerRegistrosPaging,
                                        visiblePages,
                                        urlBuscarPorDesComercial,
                                        idFiltro,
                                        this.id);

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
                    //name: 'Brands',
                    data: objChartData.PieDatas
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

Chart.clickSeriesPoint = function (urlVerRegistrosByPoint,
    urlVerRegistrosPaging,
    visiblePages,
    urlBuscarPorDesComercial,
    pCategory) {

    $.ajax({
        type: "POST",
        url: urlVerRegistrosByPoint,
        data: {
            opcion: $("#cboOpcion").val(),
            category: pCategory
        },
        beforeSend: function () {
            LoadingAdminPage.showOrHideLoadingPage(true);
        },
        success: function (response) {
            TablesTabsMis.LoadDataVerRegistros(response,
                urlVerRegistrosPaging,
                visiblePages,
                urlBuscarPorDesComercial,
                false);
        },
        error: function (dataError) {
            console.log(dataError);
            //LoadingAdminPage.showOrHideLoadingPage(false);
        }
    });
}

Chart.VerRegistrosByCategoryAndSerie = function (urlRegistrosCategoryAndSerie,
    urlVerRegistrosPaging,
    visiblePages,
    urlBuscarPorDesComercial,
    pCategory,
    pSerie) {

    $.ajax({
        type: "POST",
        url: urlRegistrosCategoryAndSerie,
        data: {
            opcion: $("#cboOpcion").val(),
            category: pCategory,
            serie: pSerie
        },
        beforeSend: function () {
            LoadingAdminPage.showOrHideLoadingPage(true);
        },
        success: function (response) {
            
            TablesTabsMis.LoadDataVerRegistros(response,
                urlVerRegistrosPaging,
                visiblePages,
                urlBuscarPorDesComercial,
                false);
        },
        error: function (dataError) {
            console.log(dataError);
            //LoadingAdminPage.showOrHideLoadingPage(false);
        }
    });

}

Chart.VerRegistrosBySlice = function (urlVerRegistros,
    urlVerRegistrosPaging,
    visiblePages,
    urlBuscarPorDesComercial,
    pIdFiltro,
    pIdSlice) {

    var fechas = FiltrosTabMis.getFechaIniAndFechaFin();
    $.ajax({
        type: "POST",
        url: urlVerRegistros,
        data: {
            idFiltro: pIdFiltro,
            idRegistro: pIdSlice,
            opcion: $("#cboOpcion").val(),
            fechaIni: fechas[0],
            fechaFin: fechas[1]
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


Chart.LoadCboThemes = function(idCbo) {
    $.each(dbThemes,
        function (index, val) {
            if (val.defaultSelected) {
                $("#" + idCbo).append('<option value="' + val.id + '" data-color="' + val.dataColor +'" selected="selected">' +val.name +'</option>');
            } else {
                $("#" + idCbo).append('<option value="' + val.id + '" data-color="' + val.dataColor + '">' + val.name + '</option>');
            }
        });
}

function altFind(arr, callback) {
    for (var i = 0; i < arr.length; i++) {
        var match = callback(arr[i]);
        if (match) {
            return arr[i];
            break;
        }
    }
}

Chart.ChangeThemeTypeColumn = function (idCboTheme,
    urlRegistrosByChartSeriesPoint,
    urlVerRegistrosPaging,
    visiblePages,
    urlBuscarPorDesComercial) {
    //var browser = AdminPage.getBrowserInfo().toLowerCase();
    //console.log("el browser es: " + browser);
    $("#" + idCboTheme).change(function () {
        var vFiltro = $(this).data("filtro");
        var idTheme = $(this).val();
        var valueTheme = altFind(dbThemes, function (e) {
            return e.id == idTheme;
        }).value;
        //var valueTheme = dbThemes.find(x => x.id == idTheme).value;

        Highcharts.setOptions(valueTheme);
        //var chartTitle = $("#" + "pieChart" + vFiltro).highcharts().options.title.text;
        var chartData = altFind(gArrayChartData, function (e) {
            return e.filtro == vFiltro;
        }).value;
        //var chartData = gArrayChartData.find(y => y.filtro == vFiltro).value;

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

        var symbol = "$";
        if (!$("#rdbUSD").is(":checked")) {
            symbol = "KG"
        } 
        Chart.getTypeColumn(vFiltro,
            chartData,
            urlRegistrosByChartSeriesPoint,
            urlVerRegistrosPaging,
            visiblePages,
            urlBuscarPorDesComercial,
            symbol
        );
    });
}


Chart.ChangeThemeTypeLineWithDataLabels = function (idCboTheme,
    urlRegistrosByChartSeriesPoint,
    urlVerRegistrosPaging,
    visiblePages,
    urlBuscarPorDesComercial) {
    //var browser = AdminPage.getBrowserInfo().toLowerCase();
    //console.log("el browser es: " + browser);
    $("#" + idCboTheme).change(function() {
        var vFiltro = $(this).data("filtro");
        var idTheme = $(this).val();

        var valueTheme = altFind(dbThemes, function (e) {
            return e.id == idTheme;
        }).value;
        //var valueTheme = dbThemes.find(x => x.id == idTheme).value;

        Highcharts.setOptions(valueTheme);
        //var chartTitle = $("#" + "pieChart" + vFiltro).highcharts().options.title.text;

        var chartData = altFind(gArrayChartData, function (e) {
            return e.filtro == vFiltro;
        }).value;
        //var chartData = gArrayChartData.find(y => y.filtro == vFiltro).value;

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
        Chart.getTypeLineWithDataLabels(vFiltro,
            chartData,
            urlRegistrosByChartSeriesPoint,
            urlVerRegistrosPaging,
            visiblePages,
            urlBuscarPorDesComercial
        );
    });
}


Chart.ChangeThemeTypeLineWithManySeries = function (idCboTheme,
    urlRegistrosByCategoryAndSerie,
    urlVerRegistrosPaging,
    visiblePages,
    urlBuscarPorDesComercial) {
    //var browser = AdminPage.getBrowserInfo().toLowerCase();
    //console.log("el browser es: " + browser);
    $("#" + idCboTheme).change(function () {
        var vFiltro = $(this).data("filtro");
        var idTheme = $(this).val();
        var valueTheme = altFind(dbThemes, function (e) {
            return e.id == idTheme;
        }).value;
        //var valueTheme = dbThemes.find(x => x.id == idTheme).value;

        Highcharts.setOptions(valueTheme);
        //var chartTitle = $("#" + "pieChart" + vFiltro).highcharts().options.title.text;

        var chartData = altFind(gArrayChartData, function (e) {
            return e.filtro == vFiltro;
        }).value;
        //var chartData = gArrayChartData.find(y => y.filtro == vFiltro).value;

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
        var symbol = "$";
        if (!$("#rdbUSD").is(":checked")) {
            symbol = "KG"
        } 

        Chart.getTypeLineWithManySeries(vFiltro,
            chartData,
            urlRegistrosByCategoryAndSerie,
            urlVerRegistrosPaging,
            visiblePages,
            urlBuscarPorDesComercial,
            symbol
        );
    });
}

Chart.ChangeThemeTypePie = function (idCboTheme,
    urlVerRegistros,
    urlVerRegistrosPaging,
    visiblePages,
    urlBuscarPorDesComercial,
    idFiltro) {
    //var browser = AdminPage.getBrowserInfo().toLowerCase();
    //console.log("el browser es: " + browser);
    $(document).on('change',
        '#' + idCboTheme,
        function () {
            var vFiltro = $(this).data("filtro");
            var idTheme = $(this).val();

            var valueTheme = altFind(dbThemes, function (e) {
                return e.id == idTheme;
            }).value;
            //var valueTheme = dbThemes.find(x => x.id == idTheme).value;

            Highcharts.setOptions(valueTheme);
            //var chartTitle = $("#" + "pieChart" + vFiltro).highcharts().options.title.text;

            var chartData = altFind(gArrayChartData, function (e) {
                return e.filtro == vFiltro;
            }).value;
            //var chartData = gArrayChartData.find(y => y.filtro == vFiltro).value;

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

            Chart.getTypePie(vFiltro,
                chartData,
                urlVerRegistros,
                urlVerRegistrosPaging,
                visiblePages,
                urlBuscarPorDesComercial,
                idFiltro);
        });

}


function TablesTabsMis() {
}

TablesTabsMis.RegisterPaging = function (urlPaging,
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
                    //console.log(idPaging);
                    var vTxtDesComercial = $("#txtDesComercial").val();
                    if (vTxtDesComercial != "" && idPaging =="VerRegistros") {
                        $.each($("#tbody" + idPaging +" .wspace-normal"),
                            function () {

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


                    var wp = $("#table" + idPaging);
                    setTimeout(AdminPage.applyDot(wp, 2, "right"), 500);
                    if (idPaging == "Partida" || idPaging == "ImporterOrExporter" || idPaging == "OriginOrDestinationCountry") {
                        //console.log(response.difCantPorPagina)
                        if (response.difCantPorPagina < 4)
                            Size.Increment();
                        else {
                            $("#table" + idPaging + " table").removeAttr('style');
                        }
                    }
                    LoadingAdminPage.showOrHideLoadingPage(false);
                },
                error: function (data) {
                    console.log(data);
                }
            });
        }
    });

}

TablesTabsMis.VerRegistro = function (urlVerRegistro,
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
        });
}

TablesTabsMis.LoadDataVerRegistros = function (objResponse,
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

        $("#tableVerRegistros").html("");
        $("#tableVerRegistros").html(objResponse.tablaVerRegistros);
        

        var wpVR = $("#tableVerRegistros");
        setTimeout(AdminPage.applyDot(wpVR, 2, "left"), 500);

        if (objResponse.totalPages > 1) {
            TablesTabsMis.RegisterPaging(urlVerRegistrosPaging,
                "VerRegistros",
                objResponse.totalPages,
                visiblePages);
        }

        TablesTabsMis.BuscarPorDesComercial(urlBuscarPorDesComercial,
            urlVerRegistrosPaging,
            visiblePages,
            enabledFiltros);

        LoadingAdminPage.showOrHideLoadingPage(false);
        ModalAdmin.registerShowByShowOption("ModalVerRegistro", true);
    }
}


TablesTabsMis.BuscarPorDesComercial = function (urlBuscarPorDesComercial,
    urlVerRegistrosPaging,
    visiblePages,
    pEnabledFiltros) {

    $("#lnkBuscarDesComercial, #lnkRestablecerDesComercial").click(function () {

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

                    var vTxtDesComercial = $("#txtDesComercial").val();
                    if (vTxtDesComercial != "") {
                        $.each($("#tbodyVerRegistros .wspace-normal"),
                            function () {

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


                    var wpVR = $("#tableVerRegistros");
                    setTimeout(AdminPage.applyDot(wpVR, 2, "left"), 500);

                    if (response.totalPages > 1) {
                        $("#pagingVerRegistros").twbsPagination('destroy');
                        $("#divPagingVerRegistros").removeClass("no-display");

                        TablesTabsMis.RegisterPaging(urlVerRegistrosPaging,
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

//Detención de eventos comunes
$(document).on("click",
    "#btnOKModalVentanaMensaje",
    function() {
        ModalAdmin.hide("ModalVentanaMensaje");
    });
$(document).on("click",
    "#btnOKModalVentanaMensajeFree",
    function () {
        ModalAdmin.hide("ModalVentanaMensajeFree");
    });

$(document).on("click",
    "#lnkActionContactUs",
    function () {
        ModalAdmin.hideAndShow("ModalVentanaMensaje", "ModalVentanaContactenos");
    });

function AdminTable() {
}

AdminTable.SelectAllChecksCurrentPage = function (idTBody, thisInput) {
    if ($(thisInput).prop("checked")) {
        $("#" + idTBody + " tr>td input[type=checkbox]").each(function () {
            $(this).prop('checked', true);
        });
    } else {
        $("#" + idTBody + " tr>td input[type=checkbox]").each(function () {
            $(this).prop('checked', false);
        });
    }
}

function getDateFormatted(inputdate) {
    var k = inputdate;
    var dt = new Date(k);
    var yr = dt.getYear() + 1900;
    var mn = dt.getMonth() + 1;
    return yr + "-" + mn + "-" + dt.getDate();

}
/**
 * Funcion que permite obtener el contenido de una cookie
 * @param pname: nombre de la cookie
 */
function getCookie(pname) {
    var name = pname + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') {
            c = c.substring(1);
        }
        if (c.indexOf(name) == 0) {
            return c.substring(name.length, c.length);
        }
    }
    return "";
}
