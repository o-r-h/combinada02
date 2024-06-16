
function TableAdminMyGroups() {
}

TableAdminMyGroups.RegisterPaging = function (urlPaging,
    pTotalPages,
    pVisiblePages) {

    $('#pagingAdminMyGroups').twbsPagination({
        totalPages: pTotalPages,
        visiblePages: pVisiblePages,
        hideOnlyOnePage: true,
        initiateStartPageClick: false,
        onPageClick: function (event, page) {

            var chkArray = [];
            $("#tbodyAdminMyGroups tr>td input[type=checkbox]:checked").each(function () {
                chkArray.push($(this).val());
            });
            var vRegistrosSeleccionados = chkArray.join(",");

            chkArray = [];
            $("#tbodyAdminMyGroups tr>td input[type=checkbox]").each(function () {
                chkArray.push($(this).val());
            });
            var vRegistrosPagina = chkArray.join(",");

            $.ajax({
                type: "POST",
                url: urlPaging,
                data: {
                    pagina: page,
                    idsSeleccionados: vRegistrosSeleccionados,
                    idsPagina: vRegistrosPagina
                },
                beforeSend: function () {
                    LoadingAdminPage.showOrHideLoadingPage(true);
                },
                success: function (response) {
                    $("#tbodyAdminMyGroups").html(response.rowsGroups);
                    //var wp = $("#table" + idPaging);
                    //setTimeout(AdminPage.applyDot(wp, 2, "right"), 500);
                    
                    LoadingAdminPage.showOrHideLoadingPage(false);
                },
                error: function (data) {
                    console.log(data);
                }
            });
        }
    });
}

TableAdminMyGroups.ShowFavorites = function(urlPostFavorites, pIdGroup, urlPagingFavorites) {
    $.ajax({
        type: "POST",
        url: urlPostFavorites,
        data: {
            idGrupo: pIdGroup
        },
        beforeSend: function () {
            LoadingAdminPage.showOrHideLoadingPage(true);
        },
        success: function (response) {

            $("#tableFavoritesByGroup").html(response.objFavoriteByGroup.FavoritesByGroupInHtml);

            if (response.objFavoriteByGroup.TotalPaginas > 0) {
                TableAdminMyGroups.RegisterPagingFavorites(urlPagingFavorites,
                    response.objFavoriteByGroup.TotalPaginas,
                    response.objFavoriteByGroup.CountVisiblePages);
            }

            LoadingAdminPage.showOrHideLoadingPage(false);
            ModalAdmin.registerShowByShowOption("ModalFavoritesByGroup", true);
            
        },
        error: function (dataError) {
            console.info(dataError);
        }
    });
}

TableAdminMyGroups.RegisterPagingFavorites = function (urlPaging,
    pTotalPages,
    pVisiblePages) {

    $('#pagingFavorites').twbsPagination({
        totalPages: pTotalPages,
        visiblePages: pVisiblePages,
        hideOnlyOnePage: true,
        initiateStartPageClick: false,
        onPageClick: function (event, page) {

            var chkArray = [];
            $("#tbodyFavorites tr>td input[type=checkbox]:checked").each(function () {
                chkArray.push($(this).val());
            });
            var vRegistrosSeleccionados = chkArray.join(",");

            chkArray = [];
            $("#tbodyFavorites tr>td input[type=checkbox]").each(function () {
                chkArray.push($(this).val());
            });
            var vRegistrosPagina = chkArray.join(",");

            $.ajax({
                type: "POST",
                url: urlPaging,
                data: {
                    pagina: page,
                    idsSeleccionados: vRegistrosSeleccionados,
                    idsPagina: vRegistrosPagina
                },
                beforeSend: function () {
                    LoadingAdminPage.showOrHideLoadingPage(true);
                },
                success: function (response) {
                    $("#tbodyFavorites").html(response.rowsFavoritesByGroup);
                    //var wp = $("#table" + idPaging);
                    //setTimeout(AdminPage.applyDot(wp, 2, "right"), 500);

                    LoadingAdminPage.showOrHideLoadingPage(false);
                },
                error: function (data) {
                    console.log(data);
                }
            });
        }
    });
}

TableAdminMyGroups.SaveUpdateGroup = function(urlUpdate, pIdGroup, pTextGroup) {
    $.ajax({
        type: "POST",
        url: urlUpdate,
        data: {
            idGroup: pIdGroup,
            textGroup : pTextGroup
        },
        beforeSend: function () {
            LoadingAdminPage.showOrHideLoadingPage(true);
        },
        success: function (response) {
            $("#tbodyAdminMyGroups").html(response.rowsGroups);
            LoadingAdminPage.showOrHideLoadingPage(false);
        },
        error: function (dataError) {
            console.info(dataError);
        }
    });
}

TableAdminMyGroups.DeleteFavorites = function(urlDelete, urlPagingFavorites) {
    var chkArray = [];
    $("#tbodyFavorites tr>td input[type=checkbox]:checked").each(function () {
        chkArray.push($(this).val());
    });
    var vRegistrosSeleccionados = chkArray.join(",");

    chkArray = [];
    $("#tbodyFavorites tr>td input[type=checkbox]").each(function () {
        chkArray.push($(this).val());
    });
    var vRegistrosPagina = chkArray.join(",");

    $.ajax({
        type: "POST",
        url: urlDelete,
        data: {
            idsSeleccionados: vRegistrosSeleccionados,
            idsPagina: vRegistrosPagina
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

                if (response.rowsGroups != "") {
                    $("#tbodyAdminMyGroups").html(response.rowsGroups);
                }

                if (response.objFavoriteByGroup != null) {
                    $("#tableFavoritesByGroup").html(response.objFavoriteByGroup.FavoritesByGroupInHtml);

                    if (response.objFavoriteByGroup.TotalPaginas > 0) {
                        TableAdminMyGroups.RegisterPagingFavorites(urlPagingFavorites,
                            response.objFavoriteByGroup.TotalPaginas,
                            response.objFavoriteByGroup.CountVisiblePages);
                    }

                    LoadingAdminPage.showOrHideLoadingPage(false);
                } else {
                    LoadingAdminPage.showOrHideLoadingPage(false);
                } 
            }
        },
        error: function (dataError) {
            console.log(dataError);
            //LoadingAdminPage.showOrHideLoadingPage(false);
        }
    });
}


function OptionAdminMyGroup() {
}

OptionAdminMyGroup.DeleteGroups = function (urlDelete, urlPaging) {

    var chkArray = [];
    $("#tbodyAdminMyGroups tr>td input[type=checkbox]:checked").each(function () {
        chkArray.push($(this).val());
    });
    var vRegistrosSeleccionados = chkArray.join(",");

    chkArray = [];
    $("#tbodyAdminMyGroups tr>td input[type=checkbox]").each(function () {
        chkArray.push($(this).val());
    });
    var vRegistrosPagina = chkArray.join(",");

    $.ajax({
        type: "POST",
        url: urlDelete,
        data: {
            idsSeleccionados: vRegistrosSeleccionados,
            idsPagina: vRegistrosPagina
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
                if (response.objAdminMyGroup != null) {
                    $("#tbodyAdminMyGroups").html(response.objAdminMyGroup.GroupsFavoritesInHtml);
                    $("#descQuantityGroups").text(response.objAdminMyGroup.DescripcionCantidad);

                    if (response.objAdminMyGroup.TotalPaginas > 0) {
                        $("#pagingAdminMyGroups").twbsPagination('destroy');
                        $("#divPagingAdminMyGroups").removeClass("no-display");

                        TableAdminMyGroups.RegisterPaging(urlPaging,
                            response.objAdminMyGroup.TotalPaginas,
                            response.objAdminMyGroup.CountVisiblePages);
                        LoadingAdminPage.showOrHideLoadingPage(false);
                    } else {
                        $("#pagingAdminMyGroups").twbsPagination('destroy');
                        $("#divPagingAdminMyGroups").addClass("no-display");
                        LoadingAdminPage.showOrHideLoadingPage(false);
                    }
                }
            }
        },
        error: function (dataError) {
            console.log(dataError);
            //LoadingAdminPage.showOrHideLoadingPage(false);
        }
    });
}