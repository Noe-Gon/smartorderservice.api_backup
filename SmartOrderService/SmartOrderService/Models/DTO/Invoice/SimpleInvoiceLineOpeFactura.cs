using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.DTO.Invoice
{
    public class SimpleInvoiceLineOpeFactura
    {
        public int codigo_ope = 0;
        public string codigo_ax = "";
        public int tipo_credito = 0;
        public string unidad_venta = "";
        public int cantidad = 0;
        public float precio = 0;
        public float descuento = 0;
        public int devolucion = 0;
        public int prestamo = 0;
        public int subpedido = 0;
        public int tipo_promocion = 0;
        public int cantidad_promocion = 0;
        public float porcentaje_iva = 0;
    }
}