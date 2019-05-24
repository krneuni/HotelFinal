using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VLO.Models
{
    public class FacturaViewModel
    {
        public List<Pedido> pedidos;
        public List<DetallePedido> detalle;
        public List<Menu> menus;
        public List<Factura> factura;
    }
}