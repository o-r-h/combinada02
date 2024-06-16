using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Veritrade2018.Helpers;

namespace Veritrade2018.Controllers
{
    public class ReferidoController : Controller
    {
        private readonly VeritradeAdmin.Seguridad _ws = new VeritradeAdmin.Seguridad();
        public bool flagUlima, flagAdex, flagUPN;

        // Ruben 202211
        // GET: Referido
        public ActionResult Index()
        {
            
            Session.RemoveAll();
            //return View();
            string url = Request.Url.AbsoluteUri;

            Dictionary<string, string> queryValues = Extensiones.GetQueryValues(url);

            string variable = queryValues.Keys.First().ToUpper();
            string idUsuario = queryValues.Values.First();
            string inicio = "", txtCodUsuario = "", txtPassword = "";
            //idUsuario = valor que viene de la ruta
            string compra = "";
            bool flag = false;
            flagUlima = flagAdex = flagUPN = false;
            string IdUsuario = "";

            // Ruben 202310
            Funciones.GrabaLog("0", "", "", "0", "0", "Referido", "Url Usado | " + url);
            
            if (!String.IsNullOrEmpty(idUsuario)) IdUsuario = idUsuario;
#if DEBUG

            string DireccionIP = Properties.Settings.Default.IP_Debug;//200.121.158.94
#else
            string DireccionIP = Request.ServerVariables["REMOTE_ADDR"];

#endif 
            // Ruben 202304
            if (Request.UrlReferrer == null)
                Funciones.GrabaLog("0", "", "", "0", "0", "Referido", "Url Referido | " + variable +  " | Nulo");
            else
            {
                Funciones.GrabaLog("0", "", "", "0", "0", "Referido", "Url Referido | " + variable + " | " + Request.UrlReferrer.ToString().ToLower());
                Funciones.GrabaLog("0", "", "", "0", "0", "Referido", "Url Referido2 | " + variable + " | " + Request.ServerVariables["HTTP_REFERER"].ToString().ToLower());
            }

            /*
            Console.WriteLine(Request.UrlReferrer);
            Console.WriteLine(Request.ServerVariables["http_referer"]);
            
            if (Request.UrlReferrer == null)
                Funciones.GrabaLog("0", "", "", "0", "0", variable + " Referido", "Nulo");
            else
            {
                Funciones.GrabaLog("0", "", "", "0", "0", variable + " Referido", Request.UrlReferrer.ToString().ToLower());
                Funciones.GrabaLog("0", "", "", "0", "0", variable + " Referido", Request.ServerVariables["http_referer"].ToString().ToLower());
            }
            */

            string URLReferido = "";
            string URL1 = "", URL2 = "", URL3 = "", URL4 = "";
            string campus = "instituto";
            string campaña = "0";

            // Ruben 202304
            if (Request.UrlReferrer != null && Request.UrlReferrer.ToString() != "")
                URLReferido = Request.UrlReferrer.ToString().ToLower();

            if (URLReferido == "" && Request.ServerVariables["HTTP_REFERER"].ToString().ToLower() != "")
                URLReferido = Request.ServerVariables["HTTP_REFERER"].ToString().ToLower();

            // Ruben 202211
            if (URLReferido != "" || variable == "SISE") // (Request.UrlReferrer != null && Request.UrlReferrer.ToString() != "")
            {
                // Ruben 202211
                //if (Request.UrlReferrer != null && Request.UrlReferrer.ToString() != "")
                //    URLReferido = Request.UrlReferrer.ToString().ToLower();
                
                //URLReferido = "http://referido.veritrade.tk/";
                //URLReferido = "http://intranet3/site/seccion.aspx"; // deloitte
                //URLReferido = "https://red.itp.gob.pe/";//ITP
                //URLReferido = "http://fresno.ulima.edu.pe/SF/sfov_bd001.nsf/VERITRADE?OpenFom"; // PARA ULIMA
                //URLReferido = "http://esanvirtual.edu.pe/"; // PARA ESAN
                //URLReferido = "http://recursosinvestigacion.upc.edu.pe"; // PARA UPC
                //URLReferido = "http://aulavirtual.adexperu.edu.pe"; //PARA ADEX
                //URLReferido = "http://net.upt.edu.pe/veritrade"; // PARA UPT
                
                switch (variable)
                {
                    case "CO_USER":
                    case "UL"://UNIV LIMA
                        campaña = "27100";
                        bool ValidaULIMA = (!String.IsNullOrEmpty(IdUsuario));
                        
                        Funciones.BuscaURLReferido("ULIMA", "0", ref URL1, ref URL2, ref URL3, ref URL4);

                        ValidaULIMA = ValidaULIMA && (URLReferido.ToLower() == URL1 || URLReferido.ToLower() == URL2 || URLReferido.ToLower() == URL3 || URLReferido.ToLower() == URL4);
                        if (ValidaULIMA)
                        {
                            bool ValidaIP = FuncionesBusiness.ValidaRangoIP(DireccionIP, "ULIMA");
                            if (ValidaIP)
                            {
                                txtCodUsuario = "UL-" + IdUsuario;
                                txtPassword = Resources.Resources.ReferidoULPassword;
                                string Empresa = "UNIVERSIDAD DE LIMA";
                                string Nombres = "Usuario";
                                string Apellidos = IdUsuario;
                                string IdAplicacion = "1";
                                if (!ExisteCodUsuario(txtCodUsuario, IdAplicacion))
                                    CrearUsuario(txtCodUsuario, txtPassword, Empresa, Nombres, Apellidos, IdAplicacion, "Convenio", "ULIMA");
                                //string URL = "MisBusquedas.aspx";
                                inicio = "referid0";
                                string origen = "http://www.veritradecorp.com";
                                //string strJS = Funciones._GetPostJS(URL, inicio, origen, CodUsuario, Password);
                                //litScript.Text = strJS;
                                flag = true;
                            }
                            else
                            {
                                Funciones.GrabaLog("0", "", "", "0", "0", "Referido", "Error | ULIMA | " + Request.QueryString["CO_USER"] + " | " + DireccionIP);
                                //flag = true;
                                flagUlima = false;
                            }

                            campus = "universidad";
                        }
                        break;
                    case "PUCP"://UNIV CATÓLICA
                        campus = "universidad";
                        campaña = "27103";
                        bool ValidaPUCP = (!String.IsNullOrEmpty(IdUsuario));

                        Funciones.BuscaURLReferido("PUCP", "0", ref URL1, ref URL2, ref URL3, ref URL4);

                        ValidaULIMA = ValidaPUCP && (URLReferido.ToLower() == URL1 || URLReferido.ToLower() == URL2 || URLReferido.ToLower() == URL3 || URLReferido.ToLower() == URL4);
                        if (ValidaULIMA)
                        {
                            bool ValidaIP = FuncionesBusiness.ValidaRangoIP(DireccionIP, "PUCP");
                            if (ValidaIP)
                            {
                                txtCodUsuario = "PUCP-" + IdUsuario;
                                txtPassword = Resources.Resources.ReferidoPUCPPassword;
                                string Empresa = "PONTIFICE UNIVERSIDAD CATOLICA";
                                string Nombres = "Usuario";
                                string Apellidos = IdUsuario;
                                string IdAplicacion = "1";
                                if (!ExisteCodUsuario(txtCodUsuario, IdAplicacion))
                                    CrearUsuario(txtCodUsuario, txtPassword, Empresa, Nombres, Apellidos, IdAplicacion, "Convenio", "PUCP");
                                //string URL = "MisBusquedas.aspx";
                                inicio = "referid0";
                                string origen = "http://www.veritradecorp.com";
                                //string strJS = Funciones._GetPostJS(URL, inicio, origen, CodUsuario, Password);
                                //litScript.Text = strJS;
                                flag = true;
                            }
                            else
                            {
                                Funciones.GrabaLog("0", "", "", "0", "0", "Referido", "Error | PUCP | " + Request.QueryString["CO_USER"] + " | " + DireccionIP);
                                //flag = true;
                                flagUlima = false;
                            }

                            
                        }
                        break;
                    case "DELOITTE"://DELOITTE
                        campaña = "27105";
                        bool ValidaDELOITTE = (!String.IsNullOrEmpty(IdUsuario));

                        Funciones.BuscaURLReferido("DELOITTE", "0", ref URL1, ref URL2, ref URL3, ref URL4);

                        // Ruben 202304
                        ValidaDELOITTE = ValidaDELOITTE && (URLReferido.Contains(URL1) || (URL2 != "" && URLReferido.Contains(URL2)) || (URL3 != "" && URLReferido.Contains(URL3)) || (URL4 != "" && URLReferido.Contains(URL4)));
                        if (ValidaDELOITTE)
                        {
                            bool ValidaIP = FuncionesBusiness.ValidaRangoIP(DireccionIP, "DELOITTE");
                            if (ValidaIP)
                            {
                                txtCodUsuario = "DELOITTE-" + IdUsuario;
                                txtPassword = Resources.Resources.ReferidoDeloittePassword;
                                string Empresa = "DELOITTE";
                                string Nombres = "Usuario";
                                string Apellidos = IdUsuario;
                                string IdAplicacion = "1";
                                if (!ExisteCodUsuario(txtCodUsuario, IdAplicacion))
                                    CrearUsuario(txtCodUsuario, txtPassword, Empresa, Nombres, Apellidos, IdAplicacion, "Convenio", "DELOITTE");
                                //string URL = "MisBusquedas.aspx";
                                inicio = "referid0";
                                string origen = "http://www.veritradecorp.com";
                                //string strJS = Funciones._GetPostJS(URL, inicio, origen, CodUsuario, Password);
                                //litScript.Text = strJS;
                                flag = true;
                            }
                            else
                            {
                                Funciones.GrabaLog("0", "", "", "0", "0", "Referido", "Error | DELOITTE | " + Request.QueryString["CO_USER"] + " | " + DireccionIP);
                                //flag = true;
                                flagUlima = false;
                            }

                            campus = "empresa";
                        }
                        break;
                    case "ITP"://DELOITTE
                        campaña = "27104";
                        bool ValidaITP = (!String.IsNullOrEmpty(IdUsuario));

                        Funciones.BuscaURLReferido("ITP", "0", ref URL1, ref URL2, ref URL3, ref URL4);

                        ValidaITP = ValidaITP && (URLReferido.ToLower() == URL1 || URLReferido.ToLower() == URL2 || URLReferido.ToLower() == URL3 || URLReferido.ToLower() == URL4);
                        if (ValidaITP)
                        {
                            bool ValidaIP = FuncionesBusiness.ValidaRangoIP(DireccionIP, "ITP");
                            if (ValidaIP)
                            {
                                txtCodUsuario = "ITP-" + IdUsuario;
                                txtPassword = Resources.Resources.ReferidoITPPassword;
                                string Empresa = "ITP";
                                string Nombres = "Usuario";
                                string Apellidos = IdUsuario;
                                string IdAplicacion = "1";
                                if (!ExisteCodUsuario(txtCodUsuario, IdAplicacion))
                                    CrearUsuario(txtCodUsuario, txtPassword, Empresa, Nombres, Apellidos, IdAplicacion, "Convenio", "ITP");
                                //string URL = "MisBusquedas.aspx";
                                inicio = "referid0";
                                string origen = "http://www.veritradecorp.com";
                                //string strJS = Funciones._GetPostJS(URL, inicio, origen, CodUsuario, Password);
                                //litScript.Text = strJS;
                                flag = true;
                            }
                            else
                            {
                                Funciones.GrabaLog("0", "", "", "0", "0", "Referido", "Error | ITP | " + Request.QueryString["CO_USER"] + " | " + DireccionIP);
                                //flag = true;
                                flagUlima = false;
                            }

                            campus = "empresa";
                        }
                        break;
                    case "ESAN":
                        bool validaESAN = (!String.IsNullOrEmpty(IdUsuario));

                        Funciones.BuscaURLReferido("ESAN", "0", ref URL1, ref URL2, ref URL3, ref URL4);

                        validaESAN = validaESAN && (URLReferido.Contains(URL1) || (URL2 != "" && URLReferido.Contains(URL2)) || (URL3 != "" && URLReferido.Contains(URL3)) || (URL4 != "" && URLReferido.Contains(URL4)));
                        if (validaESAN)
                        {
                            txtCodUsuario = "ESAN-" + IdUsuario;
                            txtPassword = Resources.Resources.ReferidoESANPassword;
                            string Empresa = "ESAN";
                            string Nombres = "Usuario";
                            string Apellidos = IdUsuario;
                            string IdAplicacion = "1";
                            if (!ExisteCodUsuario(txtCodUsuario, IdAplicacion))
                                CrearUsuario(txtCodUsuario, txtPassword, Empresa, Nombres, Apellidos, IdAplicacion, "Convenio", "ESAN");
                            inicio = "referid0";
                            string origen = "http://www.veritradecorp.com";
                            flag = true;
                        }
                        
                        break;
                    case "UC":
                        bool validaUC = (!String.IsNullOrEmpty(IdUsuario));

                        Funciones.BuscaURLReferido("UCONTINENTAL", "0", ref URL1, ref URL2, ref URL3, ref URL4);

                        validaUC = validaUC && (URLReferido.Contains(URL1) || (URL2 != "" && URLReferido.Contains(URL2)) || (URL3 != "" && URLReferido.Contains(URL3)) || (URL4 != "" && URLReferido.Contains(URL4)));
                        if (validaUC)
                        {
                            bool ValidaIP = FuncionesBusiness.ValidaRangoIP(DireccionIP, "UCONTINENTAL");
                            if(ValidaIP){
                                txtCodUsuario = "UCONTINENTAL-" + IdUsuario;
                                txtPassword = Resources.Resources.ReferidoUCPassword;
                                string Empresa = "UCONTINENTAL";
                                string Nombres = "Usuario";
                                string Apellidos = IdUsuario;
                                string IdAplicacion = "1";
                                if (!ExisteCodUsuario(txtCodUsuario, IdAplicacion))
                                    CrearUsuario(txtCodUsuario, txtPassword, Empresa, Nombres, Apellidos, IdAplicacion, "Convenio", "UCONTINENTAL");
                                inicio = "referid0";
                                string origen = "http://www.veritradecorp.com";
                                flag = true;
                            }
                            
                        }

                        break;
                    
                    // Ruben 202301
                    case "UPC":
                        campaña = "27101";
                        string IdUsuarioUPC = BuscarIdUsuario("UPC", "1");
                        bool validaUPC = !(String.IsNullOrEmpty(IdUsuario));

                        Funciones.BuscaURLReferido("UPC", IdUsuarioUPC, ref URL1, ref URL2, ref URL3, ref URL4);
                        
                        validaUPC &= (URLReferido.Contains(URL1) || (URL2 != "" && URLReferido.Contains(URL2)) || (URL3 != "" && URLReferido.Contains(URL3)) || (URL4 != "" && URLReferido.Contains(URL4)));
                        if (validaUPC)
                        {
                            bool ValidaIP = FuncionesBusiness.ValidaRangoIP(DireccionIP, "UPC");
                            if (ValidaIP)
                            {
                                txtCodUsuario = "UPC-" + IdUsuario;
                                txtPassword = Resources.Resources.ReferidoUPCPassword;
                                string Empresa = "UPC";
                                string Nombres = "Usuario";
                                string Apellidos = IdUsuario;
                                string IdAplicacion = "1";
                                if (!ExisteCodUsuario(txtCodUsuario, IdAplicacion))
                                    CrearUsuario(txtCodUsuario, txtPassword, Empresa, Nombres, Apellidos, IdAplicacion, "Convenio", "UPC");
                                inicio = "referid0";
                                string origen = "http://www.veritradecorp.com";
                                //Funciones.BuscaCodUsuario(IdUsuario, ref txtCodUsuario, ref txtPassword);
                                flag = true;
                            }
                        }
                        campus = "universidad";
                        break;

                    /*case "UPC":
                        campaña = "27101";
                        string IdUsuarioUPC = BuscarIdUsuario("UPC", "1");
                        bool validaUPC = !(String.IsNullOrEmpty(IdUsuario)) && (idUsuario==IdUsuarioUPC);
                        Funciones.BuscaURLReferido("UPC", IdUsuarioUPC, ref URL1, ref URL2, ref URL3);
                        validaUPC &= (URLReferido.Contains(URL1) || (URL2 != "" && URLReferido.Contains(URL2)) ||
                                      (URL3 != "" && URLReferido.Contains(URL3)));
                        if (validaUPC)
                        {
                            bool ValidaIP = FuncionesBusiness.ValidaRangoIP(DireccionIP, "UPC");
                            if (ValidaIP)
                            {
                                txtCodUsuario = "UPT-" + IdUsuario;
                                txtPassword = "UPT@Referid0";
                                string Empresa = "UPT";
                                string Nombres = "Usuario";
                                string Apellidos = IdUsuario;
                                string IdAplicacion = "1";
                                if (!ExisteCodUsuario(txtCodUsuario, IdAplicacion))
                                    CrearUsuario(txtCodUsuario, txtPassword, Empresa, Nombres, Apellidos, IdAplicacion, "Convenio", "UPT");
                                inicio = "referid0";
                                string origen = "http://www.veritradecorp.com";
                                //Funciones.BuscaCodUsuario(IdUsuario, ref txtCodUsuario, ref txtPassword);
                                flag = true;
                            }
                        }
                        campus = "universidad";
                        break;*/
                    case "ANALYTIC":
                        campaña = "27102";
                        bool validaUPT2 = (!String.IsNullOrEmpty(IdUsuario) && IdUsuario=="UPT");

                        Funciones.BuscaURLReferido("UPT", "0", ref URL1, ref URL2, ref URL3, ref URL4);

                        validaUPT2 = validaUPT2 && (URLReferido.Contains(URL1) || (URL2 != "" && URLReferido.Contains(URL2)) || (URL3 != "" && URLReferido.Contains(URL3)) || (URL4 != "" && URLReferido.Contains(URL4)));
                        if (validaUPT2)
                        {
                            bool ValidaIP = FuncionesBusiness.ValidaRangoIP(DireccionIP, "UPT");
                            if (ValidaIP)
                            {
                                string URL = "http://analytic.veritrade.info/PAS";
                                string origen = "http://www.veritradecorp.com";
                                string IdUsuarioUPT2 = BuscarIdUsuario("UPT", "2");
                                string CodUsuario = "", Password = "", CodEstado = "";
                                Funciones.BuscaCodUsuarioEstado(IdUsuarioUPT2, ref CodUsuario, ref Password, ref CodEstado);
                                if (CodEstado == "A")
                                {
                                    FuncionesBusiness.GrabaHistorial(IdUsuarioUPT2, DireccionIP, Request.ServerVariables["HTTP_USER_AGENT"],CodEstado);
                                    Response.Redirect(URL);
                                    //string strJS = Functions._GetPostJS(URL, inicio, origen, CodUsuario, Password);
                                    //litScript.Text = strJS;
                                    flag = true;
                                }
                            }
                            else
                            {
                                Funciones.GrabaLog("0", "", "", "0", "0", "Referido", "Error | UPT2 | " + DireccionIP);
                                flag = false;
                            }
                        }
                        campus = "universidad";
                        break;
                    case "UPT":
                        campaña = "27102";
                        bool validaUPT = (!String.IsNullOrEmpty(IdUsuario));

                        Funciones.BuscaURLReferido("UPT", "0", ref URL1, ref URL2, ref URL3, ref URL4);

                        validaUPT = validaUPT && (URLReferido.Contains(URL1) || (URL2 != "" && URLReferido.Contains(URL2)) || (URL3 != "" && URLReferido.Contains(URL3)) || (URL4 != "" && URLReferido.Contains(URL4)));
                        if (validaUPT)
                        {
                            bool ValidaIP = FuncionesBusiness.ValidaRangoIP(DireccionIP, "UPT");
                            if (ValidaIP)
                            {
                                txtCodUsuario = "UPT-" + IdUsuario;
                                txtPassword = Resources.Resources.ReferidoUPTPassword;
                                string Empresa = "UPT";
                                string Nombres = "Usuario";
                                string Apellidos = IdUsuario;
                                string IdAplicacion = "1";
                                if (!ExisteCodUsuario(txtCodUsuario, IdAplicacion))
                                    CrearUsuario(txtCodUsuario, txtPassword, Empresa, Nombres, Apellidos, IdAplicacion, "Convenio", "UPT");
                                inicio = "referid0";
                                string origen = "http://www.veritradecorp.com";
                                flag = true;
                            }

                        }
                        campus = "universidad";
                        break;

                    // Ruben 202203
                    case "CERTUS":
                        campaña = "0";
                        bool validaCERTUS = (!String.IsNullOrEmpty(IdUsuario));

                        Funciones.BuscaURLReferido("CERTUS", "0", ref URL1, ref URL2, ref URL3, ref URL4);

                        validaCERTUS = validaCERTUS && (URLReferido.Contains(URL1) || (URL2 != "" && URLReferido.Contains(URL2)) || (URL3 != "" && URLReferido.Contains(URL3)) || (URL4 != "" && URLReferido.Contains(URL4)));
                        if (validaCERTUS)
                        {
                            bool ValidaIP = FuncionesBusiness.ValidaRangoIP(DireccionIP, "CERTUS");
                            if (ValidaIP)
                            {
                                txtCodUsuario = "CERTUS-" + IdUsuario;
                                txtPassword = Resources.Resources.ReferidoUPTPassword;
                                string Empresa = "CERTUS";
                                string Nombres = "Usuario";
                                string Apellidos = IdUsuario;
                                string IdAplicacion = "1";
                                if (!ExisteCodUsuario(txtCodUsuario, IdAplicacion))
                                    CrearUsuario(txtCodUsuario, txtPassword, Empresa, Nombres, Apellidos, IdAplicacion, "Convenio", "CERTUS");
                                inicio = "referid0";
                                string origen = "http://www.veritradecorp.com";
                                flag = true;
                            }
                            else
                            {
                                Funciones.GrabaLog("0", "", "", "0", "0", "Referido", "Error | CERTUS | " + IdUsuario + " | " + DireccionIP);
                            }

                        }
                        campus = "instituto";
                        break;

                    // Ruben 202211
                    case "SISE":
                        campaña = "0";
                        
                        bool validaSISE = !string.IsNullOrEmpty(IdUsuario);
                        
                        Funciones.BuscaURLReferido("SISE", "0", ref URL1, ref URL2, ref URL3, ref URL4);

                        // Ruben 202211
                        // validaSISE = validaSISE && (URLReferido.Contains(URL1) || (URL2 != "" && URLReferido.Contains(URL2)) || (URL3 != "" && URLReferido.Contains(URL3))); // Ruben 202211

                        if (validaSISE)
                        {
                            bool ValidaIP = true; // FuncionesBusiness.ValidaRangoIP(DireccionIP, "SISE");
                            if (ValidaIP)
                            {
                                txtCodUsuario = "SISE-" + IdUsuario;
                                txtPassword = "SISE@Referid0"; //Resources.Resources.ReferidoUPTPassword;

                                string Empresa = "SISE";
                                string Nombres = "Usuario";
                                string Apellidos = IdUsuario;
                                string IdAplicacion = "1";
                                
                                if (!ExisteCodUsuario(txtCodUsuario, IdAplicacion))
                                    CrearUsuario(txtCodUsuario, txtPassword, Empresa, Nombres, Apellidos, IdAplicacion, "Convenio", "SISE");
                                
                                inicio = "referid0";
                                //string origen = "http://www.veritradecorp.com";

                                flag = true;
                            }
                            else
                            {
                                Funciones.GrabaLog("0", "", "", "0", "0", "Referido", "Error | SISE | " + IdUsuario + " | " + DireccionIP);
                            }

                        }

                        campus = "instituto";
                        
                        break;

                    // Ruben 202209
                    case "UCV":
                        campaña = "0";

                        bool validaUCV = !string.IsNullOrEmpty(IdUsuario);

                        Funciones.BuscaURLReferido("UCV", "0", ref URL1, ref URL2, ref URL3, ref URL4);

                        validaUCV = validaUCV && (URLReferido.Contains(URL1) || (URL2 != "" && URLReferido.Contains(URL2)) || (URL3 != "" && URLReferido.Contains(URL3)) || (URL4 != "" && URLReferido.Contains(URL4)));

                        if (validaUCV)
                        {
                            bool ValidaIP = true; // FuncionesBusiness.ValidaRangoIP(DireccionIP, "SISE");
                            if (ValidaIP)
                            {
                                txtCodUsuario = "UCV-" + IdUsuario;
                                txtPassword = "UCV@Referid0"; //Resources.Resources.ReferidoUPTPassword;

                                string Empresa = "UCV";
                                string Nombres = "Usuario";
                                string Apellidos = IdUsuario;
                                string IdAplicacion = "1";

                                if (!ExisteCodUsuario(txtCodUsuario, IdAplicacion))
                                    CrearUsuario(txtCodUsuario, txtPassword, Empresa, Nombres, Apellidos, IdAplicacion, "Convenio", "UCV");

                                inicio = "referid0";
                                //string origen = "http://www.veritradecorp.com";

                                flag = true;
                            }
                            else
                            {
                                Funciones.GrabaLog("0", "", "", "0", "0", "Referido", "Error | UCV | " + IdUsuario + " | " + DireccionIP);
                            }

                        }

                        campus = "universidad";

                        break;

                    // Ruben 202310
                    case "USIL":
                        campaña = "0";

                        string Universidad = variable;

                        bool validaUniversidad = !string.IsNullOrEmpty(IdUsuario);

                        Funciones.BuscaURLReferido(Universidad, "0", ref URL1, ref URL2, ref URL3, ref URL4);

                        validaUniversidad = validaUniversidad && (URLReferido.Contains(URL1) || (URL2 != "" && URLReferido.Contains(URL2)) || (URL3 != "" && URLReferido.Contains(URL3)) || (URL4 != "" && URLReferido.Contains(URL4)));

                        if (validaUniversidad)
                        {
                            bool validaIP = FuncionesBusiness.ValidaRangoIP(DireccionIP, Universidad);
                            if (validaIP)
                            {
                                txtCodUsuario = Universidad + "-" + IdUsuario;
                                txtPassword = Universidad+ "@Referid0";

                                string Empresa = Universidad;
                                string Nombres = "Usuario";
                                string Apellidos = IdUsuario;
                                string IdAplicacion = "1";

                                if (!ExisteCodUsuario(txtCodUsuario, IdAplicacion))
                                    CrearUsuario(txtCodUsuario, txtPassword, Empresa, Nombres, Apellidos, IdAplicacion, "Convenio", Universidad);

                                inicio = "referid0";

                                flag = true;
                            }
                            else
                            {
                                Funciones.GrabaLog("0", "", "", "0", "0", "Referido", "Error | " + Universidad + " | " + IdUsuario + " | " + DireccionIP);
                            }
                        }

                        campus = "universidad";

                        break;

                    case "IDUSUARIO": //PARA ADEX
                        campaña = "36100";
                        string IdUsuarioADEX = BuscarIdUsuario("ADEX-0", "1");
                        bool ValidaADEX = (IdUsuario == IdUsuarioADEX);

                        Funciones.BuscaURLReferido("ADEX", IdUsuarioADEX, ref URL1, ref URL2, ref URL3, ref URL4);

                        ValidaADEX = ValidaADEX && (URLReferido.Contains(URL1) || (URL2 != "" && URLReferido.Contains(URL2)) || (URL3 != "" && URLReferido.Contains(URL3)) || (URL4 != "" && URLReferido.Contains(URL4)));
                        if (ValidaADEX)
                        {
                            bool ValidaIP = FuncionesBusiness.ValidaRangoIP(DireccionIP, "ADEX");
                            if (ValidaIP)
                            {
                                inicio = "referid0";
                                string origen = "http://www.veritradecorp";
                                Funciones.BuscaCodUsuario(IdUsuario, ref txtCodUsuario, ref txtPassword);
                                flag = true;
                            }
                            else
                            {
                                Funciones.GrabaLog("0", "", "", "0", "0", "Referido", "Error | ADEX | " + DireccionIP);
                                //flag = true;
                                flagUlima = true;
                            }
                        }
                        else
                        {
                            string IdUsuarioUPN = BuscarIdUsuario("UPN-0", "1");
                            bool ValidaUPN = (IdUsuario == IdUsuarioUPN);

                            Funciones.BuscaURLReferido("UPN", IdUsuarioUPN, ref URL1, ref URL2, ref URL3, ref URL4);

                            ValidaUPN = ValidaUPN && (URLReferido.Contains(URL1) || (URL2 != "" && URLReferido.Contains(URL2)) || (URL3 != "" && URLReferido.Contains(URL3)) || (URL4 != "" && URLReferido.Contains(URL4)));
                            if (ValidaUPN)
                            {
                                campaña = "39100";
                                bool ValidaIP = FuncionesBusiness.ValidaRangoIP(DireccionIP, "UPN");
                                if (ValidaIP)
                                {
                                    string URL = "MisBusquedas.aspx";
                                    inicio = "referid0";
                                    string origen = "http://www.veritradecorp.com";

                                    Funciones.BuscaCodUsuario(IdUsuario, ref txtCodUsuario, ref txtPassword);
                                    //string strJS = Funciones._GetPostJS(URL, inicio, origen, CodUsuario, Password);
                                    //litScript.Text = strJS;

                                    flag = true;

                                }
                                else
                                {
                                    Funciones.GrabaLog("0", "", "", "0", "0", "Referido", "Error | UPN | " + DireccionIP);
                                    //flag = true;
                                    flagUPN = true;
                                }

                                campus = "universidad";
                            }
                        }
                        break;
                }
            }
            
            /*
            if (variable == "PUCP")
            {
                bool ValidaPUCP = (!String.IsNullOrEmpty(IdUsuario));
                //Funciones.BuscaURLReferido("PUCP", "0", ref URL1, ref URL2, ref URL3);
                //ValidaPUCP = ValidaPUCP && (URLReferido.ToLower() == URL1 || URLReferido.ToLower() == URL2 || URLReferido.ToLower() == URL3);
                if (ValidaPUCP)
                {
                    bool ValidaIP = FuncionesBusiness.ValidaRangoIP(DireccionIP, "PUCP");
                    if (ValidaIP)
                    {
                        txtCodUsuario = "PUCP-" + IdUsuario;
                        txtPassword = "PUCP@Referid0";
                        string Empresa = "PONTIFICE UNIVERSIDAD CATOLICA";
                        string Nombres = "Usuario";
                        string Apellidos = IdUsuario;
                        string IdAplicacion = "1";
                        if (!ExisteCodUsuario(txtCodUsuario, IdAplicacion))
                            CrearUsuario(txtCodUsuario, txtPassword, Empresa, Nombres, Apellidos, IdAplicacion, "Convenio", "PUCP");
                        //string URL = "MisBusquedas.aspx";
                        inicio = "referid0";
                        string origen = "http://www.veritradecorp.com";
                        //string strJS = Funciones._GetPostJS(URL, inicio, origen, CodUsuario, Password);
                        //litScript.Text = strJS;
                        flag = true;
                    }
                    else
                    {
                        Funciones.GrabaLog("0", "", "", "0", "0", "Referido", "Error | PUCP | " + Request.QueryString["PUCP"] + " | " + DireccionIP);
                        //flag = true;
                        flagUlima = false;
                    }

                    campus = "universidad";
                }
            }   */         
            

            if (flag)
                return //RedirectToAction("Redireccion", "Referido", new {inicio, txtCodUsuario, txtPassword, compra});
                Redireccion(inicio, txtCodUsuario, txtPassword,compra);
            else
            {
                var articul = "de la";
                if(campus == "instituto")
                {
                    articul = "del";
                }

                ViewData["MensajeError"] = "El acceso a VERITRADE es exclusivamente desde el campus "+articul + " " + campus +
                                           ".<br>Si desea conocer más acerca de nosotros ingrese aquí: ";
                ViewData["Campaña"] = campaña;
                return View();
            }

        }
        
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        bool ExisteCodUsuario(string CodUsuario, string IdAplicacion)
        {
            //string sql;
            //SqlConnection cn;
            //SqlCommand cmd;
            //SqlDataReader dtr;
            int cant;

            //cn = new SqlConnection();
            //cn.ConnectionString = ConfigurationManager.ConnectionStrings["cn"].ConnectionString;
            //cn.Open();

            string sql = "select count(*) as cant from Usuario where CodUsuario = '" + CodUsuario + "' and IdAplicacion =" + IdAplicacion;
            var dt = Conexion.SqlDataTable(sql,deadlock:true);

            cant = (int)dt.Rows[0]["cant"];

            if (cant > 0) return true; else return false;
        }

        string CrearUsuario(string CodUsuario, string Password, string Empresa, string Nombres, string Apellidos, string IdAplicacion, string Tipo, string Origen)
        {
            string sql;

            string IdUsuario;
            string IdVersion, IdTipo, IdOrigen, IdEstadoVenta, IdEstadoXQNo;
            string IdCargo, CodPais;
            //string Nombres = "Usuario";
            //string Apellidos = CodUsuario.Substring(3, CodUsuario.Length - 3);

            IdVersion = ObtieneIdAdminValor("02VER", "Starter Pack");
            IdTipo = ObtieneIdAdminValor("03TIP", Tipo);
            IdOrigen = ObtieneIdAdminValor("04ORG", Origen);
            IdEstadoVenta = ObtieneIdAdminValor("06EVT");
            IdEstadoXQNo = ObtieneIdAdminValor("07EXN");
            CodPais = "PE";
            IdCargo = ObtieneIdAdminValor("05CAR");

            sql = "insert into Usuario(CodUsuario, Password, Empresa, Nombres, Apellidos, ";
            sql += "IdAplicacion, IdTipo, IdOrigen, IdEstadoVenta, IdEstadoXQNo, CodPais, IdCargo, CodEstado, FecInicio, FecFin, ";
            sql += "CodSeguridad, CantUsuariosUnicos, FecRegistro, FecActualizacion) values ";
            sql += "('" + CodUsuario + "', '" + Password + "', '" + Empresa + "', '" + Nombres + "', '" + Apellidos + "', ";
            sql += IdAplicacion + ", " + IdTipo + ", " + IdOrigen + ", " + IdEstadoVenta + ", " + IdEstadoXQNo + ", ";
            sql += "'" + CodPais + "', " + IdCargo + ", 'A', " + DateTime.Now.ToString("yyyyMMdd") + "," + DateTime.Now.AddYears(1).ToString("yyyyMMdd") + ", 'IP', 1, getdate(), getdate())";

            Conexion.SqlExecute(sql);

            IdUsuario = BuscarIdUsuario(CodUsuario, IdAplicacion);

            sql = "insert into Suscripcion(IdUsuario, IdAplicacion, IdVersion, CodPais) ";
            sql += "select " + IdUsuario + " as IdUsuario, " + IdAplicacion + " as IdAplicacion, " + IdVersion + " as IdVersion, CodPais ";
            sql += "from AdminPais2 where ";
            //sql += "from AdminPais2 where CodPais not in ('XX', 'PE_I', 'PE_E', 'EC_I', 'EC_E') ";            
            if (Origen == "ULIMA" || Origen == "ESAN")
                sql += "CodPais not in ('XX', 'MXD', 'PE_I', 'PE_E', 'EC_I', 'EC_E')";
            else
                sql += "CodPais not in ('XX', 'PE_I', 'PE_E', 'EC_I', 'EC_E')";

            Conexion.SqlExecute(sql);
            return IdUsuario;
        }

        public string ObtieneIdAdminValor(string CodVariable)
        {
            string sql;

            string IdAdminValor = "";

            sql = "select IdAdminValor from AdminValor where CodVariable = '" + CodVariable + "' and Orden = 1";
            var dt = Conexion.SqlDataTable(sql,deadlock:true);

            if (dt.Rows.Count > 0)
            {
                IdAdminValor = dt.Rows[0]["IdAdminValor"].ToString();
            }
            return IdAdminValor;
        }
        
        string ObtieneIdAdminValor(string CodVariable, string Valor)
        {
            string IdAdminValor = "";

            string sql = "select IdAdminValor from AdminValor where CodVariable = '" + CodVariable + "' and Valor = '" + Valor + "' ";
            var dt = Conexion.SqlDataTable(sql,deadlock:true);
            if (dt.Rows.Count > 0)
            {
                IdAdminValor = dt.Rows[0]["IdAdminValor"].ToString();
            }

            return IdAdminValor;
        }

        string BuscarIdUsuario(string CodUsuario, string IdAplicacion)
        {
            string sql;
            string IdUsuario = "";
            sql = "select IdUsuario from Usuario where  CodUsuario = '" + CodUsuario + "' and IdAplicacion =" + IdAplicacion;
            var dt = Conexion.SqlDataTable(sql,deadlock:true);
            if (dt.Rows.Count > 0) IdUsuario = dt.Rows[0]["IdUsuario"].ToString();
            return IdUsuario;
        }

        //MisBusquedas
        public ActionResult Redireccion(string inicio, string txtCodUsuario, string txtPassword,
            string compra)
        {
            var cultura = RouteData.Values["culture"] as string;  //cultura
            if (Session["IdUsuario"] == null)
            {
                if (string.IsNullOrEmpty(inicio) ||
                    string.IsNullOrEmpty(txtCodUsuario) ||
                    string.IsNullOrEmpty(txtPassword))
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    //Si la validación es correcta se asigna los valores de sesión como por ejemplo el IdUsuario
                    Validacion(txtCodUsuario, txtPassword, inicio);
                }

                if (Session["IdUsuario"] == null)
                {
                    Response.BufferOutput = true;
                    if (Session["Error"] != null)
                    {
                        return RedirectToAction("Mostrar", "Error", new { cultura });
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    Extensiones.SetCookie("VLU", "1", TimeSpan.FromDays(360)); //JANAQ 140720 Marca login Universidad
                    Extensiones.SetCookie("Mb", "1", TimeSpan.FromDays(360));
                    Session["University"] = "1";
                    return RedirectToAction("Index", "MisBusquedas", new { compra });
                }
            }
            else
            {
                Extensiones.SetCookie("VLU", "1", TimeSpan.FromDays(360)); //JANAQ 140720 Marca login Universidad
                Extensiones.SetCookie("Mb", "1", TimeSpan.FromDays(360));
                Session["University"] = "1";
                return RedirectToAction("Index", "MisBusquedas", new { compra });
            }
        }

        private ActionResult Validacion(string txtCodUsuario, string txtPassword, string inicio)
        {
            string IdUsuario = "", IdAplicacion = "", CodSeguridad = "";
            int CantUsuariosUnicos = 0;

            string CodUsuario = txtCodUsuario.ToUpper();

#if DEBUG
            string DireccionIP = Properties.Settings.Default.IP_Debug;

#else
            string DireccionIP = Request.ServerVariables["REMOTE_ADDR"];
#endif
            if (Funciones.BuscaCodEstado(CodUsuario) == "I")
            {
                Session["Error"] = "INACTIVO";
                //return RedirectToAction("Logout", "Common");
                return RedirectToAction("Index", "Error");
            }

            bool Valida = _ws. /*Funciones.*/ Valida2(CodUsuario, txtPassword, ref IdUsuario, ref IdAplicacion, ref CodSeguridad,
                ref CantUsuariosUnicos);

            if (!Valida)
            {
                Session["Error"] = "INCORRECTO";
                //return RedirectToAction("Logout", "Common");
                return RedirectToAction("Index", "Error");
            }

            string Origen = Funciones.ObtieneOrigen(IdUsuario);


            if (Valida && (CodUsuario == "UPC" || Origen == "ULIMA" || Origen == "ESAN" || Origen == "ADEX" ||
                           Origen == "UPN" || Origen == "PUCP" || Origen == "DELOITTE" || Origen == "ITP") &&

                inicio != "referid0")
            {
                Session["Error"] = "INCORRECTO";

                //return RedirectToAction("Logout", "Common");
                return RedirectToAction("Index", "Error");
            }


            /*if (CodSeguridad == "IP" && 
                DireccionIP != Properties.Settings.Default.IP_Veritrade &&
                !_ws.ValidaIPPais(IdUsuario, DireccionIP))

            {
                Session["Error"] = "OTRO_PAIS";
                //return RedirectToAction("Logout", "Common");
                return RedirectToAction("Index", "Error");
            }*/

            if (CodSeguridad != "Off")
            {
                if (Funciones.SessionUnica(CodUsuario) && Funciones.ExisteUsuarioEnLinea(IdUsuario))
                {
                    Session["Error"] = "SESION_UNICA";
                    Session["CodUsuarioTemp"] = CodUsuario;
                    //return RedirectToAction("Logout", "Common");
                    return RedirectToAction("Index", "Error");
                }
            }





            int LimiteVisitas = 0, Visitas = 0;

            if (!Funciones.ValidaVisitasMes(IdUsuario, ref LimiteVisitas, ref Visitas))
            {

                Session["Error"] = "LIMITE_VISITAS";
                //return RedirectToAction("Logout", "Common");
                return RedirectToAction("Index", "Error");
            }

            FuncionesBusiness.GrabaHistorial(IdUsuario, DireccionIP, Request.ServerVariables["HTTP_USER_AGENT"], "");


            Session["IdUsuario"] = IdUsuario;


            var onlineUser = new OnlineUsers
            {
                IdSesion = Session.SessionID,
                IdUsuario = Session["IdUsuario"].ToString()

            };

            ActiveSessions.Sessions.AddOrUpdate(Session.SessionID, onlineUser, (k, v) => v = onlineUser);

            Session["TipoUsuario"] = Funciones.BuscaTipoUsuario(IdUsuario);
            Session["Plan"] = Funciones.ObtienePlan(IdUsuario).ToUpper();
            Session["Origen"] = Funciones.ObtieneOrigen(IdUsuario).ToUpper();

            // Ruben 202210
            if (Session["Origen"].ToString() == "ULIMA" || Session["Origen"].ToString() == "UPC" || Session["Origen"].ToString() == "ESAN" || Session["Origen"].ToString() == "ADEX" ||
                Session["Origen"].ToString() == "UPN" || Session["Origen"].ToString() == "UPT" || Session["Origen"].ToString() == "UNIV" || Session["Origen"].ToString() == "PUCP" ||
                Session["Origen"].ToString() == "CERTUS" || Session["Origen"].ToString() == "SISE" || Session["Origen"].ToString() == "UCV")
                Session["Plan"] = "UNIVERSIDADES";
            else if (Session["Plan"].ToString() == "ESENCIAL" || Session["Plan"].ToString() == "PERU UNO" || Session["Plan"].ToString() == "ECUADOR UNO" ||
                Session["Plan"].ToString() == "PERU IMEX" || Session["Plan"].ToString() == "ECUADOR IMEX")
            {
                string CodPais = "", TipoOpe = "";
                Funciones.BuscaDatosPlanEspecial(IdUsuario, ref CodPais, ref TipoOpe);
                Session["CodPais"] = CodPais;
                Session["TipoOpe"] = TipoOpe;
            }

            return null;
        }
    }
}