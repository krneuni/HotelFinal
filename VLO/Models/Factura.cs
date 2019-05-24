using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace VLO.Models
{
    public class Factura
    {
        [Key]
        public int NumFactura { get; set; }
        
        //Relacion con DetallePedido
        public int IdPedido { get; set; }
        public virtual Pedido Pedido { get; set; }
        //Fin de la relación

        [DataType(DataType.Currency)]
        [Display(Name = "Precio Total")]
        public double TotalNeto { get; set; }

        public double Total { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "Descuento")]
        public double Descuento { get; set; }

        
        [DataType(DataType.Date, ErrorMessage = "Ingresar una fecha valida")]
        [Display(Name = "Fecha de facturación ")]
        public DateTime FechaFactura { get; set; }

        
        public double Propina { get; set; }
        public string Descripcion { get; set; }


    }
}