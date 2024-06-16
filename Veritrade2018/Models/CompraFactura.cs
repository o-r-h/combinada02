using System.ComponentModel.DataAnnotations;

namespace Veritrade2018.Models
{
    public class CompraFactura
    {
        [Display(ResourceType = typeof(Resources.Resources), Name = "Compra_Pedido_Ruc")]
        public string Ruc { get; set; }

        [Display(ResourceType = typeof(Resources.Resources), Name = "Compra_Pedido_RazonSocial")]
        public string RazonSocial { get; set; }

        [Display(ResourceType = typeof(Resources.Resources), Name = "Compra_Pedido_Direccion")]
        public string Direccion { get; set; }

        public bool IsFactura { get; set; }
    }
}