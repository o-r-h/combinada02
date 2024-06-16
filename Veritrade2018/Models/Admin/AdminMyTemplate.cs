using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Veritrade2018.Models.Admin
{
    public class AdminMyTemplate
    {
        public IEnumerable<SelectListItem> Downloads { get; set; }

        public List<MyTemplate> MyTemplates { get; set; }

        public string MyFieldsTemplateInHtml { get; set; }

        public bool IsVisibleFormTemplate { get; set; }
        public bool IsVisibleBtnNewTemplate { get; set; }

        public string CurrentCodTemplate { get; set; }

        public AdminMyTemplate()
        {
            Downloads = new List<SelectListItem>();
            MyTemplates = new List<MyTemplate>();
        }
    }
}