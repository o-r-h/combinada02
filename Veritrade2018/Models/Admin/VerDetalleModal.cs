using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Web;

namespace Veritrade2018.Models.Admin
{
    public class VerDetalleModal
    {
        public bool visible { get; set; }
        public string label { get; set; }
        public string value { get; set; }

        public VerDetalleModal(string label, string value, bool visible= true)
        {
            this.label = label;
            this.value = value;
            this.visible = visible;
        }
    }
}