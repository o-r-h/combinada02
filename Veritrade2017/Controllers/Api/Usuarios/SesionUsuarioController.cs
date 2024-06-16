//using DevExpress.Utils.OAuth;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Web.Http;
using Veritrade2017.Helpers;
using Veritrade2017.Models;
using Veritrade2017.VeritradeAdmin;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Configuration;
using System.Reflection;
using System.Threading;
using System.Web;
using System.Text.RegularExpressions;
using System.Web.Hosting;

namespace Veritrade2017.Controllers.Api.Usuarios
{
    [RoutePrefix("api/SesionUsuario")]
    public class SesionUsuarioController : ApiController
    {
        [HttpGet]
        [Route("ObtenerSesionesActivas")]
        public List<int> ObtenerSesionesActivas()
        {
            List<int> idsUsuario = new List<int>();
            idsUsuario = SesionUsuario.ObtenerSesionesActivas();
            return idsUsuario;
        }

        public int Get(int id)
        {
            int sesionUsuario = SesionUsuario.ObtenerSesionActivaPorUsuario(id);
            return sesionUsuario;
        }

        public void Post(int idUsuario, bool sesionActiva)
        {
            SesionUsuario.InsertarSesionUsuario(idUsuario, sesionActiva);
        }

        public void Put(int idUsuario, bool sesionActiva)
        {
            SesionUsuario.ActualizarSesionUsuario(idUsuario, sesionActiva);
        }

        [HttpPut]
        [Route("ActualizarTokenUsuario")]
        public bool ActualizarTokenUsuario()
        {
            List<string> listaUsuarios = new List<string>();
            listaUsuarios = Functions.ObtenerUsuariosToken();

            try
            {
                Dictionary<string, string> usuarios = new Dictionary<string, string>();

                foreach (var codUsuario in listaUsuarios)
                {
                    string token = GenerarToken(codUsuario);
                    usuarios.Add(codUsuario, token);
                }

                bool resultado = Functions.ActualizarTokensUsuario(usuarios);
                return true;
            }
            catch (Exception ex)
            {
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                return false;
            }

        }

        public string GenerarToken(string codUsuario)
        {
            try { 
                var secretKey = Properties.Settings.Default.Jwt_Secret_Key;
                var audienceToken = Properties.Settings.Default.UrlWeb;
                var issuerToken = Properties.Settings.Default.UrlWeb;
                var expireTime = Properties.Settings.Default.Jwt_Expire_Days;

                var securityKey = new SymmetricSecurityKey(Encoding.Default.GetBytes(secretKey));
                var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

                ClaimsIdentity claimsIdentity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, codUsuario) });

                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtSecurityToken = tokenHandler.CreateJwtSecurityToken(
                    audience: audienceToken,
                    issuer: issuerToken,
                    subject: claimsIdentity,
                    notBefore: DateTime.Now,
                    expires: DateTime.Now.AddDays(Convert.ToInt32(expireTime)),
                    signingCredentials: signingCredentials);

                var jwtTokenString = tokenHandler.WriteToken(jwtSecurityToken);

                return jwtTokenString;
                //string urlToken = Properties.Settings.Default.UrlWeb + "/" + culture +"/setLogin?token=" + jwtTokenString;

                //return urlToken;
            }
            catch (Exception ex)
            {
                ExceptionUtility.LogException(ex, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                return null;
            }
        }

        [HttpGet]
        [Route("ValidarToken")]
        public IHttpActionResult ValidarToken(string token)
        {
            var secretKey = Properties.Settings.Default.Jwt_Secret_Key;
            var audienceToken = Properties.Settings.Default.UrlWeb;
            var issuerToken = Properties.Settings.Default.UrlWeb;
            //var expireTime = Properties.Settings.Default.Jwt_Expire_Days;
            var securityKey = new SymmetricSecurityKey(Encoding.Default.GetBytes(secretKey));

            SecurityToken securityToken;
            var tokenHandler = new JwtSecurityTokenHandler();
            TokenValidationParameters validationParameters = new TokenValidationParameters()
            {
                ValidAudience = audienceToken,
                ValidIssuer = issuerToken,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                //Comprobando caducidad del token
                LifetimeValidator = this.LifetimeValidator,
                IssuerSigningKey = securityKey
            };

            try
            {
                var resultValid = tokenHandler.ValidateToken(token, validationParameters, out securityToken);
                var claims = resultValid.Claims.ToList();
                string codUsuario = claims[0].Value;

                return Ok(codUsuario);
            }
            catch (SecurityTokenException)
            {
                return NotFound();
            }
        }
       
        public bool LifetimeValidator(DateTime? notBefore,
                                  DateTime? expires,
                                  SecurityToken securityToken,
                                  TokenValidationParameters validationParameters)
        {
            var valid = false;
            expires = ((DateTime)expires).ToLocalTime();
            notBefore = ((DateTime)notBefore).ToLocalTime();

            string logFile = "~/App_Data/ErrorLog-" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
            logFile = HostingEnvironment.MapPath(logFile);
            StreamWriter sw = new StreamWriter(logFile, true);

            sw.WriteLine("Fecha actual: {0}", DateTime.Now);
            sw.WriteLine("Fecha anterior: {0}", notBefore);
            sw.WriteLine("Fecha expiración: {0}", expires);
            sw.Close();

            if ((expires.HasValue && DateTime.Now < expires)
                && (notBefore.HasValue && DateTime.Now > notBefore))
            { valid = true; }

            return valid;
        }
    }
}
