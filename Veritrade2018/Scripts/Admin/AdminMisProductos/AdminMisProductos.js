function Size() {

}

Size.ValidationChild = function () {
    $.each($("#tbodyAdminMyProducts tr td:nth-child(5)"), function (e) {
        var arreglo = [];
        $.each($("#tbodyAdminMyProducts tr:nth-child(" + (e + 1) + ") td:nth-child(5) p"), function () {
            arreglo.push($(this).outerHeight())
        });
        var i = 0;
        $.each($("#tbodyAdminMyProducts tr:nth-child(" + (e + 1) + ") td:nth-child(6) p"), function () {

            //console.log($(this).outerHeight() + "=>" + arreglo[i])
            $(this).css('height', arreglo[i]);
            i++;
        });
    });
}

function TableAdminMyProducts() {
}

TableAdminMyProducts.SearchOrResetByNandina = function (urlSearch,
    txtNandinaF
    , urlPaging,
    urlActualizar) {

    $.ajax({
        type: "POST",
        url: urlSearch,
        data: {
            textNandina: txtNandinaF
        },
        beforeSend: function () {
            LoadingAdminPage.showOrHideLoadingPage(true);
        },
        success: function (response) {
            TableAdminMyProducts.LoadDataFiltered(response.objAdminMyProduct,
                urlPaging,
                urlActualizar);
        },
        error: function (dataError) {
            console.log(dataError);
            //LoadingAdminPage.showOrHideLoadingPage(false);
        }
    });
}

TableAdminMyProducts.SearchOrResetByPartida = function (urlSearch,
    txtPartidaF
    , urlPaging,
    urlActualizar ) {
    $.ajax({
        type: "POST",
        url: urlSearch,
        data: {
            textPartida: txtPartidaF
        },
        beforeSend: function () {
            LoadingAdminPage.showOrHideLoadingPage(true);
        },
        success: function (response) {
            TableAdminMyProducts.LoadDataFiltered(response.objAdminMyProduct,
                urlPaging,
                urlActualizar);
        },
        error: function (dataError) {
            console.log(dataError);
            //LoadingAdminPage.showOrHideLoadingPage(false);
        }
    });
}

TableAdminMyProducts.ChangeGruposF = function (urlPost,
    indexCbo,
    valueCbo,
    textCbo,
    urlPaging,
    urlActualizar) {
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
            TableAdminMyProducts.LoadDataFiltered(response.objAdminMyProduct,
                urlPaging,
                urlActualizar);
        },
        error: function (dataError) {
            console.log(dataError);
            //LoadingAdminPage.showOrHideLoadingPage(false);
        }
    });
}

TableAdminMyProducts.LoadDataFiltered = function (objAdminMyProduct, urlPaging, urlActualizar) {
    $("#tbodyAdminMyProducts").html(objAdminMyProduct.FilasProductosFavoritos);
    $("#desCantidadProductos").text(objAdminMyProduct.DescripcionCantidad);
    Size.ValidationChild();
    TableAdminMyProducts.ActualizarPartidaFav(urlActualizar);
    $("#chkAllAdminMyProducts").prop('checked', false);
    if (objAdminMyProduct.TotalPaginas > 1) {
        $("#pagingAdminMyProducts").twbsPagination('destroy');
        $("#divPagingAdminMyProducts").removeClass("no-display");

        TableAdminMyProducts.RegisterPaging(urlPaging,
            objAdminMyProduct.TotalPaginas,
            objAdminMyProduct.CountVisiblePages,
            urlActualizar);
        
        LoadingAdminPage.showOrHideLoadingPage(false);
    } else {
        $("#pagingAdminMyProducts").twbsPagination('destroy');
        $("#divPagingAdminMyProducts").addClass("no-display");
        LoadingAdminPage.showOrHideLoadingPage(false);
    }
}

TableAdminMyProducts.RegisterPaging = function (urlPaging,
    pTotalPages,
    pVisiblePages,
    pUrlActualizar) {

    TableAdminMyProducts.ActualizarPartidaFav(pUrlActualizar);

    $('#pagingAdminMyProducts').twbsPagination({
        totalPages: pTotalPages,
        visiblePages: pVisiblePages,
        hideOnlyOnePage: true,
        initiateStartPageClick: false,
        onPageClick: function (event, page) {

            var chkArray = [];
            $("#tbodyAdminMyProducts tr>td input[type=checkbox]:checked").each(function () {
                chkArray.push($(this).val());
            });
            var vRegistrosSeleccionados = chkArray.join(",");

            chkArray = [];
            $("#tbodyAdminMyProducts tr>td input[type=checkbox]").each(function () {
                chkArray.push($(this).val());
            });
            var vRegistrosPagina = chkArray.join(",");

            $.ajax({
                type: "POST",
                url: urlPaging,
                data: {
                    pagina: page,
                    idsSeleccionados: vRegistrosSeleccionados ,
                    idsPagina: vRegistrosPagina
                },
                beforeSend: function () {
                    LoadingAdminPage.showOrHideLoadingPage(true);
                },
                success: function (response) {
                    $("#tbodyAdminMyProducts").html(response.filasProductosFavoritos);
                    Size.ValidationChild();
                    //var wp = $("#table" + idPaging);
                    //setTimeout(AdminPage.applyDot(wp, 2, "right"), 500);

                    TableAdminMyProducts.ActualizarPartidaFav(pUrlActualizar);
                    $("#chkAllAdminMyProducts").prop('checked', false);

                    LoadingAdminPage.showOrHideLoadingPage(false);
                },
                error: function (data) {
                    console.log(data);
                }
            });
        }
    });
}

TableAdminMyProducts.ActualizarPartidaFav = function (urlActualizar) {
    $("a.lnkActualizarPartidaFav").click(function() {
        var vIdPartida = $(this).data("idpartida");
        var vIdTextPartidaFav = $(this).data("id-txtpartidafav");

        var vTextPartidaFav = $("#" + vIdTextPartidaFav).val();

        $.ajax({
            type: "POST",
            url: urlActualizar,
            data: {
                idPartida: vIdPartida,
                textPartidaFav: vTextPartidaFav
            },
            beforeSend: function () {
                LoadingAdminPage.showOrHideLoadingPage(true);
            },
            success: function (response) {
                $("#tbodyAdminMyProducts").html(response.filasProductosFavoritos);
                TableAdminMyProducts.ActualizarPartidaFav(urlActualizar);
                Size.ValidationChild();
                LoadingAdminPage.showOrHideLoadingPage(false);
            },
            error: function (dataError) {
                console.log(dataError);
                //LoadingAdminPage.showOrHideLoadingPage(false);
            }
        });
    });
}


function OptionAdminMyProduct() {
}

OptionAdminMyProduct.AddToGroup = function (urlAddToGroup) {
    var chkArray = [];

    $("#tbodyAdminMyProducts tr>td input[type=checkbox]:checked").each(function () {
        chkArray.push($(this).val());
    });
    var vRegistrosSeleccionados = chkArray.join(",");
    
    chkArray = [];
    $("#tbodyAdminMyProducts tr>td input[type=checkbox]").each(function () {
        chkArray.push($(this).val());
    });
    var vRegistrosPagina = chkArray.join(",");

    $.ajax({
        type: "POST",
        url: urlAddToGroup,
        data: {
            idsSeleccionados: vRegistrosSeleccionados,
            idsPagina : vRegistrosPagina
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

OptionAdminMyProduct.UpdateGroup = function (urlUpdate, pUrlActualizar) {

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
            isCheckedCreateGroup: vIsCheckedCrearGrupo ,
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
                $("#tbodyAdminMyProducts").html(response.filasProductosFavoritos);
                Size.ValidationChild();
                //var wp = $("#table" + idPaging);
                //setTimeout(AdminPage.applyDot(wp, 2, "right"), 500);
                TableAdminMyProducts.ActualizarPartidaFav(pUrlActualizar);

                if (response.itemsGruposFavoritos != null) {
                    var currentCodGrupoF = $("#cboGruposF").val();
                    AdminPage.loadDataComboBox(response.itemsGruposFavoritos, "cboGruposF");
                    AdminPage.loadDataComboBox(response.itemsSoloGruposFavoritos, "cboGrupos");
                    $("#cboGruposF").val(currentCodGrupoF);
                }

                $("#chkAllAdminMyProducts").prop('checked', false);

                LoadingAdminPage.showOrHideLoadingPage(false);
                ModalAdmin.hide("ModalAddToGroup");
                OptionAdminMyProduct.ResetModalAddToGroup();

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

OptionAdminMyProduct.ResetModalAddToGroup = function() {
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

OptionAdminMyProduct.DeleteSelection = function (urlDelete, urlPaging,
    urlActualizar) {

    var chkArray = [];

    $("#tbodyAdminMyProducts tr>td input[type=checkbox]:checked").each(function () {
        chkArray.push($(this).val());
    });
    var vRegistrosSeleccionados = chkArray.join(",");

    chkArray = [];
    $("#tbodyAdminMyProducts tr>td input[type=checkbox]").each(function () {
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
            if (response.objAdminMyProduct != null) {
                TableAdminMyProducts.LoadDataFiltered(response.objAdminMyProduct,
                    urlPaging,
                    urlActualizar);
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


