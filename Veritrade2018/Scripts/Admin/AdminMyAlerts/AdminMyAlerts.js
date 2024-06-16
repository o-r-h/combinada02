function LoadingAdminPage() {
}

LoadingAdminPage.showOrHideLoadingPage = function (pEstado) {
    if (pEstado) {
        $("#loadingPageAdmin").addClass("is-active-loading");
    } else {
        $("#loadingPageAdmin").removeClass("is-active-loading");
    }
}


LoadingAdminPage.LoadMethodsEdit = function(url) {

    $("#myTabla tbody tr").click(function () {
        var id = $(this).find(".icon-detail").attr("id");
        $("#TableView table tr").removeClass("highlight-row");
        $(this).closest("tr").addClass("highlight-row");
        //console.log($(this).attr("id"));
        $("#btnActualizarPCE").trigger("click");
        $("#cboProductOrCompany").prop("disabled", true);
        $("#btnDelete").removeClass("no-display");
        //VerDetalleFavorito('@Url.Action("VerDetalleFavorito", "AdminMisAlertasFavoritas")', id, true);
        VerDetalleFavorito(url, id, true);
    });


    $(".icon-detail").click(function () {
        $("#TableView table tr").removeClass("highlight-row");
        $(this).closest("tr").addClass("highlight-row");
        //console.log($(this).attr("id"));
        $("#btnActualizarPCE").trigger("click");
        $("#btnDelete").removeClass("no-display");
        $("#cboProductOrCompany").prop("disabled", true);
        //VerDetalleFavorito('@Url.Action("VerDetalleFavorito", "AdminMisAlertasFavoritas")', $(this).attr("id"), true);
        VerDetalleFavorito(url, $(this).attr("id"), true);
    });
}

function MyAlert() {

}
MyAlert.DeleteAlerts = function (culture, urlDelete, txtIndex, message , urlFavorito) {
    $.ajax({
        type: "POST",
        url: urlDelete,
        data: {
            culture: culture,
            idSeleccionado: txtIndex
        },
        beforeSend: function () {
            LoadingAdminPage.showOrHideLoadingPage(true);
        },
        success: function (response) {
            //let limiteAlerta = response.limiteAlerta;
            $("#idLimitAlert").val(response.limiteAlerta)
            console.log(response.limiteAlerta)
            if (response.viewTableView != "") {
                $('#btnActualizarPCE').html(response.nameBotonPC);
                $("#TableCombined").html(response.viewTableView);
                $('#SearchDataPC').addClass("no-display");
                $('#Title2PC').addClass("no-display");
                $('#ButtonsPC').addClass("no-display");
                $('#btnActualizarPCE').removeClass("no-display");
                $('#Volver').removeClass("no-display");                
                $('.st-ale').html(response.cantidadAlertas);
                $('#cboProductOrCompany').trigger('change');
                $("#cboProductOrCompany").prop("disabled", false);
                LoadingAdminPage.LoadMethodsEdit(urlFavorito);
            }
            ModalAdmin.showVentanaMensajeWithTitleAndMessage("ModalVentanaMensaje",
                "messageTitle",
                "Veritrade",
                "message",
                message,
                "lnkContactenos",
                false);
            LoadingAdminPage.showOrHideLoadingPage(false);
        },
        error: function (dataError) {
            console.log(dataError);
            LoadingAdminPage.showOrHideLoadingPage(false);
        }
    });
}
MyAlert.DeleteAlertsDetail = function (urlDelete, culture, idFavorito, descripcion, idSeleccionado) {
    $.ajax({
        type: "POST",
        url: urlDelete,
        data: {
            culture,
            idFavorito,
            idSeleccionado,
            descripcion
        },
        beforeSend: function () {
            LoadingAdminPage.showOrHideLoadingPage(true);
        },
        success: function (response) {
            //console.log(response.nuevoFiltro);
            $("select[name='lstFiltros']").html("");
            var version = navigator.appVersion.toLowerCase();

            var selected = "";

            if (version.indexOf("android") > -1)
                selected = "selected";

            $("#filtros").removeClass('no-display');
            if (response.nuevoFiltro == 0) {
                $("#filtros").addClass('no-display');
            }

            $.each(response.nuevoFiltro,
                function (index, val) {
                    $("select[name='lstFiltros']")
                        .append("<option value='" + val.value + "' " + selected + " >" + val.text + "</option>");
                });
            Filtros.adjustSizeHeightFilters();
            LoadingAdminPage.showOrHideLoadingPage(false);
        },
        error: function (dataError) {
            console.log(dataError);
            LoadingAdminPage.showOrHideLoadingPage(false);
        }
    });
}
MyAlert.InsertAlerts = function (idioma, urlInsert,
    txtIndex, idValorPadre, urlFavorito) {

    $.ajax({
        type: "POST",
        url: urlInsert,
        data: {
            culture: idioma,
            idFavorito: txtIndex,
            idValorPadre
        },
        beforeSend: function () {
            LoadingAdminPage.showOrHideLoadingPage(true);
        },
        success: function (response) { 

            $("#idLimitAlert").val(response.limiteAlerta)
            console.log(response.limiteAlerta)
            console.log(response)
            
            if (response.viewTableView != "") {
                $("#TableCombined").html(response.viewTableView);
                
                $('#btnActualizarPCE').html(response.nameBotonPC);
                $('#SearchDataPC').addClass("no-display");
                $('#Title2PC').addClass("no-display");
                $('#ButtonsPC').addClass("no-display");
                $('#btnActualizarPCE').removeClass("no-display");
                $('#lstFiltros').removeClass("ocultar");
                $('#Volver').removeClass("no-display");
                $('.st-ale').html(response.cantidadAlertas);
                LoadingAdminPage.LoadMethodsEdit(urlFavorito);
                ModalAdmin.showVentanaMensajeWithTitleAndMessage("ModalVentanaMensaje",
                    "messageTitle",
                    response.objMensaje.titulo,
                    "message",
                    response.objMensaje.mensaje,
                    "lnkContactenos",
                    response.objMensaje.flagContactenos);
            }
            else {
                ModalAdmin.showVentanaMensajeWithTitleAndMessage("ModalVentanaMensaje",
                    "messageTitle",
                    response.objMensaje.titulo,
                    "message",
                    response.objMensaje.mensaje,
                    "lnkContactenos",
                    response.objMensaje.flagContactenos);

            }
            //console.log(response.listaMyFavourites);
            if (response.listaMyFavourites != null && response.listaMyFavourites.length > 0) {
                //console.log(response.listaMyFavourites);
                $("#lstFiltros").removeClass("ocultar");
                $("select[name='lstFiltros']").html("");
                var version = navigator.appVersion.toLowerCase();
                $('.st-ale').html(response.cantidadAlertas);
                var selected = "";

                if (version.indexOf("android") > -1)
                    selected = "selected";
                $.each(response.listaMyFavourites,
                    function (index, val) {
                        //console.log(val.Text);
                        $("select[name='lstFiltros']")
                            .append("<option value='" + val.Value + "' " + selected + " >" + val.Text + "</option>");
                    });
                Filtros.adjustSizeHeightFilters();
            } else {
                //$("#lstFiltros").addClass("ocultar");
            }
            //alert("Grabado con exito");
            LoadingAdminPage.showOrHideLoadingPage(false);
        },
        error: function (dataError) {
            console.log(dataError);
            LoadingAdminPage.showOrHideLoadingPage(false);
        }
    });
}


function Filtros() {
}

Filtros.adjustSizeHeightFilters = function () {
    $('select#lstFiltros').each(function () {
        $(this).attr('size', $(this).find('option').length);
    });
    //console.log($("#lstFiltros").prop("size"))
}


function FiltrosMyAlert() {
}

FiltrosMyAlert.changePais2 = function (urlPost,
    pCodPais2) {

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
            AdminPage.loadDataComboBox(response.objMiPerfil.ListItemsPais, "cboPais");

            if (response.objMensaje != null) {
                $("#cboPais2").val(response.objMiPerfil.CodPais2Selected);
                $("#cboPais").val(response.objMiPerfil.CodPaisSelected);

                LoadingAdminPage.showOrHideLoadingPage(false);
                ModalAdmin.showVentanaMensajeWithTitleAndMessage("ModalVentanaMensaje",
                    "messageTitle",
                    response.objMensaje.titulo,
                    "message",
                    response.objMensaje.mensaje,
                    "lnkContactenos",
                    response.objMensaje.flagContactenos);
            } else {
                $("#cboPais").val(response.objMiPerfil.CodPaisSelected);
                LoadingAdminPage.showOrHideLoadingPage(false);
            }
        },
        error: function (dataError) {
            console.info(dataError);
        }
    });
}

FiltrosMyAlert.changePais = function (urlPost, pCodPais, pTextCodPais, idCboMyFilters) {
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

            if (response.objMensaje != null) {
                $("#cboPais2").val(response.objMiPerfil.CodPais2Selected);
                $("#cboPais").val(response.objMiPerfil.CodPaisSelected);

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