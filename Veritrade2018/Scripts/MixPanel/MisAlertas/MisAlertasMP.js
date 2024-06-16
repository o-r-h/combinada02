/* JANAQ 040620
 * Eventos capturados con mixpanel en la seccion mis alertas
*/

function MixPanel() { }

$(document).ready(function () {

    //Si se ingreso por url de referido (validacion para evitar trackear enlaces de universidades)
    if (getCookie("VLU") == "1") return;

    // Evento clic boton listar alertas configurados por mis registros
    $(document).on("click", "div#camposFiltros a.btn.boton-opcion", function () {
        mixpanel.track("Clic Botón Listar Alertas Configurados por mis Registros",
            {
                "Sección": "Mis Alertas",
                "Tipo de Actividad": $(".radio-inline :checked").parent()[0].innerText,
                "Región": $("select#cboPais2").children("option:selected").text(),
                "País": $("select#cboPais").children("option:selected").text(),
                "Identificador de Mis Registros Seleccionado": $(this).attr("data-field")
            });
    });

    // Evento clic ir a seccion mis busquedas
    $(document).on("click", "a.btn.boton-volver", function () {
        mixpanel.track("Clic Ir a Sección Mis-Búsquedas",
            {
                "Sección": "Mis Alertas"
            });
    });

    // Evento clic boton configurar alerta
    $(document).on("click", "button#btnActualizarE", function () {
        mixpanel.track("Clic Botón Configurar Alerta",
            {
                "Sección": "Mis Alertas"
            });
    });

    // Evento clic boton volver
    $(document).on("click", "a#Volver", function () {
        mixpanel.track("Clic Botón Volver",
            {
                "Sección": "Mis Alertas"
            });
    });

    // Evento actualizar alertas
    $(document).on("click", "button#btnActualizar", function () {

        var alertas = "";
        // Obtener alertas seleccionadas
        $("tbody#tbodyPreference").find("input:checked").parent().parent().parent().parent().find("[data-field='Nandina']").each(function (i, e) {
            alertas = alertas + e.innerText + "|";
        });

        mixpanel.track("Clic Actualizar Alertas",
            {
                "Sección": "Mis Alertas",
                "Lista de ítems configurados en la alerta": alertas
            });
    });

    // Evento clic cancelar
    $(document).on("click", "button#btnCancelar", function () {
        mixpanel.track("Clic Cancelar",
            {
                "Sección": "Mis Alertas"
            });
    });

    // Evento clic agregar
    $(document).on("click", "button#btnActualizarPCE.btn.boton-opcion", function () {
        mixpanel.track("Clic Agregar",
            {
                "Sección": "Mis Alertas"
            });
    });

    // Evento clic editar alerta
    $(document).on("click", "img.icon-detail", function () {
        var alerta = $(this).parent().parent().parent().find("[data-field='Nandina']")[0].innerText;
        mixpanel.track("Clic Editar Alerta",
            {
                "Sección": "Mis Alertas",
                "Identificador de la alerta": alerta
            });
    });

    // Evento seleccionar compañia - seleccionar producto
    $("#cboProductOrCompany2").on("change", function () {
        if ($(this).children("option:selected").text() == "") return;
        var etiquetaCombo = $(this).parent().parent().find("label")[0].innerText;
        var textoCompania = "COMPAÑÍA";
        var textoProducto = "PRODUCTO";

        if (etiquetaCombo.toUpperCase() == textoCompania) {
            mixpanel.track("Seleccionar compañía",
                {
                    "Sección": "Mis Alertas",
                    "Identificador de la compañía seleccionada": $(this).children("option:selected").text()
                });
        } else {
            mixpanel.track("Seleccionar producto",
                {
                    "Sección": "Mis Alertas",
                    "Identificador del producto seleccionado": $(this).children("option:selected").text()
                });
        }
 
    });


    // Evento eliminar alerta
    $(document).on("click", "button#btnDelete", function () {
        mixpanel.track("Eliminar Alerta",
            {
                "Sección": "Mis Alertas",
                "Identificador de la alerta": $("tbody#tbodyPreference").find("[data-field='Nandina']")[0].innerText
            });
    });
    
    
});