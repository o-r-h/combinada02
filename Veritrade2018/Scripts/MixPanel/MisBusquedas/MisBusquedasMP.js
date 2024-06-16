/* JANAQ 040620
 * Eventos capturados con mixpanel en la seccion mis busquedas
*/

function MixPanel() { }

$(document).ready(function () {

    //Si se ingreso por url de referido (validacion para evitar trackear enlaces de universidades)
    if (getCookie("VLU") == "1") return;

    // Filtros
    var fieldNamePaisOrigen = "País Origen";
    var fieldNamePaisDestino = "País Destino";
    var fieldNamePartidaAduanera = "Partida";
    var fieldNameImportador = "Importador";
    var fieldNameExportador = "Exportador";
    var fieldNameMarca = "Marca";
    var fieldNameDescComercial = "Desc. Comercial";

    //Tipo de Actividad
    var tipoActividadImportaciones = 'I';
    var tipoActividadExportaciones = 'E';

    //Tabs
    var tabDetalleExcel = "Detalle - Excel";

    var globalTabSel = "Resumen";

    // Evento seleccionar tipo de actividad
    $(".content-form-mis-busquedas .radio-inline.control-label-withoutColor-vt").on("change", function () {
         mixpanel.track("Clic Tipo de Actividad",
            {
                "Sección": "Mis Busquedas",
                "Tipo de Actividad": $(this)[0].innerText
            });
    });

    // Evento seleccionar region
    $(".content-form-mis-busquedas #cboPais2").on("change", function () {
        mixpanel.track("Seleccionar Region",
            {
                "Sección": "Mis Busquedas",
                "Región Seleccionada": $(this).children("option:selected").text()
            });
    });

    // Evento seleccionar pais
    $(".content-form-mis-busquedas #cboPais").on("change", function () {
        mixpanel.track("Seleccionar Pais",
            {
                "Sección": "Mis Busquedas",
                "Región Seleccionada": $(".content-form-mis-busquedas #cboPais2").children("option:selected").text(),
                "País Seleccionado": $(this).children("option:selected").text()
            });
    });

    // Evento seleccionar pais origen o pais destino
    $(document).on("change", ".content-form-mis-busquedas #cboPaisB", function () {
        var tipoActividadSel = $(".content-form-mis-busquedas .radio-inline.control-label-withoutColor-vt :checked").val();
        if (tipoActividadSel == tipoActividadImportaciones) {
            mixpanel.track("Seleccionar País Origen",
            {
                "Sección": "Mis Busquedas",
                "Tipo de Actividad": $(".content-form-mis-busquedas .radio-inline.control-label-withoutColor-vt :checked").parent()[0].innerText,
                "País Origen Seleccionado": $(this).children("option:selected").text()
            });
        }else{
            mixpanel.track("Seleccionar País Destino",
            {
                "Sección": "Mis Busquedas",
                "Tipo de Actividad": $(".content-form-mis-busquedas .radio-inline.control-label-withoutColor-vt :checked").parent()[0].innerText,
                "País Destino Seleccionado": $(this).children("option:selected").text()
            });
        }
    });

    // Evento seleccionar partida aduanera
    MixPanel.filtroPartidaAduanera = function () {
        $("#txtNandinaB").autocomplete("widget").delegate('li', 'click', function () {
            mixpanel.track("Seleccionar Partida Aduanera",
                {
                    "Sección": "Mis Busquedas",
                    "Partida Aduanera Ingresada": $(this)[0].textContent
                });
        });
    }

    // Evento seleccionar importador
    MixPanel.filtroImportador = function () {
        $("#txtImportadorB, #txtImportadorExpB").autocomplete("widget").delegate('li', 'click', function () {
                    mixpanel.track("Seleccionar Importador",
                        {
                            "Sección": "Mis Busquedas",
                            "Tipo de Actividad": $(".content-form-mis-busquedas .radio-inline.control-label-withoutColor-vt :checked").parent()[0].innerText,
                            "Región": $(".content-form-mis-busquedas #cboPais2").children("option:selected").text(),
                            "País": $(".content-form-mis-busquedas #cboPais").children("option:selected").text(),
                            "Importador": $(this)[0].textContent
                        });
        });
    }

    // Evento seleccionar exportador
    MixPanel.filtroExportador = function () {
        $("#txtExportadorB").autocomplete("widget").delegate('li', 'click', function () {
                mixpanel.track("Seleccionar Exportador",
                    {
                        "Sección": "Mis Busquedas",
                        "Tipo de Actividad": $(".content-form-mis-busquedas .radio-inline.control-label-withoutColor-vt :checked").parent()[0].innerText,
                        "Región": $(".content-form-mis-busquedas #cboPais2").children("option:selected").text(),
                        "País": $(".content-form-mis-busquedas #cboPais").children("option:selected").text(),
                        "Exportador": $(this)[0].textContent
                    });
        });
    }

    // Evento seleccionar marca - exportador
    MixPanel.filtroMarcaExportador = function () {
        var tipoActividadSel = $(".content-form-mis-busquedas .radio-inline.control-label-withoutColor-vt :checked").val();
            $("#txtProveedorB").autocomplete("widget").delegate('li', 'click', function () {
                    var codePaisSel = $(".content-form-mis-busquedas #cboPais").children("option:selected").val();
                    var codePaisChile = "CL";

                    if (codePaisSel == codePaisChile && tipoActividadSel == tipoActividadImportaciones) {
                        mixpanel.track("Seleccionar Marca",
                            {
                                "Sección": "Mis Busquedas",
                                "Tipo de Actividad": $(".content-form-mis-busquedas .radio-inline.control-label-withoutColor-vt :checked").parent()[0].innerText,
                                "Región": $(".content-form-mis-busquedas #cboPais2").children("option:selected").text(),
                                "País": $(".content-form-mis-busquedas #cboPais").children("option:selected").text(),
                                "Marca": $(this)[0].textContent
                            });
                    } else {
                        mixpanel.track("Seleccionar Exportador",
                            {
                                "Sección": "Mis Busquedas",
                                "Tipo de Actividad": $(".content-form-mis-busquedas .radio-inline.control-label-withoutColor-vt :checked").parent()[0].innerText,
                                "Región": $(".content-form-mis-busquedas #cboPais2").children("option:selected").text(),
                                "País": $(".content-form-mis-busquedas #cboPais").children("option:selected").text(),
                                "Exportador": $(this)[0].textContent
                            });
                    }
            });
    }

    // Evento clic buscar
    $("#btnBuscar").click(function () {
        var diffMonths = getDiffMonths($("input[name='cboDesde']").val(), $("input[name='cboHasta']").val());
        mixpanel.track("Clic Buscar",
            {
                "Sección": "Mis Busquedas",
                "Tipo de Actividad": $(".content-form-mis-busquedas .radio-inline.control-label-withoutColor-vt :checked").parent()[0].innerText,
                "Región": $(".content-form-mis-busquedas #cboPais2").children("option:selected").text(),
                "País": $(".content-form-mis-busquedas #cboPais").children("option:selected").text(),
                "Filtros Configurados": getContentSelect("#lstFiltros"),
                "Fecha Ini": getFtYearMonth($("input[name='cboDesde']").val()),
                "Fecha Fin": getFtYearMonth($("input[name='cboHasta']").val()),
                "Intervalo Meses": diffMonths,
                "Rango de fechas": getRangeDescription(diffMonths)
            });
    });

    // Evento clic video tutorial
    $(document).on("click", "button.btn.btn-video-vt.blink-button", function () {
        mixpanel.track("Clic Video Tutorial",
            {
                "Sección": "Mis Busquedas"
            });
    });

    // Evento clic agregar filtros
    $(document).on("click", "#btnAgregarDesComercial", function () {
        mixpanel.track("Clic Agregar Filtros",
            {
                "Sección": "Mis Busquedas",
                "Descripción Comercial": $("#txtDesComercialB").val()
            });
    });

    // Evento clic tab
    $(document).on("click", "#tabsMisBusquedas li.ui-tabs-tab.ui-corner-top.ui-state-default.ui-tab", function () {
        var tipoActividadSel = $(".content-form-mis-busquedas .radio-inline.control-label-withoutColor-vt :checked").val();
        globalTabSel = $(this)[0].innerText;
        if (tipoActividadSel == tipoActividadImportaciones) {
            mixpanel.track("Clic Tab",
                {
                    "Sección": "Mis Busquedas",
                    "Tab Seleccionado": $(this)[0].innerText,
                    "Tipo de Actividad": $(".content-form-mis-busquedas .radio-inline.control-label-withoutColor-vt :checked").parent()[0].innerText,
                    "Región": $(".content-form-mis-busquedas #cboPais2").children("option:selected").text(),
                    "País": $(".content-form-mis-busquedas #cboPais").children("option:selected").text(),
                    "Importador": getValueFilterFromFilters("#lstFiltros", fieldNameImportador),
                    "Exportador": getValueFilterFromFilters("#lstFiltros", fieldNameExportador),
                    "Marca": getValueFilterFromFilters("#lstFiltros", fieldNameMarca),
                    "Partida Aduanera": getValueFilterFromFilters("#lstFiltros", fieldNamePartidaAduanera),
                    "Descripción Comercial": getValueFilterFromFilters("#lstFiltros", fieldNameDescComercial),
                    "País Origen": getValueFilterFromFilters("#lstFiltros", fieldNamePaisOrigen),
                    "Fecha Ini": getFtYearMonth($("input[name='cboDesde']").val()),
                    "Fecha Fin": getFtYearMonth($("input[name='cboHasta']").val()),
                });
        } else {
            mixpanel.track("Clic Tab",
                {
                    "Sección": "Mis Busquedas",
                    "Tab Seleccionado": $(this)[0].innerText,
                    "Tipo de Actividad": $(".content-form-mis-busquedas .radio-inline.control-label-withoutColor-vt :checked").parent()[0].innerText,
                    "Región": $(".content-form-mis-busquedas #cboPais2").children("option:selected").text(),
                    "País": $(".content-form-mis-busquedas #cboPais").children("option:selected").text(),
                    "Importador": getValueFilterFromFilters("#lstFiltros", fieldNameImportador),
                    "Exportador": getValueFilterFromFilters("#lstFiltros", fieldNameExportador),
                    "Marca": getValueFilterFromFilters("#lstFiltros", fieldNameMarca),
                    "Partida Aduanera": getValueFilterFromFilters("#lstFiltros", fieldNamePartidaAduanera),
                    "Descripción Comercial": getValueFilterFromFilters("#lstFiltros", fieldNameDescComercial),
                    "País Destino": getValueFilterFromFilters("#lstFiltros", fieldNamePaisDestino),
                    "Fecha Ini": getFtYearMonth($("input[name='cboDesde']").val()),
                    "Fecha Fin": getFtYearMonth($("input[name='cboHasta']").val()),
                });
        }
    });

    // Evento clic ver detalle producto
    $(document).on("click", "#tbodyResumenPartida img.cursor-action,#tbodyTabPartida img.cursor-action", function () {
        var tipoActividadSel = $(".content-form-mis-busquedas .radio-inline.control-label-withoutColor-vt :checked").val();
        var nomTabSel = $("li.ui-tabs-tab.ui-corner-top.ui-state-default.ui-tab.ui-tabs-active.ui-state-active")[0].innerText;

        if (tipoActividadSel == tipoActividadImportaciones) {
            //mixpanel.track("Clic Ver Detalle de Producto",
            mixpanel.track("Detalle Partida Aduanera",
                {
                    "Sección": "Mis Busquedas",
                    "Tab": nomTabSel,
                    "Identificador del Producto": $(this).parent().parent()[0].innerText,
                    "Tipo de Actividad": $(".content-form-mis-busquedas .radio-inline.control-label-withoutColor-vt :checked").parent()[0].innerText,
                    "Región": $(".content-form-mis-busquedas #cboPais2").children("option:selected").text(),
                    "País Consultado": $(".content-form-mis-busquedas #cboPais").children("option:selected").text(),
                    "Importador": getValueFilterFromFilters("#lstFiltros", fieldNameImportador),
                    "Exportador": getValueFilterFromFilters("#lstFiltros", fieldNameExportador),
                    "Marca": getValueFilterFromFilters("#lstFiltros", fieldNameMarca),
                    "Partida Aduanera": getValueFilterFromFilters("#lstFiltros", fieldNamePartidaAduanera),
                    "Descripción Comercial": getValueFilterFromFilters("#lstFiltros", fieldNameDescComercial),
                    "País Origen": getValueFilterFromFilters("#lstFiltros", fieldNamePaisDestino),
                    "Fecha Ini": getFtYearMonth($("input[name='cboDesde']").val()),
                    "Fecha Fin": getFtYearMonth($("input[name='cboHasta']").val()),
                });
        } else {
            mixpanel.track("Detalle Partida Aduanera",
                {
                    "Sección": "Mis Busquedas",
                    "Tab": nomTabSel,
                    "Identificador del Producto": $(this).parent().parent()[0].innerText,
                    "Tipo de Actividad": $(".content-form-mis-busquedas .radio-inline.control-label-withoutColor-vt :checked").parent()[0].innerText,
                    "Región": $(".content-form-mis-busquedas #cboPais2").children("option:selected").text(),
                    "País Consultado": $(".content-form-mis-busquedas #cboPais").children("option:selected").text(),
                    "Importador": getValueFilterFromFilters("#lstFiltros", fieldNameImportador),
                    "Exportador": getValueFilterFromFilters("#lstFiltros", fieldNameExportador),
                    "Marca": getValueFilterFromFilters("#lstFiltros", fieldNameMarca),
                    "Partida Aduanera": getValueFilterFromFilters("#lstFiltros", fieldNamePartidaAduanera),
                    "Descripción Comercial": getValueFilterFromFilters("#lstFiltros", fieldNameDescComercial),
                    "País Destino": getValueFilterFromFilters("#lstFiltros", fieldNamePaisDestino),
                    "Fecha Ini": getFtYearMonth($("input[name='cboDesde']").val()),
                    "Fecha Fin": getFtYearMonth($("input[name='cboHasta']").val()),
                });
        }
    });

    // Evento clic agregar a mis productos
    $(document).on("click", "div#gridResumenPartida button.btn.btn-addToFilters.btn-margin-right.pull-right.btnAddMyFavourites,div#gridPartida button.btn.btn-addToFilters.btn-margin-right.pull-right.btnAddMyFavourites", function () {
        var tipoActividadSel = $(".content-form-mis-busquedas .radio-inline.control-label-withoutColor-vt :checked").val();
        var nomTabSel = $("li.ui-tabs-tab.ui-corner-top.ui-state-default.ui-tab.ui-tabs-active.ui-state-active")[0].innerText;
        var productos = "";
        // Obtener productos seleccionados
        $(this).parents(".ui-corner-bottom.ui-widget-content").find("input:checked").parent().parent().find("p").each(function (i, e) {
            productos = productos + e.innerText + "|";
        })

        if (tipoActividadSel == tipoActividadImportaciones) {
            mixpanel.track("Clic Agregar a mis Productos",
                {
                    "Sección": "Mis Busquedas",
                    "Tab": nomTabSel,
                    "Lista de Productos Seleccionados": productos,
                    "Tipo de Actividad": $(".content-form-mis-busquedas .radio-inline.control-label-withoutColor-vt :checked").parent()[0].innerText,
                    "Región": $(".content-form-mis-busquedas #cboPais2").children("option:selected").text(),
                    "País": $(".content-form-mis-busquedas #cboPais").children("option:selected").text(),
                    "Importador": getValueFilterFromFilters("#lstFiltros", fieldNameImportador),
                    "Exportador": getValueFilterFromFilters("#lstFiltros", fieldNameExportador),
                    "Marca": getValueFilterFromFilters("#lstFiltros", fieldNameMarca),
                    "Partida Aduanera": getValueFilterFromFilters("#lstFiltros", fieldNamePartidaAduanera),
                    "Descripción Comercial": getValueFilterFromFilters("#lstFiltros", fieldNameDescComercial),
                    "País Origen": getValueFilterFromFilters("#lstFiltros", fieldNamePaisOrigen),
                    "Fecha Ini": getFtYearMonth($("input[name='cboDesde']").val()),
                    "Fecha Fin": getFtYearMonth($("input[name='cboHasta']").val()),
                });
        } else {
            mixpanel.track("Clic Agregar a mis Productos",
                {
                    "Sección": "Mis Busquedas",
                    "Tab": nomTabSel,
                    "Lista de Productos Seleccionados": productos,
                    "Tipo de Actividad": $(".content-form-mis-busquedas .radio-inline.control-label-withoutColor-vt :checked").parent()[0].innerText,
                    "Región": $(".content-form-mis-busquedas #cboPais2").children("option:selected").text(),
                    "País": $(".content-form-mis-busquedas #cboPais").children("option:selected").text(),
                    "Importador": getValueFilterFromFilters("#lstFiltros", fieldNameImportador),
                    "Exportador": getValueFilterFromFilters("#lstFiltros", fieldNameExportador),
                    "Marca": getValueFilterFromFilters("#lstFiltros", fieldNameMarca),
                    "Partida Aduanera": getValueFilterFromFilters("#lstFiltros", fieldNamePartidaAduanera),
                    "Descripción Comercial": getValueFilterFromFilters("#lstFiltros", fieldNameDescComercial),
                    "País Destino": getValueFilterFromFilters("#lstFiltros", fieldNamePaisOrigen),
                    "Fecha Ini": getFtYearMonth($("input[name='cboDesde']").val()),
                    "Fecha Fin": getFtYearMonth($("input[name='cboHasta']").val()),
                });
        }
    });

    // Evento clic agregar a mis importadores
    $(document).on("click", "div#gridResumenImportador button.btn.btn-addToFilters.btn-margin-right.pull-right.btnAddMyFavourites,div#gridImportador button.btn.btn-addToFilters.btn-margin-right.pull-right.btnAddMyFavourites,div#gridResumenImportadorExp button.btn.btn-addToFilters.btn-margin-right.pull-right.btnAddMyFavourites,div#gridImportadorExp button.btn.btn-addToFilters.btn-margin-right.pull-right.btnAddMyFavourites", function () {
        var tipoActividadSel = $(".content-form-mis-busquedas .radio-inline.control-label-withoutColor-vt :checked").val();
        var nomTabSel = $("li.ui-tabs-tab.ui-corner-top.ui-state-default.ui-tab.ui-tabs-active.ui-state-active")[0].innerText;
        var importadores = "";

        // Obtener importadores seleccionados
        $(this).parents(".ui-corner-bottom.ui-widget-content").find("input:checked").parent().parent().find("p").each(function (i, e) {
            importadores = importadores + e.innerText + "|";
        })

        if (tipoActividadSel == tipoActividadImportaciones) {
            mixpanel.track("Clic Agregar a mis Importadores",
                {
                    "Sección": "Mis Busquedas",
                    "Tab": nomTabSel,
                    "Lista de Importadores Seleccionados": importadores,
                    "Tipo de Actividad": $(".content-form-mis-busquedas .radio-inline.control-label-withoutColor-vt :checked").parent()[0].innerText,
                    "Región": $(".content-form-mis-busquedas #cboPais2").children("option:selected").text(),
                    "País": $(".content-form-mis-busquedas #cboPais").children("option:selected").text(),
                    "Importador": getValueFilterFromFilters("#lstFiltros", fieldNameImportador),
                    "Exportador": getValueFilterFromFilters("#lstFiltros", fieldNameExportador),
                    "Marca": getValueFilterFromFilters("#lstFiltros", fieldNameMarca),
                    "Partida Aduanera": getValueFilterFromFilters("#lstFiltros", fieldNamePartidaAduanera),
                    "Descripción Comercial": getValueFilterFromFilters("#lstFiltros", fieldNameDescComercial),
                    "País Origen": getValueFilterFromFilters("#lstFiltros", fieldNamePaisOrigen),
                    "Fecha Ini": getFtYearMonth($("input[name='cboDesde']").val()),
                    "Fecha Fin": getFtYearMonth($("input[name='cboHasta']").val())
                });
        } else {
            mixpanel.track("Clic Agregar a mis Importadores",
                {
                    "Sección": "Mis Busquedas",
                    "Tab": nomTabSel,
                    "Lista de Importadores Seleccionados": importadores,
                    "Tipo de Actividad": $(".content-form-mis-busquedas .radio-inline.control-label-withoutColor-vt :checked").parent()[0].innerText,
                    "Región": $(".content-form-mis-busquedas #cboPais2").children("option:selected").text(),
                    "País": $(".content-form-mis-busquedas #cboPais").children("option:selected").text(),
                    "Importador": getValueFilterFromFilters("#lstFiltros", fieldNameImportador),
                    "Exportador": getValueFilterFromFilters("#lstFiltros", fieldNameExportador),
                    "Marca": getValueFilterFromFilters("#lstFiltros", fieldNameMarca),
                    "Partida Aduanera": getValueFilterFromFilters("#lstFiltros", fieldNamePartidaAduanera),
                    "Descripción Comercial": getValueFilterFromFilters("#lstFiltros", fieldNameDescComercial),
                    "País Destino": getValueFilterFromFilters("#lstFiltros", fieldNamePaisOrigen),
                    "Fecha Ini": getFtYearMonth($("input[name='cboDesde']").val()),
                    "Fecha Fin": getFtYearMonth($("input[name='cboHasta']").val())
                });
        }
    });

    // Evento clic agregar a mis exportadores
    $(document).on("click", "div#gridResumenProveedor button.btn.btn-addToFilters.btn-margin-right.pull-right.btnAddMyFavourites,div#gridProveedor button.btn.btn-addToFilters.btn-margin-right.pull-right.btnAddMyFavourites,div#gridResumenExportador button.btn.btn-addToFilters.btn-margin-right.pull-right.btnAddMyFavourites,div#gridExportador button.btn.btn-addToFilters.btn-margin-right.pull-right.btnAddMyFavourites ", function () {
        var tipoActividadSel = $(".content-form-mis-busquedas .radio-inline.control-label-withoutColor-vt :checked").val();
        var nomTabSel = $("li.ui-tabs-tab.ui-corner-top.ui-state-default.ui-tab.ui-tabs-active.ui-state-active")[0].innerText;
        var exportadores = "";
        // Obtener exportadores seleccionados
        $(this).parents(".ui-corner-bottom.ui-widget-content").find("input:checked").parent().parent().find("p").each(function (i, e) {
            exportadores = exportadores + e.innerText + "|";
        })

        if (tipoActividadSel == tipoActividadImportaciones) {
            mixpanel.track("Clic Agregar a mis Exportadores",
                {
                    "Sección": "Mis Busquedas",
                    "Tab": nomTabSel,
                    "Lista de Exportadores Seleccionados": exportadores,
                    "Tipo de Actividad": $(".content-form-mis-busquedas .radio-inline.control-label-withoutColor-vt :checked").parent()[0].innerText,
                    "Región": $(".content-form-mis-busquedas #cboPais2").children("option:selected").text(),
                    "País": $(".content-form-mis-busquedas #cboPais").children("option:selected").text(),
                    "Importador": getValueFilterFromFilters("#lstFiltros", fieldNameImportador),
                    "Exportador": getValueFilterFromFilters("#lstFiltros", fieldNameExportador),
                    "Marca": getValueFilterFromFilters("#lstFiltros", fieldNameMarca),
                    "Partida Aduanera": getValueFilterFromFilters("#lstFiltros", fieldNamePartidaAduanera),
                    "Descripción Comercial": getValueFilterFromFilters("#lstFiltros", fieldNameDescComercial),
                    "País Origen": getValueFilterFromFilters("#lstFiltros", fieldNamePaisOrigen),
                    "Fecha Ini": getFtYearMonth($("input[name='cboDesde']").val()),
                    "Fecha Fin": getFtYearMonth($("input[name='cboHasta']").val())
                });
        } else {
            mixpanel.track("Clic Agregar a mis Exportadores",
                {
                    "Sección": "Mis Busquedas",
                    "Tab": nomTabSel,
                    "Lista de Exportadores Seleccionados": exportadores,
                    "Tipo de Actividad": $(".content-form-mis-busquedas .radio-inline.control-label-withoutColor-vt :checked").parent()[0].innerText,
                    "Región": $(".content-form-mis-busquedas #cboPais2").children("option:selected").text(),
                    "País": $(".content-form-mis-busquedas #cboPais").children("option:selected").text(),
                    "Importador": getValueFilterFromFilters("#lstFiltros", fieldNameImportador),
                    "Exportador": getValueFilterFromFilters("#lstFiltros", fieldNameExportador),
                    "Marca": getValueFilterFromFilters("#lstFiltros", fieldNameMarca),
                    "Partida Aduanera": getValueFilterFromFilters("#lstFiltros", fieldNamePartidaAduanera),
                    "Descripción Comercial": getValueFilterFromFilters("#lstFiltros", fieldNameDescComercial),
                    "País Destino": getValueFilterFromFilters("#lstFiltros", fieldNamePaisOrigen),
                    "Fecha Ini": getFtYearMonth($("input[name='cboDesde']").val()),
                    "Fecha Fin": getFtYearMonth($("input[name='cboHasta']").val())
                });
        }
    });

    // Evento clic visualizar reporte financiero de importador
    $(document).on("click", "#tbodyResumenImportador img.cursor-action,#tbodyTabImportador img.cursor-action,#tbodyTabDetalleExcel a.lnkSentinel img.cursor-action,#tbodyResumenExportador img.cursor-action,#tbodyTabExportador img.cursor-action", function () {
        var tipoActividadSel = $(".content-form-mis-busquedas .radio-inline.control-label-withoutColor-vt :checked").val();
        var nomTabSel = $("li.ui-tabs-tab.ui-corner-top.ui-state-default.ui-tab.ui-tabs-active.ui-state-active")[0].innerText;

        if (tipoActividadSel == tipoActividadImportaciones) {
            mixpanel.track("Clic Visualizar Reporte Financiero de Importador",
                {
                    "Sección": "Mis Busquedas",
                    "Tab": nomTabSel,
                    "Identificador del Importador a Ver Reporte": $(this).parent().parent()[0].innerText,
                    "Tipo de Actividad": $(".content-form-mis-busquedas .radio-inline.control-label-withoutColor-vt :checked").parent()[0].innerText,
                    "Región": $(".content-form-mis-busquedas #cboPais2").children("option:selected").text(),
                    "País": $(".content-form-mis-busquedas #cboPais").children("option:selected").text(),
                    "Importador": getValueFilterFromFilters("#lstFiltros", fieldNameImportador),
                    "Exportador": getValueFilterFromFilters("#lstFiltros", fieldNameExportador),
                    "Marca": getValueFilterFromFilters("#lstFiltros", fieldNameMarca),
                    "Partida Aduanera": getValueFilterFromFilters("#lstFiltros", fieldNamePartidaAduanera),
                    "Descripción Comercial": getValueFilterFromFilters("#lstFiltros", fieldNameDescComercial),
                    "País Origen": getValueFilterFromFilters("#lstFiltros", fieldNamePaisOrigen),
                    "Fecha Ini": getFtYearMonth($("input[name='cboDesde']").val()),
                    "Fecha Fin": getFtYearMonth($("input[name='cboHasta']").val())
                });
        } else {
            mixpanel.track("Clic Visualizar Reporte Financiero de Exportador",
                {
                    "Sección": "Mis Busquedas",
                    "Tab": nomTabSel,
                    "Identificador del Exportador a Ver Reporte": $(this).parent().parent()[0].innerText,
                    "Tipo de Actividad": $(".content-form-mis-busquedas .radio-inline.control-label-withoutColor-vt :checked").parent()[0].innerText,
                    "Región": $(".content-form-mis-busquedas #cboPais2").children("option:selected").text(),
                    "País": $(".content-form-mis-busquedas #cboPais").children("option:selected").text(),
                    "Importador": getValueFilterFromFilters("#lstFiltros", fieldNameImportador),
                    "Exportador": getValueFilterFromFilters("#lstFiltros", fieldNameExportador),
                    "Marca": getValueFilterFromFilters("#lstFiltros", fieldNameMarca),
                    "Partida Aduanera": getValueFilterFromFilters("#lstFiltros", fieldNamePartidaAduanera),
                    "Descripción Comercial": getValueFilterFromFilters("#lstFiltros", fieldNameDescComercial),
                    "País Destino": getValueFilterFromFilters("#lstFiltros", fieldNamePaisOrigen),
                    "Fecha Ini": getFtYearMonth($("input[name='cboDesde']").val()),
                    "Fecha Fin": getFtYearMonth($("input[name='cboHasta']").val())
                });
        }
    });


    // Evento clic restablecer buscador
    $(document).on("click", "button#btnRestablecer", function () {
        mixpanel.track("Clic Restablecer Buscador",
            {
                "Sección": "Mis Busquedas"
            });
    });

    // Evento clic descargar reporte en formato excel
    $(document).on("click", "img.ico_excel", function () {
        var nomTabSel = $("li.ui-tabs-tab.ui-corner-top.ui-state-default.ui-tab.ui-tabs-active.ui-state-active")[0].innerText;
        var idPlantilla = nomTabSel.toLocaleUpperCase() == tabDetalleExcel.toLocaleUpperCase() ? $("div#tab-detalle #DropDownDescarga").children("option:selected").text() : "";
        var tipoActividadSel = $(".content-form-mis-busquedas .radio-inline.control-label-withoutColor-vt :checked").val();
        var letExpExcel = $(this).attr("data-exp-excel-let") ? " " + $(this).attr("data-exp-excel-let"):"";
        if (tipoActividadSel == tipoActividadImportaciones) {
            mixpanel.track("Clic Descargar Reporte en Formato Excel" + letExpExcel,
                {
                    "Sección": "Mis Busquedas",
                    "Tab": nomTabSel,
                    "Identificador de Plantilla": idPlantilla,
                    "Tipo de Actividad": $(".content-form-mis-busquedas .radio-inline.control-label-withoutColor-vt :checked").parent()[0].innerText,
                    "Región": $(".content-form-mis-busquedas #cboPais2").children("option:selected").text(),
                    "País": $(".content-form-mis-busquedas #cboPais").children("option:selected").text(),
                    "Importador": getValueFilterFromFilters("#lstFiltros", fieldNameImportador),
                    "Exportador": getValueFilterFromFilters("#lstFiltros", fieldNameExportador),
                    "Marca": getValueFilterFromFilters("#lstFiltros", fieldNameMarca),
                    "Partida Aduanera": getValueFilterFromFilters("#lstFiltros", fieldNamePartidaAduanera),
                    "Descripción Comercial": getValueFilterFromFilters("#lstFiltros", fieldNameDescComercial),
                    "País Origen": getValueFilterFromFilters("#lstFiltros", fieldNamePaisOrigen),
                    "Fecha Ini": getFtYearMonth($("input[name='cboDesde']").val()),
                    "Fecha Fin": getFtYearMonth($("input[name='cboHasta']").val()),
                });
        } else {
            mixpanel.track("Clic Descargar Reporte en Formato Excel",
                {
                    "Sección": "Mis Busquedas",
                    "Tab": nomTabSel,
                    "Identificador de Plantilla": idPlantilla,
                    "Tipo de Actividad": $(".content-form-mis-busquedas .radio-inline.control-label-withoutColor-vt :checked").parent()[0].innerText,
                    "Región": $(".content-form-mis-busquedas #cboPais2").children("option:selected").text(),
                    "País": $(".content-form-mis-busquedas #cboPais").children("option:selected").text(),
                    "Importador": getValueFilterFromFilters("#lstFiltros", fieldNameImportador),
                    "Exportador": getValueFilterFromFilters("#lstFiltros", fieldNameExportador),
                    "Marca": getValueFilterFromFilters("#lstFiltros", fieldNameMarca),
                    "Partida Aduanera": getValueFilterFromFilters("#lstFiltros", fieldNamePartidaAduanera),
                    "Descripción Comercial": getValueFilterFromFilters("#lstFiltros", fieldNameDescComercial),
                    "País Destino": getValueFilterFromFilters("#lstFiltros", fieldNamePaisOrigen),
                    "Fecha Ini": getFtYearMonth($("input[name='cboDesde']").val()),
                    "Fecha Fin": getFtYearMonth($("input[name='cboHasta']").val()),
                });
        }
    });

    // Evento clic aplicar filtros
    $(document).on("click", "div#gridResumenPartida button.btn.btn-addToFilters.pull-right.btnAddFilterAndSearch,div#gridResumenPaisOrigen button.btn.btn-addToFilters.pull-right.btnAddFilterAndSearch,div#gridResumenImportador button.btn.btn-addToFilters.pull-right.btnAddFilterAndSearch,div#gridResumenProveedor button.btn.btn-addToFilters.pull-right.btnAddFilterAndSearch,div#gridResumenViaTransp button.btn.btn-addToFilters.pull-right.btnAddFilterAndSearch,div#gridPartida button.btn.btn-addToFilters.pull-right.btnAddFilterAndSearch,div#gridImportador button.btn.btn-addToFilters.pull-right.btnAddFilterAndSearch,div#gridProveedor button.btn.btn-addToFilters.pull-right.btnAddFilterAndSearch,div#gridPaisOrigen button.btn.btn-addToFilters.pull-right.btnAddFilterAndSearch,div#gridViaTransp button.btn.btn-addToFilters.pull-right.btnAddFilterAndSearch,div#gridAduanaDUA button.btn.btn-addToFilters.pull-right.btnAddFilterAndSearch", function () {
        var tipoActividadSel = $(".content-form-mis-busquedas .radio-inline.control-label-withoutColor-vt :checked").val();
        var nomTabSel = $("li.ui-tabs-tab.ui-corner-top.ui-state-default.ui-tab.ui-tabs-active.ui-state-active")[0].innerText;
        var tbody = "tbody#tbody" + $(this)[0].dataset.idTabla;
        var filtro = $(this)[0].dataset.filtro;
        var items = "";
        $(tbody).find("input:checked").parent().parent().find("p").each(function (i, e) {
            items = items + "[" + filtro + "]" + e.innerText + "|";
        });

        if (tipoActividadSel == tipoActividadImportaciones) {
            mixpanel.track("Clic Aplicar Filtros",
                {
                    "Sección": "Mis Busquedas",
                    "Tab": nomTabSel,
                    "Lista de Items Seleccionados a Filtrar": items,
                    "Tabla": $(this).attr("data-filtro"),
                    "Tipo de Actividad": $(".content-form-mis-busquedas .radio-inline.control-label-withoutColor-vt :checked").parent()[0].innerText,
                    "Región": $(".content-form-mis-busquedas #cboPais2").children("option:selected").text(),
                    "País": $(".content-form-mis-busquedas #cboPais").children("option:selected").text(),
                    "Importador": getValueFilterFromFilters("#lstFiltros", fieldNameImportador),
                    "Exportador": getValueFilterFromFilters("#lstFiltros", fieldNameExportador),
                    "Marca": getValueFilterFromFilters("#lstFiltros", fieldNameMarca),
                    "Partida Aduanera": getValueFilterFromFilters("#lstFiltros", fieldNamePartidaAduanera),
                    "Descripción Comercial": getValueFilterFromFilters("#lstFiltros", fieldNameDescComercial),
                    "País Origen": getValueFilterFromFilters("#lstFiltros", fieldNamePaisDestino),
                    "Fecha Ini": getFtYearMonth($("input[name='cboDesde']").val()),
                    "Fecha Fin": getFtYearMonth($("input[name='cboHasta']").val())
                });
        } else {
            mixpanel.track("Clic Aplicar Filtros",
                {
                    "Sección": "Mis Busquedas",
                    "Tab": nomTabSel,
                    "Lista de Items Seleccionados a Filtrar": items,
                    "Tabla": $(this).attr("data-filtro"),
                    "Tipo de Actividad": $(".content-form-mis-busquedas .radio-inline.control-label-withoutColor-vt :checked").parent()[0].innerText,
                    "Región": $(".content-form-mis-busquedas #cboPais2").children("option:selected").text(),
                    "País": $(".content-form-mis-busquedas #cboPais").children("option:selected").text(),
                    "Importador": getValueFilterFromFilters("#lstFiltros", fieldNameImportador),
                    "Exportador": getValueFilterFromFilters("#lstFiltros", fieldNameExportador),
                    "Marca": getValueFilterFromFilters("#lstFiltros", fieldNameMarca),
                    "Partida Aduanera": getValueFilterFromFilters("#lstFiltros", fieldNamePartidaAduanera),
                    "Descripción Comercial": getValueFilterFromFilters("#lstFiltros", fieldNameDescComercial),
                    "País Destino": getValueFilterFromFilters("#lstFiltros", fieldNamePaisDestino),
                    "Fecha Ini": getFtYearMonth($("input[name='cboDesde']").val()),
                    "Fecha Fin": getFtYearMonth($("input[name='cboHasta']").val())
                });
        }
    });


    // Evento clic ver detalle de registro de transacciones comerciales
    $(document).on("click", "a.lnkVerRegistros", function () {
        var nomTabSel = $("li.ui-tabs-tab.ui-corner-top.ui-state-default.ui-tab.ui-tabs-active.ui-state-active")[0].innerText;
        var tipoActividadSel = $(".content-form-mis-busquedas .radio-inline.control-label-withoutColor-vt :checked").val();

        if (tipoActividadSel == tipoActividadImportaciones) {
            mixpanel.track("Clic Ver Detalle de Registro de la Transacciones Comerciales",
                {
                    "Sección": "Mis Busquedas",
                    "Tab": nomTabSel,
                    "Identificador de ítem seleccionado": $(this).parent().parent().find("p.wspace-normal")[0].innerText,
                    "Tabla": $(this).attr("data-filtro"),
                    "Tipo de Actividad": $(".content-form-mis-busquedas .radio-inline.control-label-withoutColor-vt :checked").parent()[0].innerText,
                    "Región": $(".content-form-mis-busquedas #cboPais2").children("option:selected").text(),
                    "País": $(".content-form-mis-busquedas #cboPais").children("option:selected").text(),
                    "Importador": getValueFilterFromFilters("#lstFiltros", fieldNameImportador),
                    "Exportador": getValueFilterFromFilters("#lstFiltros", fieldNameExportador),
                    "Marca": getValueFilterFromFilters("#lstFiltros", fieldNameMarca),
                    "Partida Aduanera": getValueFilterFromFilters("#lstFiltros", fieldNamePartidaAduanera),
                    "Descripción Comercial": getValueFilterFromFilters("#lstFiltros", fieldNameDescComercial),
                    "País Origen": getValueFilterFromFilters("#lstFiltros", fieldNamePaisDestino),
                    "Fecha Ini": getFtYearMonth($("input[name='cboDesde']").val()),
                    "Fecha Fin": getFtYearMonth($("input[name='cboHasta']").val())
                });
        } else {
            mixpanel.track("Clic Ver Detalle de Registro de la Transacciones Comerciales",
                {
                    "Sección": "Mis Busquedas",
                    "Tab": nomTabSel,
                    "Identificador de ítem seleccionado": $(this).parent().parent().find("p.wspace-normal")[0].innerText,
                    "Tabla": $(this).attr("data-filtro"),
                    "Tipo de Actividad": $(".content-form-mis-busquedas .radio-inline.control-label-withoutColor-vt :checked").parent()[0].innerText,
                    "Región": $(".content-form-mis-busquedas #cboPais2").children("option:selected").text(),
                    "País": $(".content-form-mis-busquedas #cboPais").children("option:selected").text(),
                    "Importador": getValueFilterFromFilters("#lstFiltros", fieldNameImportador),
                    "Exportador": getValueFilterFromFilters("#lstFiltros", fieldNameExportador),
                    "Marca": getValueFilterFromFilters("#lstFiltros", fieldNameMarca),
                    "Partida Aduanera": getValueFilterFromFilters("#lstFiltros", fieldNamePartidaAduanera),
                    "Descripción Comercial": getValueFilterFromFilters("#lstFiltros", fieldNameDescComercial),
                    "País Destino": getValueFilterFromFilters("#lstFiltros", fieldNamePaisDestino),
                    "Fecha Ini": getFtYearMonth($("input[name='cboDesde']").val()),
                    "Fecha Fin": getFtYearMonth($("input[name='cboHasta']").val())
                });
        }
    })

    // Evento seleccion de item especifico para filtrado de lista
    $(document).on("change", "select.pull-left.select-columnHead-tableLarge.cboFiltroTabla", function () {
        var nomTabSel = $("li.ui-tabs-tab.ui-corner-top.ui-state-default.ui-tab.ui-tabs-active.ui-state-active")[0].innerText;
        var tipoActividadSel = $(".content-form-mis-busquedas .radio-inline.control-label-withoutColor-vt :checked").val();

        if (tipoActividadSel == tipoActividadImportaciones) {
            mixpanel.track("Selección de Item Específico para fitrado de lista",
                {
                    "Sección": "Mis Busquedas",
                    "Tab": nomTabSel,
                    "Identificador de Item Seleccionado": $(this).children("option:selected").text(),
                    "Tabla": $(this).attr("data-filtro-cbo"),
                    "Tipo de Actividad": $(".content-form-mis-busquedas .radio-inline.control-label-withoutColor-vt :checked").parent()[0].innerText,
                    "Región": $(".content-form-mis-busquedas #cboPais2").children("option:selected").text(),
                    "País": $(".content-form-mis-busquedas #cboPais").children("option:selected").text(),
                    "Importador": getValueFilterFromFilters("#lstFiltros", fieldNameImportador),
                    "Exportador": getValueFilterFromFilters("#lstFiltros", fieldNameExportador),
                    "Marca": getValueFilterFromFilters("#lstFiltros", fieldNameMarca),
                    "Partida Aduanera": getValueFilterFromFilters("#lstFiltros", fieldNamePartidaAduanera),
                    "Descripción Comercial": getValueFilterFromFilters("#lstFiltros", fieldNameDescComercial),
                    "País Origen": getValueFilterFromFilters("#lstFiltros", fieldNamePaisDestino),
                    "Fecha Ini": getFtYearMonth($("input[name='cboDesde']").val()),
                    "Fecha Fin": getFtYearMonth($("input[name='cboHasta']").val())
                });
        } else {
            mixpanel.track("Selección de Item Específico para fitrado de lista",
                {
                    "Sección": "Mis Busquedas",
                    "Tab": nomTabSel,
                    "Identificador de Item Seleccionado": $(this).children("option:selected").text(),
                    "Tabla": $(this).attr("data-filtro-cbo"),
                    "Tipo de Actividad": $(".content-form-mis-busquedas .radio-inline.control-label-withoutColor-vt :checked").parent()[0].innerText,
                    "Región": $(".content-form-mis-busquedas #cboPais2").children("option:selected").text(),
                    "País": $(".content-form-mis-busquedas #cboPais").children("option:selected").text(),
                    "Importador": getValueFilterFromFilters("#lstFiltros", fieldNameImportador),
                    "Exportador": getValueFilterFromFilters("#lstFiltros", fieldNameExportador),
                    "Marca": getValueFilterFromFilters("#lstFiltros", fieldNameMarca),
                    "Partida Aduanera": getValueFilterFromFilters("#lstFiltros", fieldNamePartidaAduanera),
                    "Descripción Comercial": getValueFilterFromFilters("#lstFiltros", fieldNameDescComercial),
                    "País Destino": getValueFilterFromFilters("#lstFiltros", fieldNamePaisDestino),
                    "Fecha Ini": getFtYearMonth($("input[name='cboDesde']").val()),
                    "Fecha Fin": getFtYearMonth($("input[name='cboHasta']").val())
                });
        }
    });

    // Evento clic detalle de la importacion
    $(document).on("click", "a.lnkVerDetalleById img.cursor-action", function () {
        var nomTabDetalleExcel = "Detalle - Excel";
        var tipoActividadSel = $(".content-form-mis-busquedas .radio-inline.control-label-withoutColor-vt :checked").val();

        if (tipoActividadSel == tipoActividadImportaciones) {
            mixpanel.track("Clic Detalle de la Importación",
                {
                    "Sección": "Mis Busquedas",
                    "Tab": nomTabDetalleExcel,
                    "Identificador de la Importación Efectuada": $(this).parent().parent().parent().parent().find("p.wspace-normal")[0].innerText,
                    "Tipo de Actividad": $(".content-form-mis-busquedas .radio-inline.control-label-withoutColor-vt :checked").parent()[0].innerText,
                    "Región": $(".content-form-mis-busquedas #cboPais2").children("option:selected").text(),
                    "País": $(".content-form-mis-busquedas #cboPais").children("option:selected").text(),
                    "Importador": getValueFilterFromFilters("#lstFiltros", fieldNameImportador),
                    "Exportador": getValueFilterFromFilters("#lstFiltros", fieldNameExportador),
                    "Marca": getValueFilterFromFilters("#lstFiltros", fieldNameMarca),
                    "Partida Aduanera": getValueFilterFromFilters("#lstFiltros", fieldNamePartidaAduanera),
                    "Descripción Comercial": getValueFilterFromFilters("#lstFiltros", fieldNameDescComercial),
                    "País Origen": getValueFilterFromFilters("#lstFiltros", fieldNamePaisDestino),
                    "Fecha Ini": getFtYearMonth($("input[name='cboDesde']").val()),
                    "Fecha Fin": getFtYearMonth($("input[name='cboHasta']").val())
                });
        } else {
            mixpanel.track("Clic Detalle de la Importación",
                {
                    "Sección": "Mis Busquedas",
                    "Tab": nomTabDetalleExcel,
                    "Identificador de la Importación Efectuada": $(this).parent().parent().parent().parent().find("p.wspace-normal")[0].innerText,
                    "Tipo de Actividad": $(".content-form-mis-busquedas .radio-inline.control-label-withoutColor-vt :checked").parent()[0].innerText,
                    "Región": $(".content-form-mis-busquedas #cboPais2").children("option:selected").text(),
                    "País": $(".content-form-mis-busquedas #cboPais").children("option:selected").text(),
                    "Importador": getValueFilterFromFilters("#lstFiltros", fieldNameImportador),
                    "Exportador": getValueFilterFromFilters("#lstFiltros", fieldNameExportador),
                    "Marca": getValueFilterFromFilters("#lstFiltros", fieldNameMarca),
                    "Partida Aduanera": getValueFilterFromFilters("#lstFiltros", fieldNamePartidaAduanera),
                    "Descripción Comercial": getValueFilterFromFilters("#lstFiltros", fieldNameDescComercial),
                    "País Destino": getValueFilterFromFilters("#lstFiltros", fieldNamePaisDestino),
                    "Fecha Ini": getFtYearMonth($("input[name='cboDesde']").val()),
                    "Fecha Fin": getFtYearMonth($("input[name='cboHasta']").val())
                });
        }
    });


    // Evento clic detalle de partida aduanera
    $(document).on("click", "#tbodyTabDetalleExcel img.cursor-action", function () {
        var tipoActividadSel = $(".content-form-mis-busquedas .radio-inline.control-label-withoutColor-vt :checked").val();
        var nomTabSel = $("li.ui-tabs-tab.ui-corner-top.ui-state-default.ui-tab.ui-tabs-active.ui-state-active")[0].innerText;

        if (tipoActividadSel == tipoActividadImportaciones) {
            mixpanel.track("Clic Detalle Partida Aduanera",
                {
                    "Sección": "Mis Busquedas",
                    "Tab": nomTabSel,
                    "Tipo de Actividad": $(".content-form-mis-busquedas .radio-inline.control-label-withoutColor-vt :checked").parent()[0].innerText,
                    "Región": $(".content-form-mis-busquedas #cboPais2").children("option:selected").text(),
                    "País Consultado": $(".content-form-mis-busquedas #cboPais").children("option:selected").text(),
                    "Importador": getValueFilterFromFilters("#lstFiltros", fieldNameImportador),
                    "Exportador": getValueFilterFromFilters("#lstFiltros", fieldNameExportador),
                    "Marca": getValueFilterFromFilters("#lstFiltros", fieldNameMarca),
                    "Partida Aduanera": getValueFilterFromFilters("#lstFiltros", fieldNamePartidaAduanera),
                    "Descripción Comercial": getValueFilterFromFilters("#lstFiltros", fieldNameDescComercial),
                    "País Origen": getValueFilterFromFilters("#lstFiltros", fieldNamePaisOrigen),
                    "Fecha Ini": getFtYearMonth($("input[name='cboDesde']").val()),
                    "Fecha Fin": getFtYearMonth($("input[name='cboHasta']").val())
                });
        } else {
            mixpanel.track("Clic Detalle Partida Aduanera",
                {
                    "Sección": "Mis Busquedas",
                    "Tab": nomTabSel,
                    "Tipo de Actividad": $(".content-form-mis-busquedas .radio-inline.control-label-withoutColor-vt :checked").parent()[0].innerText,
                    "Región": $(".content-form-mis-busquedas #cboPais2").children("option:selected").text(),
                    "País Consultado": $(".content-form-mis-busquedas #cboPais").children("option:selected").text(),
                    "Importador": getValueFilterFromFilters("#lstFiltros", fieldNameImportador),
                    "Exportador": getValueFilterFromFilters("#lstFiltros", fieldNameExportador),
                    "Marca": getValueFilterFromFilters("#lstFiltros", fieldNameMarca),
                    "Partida Aduanera": getValueFilterFromFilters("#lstFiltros", fieldNamePartidaAduanera),
                    "Descripción Comercial": getValueFilterFromFilters("#lstFiltros", fieldNameDescComercial),
                    "País Destino": getValueFilterFromFilters("#lstFiltros", fieldNamePaisOrigen),
                    "Fecha Ini": getFtYearMonth($("input[name='cboDesde']").val()),
                    "Fecha Fin": getFtYearMonth($("input[name='cboHasta']").val())
                });
        }
    });

    // Evento clic de busqueda de importador / exportador
    $(document).on("click", "a[target='_blank'] img.cursor-action", function () {
        var indiceColumna = $(this).parent().parent()[0].cellIndex;
        var textoColumna = $("div#gridDetalleExcel div.table-responsive table thead tr.table-title th")[indiceColumna].innerText;
        var tipoActividadSel = $(".content-form-mis-busquedas .radio-inline.control-label-withoutColor-vt :checked").val();

        var nomTabDetalleExcel = "Detalle - Excel";

        if (tipoActividadSel == tipoActividadImportaciones) {
            mixpanel.track("Clic Búsqueda de de Importador / Exportador",
                {
                    "Sección": "Mis Busquedas",
                    "Tab": nomTabDetalleExcel,
                    "Identificador del evento": $(this).parent().parent()[0].textContent.trim(),
                    "Identificador de la Entidad": textoColumna,
                    "Tipo de Actividad": $(".content-form-mis-busquedas .radio-inline.control-label-withoutColor-vt :checked").parent()[0].innerText,
                    "Región": $(".content-form-mis-busquedas #cboPais2").children("option:selected").text(),
                    "País": $(".content-form-mis-busquedas #cboPais").children("option:selected").text(),
                    "Importador": getValueFilterFromFilters("#lstFiltros", fieldNameImportador),
                    "Exportador": getValueFilterFromFilters("#lstFiltros", fieldNameExportador),
                    "Marca": getValueFilterFromFilters("#lstFiltros", fieldNameMarca),
                    "Partida Aduanera": getValueFilterFromFilters("#lstFiltros", fieldNamePartidaAduanera),
                    "Descripción Comercial": getValueFilterFromFilters("#lstFiltros", fieldNameDescComercial),
                    "País Origen": getValueFilterFromFilters("#lstFiltros", fieldNamePaisDestino),
                    "Fecha Ini": getFtYearMonth($("input[name='cboDesde']").val()),
                    "Fecha Fin": getFtYearMonth($("input[name='cboHasta']").val())
                });
        } else {
            mixpanel.track("Clic Búsqueda de de Importador / Exportador",
                {
                    "Sección": "Mis Busquedas",
                    "Tab": nomTabDetalleExcel,
                    "Identificador del evento": $(this).parent().parent()[0].textContent.trim(),
                    "Identificador de la Entidad": textoColumna,
                    "Tipo de Actividad": $(".content-form-mis-busquedas .radio-inline.control-label-withoutColor-vt :checked").parent()[0].innerText,
                    "Región": $(".content-form-mis-busquedas #cboPais2").children("option:selected").text(),
                    "País": $(".content-form-mis-busquedas #cboPais").children("option:selected").text(),
                    "Importador": getValueFilterFromFilters("#lstFiltros", fieldNameImportador),
                    "Exportador": getValueFilterFromFilters("#lstFiltros", fieldNameExportador),
                    "Marca": getValueFilterFromFilters("#lstFiltros", fieldNameMarca),
                    "Partida Aduanera": getValueFilterFromFilters("#lstFiltros", fieldNamePartidaAduanera),
                    "Descripción Comercial": getValueFilterFromFilters("#lstFiltros", fieldNameDescComercial),
                    "País Destino": getValueFilterFromFilters("#lstFiltros", fieldNamePaisDestino),
                    "Fecha Ini": getFtYearMonth($("input[name='cboDesde']").val()),
                    "Fecha Fin": getFtYearMonth($("input[name='cboHasta']").val())
                });
        }
    });

    // Evento mis registros productos/importaciones/exportaciones
    $(document).on("click", "#btnMisPartidas,#btnImportadoresExp,#btnMisExportadores", function () {
        var tipoMisRegistros = $(this).attr("data-field");
        mixpanel.track("Clic Mis Registros " + tipoMisRegistros,
            {
                "Sección": "Mis Busquedas"
            });
    });

    // Evento clic eliminar seleccionados
    $(document).on("click", "button#btnEliminarFiltros", function () {
        mixpanel.track("Clic Eliminar seleccionados",
            {
                "Sección": "Mis Busquedas",
                "Identificador de Filtro a Eliminar": $("div.formFiltersMisBusquedas #lstFiltros").children("option:selected").text()
            });
    });

    // Evento seleccion de filtro de lista
    MixPanel.filtroLista = function () {
        $("#txtFavoritoF").autocomplete("widget").delegate('li', 'click', function (e) {
            e.stopImmediatePropagation();
            mixpanel.track("Selección de Filtro de Lista",
                {
                    "Sección": "Mis Busquedas",
                    "Texto Ingresado a Filtrar": "",
                    "Item de la Lista Seleccionado": $(this)[0].textContent
                });
        });
    }

    // Evento seleccion de item de lista
    $("div.modal-dialog #cboFavoritosF").on("change", function () {
        mixpanel.track("Seleccion de Item de Lista",
            {
                "Sección": "Mis Busquedas",
                "Item Seleccionado": $(this).children("option:selected").text()
            });
    });

    // Evento clic filtrar texto ingresado
    $(document).on("click", "a#lnkBuscarF img.cursor-action", function () {
        mixpanel.track("Clic Filtrar Texto Ingresado",
            {
                "Sección": "Mis Busquedas",
                "Texto Ingresado a Filtrar": $("input#txtFavoritoF").val()
            });
    });

    // Evento clic filtrar texto ingresado - Buscar por DUA
    $(document).on("click", "img#imgDuaExtra", function () {
        mixpanel.track("Clic Buscar por DUA",
            {
                "Sección": "Mis Busquedas",
                "Texto a buscar": $("input#txtDuaExtra").val()
            });
    });

    // Evento clic filtrar texto ingresado - Buscar por Descripcion Comercial
    $(document).on("click", "img#imgDesComExtra", function () {
        mixpanel.track("Clic Buscar por Descripción Comercial",
            {
                "Sección": "Mis Busquedas",
                "Texto a buscar": $("input#txtDesComExtra").val()
            });
    });

    // Evento clic refrescar lista de items
    $(document).on("click", "div#tab-detalle div#gridDetalleExcel a.lnkReset img,a#lnkRestablecerF img.cursor-action", function () {
        var nomTabSel = "";
        //  Si el refrescar viene de los tabs
        if ($("li.ui-tabs-tab.ui-corner-top.ui-state-default.ui-tab.ui-tabs-active.ui-state-active").length>0) {
            nomTabSel = $("li.ui-tabs-tab.ui-corner-top.ui-state-default.ui-tab.ui-tabs-active.ui-state-active")[0].innerText;
            mixpanel.track("Clic Refrescar Lista de Items",
                {
                    "Sección": "Mis Busquedas",
                    "Tab": nomTabSel
                });
        }   //  Si no viene de los filtros
        else {
            mixpanel.track("Clic Refrescar Lista de Items",
                {
                    "Sección": "Mis Busquedas"
                });
        }

       
    });

    // Evento clic agregar a filtros
    $(document).on("click", "button#btnAgregarFavAFiltros", function () {
        var filtros = "";
        // Obtener filtros seleccionados
        $("tbody#modal-favoritos-body").find("input:checked").parent().parent().find("td").not(".column-text-center.cell-vertical-align-middle").each(function (i, e) {
            filtros = filtros + e.innerText + "|";
        });

        mixpanel.track("Clic Agregar a Filtros",
            {
                "Sección": "Mis Busquedas",
                "Lista de ítems Seleccionados": filtros
            });
    });


    // Evento clic grupo
    $(document).on("click", "tbody#modal-favoritos-body a", function () {
        mixpanel.track("Clic Grupo",
            {
                "Sección": "Mis Busquedas",
                "Identificador del Grupo Seleccionado": $(this)[0].innerText
            });
    });


    // Evento clic más informacion ventana emergente mis importaciones
    $(document).on("click", ".j_logow a", function () {
        mixpanel.track("Clic Mas Información ",
            {
                "Sección": "Mis Busquedas",
                "Tab Seleccionado": globalTabSel
            });
    });

    // Evento clic calificacion de la empresa ventana emergente mis importaciones
    $(document).on("click", "span.j_link-text", function () {
        mixpanel.track("Clic Calificación de la Empresa ",
            {
                "Sección": "Mis Busquedas",
                "Tab Seleccionado": globalTabSel
            });
    });

    // Evento clic para ir al sitio web de Sunat
    $(document).on("click", ".sunat", function () {
        mixpanel.track("Clic para ir a sitio de Sunat ",
            {
                "Sección": "Mis Busquedas",
                "Tab Seleccionado": globalTabSel,
                "País Consultado": $(".content-form-mis-busquedas #cboPais").children("option:selected").text()
            });
    });

    // Evento Click Exportar/Imprimir grafico
    MixPanel.exportarGrafico = function (vAccionSeleccionada, vIdContainer) {
        var nomTabSel = $("li.ui-tabs-tab.ui-corner-top.ui-state-default.ui-tab.ui-tabs-active.ui-state-active")[0].innerText;
            mixpanel.track("Click Exportar/Imprimir Gráfico",
                {
                    "Sección": "Mis Busquedas",
                    "Id": vIdContainer,
                    "Tab": nomTabSel,
                    "Acción Seleccionada": vAccionSeleccionada,
                    "Tipo de Actividad": $(".content-form-mis-busquedas .radio-inline.control-label-withoutColor-vt :checked").parent()[0].innerText,
                    "Región": $(".content-form-mis-busquedas #cboPais2").children("option:selected").text(),
                    "País": $(".content-form-mis-busquedas #cboPais").children("option:selected").text(),
                    "Importador": getValueFilterFromFilters("#lstFiltros", fieldNameImportador),
                    "Exportador": getValueFilterFromFilters("#lstFiltros", fieldNameExportador),
                    "Marca": getValueFilterFromFilters("#lstFiltros", fieldNameMarca),
                    "Partida Aduanera": getValueFilterFromFilters("#lstFiltros", fieldNamePartidaAduanera),
                    "Descripción Comercial": getValueFilterFromFilters("#lstFiltros", fieldNameDescComercial),
                    "País Origen": getValueFilterFromFilters("#lstFiltros", fieldNamePaisOrigen),
                    "Fecha Ini": getFtYearMonth($("input[name='cboDesde']").val()),
                    "Fecha Fin": getFtYearMonth($("input[name='cboHasta']").val())
                });

    }

    // Evento Click Grafico
    MixPanel.clickGrafico = function (vIdContainer) {
        var nomTabSel = $("li.ui-tabs-tab.ui-corner-top.ui-state-default.ui-tab.ui-tabs-active.ui-state-active")[0].innerText;
        mixpanel.track("Click Gráfico",
            {
                "Sección": "Mis Busquedas",
                "Id": vIdContainer,
                "Tab": nomTabSel,
                "Tipo de Actividad": $(".content-form-mis-busquedas .radio-inline.control-label-withoutColor-vt :checked").parent()[0].innerText,
                "Región": $(".content-form-mis-busquedas #cboPais2").children("option:selected").text(),
                "País": $(".content-form-mis-busquedas #cboPais").children("option:selected").text(),
                "Importador": getValueFilterFromFilters("#lstFiltros", fieldNameImportador),
                "Exportador": getValueFilterFromFilters("#lstFiltros", fieldNameExportador),
                "Marca": getValueFilterFromFilters("#lstFiltros", fieldNameMarca),
                "Partida Aduanera": getValueFilterFromFilters("#lstFiltros", fieldNamePartidaAduanera),
                "Descripción Comercial": getValueFilterFromFilters("#lstFiltros", fieldNameDescComercial),
                "País Origen": getValueFilterFromFilters("#lstFiltros", fieldNamePaisOrigen),
                "Fecha Ini": getFtYearMonth($("input[name='cboDesde']").val()),
                "Fecha Fin": getFtYearMonth($("input[name='cboHasta']").val())
            });

    }
   
    // Funcion que devuelve la diferencia en meses de dos fechas
    function getDiffMonths(d1, d2) {
        return ((new Date(d2).getFullYear() - new Date(d1).getFullYear()) * 12) +
            (new Date(d2).getMonth() - new Date(d1).getMonth()) + 1;
    }

    // Funcion que devuelve el formato yyyy-MM segun la fecha ingresada
    function getFtYearMonth(d1) {
        return new Date(d1).getFullYear().toString() + "-" + (new Date(d1).getMonth() + 1).toString().padStart(2, "0");
    }

    // Funcion que devuelve la descripcion del rango de fechas
    function getRangeDescription(nroMeses) {
        var desc;
        switch (true) {
            case (nroMeses >= 1 && nroMeses <= 3):
                desc = "De 1 a 3 meses";
                break;
            case (nroMeses >= 3 && nroMeses <= 6):
                desc = "De 3 a 6 meses";
                break;
            case (nroMeses >= 6 && nroMeses <= 12):
                desc = "De 6 a 1 año";
                break;
            case (nroMeses >= 12 && nroMeses <= 24):
                desc = "De 1 a 2 años";
                break;
            case (nroMeses >= 24 && nroMeses <= 36):
                desc = "De 2 a 3 años";
                break;
            case (nroMeses >= 36):
                desc = "De 3 en adelante";
                break;
            default:
                desc = "";
                break;
        }
        return desc;
    }

    // Funcion que pemite obtener los valores por coma de un combo, recibe como parametro el 
    // nombre del control
    function getContentSelect(ctrlSelect) {
        var datos = "";
        $(ctrlSelect).find('option').each(function (index, element) {
             datos=datos + element.text + "|";
        });
        return datos;
    }

    // Funcion obtener el dato por el nombre del filtros desde el control filtros
    function getValueFilterFromFilters(ctrlSelect,fieldName) {
        var datoEncontrado = "";
        var datoWithoutBrackets = "";
        $(ctrlSelect).find('option').each(function (index, element) {
            datoWithoutBrackets = getFieldNameFromBrackets(element.text).toUpperCase();
            if (datoWithoutBrackets == fieldName.toUpperCase()) {
                datoEncontrado = getDataFromBrackets(element.text);
            }
        });
        return datoEncontrado;
    }

    // Funcion que pemite obtener el nombre del campo de filtros sin corchetes
    function getFieldNameFromBrackets(text) {
        var charBracketStart = "[";
        var charBracketEnd = "]";
        var posBracketStart = text.indexOf(charBracketStart);
        var posBracketEnd = text.indexOf(charBracketEnd);
        var textWithoutBrackets = text.substring(posBracketStart + 1, posBracketEnd);
        return textWithoutBrackets;
    }

    // Funcion que pemite obtener el nombre del campo de filtros sin corchetes
    function getDataFromBrackets(text) {
        var charBracketEnd = "]";
        var posBracketEnd = text.indexOf(charBracketEnd);
        var dataWithoutBrackets = text.substring(posBracketEnd + 1).trim();
        return dataWithoutBrackets;
    }

});