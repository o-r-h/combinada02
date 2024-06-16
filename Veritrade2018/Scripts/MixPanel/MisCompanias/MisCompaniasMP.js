/* JANAQ 140620
 * Eventos capturados con mixpanel en la sección mis compañias
*/

function MixPanel() { }

$(document).ready(function () {

    //Tipo de Actividad
    var tipoActividadImportaciones = 'I';
    var tipoActividadExportaciones = 'E';

    // Filtros
    var fieldNamePaisOrigen = "País Origen";
    var fieldNamePaisDestino = "País Destino";
    var fieldNamePartidaAduanera = "Partida";
    var fieldNameImportador = "Importador";
    var fieldNameExportador = "Exportador";
    var fieldNameMarca = "Marca";
    var fieldNameDescComercial = "Desc. Comercial";
    var fieldNameComp = "Mis Compañias";


    //Si se ingreso por url de referido (validacion para evitar trackear enlaces de universidades)
    if (getCookie("VLU") == "1") return;

    // Evento clic ver graficos
    $(document).on("click", "div.padding-height-sm button#btnVerGraficos", function () {
        var periodo = $("div.padding-height-sm #cboOpcion").children("option:selected").text();
        var textoMeses = "meses";
        var textoAnios = "años";
        var diffMonths = 0;
        var fecIni = "";
        var fecFin = "";

        if (periodo.toLocaleLowerCase() == textoMeses) {
            diffMonths = getDiffMonths($("input[name='cboAnioMesIni']").val(), $("input[name='cboAnioMesFin']").val());
            fecIni = getFtYearMonth($("input[name='cboAnioMesIni']").val());
            fecFin = getFtYearMonth($("input[name='cboAnioMesFin']").val());
        } else if (periodo.toLocaleLowerCase() == textoAnios) {
            fecIni = $("input[name='cboAnioIni']").val();
            fecFin = $("input[name='cboAnioFin']").val();
        } else {
            fecIni = "";
            fecFin = getFtYearMonth($("input[name='cboAnioMesFin']").val());
        }

        mixpanel.track("Clic Ver Gráficos",
            {
                "Sección": "Mis Compañías",
                "Tipo de actividad": $("div.padding-height-sm label.radio-inline :checked").parent()[0].innerText,
                "Región": $("div.padding-height-sm #cboPais2").children("option:selected").text(),
                "País": $("div.padding-height-sm #cboPais").children("option:selected").text(),
                "Compañía": $("div.padding-height-sm #cboMyFilters").children("option:selected").text(),
                "Criterio de análisis": $("div.padding-height-sm div.typeUnit-analyzeIn label.radio-inline :checked").parent()[0].innerText,
                "Periodo": periodo,
                "Fecha Ini": fecIni,
                "Fecha Fin": fecFin,
                "Intervalo Meses": diffMonths,
                "Rango de fechas": getRangeDescription(diffMonths)
            });
    });

    // Evento clic video tutorial
    $(document).on("click", "button.btn.btn-video.blink-button", function () {
        mixpanel.track("Clic Video Tutorial",
            {
                "Sección": "Mis Compañías"
            });
    });

    // Evento clic descargar reporte en formato excel
    $(document).on("click", "img.ico_excel", function () {
        var idPlantilla = $("select#cboDescargas").children("option:selected").text();

        var periodo = $("div.padding-height-sm #cboOpcion").children("option:selected").text();
        var textoMeses = "meses";
        var textoAnios = "años";
        var diffMonths = 0;
        var fecIni = "";
        var fecFin = "";

        if (periodo.toLocaleLowerCase() == textoMeses) {
            diffMonths = getDiffMonths($("input[name='cboAnioMesIni']").val(), $("input[name='cboAnioMesFin']").val());
            fecIni = getFtYearMonth($("input[name='cboAnioMesIni']").val());
            fecFin = getFtYearMonth($("input[name='cboAnioMesFin']").val());
        } else if (periodo.toLocaleLowerCase() == textoAnios) {
            fecIni = $("input[name='cboAnioIni']").val();
            fecFin = $("input[name='cboAnioFin']").val();
        } else {
            fecIni = "";
            fecFin = getFtYearMonth($("input[name='cboAnioMesFin']").val());
        }

            mixpanel.track("Clic Descargar Reporte en Formato Excel",
                {
                    "Sección": "Mis Compañías",
                    "Identificador de Plantilla": idPlantilla,
                    "Tipo de actividad": $("div.padding-height-sm label.radio-inline :checked").parent()[0].innerText,
                    "Región": $("div.padding-height-sm #cboPais2").children("option:selected").text(),
                    "País": $("div.padding-height-sm #cboPais").children("option:selected").text(),
                    "Compañía": $("div.padding-height-sm #cboMyFilters").children("option:selected").text(),
                    "Criterio de análisis": $("div.padding-height-sm div.typeUnit-analyzeIn label.radio-inline :checked").parent()[0].innerText,
                    "Periodo": $("div.padding-height-sm #cboOpcion").children("option:selected").text(),
                    "Fecha Ini": fecIni,
                    "Fecha Fin": fecFin,
                    "Intervalo Meses": diffMonths,
                    "Rango de fechas": getRangeDescription(diffMonths)
                });
    });

    // Evento seleccionar tipo de actividad
    $("div.padding-height-sm label.radio-inline:lt(2)").on("change", function () {
        mixpanel.track("Clic Tipo de Actividad",
            {
                "Sección": "Mis Compañías",
                "Tipo de Actividad": $(this)[0].innerText
            });
    });

    // Evento seleccionar region
    $("div.padding-height-sm #cboPais2").on("change", function () {
        mixpanel.track("Seleccionar Region",
            {
                "Sección": "Mis Compañías",
                "Región Seleccionada": $(this).children("option:selected").text()
            });
    });

    // Evento seleccionar pais
    $("div.padding-height-sm #cboPais").on("change", function () {
        mixpanel.track("Seleccionar Pais",
            {
                "Sección": "Mis Compañías",
                "Región Seleccionada": $("div.padding-height-sm #cboPais2").children("option:selected").text(),
                "País Seleccionado": $(this).children("option:selected").text()
            });
    });

    // Evento seleccionar compañia
    $(document).on("change", "div.padding-height-sm #cboMyFilters", function () {
        mixpanel.track("Seleccionar Compañia",
            {
                "Sección": "Mis Compañías",
                "Tipo de actividad": $("div.padding-height-sm label.radio-inline :checked").parent()[0].innerText,
                "Producto Seleccionado": $(this).children("option:selected").text()
            });
    });

    // Evento seleccionar moneda a analizar
    $("div.padding-height-sm div.typeUnit-analyzeIn label.radio-inline").on("change", function () {
        mixpanel.track("Clic Moneda a Analizar",
            {
                "Sección": "Mis Compañías",
                "Moneda a Analizar": $(this)[0].innerText
            });
    });

    // Evento Click Exportar/Imprimir grafico
    MixPanel.exportarGrafico = function (vAccionSeleccionada, vIdContainer) {
        var periodo = $("div.padding-height-sm #cboOpcion").children("option:selected").text();
        var textoMeses = "meses";
        var textoAnios = "años";
        var diffMonths = 0;
        var fecIni = "";
        var fecFin = "";

        if (periodo.toLocaleLowerCase() == textoMeses) {
            diffMonths = getDiffMonths($("input[name='cboAnioMesIni']").val(), $("input[name='cboAnioMesFin']").val());
            fecIni = getFtYearMonth($("input[name='cboAnioMesIni']").val());
            fecFin = getFtYearMonth($("input[name='cboAnioMesFin']").val());
        } else if (periodo.toLocaleLowerCase() == textoAnios) {
            fecIni = $("input[name='cboAnioIni']").val();
            fecFin = $("input[name='cboAnioFin']").val();
        } else {
            fecIni = "";
            fecFin = getFtYearMonth($("input[name='cboAnioMesFin']").val());
        }

        mixpanel.track("Click Exportar/Imprimir Gráfico",
            {
                "Sección": "Mis Compañías",
                "Id": vIdContainer,
                "Acción Seleccionada": vAccionSeleccionada,
                "Tipo de actividad": $("div.padding-height-sm label.radio-inline :checked").parent()[0].innerText,
                "Región": $("div.padding-height-sm #cboPais2").children("option:selected").text(),
                "País": $("div.padding-height-sm #cboPais").children("option:selected").text(),
                "Compañía": $("div.padding-height-sm #cboMyFilters").children("option:selected").text(),
                "Criterio de análisis": $("div.padding-height-sm div.typeUnit-analyzeIn label.radio-inline :checked").parent()[0].innerText,
                "Periodo": periodo,
                "Fecha Ini": fecIni,
                "Fecha Fin": fecFin,
                "Intervalo Meses": diffMonths,
                "Rango de fechas": getRangeDescription(diffMonths)
            });

    }

    // Evento Click Grafico
    MixPanel.clickGrafico = function (vIdContainer) {
        var periodo = $("div.padding-height-sm #cboOpcion").children("option:selected").text();
        var textoMeses = "meses";
        var textoAnios = "años";
        var diffMonths = 0;
        var fecIni = "";
        var fecFin = "";

        if (periodo.toLocaleLowerCase() == textoMeses) {
            diffMonths = getDiffMonths($("input[name='cboAnioMesIni']").val(), $("input[name='cboAnioMesFin']").val());
            fecIni = getFtYearMonth($("input[name='cboAnioMesIni']").val());
            fecFin = getFtYearMonth($("input[name='cboAnioMesFin']").val());
        } else if (periodo.toLocaleLowerCase() == textoAnios) {
            fecIni = $("input[name='cboAnioIni']").val();
            fecFin = $("input[name='cboAnioFin']").val();
        } else {
            fecIni = "";
            fecFin = getFtYearMonth($("input[name='cboAnioMesFin']").val());
        }

        mixpanel.track("Click Gráfico",
            {
                "Sección": "Mis Compañías",
                "Id": vIdContainer,
                "Tipo de actividad": $("div.padding-height-sm label.radio-inline :checked").parent()[0].innerText,
                "Región": $("div.padding-height-sm #cboPais2").children("option:selected").text(),
                "País": $("div.padding-height-sm #cboPais").children("option:selected").text(),
                "Compañía": $("div.padding-height-sm #cboMyFilters").children("option:selected").text(),
                "Criterio de análisis": $("div.padding-height-sm div.typeUnit-analyzeIn label.radio-inline :checked").parent()[0].innerText,
                "Periodo": periodo,
                "Fecha Ini": fecIni,
                "Fecha Fin": fecFin,
                "Intervalo Meses": diffMonths,
                "Rango de fechas": getRangeDescription(diffMonths)
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

    // Funcion que devuelve el formato yyyy segun la fecha ingresada
    function getFtYear(d1) {
        return new Date(d1).getFullYear().toString();
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

    // Funcion obtener el dato por el nombre del filtros desde el control filtros
    function getValueFilterFromFilters(ctrlSelect, fieldName) {
        var datoEncontrado = "";
        var datoWithoutBrackets = "";
        $(ctrlSelect).find('option').each(function (index, element) {
            datoWithoutBrackets = getFieldNameFromBrackets(element.text).toUpperCase();
            console.log("datoWithoutBrackets");
            console.log(datoWithoutBrackets);
            //if (datoWithoutBrackets == fieldName.toUpperCase()) {
            //    datoEncontrado = getDataFromBrackets(element.text);
            //}
        });
        return datoEncontrado;
    }

    // Evento clic más informacion ventana emergente mis importaciones
    $(document).on("click", ".j_logow a", function () {
        mixpanel.track("Clic Mas Información ",
            {
                "Sección": "Mis Compañías",
                "País": $("#cboPais").children("option:selected").text()
            });
    });

    // Evento clic calificacion de la empresa ventana emergente mis importaciones
    $(document).on("click", "span.j_link-text", function () {
        mixpanel.track("Clic Calificación de la Empresa ",
            {
                "Sección": "Mis Compañías"
            });
    });

    //Clic Detalle Partida Aduanera
    $(document).on("click", ".verDetallePartidas", function () {
        mixpanel.track("Clic Detalle Partida Aduanera",
            {
                "Sección": "Mis Compañías",
                "País Consultado": $("#cboPais").children("option:selected").text()
            });
        //var tipoActividadSel = $(".radio-inline :checked").val();
        //if (tipoActividadSel == tipoActividadImportaciones) {
        //    mixpanel.track("Clic Detalle Partida Aduanera",
        //        {
        //            "Sección": "Mis Compañías",
        //            "Tipo de Actividad": $(".radio-inline :checked").parent()[0].innerText,
        //            "Región": $("#cboPais2").children("option:selected").text(),
        //            "País": $("#cboPais").children("option:selected").text(),
        //            "Compañias": $("#cboMyFilters").children("option:selected").text(),
        //            "Período": $("#cboOpcion").children("option:selected").text()
        //        });
    });

    // Evento clic para ir al sitio web de Sunat
    $(document).on("click", ".sunat", function () {
        mixpanel.track("Clic para ir a sitio de Sunat ",
            {
                "Sección": "Mis Compañías",
                "País Consultado": $("#cboPais").children("option:selected").text()
            });
    });
});