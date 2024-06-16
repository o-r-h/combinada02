
function OptionMyTemplate() {
}

OptionMyTemplate.changeTipoOpe = function (urlPost) {

    $.ajax({
        type: "POST",
        url: urlPost,
        data: {
            isCheckedExportacion: $("#rdbExp").is(":checked")
        },
        beforeSend: function () {
            LoadingAdminPage.showOrHideLoadingPage(true);
        },
        success: function (response) {
            AdminPage.loadDataComboBox(response.objMiPerfil.ListItemsPais, "cboPais");
            AdminPage.loadDataComboBox(response.objAdminMyTemplate.Downloads,"cboDescargas");
            $("#tbodyTemplate").html(response.objAdminMyTemplate.MyFieldsTemplateInHtml);
            OptionMyTemplate.HideFormTemplate();
            OptionMyTemplate.SetVisibleBtnNewTemplate(response.objAdminMyTemplate.IsVisibleBtnNewTemplate);

            $("#cboPais").val(response.objMiPerfil.CodPaisSelected);
            
            LoadingAdminPage.showOrHideLoadingPage(false);
        },
        error: function (dataError) {
            console.info(dataError);
        }
    });
}

OptionMyTemplate.changePais2 = function (urlPost,
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
            AdminPage.loadDataComboBox(response.objAdminMyTemplate.Downloads, "cboDescargas");
            $("#tbodyTemplate").html(response.objAdminMyTemplate.MyFieldsTemplateInHtml);
            OptionMyTemplate.HideFormTemplate();
            OptionMyTemplate.SetVisibleBtnNewTemplate(response.objAdminMyTemplate.IsVisibleBtnNewTemplate);

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

OptionMyTemplate.changePais = function (urlPost, pCodPais, pTextCodPais) {
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
            AdminPage.loadDataComboBox(response.objAdminMyTemplate.Downloads, "cboDescargas");
            $("#tbodyTemplate").html(response.objAdminMyTemplate.MyFieldsTemplateInHtml);
            OptionMyTemplate.HideFormTemplate();
            OptionMyTemplate.SetVisibleBtnNewTemplate(response.objAdminMyTemplate.IsVisibleBtnNewTemplate);

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

OptionMyTemplate.ChangeDownloads = function(urlPost, codTemplate) {
    $.ajax({
        type: "POST",
        url: urlPost,
        data: {
            codPlantilla : codTemplate
        },
        beforeSend: function () {
            LoadingAdminPage.showOrHideLoadingPage(true);
        },
        success: function (response) {
            $("#tbodyTemplate").html(response.rowsFieldsTemplate);
            LoadingAdminPage.showOrHideLoadingPage(false);
            $("#chkDefault").prop('checked', response.IsCheckedDefault);
        },
        error: function (dataError) {
            console.log(dataError);
            //LoadingAdminPage.showOrHideLoadingPage(false);
        }
    });
}

OptionMyTemplate.NewTemplate = function(urlPost) {
    $.ajax({
        type: "POST",
        url: urlPost,
        data: {
            codPlantilla: $("#cboDescargas").val()
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
                if (!$("#formTemplate").hasClass('no-display'))
                    $("#formTemplate").addClass("no-display");
            } else {
                OptionMyTemplate.ShowFormTemplate("");
                $("#tbodyTemplate").html(response.objAdminMyTemplate.MyFieldsTemplateInHtml);
                LoadingAdminPage.showOrHideLoadingPage(false);
            }
        },
        error: function (dataError) {
            console.log(dataError);
            //LoadingAdminPage.showOrHideLoadingPage(false);
        }
    });
}

OptionMyTemplate.ShowFormTemplate = function(currentTextTemplate ) {
    $("#txtTemplate").val(currentTextTemplate);
//    $("#chkDefault").prop('checked', false);
    $("#formTemplate").removeClass("no-display");
    $("#txtTemplate").focus();
}

OptionMyTemplate.SetVisibleBtnNewTemplate = function(flasVisible)
{
    if (!flasVisible) {
        $("#divBtnNewTemplate").addClass("no-display");
    } else {
        $("#divBtnNewTemplate").removeClass("no-display");
    }
}

OptionMyTemplate.HideFormTemplate = function () {
    $("#txtTemplate").val("");
    $("#chkDefault").prop('checked', false);
    $("#formTemplate").addClass("no-display");
}

OptionMyTemplate.SaveTemplate = function (urlSave, culture) {

    var valuesFields = [];
    $("#tbodyTemplate tr>td.valueField").each(function () {
        valuesFields.push($(this).data("field"));
    });
    var vValuesFields = valuesFields.join("¬");

    var checkFields = [];
    $("#tbodyTemplate  tr>td.checkField input[type=checkbox]").each(function () {
        checkFields.push($(this).is(":checked"));
    });
    var vCheckFields = checkFields.join("¬");

    var fieldsCustomizes = [];
    $("#tbodyTemplate  tr>td.textFavorite input[type=text]").each(function () {
        if ($(this).val() != "") {
            fieldsCustomizes.push($(this).val());
        } else {
            fieldsCustomizes.push($(this).data("fieldfavorite"));
        }
    });
    var vFieldsCustomizes = fieldsCustomizes.join("¬");
    if (culture == 'es') {
        var replace = " [Por Defecto]";
    }
    else {
        var replace = " [Default]";
    }
    $.ajax({
        type: "POST",
        url: urlSave,
        data: {
            indexPlantilla: $("#cboDescargas").prop('selectedIndex'),
            codPlantilla: $("#cboDescargas").val(),
            textTemplate: $("#txtTemplate").val().replace(replace,""),
            isCheckedDefault: $("#chkDefault").is(":checked"),
            fieldsTemplates: vValuesFields,
            checkFieldsTemplates: vCheckFields,
            fieldsCustomizeTemplates: vFieldsCustomizes
        },
        beforeSend: function () {
            LoadingAdminPage.showOrHideLoadingPage(true);
        },
        success: function (response) {
            console.info(response.objAdminMyTemplate.Downloads.length);

            if (response.objAdminMyTemplate.Downloads.length > 0)  {
                AdminPage.loadDataComboBox(response.objAdminMyTemplate.Downloads, "cboDescargas");
                $("#cboDescargas").val(response.objAdminMyTemplate.CurrentCodTemplate);
            }
            $("#tbodyTemplate").html(response.objAdminMyTemplate.MyFieldsTemplateInHtml);
            //OptionMyTemplate.SetVisibleBtnNewTemplate(response.objAdminMyTemplate.IsVisibleBtnNewTemplate);
            LoadingAdminPage.showOrHideLoadingPage(false);
        },
        error: function (dataError) {
            console.log(dataError);
            //LoadingAdminPage.showOrHideLoadingPage(false);
        }
    });
}

OptionMyTemplate.DeleteTemplate = function(urlDelete , cod) {
    $.ajax({
        type: "POST",
        url: urlDelete,
        data: {
            codigo: cod
        },
        beforeSend: function () {
            LoadingAdminPage.showOrHideLoadingPage(true);
        },
        success: function (response) {

            AdminPage.loadDataComboBox(response.objAdminMyTemplate.Downloads, "cboDescargas");
            $("#cboDescargas").trigger('change');
            LoadingAdminPage.showOrHideLoadingPage(false);
        },
        error: function (dataError) {
            console.log(dataError);
            //LoadingAdminPage.showOrHideLoadingPage(false);
        }
    });
}

OptionMyTemplate.CancelTemplate = function (urlCancel) {

    var vIndexPlantilla = $("#cboDescargas").prop('selectedIndex');

    $.ajax({
        type: "POST",
        url: urlCancel,
        data: {
            codPlantilla: $("#cboDescargas").val(),
            indexPlantilla: vIndexPlantilla 
        },
        beforeSend: function () {
            LoadingAdminPage.showOrHideLoadingPage(true);
        },
        success: function (response) {
            $("#tbodyTemplate").html(response.objAdminMyTemplate.MyFieldsTemplateInHtml);

            if (vIndexPlantilla == 0) {
                OptionMyTemplate.HideFormTemplate();
            }
            OptionMyTemplate.SetVisibleBtnNewTemplate(response.objAdminMyTemplate.IsVisibleBtnNewTemplate);

            LoadingAdminPage.showOrHideLoadingPage(false);
        },
        error: function (dataError) {
            console.log(dataError);
            //LoadingAdminPage.showOrHideLoadingPage(false);
        }
    });
}
$('#chkAllFields').click(function () {
    $('.c_template_checkbox').prop('checked', $('#chkAllFields').prop('checked'));

});



