using System;
using System.Collections;
using System.Data;
using Veritrade2017.Helpers;

namespace Veritrade2017.Models
{
    public class CorreoBienvenida
    {
        public string nombre { get; set; }
        public string nombrePlan { get; set; }
        public string user { get; set; }
        public string pass { get; set; }
        public string vigenciaInicio { get; set; }
        public string vigenciaFin { get; set; }
        public string paises { get; set; }
        public string modulos { get; set; }
        public string numUsuarios { get; set; }
        public string favoritos { get; set; }
        public string numGrupos { get; set; }
        public string numFavoritos { get; set; }
        public string numPlantillasImp { get; set; }
        public string numPlantillasExp { get; set; }
        public string plantillas { get; internal set; }
    }
}