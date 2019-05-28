using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using VLO.Models;

namespace VLO.Controllers
{
    public class FacturasController : Controller
    {
        private Context db = new Context();

        // GET: Facturas
        public ActionResult Index()
        {
            var factura = db.Factura.Include(f => f.Pedido);
            
            return View(factura.ToList());
        }


        [HttpPost]
        public ActionResult Index(DateTime FI, DateTime FF)
        {
            try
            {
                DateTime FM = FF.AddDays(1);
                var Registro = (from t in db.Factura
                                where (t.FechaFactura >= FI && t.FechaFactura <= FM)
                                orderby t.NumFactura ascending
                                select t).ToList();


                if (Registro != null)
                {
                    ViewBag.sum = (from x in db.Factura where (x.FechaFactura >= FI && x.FechaFactura <= FM) select x.TotalNeto).Sum();
                        
                }
                else
                {
                    ViewBag.Error = "No hay datos";
                }
                return View(Registro);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult Factura()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Factura(int? idPedido, int? idDetalle)
        {

            var orden = db.Pedido.Where(x => x.Estado == 3 && x.IdPedido==idPedido).ToList();
            var detalle = db.DetallePedido.Where(d => d.IdPedido == idPedido).ToList();
            //var fact = db.Factura.Where(x => x.IdDetalle == idDetalle).ToList();
            FacturaViewModel cvm = new FacturaViewModel();
            cvm.pedidos = orden;
            cvm.detalle = detalle;
            cvm.menus = db.Menus.ToList();
            cvm.factura = db.Factura.ToList();
            return View(cvm);
        }


        public ActionResult Reporte(int? idPedido, int? idDetalle)
        {
           

            var orden = db.Pedido.Where(x => x.Estado == 3 && x.IdPedido == idPedido).ToList();
            var detalle = db.DetallePedido.Where(d => d.IdPedido == idPedido).ToList();
            //var fact = db.Factura.Where(x => x.IdDetalle == idDetalle).ToList();
            FacturaViewModel cvm = new FacturaViewModel();
            cvm.pedidos = orden;
            cvm.detalle = detalle;
            cvm.menus = db.Menus.ToList();
            cvm.factura = db.Factura.ToList();
            return View(cvm);

        }

        // GET: Facturas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Factura factura = db.Factura.Find(id);
            if (factura == null)
            {
                return HttpNotFound();
            }
            return View(factura);
        }

        // GET: Facturas/Create
        public ActionResult Create()
        {
            ViewBag.IdDetalle = new SelectList(db.DetallePedido, "IdDetalle", "IdDetalle");
            return View();
        }

        // POST: Facturas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "NumFactura,IdDetalle,Precio,Descuento,FechaFactura, Descripcion, Propina")] Factura factura)
        {
            if (ModelState.IsValid)
            {
                db.Factura.Add(factura);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdDetalle = new SelectList(db.Pedido, "IdPedido", "IdPedido", factura.IdPedido);
            return View(factura);
        }

        // GET: Facturas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Factura factura = db.Factura.Find(id);
            if (factura == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdDetalle = new SelectList(db.Pedido, "IdPedido", "IdPedido", factura.IdPedido);
            return View(factura);
        }

        // POST: Facturas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "NumFactura,IdDetalle,Precio,Descuento,FechaFactura, Descripcion, Propina")] Factura factura)
        {
            if (ModelState.IsValid)
            {
                db.Entry(factura).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdDetalle = new SelectList(db.Pedido, "IdPedido", "IdPedido", factura.IdPedido);
            return View(factura);
        }

        // GET: Facturas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Factura factura = db.Factura.Find(id);
            if (factura == null)
            {
                return HttpNotFound();
            }
            return View(factura);
        }

        // POST: Facturas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Factura factura = db.Factura.Find(id);
            db.Factura.Remove(factura);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
