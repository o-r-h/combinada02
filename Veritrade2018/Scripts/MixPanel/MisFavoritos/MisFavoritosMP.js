/* JANAQ 090620
 * Eventos capturados con mixpanel en la sección mis favoritos
*/

function MixPanel() { }

$(document).ready(function () {

    //Si se ingreso por url de referido (validacion para evitar trackear enlaces de universidades)
    if (getCookie("VLU") == "1") return;

    // Evento clic listar favoritos configurados del usuario
    $(document).on("click", "di#camposFiltros a.btn.boton-opcion", function () {
        mixpanel.track("Clic Listar Favoritos Configurados del Usuario",
            {
                "Sección": "Mis Favoritos",
                "Tipo de Actividad Comercial": $(".radio-inline :checked").parent()[0].innerText,
                "Región": $("select#cboPais2").children("option:selected").text(),
                "País": $("select#cboPais").children("option:selected").text(),
                "Identificador de tipos de registros a listar como favoritos": $(this).attr("data-field")
            });
    });

    // Evento clic agregar nuevo registro
    $(document).on("click", "a[data-field='nomBtnAddNewProduct']", function () {
        mixpanel.track("Clic Agregar Nuevo Registro",
            {
                "Sección": "Mis Favoritos",
                "Tipo de Registro": $(this)[0].innerText
            });
    });

    // Evento clic agregar seleccionado a grupo
    $(document).on("click", "button.btn.boton-opcion.margin-height-sm.btnAddToGroup", function () {
        mixpanel.track("Clic Agregar Seleccionado a Grupo",
            {
                "Sección": "Mis Favoritos"
            });
    });

    // Evento clic eliminar seleccionados
    $(document).on("click", "button.btn.boton-delete.margin-height-sm.btnDeleteSelection", function () {
        var grupos = "";
        // Obtener grupos seleccionados
        $("tbody#tbodyAdminMyProducts").find("input:checked").parent().parent().parent().find("[data-field='nomGrupo']").each(function (i, e) {
            grupos = grupos + e.innerText + "|";
        });

        mixpanel.track("Clic Eliminar Seleccionados",
            {
                "Sección": "Mis Favoritos",
                "Lista de registros a eliminar": grupos
            });
    });

    // Evento clic actualizar descripcion personalizada del registro
    $(document).on("click", "a.lnkActualizarPartidaFav", function () {
        var grupo = $(this).parent().parent().find("[data-field='nomGrupo']")[0].innerText;
        var grupoModificado = $(this).parent().find("input")[0].value;

        mixpanel.track("Clic Actualizar Descripción Personalizada del Registro",
            {
                "Sección": "Mis Favoritos",
                "Identificador del Registro": grupo,
                "Texto Personalizado para el Usuario": grupoModificado
            });
    });

    // Evento clic ir a seccion mis busquedas
    $(document).on("click", "a.btn.boton-volver", function () {
        mixpanel.track("Clic Ir a Sección Mis-Búsquedas",
            {
                "Sección": "Mis Favoritos"
            });
    });

});