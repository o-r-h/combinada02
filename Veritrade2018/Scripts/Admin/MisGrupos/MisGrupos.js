

function FiltrosMiPerfil() {
}

FiltrosMiPerfil.changeTipoOpe = function (urlPost,
    tipoOpe) {

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
            $("#camposFiltros").html("");
            $("#camposFiltros").html(response.myFavoritesAndMyGroups);

            LoadingAdminPage.showOrHideLoadingPage(false);
        },
        error: function (dataError) {
            console.info(dataError);
        }
    });
}

FiltrosMiPerfil.changePais2 = function (urlPost,
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
            $("#camposFiltros").html("");
            $("#camposFiltros").html(response.myFavoritesAndMyGroups);

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

FiltrosMiPerfil.changePais = function (urlPost, pCodPais, pTextCodPais, idCboMyFilters) {
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
            $("#camposFiltros").html("");
            $("#camposFiltros").html(response.myFavoritesAndMyGroups);

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