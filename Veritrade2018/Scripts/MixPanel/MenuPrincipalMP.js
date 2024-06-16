/* JANAQ 070620
 * Eventos capturados con mixpanel en el menu principal
*/

$(document).ready(function () {

    //Si se ingreso por url de referido (validacion para evitar trackear enlaces de universidades)
    if (getCookie("VLU") == "1") return;

    // Evento clic idioma
    $(document).on("click", "#idioma", function () {
        var idiomaIngles = "ENGLISH +";
        var idiomaEspaniol = "ESPAÑOL +";
        var idiomaAConvertir = "";

        if ($(this)[0].innerText == idiomaIngles) {
            idiomaAConvertir = idiomaEspaniol;
        } else {
            idiomaAConvertir = idiomaIngles;
        }

        mixpanel.track("Clic Idioma",
            {
                "Sección": "Menu Principal",
                "Idioma para Visualizar": idiomaAConvertir
            });
    });

    // Evento clic opciones menu principal
    $(document).on("click", "ul.dropdown-menu a", function () {
        mixpanel.track("Clic " + $(this).attr("data-field"),
            {
                "Sección": "Menu Principal"
            });
    });

    // Evento clic ir a seccion mis busquedas
    $(document).on("click", "a.navbar-brand img", function () {
        mixpanel.track("Clic Ir a Sección Mis-Búsquedas" ,
            {
                "Sección": "Menu Principal"
            });
    });
});
