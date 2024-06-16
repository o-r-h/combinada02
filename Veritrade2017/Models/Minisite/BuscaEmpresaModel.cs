using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Microsoft.Ajax.Utilities;

namespace Veritrade2017.Models.Minisite
{
    public class BuscaEmpresaModel
    {
        public string Empresa { get; set; }
        public  string Trib { get; set; }
        public string Ruc { get; set; }
        public string Pais { get; set; }
        public string Uri { get; set; }

        private string json_path = HttpContext.Current.Server.MapPath("~/Models/Minisite/Json/last_searchs.json");
        public void SaveLastSearches(string slug, string ruc)
        {
            try
            {
                var found = Minisite.Empresa.GetEmpresaFound(slug, ruc);
                if (found != null)
                {
                    string newJson;
                    string path_file = json_path;
                    using (StreamReader r = new StreamReader(path_file))
                    {
                        var list = new List<BuscaEmpresaModel>();
                        string json = r.ReadToEnd();
                        if (json.Trim().Length > 0)
                        {
                            list= JsonConvert.DeserializeObject<List<BuscaEmpresaModel>>(json);
                        }
                        list.Add(found);
                        newJson = JsonConvert.SerializeObject(list);
                    }
                    File.WriteAllText(path_file, newJson);
                }
            }
            catch(Exception ex)
            {
                string msg = ex.Message;
            }

        }

        public List<BuscaEmpresaModel> GetLastSearches(int limit = 10)
        {
            var lst = new List<BuscaEmpresaModel>();
            string path_file = json_path;
            using (StreamReader r = new StreamReader(path_file))
            {
                try
                {
                    string json = r.ReadToEnd();
                    if (json.Trim().Length > 0)
                    {
                        lst = JsonConvert.DeserializeObject<List<BuscaEmpresaModel>>(json);
                    }
                }
                catch (Exception ex)
                {
                    string msg = ex.Message;
                    lst = new List<BuscaEmpresaModel>();
                }
            }

            if (lst.Count > 0)
            {
                //lst = lst.GetRange(0, lst.Count - cont);
                lst.Reverse();
                lst = lst.DistinctBy(x => x.Ruc ).ToList();
                
                lst = lst.Take(limit).ToList();
            }
            return lst;
        }
        public int CountJson()
        {
            var lst = new List<BuscaEmpresaModel>();
            string path_file = json_path;
            using (StreamReader r = new StreamReader(path_file))
            {
                try
                {
                    string json = r.ReadToEnd();
                    if (json.Trim().Length > 0)
                    {
                        lst = JsonConvert.DeserializeObject<List<BuscaEmpresaModel>>(json);
                    }
                }
                catch
                {
                    lst = new List<BuscaEmpresaModel>();
                }
            }

            return lst.Count;
        }
    }
}