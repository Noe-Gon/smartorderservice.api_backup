using LibreriaCFDI;
using LibreriaCFDI.Clases;
using SmartOrderService.CustomExceptions;
using SmartOrderService.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Utils
{
    public class InvoiceData
    {
        //agregar comentario
        SmartOrderModel db = new SmartOrderModel();

        public Factura Stamp(int invoiceId)
        {
            //Timbrado timbrado = new Timbrado();
            Factura factura = null;

            var invoice = db.facturas.Where(f => f.foliointerno == invoiceId).FirstOrDefault();
            //var invoice = (from table in db.facturas orderby table.foliointerno descending
            //           select table).FirstOrDefault();

            if (invoice == null) {
                throw new InvoiceException();
            }
            else
            {
                //hacer recorrido
                Facturacion.DatosFacturas datosfactura = new Facturacion.DatosFacturas();
                factura = datosfactura.Llenarfactura(invoice.foliointerno);

            }


            return factura;
        }
    }
}