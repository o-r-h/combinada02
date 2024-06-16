function Size() {

}

Size.ValidationChild = function () {
    $.each($("#tbodyAdminMyFavorites tr td:nth-child(4)"), function (e) {
        var arreglo = [];
        $.each($("#tbodyAdminMyFavorites tr:nth-child(" + (e + 1) + ") td:nth-child(4) p"), function () {
            arreglo.push($(this).outerHeight())
        });
        var i = 0;
        $.each($("#tbodyAdminMyFavorites tr:nth-child(" + (e + 1) + ") td:nth-child(5) p"), function () {

            //console.log($(this).outerHeight() + "=>" + arreglo[i])
            $(this).css('height', arreglo[i]);
            i++;
        });
    });
}

function TableAdminMyFavorites() {
}
/**
 * 
 * @param {string} urlPaging: url del controller para obtener los datos
 * @param {any} pTotalPages: cantidad de páginas a generar según cantidad de datos
 * @param {any} pVisiblePages: cantidad de páginas visibles del paginador
 */
TableAdminMyFavorites.RegisterPaging = function (urlPaging,
    pTotalPages,
    pVisiblePages) {

    $('#pagingAdminMyFavorites').twbsPagination({
        totalPages: pTotalPages,
        visiblePages: pVisiblePages,
        hideOnlyOnePage: true,
        initiateStartPageClick: false,
        onPageClick: function (event, page) {

            var chkArray = [];
            $("#tbodyAdminMyFavorites tr>td input[type=checkbox]:checked").each(function () {
                chkArray.push($(this).val());
            });
            var vRegistrosSeleccionados = chkArray.join(",");

            chkArray = [];
            $("#tbodyAdminMyFavorites tr>td input[type=checkbox]").each(function () {
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
                    $("#tbodyAdminMyFavorites").html(response.rowsFavorites);
                    Size.ValidationChild();
                    //var wp = $("#table" + idPaging);
                    //setTimeout(AdminPage.applyDot(wp, 2, "right"), 500);
                    
                    $("#chkAllAdminMyFavorites").prop('checked', false);
                    LoadingAdminPage.showOrHideLoadingPage(false);
                },
                error: function (data) {
                    console.log(data);
                }
            });
        }
    });
}

TableAdminMyFavorites.LoadDataFiltered = function (objAdminMyFavorite, urlPaging) {
    $("#tbodyAdminMyFavorites").html(objAdminMyFavorite.RowsFavoritesUniquesInHtml);
    Size.ValidationChild();
    $("#descQuantityFavorites").text(objAdminMyFavorite.DescripcionCantidad);

    $("td [name='chekFavorites']").addClass("mt_m8");

    $("#chkAllAdminMyFavorites").prop('checked', false);
    if (objAdminMyFavorite.TotalPaginas > 1) {
        $("#pagingAdminMyFavorites").twbsPagination('destroy');
        $("#divPagingAdminMyFavorites").removeClass("no-display");

        TableAdminMyFavorites.RegisterPaging(urlPaging,
            objAdminMyFavorite.TotalPaginas,
            objAdminMyFavorite.CountVisiblePages);

        LoadingAdminPage.showOrHideLoadingPage(false);
    } else {
        $("#pagingAdminMyFavorites").twbsPagination('destroy');
        $("#divPagingAdminMyFavorites").addClass("no-display");
        LoadingAdminPage.showOrHideLoadingPage(false);
    }
}

TableAdminMyFavorites.SearchOrResetByCompany = function (urlSearch,
    pTextCompany,
    urlPaging) {
    $.ajax({
        type: "POST",
        url: urlSearch,
        data: {
            textCompany: pTextCompany
        },
        beforeSend: function () {
            LoadingAdminPage.showOrHideLoadingPage(true);
        },
        success: function (response) {
            TableAdminMyFavorites.LoadDataFiltered(response.objAdminMyFavorite,
                urlPaging);
        },
        error: function (dataError) {
            console.log(dataError);
            //LoadingAdminPage.showOrHideLoadingPage(false);
        }
    });
}

TableAdminMyFavorites.ChangeGruposF = function (urlPost,
    indexCbo,
    valueCbo,
    textCbo,
    urlPaging) {
    $.ajax({
        type: "POST",
        url: urlPost,
        data: {
            indexCboGruposF: indexCbo,
            valueCboGruposF: valueCbo,
            textCboGruposF: textCbo
        },
        beforeSend: function () {
            LoadingAdminPage.showOrHideLoadingPage(true);
        },
        success: function (response) {
            TableAdminMyFavorites.LoadDataFiltered(response.objAdminMyFavorite,
                urlPaging);
        },
        error: function (dataError) {
            console.log(dataError);
            //LoadingAdminPage.showOrHideLoadingPage(false);
        }
    });
}



function OptionAdminMyFavorite() {
}

OptionAdminMyFavorite.AddToGroup = function (urlAddToGroup) {
    var chkArray = [];

    $("#tbodyAdminMyFavorites tr>td input[type=checkbox]:checked").each(function () {
        chkArray.push($(this).val());
    });
    var vRegistrosSeleccionados = chkArray.join(",");

    chkArray = [];
    $("#tbodyAdminMyFavorites tr>td input[type=checkbox]").each(function () {
        chkArray.push($(this).val());
    });
    var vRegistrosPagina = chkArray.join(",");

    $.ajax({
        type: "POST",
        url: urlAddToGroup,
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
                LoadingAdminPage.showOrHideLoadingPage(false);

                ModalAdmin.registerShowByShowOption("ModalAddToGroup", true);
            }
        },
        error: function (dataError) {
            console.info(dataError);
        }
    });
}

OptionAdminMyFavorite.UpdateGroup = function (urlUpdate) {

    var vIsCheckedCrearGrupo = $("#rdbCreaGrupo").is(":checked");
    var vTextNewGroupo = "";
    if (vIsCheckedCrearGrupo) {
        vTextNewGroupo = $("#txtNuevoGrupo").val();
        if (vTextNewGroupo.trim() == "") {
            //$("#lblMsgAddToGroup").text("")
            $("#txtNuevoGrupo").focus();
            return;
        }
    }

    $.ajax({
        type: "POST",
        url: urlUpdate,
        data: {
            isCheckedCreateGroup: vIsCheckedCrearGrupo,
            textNewGroup: vTextNewGroupo,
            codGrupo: $("#cboGrupos").val()
        },
        beforeSend: function () {
            LoadingAdminPage.showOrHideLoadingPage(true);
        },
        success: function (response) {
            if (response.flagExisteGrupo) {
                LoadingAdminPage.showOrHideLoadingPage(false);
                ModalAdmin.showVentanaMensajeWithTitleAndMessage("ModalVentanaMensaje",
                    "messageTitle",
                    response.objMensaje.titulo,
                    "message",
                    response.objMensaje.mensaje,
                    "lnkContactenos",
                    response.objMensaje.flagContactenos);
            } else {
                $("#tbodyAdminMyFavorites").html(response.rowsFavorites);
                Size.ValidationChild();
                //var wp = $("#table" + idPaging);
                //setTimeout(AdminPage.applyDot(wp, 2, "right"), 500);
                
                if (response.itemsGruposFavoritos != null) {
                    var currentCodGrupoF = $("#cboGruposF").val();
                    AdminPage.loadDataComboBox(response.itemsGruposFavoritos, "cboGruposF");
                    AdminPage.loadDataComboBox(response.itemsSoloGruposFavoritos, "cboGrupos");
                    $("#cboGruposF").val(currentCodGrupoF);
                }

                $("#chkAllAdminMyFavorites").prop('checked', false);

                LoadingAdminPage.showOrHideLoadingPage(false);
                ModalAdmin.hide("ModalAddToGroup");
                OptionAdminMyFavorite.ResetModalAddToGroup();

                ModalAdmin.showVentanaMensajeWithTitleAndMessage("ModalVentanaMensaje",
                    "messageTitle",
                    response.objMensaje.titulo,
                    "message",
                    response.objMensaje.mensaje,
                    "lnkContactenos",
                    response.objMensaje.flagContactenos);
            }
        },
        error: function (dataError) {
            console.info(dataError);
        }
    });
}

OptionAdminMyFavorite.ResetModalAddToGroup = function () {
    $("#txtNuevoGrupo").val("");

    if ($("#cboGrupos > option").length > 0) {
        $('#rdbCreaGrupo').prop('checked', false);
        $('#rdbAgregaAGrupo').prop('checked', true);
        $("#rdbAgregaAGrupo").removeAttr("disabled");
        $("#cboGrupos").removeAttr("disabled");
        $('#cboGrupos :nth-child(1)').prop('selected', true);
    } else {
        $('#rdbCreaGrupo').prop('checked', true);
        $('#rdbAgregaAGrupo').prop('checked', false);
        $("#rdbAgregaAGrupo").attr("disabled", "disabled");
        $("#cboGrupos").attr("disabled", "disabled");
    }
}

OptionAdminMyFavorite.DeleteSelection = function (urlDelete, urlPaging) {

    var chkArray = [];
    $("#tbodyAdminMyFavorites tr>td input[type=checkbox]:checked").each(function () {
        chkArray.push($(this).val());
    });
    var vRegistrosSeleccionados = chkArray.join(",");

    chkArray = [];
    $("#tbodyAdminMyFavorites tr>td input[type=checkbox]").each(function () {
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
            if (response.objAdminMyFavorite != null) {
                TableAdminMyFavorites.LoadDataFiltered(response.objAdminMyFavorite,
                    urlPaging);
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
            }
        },
        error: function (dataError) {
            console.log(dataError);
            //LoadingAdminPage.showOrHideLoadingPage(false);
        }
    });
}
