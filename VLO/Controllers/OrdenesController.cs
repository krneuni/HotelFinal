using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using VLO.Models;

namespace VLO.Controllers
{
    public class OrdenesController : Controller
    {
        private Context db = new Context();

        // GET: Ordenes
        public ActionResult Index()
        {
            OrdenesViewModel All = new OrdenesViewModel();
            All.DetallePedido = db.DetallePedido.ToList();
            All.Pedido = db.Pedido.ToList();
            All.Mesa = db.Mesa.ToList();
            return View(All);
        }

        public ActionResult Ordenes()
        {
            var orden = db.Pedido.Where(x => x.Estado == 1).ToList();
            var detalle = db.DetallePedido.Where(x => x.Estado == 1).ToList();
            CocinaViewModel cvm = new CocinaViewModel();
            cvm.pedidos = orden;
            cvm.detalle = detalle;
            cvm.menus = db.Menus.ToList();
            

            
            return View(cvm);
        }
        
        [HttpGet]
        public ActionResult TerminarOrdenCocina(int iddetalle)
        {
            
            DetallePedido dp = db.DetallePedido.Find(iddetalle);
            dp.Estado = 2;
            db.Entry(dp).State = EntityState.Modified;
            db.SaveChanges();

            var pe = db.Pedido.Find(dp.IdPedido);
            var detped = db.DetallePedido.Where(x=>x.IdPedido==dp.IdPedido).ToList();
            var cantidaddetalle = detped.Count;
            //Recorrer
            var bandera = 0;
            foreach (var io in detped){if (io.Estado == 2) bandera++;}
            if (bandera == cantidaddetalle)
            {
                Pedido de = db.Pedido.Find(dp.IdPedido);
                de.Estado = 2;
                db.Entry(de).State = EntityState.Modified;
                db.SaveChanges();
            }



            return Redirect("/Ordenes/Ordenes");
        }

        public ActionResult OrdenesTerminadas()
        {
            var orden = db.Pedido.Where(x => x.Estado == 3).ToList();
            var detalle = db.DetallePedido.Where(x => x.Estado == 2).ToList();
            CocinaViewModel cvm = new CocinaViewModel();
            cvm.pedidos = orden;
            cvm.detalle = detalle;
            cvm.menus = db.Menus.ToList();
            return View(cvm);
        }

       


        //VistaMenu
        public ActionResult Menu(int? id, int? idPedido)
        {

            

            ViewBag.mesa = id;
           
                
            var Dpedido = (db.Pedido.Where(x=>x.Estado !=3 && x.IdMesa==id)).FirstOrDefault();
            var pedido = (db.Pedido.Where(x => x.Estado != 3 && x.IdMesa == id)).Count();
            if (pedido>0)
            {
                ViewBag.cliente = Dpedido.Cliente;
                ViewBag.cantidad = Dpedido.Cantidad;
                ViewBag.pedido = Dpedido.IdPedido;
            }
            
            
            
            OrdenesViewModel ovm = new OrdenesViewModel();
            ovm.DetallePedido = new List<DetallePedido>();
            ovm.Menus = db.Menus.ToList();
            ovm.TiposMenu = db.TipoMenus.ToList();
            if (Dpedido != null)
            {
                var queryOrd = db.DetallePedido.Where(d => d.IdPedido == Dpedido.IdPedido).ToList();
                ovm.DetallePedido = queryOrd;
            }
            return View(ovm);
        }


        //Agregar orden y guardar datos en las diferentes tablas
        [HttpPost]
        public async Task<ActionResult> AddOrden(AddOrdenViewModel aovm, int? idPedido)
        {
            
            int user = Convert.ToInt32(Session["Id"]);
            var buscarPedido = db.Pedido.Find(idPedido);
            if (buscarPedido != null)
            {
                buscarPedido.Estado = 1;
                db.Entry(buscarPedido).State = EntityState.Modified;
                await db.SaveChangesAsync();
            }
            if (buscarPedido == null)
            {
                Pedido p = new Pedido();
                p.Cantidad = aovm.numpersonas;
                p.Cliente = aovm.cliente;
                p.IdUser = user;
                p.Estado = 1;
                p.IdMesa = aovm.mesa;
                p.FechaInicial = Convert.ToString(DateTime.Now);
                db.Pedido.Add(p);
                await db.SaveChangesAsync();
            }
           
 

            var lastPedido = db.Pedido.OrderByDescending(x=>x.IdPedido).FirstOrDefault();
            for (var i = 0; i < aovm.id.Count; i++)
            {
                DetallePedido dp = new DetallePedido();
                dp.IdMenu = aovm.id[i];
                dp.cantidad = aovm.cantidad[i];
                dp.IdPedido = lastPedido.IdPedido;
                dp.Comentarios = aovm.comentarios[i];
                dp.Estado = 1;
                dp.Termino = ((aovm.termino[i] == null || aovm.termino[i]== "undefined") ?"NaN":aovm.termino[i]);
                db.DetallePedido.Add(dp);
                await db.SaveChangesAsync();

                //Encontrar el menu
                var menu = db.Menus.Find(dp.IdMenu);
                //Buscar los menus en la receta
                var recmenu = (from u in db.Receta where u.IdMenu == menu.IdMenu select u).ToList();
                //Recorrer
                foreach (var io in recmenu)
                {
                    //Encontrar los productos que se utilizan
                    Productos de = db.Productos.Find(io.IdProducto);
                    
                    //Resta de la cantidad que se pide menos la cantidad utilizada

                    var Descuento = io.CantidadUtilizada * dp.cantidad;
                    de.Cantidad =de.Cantidad - Descuento;
                    db.Entry(de).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }

            //Encontrar las mesas
            Mesa d = db.Mesa.Find(aovm.mesa);
            //Cambia el estado de la mesa
            d.Estado = false;
            db.Entry(d).State = EntityState.Modified;
            db.SaveChanges();

            return Redirect("Index");
        }
        
        [HttpPost]
        public ActionResult Pagos(int? idpedido)
        {
            
            var orden = db.Pedido.Where(x => x.Estado == 2).ToList();
            var queryOrd = db.DetallePedido.Where(d => d.IdPedido == idpedido).ToList();
            CocinaViewModel cvm = new CocinaViewModel();
            cvm.pedidos = orden;
            cvm.detalle = queryOrd;
            cvm.menus = db.Menus.ToList();
            return View(cvm);
        }

        [HttpPost]
        public ActionResult RealizarPago(double txttotal,double Total, int idPedido, int idDetalle, double Descuento, string Descripcion, double propina, AddOrdenViewModel cvm)
        {
            
            Factura p = new Factura();
            //p.NumFactura =1;
            p.IdPedido = idPedido;
            p.TotalNeto = txttotal;
            p.Total = Total;
            p.Descuento = Descuento;
            p.Descripcion = Descripcion;
            p.Propina = propina;
           
            
            p.FechaFactura = DateTime.Now.Date;
            db.Factura.Add(p);
            db.SaveChanges();


            
            var det = (from x in db.DetallePedido where x.IdPedido == idPedido select x).ToList();
            foreach (var i in det)
            {
                DetallePedido de = db.DetallePedido.Find(i.IdDetalle);
                de.Estado = 3;
                db.Entry(de).State = EntityState.Modified;
                db.SaveChanges();


            }

            //Buscar cada Pedido
            var pedido = (from u in db.Pedido where u.IdPedido == idPedido select u).ToList();
            //Recorrer
            foreach (var io in pedido)
            {
                Pedido pe = db.Pedido.Find(io.IdPedido);
                pe.Estado = 3;
                db.Entry(pe).State = EntityState.Modified;
                db.SaveChanges();
            }

            
            //var mesa = db.Mesa.Where(x => x.IdMesa == pedido[0].IdMesa).FirstOrDefault();
            Mesa d = db.Mesa.Find(pedido[0].IdMesa);
                d.Estado = true;
                db.Entry(d).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Factura","Facturas", new { idPedido = pedido[0].IdPedido, idDetalle =  det[0].IdDetalle  });


            }
            
        }
        

    }