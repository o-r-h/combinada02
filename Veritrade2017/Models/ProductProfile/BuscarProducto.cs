using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Veritrade2017.Models.Admin;
using Veritrade2017.Models.Minisite;
using Microsoft.Ajax.Utilities;

namespace Veritrade2017.Models.ProductProfile
{
    public class BuscarProducto
    {
        public string Producto { get; set; }
        public string Pais { get; set; }
        public string Uri { get; set; }
        public string CodPartida { get; set; }

        private string json_path = HttpContext.Current.Server.MapPath("~/Models/ProductProfile/Json/last_searchs.json");

        private string json_path_en =
            HttpContext.Current.Server.MapPath("~/Models/ProductProfile/Json/last_searches_en.json");

        public void SaveLastSearches(string uri, string codPartida, int IdPais, string culture)
        {
            try
            {
                var found = MisProductos.GetProducto(uri, codPartida, IdPais, culture);
                if (found != null)
                {
                    string newJson;
                    string path_file = json_path;
                    using (StreamReader r = new StreamReader(path_file))
                    {
                        var list = new List<BuscarProducto>();
                        string json = r.ReadToEnd();
                        if (json.Trim().Length > 0)
                        {
                            list = JsonConvert.DeserializeObject<List<BuscarProducto>>(json);
                        }

                        list.Add(found);
                        newJson = JsonConvert.SerializeObject(list);
                    }

                    File.WriteAllText(path_file, newJson);
                }
            }
            catch
            {
                ;
            }

        }

        public void SaveLastSearches_English(string uri, string codPartida, int IdPais, string culture)
        {
            try
            {
                var found = MisProductos.GetProducto(uri, codPartida, IdPais, culture);
                if (found != null)
                {
                    string newJson;
                    string path_file = json_path_en;
                    using (StreamReader r = new StreamReader(path_file))
                    {
                        var list = new List<BuscarProducto>();
                        string json = r.ReadToEnd();
                        if (json.Trim().Length > 0)
                        {
                            list = JsonConvert.DeserializeObject<List<BuscarProducto>>(json);
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

        public List<BuscarProducto> GetLastSearches(int limit = 10)
        {
            var lst = new List<BuscarProducto>();
            string path_file = json_path;
            using (StreamReader r = new StreamReader(path_file))
            {
                try
                {
                    string json = r.ReadToEnd();
                    if (json.Trim().Length > 0)
                    {
                        lst = JsonConvert.DeserializeObject<List<BuscarProducto>>(json);
                    }
                }
                catch (Exception ex)
                {
                    string msg = ex.Message;
                    lst = new List<BuscarProducto>();
                }
            }

            if (lst.Count > 0)
            {
                //lst = lst.GetRange(0, lst.Count - cont);
                lst.Reverse();
                lst = lst.DistinctBy(x => x.CodPartida).ToList();

                lst = lst.Take(limit).ToList();
            }

            return lst;
        }

        public List<BuscarProducto> GetLastSearchesEnglish(int limit = 10)
        {
            var lst = new List<BuscarProducto>();
            string path_file = json_path_en;
            using (StreamReader r = new StreamReader(path_file))
            {
                try
                {
                    string json = r.ReadToEnd();
                    if (json.Trim().Length > 0)
                    {
                        lst = JsonConvert.DeserializeObject<List<BuscarProducto>>(json);
                    }
                }
                catch
                {
                    lst = new List<BuscarProducto>();
                }
            }

            if (lst.Count > 0)
            {
                //lst = lst.GetRange(0, lst.Count - cont);
                lst.Reverse();
                lst = lst.DistinctBy(x => x.CodPartida).ToList();

                lst = lst.Take(limit).ToList();
            }

            return lst;
        }

        public int CountJson()
        {
            var lst = new List<BuscarProducto>();
            string path_file = json_path;
            using (StreamReader r = new StreamReader(path_file))
            {
                try
                {
                    string json = r.ReadToEnd();
                    if (json.Trim().Length > 0)
                    {
                        lst = JsonConvert.DeserializeObject<List<BuscarProducto>>(json);
                    }
                }
                catch
                {
                    lst = new List<BuscarProducto>();
                }
            }

            return lst.Count;
        }
    }
}