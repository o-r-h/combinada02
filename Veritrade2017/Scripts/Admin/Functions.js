function f_IsEmpty(s) {
    if (s == "" || s == null) return true; else return false;
}

function f_InvalidChars(s) {
    //var InvalidChars = "+='%#_";
    var InvalidChars = "'*%";
    if (f_IsEmpty(s)) return false;
    for (var i = 0; i < s.length; i++)
        for (var j = 0; j < InvalidChars.length; j++)
        if (s.charAt(i) == InvalidChars.charAt(j)) return true;
    return false;
}

function f_InvalidChars2(s) {
    //var InvalidChars = "+='%#_";
    var InvalidChars = "\"'*%";
    if (f_IsEmpty(s)) return false;
    for (var i = 0; i < s.length; i++)
        for (var j = 0; j < InvalidChars.length; j++)
            if (s.charAt(i) == InvalidChars.charAt(j)) return true;
    return false;
}

function f_Trim(s) {
    while (s.charAt(0) == " ") s = s.substring(1, s.length);
    if (s.length > 1)
        while (s.charAt(s.length - 1) == " ") s = s.substring(0, s.length - 1);
    return s;
}

function f_WordCount(s) {
    var cont = 0;
    for (var i = 0; i < s.length; i++)
        if (s.charAt(i) == " " && s.charAt(i + 1) != " ")
        cont++;
    cont = cont + 1;
    return cont;
}

function f_GetWord(s, n) {
    var ini = 0;
    var fin = 0;
    if (n == 1) {
        for (var i = 0; i < s.length; i++)
            if (s.charAt(i) == " ") {
            fin = i; break;
        }
    }
    else {
        var num = 1;
        while (num != n) {
            var FindWord = false;
            for (var i = ini; i < s.length; i++) {
                if (s.charAt(i) == " " && s.charAt(i + 1) != " ") {
                    ini = i + 1; num++; FindWord = true; break;
                }
            }
            if (FindWord == false) break;
        }
        for (var i = ini; i < s.length; i++) {
            if (s.charAt(i) == " ") {
                fin = i; break;
            }
        }
    }
    if (FindWord == false) return "";
    if (fin == 0) fin = s.length;
    return s.substring(ini, fin)
}

function f_VerificaBusquedaNandina(txtNandinaB, Ayuda) {    
    palabra = txtNandinaB.value;
    palabra = f_Trim(palabra);
    if (f_IsEmpty(palabra) || palabra == Ayuda ) {
        alert("Se debe ingresar un texto");
        txtNandinaB.focus();
        return false;
    }
    if (f_InvalidChars(palabra)) {
        alert("El texto ingresado contiene caracteres invalidos: '*%");
        txtNandinaB.focus();
        return false;
    }
    if (f_WordCount(palabra) > 1) {
        alert("El texto ingresado contiene mas de un Código de partida");
        txtNandinaB.focus();
        return false;
    }
    if (isNaN(palabra)) {
        alert("Ingrese sólo dígitos");
        txtNandinaB.focus();
        return false;
    }
    if (palabra.length < 4) {
        alert(Ayuda);
        txtNandinaB.focus();
        return false;
    }

    return true;
}

function f_VerificaBusquedaPartida(txtPartidaB, Ayuda, hdfPalabraB1, hdfPalabraB2, hdfPalabraB3) {
    palabra = txtPartidaB.value;
    palabra = f_Trim(palabra);
    if (f_IsEmpty(palabra) || palabra == Ayuda) {
        alert("Se debe ingresar un texto");
        txtPartidaB.focus();
        return false;
    }
    if (f_InvalidChars(palabra)) {
        alert("El texto ingresado contiene caracteres invalidos: '*%");
        txtPartidaB.focus();
        return false;
    }
    if (f_WordCount(palabra) > 3) {
        alert("El texto ingresado contiene mas de 3 palabras");
        txtPartidaB.focus();
        return false;
    }
    var palabra1 = f_GetWord(palabra, 1)
    var palabra2 = f_GetWord(palabra, 2)
    var palabra3 = f_GetWord(palabra, 3)

    var cond1 = (palabra1.length >= 3 || (palabra2 != "" && palabra2.length >= 3) || (palabra3 != "" && palabra3.length >= 3))
    var cond2 = (palabra1.length < 2 || (palabra2 != "" && palabra2.length < 2) || (palabra3 != "" && palabra3.length < 2))

    if (!cond1 || cond2) {
        alert("La longitud de una o dos palabras debe ser de 3 o mas caracteres. La otra palabra puede tener dos caracteres");
        txtPartidaB.focus();
        return false;
    }
    hdfPalabraB1.value = palabra1
    hdfPalabraB2.value = palabra2
    hdfPalabraB3.value = palabra3

    return true;
}

function f_VerificaBusquedaComercial(txtDesComercialB, Ayuda, hdfPalabraB1, hdfPalabraB2, hdfPalabraB3) {
    palabra = txtDesComercialB.value;
    palabra = f_Trim(palabra);
    if (f_IsEmpty(palabra) || palabra == Ayuda) {
        alert("Por favor ingrese un texto a buscar");
        txtDesComercialB.focus();
        return false;
    }

    if (f_InvalidChars(palabra)) {
        alert("El texto ingresado contiene caracteres invalidos: '*%");
        txtDesComercialB.focus();
        return false;
    }

    if (palabra.charAt(0) == ("\"") && palabra.charAt(palabra.length - 1) == ("\"")) {
        if (palabra.length < 5) {
            alert("La longitud del texto exacto a buscar debe ser de 5 o mas caracteres");
            txtDesComercialB.focus();
            return false;
        }
        hdfPalabraB1.value = palabra
        return true;
    }
    palabra = palabra.replace("\"", "")

    if (f_WordCount(palabra) > 3) {
        alert("El texto ingresado contiene mas de 3 palabras");
        txtDesComercialB.focus();
        return false;
    }
    var palabra1 = f_GetWord(palabra, 1)
    var palabra2 = f_GetWord(palabra, 2)
    var palabra3 = f_GetWord(palabra, 3)

	var cond1 = (palabra1.length >= 3 || (palabra2 != "" && palabra2.length >= 3) || (palabra3 != "" && palabra3.length >= 3))
	var cond2 = (palabra1.length < 2 || (palabra2 != "" && palabra2.length < 2) || (palabra3 != "" && palabra3.length < 2))
	
	if (!cond1 || cond2) {
        alert("La longitud de una o dos palabras debe ser de 3 o mas caracteres. La otra palabra puede tener dos caracteres");
        txtDesComercialB.focus();
        return false;
    }
    hdfPalabraB1.value = palabra1
    hdfPalabraB2.value = palabra2
    hdfPalabraB3.value = palabra3
    return true;
}

function f_ValidaEmpresaBuscada(objPalabrasId, objPalabra1Id, objPalabra2Id, objPalabra3Id) {

    var objPalabras = document.getElementById(objPalabrasId)
    var objPalabra1 = document.getElementById(objPalabra1Id)
    var objPalabra2 = document.getElementById(objPalabra2Id)
    var objPalabra3 = document.getElementById(objPalabra3Id)

    ayuda = "Ingrese varias palabras o partes de estas, separadas por espacios";

    palabra = objPalabras.value;
    palabra = f_Trim(palabra);
    if (f_IsEmpty(palabra) || palabra == ayuda) {
        //alert("Please input a text search");
        alert("Por favor ingrese un texto a buscar");
        objPalabras.focus();
        return false;
    }
    if (f_InvalidChars(palabra)) {
        //alert("Text search contains invalid chars: ' %");
        alert("El texto ingresado contiene caracteres inválidos: '*%");
        objPalabras.focus();
        return false;
    }
    if (f_WordCount(palabra) > 3) {
        //alert("Text search contains more than 3 words");
        alert("El texto ingresado contiene mas de 3 palabras");
        objPalabras.focus();
        return false;
    }
    var palabra1 = f_GetWord(palabra, 1)
    var palabra2 = f_GetWord(palabra, 2)
    var palabra3 = f_GetWord(palabra, 3)

    var cond1 = (palabra1 == "&" || palabra1.length >= 2);
    var cond2 = (palabra2 == "" || palabra2 == "&" || palabra2.length >= 2);
    var cond3 = (palabra3 == "" || palabra3 == "&" || palabra3.length >= 2);

    if (!cond1 || !cond2 || !cond3) {
        //alert("Words contains less than 2 chars");
        alert("Las palabras deben tener 2 o más caracteres")
        objPalabras.focus();
        return false;
    }
    objPalabra1.value = palabra1
    objPalabra2.value = palabra2
    objPalabra3.value = palabra3
    return true;
}

function f_ValidaRUC(objRUCId) {
    var objRUC = document.getElementById(objRUCId)

    ayuda = "Ingrese al menos 8 dígitos del RUC";
    
    var RUC = objRUC.value;

    RUC = f_Trim(RUC);

    if (f_IsEmpty(RUC)|| RUC == ayuda) {
        //alert("Please input a Tax ID");
        alert('Por favor ingrese los dígitos del RUC')
        objRUC.focus();
        return false;
    }
    if (f_WordCount(RUC) > 1) {
        alert("El texto ingresado contiene mas de un RUC a buscar");
        objRUC.focus();
        return false;
    }
    if (isNaN(RUC)) {
        alert("Ingrese sólo dígitos");
        objRUC.focus();
        return false;
    }
    if (RUC.length < 8) {
        //alert("Tax ID contains less than 8 characters");
        alert(ayuda);
        objRUC.focus();
        return false;
    }

    return true;
}