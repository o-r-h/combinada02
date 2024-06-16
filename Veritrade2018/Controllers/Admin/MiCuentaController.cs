using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using Dapper;
using Veritrade2018.App_Start;
using Veritrade2018.Helpers;

namespace Veritrade2018.Controllers.Admin
{
    public class MiCuentaController : BaseController
    {

        #region PrivateMethods
        private async Task<int> UsuariosAsync()
        {
            using (var db = new ConexProvider().Open)
            {
                //var sql = "select Usuarios from Usuario where IdUsuario = @IdUsuario";
                var sql = @"select C.Usuarios from Usuario UL, VeritradeAdmin.dbo.Cliente C  
                            where UL.IdUsuarioCliente = C.IdUsuarioLegacy and UL.IdUsuario = @IdUsuario";
                var ret = await db.QueryFirstOrDefaultAsync<int>(sql, new { IdUsuario = Session["IdUsuario"].ToString() });
                return ret;
            }
        }

        private int Usuarios()
        {
            using (var db = new ConexProvider().Open)
            {
                //var sql = "select Usuarios from Usuario where IdUsuario = @IdUsuario";
                var sql = @"select C.Usuarios from Usuario UL, VeritradeAdmin.dbo.Cliente C  
                            where UL.IdUsuarioCliente = C.IdUsuarioLegacy and UL.IdUsuario = @IdUsuario";
                var ret = db.QueryFirstOrDefault<int>(sql, new { IdUsuario = Session["IdUsuario"].ToString() });
                return ret;
            }
        }

        private async Task<int> UsuariosActivosAsync()
        {
            using (var db = new ConexProvider().Open)
            {
                //var sql = "select count(*) as Cant from Usuario where IdUsuarioCliente = @IdUsuario and CodEstado = 'A' ";
                var sql = "select count(*) as Cant from Usuario U, Usuario U2 " +
                    "where U.IdUsuarioCliente = U2.IdUsuarioCliente and U2.IdUsuario = @IdUsuario and U.CodEstado = 'A' and U.IdTipo = 10";
                var ret = await db.QueryFirstOrDefaultAsync<int>(sql, new { IdUsuario = Session["IdUsuario"].ToString() });
                return ret;
            }
        }

        private int UsuariosActivos()
        {
            using (var db = new ConexProvider().Open)
            {
                //var sql = "select count(*) as Cant from Usuario where IdUsuarioCliente = @IdUsuario and CodEstado = 'A' ";
                var sql = "select count(*) as Cant from Usuario U, Usuario U2 " +
                    "where U.IdUsuarioCliente = U2.IdUsuarioCliente and U2.IdUsuario = @IdUsuario and U.CodEstado = 'A' and U.IdTipo = 10";
                var ret = db.QueryFirstOrDefault<int>(sql, new { IdUsuario = Session["IdUsuario"].ToString() });
                return ret;
            }
        }

        private async Task<UsuarioView> GiveMe(int id)
        {
            using (var db = new ConexProvider().Open)
            {
                /*var sql = "select *," +
                          "Id = IdUsuario, " +
                          "TipoUsuario = case FlagUsuarioPrincipal when 'S' then '"+Resources.AdminResources.LabelMain_Text+ "' else '" + Resources.AdminResources.LabelSecondary_Text + "' end, " +
                          "FechaCreacion =  Convert(varchar(10), FecRegistro, 103 ), " +
                          "Estado = case CodEstado when 'A' then '" + Resources.AdminResources.LabelActive_Text + "' else '" + Resources.AdminResources.LabelInactive_Text + "' end, " +
                          "EnableCodUsuario = cast( case FlagUsuarioPrincipal when 'S' then 0 else 1 end as bit),    " +
                          "EnableEstado = cast( case FlagUsuarioPrincipal when 'S' then 0 else 1 end as bit),    " +
                          "CheckedEstado = cast( case CodEstado when 'A' then 1 else 0 end as bit)    " +
                          "from Usuario where IdUsuarioCliente = @id And IdUsuario=@idusu ";  */

                var sql = "select U.*," +
                         "Id = U.IdUsuario, " +
                         "TipoUsuario = case U.FlagUsuarioPrincipal when 'S' then '" + Resources.AdminResources.LabelMain_Text + "' else '" + Resources.AdminResources.LabelSecondary_Text + "' end, " +
                         "FechaCreacion =  Convert(varchar(10), U.FecRegistro, 103 ), " +
                         "Estado = case U.CodEstado when 'A' then '" + Resources.AdminResources.LabelActive_Text + "' else '" + Resources.AdminResources.LabelInactive_Text + "' end, " +
                         "EnableCodUsuario = cast( case U.FlagUsuarioPrincipal when 'S' then 0 else 1 end as bit),    " +
                         "EnableEstado = cast( case U.FlagUsuarioPrincipal when 'S' then 0 else 1 end as bit),    " +
                         "CheckedEstado = cast( case U.CodEstado when 'A' then 1 else 0 end as bit)    " +
                         "from Usuario U, Usuario U2 where U.IdUsuarioCliente = U2.IdUsuarioCliente AND  U.IdUsuario=@idusu and U.IdTipo = 10 ";

                var result = await db.QueryFirstAsync<UsuarioView>(sql, new { id = Session["IdUsuario"].ToString(), idusu = id });
                return result;
            }
        }

        private bool ExisteCodUsuario(string CodUsuario, int IdUsuario = 0)
        {
            string sql;
            sql = "select count(*) as Cant from Usuario where CodUsuario = '" + CodUsuario + "' ";
            if (IdUsuario >0)
                sql += "and IdUsuario <> " + IdUsuario;

            using (var db = new ConexProvider().Open)
            {
                var result = db.QueryFirstOrDefault<int>(sql);
                return result > 0;
            }
        }
        #endregion


        // GET: MiCuenta
        public async Task<ActionResult> Index(string culture)
        {
            if (Session["IdUsuario"] == null)
                return RedirectToAction("Logout", "Common");
            if (!Funciones.EsUsuarioPrincipal(Session["IdUsuario"].ToString()))
                return RedirectToAction("Index", "MisBusquedas");

            ViewBag.nombreUsuario = Funciones.BuscaUsuario(Session["IdUsuario"].ToString());
            ViewBag.tipoUsuario = Session["TipoUsuario"].ToString();
            ViewBag.plan = Session["Plan"].ToString();

            ViewData["TitlePerfilBar"] = Resources.Resources.MyAccount_Title;
            ViewData["IngresoComoFreeTrial"] = Session["IngresoComoFreeTrial"] != null && (bool)Session["IngresoComoFreeTrial"];
            Session["Idioma"] = culture;
            return View ( (await GetInfo()).ToExpando() );
        }

        public async Task<object> GetInfo()
        {
            using (var db = new ConexProvider().Open)
            {
                string id = Session["IdUsuario"].ToString();
                //var sql = "select Usuarios from Usuario where IdUsuario = @id";
                var sql1 = @"select C.Usuarios from Usuario UL, VeritradeAdmin.dbo.Cliente C  
                            where UL.IdUsuarioCliente = C.IdUsuarioLegacy and UL.IdUsuario = @id";
                //var nUsuarios = await db.QueryFirstAsync<int>("select Usuarios from Usuario where IdUsuario = @id ", new {id = id});
                var nUsuarios = await db.QueryFirstAsync<int>(sql1, new { id = id });
                /*var sql = 
                    "select U.*, 'LATAM FULL' as [Plan], substring(convert(char(8), FecInicio), 7, 2) + '/' + substring(convert(char(8), FecInicio), 5, 2) + '/' + substring(convert(char(8), FecInicio), 1,4) as FecInicio2, ";
                sql += 
                    "substring(convert(char(8), FecFin), 7, 2) + '/' + substring(convert(char(8), FecFin), 5, 2) + '/' + substring(convert(char(8), FecFin), 1, 4) as FecFin2 from Usuario u where IdUsuario = @id";*/
                var sql = @"select C.Cliente as Empresa, C.RUC, C.[Plan], C.Usuarios, C.Precio as ImporteUSD, 
                            convert(varchar(10), FechaInicio, 103) as FecInicio2, 
                            convert(varchar(10), FechaFin, 103) as FecFin2 
                            from Usuario UL, VeritradeAdmin.dbo.V_Cliente C where UL.IdUsuarioCliente = C.IdUsuarioLegacy and UL.IdUsuario = @id";
                var oPlan = await db.QueryFirstAsync<dynamic>(sql, new {id = id});
                return new {nUsuarios, oPlan};
            }
        }

        [SessionExpireFilter]
        [HttpPost]
        public async Task<JsonResult> GetData()
        {
            using (var db = new ConexProvider().Open)
            {
                var iUsers = await UsuariosAsync();
                var canAdd = await UsuariosActivosAsync() < iUsers;
                /*var sql = "select *," +
                          "Id = IdUsuario, " +
                          "Nro = Row_Number() Over (order by IdUsuario Asc), " +
                          "TipoUsuario = case FlagUsuarioPrincipal when 'S' then '"+Resources.AdminResources.LabelMain_Text+"' else '" + Resources.AdminResources.LabelSecondary_Text + "' end, " +
                          "FechaCreacion =  Convert(varchar(10), FecRegistro, 103 ), " +
                          "Estado = case CodEstado when 'A' then '" + Resources.AdminResources.LabelActive_Text + "' else '"+ Resources.AdminResources.LabelInactive_Text + "' end " +
                          "from Usuario where IdUsuarioCliente = @id";*/

                var sql = "select U.*," +
                          "Id = U.IdUsuario, " +
                          "Nro = Row_Number() Over (order by isnull(U.FlagUsuarioPrincipal, 'N') desc, U.CodEstado, U.CodUsuario), " +
                          "TipoUsuario = case U.FlagUsuarioPrincipal when 'S' then '" + Resources.AdminResources.LabelMain_Text + "' else '" + Resources.AdminResources.LabelSecondary_Text + "' end, " +
                          "FechaCreacion =  Convert(varchar(10), U.FecRegistro, 103 ), " +
                          "Estado = case U.CodEstado when 'A' then '" + Resources.AdminResources.LabelActive_Text + "' else '" + Resources.AdminResources.LabelInactive_Text + "' end " +
                          "from Usuario U, Usuario U2 " +
                          "where U.IdUsuarioCliente = U2.IdUsuarioCliente and U2.IdUsuario = @id and U.IdTipo = 10 " +
                          "order by isnull(U.FlagUsuarioPrincipal, 'N') desc, U.CodEstado, U.CodUsuario";

                var result = await db.QueryAsync<UsuarioView> (sql, new { id = Session["IdUsuario"].ToString() });
                return Json ( new {lst= result.ToList(), visibleAdd = canAdd } );
            }
        }

        [SessionExpireFilter]
        [HttpPost]
        public async Task<JsonResult> EditMe(int id)
        {
            using (var db = new ConexProvider().Open)
            {   
                return Json(await this.GiveMe(id));
            }
        }

        [SessionExpireFilter]
        [HttpPost]
        public async Task<JsonResult> AddMe()
        {
            using (var db = new ConexProvider().Open)
            {
                //object _ret = dyna{message = string.Empty  };

                var iUsers = await UsuariosAsync();
                var canAdd = await UsuariosActivosAsync() < iUsers;
                if (!canAdd)
                {
                    //_ret.message = $"{Resources.Resources.MyAccount_Message01} : '{iUsers}' ";
                    //_ret.titleMessage = Resources.Resources.MyAccount_Title;
                    return Json(new {message = $"{Resources.Resources.MyAccount_Message01} : '{iUsers}' ", titleMessage = Resources.Resources.MyAccount_Title});
                }
                else
                {
                    return Json(new  UsuarioView { Estado = "Activo", CheckedEstado  = true, EnableEstado  = false, EnableCodUsuario  = true, CodEstado="A" });
                }
            }
        }

        [SessionExpireFilter]
        [HttpPost]
        public /*async Task<JsonResult>*/ JsonResult  SaveMe(UsuarioView model)
        {
            var IdUsuario = Session["IdUsuario"].ToString();
            var id = int.Parse(model.Id??"0");

            if (ExisteCodUsuario(model.CodUsuario, id))
            {
                return Json(new { message = Resources.Resources.MyAccount_Message02, titleMessage = Resources.Resources.MyAccount_Title });
            }

            var iUsers = Usuarios();
            var canAdd = UsuariosActivos() < iUsers;
            if (id == 0)
            {   
                if (!canAdd)
                {
                    return Json(new { message = $"{Resources.Resources.MyAccount_Message01} : '{iUsers}' ", titleMessage = Resources.Resources.MyAccount_Title });
                }

                try
                {
                    using (var tx = new TransactionScope())
                    {
                        string sql;
                        int IdUsuarioLegacy = 0;                        

                        using (var db = new ConexProvider().Open)
                        {

                            sql = "SELECT IdUsuarioCliente from Usuario where idUsuario = @IdUsuarioPrincipal";
                            var _row1 = db.QueryFirstOrDefault(sql, new { IdUsuarioPrincipal = IdUsuario });

                            var IdUsuarioAux = 0;

                            if (_row1 != null)
                            {
                                IdUsuarioAux = _row1.IdUsuarioCliente;
                            }

                            db.Execute("[dbo].[CreaUsuarioSecundario]",
                                new
                                {
                                    UsuarioCorreo = model.CodUsuario,
                                    Contraseña = model.Password,
                                    model.Nombres,
                                    model.Apellidos,
                                    model.Telefono,
                                    IdUsuarioPrincipal = IdUsuarioAux,//IdUsuario
                                    CodEstado = "A"
                                },
                                commandType: CommandType.StoredProcedure);


                            sql = "select max(IdUsuario) as IdUsuario from Usuario";
                            IdUsuarioLegacy = db.QueryFirstOrDefault<int>(sql);

                            //}

                            //using (var dba = new ConexProvider(Variables.ConfigManager.ADMIN).Open)
                            {
                                sql =
                                    "select IdCliente, CodPais from VeritradeAdmin.dbo.Cliente where IdUsuarioLegacy = @IdUsuarioPrincipal";
                                var _row = db.QueryFirstOrDefault(sql, new {IdUsuarioPrincipal = IdUsuario});


                                int IdCliente = 0;
                                string CodPais = string.Empty;
                                if (_row != null)
                                {
                                    IdCliente = _row.IdCliente;
                                    CodPais = _row.CodPais;
                                }


                                db.Execute("VeritradeAdmin.[dbo].DTHP_INSERT_USUARIO",
                                    new
                                    {
                                        IdCliente,
                                        UsuarioCorreo = model.CodUsuario,
                                        Contraseña = model.Password,
                                        model.Nombres,
                                        model.Apellidos,
                                        model.Telefono,
                                        Correo = model.CodUsuario,
                                        CodTipoUsuario = "SEC",
                                        FlagSesionUnica = "S",
                                        FlagValidarIP = "S",
                                        CodPaisIPOrig = CodPais,
                                        CodPaisIP = CodPais,
                                        FlagIP = "D",
                                        CodEstado = "ACT",
                                        IdUsuarioLegacy
                                    },
                                    commandType: CommandType.StoredProcedure);

                            }
                            id = IdUsuarioLegacy;
                            tx.Complete();
                        }
                    }

                    //JANAQ 160620 Creacion de usuario MixPanel
                    FuncionesBusiness.CrearUsuarioMixPanel(id.ToString(), Request.Url.Host);
                }
                catch (Exception ex)
                {
                    return Json(new
                    {
                        message = Resources.Resources.Request_Error_Message,
                        titleMessage = Resources.Resources.MyAccount_Title,
                        log = ex.Message
                    });
                }
            }
            else
            {   
                if (!canAdd && model.CodEstado == "I" && model.Estado == "A")
                {
                    return Json(new { message = Resources.Resources.MyAccount_Message03, titleMessage = Resources.Resources.MyAccount_Title });
                }

                try
                {
                    int IdUsuario2;
                    string CodTipoUsuario;
                    string CodPais;

                    using (var tx = new TransactionScope())
                    {
                        using (var db = new ConexProvider().Open)
                        {

                            db.Execute("[dbo].[ActualizaUsuarioPrimarioSecundario_Web]",
                                new
                                {
                                    IdUsuario = id,
                                    UsuarioCorreo = model.CodUsuario,
                                    Contraseña = model.Password,
                                    model.Nombres,
                                    model.Apellidos,
                                    model.Telefono,
                                    CodEstado = model.Estado
                                },
                                commandType: CommandType.StoredProcedure);

                            string sql =
                                "select UL.IdUsuario, isnull(U.FlagUsuarioPrincipal, 'N') as FlagUsuarioPrincipal, U.CodPais ";
                            sql += "from Usuario U, [VeritradeAdmin].dbo.Usuario UL ";
                            sql += "where U.IdUsuario = UL.IdUsuarioLegacy and U.IdUsuario =@IdUsuario";
                            var _row = db.QueryFirstOrDefault<dynamic>(sql, new {IdUsuario = id});

                            if(_row != null)
                            {
                                IdUsuario2 = _row.IdUsuario;
                                CodTipoUsuario = "PRI";
                                if (_row.FlagUsuarioPrincipal == "N") CodTipoUsuario = "SEC";
                                CodPais = _row.CodPais;


                                db.Execute("VeritradeAdmin.[dbo].DTHP_UPDATE_USUARIO",
                                        new
                                        {
                                            IdUsuario = IdUsuario2,
                                            UsuarioCorreo = model.CodUsuario,
                                            Contraseña = model.Password,
                                            model.Nombres,
                                            model.Apellidos,
                                            model.Telefono,
                                            Correo = model.CodUsuario,
                                            CodTipoUsuario = CodTipoUsuario,
                                            FlagSesionUnica = "S",
                                            FlagValidarIP = "S",
                                            CodPaisIPOrig = CodPais,
                                            CodPaisIP = CodPais,
                                            FlagIP = "D",
                                            CodEstado = model.Estado == "I" ? "INT" : "ACT"
                                        },
                                        commandType: CommandType.StoredProcedure);
                            }

                            

                            tx.Complete();
                        }
                    }

                    //JANAQ 160620 Creacion de usuario MixPanel
                    FuncionesBusiness.CrearUsuarioMixPanel(id.ToString(), Request.Url.Host);
                }
                catch (Exception ex)
                {
                    return Json(new
                    {
                        message = Resources.Resources.Request_Error_Message,
                        titleMessage = Resources.Resources.MyAccount_Title,
                        log = ex.Message
                    });
                }
            }
            return Json(new { id });
        }
        [SessionExpireFilter]
        [HttpPost]
        public ActionResult SetCulture(string culture)
        {
            culture = CultureHelper.GetImplementedCulture(culture);
            RouteData.Values["culture"] = culture; // set culture 

            return RedirectToAction("Index");
        }
    }

    public class UsuarioView
    {
        public string Id { get; set; }
        public int? Nro { get; set; } = null;
        public string  CodUsuario { get; set; }
        public string Password { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string Telefono { get; set; }
        public string TipoUsuario { get; set; }
        public string FechaCreacion { get; set; } = string.Empty;
        public string Estado { get; set; }
        public string CodEstado { get; set; }
        public string LabelEdit { get; set; } = Resources.Resources.Ctrl_Modify;
        public string LabelSave { get; set; } = Resources.Resources.Ctrl_Save;
        public string LabelCancel { get; set; } = Resources.Resources.Ctrl_Undo;
        public bool EnableCodUsuario { get; set; }
        public bool EnableEstado { get; set; }
        public bool CheckedEstado { get; set; }
    }
}