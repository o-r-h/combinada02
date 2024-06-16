/* JANAQ 080620
 * Eventos capturados con mixpanel en la sección mis plantillas
*/

function MixPanel() { }

$(document).ready(function () {

    //Si se ingreso por url de referido (validacion para evitar trackear enlaces de universidades)
    if (getCookie("VLU") == "1") return;

    // Evento seleccionar plantilla
    $("select#cboDescargas").on("change", function () {
        mixpanel.track("Seleccionar Plantilla",
            {
                "Sección": "Mis Plantillas",
                "Identificador Plantilla Seleccionada": $(this).children("option:selected").text()
            });
    });

    // Evento clic nueva plantilla
    $(document).on("click", "button#btnNewTemplate", function () {
        mixpanel.track("Clic Nueva Plantilla",
            {
                "Sección": "Mis Plantillas"
            });
    });

    // Evento clic grabar plantilla
    $(document).on("click", "button#saveTemplate", function () {
        var camposPlantilla = "";
        // Obtener productos seleccionados
        $("tbody#tbodyTemplate").find("input:checked").parent().parent().parent().parent().find(".valueField").each(function (i, e) {
            camposPlantilla = camposPlantilla + e.innerText + "|";
        })

        mixpanel.track("Clic Grabar Plantilla",
            {
                "Sección": "Mis Plantillas",
                "Nombre de la plantilla": $("input#txtTemplate").val(),
                'Check "Por Defecto"': $("input#chkDefault")[0].checked,
                "Lista de Campos de la Plantilla": camposPlantilla
            });
    });
    
    // Evento clic cancelar
    $(document).on("click", "button#cancelTemplate", function () {
        mixpanel.track("Clic Cancelar",
            {
                "Sección": "Mis Plantillas"
            });
    });

    // Evento clic eliminar plantilla
    $(document).on("click", "button#deleteTemplate", function () {
        mixpanel.track("Clic Eliminar Plantilla",
            {
                "Sección": "Mis Plantillas",
                "Identificador de la plantilla seleccionada": $("select#cboDescargas").children("option:selected").text()
            });
    });

    // Evento clic ir a seccion mis busquedas
    $(document).on("click", "a.btn.boton-volver", function () {
        mixpanel.track("Clic Ir a Sección Mis-Búsquedas",
            {
                "Sección": "Mis Plantillas"
            });
    });

});