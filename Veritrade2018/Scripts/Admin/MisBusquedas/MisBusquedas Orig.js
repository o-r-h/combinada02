/**
 * Funciones de Modals
 */

function ModalAdmin() {
}

ModalAdmin.registerShowByShowOption = function (idModal, showOption) {
    $('#' + idModal).modal({
        show: showOption,
        backdrop: 'static',
        keyboard: false
    });
}

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
 * Modal país
 */
function TableInfoCountry() {
}

TableInfoCountry.prototype.showModalInfoMexicoDetalleFull = function () {
    $('#ModalInfoMexicoDetalleFull').modal({
        show: true,
        backdrop: 'static',
        keyboard: false
    });
}

TableInfoCountry.prototype.showModalInfoMexicoDetalleMaritimo = function () {
    $('#ModalInfoMexicoDetalleMaritimo').modal({
        show: true,
        backdrop: 'static',
        keyboard: false
    });
}

/**
 * Funciones de Filtros
 */

function Filtros() {
}

Filtros.cleanFieldsByFieldChange = function () {
    $(document).on("click",
        "#txtDesComercialB, #txtNandinaB, #txtImportadorB, #txtProveedorB, #txtExportadorB, #txtImportadorExpB ",
        function () {
            var inputFocus = $(this).attr("name");

            if (inputFocus !== "txtDesComercialB" && $("#txtDesComercialB").length) {
                $("#txtDesComercialB").val("");
            }

            if (inputFocus !== "txtNandinaB" && $("#txtNandinaB").length) {
                $("#txtNandinaB").val("");
            }

            if (inputFocus !== "txtImportadorB" && $("#txtImportadorB").length) {
                $("#txtImportadorB").val("");
            }

            if (inputFocus !== "txtProveedorB" && $("#txtProveedorB").length) {
                $("#txtProveedorB").val("");
            }

            if (inputFocus !== "txtExportadorB" && $("#txtExportadorB").length) {
                $("#txtExportadorB").val("");
            }

            if (inputFocus !== "txtImportadorExpB" && $("#txtImportadorExpB").length) {
                $("#txtImportadorExpB").val("");
            }
        });

    $(document).on("blur",
        "#txtNandinaB, #txtImportadorB, #txtProveedorB, #txtExportadorB, #txtImportadorExpB",
        function () {
            $(this).val("");
        });
}

Filtros.adjustSizeHeightFilters = function () {
    $('select#lstFiltros').each(function () {
        $(this).attr('size', $(this).find('option').length);
    });
}

Filtros.addFilterToFiltersList = function (listaOptions) {
    //MensajesConsole();
    //console.log(listaOptions);
    var version = navigator.appVersion.toLowerCase();
   
    var selected = "";

    if (version.indexOf("android") > -1)
        selected = "selected";
    if ($("#lstFiltros option").length <= 0) {
        $("#filtros").removeClass('no-display');
        $.each(listaOptions,
            function (index, val) {
                $("select[name='lstFiltros']")
                    .append("<option value='" + val.value + "' " +  selected + " >" + val.text + "</option>");
            });

        $("#btnRestablecer").removeClass('no-display');

    } else {
        $.each(listaOptions,
            function (index, val) {
                $("select[name='lstFiltros']")
                    .append("<option value='" + val.value + "' " + selected + " >" + val.text + "</option>");
            });
    }
    Filtros.adjustSizeHeightFilters();
}

Filtros.disableAndHideFiltersFieldsByMyGroups = function (pEstado) {
    if (pEstado) {
        $("#cboPaisB").attr("disabled", "disabled");
        $("#txtNandinaB").attr("disabled", "disabled");
        $("#txtImportadorB").attr("disabled", "disabled");
        $("#txtExportadorB").attr("disabled", "disabled");
        $("#txtProveedorB").attr("disabled", "disabled");
        $("#txtImportadorExpB").attr("disabled", "disabled");

        $("#btnMisPartidas").addClass("no-display");
        $("#bntMisImportadores").addClass("no-display");
        $("#btnMisProveedores").addClass("no-display");
        $("#btnMisExportadores").addClass("no-display");
        $("#btnImportadoresExp").addClass("no-display");
    }
}

Filtros.loadDataComboBox = function (optionList, idComboBox) {
    $("select#" + idComboBox).html("");
    $.each(optionList,
        function (index, val) {
            $("select#" + idComboBox).append("<option value=" + val.Value + ">" + val.Text + "</option>");
        });
}

Filtros.setVisibleMyFavourites = function (flagVisible) {
    if (flagVisible) {
        $("#btnMisPartidas").removeClass("no-display");
        $("#bntMisImportadores").removeClass("no-display");
        $("#btnMisProveedores").removeClass("no-display");
        $("#btnMisExportadores").removeClass("no-display");
        $("#btnImportadoresExp").removeClass("no-display");
    } else {
        $("#btnMisPartidas").addClass("no-display");
        $("#bntMisImportadores").addClass("no-display");
        $("#btnMisProveedores").addClass("no-display");
        $("#btnMisExportadores").addClass("no-display");
        $("#btnImportadoresExp").addClass("no-display");
    }
}

Filtros.onChangeCboDesde = function(pDateDesde) {
    var currentDateTo = $('#cboHasta').datepicker('getDate');
    if (pDateDesde.getTime() > currentDateTo.getTime()) {
        $('#cboHasta').datepicker('setStartDate', pDateDesde);
        $("#cboHasta").datepicker("setDate", pDateDesde);
    } else {
        $('#cboHasta').datepicker('setStartDate', pDateDesde);
        $("#cboHasta").datepicker("setDate", currentDateTo);
    }
}

Filtros.resetFilters = function (flagMyFavourites) {
    $("#txtNandinaB").val('');
    $("#txtImportadorB").val('');
    $("#txtProveedorB").val('');
    $("#txtExportadorB").val('');
    $("#txtImportadorExpB").val('');

    $("#btnAgregarDesComercial").removeClass("no-display");

    $("#cboPaisB").removeAttr("disabled");

    $("#txtNandinaB").removeAttr("disabled");
    $("#txtImportadorB").removeAttr("disabled");
    $("#txtExportadorB").removeAttr("disabled");
    $("#txtProveedorB").removeAttr("disabled");
    $("#txtImportadorExpB").removeAttr("disabled");

    Filtros.setVisibleMyFavourites(flagMyFavourites);

    $("select#lstFiltros").html("");
    $("#filtros").addClass('no-display');
    $("#btnRestablecer").addClass('no-display');

    $("#content_tabs").html("");
    $("#divRegisterFound").addClass('no-display');

}

Filtros.cargarPeriodos = function (dataPeriodos) {
    //$("input[name='cboDesde']").removeAttr("disabled");
    //$("input[name='cboHasta']").removeAttr("disabled");
    
    //var re = /-?\d+/; 
    //var vFechaInfoIni = re.exec(dataPeriodos.FechaInfoIni); 
    //vFechaInfoIni = new Date(parseInt(vFechaInfoIni[0]));

    //var vFechaInfoFin = re.exec(dataPeriodos.FechaInfoFin); 
    //vFechaInfoFin = new Date(parseInt(vFechaInfoFin[0]));

    //$('#cboDesde').datepicker('setStartDate', vFechaInfoIni);
    //$('#cboDesde').datepicker('setEndDate', vFechaInfoFin);
    //$("#cboDesde").datepicker("setDate", vFechaInfoFin);


    //$('#cboHasta').datepicker('setStartDate', vFechaInfoFin);
    //$('#cboHasta').datepicker('setEndDate', vFechaInfoFin);
    //$('#cboHasta').datepicker('setDate', vFechaInfoFin);


    //if (!dataPeriodos.EnabledAnioMesIni) {
    //    $("input[name='cboDesde']").attr("disabled", "disabled");
    //}

    //if (!dataPeriodos.EnabledAnioMesFin) {
    //    $("input[name='cboHasta']").attr("disabled", "disabled");
    //}

    $("input[name='cboDesde']").removeAttr("disabled");
    $("input[name='cboHasta']").removeAttr("disabled");

    var  vFechaInfoIni = moment(dataPeriodos.FechaInfoIni).add(1, "days");
    var vFechaInfoFin = moment(dataPeriodos.FechaInfoFin).add(1, "days");

    $('#cboDesde').datepicker('setStartDate', new Date(vFechaInfoIni.year(), vFechaInfoIni.month()));
    $('#cboDesde').datepicker('setEndDate', new Date(vFechaInfoFin.year(), vFechaInfoFin.month()));
    $("#cboDesde").datepicker("setDate", new Date(vFechaInfoFin.year(), vFechaInfoFin.month()));

    $('#cboHasta').datepicker('setStartDate', new Date(vFechaInfoFin.year(), vFechaInfoFin.month()));
    $('#cboHasta').datepicker('setEndDate', new Date(vFechaInfoFin.year(), vFechaInfoFin.month()));
    $('#cboHasta').datepicker('setDate', new Date(vFechaInfoFin.year(), vFechaInfoFin.month()));
    

    if (!dataPeriodos.EnabledAnioMesIni) {
        $("input[name='cboDesde']").attr("disabled", "disabled");
    }

    if (!dataPeriodos.EnabledAnioMesFin) {
        $("input[name='cboHasta']").attr("disabled", "disabled");
    }
}

Filtros.restablecer = function (urlPost, pIdioma, codPais) {
    $.ajax({
        type: "POST",
        url: urlPost,
        data: {
            cboPaisValue: $("#cboPais").find("option:selected").val(),
            cboPais2Value: $("#cboPais2").find("option:selected").val(),
            tipoOpe: $('input:radio[name=rdbTipoOpe]:checked').val(),
            idioma: pIdioma
        },
        beforeSend: function () {
            LoadingAdminPage.showOrHideLoadingPage(true);
        },
        success: function (response) {
            Filtros.loadDataComboBox(response.listOriginOrDestinationCountry, "cboPaisB");
            Filtros.cargarPeriodos(response.objPeriodos);
            Filtros.resetFilters(response.flagVisibleMisFiltros);

            LoadingAdminPage.showOrHideLoadingPage(false);

        },
        error: function (dataError) {
            console.info(dataError);
        }
    });
}

Filtros.setVisibleFilltrosDeAgregar = function (pFlag) {
    if (pFlag) {
        $("#btnAgregarDesComercial").removeClass("no-display");
        $("#btnMisPartidas").removeClass("no-display");
        $("#bntMisImportadores").removeClass("no-display");
        $("#btnMisProveedores").removeClass("no-display");
        $("#btnMisExportadores").removeClass("no-display");
        $("#btnImportadoresExp").removeClass("no-display");
    } else {
        $("#btnAgregarDesComercial").addClass("no-display");
        $("#btnMisPartidas").addClass("no-display");
        $("#bntMisImportadores").addClass("no-display");
        $("#btnMisProveedores").addClass("no-display");
        $("#btnMisExportadores").addClass("no-display");
        $("#btnImportadoresExp").addClass("no-display");
    }
}

Filtros.setFiltrosBuscar = function (pFlag) {
    if (!pFlag) {
        $("#btnMisPartidas").removeClass("no-display");
        $("#bntMisImportadores").removeClass("no-display");
        $("#btnMisProveedores").removeClass("no-display");
        $("#btnMisExportadores").removeClass("no-display");
        $("#btnImportadoresExp").removeClass("no-display");
    } else {
        $("#btnMisPartidas").addClass("no-display");
        $("#bntMisImportadores").addClass("no-display");
        $("#btnMisProveedores").addClass("no-display");
        $("#btnMisExportadores").addClass("no-display");
        $("#btnImportadoresExp").addClass("no-display");
    }
}

Filtros.changeTipoOpe = function (urlPost) {
    var vCodPais = $("#cboPais").find("option:selected").val();
    $.ajax({
        type: "POST",
        url: urlPost,
        data: {
            tipoOpe: $('input:radio[name=rdbTipoOpe]:checked').val(),
            codPais: vCodPais,
            codPais2: $("#cboPais2").find("option:selected").val()
        },
        beforeSend: function () {
            LoadingAdminPage.showOrHideLoadingPage(true);
        },
        success: function (response) {

            if (response.visibleInfoBO) {
                $("#messageBo").removeClass("no-display");
            } else {
                $("#messageBo").addClass("no-display");
            }

            $("#camposFiltros").html("");
            $("#camposFiltros").html(response.vistaFiltros);
            if (response.listaPaises.length > 0) {
                Filtros.loadDataComboBox(response.listaPaises, "cboPais");
                $("#cboPais").val(vCodPais);
            }
            Filtros.resetFilters(response.flagVisibleMisFiltros);
            Filtros.cargarPeriodos(response.objPeriodos);
            $("#RangoFechas").text(response.rangoInfoEnLinea);
            initAutocompleteFiltros();

            // JANAQ 060620
            if (typeof initAutoCompleteFiltrosMixPanel == 'function') {
                initAutoCompleteFiltrosMixPanel(); 
            }
            
            //RegisterEventsListenerDeMisFavorito();
            LoadingAdminPage.showOrHideLoadingPage(false);
        },
        error: function (dataError) {
            console.info(dataError);
        }
    });
}

Filtros.changeCboPais = function (urlPost, urlBusquedasUs, pIdioma) {
    var codPais = $("#cboPais").find("option:selected").val();
    $.ajax({
        type: "POST",
        url: urlPost,
        data: {
            codPais: codPais,
            codPaisText: $("#cboPais").find("option:selected").text(),
            codPais2: $("#cboPais2").find("option:selected").val(),
            tipoOpe: $('input:radio[name=rdbTipoOpe]:checked').val(),
            idioma: pIdioma
        },
        beforeSend: function () {
            LoadingAdminPage.showOrHideLoadingPage(true);
        },
        success: function (response) {

            if (response.redirectUS) {
                window.location.href = urlBusquedasUs;
            } else {

                if (response.visibleInfoCL) {
                    $("#messageCl").removeClass("no-display");
                } else {
                    $("#messageCl").addClass("no-display");
                }

                if (response.visibleInfoBO) {
                    $("#messageBo").removeClass("no-display");
                } else {
                    $("#messageBo").addClass("no-display");
                }

                $("#camposFiltros").html("");
                $("#camposFiltros").html(response.vistaFiltros);

                //Filtros.loadDataComboBox(response.paisesOrigenOdestino, "cboPaisB");
                Filtros.cargarPeriodos(response.objPeriodos);
                //console
                //if (response.tipoUsuarioEsFreeTrial) {
                //    $("#rangoInfoEnLinea").addClass("no-display");
                //} else {
                //    $("#rangoInfoEnLinea").removeClass("no-display");
                //    $("#RangoFechas").text(response.rangoInfoEnLinea);
                //}

                $("#rangoInfoEnLinea").removeClass("no-display");
                $("#RangoFechas").text(response.rangoInfoEnLinea);
                if (response.rangoInfoEnLineaFree != "") {
                    $("#RangoFechasFree").text(response.rangoInfoEnLineaFree);
                }
                Filtros.resetFilters(response.flagVisibleMisFiltros);

                if (response.objMensaje != null) {
                    ModalAdmin.showVentanaMensajeWithTitleAndMessage("ModalVentanaMensaje",
                        "messageTitle",
                        response.objMensaje.titulo,
                        "message",
                        response.objMensaje.mensaje,
                        "lnkContactenos",
                        response.objMensaje.flagContactenos);
                }

                if ($("#cboPais option[value='" + response.codPaisT + "']").length)
                    $("#cboPais").val();

                if (response.modificarCombo) {
                    $("#cboPais").val(response.codPaisT);
                }

                //RegisterEventsListenerDeMisFavorito();
                initAutocompleteFiltros();

                // JANAQ 060620
                if (typeof initAutoCompleteFiltrosMixPanel == 'function') {
                    initAutoCompleteFiltrosMixPanel();
                }

                LoadingAdminPage.showOrHideLoadingPage(false);
            }

            if (codPais == "MXD"){
                TableInfoCountry.prototype.showModalInfoMexicoDetalleFull();
            }
            //if (codPais == "MXM") {
            //    TableInfoCountry.prototype.showModalInfoMexicoDetalleMaritimo();
            //}
        },
        error: function (dataError) {
            console.info(dataError);
        }
    });
}

Filtros.changeCboPais2 = function (urlPost, urlBusquedasUs, pIdioma) {

    $.ajax({
        type: "POST",
        url: urlPost,
        data: {
            codPais2: $("#cboPais2").find("option:selected").val(),
            textoPais2: $("#cboPais2").find("option:selected").text(),
            textoPais: $("#cboPais").find("option:selected").text(),
            tipoOpe: $('input:radio[name=rdbTipoOpe]:checked').val(),
            idioma: pIdioma
        },
        beforeSend: function () {
            //console.log($("#cboPais2").find("option:selected").text())
            LoadingAdminPage.showOrHideLoadingPage(true);
        },
        success: function (response) {

            if (response.redirectUS) {
                window.location.href = urlBusquedasUs;
            } else {

                if (response.visibleInfoCL) {
                    $("#messageCl").removeClass("no-display");
                } else {
                    $("#messageCl").addClass("no-display");
                }

                if (response.visibleInfoBO) {
                    $("#messageBo").removeClass("no-display");
                } else {
                    $("#messageBo").addClass("no-display");
                }

                Filtros.loadDataComboBox(response.listaPaises, "cboPais");
                if (response.codPaisSeleccionar == "0") {
                    $("#cboPais").prop('selectedIndex', 0);
                } else {
                    $("#cboPais2").val(response.codPais2Seleccionar);
                    $("#cboPais").val(response.codPaisSeleccionar);
                }


                $("#camposFiltros").html("");
                $("#camposFiltros").html(response.vistaFiltros);

                Filtros.cargarPeriodos(response.objPeriodos);

                /*if (response.tipoUsuarioEsFreeTrial) {
                    $("#rangoInfoEnLinea").addClass("no-display");
                } else {
                    $("#rangoInfoEnLinea").removeClass("no-display");
                    $("#RangoFechas").text(response.rangoInfoEnLinea);
                }*/
                $("#rangoInfoEnLinea").removeClass("no-display");
                $("#RangoFechas").text(response.rangoInfoEnLinea);

                if (response.rangoInfoEnLineaFree != "") {
                    $("#RangoFechasFree").text(response.rangoInfoEnLineaFree);
                }

                Filtros.resetFilters(response.flagVisibleMisFiltros);

                if (response.objMensaje != null) {
                    ModalAdmin.showVentanaMensajeWithTitleAndMessage("ModalVentanaMensaje",
                        "messageTitle",
                        response.objMensaje.titulo,
                        "message",
                        response.objMensaje.mensaje,
                        "lnkContactenos",
                        response.objMensaje.flagContactenos);
                }

                //RegisterEventsListenerDeMisFavorito();
                initAutocompleteFiltros();

                // JANAQ 060620
                if (typeof initAutoCompleteFiltrosMixPanel == 'function') {
                    initAutoCompleteFiltrosMixPanel();
                }

                LoadingAdminPage.showOrHideLoadingPage(false);
            }
        },
        error: function (dataError) {
            console.info(dataError);
        }
    });
}

Filtros.changeCboPaisB = function (urlPost) {
        $.ajax({
            type: "POST",
            url: urlPost,
            data: {
                tipoOpe: $('input:radio[name=rdbTipoOpe]:checked').val(),
                textoCboPaisB: $("#cboPaisB").find("option:selected").text(),
                valorCboPaisB: $("#cboPaisB").find("option:selected").val(),
                indexCboPaisB: $("#cboPaisB").prop('selectedIndex')
    },
        beforeSend: function () {
            LoadingAdminPage.showOrHideLoadingPage(true);
        },
        success: function (response) {
            Filtros.resetFilters(response.flagVisibleMisFiltros);
            if (response.nuevosFiltros.length > 0) {
                Filtros.addFilterToFiltersList(response.nuevosFiltros);
            }

            LoadingAdminPage.showOrHideLoadingPage(false);
        },
        error: function (dataError) {
            console.info(dataError);
        }
    });
}

Filtros.clickEliminarFiltros = function (urlPost, pIdioma) {

    var numOptionPrevioEliminar = $("#lstFiltros option").length;
    var numFiltrosEliminar = $("#lstFiltros > option:selected").length;

    var vFiltrarDatos = true;
    var vTextRecordsFound = $("#totalRecordsFound").text().trim();

    if (vTextRecordsFound == "" ||
        vTextRecordsFound.substr(0, 1).toUpperCase() == "N") {
        vFiltrarDatos = false;
    }

    $.ajax({
        type: "POST",
        traditional: true,
        url: urlPost,
        data: {
            filtrosSeleccionados: $("#lstFiltros").val(),
            nroTotalFiltros: numOptionPrevioEliminar,
            nroFiltrosEliminar: numFiltrosEliminar,
            codPais: $("#cboPais").find("option:selected").val(),
            codPais2: $("#cboPais2").find("option:selected").val(),
            tipoOpe: $('input:radio[name=rdbTipoOpe]:checked').val(),
            anioMesIni: Fecha.getYearAndMonth($('#cboDesde').datepicker('getDate')),
            anioMesFin: Fecha.getYearAndMonth($('#cboHasta').datepicker('getDate')),
            indexCboPaisB: $("#cboPaisB").prop('selectedIndex'),
            codPaisB: $("#cboPaisB").find("option:selected").val(),
            filtrarDatos: vFiltrarDatos,
            idioma: pIdioma
        },
        beforeSend: function () {
            LoadingAdminPage.showOrHideLoadingPage(true);
        },
        success: function (response) {
            console.log(response);
            if (response.objMensaje == null) {

                if ((numOptionPrevioEliminar - numFiltrosEliminar) <= 0) {
                    Filtros.loadDataComboBox(response.objRestablecer.listOriginOrDestinationCountry, "cboPaisB");
                    Filtros.cargarPeriodos(response.objRestablecer.objPeriodos);
                    Filtros.resetFilters(response.objRestablecer.flagVisibleMisFiltros);
                    LoadingAdminPage.showOrHideLoadingPage(false);
                } else {

                    if (response.flagCboPaisB) {
                        $("#cboPaisB").prop('selectedIndex', 0);
                    }

                    $("#lstFiltros > option:selected").each(function () {
                        $(this).remove();
                    });

                    Filtros.adjustSizeHeightFilters();

                    if (response.gridData != null) {
                        CargarDatosDeTablas(response.gridData, response.totalRecordsFound, response.hideTabExcel, response.FlagRegMax);
                    } else {
                        LoadingAdminPage.showOrHideLoadingPage(false);
                    }
                }
            } else {

                if (response.cantReg == 0) {
                    $("#lstFiltros > option:selected").each(function () {
                        $(this).remove();
                    });

                    Filtros.adjustSizeHeightFilters();

                    if (response.totalRecordsFound != undefined) {
                        $("#divRegisterFound").removeClass("no-display");
                        $("#totalRecordsFound").text(response.totalRecordsFound);
                    }

                    //    $("#content_tabs").html("");

                    //}

                    $("#content_tabs").html("");
                    Filtros.disableAndHideFiltersFieldsByMyGroups(true);
                    //CargarDatosDeTablas(response.gridData, response.totalRecordsFound);

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
                    ModalAdmin.showVentanaMensajeWithTitleAndMessage("ModalVentanaMensaje",
                        "messageTitle",
                        response.objMensaje.titulo,
                        "message",
                        response.objMensaje.mensaje,
                        "lnkContactenos",
                        response.objMensaje.flagContactenos);
                }
            }
            
        },
        error: function (dataError) {
            console.log("error");
            console.info(dataError);
        }
    });
}

Filtros.clickAgregarDesComercial = function (urlPost, pIdioma) {
    var vTxtDesComercialB = $("#txtDesComercialB").val();
    if (vTxtDesComercialB == "")
        return;

    $.ajax({
        type: "POST",
        url: urlPost,
        data: {
            idioma : pIdioma,
            txtDesComercialB: vTxtDesComercialB,
            numFiltrosExistentes: $("#lstFiltros option").length
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
                Filtros.addFilterToFiltersList(response.listaPalabras);
                //Filtros.setVisibleFilltrosDeAgregar(!response.maximoLimiteFiltros);
                //Filtros.setFiltrosBuscar(response.validarBotones);
                $("#txtDesComercialB").val("");
                LoadingAdminPage.showOrHideLoadingPage(false);
            }
            
        },
        error: function (dataError) {
            console.info(dataError);
        }
    });
}

Filtros.agregarPartida = function (urlPost, pIdSeleccionado) {
    $.ajax({
        type: "POST",
        url: urlPost,
        data: {
            idSeleccionado: pIdSeleccionado,
            codPais: $("#cboPais").find("option:selected").val(),
            tipoOpe: $('input:radio[name=rdbTipoOpe]:checked').val(),
            numFiltrosExistentes: $("#lstFiltros option").length
        },
        beforeSend: function () {
            LoadingAdminPage.showOrHideLoadingPage(true);
        },
        success: function (response) {
            if (response.mensaje == "") {
                Filtros.addFilterToFiltersList(response.nuevoFiltro);
                Filtros.setVisibleFilltrosDeAgregar(!response.maximoLimiteFiltros);
            }
            LoadingAdminPage.showOrHideLoadingPage(false);
        }
    });
}

Filtros.comunAgregarFitroAutocompletado = function (urlPost, pIdSeleccionado, pTextoSeleccionado) {
    $.ajax({
        type: "POST",
        url: urlPost,
        data: {
            idSeleccionado: pIdSeleccionado,
            textoSeleccionado: pTextoSeleccionado,
            codPais: $("#cboPais").find("option:selected").val(),
            numFiltrosExistentes: $("#lstFiltros option").length
        },
        beforeSend: function () {
            LoadingAdminPage.showOrHideLoadingPage(true);
        },
        success: function (response) {
            if (response.mensaje == "") {
                Filtros.addFilterToFiltersList(response.nuevoFiltro);
                Filtros.setVisibleFilltrosDeAgregar(!response.maximoLimiteFiltros);
            }

            LoadingAdminPage.showOrHideLoadingPage(false);
        }
    });
}

Filtros.agregarImportadorExp = function (urlPost, pIdSeleccionado, pTextoSeleccionado) {
    $.ajax({
        type: "POST",
        url: urlPost,
        data: {
            idSeleccionado: pIdSeleccionado,
            importadorExpB: pTextoSeleccionado,
            numFiltrosExistentes: $("#lstFiltros option").length
        },
        beforeSend: function () {
            LoadingAdminPage.showOrHideLoadingPage(true);
        },
        success: function (response) {
            if (response.mensaje == "") {
                Filtros.addFilterToFiltersList(response.nuevoFiltro);
                Filtros.setVisibleFilltrosDeAgregar(!response.maximoLimiteFiltros);
            }
            LoadingAdminPage.showOrHideLoadingPage(false);
        }
    });
}

Filtros.txtNandinaBautocompleteListener = function (urlPost, urlAgregarFiltro, pIdioma) {
    $("#txtNandinaB").autocomplete({
        //appendTo: "#autocompletePartida",
        source: function (request, response) {
            $.ajax({
                url: urlPost,
                type: "POST",
                dataType: "json",
                data: {
                    nandinaB: request.term,
                    codPais: $("#cboPais").find("option:selected").val(),
                    codPais2: $("#cboPais2").find("option:selected").val()
                },
                success: function (data) {
                    
                    response(
                        $.map(data.Data,
                            function (item) {
                                return { id: item.id, label: item.value, value: item.value };
                            }));
                }
            });
        },
        minLength: 2,
        select: function (event, ui) {
            Filtros.agregarPartida(urlAgregarFiltro, ui.item.id);

            $(this).val("");
            $(this).blur();
            return false;
        }
    });
}

Filtros.txtImportadorBautocompleteListener = function (urlPost, urlAgregarFiltro) {
    $("#txtImportadorB").autocomplete({
        //appendTo: '#autocompleteImportadorB',
        source: function (request, response) {
            $.ajax({
                url: urlPost,
                type: "POST",
                dataType: "json",
                data: {
                    codPais: $("#cboPais").find("option:selected").val(),
                    importadorB: request.term,
                    tipoOpe: $('input:radio[name=rdbTipoOpe]:checked').val()
                },
                success: function (data) {
                    response($.map(data,
                        function (item) {
                            return {
                                id: item.id,
                                label: item.value,
                                value: item.value,
                                texto: item.texto
                            };
                        }));
                }
            });
        },
        minLength: 2,
        select: function (event, ui) {
            Filtros.comunAgregarFitroAutocompletado(urlAgregarFiltro, ui.item.id, ui.item.value);
            $(this).val("");
            $(this).blur();
            return false;
        }
    });

}

Filtros.txtProveedorBautocompleteListener = function (urlPost, urlAgregarFiltro) {
    $("#txtProveedorB").autocomplete({
        //appendTo: '#autocompleteProveedorB',
        source: function (request, response) {
            $.ajax({
                url: urlPost,
                type: "POST",
                dataType: "json",
                data: {
                    proveedorB: request.term
                },
                success: function (data) {
                   
                    response($.map(data,
                        function (item) {
                            return {
                                id: item.id,
                                label: item.value,
                                value: item.value,
                                texto: item.texto
                            };
                        }));
                }
            });
        },
        minLength: 2,
        select: function (event, ui) {
           
            Filtros.comunAgregarFitroAutocompletado(urlAgregarFiltro, ui.item.id, ui.item.value);

            $(this).val("");
            $(this).blur();
            return false;
        }
    });
}

Filtros.txtExportadorBautocompleteListener = function (urlPost, urlAgregarFiltro) {
    $("#txtExportadorB").autocomplete({
        //appendTo: 'autocompleteExportadorB',
        source: function (request, response) {
            $.ajax({
                url: urlPost,
                type: "POST",
                dataType: "json",
                data: {
                    importadorB: request.term,
                    codPais: $("#cboPais").find("option:selected").val(),
                    tipoOpe: $('input:radio[name=rdbTipoOpe]:checked').val()
                },
                success: function (data) {
                    response($.map(data,
                        function (item) {
                            return { id: item.id, label: item.value, value: item.value };
                        }));
                }
            });
        },
        minLength: 2,
        select: function (event, ui) {
            Filtros.comunAgregarFitroAutocompletado(urlAgregarFiltro, ui.item.id, ui.item.value);
            $(this).val("");
            $(this).blur();
            return false;
        }
    });
}

Filtros.txtImportadorExpBautocompleteListener = function (urlPost, urlAgregarFiltro) {
    $("#txtImportadorExpB").autocomplete({
        //appendTo: '#autocompleteImportadorExpB',
        source: function (request, response) {
            $.ajax({
                url: urlPost,
                type: "POST",
                dataType: "json",
                data: {
                    proveedorB: request.term
                },
                success: function (data) {
               
                    response($.map(data,
                        function (item) {
                            return {
                                id: item.id,
                                label: item.value,
                                value: item.value,
                                texto: item.texto
                            };
                        }));
                }
            });
        },
        minLength: 2,
        select: function (event, ui) {
           
            //Filtros.addFilterSelected("txtImportadorExpB", ui.item.id, '@Url.Action("AgregaImportadorExp", "Consulta")');
            Filtros.comunAgregarFitroAutocompletado(urlAgregarFiltro, ui.item.id, ui.item.value);
            $(this).val("");
            $(this).blur();
            return false;
        }
    });
}

/**
 * Funciones de Mis Favoritos
 */
function MisFavoritos() {
}

MisFavoritos.showMyFovourites = function (modalTitle, favouriteType, urlPost, pIdioma, textFieldSelectInit) {
    $("#modal-favoritos-titulo").text(modalTitle);

    $.ajax({
        type: "POST",
        url: urlPost,
        data: {
            tipoFavorito: favouriteType,
            codPais2: $("#cboPais2").find("option:selected").val(),
            codPais: $("#cboPais").find("option:selected").val(),
            tipoOpe: $('input:radio[name=rdbTipoOpe]:checked').val(),
            idioma: pIdioma
        },
        beforeSend: function () {
            LoadingAdminPage.showOrHideLoadingPage(true);
        },
        success: function (response) {
            $("#txtFavoritoF").val("");
            $("#modal-favoritos-body").html("");
            $("#modal-favoritos-body").html(response.htmlRowsFavourites);

            $("select[name='cboFavoritosF']").html("");
            $("select[name='cboFavoritosF']")
                .append("<option selected='selected' value='0'>[ " + textFieldSelectInit + " ]</option>");

            $.each(response.listFavouritesOptions,
                function (index, val) {
                    $("select[name='cboFavoritosF']")
                        .append("<option value=" + val.IdFavorito + ">" + val.Favorito + "</option>");
                });

            $("#pagingFavourites").html("");
            $("#pagingFavourites").html(response.htmlPagingFavourites);

            $('#pagingFavourites').data('tipoFavorito', favouriteType);

            LoadingAdminPage.showOrHideLoadingPage(false);

            $('#ModalFavoritos').modal({
                show: true,
                backdrop: 'static',
                keyboard: false
            });
        },
        error: function (data) {
            console.log(data);
        }
    });
}




MisFavoritos.cboFavoritosF_SelectedIndexChangedListener = function (urlPost, pIdioma) {
    $("#cboFavoritosF").change(function () {
        $.ajax({
            type: "POST",
            url: urlPost,
            data: {
                cboFavoritosFDataText: $(this).find("option:selected").text(),
                cboFavoritosFDataValue: $(this).find("option:selected").val(),
                idioma: pIdioma,
                tipoOpe: $('input:radio[name=rdbTipoOpe]:checked').val(),
                numFiltrosExistentes: $("#lstFiltros option").length
            },
            beforeSend: function () {
                LoadingAdminPage.showOrHideLoadingPage(true);
            },
            success: function (response) {

                if (response.objMensaje != null) {
                    ModalAdmin.showVentanaMensajeWithTitleAndMessage("ModalVentanaMensaje",
                        "messageTitle",
                        response.objMensaje.titulo,
                        "message",
                        response.objMensaje.mensaje,
                        "lnkContactenos",
                        response.objMensaje.flagContactenos);
                } else {
                    if (response.nuevoFiltro) {
                        Filtros.addFilterToFiltersList(response.newItems);
                    }
                    Filtros.disableAndHideFiltersFieldsByMyGroups(response.desabilitarControles);

                    if (response.maximoLimiteFiltros) {
                        Filtros.setVisibleFilltrosDeAgregar(!response.maximoLimiteFiltros);
                    }
                }

                LoadingAdminPage.showOrHideLoadingPage(false);
                ModalAdmin.hide("ModalFavoritos");
            },
            error: function (dataError) {
                console.info(dataError);
            }
        });
    });
}

MisFavoritos.AgregarFavAFiltrosClickEventListener = function (urlPost, pIdioma) {
    var chkArray = [];
    LoadingAdminPage.showOrHideLoadingPage(true);
    $("#modal-favoritos-body tr>td input:checked").each(function () {
        chkArray.push($(this).val());
    });

    if (chkArray.length > 0) {

        var vFavoritosSeleccionados = chkArray.join(",");

        $.ajax({
            type: "POST",
            url: urlPost,
            traditional: true,
            data: {
                favoritosSeleccionados: vFavoritosSeleccionados,
                codPais2: $("#cboPais2").find("option:selected").val(),
                codPais: $("#cboPais").find("option:selected").val(),
                tipoOpe: $('input:radio[name=rdbTipoOpe]:checked').val(),
                idioma: pIdioma,
                numFiltrosExistentes: $("#lstFiltros option").length
            },
            beforeSend: function () {
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
                }
                else {
                    if (response.nuevosFiltros) {
                        Filtros.addFilterToFiltersList(response.listNuevosFiltros);
                    }
                        Filtros.setVisibleFilltrosDeAgregar(!response.maximoLimiteFiltros);
                }
                LoadingAdminPage.showOrHideLoadingPage(false);
                ModalAdmin.hide("ModalFavoritos");
            },
            error: function (dataError) {
                console.info(dataError);
            }
        });

    } else {
        LoadingAdminPage.showOrHideLoadingPage(false);
        ModalAdmin.hide("ModalFavoritos");
    }
}

MisFavoritos.AgregarGrpAFiltros_Click = function (urlPost, pIdGrupo) {
    $.ajax({
        type: "POST",
        url: urlPost,
        data: {
            idGrupo: pIdGrupo
        },
        beforeSend: function () {
        },
        success: function (response) {
            if (response.nuevoFiltro) {
                Filtros.addFilterToFiltersList(response.newItems);
            }
            Filtros.disableAndHideFiltersFieldsByMyGroups(response.desabilitarControles);
            ModalAdmin.hide("ModalFavoritos");
        },
        error: function (dataError) {
            console.info(dataError);
        }
    });

}

MisFavoritos.txtFavoritoFautocompleteListener = function (urlPost, urlAgregarFiltro, pIdioma) {

    $("#txtFavoritoF").autocomplete({
        appendTo: "#autocompleteFavoritoF",
        source: function (request, response) {
            $.ajax({
                url: urlPost,
                type: "POST",
                dataType: "json",
                data: {
                    codPais: $("#cboPais").find("option:selected").val(),
                    codPais2: $("#cboPais2").find("option:selected").val(),
                    tipoOpe: $('input:radio[name=rdbTipoOpe]:checked').val(),
                    favoritoB: request.term,
                    idioma: pIdioma
                },
                success: function (data) {
                    response(
                        $.map(data.listaFavoritos,
                            function (item) {

                                return {
                                    id: item.IdFavorito,
                                    label: item.Favorito,
                                    value: item.Favorito
                                };
                            }));
                }
            });
        },
        minLength: 2,
        select: function (event, ui) {
            MisFavoritos.agregarFavoritoAutocompleteSelect(ui.item.id, ui.item.label, urlAgregarFiltro, pIdioma);
            $(this).val("");
            $(this).blur();
            return false;
        }
    });
}

MisFavoritos.agregarFavoritoAutocompleteSelect =
    function (pidAutocompletado, pTextFavoritoF, urlAgregarFiltro, pIdioma) {
        $.ajax({
            type: "POST",
            url: urlAgregarFiltro,
            data: {
                idAutocompletado: pidAutocompletado,
                textFavoritoF: pTextFavoritoF,
                tipoOpe: $('input:radio[name=rdbTipoOpe]:checked').val(),
                numFiltrosExistentes: $("#lstFiltros option").length,
                idioma: pIdioma
            },
            beforeSend: function () {
                LoadingAdminPage.showOrHideLoadingPage(true);
            },
            success: function (response) {
                if (response.existeData) {

                    if (response.nuevoFiltro) {
                        Filtros.addFilterToFiltersList(response.nuevosFiltros);
                    }
                    Filtros.disableAndHideFiltersFieldsByMyGroups(response.desabilitarControles);
                    if (response.maximoLimiteFiltros) {
                        Filtros.setVisibleFilltrosDeAgregar(!response.maximoLimiteFiltros);
                    }
                    LoadingAdminPage.showOrHideLoadingPage(false);
                    ModalAdmin.hide("ModalFavoritos");
                } else {
                    LoadingAdminPage.showOrHideLoadingPage(false);
                }
               
            },
            error: function (dataError) {

            }
        });
    }

MisFavoritos.clickBuscarF = function (urlPost, pIdioma) {
    $.ajax({
        url: urlPost,
        type: "POST",
        dataType: "json",
        data: {
            txtFavoritoF: $("#txtFavoritoF").val(),
            codPais2: $("#cboPais2").find("option:selected").val(),
            codPais: $("#cboPais").find("option:selected").val(),
            tipoOpe: $('input:radio[name=rdbTipoOpe]:checked').val(),
            idioma: pIdioma
        },
        beforeSend: function () {
            LoadingAdminPage.showOrHideLoadingPage(true);
        },
        success: function (response) {

            $("#modal-favoritos-body").html("");
            $("#modal-favoritos-body").html(response.htmlRowsFavourites);


            $("#pagingFavourites").html(response.htmlPagingFavourites);

            LoadingAdminPage.showOrHideLoadingPage(false);
        },
        error: function (dataError) {

        }
    });
}

MisFavoritos.clickRestablecerF = function (urlPost, pIdioma) {
    $.ajax({
        url: urlPost,
        type: "POST",
        dataType: "json",
        data: {
            codPais2: $("#cboPais2").find("option:selected").val(),
            codPais: $("#cboPais").find("option:selected").val(),
            tipoOpe: $('input:radio[name=rdbTipoOpe]:checked').val(),
            idioma: pIdioma
        },
        beforeSend: function () {
            LoadingAdminPage.showOrHideLoadingPage(true);
        },
        success: function (response) {

            $("#txtFavoritoF").val("");

            $("#modal-favoritos-body").html("");
            $("#modal-favoritos-body").html(response.htmlRowsFavourites);


            $("#pagingFavourites").html(response.htmlPagingFavourites);

            LoadingAdminPage.showOrHideLoadingPage(false);
        },
        error: function (dataError) {
        }
    });
}

MisFavoritos.pagingClickListener = function (pIdioma) {
    $('#pagingFavourites').on('click',
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
                        tipoFavorito: $('#pagingFavourites').data('tipoFavorito'),
                        codPais2: $("#cboPais2").find("option:selected").val(),
                        codPais: $("#cboPais").find("option:selected").val(),
                        tipoOpe: $('input:radio[name=rdbTipoOpe]:checked').val(),
                        idioma: pIdioma
                    },
                    beforeSend: function () {
                        LoadingAdminPage.showOrHideLoadingPage(true);
                    },
                    success: function (response) {
                        $("#txtFavoritoF").val("");
                        $("#modal-favoritos-body").html("");
                        $("#modal-favoritos-body").html(response.htmlRowsFavourites);

                        $("#pagingFavourites").html(response.htmlPagingFavourites);

                        LoadingAdminPage.showOrHideLoadingPage(false);

                    },
                    error: function (data) {
                        console.log(data);
                    }
                });
            }
        });
}


function Busqueda() {
}

Busqueda.getBrowserInfo = function () {
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

Busqueda.searchDataByFilters = function (urlPost, pIdioma) {
    $.ajax({
        url: urlPost,
        type: "POST",
        data: {
            codPais: $("#cboPais").find("option:selected").val(),
            codPais2: $("#cboPais2").find("option:selected").val(),
            tipoOpe: $('input:radio[name=rdbTipoOpe]:checked').val(),
            anioMesIni: Fecha.getYearAndMonth($('#cboDesde').datepicker('getDate')),
            anioMesFin: Fecha.getYearAndMonth($('#cboHasta').datepicker('getDate')),
            indexCboPaisB: $("#cboPaisB").prop('selectedIndex'),
            codPaisB: $("#cboPaisB").find("option:selected").val(),
            idioma: pIdioma,
            codPaisText: $("#cboPais").find("option:selected").text()
        },
        beforeSend: function () {
            LoadingAdminPage.showOrHideLoadingPage(true);
        },
        success: function (response) {
            //console.log(response)
            //$("#divRegisterFound").addClass('no-display');
            if (response.objMensaje != null) {

                if (response.totalRecordsFound != "undefined") {
                    $("#divRegisterFound").removeClass("no-display");
                    $("#totalRecordsFound").text(response.totalRecordsFound);
                }
                

                if (response.gridData != null) {
                    $("#content_tabs").html("");
                    Filtros.disableAndHideFiltersFieldsByMyGroups(true);
                    CargarDatosDeTablas(response.gridData, response.totalRecordsFound, response.hideTabExcel, response.FlagRegMax);
                }
                ModalAdmin.showVentanaMensajeWithTitleAndMessage("ModalVentanaMensaje",
                    "messageTitle",
                    response.objMensaje.titulo,
                    "message",
                    response.objMensaje.mensaje,
                    "lnkContactenos",
                    response.objMensaje.flagContactenos);
                LoadingAdminPage.showOrHideLoadingPage(false);
            } else {
                $("#content_tabs").html("");
                Filtros.disableAndHideFiltersFieldsByMyGroups(true);
                CargarDatosDeTablas(response.gridData, response.totalRecordsFound);
            }
            MensajesConsole();
           
            try {
                //console.log(setModalsForFreeTrail)
                if (setModalsForFreeTrail != "undefined") {
                    // console.log("fired outer funcion");
                    window["setModalsForFreeTrail"]();
                }
            } catch (e) {

            }
            
        },
        error: function (dataError) {
            console.log(dataError);
        }
    });
}

Busqueda.scrollToHere = function () {
    $('html,body').animate({
        scrollTop: $("#RangoFechas").offset().top
    }, 800);
}

Busqueda.verInfoComplementario = function (
    urlVerInfo,
    tipoFiltroTab,
    urlOrdenarTabla,
    urlPaging,
    urlVerRegistros,
    urlPageChangeVerRegistro,
    urlBuscarPorDuaVerRegistro,
    urlBuscarPorDesComercialVerRegistro, hideTabExcel, FlagRegMax
) {
    $("#tabInfoComplementario").click(function () {
        var vThis = this;
        //console.log($(vThis).data('existe-vista'))
        //console.log($(vThis).data('existe-info'))
        if (!$(vThis).data('existe-vista')) {
            $.ajax({
                url: urlVerInfo,
                type: "POST",
                data: {
                    tipoOpe: $('input:radio[name=rdbTipoOpe]:checked').val(),
                    codPais: $("#cboPais").find("option:selected").val(),
                    codPais2Aux: $("#cboPais2").find("option:selected").val(),
                    indexCboPaisB: $("#cboPaisB").prop('selectedIndex'),
                    codPaisB: $("#cboPaisB").find("option:selected").val(),
                    anioMesIni: Fecha.getYearAndMonth($('#cboDesde').datepicker('getDate')),
                    anioMesFin: Fecha.getYearAndMonth($('#cboHasta').datepicker('getDate')),
                    hideTabExcel: hideTabExcel,
                    FlagRegMax: FlagRegMax,
                    
                },
                beforeSend: function () {
                    $("#tab-InfoComplementario").css({ "display": "none" });
                    LoadingAdminPage.showOrHideLoadingPage(true);
                },
                success: function (response) {
                    
                    $(vThis).data('existe-vista', true);
                    if (response.gridData.length > 0) {
                        $("#tab-InfoComplementario").removeAttr("style");
                        gArrayChartData = []; //clear variable chartData
                        $.each(response.gridData,
                            function (i, item) {
                                if (item.tabDataNumRows > 0) {
                                    var wp = $("#grid" + item.tabDataName);
                                    $("#grid" + item.tabDataName).html("");
                                    $("#grid" + item.tabDataName).html(item.tabDataList);
                                    if (item.tabTotalPages > 1) {
                                        
                                        TablaDeDatos.OnPageClick(item.tabDataName,
                                            tipoFiltroTab,
                                            urlPaging,
                                            urlVerRegistros,
                                            urlPageChangeVerRegistro,
                                            urlBuscarPorDuaVerRegistro,
                                            urlBuscarPorDesComercialVerRegistro,
                                            item.tabTotalPages);
                                    }

                                    if (item.tabPieData != "") {

                                        Chart.LoadCboThemes("themesChart" + item.tabDataName);



                                        $('#themesChart' + item.tabDataName).colorselector();

                                        Highcharts.setOptions(dbThemes[0].value);

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
                                            }),
                                            lang: gLangOptionHighcharts
                                        });

                                        ColumnAdmin.loadData("pieChart" + item.tabDataName,
                                            item.pieTitle,
                                            item.tabPieData,
                                            urlVerRegistros,
                                            urlPageChangeVerRegistro,
                                            urlBuscarPorDuaVerRegistro,
                                            urlBuscarPorDesComercialVerRegistro,
                                            item.tabDataName,
                                            item.chartMyChart);

                                        var auxDataPie = {};

                                        auxDataPie.filtro = item.tabDataName;
                                        auxDataPie.value = item.chartMyChart;

                                        gArrayPiesData.push(auxDataPie);
                                        gArrayChartData.push(auxDataPie);

                                        var auxDataPie = {};

                                        auxDataPie.filtro = item.tabDataName;
                                        auxDataPie.value = item.chartMyChart;
                                        gArrayPiesData.forEach(function (i) {

                                            if (i.filtro == 'InfoTabla') {
                                                i.value = item.chartMyChart;
                                            }
                                        });

                                        gArrayChartData.forEach(function (i) {
                                            if (i.filtro == 'InfoTabla') {
                                                i.value = item.chartMyChart;
                                            }
                                        });

                                    }
                                    setTimeout(applyDot(wp), 500);
                                }
                            });

                        $('#pagingTabInfoTabla').twbsPagination('showOnlyPage', 1);
                        TablaDeDatos.lnkOrdenarTablaClick(urlOrdenarTabla,
                            urlVerRegistros,
                            urlPageChangeVerRegistro,
                            urlBuscarPorDuaVerRegistro,
                            urlBuscarPorDesComercialVerRegistro,
                            "",
                            "")

                        /*VerRegistro.lnkVerRegistroClick(urlVerRegistros,
                            urlPageChangeVerRegistro,
                            urlBuscarPorDuaVerRegistro,
                            urlBuscarPorDesComercialVerRegistro);*/


                    } else {
                        $(vThis).data('existe-info', false);
                        $("#tabsMisBusquedas").tabs("option", "active", 0);
                        
                        ModalAdmin.showVentanaMensajeWithTitleAndMessage("ModalVentanaMensaje",
                            "messageTitle",
                            "Veritrade",
                            "message",
                            'No hay Datos de Informacion Complementaria',
                            "lnkContactenos",
                            false);
                    }



                    LoadingAdminPage.showOrHideLoadingPage(false);
                },
                error: function (dataError) {
                    console.log(dataError);
                }
            });
        } else if (!$(vThis).data('existe-info')) {
            $("#tabsMisBusquedas").tabs("option", "active", 0);
            ModalAdmin.showVentanaMensajeWithTitleAndMessage("ModalVentanaMensaje",
                "messageTitle",
                "Veritrade",
                "message",
                'No hay Datos de Informacion Complementaria',
                "lnkContactenos",
                false);
        }

    });
}

Busqueda.verMarcas = function (urlVerMarcas,
    tipoFiltroTab,
    urlOrdenarTabla,
    urlPaging,
    urlVerRegistros,
    urlPageChangeVerRegistro,
    urlBuscarPorDuaVerRegistro,
    urlBuscarPorDesComercialVerRegistro, hideTabExcel, FlagRegMax) {
    $("#tabMarcasModelos").click(function () {
        var vThis = this;
        if (!$(vThis).data('existe-vista')) {
            $.ajax({
                url: urlVerMarcas,
                type: "POST",
                data: {
                    tipoOpe: $('input:radio[name=rdbTipoOpe]:checked').val(),
                    codPais: $("#cboPais").find("option:selected").val(),
                    indexCboPaisB: $("#cboPaisB").prop('selectedIndex'),
                    hideTabExcel: hideTabExcel,
                    FlagRegMax: FlagRegMax
                },
                beforeSend: function () {
                    LoadingAdminPage.showOrHideLoadingPage(true);
                },
                success: function (response) {
                    gArrayChartData = []; //clear variable chartData
                    $.each(response.gridData,
                        function (i, item) {
                            if (item.tabDataNumRows > 0) {
                                var wp = $("#grid" + item.tabDataName);
                                $("#grid" + item.tabDataName).html("");
                                $("#grid" + item.tabDataName).html(item.tabDataList);
                                if (item.tabTotalPages > 1) {
                                    TablaDeDatos.OnPageClickMarcaModelo(item.tabDataName,
                                        tipoFiltroTab,
                                        urlPaging,
                                        urlVerRegistros,
                                        urlPageChangeVerRegistro,
                                        urlBuscarPorDuaVerRegistro,
                                        urlBuscarPorDesComercialVerRegistro,
                                        item.tabTotalPages);
                                }

                                if (item.tabPieData != "") {

                                    Chart.LoadCboThemes("themesChart" + item.tabDataName);

                                    
                                    
                                    $('#themesChart' + item.tabDataName).colorselector();

                                    Highcharts.setOptions(dbThemes[0].value);
                                    
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
                                        }),
                                        lang: gLangOptionHighcharts
                                    });
                                    
                                    PieAdmin.loadData("pieChart" + item.tabDataName,
                                        item.pieTitle,
                                        item.tabPieData,
                                        urlVerRegistros,
                                        urlPageChangeVerRegistro,
                                        urlBuscarPorDuaVerRegistro,
                                        urlBuscarPorDesComercialVerRegistro,
                                        item.tabDataName);


                                    var auxDataPie = {};

                                    auxDataPie.filtro = item.tabDataName;
                                    auxDataPie.value = item.tabPieData;
                                    gArrayPiesData.forEach(function (i) {
                                        if (i.filtro == 'Marca') {
                                            i.value = item.tabPieData;
                                        }
                                    });

                                    gArrayChartData.forEach(function (i) {
                                        if (i.filtro == 'Marca') {
                                            i.value = item.tabPieData;
                                        }
                                    });

                                    //gArrayPiesData.push(auxDataPie);

                                    //gArrayPiesData[1] = auxDataPie;
                                    //gArrayChartData[1] = auxDataPie;
                                    //gArrayChartData.push(auxDataPie);

                                    //PieAdmin.changeCboThemes(urlVerRegistros,
                                    //    urlPageChangeVerRegistro,
                                    //    urlBuscarPorDuaVerRegistro,
                                    //    urlBuscarPorDesComercialVerRegistro);                                    
                                }
                                setTimeout(applyDot(wp), 500);
                            }
                            VerRegistro.lnkVerRegistroClick(urlVerRegistros,
                                urlPageChangeVerRegistro,
                                urlBuscarPorDuaVerRegistro,
                                urlBuscarPorDesComercialVerRegistro,
                                "#grid" + item.tabDataName);
                        });
                    TablaDeDatos.lnkOrdenarTablaClick(urlOrdenarTabla,
                        urlVerRegistros,
                        urlPageChangeVerRegistro,
                        urlBuscarPorDuaVerRegistro,
                        urlBuscarPorDesComercialVerRegistro,
                        "",
                        "");

                    

                    $(vThis).data('existe-vista', "true");
                    

                    LoadingAdminPage.showOrHideLoadingPage(false);
                },
                error: function (dataError) {
                    console.log(dataError);
                }
            });
        }
    });
}



Busqueda.extraFiltroDetalleExcel = function (urlVerDetalle,
    tipoFiltroTab,
    urlOrdenarTabla,
    urlPaging,
    urlVerRegistros,
    urlPageChangeVerRegistro,
    urlBuscarPorDuaVerRegistro,
    urlBuscarPorDesComercialVerRegistro,
    urlVerSentinel,
    urlIrASentinel, urlDetalleExcelId
    , txtDua, txtDesCom, lbl) {
    //var _applyFilter = function (txtDua, txtDesCom, lbl) {

        var txtPais = $("#cboPais").find(":selected").text();
        $.ajax({
            url: urlVerDetalle,
            type: "POST",
            data: {
                tipoOpe: $('input:radio[name=rdbTipoOpe]:checked').val(),
                codPais: $("#cboPais").find("option:selected").val(),
                indexCboPaisB: $("#cboPaisB").prop('selectedIndex'),
                txtPais: txtPais,
                txtDua: txtDua,
                txtDesCom: txtDesCom,
                codPais2: $("#cboPais2").find("option:selected").val(),
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
                    $.each(response.gridData,
                        function (i, item) {

                            if (item.tabDataNumRows > 0) {
                                var wp = $("#grid" + item.tabDataName);
                                $("#grid" + item.tabDataName).html("");
                                $("#grid" + item.tabDataName).html(item.tabDataList);
                                if (item.tabTotalPages > 1) {
                                    TablaDeDatos.OnPageClickDetalleExcel(item.tabDataName,
                                        tipoFiltroTab,
                                        urlPaging,
                                        urlVerRegistros,
                                        urlPageChangeVerRegistro,
                                        urlBuscarPorDuaVerRegistro,
                                        urlBuscarPorDesComercialVerRegistro,
                                        item.tabTotalPages,
                                        txtPais,
                                        urlVerSentinel,
                                        urlIrASentinel, urlDetalleExcelId, txtDua,
                                        txtDesCom);

                                }
                                if (txtDesCom != "") {
                                    $.each($("#gridDetalleExcel p"), function () {

                                        var arreglo = $(this).html().split("<a");
                                        var etiqueta = '<a ' + arreglo[1];
                                        var term = txtDua != "" ? txtDua : txtDesCom;
                                        //console.log(term);
                                        //var src_str = $(this).html();
                                        var src_str = arreglo[0];
                                        //var term = "mY text";
                                        term = term.replace(/(\s+)/, "(<[^>]+>)*$1(<[^>]+>)*");
                                        //console.log(term);
                                        var pattern = new RegExp("(" + term + ")", "gi");

                                        src_str = src_str.replace(pattern, "<mark>$1</mark>");
                                        src_str = src_str.replace(/(<mark>[^<>]*)((<[^>]+>)+)([^<>]*<\/mark>)/, "$1</mark>$2<mark>$4");

                                        //$(this).html(src_str);
                                        $(this).html(src_str + etiqueta);
                                        //console.log(src_str + etiqueta);
                                    });
                                }
                                

                                //$("#grid" + item.tabDataName).closest("th").find("input").eq(0);
                               /* var term = !txtDua.localeCompare("") ? txtDua : txtDesCom;
                                var src_str = $("#grid" + item.tabDataName).html();
                                //var term = "mY text";
                                term = term.replace(/(\s+)/, "(<[^>]+>)*$1(<[^>]+>)*");
                                var pattern = new RegExp("(" + term + ")", "gi");

                                src_str = src_str.replace(pattern, "<mark>$1</mark>");
                                src_str = src_str.replace(/(<mark>[^<>]*)((<[^>]+>)+)([^<>]*<\/mark>)/, "$1</mark>$2<mark>$4");

                                $("#grid" + item.tabDataName).html(src_str);*/

                                setTimeout(applyDot(wp, 3, "left"), 500);
                            }
                            if (lbl !== undefined) $(lbl).html(item.lblRecords);  
                        });

                    setTimeout(function () {
                        TablaDeDatos.ClickColumnLnkSentinel(urlVerSentinel, urlIrASentinel);
                        TablaDeDatos.ClickColumnLnkVerDetalleExelById(urlDetalleExcelId);
                    }, 500);

                    LoadingAdminPage.showOrHideLoadingPage(false);
                }

                $("#txtDuaExtra").val(txtDua);
                $("#txtDesComExtra").val(txtDesCom);

            },
            error: function (dataError) {
                console.log(dataError);
            }
        });
    //};


}

function applyDot(wp, rows, placement) {

    //$(".more_less").popover({ container: 'body', trigger: "hover", placement: "left" });
    if (rows === undefined) rows = 1;
    if (placement === undefined) placement = "right";
    var browser = Busqueda.getBrowserInfo().toLowerCase();
    
    /*console.log("el browser es: " + browser);
    console.log("es chrome" + browser.indexOf("chrome"));
    console.log("es firefox" + browser.indexOf("firefox"));*/
    //console.log(screen.width + " x " + screen.height);


    $(wp).find(".wspace-normal").each(function (i, e) {

        var height = $(e).outerHeight();
        if (browser.indexOf("chrome") > -1)
            height = 12.5;
        else if (browser.indexOf("firefox") > -1)
            height = 13.5;
        //console.log(rows)
        $(e).dotdotdot({
            keep: ".more_less",
            ellipsis: "",
            height: (rows) * height,
            callback: function (isTruncated) {
                $(this).find(".more_less").css("display", (!isTruncated ? "none" : "inline"))
                    .popover({ container: 'body', trigger: "hover", placement: placement });
            }
        });
    });

}

Busqueda.verDetalleExcel = function (urlVerDetalle,
    tipoFiltroTab,
    urlOrdenarTabla,
    urlPaging,
    urlVerRegistros,
    urlPageChangeVerRegistro,
    urlBuscarPorDuaVerRegistro,
    urlBuscarPorDesComercialVerRegistro,
    urlVerSentinel,
    urlIrASentinel, urlDetalleExcelId) {
    $("#tabDetalleExcel").click(function () {
        var vThis = this;
        if (!$(vThis).data('existe-vista')) {
            var txtPais = $("#cboPais").find(":selected").text();
            $.ajax({
                url: urlVerDetalle,
                type: "POST",
                data: {
                    tipoOpe: $('input:radio[name=rdbTipoOpe]:checked').val(),
                    codPais: $("#cboPais").find("option:selected").val(),
                    indexCboPaisB: $("#cboPaisB").prop('selectedIndex'),
                    txtPais: txtPais,
                    txtDua: "",
                    txtDesCom: "",
                    codPais2: $("#cboPais2").find("option:selected").val(),
                },
                beforeSend: function () {
                    LoadingAdminPage.showOrHideLoadingPage(true);
                },
                success: function (response) {
                    $.each(response.gridData,
                        function (i, item) {
                            if (item.tabDataNumRows > 0) {
                                var wp = $("#grid" + item.tabDataName);
                                $("#grid" + item.tabDataName).html("");
                                $("#grid" + item.tabDataName).html(item.tabDataList);
                                if (item.tabTotalPages > 1) {
                                    TablaDeDatos.OnPageClickDetalleExcel(item.tabDataName,
                                        tipoFiltroTab,
                                        urlPaging,
                                        urlVerRegistros,
                                        urlPageChangeVerRegistro,
                                        urlBuscarPorDuaVerRegistro,
                                        urlBuscarPorDesComercialVerRegistro,
                                        item.tabTotalPages,
                                        txtPais,
                                        urlVerSentinel,
                                        urlIrASentinel, urlDetalleExcelId,"", "");
                                }
                                //$(".more_less").popover({ container: 'body', trigger: "hover", placement: "left" });
                                setTimeout(applyDot(wp, 3, "left"), 500);
                            }
                        });


                    //VerRegistro.lnkVerRegistroClick(urlVerRegistros,
                    //    urlPageChangeVerRegistro,
                    //    urlBuscarPorDuaVerRegistro,
                    //    urlBuscarPorDesComercialVerRegistro);

                    $(vThis).data('existe-vista', "true");

                    setTimeout(function () {
                        TablaDeDatos.ClickColumnLnkSentinel(urlVerSentinel, urlIrASentinel);
                        TablaDeDatos.ClickColumnLnkVerDetalleExelById(urlDetalleExcelId);
                    }, 500);

                    LoadingAdminPage.showOrHideLoadingPage(false);
                    //console.log(setModalsForFreeTrail)
                    try {
                        if (setModalsForFreeTrail != "undefined") {
                            // console.log("fired outer funcion");
                            window["setModalsForFreeTrail"]();
                        }
                    } catch (e) {

                    }
                   
                },
                error: function (dataError) {
                    console.log(dataError);
                }
            });
        }
    });
}


Busqueda.verInfoTabla = function (urlInfoTabla,
    tipoFiltroTab,
    urlOrdenarTabla,
    urlPaging,
    urlVerRegistros,
    urlPageChangeVerRegistro,
    urlBuscarPorDuaVerRegistro,
    urlBuscarPorDesComercialVerRegistro,
    urlVerSentinel,
    urlIrASentinel, urlDetalleExcelId) {
    $("#tab-InfoTabla").click(function () {
        var vThis = this;
        if (!$(vThis).data('existe-vista')) {
            var txtPais = $("#cboPais").find(":selected").text();
            $.ajax({
                url: urlInfoTabla,
                type: "POST",
                data: {
                    tipoOpe: $('input:radio[name=rdbTipoOpe]:checked').val(),
                    codPais: $("#cboPais").find("option:selected").val(),
                    indexCboPaisB: $("#cboPaisB").prop('selectedIndex'),
                    codPais2: $("#cboPais2").find("option:selected").val(),
                },
                beforeSend: function () {
                    LoadingAdminPage.showOrHideLoadingPage(true);
                },
                success: function (response) {
                    $.each(response.gridData,
                        function (i, item) {
                            if (item.tabDataNumRows > 0) {
                                var wp = $("#grid" + item.tabDataName);
                                $("#grid" + item.tabDataName).html("");
                                $("#grid" + item.tabDataName).html(item.tabDataList);
                                if (item.tabTotalPages > 1) {
                                    TablaDeDatos.OnPageClickDetalleExcel(item.tabDataName,
                                        tipoFiltroTab,
                                        urlPaging,
                                        urlVerRegistros,
                                        urlPageChangeVerRegistro,
                                        urlBuscarPorDuaVerRegistro,
                                        urlBuscarPorDesComercialVerRegistro,
                                        item.tabTotalPages,
                                        txtPais,
                                        urlVerSentinel,
                                        urlIrASentinel, urlDetalleExcelId, "", "");
                                }
                                //$(".more_less").popover({ container: 'body', trigger: "hover", placement: "left" });
                                setTimeout(applyDot(wp, 3, "left"), 500);
                            }
                        });


                    //VerRegistro.lnkVerRegistroClick(urlVerRegistros,
                    //    urlPageChangeVerRegistro,
                    //    urlBuscarPorDuaVerRegistro,
                    //    urlBuscarPorDesComercialVerRegistro);

                    $(vThis).data('existe-vista', "true");

                    setTimeout(function () {
                        TablaDeDatos.ClickColumnLnkSentinel(urlVerSentinel, urlIrASentinel);
                        TablaDeDatos.ClickColumnLnkVerDetalleExelById(urlDetalleExcelId);
                    }, 500);

                    LoadingAdminPage.showOrHideLoadingPage(false);
                    //console.log(setModalsForFreeTrail)
                    try {
                        if (setModalsForFreeTrail != "undefined") {
                            // console.log("fired outer funcion");
                            window["setModalsForFreeTrail"]();
                        }
                    } catch (e) {

                    }

                },
                error: function (dataError) {
                    console.log(dataError);
                }
            });
        }
    });
}

function TablaDeDatos() {
}

/**
 * Responde a un cambio de item del combo de la cabecera de tablas de datos.
 * @param {string} urlPost url de petición de datos 
 * @param {string} pIdioma
 */
TablaDeDatos.CboFiltroIndexChanged = function (urlPost, pIdioma) {
    $(".cboFiltroTabla").change(function () {
        var vThis = this;
        var idCbo = $(this).attr("data-filtro-cbo");
        var valor = $(this).val();
        $.ajax({
            type: "POST",
            url: urlPost,
            data: {
                filtro: idCbo,
                valorFiltro: valor,
                tipoOpe: $('input:radio[name=rdbTipoOpe]:checked').val(),
                codPais: $("#cboPais").find("option:selected").val(),
                codPais2: $("#cboPais2").find("option:selected").val(),
                indexCboPaisB: $("#cboPaisB").prop('selectedIndex'),
                codPaisB: $("#cboPaisB").find("option:selected").val(),
                anioMesIni: Fecha.getYearAndMonth($('#cboDesde').datepicker('getDate')),
                anioMesFin: Fecha.getYearAndMonth($('#cboHasta').datepicker('getDate')),
                numFiltrosExistentes: $("#lstFiltros option").length,
                idioma: pIdioma
            },
            beforeSend: function () {
                LoadingAdminPage.showOrHideLoadingPage(true);
            },
            success: function (response) {
                if (!response.maximoLimiteFiltros) {
                    if (response.nuevoFiltro) {
                        Filtros.addFilterToFiltersList(response.nuevosFiltros);
                        CargarDatosDeTablas(response.gridData, response.totalRecordsFound, response.hideTabExcel, response.FlagRegMax);
                    } else {
                        $(vThis).prop('selectedIndex', 0);
                        LoadingAdminPage.showOrHideLoadingPage(false);
                    }
                } else {
                    $(vThis).prop('selectedIndex', 0);
                    LoadingAdminPage.showOrHideLoadingPage(false);
                }
            },
            error: function (dataError) {
                console.log(dataError);
                //LoadingAdminPage.showOrHideLoadingPage(false);
            }
        });
    });
}

TablaDeDatos.OnPageClick = function (pFiltro,
    pTipoFiltro,
    urlPost,
    urlVerRegistro,
    urlPageChangeVerRegistro,
    urlBuscarPorDuaVerRegistro,
    urlBuscarPorDesComercialVerRegistro,
    urlVerSentinel,
    ulrIrASentinel,
    totalPages, numPages) {
    if (numPages == undefined) numPages = 10;
    $('#paging' + pTipoFiltro + pFiltro).twbsPagination({
        totalPages: totalPages,
        visiblePages: numPages,//10,
        hideOnlyOnePage: true,
        initiateStartPageClick: false,
        onPageClick: function (event, page) {
            var chkArray = [];
            //console.log("page" + page)
            $("#tbody" + pTipoFiltro + pFiltro + " tr>td input:checked").each(function () {
                chkArray.push($(this).val());
            });
            var vRegistrosSeleccionados = chkArray.join(",");

            chkArray = [];
            $("#tbody" + pTipoFiltro + pFiltro + " tr>td input").each(function () {
                chkArray.push($(this).val());
            });
            var vRegistrosPagina = chkArray.join(",");

            $.ajax({
                type: "POST",
                url: urlPost,
                data: {
                    filtro: pFiltro,
                    tipoFiltro: pTipoFiltro,
                    pagina: page,
                    codPais2: $("#cboPais2").find("option:selected").val(),
                    codPais: $("#cboPais").find("option:selected").val(),
                    tipoOpe: $('input:radio[name=rdbTipoOpe]:checked').val(),
                    idTabla: $('#paging' + pTipoFiltro + pFiltro).data("id-tabla"),
                    idsSeleccionados: vRegistrosSeleccionados,
                    idsPagina: vRegistrosPagina,
                    anioMesIni: Fecha.getYearAndMonth($('#cboDesde').datepicker('getDate')),
                    anioMesFin: Fecha.getYearAndMonth($('#cboHasta').datepicker('getDate')),
                    indexCboPaisB: $("#cboPaisB").prop('selectedIndex'),
                    codPaisB: $("#cboPaisB").find("option:selected").val()
                },
                beforeSend: function () {
                    LoadingAdminPage.showOrHideLoadingPage(true);
                },
                success: function (response) {
                    var wp = $("#tbody" + pTipoFiltro + pFiltro);
                    $("#tbody" + pTipoFiltro + pFiltro).html(response.filasNuevaPagina);
                    $("#chkAll" + pTipoFiltro + pFiltro).prop('checked', false);
                    VerRegistro.lnkVerRegistroClick(urlVerRegistro,
                        urlPageChangeVerRegistro,
                        urlBuscarPorDuaVerRegistro,
                        urlBuscarPorDesComercialVerRegistro,
                        "#tbody" + pTipoFiltro + pFiltro);
                    TablaDeDatos.ClickColumnLnkSentinel(urlVerSentinel, ulrIrASentinel);
                    LoadingAdminPage.showOrHideLoadingPage(false);
                    //$(".more_less").popover({ container: 'body', trigger: "hover" });
                    setTimeout(applyDot(wp, (pTipoFiltro == "Tab" && pFiltro == "Partida" ? 2 : 1)), 500);
                },
                error: function (data) {
                    console.log(data);
                }
            });
            
        }
    });

    //setTimeout(function () {
    //    if ($('#paging' + pTipoFiltro + pFiltro).find("li").length === 0)
    //        $('#paging' + pTipoFiltro + pFiltro).html("<li class=\"page-item active\"><a href=\"javascript:void(0)\" class=\"page-link\">1</a></li>");
    //}, 800);

    
}

TablaDeDatos.OnPageClickMarcaModelo = function (pFiltro,
    pTipoFiltro,
    urlPost,
    urlVerRegistro,
    urlPageChangeVerRegistro,
    urlBuscarPorDuaVerRegistro,
    urlBuscarPorDesComercialVerRegistro,
    totalPages) {
    $('#paging' + pTipoFiltro + pFiltro).twbsPagination({
        totalPages: totalPages,
        visiblePages: 5,
        hideOnlyOnePage: true,
        initiateStartPageClick: false,
        onPageClick: function (event, page) {
            $.ajax({
                type: "POST",
                url: urlPost,
                data: {
                    filtro: pFiltro,
                    pagina: page,
                    codPais: $("#cboPais").find("option:selected").val(),
                    tipoOpe: $('input:radio[name=rdbTipoOpe]:checked').val(),
                    idTabla: $('#paging' + pTipoFiltro + pFiltro).data("id-tabla")
                },
                beforeSend: function () {
                    LoadingAdminPage.showOrHideLoadingPage(true);
                },
                success: function (response) {
                    var wp = $("#tbody" + pTipoFiltro + pFiltro);
                    $("#tbody" + pTipoFiltro + pFiltro).html(response.filasNuevaPagina);
                    VerRegistro.lnkVerRegistroClick(urlVerRegistro,
                        urlPageChangeVerRegistro,
                        urlBuscarPorDuaVerRegistro,
                        urlBuscarPorDesComercialVerRegistro,
                        "#tbody" + pTipoFiltro + pFiltro);

                    setTimeout(applyDot(wp), 500);
                    LoadingAdminPage.showOrHideLoadingPage(false);
                },
                error: function (data) {
                    console.log(data);
                }
            });
        }
    });
}


TablaDeDatos.OnPageClickDetalleExcel = function (pFiltro,
    pTipoFiltro,
    urlPost,
    urlVerRegistro,
    urlPageChangeVerRegistro,
    urlBuscarPorDuaVerRegistro,
    urlBuscarPorDesComercialVerRegistro,
    totalPages,
    txtPais,
    urlVerSentinel,
    ulrIrASentinel, urlDetalleExcelId, txtDua,
    txtDesCom) {
    $('#paging' + pTipoFiltro + pFiltro).twbsPagination({
        totalPages: totalPages,
        visiblePages: 5,
        hideOnlyOnePage: true,
        initiateStartPageClick: false,
        onPageClick: function (event, page) {
            $.ajax({
                type: "POST",
                url: urlPost,
                data: {
                    filtro: pFiltro,
                    pagina: page,
                    codPais: $("#cboPais").find("option:selected").val(),
                    tipoOpe: $('input:radio[name=rdbTipoOpe]:checked').val(),
                    idTabla: $('#paging' + pTipoFiltro + pFiltro).data("id-tabla"),
                    txtPais: txtPais,
                    txtDua: txtDua,
                    txtDesCom: txtDesCom,
                    codPais2: $("#cboPais2").find("option:selected").val()
                },
                beforeSend: function () {
                    LoadingAdminPage.showOrHideLoadingPage(true);
                },
                success: function (response) {
                    var wp = $("#tbody" + pTipoFiltro + pFiltro);
                    $("#tbody" + pTipoFiltro + pFiltro).html(response.filasNuevaPagina);
                    //VerRegistro.lnkVerRegistroClick(urlVerRegistro,
                    //    urlPageChangeVerRegistro,
                    //    urlBuscarPorDuaVerRegistro,
                    //    urlBuscarPorDesComercialVerRegistro);
                    //TablaDeDatos.ClickColumnLnkSentinel(urlVerSentinel, ulrIrASentinel);

                    TablaDeDatos.ClickColumnLnkSentinel(urlVerSentinel, ulrIrASentinel);

                    TablaDeDatos.ClickColumnLnkVerDetalleExelById(urlDetalleExcelId);

                    LoadingAdminPage.showOrHideLoadingPage(false);

                    $("#txtDuaExtra").val(txtDua);
                    $("#txtDesComExtra").val(txtDesCom);
                    //setTimeout(function() {
                    //    $(".more_less").popover({ container: 'body', trigger: "hover", placement: "left" });
                    //}, 500);

                    if (txtDesCom != "") {
                        $.each($("#gridDetalleExcel p"), function () {

                            var arreglo = $(this).html().split("<a");
                            var etiqueta = '<a ' + arreglo[1];
                            var term = txtDua != "" ? txtDua : txtDesCom;
                            //console.log(term);
                            //var src_str = $(this).html();
                            var src_str = arreglo[0];
                            //var term = "mY text";
                            term = term.replace(/(\s+)/, "(<[^>]+>)*$1(<[^>]+>)*");
                            //console.log(term);
                            var pattern = new RegExp("(" + term + ")", "gi");

                            src_str = src_str.replace(pattern, "<mark>$1</mark>");
                            src_str = src_str.replace(/(<mark>[^<>]*)((<[^>]+>)+)([^<>]*<\/mark>)/, "$1</mark>$2<mark>$4");

                            //$(this).html(src_str);
                            $(this).html(src_str + etiqueta);
                            //console.log(src_str + etiqueta);
                        });
                    }

                    

                    setTimeout(applyDot(wp, 3, "left"), 500);

                },
                error: function (data) {
                    console.log(data);
                }
            });
        }
    });
}


TablaDeDatos.SeleccionaTodo = function (idTBody, thisInput) {
    var bol = $(thisInput).prop("checked");
    $("input[type=checkbox]").prop("checked", false);
    $(thisInput).prop("checked", bol)
    if (bol) {
        $("#" + idTBody + " tr>td input").each(function () {
            $(this).prop('checked', true);
        });
    } else {
        $("#" + idTBody + " tr>td input").each(function () {
            $(this).prop('checked', false);
        });
    }
}

TablaDeDatos.ButtonAgregarFiltrosClick = function (urlPost, pIdioma) {
    $(".btnAddFilterAndSearch").click(function () {
        LoadingAdminPage.showOrHideLoadingPage(true);
        var vFiltro = $(this).data("filtro");
        var vIdTabla = $(this).data("id-tabla");

        var chkArray = [];
        $("#tbody" + vIdTabla + " tr>td input:checked").each(function () {
            chkArray.push($(this).val());
        });
        var vRegistrosSeleccionados = chkArray.join(",");

        chkArray = [];
        $("#tbody" + vIdTabla + " tr>td input").each(function () {
            chkArray.push($(this).val());
        });
        var vRegistrosPagina = chkArray.join(",");

        $.ajax({
            type: "POST",
            url: urlPost,
            data: {
                filtro: vFiltro,
                idTabla: vIdTabla,
                codPais: $("#cboPais").find("option:selected").val(),
                tipoOpe: $('input:radio[name=rdbTipoOpe]:checked').val(),
                codPais2: $("#cboPais2").find("option:selected").val(),
                anioMesIni: Fecha.getYearAndMonth($('#cboDesde').datepicker('getDate')),
                anioMesFin: Fecha.getYearAndMonth($('#cboHasta').datepicker('getDate')),
                indexCboPaisB: $("#cboPaisB").prop('selectedIndex'),
                codPaisB: $("#cboPaisB").find("option:selected").val(),
                idsSeleccionados: vRegistrosSeleccionados,
                idsPagina: vRegistrosPagina,
                numFiltrosExistentes: $("#lstFiltros option").length,
                idioma: pIdioma
            },
            beforeSend: function () {
                //LoadingAdminPage.showOrHideLoadingPage(true);
            },
            success: function (response) {
                if (!response.maximoLimiteFiltros) {

                    if (response.objMensaje == null) {
                        if (response.nuevosFiltros != null) {
                            Filtros.addFilterToFiltersList(response.nuevosFiltros);

                            CargarDatosDeTablas(response.gridData, response.totalRecordsFound, response.hideTabExcel, response.FlagRegMax);

                        } else {
                            LoadingAdminPage.showOrHideLoadingPage(false);
                        }
                    } else {
                        if (!response.existenFiltros) {
                            LoadingAdminPage.showOrHideLoadingPage(false);
                            ModalAdmin.showVentanaMensajeWithTitleAndMessage("ModalVentanaMensaje",
                                "messageTitle",
                                response.objMensaje.titulo,
                                "message",
                                response.objMensaje.mensaje,
                                "lnkContactenos",
                                response.objMensaje.flagContactenos);
                        } else {
                            $("#chkAll" + vIdTabla).prop('checked', false);
                            $("#tbody" + vIdTabla + " tr>td input").each(function() {
                                $(this).prop('checked', false);
                            });
                            Busqueda.scrollToHere();
                            LoadingAdminPage.showOrHideLoadingPage(false);
                        }

                    }
                } else {
                    $("#chkAll" + vIdTabla).prop('checked', false);
                    $("#tbody" + vIdTabla + " tr>td input").each(function () {
                        $(this).prop('checked', false);
                    });
                    Busqueda.scrollToHere();

                    LoadingAdminPage.showOrHideLoadingPage(false);
                    ModalAdmin.showVentanaMensajeWithTitleAndMessage("ModalVentanaMensaje",
                        "messageTitle",
                        response.objMensaje.titulo,
                        "message",
                        response.objMensaje.mensaje,
                        "lnkContactenos",
                        response.objMensaje.flagContactenos);

                }
                MensajesConsole();
            },
            error: function (dataError) {
                console.log(dataError);
                //LoadingAdminPage.showOrHideLoadingPage(false);
            }
        });
    });
}

TablaDeDatos.lnkOrdenarTablaClick = function (urlPost,
    urlVerRegistro,
    urlPageChangeVerRegistro,
    urlBuscarPorDuaVerRegistro,
    urlBuscarPorDesComercialVerRegistro,
    urlVerSentinel,
    ulrIrASentinel) {

    

    $(".lnkOrdenarTabla").on('click',
        function () {
            var vFiltro = $(this).data("filtro");
            var vTipoFiltro = $(this).data("tipofiltro");

            $.ajax({
                type: "POST",
                url: urlPost,
                data: {
                    filtro: vFiltro,
                    tipoFiltro: vTipoFiltro,
                    orden: $(this).data("orden"),
                    codPais: $("#cboPais").find("option:selected").val(),
                    tipoOpe: $('input:radio[name=rdbTipoOpe]:checked').val(),
                    indexCboPaisB: $("#cboPaisB").prop('selectedIndex'),
                    anioMesIni: Fecha.getYearAndMonth($('#cboDesde').datepicker('getDate')),
                    anioMesFin: Fecha.getYearAndMonth($('#cboHasta').datepicker('getDate')),
                    codPaisB: $("#cboPaisB").find("option:selected").val()
                },
                beforeSend: function () {
                    LoadingAdminPage.showOrHideLoadingPage(true);
                },
                success: function (response) {
                    console.log(vFiltro)
                    if (response.ordenarTabla) {
                        var wp = $("#tbody" + vTipoFiltro + vFiltro);
                        $("#tbody" + vTipoFiltro + vFiltro).html(response.filasOrdenas);
                        $('#paging' + vTipoFiltro + vFiltro).twbsPagination('showOnlyPage', 1);
                        if (vFiltro != "InfoTabla") {
                            VerRegistro.lnkVerRegistroClick(urlVerRegistro,
                                urlPageChangeVerRegistro,
                                urlBuscarPorDuaVerRegistro,
                                urlBuscarPorDesComercialVerRegistro,
                                "#tbody" + vTipoFiltro + vFiltro
                            );
                        }                        
                        TablaDeDatos.ClickColumnLnkSentinel(urlVerSentinel, ulrIrASentinel);
                        LoadingAdminPage.showOrHideLoadingPage(false);
                        setTimeout(applyDot(wp, (vTipoFiltro == "Tab" && vFiltro == "Partida" ? 2 : 1)), 500);
                    } else {
                        LoadingAdminPage.showOrHideLoadingPage(false);
                    }

                },
                error: function (dataError) {
                    //console.log(dataError);
                    LoadingAdminPage.showOrHideLoadingPage(false);
                }
            });
        });
}

TablaDeDatos.ButtonAgregarAFavoritosClick = function (urlPost, pIdioma) {
    $(".btnAddMyFavourites").on("click",
        function () {
            LoadingAdminPage.showOrHideLoadingPage(true);

            var idTbodyTabla = $(this).data("id-tabla");
            //var vFiltro = $(this).data("filtro");

            var chkArray = [];
            $("#tbody" + idTbodyTabla + " tr>td input:checked").each(function () {
                chkArray.push($(this).val());
            });
            var vRegistrosSeleccionados = chkArray.join(",");

            chkArray = [];
            $("#tbody" + idTbodyTabla + " tr>td input").each(function () {
                chkArray.push($(this).val());
            });
            var vRegistrosPagina = chkArray.join(",");

            $.ajax({
                type: "POST",
                url: urlPost,
                data: {
                    filtro: $(this).data("filtro"),
                    idTabla: idTbodyTabla,
                    codPais: $("#cboPais").find("option:selected").val(),
                    codPais2: $("#cboPais2").find("option:selected").val(),
                    tipoOpe: $('input:radio[name=rdbTipoOpe]:checked').val(),
                    idsSeleccionados: vRegistrosSeleccionados,
                    idsPagina: vRegistrosPagina,
                    idioma: pIdioma
                },
                beforeSend: function () {
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
                        $("#tituloAgregarFavoritos").text(response.tituloFavoritos);
                        $("#lblAgregarAFavorito").text(response.lblAgregarAFavorito);
                        $("#tablaAgregarFavoritos").html(response.favoritosAagregar);

                        $("#txtNuevoGrupo").val("");
                        $("#chkAgregarAFavorito").prop('checked', true);
                        $("#chkCrearGrupo").prop('checked', false);
                        $("#txtNuevoGrupo").attr("disabled", "disabled");

                        if (response.listaGrupos != null && response.listaGrupos.length  > 0) {
                            $("select#cboGrupos").html("");
                            $.each(response.listaGrupos,
                                function (index, val) {
                                    $("select#cboGrupos")
                                        .append("<option value=" + val.value + ">" + val.text + "</option>");
                                });
                            $("#chkAgregarAGrupo").removeAttr("disabled");
                        } else {
                 
                            $("#chkAgregarAGrupo").attr("disabled", "disabled");
                        }

                        $("#chkAgregarAGrupo").prop('checked', false);
                        $("#cboGrupos").attr("disabled", "disabled");
                        $("#lblEstadoGrupos").text("");
                        //console.log(response.flagCheckAlertas);
                        if (response.flagCheckAlertas) {
                            $("#chkAlerta").removeClass("no-display");
                        }
                        else {
                            $("#chkAlerta").addClass("no-display");
                        }
                        LoadingAdminPage.showOrHideLoadingPage(false);
                        ModalAdmin.registerShowByShowOption("ModalAgregarFavoritos", true);
                    }
                },
                error: function (dataError) {
                    console.log(dataError);
                    //LoadingAdminPage.showOrHideLoadingPage(false);
                }
            });
        });
}

TablaDeDatos.VerMensajePlanNoSentinel = function (mensaje) {
    ModalAdmin.showVentanaMensajeWithTitleAndMessage("ModalVentanaMensaje",
        "messageTitle",
        'Sentinel',
        "message",
        //'Su plan no permite ver la información crediticia de esta empresa',
        mensaje,
        "lnkContactenos",
        true);
}

TablaDeDatos.VerMensajePlanNoInformaColombia = function (mensaje) {
    ModalAdmin.showVentanaMensajeWithTitleAndMessage("ModalVentanaMensaje",
        "messageTitle",
        'Informa Colombia',
        "message",
        //'Su plan no permite ver la información crediticia de esta empresa',
        mensaje,
        "lnkContactenos",
        true);
}

TablaDeDatos.ClickColumnLnkSentinel = function (urlPost, urlIrASentinel) {
    $(document).off('click.myNamespace'); 
    $(document).on("click.myNamespace", ".lnkSentinel", ".lnkArancelesPartida",
        function () {
            var vNroDoc = $(this).data("idruc");
            $.ajax({
                type: "POST",
                url: urlPost,
                data: {
                    nroDoc: vNroDoc,
                    codPais: $("#cboPais").find("option:selected").val(),
                    tipoOpe: $('input:radio[name=rdbTipoOpe]:checked').val(),
                    fechaInicial: Fecha.getYearAndMonth($('#cboDesde').datepicker('getDate')),
                    fechaFin: Fecha.getYearAndMonth($('#cboHasta').datepicker('getDate'))
                },
                beforeSend: function () {
                    LoadingAdminPage.showOrHideLoadingPage(true);
                },
                success: function (response) {

                    $("#infoSentinel").html("");
                    $("#infoSentinel").html(response.infoSentinel);

                    VerInfoSentinel.RegisterClickLnkSentinel(urlIrASentinel);

                    LoadingAdminPage.showOrHideLoadingPage(false);

                    ModalAdmin.registerShowByShowOption("ModalInfoSentinel", true);

                },
                error: function (dataError) {
                    console.log(dataError);
                    //LoadingAdminPage.showOrHideLoadingPage(false);
                }
            });

        });
}

TablaDeDatos.ObtenerImportadorColombia = function (idImportacion) {
    $.ajax({
        type: "POST",
        url: '/es/mis-busquedas/ObtenerImportador',
        data: {
            codPais: $("#cboPais").find("option:selected").val(),
            idImportacion: idImportacion
        },
        beforeSend: function () {
            LoadingAdminPage.showOrHideLoadingPage(true);
        },
        success: function (response) {
            TablaDeDatos.ClickInformaColombia(response.idImportador);
        },
        error: function (dataError) {
            console.log(dataError);
        }
    });
};

TablaDeDatos.ClickInformaColombia = function (ruc) {
    $.ajax({
        type: "POST",
        url: '/es/mis-busquedas/VerInformaColombia',
        data: {
            ruc: ruc,
            codPais: $("#cboPais").find("option:selected").val(),
            tipoOpe: $('input:radio[name=rdbTipoOpe]:checked').val(),
            fechaInicial: Fecha.getYearAndMonth($('#cboDesde').datepicker('getDate')),
            fechaFin: Fecha.getYearAndMonth($('#cboHasta').datepicker('getDate'))
        },
        beforeSend: function () {
            LoadingAdminPage.showOrHideLoadingPage(true);
        },
        success: function (response) {

            $("#infoColombia").html("");
            $("#infoColombia").html(response.informaColombia);
            LoadingAdminPage.showOrHideLoadingPage(false);
            $("#ModalInfoColombia").on('shown.bs.modal', function () {
                mostrarGraficos();
            });
            ModalAdmin.registerShowByShowOption("ModalInfoColombia", true);
            

        },
        error: function (dataError) {
            console.log(dataError);
            //LoadingAdminPage.showOrHideLoadingPage(false);
        }
    });
};

TablaDeDatos.ClickFreeTrial = function () {
    //ModalAdmin.registerShowByShowOption("ModalRequestFreeTrial", true);
    $('#ModalRequestFreeTrial').modal({
        show: true,
        backdrop: 'static',
        keyboard: false
    });
}

TablaDeDatos.ClickArancelesPartida = function (descripcion) {

    var firstChar = descripcion.match('[a-zA-Z]');
    var index = descripcion.indexOf(firstChar);

    if (index > 0) {
        var cod_partida = descripcion.substring(0, descripcion.indexOf(' '));
    } else {
        var cod_partida = descripcion;
    }

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

TablaDeDatos.ClickColumnLnkVerDetalleExelById = function (urlPost) {
    $("a.lnkVerDetalleById").on("click",
        function () {
            //console.log("log - link");
            var vNroDoc = $(this).data("id");
            $.ajax({
                type: "POST",
                url: urlPost,
                data: {
                    tipoOpe: $('input:radio[name=rdbTipoOpe]:checked').val(),
                    codPais: $("#cboPais").find("option:selected").val(),
                    id: vNroDoc
                },
                beforeSend: function () {
                    LoadingAdminPage.showOrHideLoadingPage(true);
                },
                success: function (response) {

                    $("#infoDEResult").html("");
                    $("#infoDEResult").html(response.result);

                    //VerInfoSentinel.RegisterClickLnkSentinel(urlIrASentinel);

                    LoadingAdminPage.showOrHideLoadingPage(false);

                    ModalAdmin.registerShowByShowOption("ModalVerDetalleId", true);

                },
                error: function (dataError) {
                    console.log(dataError);
                    //LoadingAdminPage.showOrHideLoadingPage(false);
                }
        });
    });
}

function AgregarFavoritos() {
}

AgregarFavoritos.agregarAFavClick = function (urlPost, msjValidaChks, msjValidaCrearGrupo, pIdioma) {

    if (!$("#chkAgregarAFavorito").is(":checked") &&
        !$("#chkCrearGrupo").is(":checked") &&
        !$("#chkAgregarAGrupo").is(":checked") &&
        !$("#chkAgregarAlerta").is(":checked")) {
        $("#lblEstadoGrupos").text(msjValidaChks);
    } else {
        var vTxtNuevoGrupo = $("#txtNuevoGrupo").val();
        vTxtNuevoGrupo = vTxtNuevoGrupo.trim().toUpperCase();
        if ($("#chkCrearGrupo").is(":checked") && vTxtNuevoGrupo == "") {
            $("#lblEstadoGrupos").text(msjValidaCrearGrupo);
            $("#txtNuevoGrupo").removeAttr("disabled");
            $("#txtNuevoGrupo").focus();
        } else {

            LoadingAdminPage.showOrHideLoadingPage(true);

            var chkArray = [];
            var txtPartidaFavArray = [];
            var vIdCheck;
            $("#tbodyAgregarFavoritos tr>td input:checked").each(function () {
                vIdCheck = $(this).val();

                chkArray.push(vIdCheck);

                if ($("#txtPartidaFav" + vIdCheck).val() != "") {
                    txtPartidaFavArray.push(vIdCheck + "|" + $("#txtPartidaFav" + vIdCheck).val());
                }

            });
            var vRegistrosSeleccionados = chkArray.join(",");
            var vTextosPartidaFav = txtPartidaFavArray.join(",");

            if (vRegistrosSeleccionados.length > 0) {
                $.ajax({
                    type: "POST",
                    url: urlPost,
                    data: {
                        checkedAgregarAFavorito: $("#chkAgregarAFavorito").is(":checked"),
                        checkedCrearGrupo: $("#chkCrearGrupo").is(":checked"),
                        checkedAgregarAGrupo: $("#chkAgregarAGrupo").is(":checked"),
                        checkedAgregarAlerta: $("#chkAgregarAlerta").is(":checked"),
                        textNuevoGrupo: $("#txtNuevoGrupo").val(),
                        codPais: $("#cboPais").find("option:selected").val(),
                        codPais2: $("#cboPais2").find("option:selected").val(),
                        tipoOpe: $('input:radio[name=rdbTipoOpe]:checked').val(),
                        valueCboGrupos: $("#cboGrupos").find("option:selected").val(),
                        idsSeleccionados: vRegistrosSeleccionados,
                        textosPartidaFav: vTextosPartidaFav,
                        idioma: pIdioma
                    },
                    beforeSend: function () {
                    },
                    success: function (response) {
                        if (response.mensaje != "") {
                            LoadingAdminPage.showOrHideLoadingPage(false);
                            $("#lblEstadoGrupos").text(response.mensaje);
                        } else {
                            if (response.objMensaje != null) {
                                LoadingAdminPage.showOrHideLoadingPage(false);

                                if (response.idTabla != "") {
                                    $("#chkAll" + response.idTabla).prop('checked', false);
                                    $("#tbody" + response.idTabla + " tr>td input").each(function () {
                                        $(this).prop('checked', false);
                                    });
                                }
                                ModalAdmin.hide("ModalAgregarFavoritos");

                                ModalAdmin.showVentanaMensajeWithTitleAndMessage("ModalVentanaMensaje",
                                    "messageTitle",
                                    response.objMensaje.titulo,
                                    "message",
                                    response.objMensaje.mensaje,
                                    "lnkContactenos",
                                    response.objMensaje.flagContactenos);

                            }
                        }
                    },
                    error: function (dataError) {
                        console.log(dataError);
                        //LoadingAdminPage.showOrHideLoadingPage(false);
                    }
                });
            } else {
                LoadingAdminPage.showOrHideLoadingPage(false);
                $("#lblEstadoGrupos").text(msjValidaChks);
            }
        }
    }
}

AgregarFavoritos.chkCrearGrupoChange = function () {
    $("#chkCrearGrupo").change(function () {
        if ($(this).is(":checked")) {
            $("#txtNuevoGrupo").removeAttr("disabled");
            $("#chkAgregarAGrupo").prop('checked', false);
            $("#cboGrupos").attr("disabled", "disabled");
            $("#txtNuevoGrupo").focus();
        }
    });
}

AgregarFavoritos.chkAgregarAGrupoChange = function () {
    $("#chkAgregarAGrupo").change(function () {
        if ($(this).is(":checked")) {
            $("#chkCrearGrupo").prop('checked', false);
            $("#txtNuevoGrupo").val("");
            $("#txtNuevoGrupo").attr("disabled", "disabled");
            $("#cboGrupos").removeAttr("disabled");
            $("#cboGrupos").focus();
        }
    });
}


function VerRegistro() {
}

VerRegistro.lnkVerRegistroClick = function (urlPost,
    urlPageChangeTable,
    urlBuscarPorDua,
    urlBuscarPorDesComercial,
    tabla) {
    var variable = "a.lnkVerRegistros";

    if (tabla != "") {
        variable = tabla + " a.lnkVerRegistros";
    }

    $(variable).on("click",
        function () {
            var vFiltro = $(this).data("filtro");
            
            var vIdRegistro = $(this).data("idregistro");

            var listaFiltros = [];
            $("#lstFiltros option").each(function () {
                var vThis = $(this);
                if (vThis.length) {
                    listaFiltros.push(vThis.text().trim());
                }
            });
            var vListaFiltros = listaFiltros.join(" | ");
            
            $.ajax({
                type: "POST",
                url: urlPost,
                data: {
                    filtro: vFiltro,
                    idregistro: vIdRegistro,
                    tipoOpe: $('input:radio[name=rdbTipoOpe]:checked').val(),
                    codPais: $("#cboPais").find("option:selected").val(),
                    codPais2: $("#cboPais2").find("option:selected").val(),
                    anioMesIni: Fecha.getYearAndMonth($('#cboDesde').datepicker('getDate')),
                    anioMesFin: Fecha.getYearAndMonth($('#cboHasta').datepicker('getDate')),
                    indexCboPaisB: $("#cboPaisB").prop('selectedIndex'),
                    codPaisB: $("#cboPaisB").find("option:selected").val(),
                    filtros: vListaFiltros,
                    numFiltrosExistentes: $("#lstFiltros option").length
                },
                beforeSend: function () {
                    LoadingAdminPage.showOrHideLoadingPage(true);
                },
                success: function (response) {
                    if (response.excelHabilitado) {
                        $("#downloadFileVerRegistro").show();
                    } else {
                        $("#downloadFileVerRegistro").hide();
                    }
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
                        $("#titulo-verRegistro").html(response.lblFiltroSel);
                        $("#recordsFoundVerRegistro").text(response.lblRecordsFound);
                        
                        $("#textoInfoComplementario").html(response.textoInfoComplementario);
                        if (!response.maximoLimiteFiltros) {
                            if (response.lnkAgregarFiltroSelVisible) {
                                $("#lnkAgregarFiltroSel").removeClass("no-display");
                            } else {
                                $("#lnkAgregarFiltroSel").addClass("no-display");
                            }
                        } else {
                            $("#lnkAgregarFiltroSel").addClass("no-display");
                        }

                        if (response.cboDescargas2Visible) {
                            $("#cboDescargasVerRegistro").removeClass("no-display");
                            $("select#cboDescargasVerRegistro").html("");
                            $.each(response.optionsDescargas,
                                function (index, val) {
                                    $("select#cboDescargasVerRegistro")
                                        .append("<option value=" + val.value + ">" + val.text + "</option>");
                                });
                        } else {
                            $("#cboDescargasVerRegistro").addClass("no-display");
                        }

                        if (response.lnkAgregarSelAFavoritosVisible) {
                            $("#lnkAgregarSelAFavoritos").removeClass("no-display");
                            $("#lnkAgregarSelAFavoritos").text(response.lnkAgregarSelAFavoritos);
                            $("#lnkAgregarSelAFavoritos").val(response.idDescargaDefault);
                        } else {
                            $("#lnkAgregarSelAFavoritos").addClass("no-display");
                        }

                        $("#tablaVerRegistros").html("");
                        $("#tablaVerRegistros").html(response.tablaVerRegistro);


                        if (response.totalPages > 1) {
                            VerRegistro.TablePageIndexChanging(urlPageChangeTable, response.totalPages);
                        }
                        VerRegistro.RegistrarClickBuscarPorDua(urlBuscarPorDua, urlPageChangeTable);
                        VerRegistro.RegistrarClicBuscarPorDesComercial(urlBuscarPorDesComercial, urlPageChangeTable);
                        LoadingAdminPage.showOrHideLoadingPage(false);

                        ModalAdmin.registerShowByShowOption("ModalVerRegistro", true);
                    }

                },
                error: function (dataError) {
                    console.log(dataError);
                    //LoadingAdminPage.showOrHideLoadingPage(false);
                }
            });

        });
}

VerRegistro.TablePageIndexChanging = function (pUrlPageChangeTable, pTotalPages) {

    $('#pagingVerRegistros').twbsPagination({
        totalPages: pTotalPages,
        visiblePages: 10,
        hideOnlyOnePage: true,
        initiateStartPageClick: false,
        onPageClick: function (event, page) {
            $.ajax({
                type: "POST",
                url: pUrlPageChangeTable,
                data: {
                    pagina: page,
                    codPais: $("#cboPais").find("option:selected").val(),
                    codPais2: $("#cboPais2").find("option:selected").val(),
                    tipoOpe: $('input:radio[name=rdbTipoOpe]:checked').val()
                },
                beforeSend: function () {
                    LoadingAdminPage.showOrHideLoadingPage(true);
                },
                success: function (response) {
                    //$("#paingVerRegistros li").removeClass("active");
                    //$(vThis).parent().addClass("active");
                    //console.log("entro")
                    $("#tbodyVerRegistros").html(response.filasVerRegistro);
                    var vTxtDesComercial = $("#txtDesComercialBB").val();
                    if (vTxtDesComercial != "") {
                        $.each($("#tbodyVerRegistros p"), function () {
                            //console.log($(this).html())
                            //var arreglo = $(this).html().split("<a");
                            //var etiqueta = '<a ' + arreglo[1];
                            var term = vTxtDesComercial;
                            //console.log(term);
                            var src_str = $(this).html();
                            //var src_str = arreglo[0];
                            //var term = "mY text";
                            term = term.replace(/(\s+)/, "(<[^>]+>)*$1(<[^>]+>)*");
                            //console.log(term);
                            var pattern = new RegExp("(" + term + ")", "gi");

                            src_str = src_str.replace(pattern, "<mark>$1</mark>");
                            src_str = src_str.replace(/(<mark>[^<>]*)((<[^>]+>)+)([^<>]*<\/mark>)/, "$1</mark>$2<mark>$4");

                            $(this).html(src_str);
                            //$(this).html(src_str + etiqueta);
                            //console.log(src_str + etiqueta);
                        });
                    }

                    LoadingAdminPage.showOrHideLoadingPage(false);
                },
                error: function (data) {
                    console.log(data);
                }
            });
        }
    });



    //$('#pagingVerRegistros').on('click',
    //    'a',
    //    function (e) {
    //        //prevent action link normal functionality
    //        e.preventDefault();

    //        var vThis = this;

    //        if (!$(this).parent().hasClass("active")) {

    //            $.ajax({
    //                type: "POST",
    //                url: this.href,
    //                data: {
    //                    codPais: $("#cboPais").find("option:selected").val(),
    //                    codPais2: $("#cboPais2").find("option:selected").val(),
    //                    tipoOpe: $('input:radio[name=rdbTipoOpe]:checked').val(),
    //                    idioma: pIdioma
    //                },
    //                beforeSend: function () {
    //                    LoadingAdminPage.showOrHideLoadingPage(true);
    //                },
    //                success: function (response) {
    //                    $("#paingVerRegistros li").removeClass("active");
    //                    $(vThis).parent().addClass("active");

    //                    $("#tbodyVerRegistros").html(response.filasVerRegistro);

    //                    LoadingAdminPage.showOrHideLoadingPage(false);
    //                },
    //                error: function (data) {
    //                    console.log(data);
    //                }
    //            });

    //        }
    //    });
}

VerRegistro.RegistrarClickBuscarPorDua = function (urlBuscarPorDua,
    urlPageChangeTable) {
    $("#lnkBuscarDUA2 , #lnkRestablecerDUA2").click(function () {
        //console.log(this)
        var vIdRegistro = $(this).data("idregistro");

        if (vIdRegistro == undefined) {
            vIdRegistro = $("#cboPais").find("option:selected").val();
        }

        console.log(vIdRegistro)
        var vTxtDuaB = $("#txtDUAB").val();
        if (this == $("#lnkRestablecerDUA2")[0]) {
            vTxtDuaB = "";
            $("#txtDUAB").val("");
        }

        $.ajax({
            type: "POST",
            url: urlBuscarPorDua,
            data: {
                txtDuaB: vTxtDuaB,
                tipoOpe: $('input:radio[name=rdbTipoOpe]:checked').val(),
                codPais: vIdRegistro,
                codPais2: $("#cboPais2").find("option:selected").val(),
                anioMesIni: Fecha.getYearAndMonth($('#cboDesde').datepicker('getDate')),
                anioMesFin: Fecha.getYearAndMonth($('#cboHasta').datepicker('getDate')),
                indexCboPaisB: $("#cboPaisB").prop('selectedIndex'),
                codPaisB: $("#cboPaisB").find("option:selected").val(),
                codPaisRep: $("#cboPais").find("option:selected").val()
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
                    $("#lblResultadoDesComercial2").text("");
                    $("#txtDesComercialBB").val("");

                    $("#lblResultadoDUA2").text(response.resultadoDuaVerRegistro);
                    $("#tbodyVerRegistros").html(response.tablaVerRegistro);
                    if (response.totalPages > 1) {
                        $("#pagingVerRegistros").twbsPagination('destroy');
                        $("#divPagingVerRegistros").removeClass("no-display");
                        VerRegistro.TablePageIndexChanging(urlPageChangeTable, response.totalPages);
                    } else {
                        $("#pagingVerRegistros").twbsPagination('destroy');
                        $("#divPagingVerRegistros").addClass("no-display");
                    }
                    LoadingAdminPage.showOrHideLoadingPage(false);
                }
            },
            error: function (dataError) {
                console.log(dataError);
                //LoadingAdminPage.showOrHideLoadingPage(false);
            }
        });
    });
}

VerRegistro.RegistrarClicBuscarPorDesComercial = function (urlBuscaPorDesComercial, urlPageChangeTable) {
    $("#lnkBuscarDesComercial2, #lnkRestablecerDesComercial2").click(function () {
        var vTxtDesComercial = $("#txtDesComercialBB").val();
        if (this == $("#lnkRestablecerDesComercial2")[0]) {
            vTxtDesComercial = "";
            $("#txtDesComercialBB").val("");
        }

        $.ajax({
            type: "POST",
            url: urlBuscaPorDesComercial,
            data: {
                txtDesComercialBB: vTxtDesComercial,
                tipoOpe: $('input:radio[name=rdbTipoOpe]:checked').val(),
                codPais: $("#cboPais").find("option:selected").val(),
                codPais2: $("#cboPais2").find("option:selected").val(),
                anioMesIni: Fecha.getYearAndMonth($('#cboDesde').datepicker('getDate')),
                anioMesFin: Fecha.getYearAndMonth($('#cboHasta').datepicker('getDate')),
                indexCboPaisB: $("#cboPaisB").prop('selectedIndex'),
                codPaisB: $("#cboPaisB").find("option:selected").val(),
                codPaisRep: $("#cboPais").find("option:selected").val()
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
                    $("#lblResultadoDUA2").val("");
                    $("#txtDUAB").val("");

                    $("#lblResultadoDesComercial2").text(response.resultadoDesComercialVerRegistro);
                    $("#tbodyVerRegistros").html(response.tablaVerRegistro);

                    if (vTxtDesComercial != "") {
                        $.each($("#tbodyVerRegistros p"), function () {
                            //console.log($(this).html())
                            //var arreglo = $(this).html().split("<a");
                            //var etiqueta = '<a ' + arreglo[1];
                            var term = vTxtDesComercial;
                            //console.log(term);
                            var src_str = $(this).html();
                            //var src_str = arreglo[0];
                            //var term = "mY text";
                            term = term.replace(/(\s+)/, "(<[^>]+>)*$1(<[^>]+>)*");
                            //console.log(term);
                            var pattern = new RegExp("(" + term + ")", "gi");

                            src_str = src_str.replace(pattern, "<mark>$1</mark>");
                            src_str = src_str.replace(/(<mark>[^<>]*)((<[^>]+>)+)([^<>]*<\/mark>)/, "$1</mark>$2<mark>$4");

                            $(this).html(src_str);
                            //$(this).html(src_str + etiqueta);
                            //console.log(src_str + etiqueta);
                        });
                    }

                    if (response.totalPages > 1) {
                        $("#pagingVerRegistros").twbsPagination('destroy');
                        $("#divPagingVerRegistros").removeClass("no-display");
                        VerRegistro.TablePageIndexChanging(urlPageChangeTable, response.totalPages);
                    } else {
                        $("#pagingVerRegistros").twbsPagination('destroy');
                        $("#divPagingVerRegistros").addClass("no-display");
                    }
                    LoadingAdminPage.showOrHideLoadingPage(false);
                }
            },
            error: function (dataError) {
                console.log(dataError);
                //LoadingAdminPage.showOrHideLoadingPage(false);
            }
        });
    });
}

VerRegistro.AgregarFiltroSeleccionado = function (urlAgregarFiltroSeleccionado) {
    $.ajax({
        type: "POST",
        url: urlAgregarFiltroSeleccionado,
        data: {
            codPais: $("#cboPais").find("option:selected").val(),
            codPais2: $("#cboPais2").find("option:selected").val(),
            tipoOpe: $('input:radio[name=rdbTipoOpe]:checked').val(),
            anioMesIni: Fecha.getYearAndMonth($('#cboDesde').datepicker('getDate')),
            anioMesFin: Fecha.getYearAndMonth($('#cboHasta').datepicker('getDate')),
            indexCboPaisB: $("#cboPaisB").prop('selectedIndex'),
            codPaisB: $("#cboPaisB").find("option:selected").val()
        },
        beforeSend: function () {
            LoadingAdminPage.showOrHideLoadingPage(true);
        },
        success: function (response) {
            if (response.nuevoFiltro) {
                Filtros.addFilterToFiltersList(response.nuevosFiltros);
                CargarDatosDeTablas(response.gridData, response.totalRecordsFound, response.hideTabExcel, response.FlagRegMax);
                ModalAdmin.hide("ModalVerRegistro");
            } else {
                LoadingAdminPage.showOrHideLoadingPage(false);
                ModalAdmin.hide("ModalVerRegistro");
            }
            MensajesConsole();
        },
        error: function (dataError) {
            console.log(dataError);
            //LoadingAdminPage.showOrHideLoadingPage(false);
        }
    });
}

VerRegistro.AgregarFiltrosSeleccionadoAFavoritos = function (urlAgregarFiltroSeleccioandoAfavoritos) {
    $.ajax({
        type: "POST",
        url: urlAgregarFiltroSeleccioandoAfavoritos,
        data: {
            codPais: $("#cboPais").find("option:selected").val(),
            codPais2: $("#cboPais2").find("option:selected").val(),
            tipoOpe: $('input:radio[name=rdbTipoOpe]:checked').val()
        },
        beforeSend: function () {
            LoadingAdminPage.showOrHideLoadingPage(true);
        },
        success: function (response) {
            $("#tituloAgregarFavoritos").text(response.tituloFavoritos);
            $("#lblAgregarAFavorito").text(response.lblAgregarAFavorito);
            $("#tablaAgregarFavoritos").html(response.favoritosAagregar);

            $("#txtNuevoGrupo").val("");
            $("#chkAgregarAFavorito").prop('checked', true);
            $("#chkCrearGrupo").prop('checked', false);
            $("#txtNuevoGrupo").attr("disabled", "disabled");

            if (response.listaGrupos != null) {
                $("select#cboGrupos").html("");
                $.each(response.listaGrupos,
                    function (index, val) {
                        $("select#cboGrupos")
                            .append("<option value=" + val.value + ">" + val.text + "</option>");
                    });
                $("#chkAgregarAGrupo").removeAttr("disabled");
            } else {
                $("#chkAgregarAGrupo").attr("disabled", "disabled");
            }

            $("#chkAgregarAGrupo").prop('checked', false);
            $("#cboGrupos").attr("disabled", "disabled");
            $("#lblEstadoGrupos").text("");

            ModalAdmin.hide("ModalVerRegistro");
            LoadingAdminPage.showOrHideLoadingPage(false);
            ModalAdmin.registerShowByShowOption("ModalAgregarFavoritos", true);
        },
        error: function (dataError) {
            console.log(dataError);
            //LoadingAdminPage.showOrHideLoadingPage(false);
        }
    });
}

function VerInfoSentinel() {
}
VerInfoSentinel.RegisterClickLnkSentinel = function (urlIrASentinel) {
    $("#lnkSentinel, #lnkContactoSentienl").click(function (e) {
        e.preventDefault();
        var win = window.open(this.href);
        if (win) {
            win.focus();
            $.ajax({
                type: "POST",
                url: urlIrASentinel,
                data: {},
                beforeSend: function () {
                },
                success: function (response) {
                    //console.log(response.respuesta);
                },
                error: function (dataError) {
                    console.log(dataError);
                    //LoadingAdminPage.showOrHideLoadingPage(false);
                }
            });
        } else {
            alert("Por favor permita las ventanas emergentes para este sitio web");
        }
    });
}

function ColumnAdmin() {
}

ColumnAdmin.loadData = function (idChart, titlePie, dataPie,
    urlVerRegistros,
    urlPageChangeTable,
    urlBuscarPorDua,
    urlBuscarPorDesComercial,
    filtro,
    objChartData) {
    titlePie = titlePie == undefined ? filtro : titlePie;
    //console.log(objChartData)
    Highcharts.chart(idChart, {
        title: {
            text: titlePie,
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
            //tickInterval: 10000,
            title: {
                text: ''
            },
            labels: {
                formatter: function () {
                    return Highcharts.numberFormat(this.value, 0, '.', ',')//this.value;
                }
            }
        },
        tooltip: {
            //headerFormat: '',
            //pointFormat: '<b>{series.name}:</b> $ {point.y:,.0f}'
            formatter: function () {
                var yVal = this.y;
                return "<b>" + this.x + '</b>: ' + "" + Highcharts.numberFormat(this.point.y, 0, '.', ','); //  yVal.toLocaleString('en');
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
                            //console.log(this)
                            if (this.id != "") {
                                ColumnAdmin.clickSlice(urlVerRegistros,
                                    urlPageChangeTable,
                                    urlBuscarPorDua,
                                    urlBuscarPorDesComercial,
                                    filtro,
                                    this.id);

                                // JANAQ Clic Grafico 200720
                                if (typeof MixPanel.clickGrafico == 'function') {
                                    MixPanel.clickGrafico(idChart);
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
                data: objChartData.Column[0].data
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
                                MixPanel.exportarGrafico(gOptionGrafico.downloadPNG, idChart);
                            }
                            this.exportChart();
                        }
                        },
                        'separator',
                        {
                        text: gLangOptionHighcharts.printChart,
                        onclick: function () {

                            if (typeof MixPanel.exportarGrafico == 'function') {
                                MixPanel.exportarGrafico(gOptionGrafico.printChart, idChart);
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

ColumnAdmin.clickSlice = function (urlVerRegistros,
    urlPageChangeTable,
    urlBuscarPorDua,
    urlBuscarPorDesComercial,
    pFiltro,
    idPoint) {

    var listaFiltros = [];
    $("#lstFiltros option").each(function () {
        var vThis = $(this);
        if (vThis.length) {
            //listaFiltros.push(vThis.text().trim());
            var pos = vThis.text().trim().indexOf("] ")
            var division1 = vThis.text().trim().substring(0, pos + 2);
            var division2 = vThis.text().trim().substring(pos + 2);
            var segundo = division2;
            var posicion = segundo.indexOf(" ");
            var dev1 = segundo.substring(0, posicion)
            var dev2 = segundo.substring(posicion)

            if (dev1.length > 6) {
                dev1 = dev1.substring(0, 6);
            }

            var union = division1 + dev1 + dev2;

            //listaFiltros.push(vThis.text().trim());
            listaFiltros.push(union);
        }
    });
    var vListaFiltros = listaFiltros.join(" | ");

    $.ajax({
        type: "POST",
        url: urlVerRegistros,
        data: {
            filtro: pFiltro,
            idregistro: idPoint,
            tipoOpe: $('input:radio[name=rdbTipoOpe]:checked').val(),
            codPais: $("#cboPais").find("option:selected").val(),
            codPais2: $("#cboPais2").find("option:selected").val(),
            anioMesIni: Fecha.getYearAndMonth($('#cboDesde').datepicker('getDate')),
            anioMesFin: Fecha.getYearAndMonth($('#cboHasta').datepicker('getDate')),
            indexCboPaisB: $("#cboPaisB").prop('selectedIndex'),
            codPaisB: $("#cboPaisB").find("option:selected").val(),
            filtros: vListaFiltros,
            codPais3: $("#cboPais").find("option:selected").val(),
            numFiltrosExistentes: $("#lstFiltros option").length
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
                $("#titulo-verRegistro").html(response.lblFiltroSel);
                $("#recordsFoundVerRegistro").text(response.lblRecordsFound);
                $("#textoInfoComplementario").html(response.textoInfoComplementario);
                if (response.lnkAgregarFiltroSelVisible) {
                    $("#lnkAgregarFiltroSel").removeClass("no-display");
                } else {
                    $("#lnkAgregarFiltroSel").addClass("no-display");
                }

                if (response.cboDescargas2Visible) {
                    $("#cboDescargasVerRegistro").removeClass("no-display");
                    $("select#cboDescargasVerRegistro").html("");
                    $.each(response.optionsDescargas,
                        function (index, val) {
                            $("select#cboDescargasVerRegistro")
                                .append("<option value=" + val.value + ">" + val.text + "</option>");
                        });
                } else {
                    $("#cboDescargasVerRegistro").addClass("no-display");
                }

                if (response.lnkAgregarSelAFavoritosVisible) {
                    $("#lnkAgregarSelAFavoritos").removeClass("no-display");
                    $("#lnkAgregarSelAFavoritos").text(response.lnkAgregarSelAFavoritos);
                    $("#lnkAgregarSelAFavoritos").val(response.idDescargaDefault);
                } else {
                    $("#lnkAgregarSelAFavoritos").addClass("no-display");
                }

                $("#tablaVerRegistros").html("");
                $("#tablaVerRegistros").html(response.tablaVerRegistro);


                if (response.totalPages > 1) {
                    VerRegistro.TablePageIndexChanging(urlPageChangeTable, response.totalPages);
                }
                VerRegistro.RegistrarClickBuscarPorDua(urlBuscarPorDua, urlPageChangeTable);
                VerRegistro.RegistrarClicBuscarPorDesComercial(urlBuscarPorDesComercial, urlPageChangeTable);
                LoadingAdminPage.showOrHideLoadingPage(false);

                ModalAdmin.registerShowByShowOption("ModalVerRegistro", true);
            }

        },
        error: function (dataError) {
            console.log(dataError);
            //LoadingAdminPage.showOrHideLoadingPage(false);
        }
    });

}

ColumnAdmin.loadCboThemes = function (idCbo) {
    $.each(dbThemes,
        function (index, val) {
            $("#" + idCbo).append("<option value=" + val.id + ">" + val.name + "</option>");
        });
}

ColumnAdmin.changeCboThemes = function (urlVerRegistros,
    urlPageChangeVerRegistro,
    urlBuscarPorDuaVerRegistro,
    urlBuscarPorDesComercialVerRegistro) {
    //var browser = AdminPage.getBrowserInfo().toLowerCase();
    //console.log("el browser es: " + browser);
    $(document).on('change',
        '.select-themes',
        function () {
            var vFiltro = $(this).data("filtro");
            var idTheme = $(this).val();

            var valueTheme = altFind(dbThemes, function (e) {
                return e.id == idTheme;
            }).value;
            //var valueTheme = dbThemes.find(x => x.id == idTheme).value;
            //console.log(valueTheme); 


            Highcharts.setOptions(valueTheme);
            var chartTitle = $("#" + "pieChart" + vFiltro).highcharts().options.title.text;

            //console.log(gArrayPiesData);

            var dataPie = altFind(gArrayPiesData, function (e) {
                return e.filtro == vFiltro;
            }).value;
            //console.log(dataPie); 
            //var dataPie = gArrayPiesData.find(y => y.filtro == vFiltro).value;
            //var dataPie = gArrayChartData.find(y => y.filtro == vFiltro).value;

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

            ColumnAdmin.loadData("pieChart" + vFiltro,
                chartTitle,
                dataPie,
                urlVerRegistros,
                urlPageChangeVerRegistro,
                urlBuscarPorDuaVerRegistro,
                urlBuscarPorDesComercialVerRegistro,
                vFiltro);
        });
}

function PieAdmin() {
}

PieAdmin.loadData = function (idChart, titlePie, dataPie,
    urlVerRegistros,
    urlPageChangeTable,
    urlBuscarPorDua,
    urlBuscarPorDesComercial,
    filtro) {
    titlePie = titlePie == undefined ? filtro : titlePie;
    
    Highcharts.chart(idChart, {
        chart: {
            plotBackgroundColor: null,
            plotBorderWidth: null,
            plotShadow: false,
            type:'pie'
        },
        title: {
            text: titlePie
        },
        tooltip: {
            headerFormat:'',
            pointFormat: '{point.custom}: <b>{point.percentage:.1f}%</b>',
            //enabled: false
        },
        plotOptions: {
            series: {
                allowPointSelect: true
            },
            pie: {
                dataLabels: {
                    enabled: true,
                    format: '<b>{point.name}</b>: {point.percentage:.1f} %',
                    style: {
                        color: (Highcharts.theme && Highcharts.theme.contrastTextColor) || 'black'
                    }
                },
                cursor: "pointer",
                point: {
                    events: {
                        click: function () {
                            if (this.id != "") {
                                PieAdmin.clickSlice(urlVerRegistros,
                                    urlPageChangeTable,
                                    urlBuscarPorDua,
                                    urlBuscarPorDesComercial,
                                    filtro,
                                    this.id);

                                // JANAQ Clic Grafico 200720
                                if (typeof MixPanel.clickGrafico == 'function') {
                                    MixPanel.clickGrafico(idChart);
                                }
                            }
                        }
                    }
                }
            }
        },
        series: [{
            //name: '',
            colorByPoint: true,
            data: dataPie
        }],
        exporting: {
            buttons: {
                contextButton: {
                      // JANAQ Clic Exportar/Imprimir Grafico 190720
                    menuItems: [{
                        text: gLangOptionHighcharts.downloadPNG,
                        onclick: function () {

                            if (typeof MixPanel.exportarGrafico == 'function') {
                                MixPanel.exportarGrafico(gOptionGrafico.downloadPNG, idChart);
                            }
                            this.exportChart();
                        }
                        },
                        'separator',
                        {
                        text: gLangOptionHighcharts.printChart,
                            onclick: function () {

                            if (typeof MixPanel.exportarGrafico == 'function') {
                                MixPanel.exportarGrafico(gOptionGrafico.printChart, idChart);
                            }
                            this.print();
                        }
                        }],
                    symbol: 'download',
                    symbolFill:'#54b6e8',
                    _titleKey: "exportMenuButtonTitle"
                }
            }
        }
    });
}

PieAdmin.clickSlice = function (urlVerRegistros,
    urlPageChangeTable,
    urlBuscarPorDua,
    urlBuscarPorDesComercial,
    pFiltro,
    idPoint) {
    $.ajax({
        type: "POST",
        url: urlVerRegistros,
        data: {
            filtro: pFiltro,
            idregistro: idPoint,
            tipoOpe: $('input:radio[name=rdbTipoOpe]:checked').val(),
            codPais: $("#cboPais").find("option:selected").val(),
            codPais2: $("#cboPais2").find("option:selected").val(),
            anioMesIni: Fecha.getYearAndMonth($('#cboDesde').datepicker('getDate')),
            anioMesFin: Fecha.getYearAndMonth($('#cboHasta').datepicker('getDate')),
            indexCboPaisB: $("#cboPaisB").prop('selectedIndex'),
            codPaisB: $("#cboPaisB").find("option:selected").val(),
            numFiltrosExistentes: $("#lstFiltros option").length
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
                $("#titulo-verRegistro").html(response.lblFiltroSel);
                $("#recordsFoundVerRegistro").text(response.lblRecordsFound);
                if (response.lnkAgregarFiltroSelVisible) {
                    $("#lnkAgregarFiltroSel").removeClass("no-display");
                } else {
                    $("#lnkAgregarFiltroSel").addClass("no-display");
                }

                if (response.cboDescargas2Visible) {
                    $("#cboDescargasVerRegistro").removeClass("no-display");
                    $("select#cboDescargasVerRegistro").html("");
                    $.each(response.optionsDescargas,
                        function (index, val) {
                            $("select#cboDescargasVerRegistro")
                                .append("<option value=" + val.value + ">" + val.text + "</option>");
                        });
                } else {
                    $("#cboDescargasVerRegistro").addClass("no-display");
                }

                if (response.lnkAgregarSelAFavoritosVisible) {
                    $("#lnkAgregarSelAFavoritos").removeClass("no-display");
                    $("#lnkAgregarSelAFavoritos").text(response.lnkAgregarSelAFavoritos);
                    $("#lnkAgregarSelAFavoritos").val(response.idDescargaDefault);
                } else {
                    $("#lnkAgregarSelAFavoritos").addClass("no-display");
                }

                $("#tablaVerRegistros").html("");
                $("#tablaVerRegistros").html(response.tablaVerRegistro);


                if (response.totalPages > 1) {
                    VerRegistro.TablePageIndexChanging(urlPageChangeTable, response.totalPages);
                }
                VerRegistro.RegistrarClickBuscarPorDua(urlBuscarPorDua, urlPageChangeTable);
                VerRegistro.RegistrarClicBuscarPorDesComercial(urlBuscarPorDesComercial, urlPageChangeTable);
                LoadingAdminPage.showOrHideLoadingPage(false);

                ModalAdmin.registerShowByShowOption("ModalVerRegistro", true);
            }

        },
        error: function (dataError) {
            console.log(dataError);
            //LoadingAdminPage.showOrHideLoadingPage(false);
        }
    });

}

PieAdmin.loadCboThemes = function (idCbo) {
    $.each(dbThemes,
        function (index, val) {
            $("#" + idCbo).append("<option value=" + val.id + ">" + val.name + "</option>");
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

PieAdmin.changeCboThemes = function(urlVerRegistros,
    urlPageChangeVerRegistro,
    urlBuscarPorDuaVerRegistro,
    urlBuscarPorDesComercialVerRegistro) {
    //var browser = AdminPage.getBrowserInfo().toLowerCase();
    //console.log("el browser es: " + browser);
    $(document).on('change',
        '.select-themes',
        function () {
            var vFiltro = $(this).data("filtro");
            //console.log(vFiltro)
            var idTheme = $(this).val();
     
            var valueTheme = altFind(dbThemes, function (e) {
                return e.id == idTheme;
            }).value;
            //var valueTheme = dbThemes.find(x => x.id == idTheme).value;
            //console.log(valueTheme); 
            

            Highcharts.setOptions(valueTheme);
            var chartTitle = $("#" + "pieChart" + vFiltro).highcharts().options.title.text;

            //console.log(gArrayPiesData);

            var dataPie = altFind(gArrayPiesData, function (e) {
                return e.filtro == vFiltro;
            }).value;
            //console.log(dataPie); 
            //var dataPie = gArrayPiesData.find(y => y.filtro == vFiltro).value;
            //var dataPie = gArrayChartData.find(y => y.filtro == vFiltro).value;

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

            if (vFiltro == "InfoTabla") {
                var chartData = altFind(gArrayPiesData, function (e) {
                    return e.filtro == vFiltro;
                }).value;
                ColumnAdmin.loadData("pieChart" + vFiltro,
                    chartTitle,
                    dataPie,
                    urlVerRegistros,
                    urlPageChangeVerRegistro,
                    urlBuscarPorDuaVerRegistro,
                    urlBuscarPorDesComercialVerRegistro,
                    vFiltro,
                    chartData);
            } else {
                PieAdmin.loadData("pieChart" + vFiltro,
                    chartTitle,
                    dataPie,
                    urlVerRegistros,
                    urlPageChangeVerRegistro,
                    urlBuscarPorDuaVerRegistro,
                    urlBuscarPorDesComercialVerRegistro,
                    vFiltro);
            }

            
        });
}

function inicioBusqueda() {
    var codPais = $("#cboPais").find("option:selected").val();
    if (codPais == "MXD") {
        TableInfoCountry.prototype.showModalInfoMexicoDetalleFull();
    }
    //if (codPais == "MXM") {
    //    TableInfoCountry.prototype.showModalInfoMexicoDetalleMaritimo();
    //}
}

inicioBusqueda();
