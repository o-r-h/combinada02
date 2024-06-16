/**Funcion reutilizable: setTimeOut(recargar cada 5 segundo)
 * */

Refresh = function (urlPost, culture, cont) {
    $.ajax({
        url: urlPost,
        data: {
            culture: culture,
            cont: cont
        },
        type: "POST",
        success: function (response) {
            console.log(cont);
            $("#LastSearches").html(response.viewLastSearches);
        },
        error: function (dataError) {
            console.log(dataError);
        }
    });
}
/**
 * Funciones de Loading
 */
function LoadingAdminPage() {
}
LoadingAdminPage.showOrHideLoadingPageProduct = function (pEstado) {
    if (pEstado) {
        $("#loadingPageProduct").addClass("is-active-loadingP");
    } else {
        $("#loadingPageProduct").removeClass("is-active-loadingP");
    }
}

LoadingAdminPage.showOrHideLoadingPage = function (pEstado) {
    if (pEstado) {
        $("#loadingPageAdmin").addClass("is-active-loading");
    } else {
        $("#loadingPageAdmin").removeClass("is-active-loading");
    }
}

LoadingAdminPage.showOrHideLoadingPageCompany = function (pEstado) {
    if (pEstado) {
        $("#loadingPageCompany").addClass("is-active-loadingP");
    } else {
        $("#loadingPageCompany").removeClass("is-active-loadingP");
    }
}

function AdminPage() {
}

AdminPage.loadDataComboBox = function (optionList, idComboBox) {
    $("select#" + idComboBox).html("");
    $.each(optionList,
        function (index, val) {
            $("select#" + idComboBox).append("<option value=" + val.Value + ">" + val.Text + "</option>");
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
Fecha.getYear = function (pDate) {
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

FiltrosTabMis.changeTipoOpe = function (urlPost, tipoOpe, idCbo) {
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

FiltrosTabMis.changePais2 = function (urlPost, pCodPais2, idCboMyFilters) {
    $.ajax({
        type: "POST",
        url: urlPost,
        data: {
            codPais2: pCodPais2
        },
        beforeSend: function () {
            LoadingAdminPage.showOrHideLoadingPage(true);
        },
        success: function (response) {

            AdminPage.loadDataComboBox(response.objMySearchForm.ListItemsPais, "cboPais");

            if (response.objMySearchForm.indexCodPais == "0") {
                $("#cboPais").prop('selectedIndex', 0);
            } else {
                $("#cboPais").val(response.objMySearchForm.CodPaisSelected);
            }

            AdminPage.loadDataComboBox(response.objMySearchForm.ListItemsMyFilters, idCboMyFilters);
            FiltrosTabMis.loadPeriods(response.objMySearchForm.FiltroPeriodo);


            if (response.objMySearchForm.objMensaje != null) {
                ModalAdmin.showVentanaMensajeWithTitleAndMessage("ModalVentanaMensaje",
                    "messageTitle",
                    response.objMensaje.titulo,
                    "message",
                    response.objMensaje.mensaje,
                    "lnkContactenos",
                    response.objMensaje.flagContactenos);
            }

            LoadingAdminPage.showOrHideLoadingPage(false);
        },
        error: function (dataError) {
            console.info(dataError);
        }
    });
}

FiltrosTabMis.changePais = function (urlPost, pCodPais, pTextCodPais, idCboMyFilters) {
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
            LoadingAdminPage.showOrHideLoadingPage(false);
        },
        error: function (dataError) {
            console.info(dataError);
        }
    });
}

FiltrosTabMis.getFechaIniAndFechaFin = function () {
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
            console.log(response);

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
    var re = /-?\d+/;

    var vFechaInfoIni = re.exec(periodos.FechaInfoIni);
    vFechaInfoIni = new Date(parseInt(vFechaInfoIni[0]));
    var vFechaInfoFin = re.exec(periodos.FechaInfoFin);
    vFechaInfoFin = new Date(parseInt(vFechaInfoFin[0]));

    var vDefaultFechaInfoIni = re.exec(periodos.DefaultFechaInfoIni);
    vDefaultFechaInfoIni = new Date(parseInt(vDefaultFechaInfoIni[0]));
    var vDefaultFechaInfoFin = re.exec(periodos.DefaultFechaInfoFin);
    vDefaultFechaInfoFin = new Date(parseInt(vDefaultFechaInfoFin[0]));

    var vFechaAnioIni = re.exec(periodos.FechaAnioIni);
    vFechaAnioIni = new Date(parseInt(vFechaAnioIni[0]));
    var vFechaAnioFin = re.exec(periodos.FechaAnioFin);
    vFechaAnioFin = new Date(parseInt(vFechaAnioFin[0]));

    var vDefaultAnioIni = re.exec(periodos.DefaultFechaAnioIni);
    vDefaultAnioIni = new Date(parseInt(vDefaultAnioIni[0]));
    var vDefaultAnioFin = re.exec(periodos.DefaultFechaAnioFin);
    vDefaultAnioFin = new Date(parseInt(vDefaultAnioFin[0]));

    $('#cboAnioMesIni').datepicker('setStartDate', vFechaInfoIni);
    $('#cboAnioMesIni').datepicker('setEndDate', vFechaInfoFin);
    $("#cboAnioMesIni").datepicker("setDate", vDefaultFechaInfoIni);

    $('#cboAnioMesFin').datepicker('setStartDate', vDefaultFechaInfoIni);
    $('#cboAnioMesIni').datepicker('setEndDate', vFechaInfoFin);
    $("#cboAnioMesFin").datepicker("setDate", vDefaultFechaInfoFin);

    $('#cboAnioIni').datepicker('setStartDate', vFechaAnioIni);
    $('#cboAnioIni').datepicker('setEndDate', vFechaAnioFin);
    $("#cboAnioIni").datepicker("setDate", vDefaultAnioIni);

    $('#cboAnioFin').datepicker('setStartDate', vDefaultAnioIni);
    $('#cboAnioFin').datepicker('setEndDate', vDefaultAnioFin);
    $("#cboAnioFin").datepicker("setDate", vDefaultAnioFin);

}

FiltrosTabMis.changeOptionPeriod = function (vThis) {
    LoadingAdminPage.showOrHideLoadingPage(true);
    var currentValue = $(vThis).val();
    if (currentValue == "MESES") {
        var endAnioMesFin = $('#cboAnioMesFin').datepicker('getEndDate');

        var dAnioMesFin = endAnioMesFin.getUTCDate();
        var mAnioMesFin = endAnioMesFin.getUTCMonth() + 1;
        var yAnioMesFin = endAnioMesFin.getUTCFullYear();
        var dateEndAnioMesFin = new Date(yAnioMesFin + "/" + mAnioMesFin + "/" + dAnioMesFin);
        var auxDateEndAnioMesFin = new Date(yAnioMesFin + "/" + mAnioMesFin + "/" + dAnioMesFin);;

        dateEndAnioMesFin.setMonth(dateEndAnioMesFin.getMonth() - 2);

        $('#cboAnioMesFin').datepicker('setStartDate', dateEndAnioMesFin);
        $('#cboAnioMesFin').datepicker('setDate', auxDateEndAnioMesFin);
        $('#cboAnioMesIni').datepicker('setDate', dateEndAnioMesFin);

    } else if (currentValue == "ULT12" || currentValue == "YTD") {
        //var currentDateAnioMesFin = $('#cboAnioMesFin').datepicker('getDate');
        var endDateAnioMesFin = $('#cboAnioMesFin').datepicker('getEndDate');
        var dEndDate = endDateAnioMesFin.getUTCDate();
        var mEndDate = endDateAnioMesFin.getUTCMonth() + 1;
        var yEndDate = endDateAnioMesFin.getUTCFullYear();
        var dateEndDate = new Date(yEndDate + "/" + mEndDate + "/" + dEndDate);

        var startDateAnioMesIni = $('#cboAnioMesIni').datepicker('getStartDate');
        var d = startDateAnioMesIni.getUTCDate();
        var m = startDateAnioMesIni.getUTCMonth() + 1;
        var y = startDateAnioMesIni.getUTCFullYear();
        var newStartDate = new Date(y + "/" + m + "/" + d);

        $('#cboAnioMesFin').datepicker('setStartDate', newStartDate);
        $("#cboAnioMesFin").datepicker("setDate", dateEndDate);
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

Chart.getTypeColumn = function (idContainer, objChartData) {
    Highcharts.chart(idContainer,
        {
            title: {
                text: objChartData.Title
            },
            legend: {
                enabled: false
            },
            colors: ['#4F81BD'],
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
                        return '$' + this.value.toLocaleString('en');
                    }
                }
            },
            tooltip: {
                //headerFormat: '',
                //pointFormat: '<b>{series.name}:</b> $ {point.y:,.0f}'
                formatter: function () {
                    var yVal = this.y;
                    return "<b>" + this.x + '</b>: $' + Highcharts.numberFormat(this.point.y, 0, '.', ','); //  yVal.toLocaleString('en');
                }
            },
            plotOptions: {
                column: {
                    pointPadding: 0,
                    borderWidth: 0
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
                        menuItems: ['downloadPNG', 'separator', 'printChart'],
                        symbol: 'download',
                        symbolFill: '#54b6e8',
                        _titleKey: "exportMenuButtonTitle"
                    }
                }
            }
        });
}

Chart.getTypeLineWithDataLabels = function (idContainer, objChartData) {
    Highcharts.chart(idContainer,
        {
            title: {
                text: objChartData.Title
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

Chart.getTypeLineWithManySeries = function (idContainer, objChartData) {
    Highcharts.chart(idContainer,
        {
            chart: {
                type: 'spline'
            },
            title: {
                text: objChartData.Title
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
            series: objChartData.Series,
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

function getDateFormatted(inputdate) {
    var k = inputdate;
    var dt = new Date(k);
    var yr = dt.getYear() + 1900;
    var mn = dt.getMonth() + 1;
    return yr + "-" + mn + "-" + dt.getDate();

}
