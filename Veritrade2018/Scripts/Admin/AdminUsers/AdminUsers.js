window.admin_users = {
    options: {},
    init: function (_options) {
        // Ajax events fire in following order
        $(document).ajaxStart(function () {
            LoadingAdminPage.showOrHideLoadingPage(true);
        }).ajaxSend(function (e, xhr, opts) {
        }).ajaxError(function (e, jqxhr, opts) {
            if (jqxhr.status == 401) {
                window.location.reload(true);
            }
        }).ajaxSuccess(function (e, xhr, opts) {
        }).ajaxComplete(function (e, xhr, opts) {
        }).ajaxStop(function () {
            LoadingAdminPage.showOrHideLoadingPage(false);
        });

        $.extend(this.options, _options);
        return this;
    },
    getData: function () {
        $.post(this.options.urlData, function (data) {
            var template = $('#tplViewUserRow').html();
            Mustache.parse(template);   // optional, speeds up future uses
            $("#tbl-users").html(Mustache.render(template, data.lst));
            if (!data.visibleAdd)
                $("#btnAdd").hide();
            else
                $("#btnAdd").show();
        });
    },
    editMe: function (row, undo, callback) {
        if (undo && $("#tbl-users").find("tr[data-new]").length) {
            $(row).remove();
            return false;
        }
        $.post(this.options.urlEdit, { id: $(row).data("id") }, function (data) {
            var template = $('#' + (undo ? "tplViewUserRow" : "tplAddEditUserRow")).html();
            Mustache.parse(template); // optional, speeds up future uses
            var rendered =  Mustache.render(template, $.extend(data, { Nro: $(row).data("nro") }))
            $(row).replaceWith(rendered);
            var $_row = $(row);
            
            if (typeof callback === "function")
                callback(rendered);
            
        });
    },
    addMe: function () {
        var hasNewRow = $("#tbl-users").find("tr[data-new]").length > 0;
        if (hasNewRow) {
            $("#tbl-users").find("tr[data-new] input:eq(0)").focus();
            return false;
        }

        $.post(this.options.urlAdd, function (data) {
            if (data.message !== undefined) {
                ModalAdmin.showVentanaMensajeWithTitleAndMessage("ModalVentanaMensaje",
                    "messageTitle",
                    data.titleMessage,
                    "message",
                    data.message,
                    "lnkContactenos",
                    false);
            } else {
                var template = $('#tplAddEditUserRow').html();
                Mustache.parse(template); // optional, speeds up future uses
                if (!hasNewRow)
                    $(Mustache.render(template, $.extend(data, { IsNew : true} ))).appendTo("#tbl-users");
            }
        });
    },
    saveMe: function (row) {
        if ($(row).find(".msg").is(":visible")) {
            return false;
        }

        var Estado = "&Estado=" + ($("[name='Estado']").prop("checked") ? "A" : "I");
        $.post(this.options.urlSave, $(row).find(":input:not([type='checkbox'])").serialize() + Estado ,  function (data) {
            if (data.message !== undefined) {
                ModalAdmin.showVentanaMensajeWithTitleAndMessage("ModalVentanaMensaje",
                    "messageTitle",
                    data.titleMessage,
                    "message",
                    data.message,
                    "lnkContactenos",
                    false);
            } else {
                if (data.id)
                    window.admin_users.getData();
            }
        });
    }
};  