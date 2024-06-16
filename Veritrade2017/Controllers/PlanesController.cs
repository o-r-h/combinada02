using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;
using Newtonsoft.Json.Linq;
using Stripe;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.Mvc;
using DevTrends.MvcDonutCaching;
using Veritrade2017.Helpers;
using Veritrade2017.Models;
using System.Net;
using Transbank.Webpay;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Collections;
using System.Globalization;
using System.Reflection;

using System.Data;

namespace Veritrade2017.Controllers
{
    public class PlanesController : BaseController
    {
        private readonly VeritradeAdmin.Seguridad _ws = new VeritradeAdmin.Seguridad();
        private readonly VeritradeServicios.ServiciosCorreo _wsc = new VeritradeServicios.ServiciosCorreo();
        private readonly string _path = SettingUtility.GetUrlBackOld();
        private readonly string _mensajeFactura = "Enviar Factura a: ";

        //ALIGNET
        private readonly string _idcommerce = Properties.Settings.Default.AlignetIdCommerce;

        private readonly string _idwallet = Properties.Settings.Default.AlignetIdWallet;
        private readonly string _idadquirente = Properties.Settings.Default.AlignetIdAdquiriente;
        private readonly string _moneda = Properties.Settings.Default.AlignetMoneda;
        private readonly string _codpedido = Properties.Settings.Default.AlignetCodPedido;
        private readonly string _passwallet = Properties.Settings.Default.AlignetPassWallet;
        private readonly string _passpasarela = Properties.Settings.Default.AlignetPassPasarela;
        private readonly string _enviroment = Properties.Settings.Default.AlignetEnviroment;

        //Stripe
        private readonly string _apikey = Properties.Settings.Default.StripeApiKey;
        
        [HttpGet]
        [DonutOutputCache(CacheProfile = "CacheExtraLarge")]
        public ActionResult Index(string culture)
        {
            if (Session["IdUsuario"] != null)
            {
                var idusuario = Convert.ToInt32(Session["IdUsuario"].ToString());
                var usuarioRegistrado = Usuario.GetUsuario(idusuario);
                ViewData["registrado"] = usuarioRegistrado.Nombres;
            }

            ViewBag.Menu = "planes";
            //ViewData["CodCampaña"] = culture.Equals("es") ? "51001" : "51001I";

            var codPaisIp = string.Empty;
#if DEBUG
            var direccionIp = Properties.Settings.Default.IP_Debug;
#else
            var direccionIp = Request.ServerVariables["REMOTE_ADDR"];
#endif
            _ws.BuscaUbicacionIP2(direccionIp, ref codPaisIp);
            Session["IPPaisIngreso"] = codPaisIp;

            ViewData["igv"] = "";
            if (codPaisIp == "PE")
                ViewData["igv"] = "+ IGV";

            ViewData["igv2"] = ""; // Ruben 202404

            var planPrecio = Planes.GetPlanPrecio(codPaisIp);

            //var c = "";
            //if (RouteData.Values["campania"] != null) c = RouteData.Values["campania"].ToString();

            //if (Session["c"] != null && c != "") c = Session["c"].ToString();

            //if (c == "") c = culture.Equals("es") ? "51001" : "51001I";

            //if (Session["c"] == null) Session["c"] = c;

            ////var campania = Session["c"].ToString();
            //var c = "";
            //if (RouteData.Values["campania"] != null) c = RouteData.Values["campania"].ToString();
            //if (Session["c"] != null)
            //{
            //    if (Session["c"].ToString() == "51001" || Session["c"].ToString() == "51001I")
            //    {
            //        c = Session["c"].ToString();
            //    }
            //}

            //Session.RemoveAll();

            //if (c != "" && (c == "51001" || c == "51001I"))
            //{
            //    Session["c"] = c;
            //    ViewData["CodCampaña"] = c;
            //}
            ////Nuevo código Gabriel
            //else
            //{
            //    var arr = new string[] { "14100", "14100I", "20100", "20100I" };
            //    if (!arr.Contains(c) && !string.IsNullOrEmpty(c))
            //    {
            //        Session["c"] = c;
            //        ViewData["CodCampaña"] = c;
            //    }
            //    else
            //    {
            //        ViewData["CodCampaña"] = culture.Equals("es") ? "51001" : "51001I";
            //        Session["c"] = ViewData["CodCampaña"].ToString();
            //    }
            //}

            ViewData["idUsuario"] = string.Empty;
            ViewData["serviciosMenu"] = new Servicios().GetServicios(culture);
            ViewData["paisesMenu"] = ServiciosPaises.GetList(culture);
            ViewData["planes"] = Planes.GetPlanes(culture, codPaisIp, planPrecio);

            // Ruben 202306
            ViewData["CodMonedaPlan"] = "USD";
            ViewData["codPaisIp"] = codPaisIp;

            string sql = "select Region from VeritradeAdmin.dbo.V_Region_Pais where CodPais = '" + codPaisIp + "'";

            DataTable dt2 = Conexion.SqlDataTable(sql);
            if (dt2.Rows.Count > 0)
            {
                DataRow dr2 = dt2.Rows[0];
                string Region = dr2["Region"].ToString();
                if (Region == "UNION EUROPEA")
                {
                    ViewData["CodMonedaPlan"] = "€";
                    ViewData["igv2"] = "+ IVA (si aplica)"; // Ruben 202404
                    if (culture == "en")
                        ViewData["igv2"] = "+ IVA (if applies)"; // Ruben 202404
                }
            }

            // Ruben 202303
            if (culture == "es")
                ViewBag.Canonical = "https://www.veritradecorp.com/es/planes";
            else
                ViewBag.Canonical = "https://www.veritradecorp.com/en/plans";

            return View();
        }

        [HttpPost, ActionName("Index")]
        public ActionResult IndexPost(string culture)
        {
            if (Session["IdUsuario"] != null)
            {
                var idusuario = Convert.ToInt32(Session["IdUsuario"].ToString());
                var usuarioRegistrado = Usuario.GetUsuario(idusuario);
                ViewData["registrado"] = usuarioRegistrado.Nombres;
            }

            ViewBag.Menu = "planes";

            var codPaisIp = string.Empty;
#if DEBUG
            var direccionIp = Properties.Settings.Default.IP_Debug;
#else
            var direccionIp = Request.ServerVariables["REMOTE_ADDR"];
#endif
            _ws.BuscaUbicacionIP2(direccionIp, ref codPaisIp);

            if (codPaisIp == "PE")
            {
                ViewData["igv"] = "+IGV";
            }
            else
            {
                ViewData["igv"] = " ";
            }            

            var planPrecio = Planes.GetPlanPrecio(codPaisIp);

            var id = Convert.ToInt32(Request.Form["idUsuario"]);
            var usuario = Usuario.GetUsuario(id);
            Session["Empresa"] = usuario.Empresa;
            Session["CodPais"] = usuario.CodPais;
            Session["Nombres"] = usuario.Nombres;
            Session["Apellidos"] = usuario.Apellidos;
            Session["Telefono"] = usuario.Telefono;
            Session["Email1"] = usuario.Email1;

            ViewData["Email1"] = usuario.Email1;
            ViewData["serviciosMenu"] = new Servicios().GetServicios(culture);
            ViewData["paisesMenu"] = ServiciosPaises.GetList(culture);
            ViewData["planes"] = Planes.GetPlanes(culture, codPaisIp, planPrecio);

            // Ruben 202303
            if (culture == "es")
                ViewBag.Canonical = "https://www.veritradecorp.com/es/planes";
            else
                ViewBag.Canonical = "https://www.veritradecorp.com/en/plans";

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Cotizacion(Solicitud model)
        {
            if (ModelState.IsValid)
            {
                var codSolicitud = "COTIZACION";
                var nombres = model.NombreCompleto;
                var empresa = model.Empresa;
                var telefono = model.Email;
                var email1 = model.Email;
                var mensaje = model.Mensaje;
#if DEBUG
                var direccionIp = Properties.Settings.Default.IP_Debug;
#else
                var direccionIp = Request.ServerVariables["REMOTE_ADDR"];
#endif
                Solicitud.EnviaSolicitud(codSolicitud, nombres, empresa, telefono, email1, mensaje, direccionIp);

                return Json(new
                {
                    EnableSuccess = true,
                    SuccessTitle = Resources.Resources.Request_Title,
                    SuccessMsg = Resources.Resources.Request_Message
                }, JsonRequestBehavior.AllowGet);
            }

            return Json(new
            {
                EnableError = true,
                ErrorTitle = Resources.Resources.Request_Error_Title,
                ErrorMsg = Resources.Resources.Request_Error_Message
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [DonutOutputCache(CacheProfile = "CacheExtraLarge")]
        public ActionResult Compra(string culture, int id)
        {
            if (Session["IdUsuario"] != null)
            {
                var idusuario = Convert.ToInt32(Session["IdUsuario"].ToString());
                var usuarioRegistrado = Usuario.GetUsuario(idusuario);
                ViewData["registrado"] = usuarioRegistrado.Nombres;
            }

            ViewBag.Menu = "planes";
            ViewData["serviciosMenu"] = new Servicios().GetServicios(culture);
            ViewData["paisesMenu"] = ServiciosPaises.GetList(culture);
            ViewData["plan"] = Planes.GetDetallePlan(id);

            var codPaisIp = string.Empty;
#if DEBUG
            var direccionIp = Properties.Settings.Default.IP_Debug;
#else
            var direccionIp = Request.ServerVariables["REMOTE_ADDR"];
#endif
            _ws.BuscaUbicacionIP2(direccionIp, ref codPaisIp);

            var planPrecio = Planes.GetPlanPrecio(codPaisIp);

            var c = string.Empty;
            if (Session["c"] != null) c = Session["c"].ToString();
            ViewData["cod_campaña"] = c;

            if (!Planes.PlanExist(id, codPaisIp, planPrecio))
            {
                return RedirectToAction("Index");
            }

            ViewData["igv"] = GetIgv(ref codPaisIp);
            ViewData["pais"] = codPaisIp;
            ViewData["planId"] = id;

            if (Session["numeroOperacion"] != null)
            {
                var codigo = Session["numeroOperacion"].ToString();
                PedidosPayMe.LeaveCode(codigo);
                Session["numeroOperacion"] = string.Empty;
            }

            var empresa = string.Empty;
            if (Session["Empresa"] != null)
            {
                empresa = Session["Empresa"].ToString();
            }
            ViewData["Empresa"] = empresa;

            var pais = string.Empty;
            if (Session["CodPais"] != null)
            {
                pais = Session["CodPais"].ToString();
            }
            ViewData["CodPais"] = pais;

            var nombre = string.Empty;
            if (Session["Nombres"] != null)
            {
                nombre = Session["Nombres"].ToString();
            }
            ViewData["Nombres"] = nombre;

            var apellidos = string.Empty;
            if (Session["Apellidos"] != null)
            {
                apellidos = Session["Apellidos"].ToString();
            }
            ViewData["Apellidos"] = apellidos;

            var telefono = string.Empty;
            if (Session["Telefono"] != null)
            {
                telefono = Session["Telefono"].ToString();
            }
            ViewData["Telefono"] = telefono;

            var email = string.Empty;
            if (Session["Email1"] != null)
            {
                email = Session["Email1"].ToString();
            }
            ViewData["Email1"] = email;

            if (codPaisIp == "PE")
            {
                ViewData["metodo"] = "payme";
            }
            else if (codPaisIp == "CL" && Convert.ToBoolean(Properties.Settings.Default.transbankHabilitado)) {
                ViewData["metodo"] = "transbank";
            }
            else
            {
                ViewData["metodo"] = "stripe";
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ValidarPerfilTransbank(CompraPerfil model, CompraFactura factura, int planId, string campania, string culture)
        {
            var codPaisIp = string.Empty;
            var igv = GetIgv(ref codPaisIp);

            var planPrecioAux = Planes.GetPlanPrecio(codPaisIp);
            if (ModelState.IsValid && Planes.PlanExist(planId, codPaisIp, planPrecioAux))
            {
                // Ruben 202206
                CreaUsuarioFormPlan(model.Nombres, model.Apellidos, model.Empresa, model.Email, model.Telefono, model.Pais);

                if (codPaisIp == "CL")
                {
                    var planDetalle = Planes.GetDetallePlan(planId);
                    var planPrecio = PlanesPrecio.GetPrecioPlan(planId);
                    
                    planPrecio.Precio *= ValorDolar.traerValorActual();
                    planPrecio.Precio += Math.Round(Convert.ToDecimal(igv) * planPrecio.Precio, 2); 
                    var configuration = new Configuration();

#if DEBUG
                    configuration = Configuration.ForTestingWebpayPlusNormal();
                    



#else
                    configuration.Environment = "PRODUCCION";
                    configuration.WebpayCertPath = Properties.Settings.Default.PublicCertPath;

                    configuration.CommerceCode = Properties.Settings.Default.CodTransbankProduccion;
                    configuration.PrivateCertPfxPath = Properties.Settings.Default.PrivateCertPath;
                    configuration.Password = Properties.Settings.Default.PrivateCertPass;
#endif
                    /** Creacion Objeto Webpay */

                    var transaction = new Webpay(configuration);

                    /** Crea URL de Aplicación */
                    string baseurl = Properties.Settings.Default.UrlWeb;
                    //string baseurl = "https://www.veritradecorp.com/";


                    /** URL Retorno */
                    string urlReturn =baseurl + "/" + culture + "/compra/ValidarRetornoTransbank";

                    /** Monto de la transacción */
                    decimal amount = Decimal.Truncate(planPrecio.Precio);

                    /** Orden de compra de la tienda */
                    string buyOrder = PedidosTransbank.GenNewCode();

                    /** URL Final */
                    string urlFinal = baseurl + "/" + culture + "/compra/FinTransbank/" + buyOrder;

                    /** (Opcional) Identificador de sesión, uso interno de comercio */
                    string sessionId = Session.SessionID;

                    var result = transaction.NormalTransaction.initTransaction(amount, buyOrder, sessionId, urlReturn, urlFinal);

                    var pedidoTransbank = new PedidosTransbank
                    {
                        OrdenCompra = buyOrder,
                        Nombres = model.Nombres,
                        Apellidos = model.Apellidos,
                        Email = model.Email,
                        Empresa = model.Empresa,
                        Telefono = model.Telefono,
                        Pais = model.Pais,
                        Token = result.token,
                        Procesado = false,
                        Aprobado = false,
                        FechaCreacion = DateTime.Now,
                        FechaActualizacion = DateTime.Now,
                        Monto = amount.ToString(),
                        IdPlan = planId,
                        FacturaDireccion = factura.Direccion,
                        FacturaRazonSocial = factura.RazonSocial,
                        FacturaRuc = factura.Ruc
                    };

                    PedidosTransbank.Save(pedidoTransbank);

                    Session["transbankToken"] = result.token;
                    return Json(new
                    {
                        ErrorType = 0,
                        Token = result.token,
                        Url = result.url
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new
            {
                ErrorType = 1
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult TransaccionPruebaTransbank()
        {
            var configuration = new Configuration();
#if DEBUG
            configuration = Configuration.ForTestingWebpayPlusNormal();




#else
            configuration.Environment = "PRODUCCION";
            configuration.WebpayCertPath = Properties.Settings.Default.PublicCertPath;

            configuration.CommerceCode = Properties.Settings.Default.CodTransbankProduccion;
            configuration.PrivateCertPfxPath = Properties.Settings.Default.PrivateCertPath;
            configuration.Password = Properties.Settings.Default.PrivateCertPass;

#endif
            /** Creacion Objeto Webpay */

            var transaction = new Webpay(configuration);

            /** Crea URL de Aplicación */
            string baseurl = Properties.Settings.Default.UrlWeb;


            /** URL Retorno */
            string urlReturn = baseurl + "/es/compra/ValidarRetornoTransbank";

            

            /** Orden de compra de la tienda */
            string buyOrder = PedidosTransbank.GenNewCode();

            /** URL Final */
            string urlFinal = baseurl + "/es/compra/FinTransbank/" + buyOrder;

            /** (Opcional) Identificador de sesión, uso interno de comercio */
            string sessionId = Session.SessionID;
            try
            {
                var result = transaction.NormalTransaction.initTransaction(10, buyOrder, sessionId, urlReturn, urlFinal);

                var pedidoTransbank = new PedidosTransbank
                {
                    OrdenCompra = buyOrder,
                    Nombres = "Usuario",
                    Apellidos = "Prueba",
                    Email = "usuario.prueba@pruebaZeke.cl",
                    Empresa = "zeke",
                    Telefono = "123123",
                    Pais = "CL",
                    Token = result.token,
                    Procesado = false,
                    Aprobado = false,
                    FechaCreacion = DateTime.Now,
                    FechaActualizacion = DateTime.Now,
                    Monto = "10",
                    IdPlan = 1,
                    FacturaDireccion = "",
                    FacturaRazonSocial = "",
                    FacturaRuc = ""
                };

                PedidosTransbank.Save(pedidoTransbank);

                Session["transbankToken"] = result.token;
                ViewData["ErrorType"] = 0;
                ViewData["Token"] = result.token;
                ViewData["Url"] = result.url;

            } catch(Exception e)
            {
                ViewData["ErrorType"] = 1;
                ViewData["Token"] = "123";
                ViewData["Url"] = "123";
                ViewData["ex"] = e.Message;
            }

            ViewData["serviciosMenu"] = new Servicios().GetServicios("es");
            ViewData["paisesMenu"] = ServiciosPaises.GetList("es");
            ViewData["ayuda"] = new Ayuda().GetAyuda("es");
            return View("Compra/prueba_transbank");
      
        }

        [HttpPost]
        public ActionResult ValidarRetornoTransbank(string culture, string token_ws)
        {
            var codes = new Dictionary<string, string>
            {
                { "0", "Transacci&oacute;n aprobada" },
                { "-1", "Rechazo de transacci&oacute;n" },
                { "-2", "Transacci&oacute;n debe reintentarse" },
                { "-3", "Error en transacci&oacute;n" },
                { "-4", "Rechazo de transacci&oacute;n" },
                { "-5", "Rechazo por error de tasa" },
                { "-6", "Excede cupo m&aacute;ximo mensual" },
                { "-7", "Excede l&iacute;mite diario por transacci&oacute;n" },
                { "-8", "Rubro no autorizado" }
            };
            var configuration = new Configuration();
#if DEBUG
            configuration = Configuration.ForTestingWebpayPlusNormal();




#else
                    configuration.Environment = "PRODUCCION";
                    configuration.WebpayCertPath = Properties.Settings.Default.PublicCertPath;

                    configuration.CommerceCode = Properties.Settings.Default.CodTransbankProduccion;
                    configuration.PrivateCertPfxPath = Properties.Settings.Default.PrivateCertPath;
                    configuration.Password = Properties.Settings.Default.PrivateCertPass;
#endif
            /** Creacion Objeto Webpay */
            var transaction = new Webpay(configuration);
            var result = transaction.NormalTransaction.getTransactionResult(token_ws);
            var transactionLog = new CompraTransbankLog
            {
                Token = token_ws,
                AccountingDate = result.accountingDate,
                BuyOrder = result.buyOrder,
                CardNumber = result.cardDetail.cardNumber,
                Amount = result.detailOutput[0].amount,
                AuthorizationCode = result.detailOutput[0].authorizationCode,
                CommerceCode = result.detailOutput[0].commerceCode,
                PaymentTypeCode = result.detailOutput[0].paymentTypeCode,
                ResponseCode = result.detailOutput[0].responseCode,
                ResponseText = codes[result.detailOutput[0].responseCode.ToString()],
                SharesAmount = result.detailOutput[0].sharesAmount,
                SharesNumber = result.detailOutput[0].sharesNumber,
                SharesNumberSpecified = result.detailOutput[0].sharesNumberSpecified,
                SessionId = result.sessionId,
                TransactionDate = result.transactionDate,
                TransactionDateSpecified = result.transactionDateSpecified,
                VCI = result.VCI
            };

            CompraTransbankLog.SaveLog(transactionLog);
            //var ack = false;
            var message = "";
            /*try {
                ack = transaction.NormalTransaction.acknowledgeTransaction(token_ws);
            } catch(Exception e)
            {
                message = "Pago RECHAZADO por timeout webpay [Codigo]=> " + result.detailOutput[0].responseCode + " [Descripcion]=> " + codes[result.detailOutput[0].responseCode.ToString()];
                return Json(new
                {
                    message = message
                }, JsonRequestBehavior.AllowGet);
            }*/


            PedidosTransbank pedidoTransbank = PedidosTransbank.GetByBuyOrder(result.buyOrder);
            pedidoTransbank.Procesado = true;
            pedidoTransbank.FechaActualizacion = DateTime.Now;
            if (result.detailOutput[0].responseCode == 0)
            {
                
                message = "Pago ACEPTADO por webpay (se deben guardar datos para mostrar voucher)";
                pedidoTransbank.Aprobado = true;
                var planId = Convert.ToInt32(pedidoTransbank.IdPlan);

                var nombre = pedidoTransbank.Nombres + " " + pedidoTransbank.Apellidos;
                var nombre_ = pedidoTransbank.Nombres;
                var apellidos_ = pedidoTransbank.Apellidos;
                var email = pedidoTransbank.Email;
                var monto = pedidoTransbank.Monto;
                var pais = pedidoTransbank.Pais;
                var telefono = pedidoTransbank.Telefono;
                var empresa = pedidoTransbank.Empresa;
                var descripcion = Planes.GetDetallePlan(planId).Nombre;
                var facturaDireccion = pedidoTransbank.FacturaDireccion ?? "";
                var facturaRazonSocial = pedidoTransbank.FacturaRazonSocial ?? "";
                var facturaRuc = pedidoTransbank.FacturaRuc ?? "";
                var montoSinIgv = Convert.ToDecimal(pedidoTransbank.Monto);
                var codCampania = (Session["c"] as string);

                
                var clave = CreaUsuarioPlan(email, nombre_, apellidos_, empresa, telefono, culture, codCampania, pais,
                    planId, descripcion, montoSinIgv, facturaRuc, facturaDireccion, facturaRazonSocial);
                

                

                var idUsuario = Convert.ToInt32(Usuario.GetIdUsuario(email));
                pedidoTransbank.IdUsuario = idUsuario;

                var registroCompras = new Compras
                {
                    IdUsuario = idUsuario,
                    NombrePlan = descripcion,
                    Monto = Convert.ToInt32(Convert.ToDecimal(monto)),
                    Nombre = nombre,
                    Correo = email,
                    Empresa = empresa,
                    Telefono = telefono,
                    Pais = pais,
                    FacturaRuc = facturaRuc,
                    FacturaDireccion = facturaDireccion,
                    FacturaRazonSocial = facturaRazonSocial,
                    Pasarela = 2
                };

                Compras.InsertCompra(registroCompras);
            }
            else
            {
                pedidoTransbank.Aprobado = false;
                message = "Pago RECHAZADO por webpay [Codigo]=> " + result.detailOutput[0].responseCode + " [Descripcion]=> " + codes[result.detailOutput[0].responseCode.ToString()];
            }
            PedidosTransbank.Save(pedidoTransbank);
            ViewData["message"] = message;
            ViewData["token"] = token_ws;
            ViewData["resultado"] = result.detailOutput[0].responseCode;
            ViewData["url"] = result.urlRedirection;
            return PartialView("Compra/Retorno_Transbank");
        }

        [HttpPost]
        public ActionResult FinTransbank(string culture, string buyOrder)
        {
            /*return Json(new
            {
                message = "Fin transbank"
            }, JsonRequestBehavior.AllowGet);*/
            string token = "";
            if (Request.Form["token_ws"] != null) {
                token = Request.Form["token_ws"];
            }
            else if (Request.Form["TBK_TOKEN"] != null)
            {
                token = Request.Form["TBK_TOKEN"];
            }
            PedidosTransbank pedidoTransbank = PedidosTransbank.GetByBuyOrder(buyOrder);
            
            ViewData["serviciosMenu"] = new Servicios().GetServicios(culture);
            ViewData["paisesMenu"] = ServiciosPaises.GetList(culture);
            if (pedidoTransbank.Aprobado) {
                Usuario usuario = Usuario.GetUsuario(pedidoTransbank.IdUsuario);
                string pass = Usuario.GetPassword(usuario.Email1);
                ViewData["User"] = usuario.Email1;
                ViewData["Password"] = pass;
                return View("Confirmacion_Compra");
            } else
            {
                return View("Error_Compra");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ValidarPerfil(CompraPerfil model, CompraFactura factura, int planId, string campania)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                   | SecurityProtocolType.Tls11
                   | SecurityProtocolType.Tls12
                   | SecurityProtocolType.Ssl3;

            var codPaisIp = string.Empty;
            var igv = GetIgv(ref codPaisIp);

            var planPrecioAux = Planes.GetPlanPrecio(codPaisIp);

            if (ModelState.IsValid && Planes.PlanExist(planId, codPaisIp, planPrecioAux))
            {
                // Ruben 202206
                CreaUsuarioFormPlan(model.Nombres, model.Apellidos, model.Empresa, model.Email, model.Telefono, model.Pais);

                if (codPaisIp == "PE")
                {
                    var planDetalle = Planes.GetDetallePlan(planId);
                    var planPrecio = PlanesPrecio.GetPrecioPlan(planId);

                    var idEntCommerce = _idwallet;
                    var codCardHolderCommerce = "USU001";
                    var names = model.Nombres;
                    var lastNames = model.Apellidos;
                    var mail = model.Email;
                    var reserved1 = model.Pais;
                    var reserved2 = model.Telefono;
                    var reserved3 = model.Empresa ?? string.Empty;
                    var reserved4 = planId.ToString();
                    var reserved5 = planDetalle.Nombre;
                    var reserved6 = factura.Direccion ?? string.Empty;
                    var reserved7 = factura.RazonSocial ?? string.Empty;
                    var reserved8 = factura.Ruc ?? string.Empty;
                    var reserved9 = Convert.ToInt32(planPrecio.Precio).ToString();
                    var reserved10 = campania;

                    //Clave SHA-2 de Wallet
                    var claveSecretaWallet = _passwallet;

                    //Creacion del registerVerification para el envio a Wallet
                    var registerVerification =
                        Sha2.getStringSHA(idEntCommerce + codCardHolderCommerce + mail + claveSecretaWallet);

                    //string ansCode = "";
                    string ansDescription;
                    string codAsoCardHolderWallet;
                    string date;
                    string hour;

                    //Inicializacion del Servicio Web y de parametros de envio
                    string result1;
                    if (_enviroment == "TEST")
                    {
                        //Consumo del metodo RegisterCardHolder y recepcion del resultado encriptado
                        var walletCommerce = new PaymeTestReference.WalletCommerceClient();
                        result1 = walletCommerce.RegisterCardHolder(idEntCommerce, codCardHolderCommerce, mail, names,
                            lastNames, reserved1, reserved2, reserved3, registerVerification, out ansDescription,
                            out codAsoCardHolderWallet, out date, out hour);
                    }
                    else
                    {
                        //Consumo del metodo RegisterCardHolder y recepcion del resultado encriptado
                        var walletCommerce = new PaymeReference.WalletCommerceClient();
                        result1 = walletCommerce.RegisterCardHolder(idEntCommerce, codCardHolderCommerce, mail, names,
                            lastNames, reserved1, reserved2, reserved3, registerVerification, out ansDescription,
                            out codAsoCardHolderWallet, out date, out hour);
                    }

                    //CONEXION A VPOS2 
                    var adquiriente = _idadquirente;
                    var comercio = _idcommerce;
                    var monto = Convert.ToInt32((planPrecio.Precio + igv * planPrecio.Precio) * 100)
                        .ToString(); // Monto en centavos


                    var moneda = _moneda;

                    string numeroOperacion = PedidosPayMe.GetLastCode();
                    ConsultaCodigo(ref numeroOperacion);
                    Session["numeroOperacion"] = numeroOperacion;
                    PedidosPayMe.SetCode(numeroOperacion);

                    //Clave SHA-2.
                    var claveSecretaPasarela = _passpasarela;
                    var purVerification =
                        Sha2.getStringSHA(adquiriente + comercio + numeroOperacion + monto + moneda +
                                          claveSecretaPasarela);

                    var payMe = new CompraPayMe
                    {
                        AcquirerId = adquiriente,
                        IdCommerce = comercio,
                        PurchaseAmount = monto,
                        PurchaseCurrencyCode = moneda,
                        PurchaseOperationNumber = numeroOperacion,
                        PurchaseVerification = purVerification,
                        IdEntCommerce = idEntCommerce,
                        CodCardHolderCommerce = codCardHolderCommerce,
                        Names = names,
                        LastNames = lastNames,
                        Mail = mail,
                        Lenguaje = "SP",
                        UserCodePayme = result1,
                        Reserved1 = reserved1,
                        Reserved2 = reserved2,
                        Reserved3 = reserved3,
                        Reserved4 = reserved4,
                        Reserved5 = reserved5,
                        Reserved6 = reserved6,
                        Reserved7 = reserved7,
                        Reserved8 = reserved8,
                        Reserved9 = reserved9,
                        Reserved10 = reserved10
                    };

                    return PartialView("Compra/Compra_Payme", payMe);
                }

                Session["CorreoUsuario"] = model.Email;
                var stripe = new CompraDetalle
                {
                    NombreTarjeta = model.Nombres.Trim() + " " + model.Apellidos.Trim(),
                    Correo = model.Email,
                    ChargeId = planId,
                    Telefono = model.Telefono,
                    Empresa = model.Empresa,
                    Pais = model.Pais,
                    CodCampania = campania,
                    IsAgree = true
                };
                ViewData["pais"] = codPaisIp;
                ViewData["names"] = model.Nombres;
                ViewData["LastNames"] = model.Apellidos;
                return PartialView("Compra/Compra_Section", stripe);
            }

            return Json(new
            {
                Success = false,
                ErrorType = 0,
                ErrorTitle = "Error",
                ErrorMsg = "Por favor, llene todos los campos obligatorios"
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Charged(string culture)
        {
            var transaction = new CompraPayMeLog
            {
                AuthorizationResult = Request.Form["authorizationResult"],
                AuthorizationCode = Request.Form["authorizationCode"],
                ErrorCode = Request.Form["errorCode"],
                ErrorMessage = Request.Form["errorMessage"],
                Bin = Request.Form["bin"],
                Brand = Request.Form["brand"],
                PaymentReferenceCode = Request.Form["paymentReferenceCode"],
                PurchaseVerification = Request.Form["purchaseVerification"],
                CardType = Request.Form["reserved22"],
                BankName = Request.Form["reserved23"],
                PurchaseOperationNumber = Request.Form["purchaseOperationNumber"]
            };

            if (transaction.AuthorizationResult == "00")
            {
                PedidosPayMe.EndCode(transaction.PurchaseOperationNumber);
            }
            CompraPayMeLog.SaveLog(transaction);

            ViewData["serviciosMenu"] = new Servicios().GetServicios(culture);
            ViewData["paisesMenu"] = ServiciosPaises.GetList(culture);

            if (transaction.AuthorizationResult == "00")
            {
                var nombre = Request.Form["shippingFirstName"] + " " + Request.Form["shippingLastName"];
                var nombre_ = Request.Form["shippingFirstName"];
                var apellidos_ = Request.Form["shippingLastName"];
                var email = Request.Form["shippingEMail"];
                var monto = Convert.ToInt32(Request.Form["purchaseAmount"]) / 100;
                var pais = Request.Form["reserved1"];
                var telefono = Request.Form["reserved2"];
                var empresa = Request.Form["reserved3"];
                var plan = Request.Form["reserved4"];
                var descripcion = Request.Form["reserved5"];
                var facturaDireccion = Request.Form["reserved6"];
                var facturaRazonSocial = string.IsNullOrEmpty(Request.Form["reserved7"])
                    ? ""
                    : _mensajeFactura + Request.Form["reserved7"];
                var facturaRuc = Request.Form["reserved8"];
                var montoSinIgv = Convert.ToDecimal(Request.Form["reserved9"]);
                var codCampania = (Session["c"] as string) ??  Request.Form["reserved10"];

                var planId = Convert.ToInt32(plan);
                var clave = CreaUsuarioPlan(email, nombre_, apellidos_, empresa, telefono, culture, codCampania, pais,
                    planId, descripcion, montoSinIgv, facturaRuc, facturaDireccion, facturaRazonSocial);

                var idUsuario = Convert.ToInt32(Usuario.GetIdUsuario(email));

                var registroCompras = new Compras
                {
                    IdUsuario = idUsuario,
                    NombrePlan = plan,
                    Monto = monto,
                    Nombre = nombre,
                    Correo = email,
                    Empresa = empresa,
                    Telefono = telefono,
                    Pais = pais,
                    FacturaRuc = facturaRuc,
                    FacturaDireccion = facturaDireccion,
                    FacturaRazonSocial = facturaRazonSocial,
                    Pasarela = 0
                };

                Compras.InsertCompra(registroCompras);

                ViewData["User"] = email;
                ViewData["Password"] = clave;
                return View("Confirmacion_Compra");
            }

            ViewData["error_message"] = "La transacción no se pudo realizar correctamente.";
            return View("Error_Compra");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Payment(CompraDetalle model, CompraFactura factura, string culture)
        {
            var plan = model.ChargeId;
            string codPaisIp = string.Empty;
#if DEBUG
            var direccionIp = Properties.Settings.Default.IP_Debug;
#else
            var direccionIp = Request.ServerVariables["REMOTE_ADDR"];
#endif
            _ws.BuscaUbicacionIP2(direccionIp, ref codPaisIp);            

            var planPrecioAux = Planes.GetPlanPrecio(codPaisIp);

            if (ModelState.IsValid && Planes.PlanExist(plan, codPaisIp, planPrecioAux))
            {
                // Ruben 202304
                CreaUsuarioFormPlan(Request.Form["Nombres"], Request.Form["Apellidos_"], Request.Form["Empresa"], Request.Form["Email"], Request.Form["Telefono"], Request.Form["Pais"]);

                StripeConfiguration.SetApiKey(_apikey);
                ViewData["serviciosMenu"] = new Servicios().GetServicios(culture);
                ViewData["paisesMenu"] = ServiciosPaises.GetList(culture);
                try
                {
                    var customers = new StripeCustomerService();
                    var charges = new StripeChargeService();
                    var planDetail = Planes.GetDetallePlan(plan);
                    var planPrecio = PlanesPrecio.GetPrecioPlan(plan);
                    var monto = Convert.ToInt32(planPrecio.Precio);
                    var descripcion = planDetail.Nombre;
                    var campania = model.CodCampania;

                    var customer = customers.Create(new StripeCustomerCreateOptions
                    {
                        Email = model.Correo,
                        Description = model.NombreTarjeta,
                        SourceToken = model.StripeToken
                    });

                    var charge = charges.Create(new StripeChargeCreateOptions
                    {
                        Amount = monto * 100,
                        Description = descripcion,
                        Currency = "usd",
                        CustomerId = customer.Id
                    });

                    var transaction = new CompraStripeLog()
                    {
                        ChargeId = charge.Id,
                        Message = charge.Outcome.SellerMessage,
                        Parameter = charge.Description,
                        CarId = charge.Source.Card.Id,
                        Brand = charge.Source.Card.Brand,
                        Country = charge.Source.Card.Country,
                        Last4 = charge.Source.Card.Last4,
                        Name = model.NombreTarjeta
                    };

                    CompraStripeLog.SaveLog(transaction);

                    var email = string.IsNullOrEmpty(Request.Form["Email"]) ? model.Correo : Request.Form["Email"];
                    var nombre = string.IsNullOrEmpty(Request.Form["Nombres"])
                        ? model.NombreTarjeta
                        : Request.Form["Nombres"];
                    var apellidos = Request.Form["Apellidos_"];
                    var empresa = Request.Form["Empresa"];
                    var telefono = Request.Form["Telefono"];
                    var pais = Request.Form["Pais"];

                    var ruc = factura.Ruc;
                    var direccion = factura.Direccion;
                    var razonSocial = string.IsNullOrEmpty(factura.RazonSocial)
                        ? ""
                        : _mensajeFactura + factura.RazonSocial;

                    var clave = CreaUsuarioPlan(email, nombre, apellidos, empresa, telefono, culture, campania,
                        pais, plan, descripcion, monto, ruc, direccion, razonSocial);

                    var idUsuario = Convert.ToInt32(Usuario.GetIdUsuario(model.Correo));

                    var registroCompras = new Compras
                    {
                        IdUsuario = idUsuario,
                        NombrePlan = descripcion,
                        Monto = monto,
                        Nombre = model.NombreTarjeta,
                        Correo = model.Correo,
                        Empresa = empresa,
                        Telefono = telefono,
                        Pais = pais,
                        FacturaRuc = factura.Ruc,
                        FacturaDireccion = factura.Direccion,
                        FacturaRazonSocial = factura.RazonSocial,
                        Pasarela = 1
                    };

                    Compras.InsertCompra(registroCompras);

                    ViewData["User"] = email;
                    ViewData["Password"] = clave;
                    return View("Confirmacion_Compra");
                }
                catch (StripeException e)
                {
                    var tokenService = new StripeTokenService();
                    StripeToken token = tokenService.Get(model.StripeToken);

                    var transaction = new CompraStripeLog()
                    {
                        ChargeId = e.StripeError.ChargeId,
                        Code = e.StripeError.Code,
                        DeclineCode = e.StripeError.DeclineCode,
                        Error = e.StripeError.Error,
                        ErrorDescription = e.StripeError.ErrorDescription,
                        ErrorType = e.StripeError.ErrorType,
                        Message = e.StripeError.Message,
                        Parameter = e.StripeError.Parameter,
                        CarId = token.StripeCard.Id,
                        Brand = token.StripeCard.Brand,
                        Country = token.StripeCard.Country,
                        Last4 = token.StripeCard.Last4,
                        Name = model.NombreTarjeta
                    };

                    CompraStripeLog.SaveLog(transaction);

                    ViewData["error_message"] = e.StripeError.Message;
                    return View("Error_Compra");
                }
            }

            return View("Compra/Compra_Section", model);
        }

        public FileResult Pdf(string culture)
        {
            string codPaisIp = string.Empty;
#if DEBUG
            var direccionIp = Properties.Settings.Default.IP_Debug;
#else
            var direccionIp = Request.ServerVariables["REMOTE_ADDR"];
#endif
            _ws.BuscaUbicacionIP2(direccionIp, ref codPaisIp);

            var planPrecio = Planes.GetPlanPrecio(codPaisIp);

            var planes = Planes.GetPlanes(culture, codPaisIp, planPrecio);

            using (var stream = new MemoryStream())
            {
                var standardFont = new Font(Font.FontFamily.HELVETICA, 8, Font.NORMAL, BaseColor.BLACK);
                var titleFont = new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD, BaseColor.BLACK);
                var headerFont = new Font(Font.FontFamily.HELVETICA, 14, Font.BOLD, BaseColor.BLACK);
                var spacing = new Font(Font.FontFamily.HELVETICA, 3, Font.BOLD, BaseColor.WHITE);

                var pdfDoc = new Document(PageSize.A4, 25, 10, 25, 10);
                var pdfWriter = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
                pdfDoc.Open();

#region Agregando logo en cabecera

                // PDF document size      
                Rectangle page = pdfDoc.PageSize;

                // create two column table
                float[] columnWidths = {1, 9};
                PdfPTable tblCabecera = new PdfPTable(columnWidths);

                tblCabecera.TotalWidth = page.Width * 0.3f;
                tblCabecera.HorizontalAlignment = Element.ALIGN_LEFT;

                // Configuramos el título de las columnas de la tabla

                float cellHeight = pdfDoc.TopMargin;

                var image = iTextSharp.text.Image.GetInstance(Server.MapPath("~/Content/Images/veritrade_logo.png"));
                PdfPCell CLogo = new PdfPCell(image);
                CLogo.Column.FilledWidth = 4f;
                CLogo.HorizontalAlignment = Element.ALIGN_LEFT;
                CLogo.FixedHeight = (float) (cellHeight * 1.5f);
                CLogo.Border = PdfPCell.NO_BORDER;


                var tituloDoc = culture.Equals("es")
                    ? "       Planes y Precios Veritrade"
                    : "       Veritrade Price and Plans";
                PdfPCell cTitulo = new PdfPCell(new Phrase(tituloDoc, headerFont));
                cTitulo.Border = PdfPCell.NO_BORDER;
                cTitulo.PaddingLeft = (float) 1f;
                cTitulo.VerticalAlignment = Element.ALIGN_MIDDLE;
                cTitulo.FixedHeight = cellHeight;


                // Añadimos las celdas a la tabla

                tblCabecera.AddCell(CLogo);
                tblCabecera.AddCell(cTitulo);

#endregion

                // Escribimos el encabezamiento en el documento
                pdfDoc.Add(tblCabecera);
                Chunk linebreak = new Chunk(new LineSeparator(1f, 100f, BaseColor.BLACK, Element.ALIGN_CENTER, -1));
                pdfDoc.Add(new Paragraph(" ", spacing));
                pdfDoc.Add(linebreak);
                pdfDoc.Add(new Paragraph(" ", spacing));
                pdfDoc.Add(Chunk.NEWLINE);


                // Creamos una tabla que contendrá el nombre, apellido y país
                // de nuestros visitante.
                var tblPrueba = new PdfPTable(4);
                tblPrueba.WidthPercentage = 100;

                // Configuramos el título de las columnas de la tabla
                var clTitulo = new PdfPCell(new Phrase(Resources.Resources.Plans_Title_Plans, titleFont));
                clTitulo.BorderWidth = 0;
                clTitulo.BorderWidthBottom = 0.75f;
                tblPrueba.AddCell(clTitulo);

                foreach (var plan in planes)
                {
                    clTitulo = new PdfPCell(new Phrase(plan.Nombre, titleFont));
                    clTitulo.BorderWidth = 0;
                    clTitulo.BorderWidthBottom = 0.75f;
                    tblPrueba.AddCell(clTitulo);
                }

                // Precios
                var clPrecios = new PdfPCell(new Phrase(Resources.Resources.Plans_Title_Price, standardFont));
                clPrecios.BorderWidth = 0;
                clPrecios.BorderWidthBottom = 0.75f;
                tblPrueba.AddCell(clPrecios);

                foreach (var plan in planes)
                {
                    var precio = PlanesPrecio.GetPrecioPlan(plan.Id);
                    var total = "US $ " + Convert.ToInt32(precio.Precio).ToString("N0") + " " +
                                precio.Periodo.Descripcion;

                    string sql = "select Region from VeritradeAdmin.dbo.V_Region_Pais where CodPais = '" + codPaisIp + "'";

                    DataTable dt2 = Conexion.SqlDataTable(sql);
                    if (dt2.Rows.Count > 0)
                    {
                        total = "€ " + Convert.ToInt32(precio.Precio).ToString("N0") + (culture == "es" ? " + IVA (si aplica)" : " + IVA (if applies)") + " " + precio.Periodo.Descripcion;
                    }

                    clPrecios = new PdfPCell(new Phrase(total, standardFont));
                    clPrecios.BorderWidth = 0;
                    clPrecios.BorderWidthBottom = 0.75f;
                    tblPrueba.AddCell(clPrecios);
                }

                // Paises
                var clPaises = new PdfPCell(new Phrase(Resources.Resources.Plans_Title_Countries, standardFont));
                clPaises.BorderWidth = 0;
                clPaises.BorderWidthBottom = 0.75f;
                tblPrueba.AddCell(clPaises);

                foreach (var plan in planes)
                {
                    var detalle = PlanesPrecioDetalle.GetPaisesDescripcion(plan.Id);
                    detalle = Regex.Replace(detalle, "<.*?>", String.Empty);
                    clPaises = new PdfPCell(new Phrase(detalle, standardFont));
                    clPaises.BorderWidth = 0;
                    clPaises.BorderWidthBottom = 0.75f;
                    tblPrueba.AddCell(clPaises);
                }

                // Informacion Comercial
                var clInfo = new PdfPCell(new Phrase(Resources.Resources.Plans_Title_Information, standardFont));
                clInfo.BorderWidth = 0;
                clInfo.BorderWidthBottom = 0.75f;
                tblPrueba.AddCell(clInfo);

                foreach (var plan in planes)
                {
                    var detalle = PlanesPrecioDetalle.GetInfoComexDescripcion(plan.Id);
                    detalle = Regex.Replace(detalle, "<.*?>", String.Empty);
                    clInfo = new PdfPCell(new Phrase(detalle, standardFont));
                    clInfo.BorderWidth = 0;
                    clInfo.BorderWidthBottom = 0.75f;
                    tblPrueba.AddCell(clInfo);
                }

                // Modulos
                var clModulos = new PdfPCell(new Phrase(Resources.Resources.Plans_Title_Modules, standardFont));
                clModulos.BorderWidth = 0;
                clModulos.BorderWidthBottom = 0.75f;
                tblPrueba.AddCell(clModulos);

                foreach (var plan in planes)
                {
                    var detalle = PlanesPrecioDetalle.GetModulosDescripcion(plan.Id);
                    detalle = Regex.Replace(detalle, "<.*?>", String.Empty);
                    clModulos = new PdfPCell(new Phrase(detalle, standardFont));
                    clModulos.BorderWidth = 0;
                    clModulos.BorderWidthBottom = 0.75f;
                    tblPrueba.AddCell(clModulos);
                }

                // Usuario
                var clUsuario = new PdfPCell(new Phrase(Resources.Resources.Plans_Title_Users, standardFont));
                clUsuario.BorderWidth = 0;
                clUsuario.BorderWidthBottom = 0.75f;
                tblPrueba.AddCell(clUsuario);

                foreach (var plan in planes)
                {
                    var detalle = PlanesPrecioDetalle.GetUsuariosDescripcion(plan.Id);
                    detalle = Regex.Replace(detalle, "<.*?>", String.Empty);
                    clUsuario = new PdfPCell(new Phrase(detalle, standardFont));
                    clUsuario.BorderWidth = 0;
                    clUsuario.BorderWidthBottom = 0.75f;
                    tblPrueba.AddCell(clUsuario);
                }

                // Ingresos y Descargas
                var clIngDes = new PdfPCell(new Phrase(Resources.Resources.Plans_Title_Downloads, standardFont));
                clIngDes.BorderWidth = 0;
                clIngDes.BorderWidthBottom = 0.75f;
                tblPrueba.AddCell(clUsuario);

                foreach (var plan in planes)
                {
                    var detalle = PlanesPrecioDetalle.GetIngresosDescargasDescripcion(plan.Id);
                    detalle = Regex.Replace(detalle, "<.*?>", String.Empty);
                    clIngDes = new PdfPCell(new Phrase(detalle, standardFont));
                    clIngDes.BorderWidth = 0;
                    clIngDes.BorderWidthBottom = 0.75f;
                    tblPrueba.AddCell(clIngDes);
                }

                // Favoritos y Grupos
                var clFavGru = new PdfPCell(new Phrase(Resources.Resources.Plans_Title_Favorites, standardFont));
                clFavGru.BorderWidth = 0;
                clFavGru.BorderWidthBottom = 0.75f;
                tblPrueba.AddCell(clFavGru);

                foreach (var plan in planes)
                {
                    var detalle = PlanesPrecioDetalle.GetFavoritosGruposDescripcion(plan.Id);
                    detalle = Regex.Replace(detalle, "<.*?>", String.Empty);
                    clFavGru = new PdfPCell(new Phrase(detalle, standardFont));
                    clFavGru.BorderWidth = 0;
                    clFavGru.BorderWidthBottom = 0.75f;
                    tblPrueba.AddCell(clFavGru);
                }

                // Plantillas Excel
                var clExcel = new PdfPCell(new Phrase(Resources.Resources.Plans_Title_Excel, standardFont));
                clExcel.BorderWidth = 0;
                clExcel.BorderWidthBottom = 0.75f;
                tblPrueba.AddCell(clExcel);

                foreach (var plan in planes)
                {
                    var detalle = PlanesPrecioDetalle.GetPlantillasDescripcion(plan.Id);
                    detalle = Regex.Replace(detalle, "<.*?>", String.Empty);
                    clExcel = new PdfPCell(new Phrase(detalle, standardFont));
                    clExcel.BorderWidth = 0;
                    clExcel.BorderWidthBottom = 0.75f;
                    tblPrueba.AddCell(clExcel);
                }

                pdfDoc.Add(tblPrueba);
                pdfWriter.CloseStream = false;
                pdfDoc.Close();

                return File(stream.ToArray(), "application/pdf", "Price_Plans.pdf");
            }
        }

        public decimal GetIgv(ref string codPaisIp)
        {
#if DEBUG
            var direccionIp = Properties.Settings.Default.IP_Debug;
#else
            var direccionIp = Request.ServerVariables["REMOTE_ADDR"];
#endif
            _ws.BuscaUbicacionIP2(direccionIp, ref codPaisIp);
            if (codPaisIp != null && codPaisIp == "PE")
            {
                return (decimal)0.18;
            }
            if (codPaisIp != null && codPaisIp == "CL")
            {
                return (decimal)0.19;
            }

            return 0;
        }

        public void ConsultaCodigo(ref string numeroPedido)
        {
            //Encriptacion de Parametros antes de enviarse
            var idAcquirer = _idadquirente;
            var idCommerce = _idcommerce;
            var operationNumber = _codpedido + numeroPedido;

            //Clave SHA-2 de la Pasarela
            var claveSecreta = _passpasarela;

            //Creacion del purchaseVerification
            var purchaseVerification = Sha2.getStringSHA(idAcquirer + idCommerce + operationNumber + claveSecreta);

            var url = _enviroment == "TEST"
                ? "https://integracion.alignetsac.com/VPOS2/rest/operationAcquirer/consulte"
                : "https://vpayment.verifika.com/VPOS2/rest/operationAcquirer/consulte";

            //-https://vpayment.verifika.com/VPOS2/rest/operationAcquirer/consulte

            //Creacion del Objeto JSON
            var data = "{\"idAcquirer\":\"" + idAcquirer + "\",\"idCommerce\":\"" + idCommerce +
                       "\",\"operationNumber\":\"" + operationNumber + "\",\"purchaseVerification\":\"" +
                       purchaseVerification + "\"}";

            string responseData;

            try
            {
                //Consumo del Servicio Rest de Consulta de Transacciones
                System.Net.WebRequest hwrequest = System.Net.WebRequest.Create(url);
                hwrequest.ContentType = "application/json";
                hwrequest.Method = "POST";

                System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
                byte[] postByteArray = encoding.GetBytes(data);
                hwrequest.ContentLength = postByteArray.Length;

                System.IO.Stream postStream = hwrequest.GetRequestStream();
                postStream.Write(postByteArray, 0, postByteArray.Length);
                postStream.Close();

                System.Net.WebResponse hwresponse = hwrequest.GetResponse();
                System.IO.StreamReader responseStream = new System.IO.StreamReader(hwresponse.GetResponseStream());
                responseData = responseStream.ReadToEnd();

                //Recepcion e interpretacion de los datos de respuesta
                var respuesta = JObject.Parse(responseData);
                var resultado = respuesta.GetValue("result").ToString();
                hwresponse.Close();
                if (!resultado.Equals("0"))
                {
                    numeroPedido = PedidosPayMe.GetOtherCode(numeroPedido);
                    ConsultaCodigo(ref numeroPedido);
                }
            }
            catch (Exception ex)
            {
                responseData = "An error occurred: " + ex.Message;
            }
        }

        public string CreaUsuarioPlan(string email1, string nombres, string apellidos, string empresa, string telefono,
            string idioma, string codCampaña, string pais, int plan, string plandescripcion, decimal planPrecio,
            string ruc, string direccion, string razonSocial)
        {
            var codUsuario = email1.ToLower();
            Thread thread, thread2;
            object parameters, parameters2;
            string idUsuario = string.Empty;

            if (!Functions.ExisteCodCampaña(codCampaña)) codCampaña = "0";
#if DEBUG
            bool existeUsuario = Usuario.ExisUsuario(codUsuario);
#else
            bool existeUsuario = _ws.ExisteCodUsuario(codUsuario);
#endif
            if (existeUsuario)
            {
                Usuario.UpdateState(codUsuario, planPrecio, codCampaña, nombres, apellidos, empresa);
                Usuario.UpdateFactura(codUsuario, ruc, direccion, razonSocial);
                parameters = new object[] { codUsuario, idioma, plan, plandescripcion };
                EnviaCorreoConfirmacion(parameters);

                parameters2 = new object[] { codUsuario, idioma, plan };
                EnviaCorreoCompraPlan(parameters2);

                return Usuario.GetPassword(codUsuario);
            }
#if DEBUG
            var direccionIp = Properties.Settings.Default.IP_Debug;
#else
            var direccionIp = Request.ServerVariables["REMOTE_ADDR"];
#endif
            var empPer = empresa != string.Empty ? "EMP" : "PER";

            var randomGenerator = RandomNumberGenerator.Create();
            byte[] data = new byte[6];
            randomGenerator.GetBytes(data);
            string password = BitConverter.ToString(data);

            password = password.Replace("-","");

            long passInt = Convert.ToInt64(password, 16);

            password = passInt.ToString().Substring(0, 6);

            idUsuario = Functions.CrearUsuarioPlan(empPer, codUsuario, password, nombres, apellidos, string.Empty, empresa, ruc,
                telefono, string.Empty, email1.ToLower(), "68", razonSocial, "67", direccionIp, codCampaña,
                string.Empty, string.Empty, direccion, planPrecio, plan, string.Empty, pais);

            parameters = new object[] {codUsuario, idioma, plan, plandescripcion};
            EnviaCorreoConfirmacion(parameters);

            EnviaCorreoCompraPlan(parameters);
            //thread2 = new Thread(EnviaCorreoCompraPlan);
            //parameters2 = new object[] {codUsuario, idioma, plan };
            //thread2.Start(parameters2);

            //JANAQ 160620 Creacion de usuario MixPanel
            FuncionesBusiness.CrearUsuarioMixPanel(idUsuario, Request.Url.Host);
            
            return password;
        }

        public void EnviaCorreoCompraPlan(object parametersCompra)
        {
            try
            {
                var parameterArray = parametersCompra as object[];
                if (parameterArray != null)
                {
                    var codUsuario = (string) parameterArray[0];
                    var idioma = (string) parameterArray[1];
                    CultureInfo ci = new CultureInfo(idioma);
                    Thread.CurrentThread.CurrentUICulture = ci;
                    Thread.CurrentThread.CurrentCulture = ci;
                    var planId = (int) parameterArray[2];

                    var dr = Funciones.ObtieneUsuarioCorreo(codUsuario);

                    var nombres = dr["Nombres"].ToString();
                    var email1 = dr["Email1"].ToString();

                    string emailEnvio = string.Empty, emailEnvioNombre = string.Empty, emailEnvioPassword = string.Empty;
                    Functions.BuscaDatosCorreoEnvio("4", ref emailEnvio, ref emailEnvioNombre, ref emailEnvioPassword);

                    string subject, url;
                    var fromName = emailEnvioNombre;
                    var fromEmail = emailEnvio;
                    var toName = nombres;
                    var toEmail = email1;

                    string bccName = "Info Veritrade";
                    string bccEmail = "info@veritrade-ltd.com";
                    string ccName = "Info Veritrade";
                    string ccEmail = "info@veritrade-ltd.com";

                    if (idioma == "es")
                    {
                        subject = "Bienvenido a Veritrade - www.veritradecorp.com";
                        //url = _path + "/Correos.aspx?Opcion=BIENVENIDO_COMPRA&CodUsuario=" +
                        //      codUsuario + "&PlanId=" + plan;
                    }
                    else
                    {
                        subject = "Welcome to Veritrade - www.veritradecorp.com";
                        //url = _path + "/Correos.aspx?Opcion=BIENVENIDO_COMPRA_EN&CodUsuario=" +
                        //      codUsuario + "&PlanId=" + plan;
                    }

                    var plan = Planes.GetDetallePlan(planId);
                    var planNombre = plan.Nombre;
                    PlanesPrecioDetalle detallePlan = PlanesPrecioDetalle.GetPlanDetalle(planId);
                

                    CorreoBienvenida model = new CorreoBienvenida();
                    model.nombre = nombres;
                    model.nombrePlan = planNombre;
                    model.user = codUsuario;
                    model.pass = dr["Password"].ToString();
                    model.vigenciaInicio = DateTime.ParseExact(dr["FecInicio"].ToString(), "yyyyMMdd", null).ToString("dd-MM-yyyy");
                    model.vigenciaFin = DateTime.ParseExact(dr["FecFin"].ToString(), "yyyyMMdd", null).ToString("dd-MM-yyyy");
                    model.paises = detallePlan.PaisesDescripcion;
                    model.modulos = detallePlan.ModulosDescripcion;
                    model.numUsuarios = detallePlan.UsuariosDescripcion;
                    model.favoritos = detallePlan.FavoritosGruposDescripcion;
                    model.plantillas = detallePlan.PlantillasDescripcion;

                    string bodyBienvenida = Funciones.RenderMailViewToString(this.ControllerContext, "Partials/Bienvenida_Compra_Email", model);

                    parametersCompra = new object[] { fromName, fromEmail, toName, toEmail, ccName, ccEmail, bccName, bccEmail, subject, bodyBienvenida,
                       emailEnvio, emailEnvioPassword };
                    Thread threadBienvenida = new Thread(EmailUtils.EnviaEmail);
                    threadBienvenida.Start(parametersCompra);
                }
            }
            catch(Exception ex)
            {
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
            }
        }

        [HttpGet]
        public void CorreoConfirmacionPrueba()
        {
            var codUsuario = "mauricio.castillo@zeke.cl";
            var idioma = "es";
            var plan = 1;
            var planNombre = "";
            object param = new object[] {
                codUsuario
                ,idioma
                ,plan
                ,planNombre
            };
            EnviaCorreoConfirmacion(param);
        }

        // Ruben 202209
        public void EnviaCorreoConfirmacion(object parameters)
        {
            try
            {
                var parameterArray = parameters as object[];
                if (parameterArray != null)
                {
                    var codUsuario = (string)parameterArray[0];
                    var idioma = (string)parameterArray[1];
                    CultureInfo ci = new CultureInfo(idioma);
                    Thread.CurrentThread.CurrentUICulture = ci;
                    Thread.CurrentThread.CurrentCulture = ci;
                    var planId = (int)parameterArray[2];

                    var plan = Planes.GetDetallePlan(planId);
                    var planNombre = plan.Nombre;
                    var dr = Funciones.ObtieneUsuario(codUsuario);

                    var nombres = dr["Nombres"].ToString();
                    var email1 = dr["Email1"].ToString();

                    string emailEnvio = string.Empty, emailEnvioNombre = string.Empty, emailEnvioPassword = string.Empty;
                    // Ruben 202209
                    Functions.BuscaDatosCorreoEnvio("6", ref emailEnvio, ref emailEnvioNombre, ref emailEnvioPassword);

                    string subject, url;
                    var fromName = emailEnvioNombre;
                    var fromEmail = emailEnvio;
                    var toName = nombres;
                    var toEmail = email1;

                    string bccName = "Info Veritrade";
                    string bccEmail = "info@veritrade-ltd.com";
                    string ccName = "Info Veritrade";
                    string ccEmail = "info@veritrade-ltd.com";

                    if (idioma == "es")
                    {
                        subject = "Confirmación de compra - Plan " + planNombre;
                        //url = _path + "/Correos.aspx?Opcion=INFO_COMPRA&CodUsuario=" +
                        //      codUsuario + "&PlanId=" + plan;
                    }
                    else
                    {
                        subject = "Purchase confirmation - " + planNombre + " Plan";
                        //url = _path + "/Correos.aspx?Opcion=INFO_COMPRA_EN&CodUsuario=" +
                        //      codUsuario + "&PlanId=" + plan;
                    }

                    CorreoConfirmacion model = new CorreoConfirmacion();
                    model.nombre = nombres;
                    model.nombrePlan = planNombre;

                    if (dr["Pais"].ToString() == "CHILE")
                    {
                        var planPrecio = Convert.ToDecimal(dr["ImporteUSD"]);

                        model.valorPlan = $"$ {Decimal.Round(planPrecio)}";
                    }
                    else
                    {
                        var planPrecio = PlanesPrecio.GetPrecioPlan(planId);
                        model.valorPlan = $"US$ {planPrecio.Precio}";
                    }

                    string bodyConfirmacion = Funciones.RenderMailViewToString(this.ControllerContext, "Partials/Confirmacion_Compra_Email", model);

                    parameters = new object[] { fromName, fromEmail, toName, toEmail, ccName, ccEmail, bccName, bccEmail, subject, bodyConfirmacion,
                    emailEnvio, emailEnvioPassword };
                    Thread threadConfirmacion = new Thread(EmailUtils.EnviaEmail);
                    threadConfirmacion.Start(parameters);

                    //EmailUtils.EnviaEmail(fromName, fromEmail, toName, toEmail, null, null, bccName, bccEmail, subject, body,
                    //emailEnvio, emailEnvioPassword);
                }
            }
            catch(Exception ex)
            {
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
            }
            
        }

        // Ruben 20303
        void CreaUsuarioFormPlan(string Nombres, string Apellidos, string Empresa, string Correo, string Telefono, string CodPais)
        {
            string sql;

            sql = "insert into UsuarioFormPlan(Nombres, Apellidos, Empresa, Correo, Telefono, CodPais, FechaCreacion) ";
            sql += "values ('" + Nombres + "', '" + Apellidos + "', '" + Empresa + "', '" + Correo + "','" + Telefono + "', '" + CodPais + "', getdate())";

            Conexion.SqlExecute(sql);

            string CodUsuario = Correo.ToLower();
            string EmpPer = Empresa != "" ? "EMP" : "PER";
            string DireccionIP = Request.ServerVariables["REMOTE_ADDR"];
            string CodCampaña = "01200000000009";

            sql = "select count(*) as Cant from Usuario where CodUsuario = '" + CodUsuario + "'";

            DataTable dt = Conexion.SqlDataTable(sql);

            bool ExisteUsuario = Convert.ToInt32(dt.Rows[0]["Cant"]) > 0;

            if (!ExisteUsuario)
            {                                                
                var randomGenerator = RandomNumberGenerator.Create();
                byte[] data = new byte[6];
                randomGenerator.GetBytes(data);
                string Password = BitConverter.ToString(data);
                Password = Password.Replace("-", "");
                long PassInt = Convert.ToInt64(Password, 16);
                Password = PassInt.ToString().Substring(0, 6);

                Functions.CrearUsuarioFreeTrial(EmpPer, CodUsuario, Password, Nombres, Apellidos, "", Empresa, "", Telefono, "", Correo.ToLower(), "68", "", "67", DireccionIP, CodCampaña, "", "", "", CodPais);
            }
            else
            {
                sql = "select Password from Usuario where CodUsuario = '" + CodUsuario + "'";

                DataTable dt1 = Conexion.SqlDataTable(sql);

                string Password = dt1.Rows[0]["Password"].ToString();

                Functions.ActualizarUsuarioFreeTrial(EmpPer, CodUsuario, Password, Nombres, Apellidos, "", Empresa, "", Telefono, "", Correo.ToLower(), "68", "", "67", DireccionIP, CodCampaña, "", "", "", CodPais);
            }

        }

        //[HttpPost]
        //public ActionResult SetCulture(string culture)
        //{
        //    culture = CultureHelper.GetImplementedCulture(culture);
        //    RouteData.Values["culture"] = culture; // set culture 

        //    //var codCampania = "";

        //    //if (culture == "es")
        //    //{
        //    //    //Session["c"] = "20100";
        //    //    Session["c"] = Session["c"].ToString().Replace("I", "");
        //    //    codCampania = Session["c"].ToString();
        //    //    return RedirectToRoute("Planes", new {culture, campania = codCampania});
        //    //}
        //    //else
        //    //{
        //    //    //Session["c"] = "20100I";
        //    //    Session["c"] = Session["c"].ToString().Replace("I", "")+"I";
        //    //    codCampania = Session["c"].ToString();
        //    //    return RedirectToRoute("PlanesUS", new { culture, campania = codCampania});
        //    //}
        //    ////return RedirectToAction("Index");
        //    return RedirectToAction("Index");
        //}
    }
}