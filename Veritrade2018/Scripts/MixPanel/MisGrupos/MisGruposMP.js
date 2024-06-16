/* JANAQ 080620
 * Eventos capturados con mixpanel en la seccion mis grupos
*/

function MixPanel() { }

$(document).ready(function () {

    //Si se ingreso por url de referido (validacion para evitar trackear enlaces de universidades)
    if (getCookie("VLU") == "1") return;

    // Evento clic listar grupos configurados del usuario por tipo de registro
    $(document).on("click", "di#camposFiltros a.btn.boton-opcion", function () {
        mixpanel.track("Clic Listar Grupos Configurados del Usuario por Tipo de Registro",
            {
                "Sección": "Mis Grupos",
                "Tipo de Actividad Comercial": $(".radio-inline :checked").parent()[0].innerText,
                "Región": $("select#cboPais2").children("option:selected").text(),
                "País": $("select#cboPais").children("option:selected").text(),
                "Identificador de tipos de registros a listar como favoritos": $(this).attr("data-field")
            });
    });

    // Evento clic agregar favoritos a grupo
    $(document).on("click", "a.btn.boton-opcion.margin-height-sm", function () {
        mixpanel.track("Clic Agregar Favoritos a Grupo",
            {
                "Sección": "Mis Grupos"
            });
    });

    // Evento clic eliminar grupos seleccionados
    $(document).on("click", "button.btn.boton-delete.margin-height-sm.clickDeleteSelectedGroups", function () {

        var grupos = "";
        // Obtener grupos seleccionados
        $("tbody#tbodyAdminMyGroups").find("input:checked").parent().parent().find("label").each(function (i, e) {
            grupos = grupos + e.innerText + "|";
        });

        mixpanel.track("Clic Eliminar Grupos Seleccionados",
            {
                "Sección": "Mis Grupos",
                "Lista de grupos seleccionados para su eliminación": grupos
            });
    });
    
    // Evento clic volver
    $(document).on("click", "a.btn.boton-volver.margin-height-sm", function () {
        mixpanel.track("Clic Volver",
            {
                "Sección": "Mis Grupos"
            });
    });

    // Evento clic mostrar favoritos del grupo
    $(document).on("click", "a.lnkOperacionesColumnaTabla.lnkShowfavorites.clickShowFavorites", function () {
        var grupo = $(this).parent().parent().find("label")[0].innerText;
        mixpanel.track("Clic Mostrar Favoritos del Grupo",
            {
                "Sección": "Mis Grupos",
                "Identificador del grupo del cual se visualizará sus favoritos": grupo
            });
    });

    // Evento clic modificar grupo
    $(document).on("click", "a.lnkOperacionesColumnaTabla.lnkEditGroup.clickModifyGroup", function () {
        var grupo = $(this).parent().parent().find("label")[0].innerText;
        mixpanel.track("Clic Modificar Grupo",
            {
                "Sección": "Mis Grupos",
                "Identificador del grupo": grupo
            });
    });

    // Evento clic grabar modificacion de grupo
    $(document).on("click", "a.clickSaveUpdateGroup", function () {
        var grupo = $(this).parent().parent().parent().parent().find("label")[0].innerText;
        var grupoModificado = $(this).parent().find("input")[0].value;
        mixpanel.track("Clic Grabar Modificación de Grupo",
            {
                "Sección": "Mis Grupos",
                "Identificador del grupo": grupo,
                "Texto ingresado por el usuario": grupoModificado
            });
    });

    // Evento clic cancelar modificacion de grupo
    $(document).on("click", "a.clickCancelModifyGroup", function () {
        var grupo = $(this).parent().parent().parent().parent().find("label")[0].innerText;
        mixpanel.track("Clic Cancelar Modificación de Grupo",
            {
                "Sección": "Mis Grupos",
                "Identificador del grupo": grupo
            });
    });

    // Evento clic eliminar registro de grupo
    $(document).on("click", "button#btnDeleteFavorite", function () {

        var grupos = "";
        // Obtener grupos seleccionados
        $("tbody#tbodyFavorites").find("input:checked").parent().parent().find("[data-field='nomGrupo']").each(function (i, e) {
            grupos = grupos + e.innerText + "|";
        });

        mixpanel.track("Clic Eliminar Registro de Grupo",
            {
                "Sección": "Mis Grupos",
                "Lista de registros a retirar del grupo": grupos
            });
    });

    // Evento clic ir a seccion mis busquedas
    $(document).on("click", "a.btn.boton-volver", function () {
        mixpanel.track("Clic Ir a Sección Mis-Búsquedas",
            {
                "Sección": "Mis Grupos"
            });
    });

});