using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using OpeCDLib.Models;
using SmartOrderService.DB;

namespace SmartOrderService.Services
{
    public class OpeCDService
    {
        public List<Venta> CreateVentas(List<so_sale> sales) {

            List<Venta> ventas = new List<Venta>();

            foreach (var sale in sales) {

                var delivery = sale.so_delivery;

                int ReasonDevolutionCode,CUC,CodOpe;

                var facturado = sale.so_invoice_opefactura.FirstOrDefault() != null;

                try
                {
                    ReasonDevolutionCode = Int32.Parse(
                                           sale.so_delivery
                                           .so_delivery_devolution
                                           .so_reason_devolution.code);

                }
                catch (Exception e)
                {
                    ReasonDevolutionCode = 0;
                }

                try
                {                   
                    CUC = Int32.Parse(sale.so_customer.code);
                    CodOpe = (sale.tag != null && sale.tag.Length > 0) ? Int32.Parse(sale.tag) : 0;
                }
                catch (Exception e)
                {
                    CUC = 0;
                    CodOpe = 0;
                }

                var detalles = createDetalles(sale.so_sale_detail.ToList());
                detalles.AddRange(createDetalles(sale.so_sale_replacement.ToList()));

                var promociones = createPromociones(sale.so_sale_promotion.ToList());

                ventas.Add(new Venta() {

                    RazonDev = ReasonDevolutionCode,
                    CodOpe = CodOpe,
                    CUC = CUC,
                    Code = sale.so_customer.code,
                    WBCSaleId = sale.saleId,
                    Detalles = detalles,
                    Facturado = facturado,
                    Promociones = promociones

                });

            } 
            
                       
            return ventas;
        }

        public List<Detalle> createDetalles(List<so_sale_replacement> replacements)
        {
            List<Detalle> detalles = new List<Detalle>();

            foreach (var replacement in replacements) {

                detalles.Add(new Detalle() {
                    Producto = Int32.Parse(replacement.so_replacement.code),
                    Cantidad = replacement.amount,
                    CantidadCredito = 0
                });
            }
            return detalles;
        }

        public List<Detalle> createDetalles(List<so_sale_detail> details)
        {
            List<Detalle> detalles = new List<Detalle>();

            foreach(var detail in details)
            {
                detalles.Add(new Detalle() {
                    Producto = Int32.Parse(detail.so_product.code),
                    Cantidad = detail.amount - detail.credit_amount,
                    CantidadCredito = detail.credit_amount,
                    Precio =  detail.sale_price == null ? 0 : (Single)detail.sale_price,
                    PrecioBase = detail.base_price == null ? 0 : (decimal)detail.base_price,
                    Descuento = detail.discount == null ? 0 : (Single)detail.discount,
                    ImporteDesc = detail.discount_amount == null ? 0 : (decimal)detail.discount_amount,
                    PorcentDesc = detail.discount_percent == null ? 0 : (decimal)detail.discount_percent,
                    PrecioNeto = detail.net_price == null ? 0 : (decimal)detail.net_price,
                    PrecioSinImpuesto = detail.price_without_taxes == null ? 0 : (decimal)detail.price_without_taxes,
                    DesctoSinImpuesto = detail.discount_without_taxes == null ? 0 : (decimal)detail.discount_without_taxes,
                    Iva = detail.vat_tax == null ? 0 : (decimal)detail.vat_tax,
                    Ieps = detail.ieps == null ? 0 : (decimal)detail.ieps,
                    IepsCuota = detail.ieps_fee == null ? 0 : (decimal)detail.ieps_fee,
                    IepsBotana = detail.ieps_snack == null ? 0 : (decimal)detail.ieps_snack,
                    TasaIva = detail.vat_tax_rate == null ? 0 : (Single)detail.vat_tax_rate,
                    TasaIeps = detail.ieps_rate == null ? 0 : (decimal)detail.ieps_rate,
                    TasaIepsCuota = detail.ieps_fee_rate == null ? 0 : (decimal)detail.ieps_fee_rate,
                    TasaIepsBotana = detail.ieps_snack_rate == null ? 0 : (decimal)detail.ieps_snack_rate,
                    Litros = detail.liters == null ? 0 : (int)detail.liters,
                });
            }
            return detalles;
        }

        internal List<Venta> CreateVentas(List<so_delivery> deliveriesReached)
        {
            List<Venta> ventas = new List<Venta>();

            foreach (var delivery in deliveriesReached)
            {
                

                int ReasonDevolutionCode, CUC, CodOpe;

                try
                {
                    ReasonDevolutionCode = Int32.Parse(delivery.so_delivery_devolution.so_reason_devolution.code);
                }
                catch (Exception e)
                {
                    ReasonDevolutionCode = 0;
                    continue;

                }

                try
                {
                    CUC = Int32.Parse(delivery.so_customer.code);
                    CodOpe =  0;
                }
                catch (Exception e)
                {
                    CUC = 0;
                    CodOpe = 0;
                }

                ventas.Add(new Venta()
                {

                    RazonDev = ReasonDevolutionCode,
                    CodOpe = CodOpe,
                    CUC = CUC,
                    Facturado = false,
                    WBCSaleId = 0
                });

            }


            return ventas;
        }

        public List<Promocion> createPromociones(List<so_sale_promotion> promotions)
        {
            List<Promocion> promociones = new List<Promocion>();

            foreach (var promotion in promotions) {

                foreach (var promociondet in promotion.so_sale_promotion_detail) {
                    promociones.Add(new Promocion() {
                        Producto = Int32.Parse(promociondet.so_product.code),
                        Cantidad = promociondet.amount,
                        Tipo = Int32.Parse(promotion.so_promotion.code),
                        Precio = promociondet.sale_price == null ? 0 : (float)promociondet.sale_price,
                        PrecioBase = promociondet.base_price == null ? 0 : (decimal)promociondet.base_price,
                        Descuento = promociondet.discount == null ? 0 : (float)promociondet.discount,
                        ImporteDesc = promociondet.discount_amount == null ? 0 : (decimal)promociondet.discount_amount,
                        PorcentDesc = promociondet.discount_percent == null ? 0 : (decimal)promociondet.discount_percent,
                        PrecioNeto = promociondet.net_price == null ? 0 : (decimal)promociondet.net_price,
                        PrecioSinImpuesto = promociondet.price_without_taxes == null ? 0 : (decimal)promociondet.price_without_taxes,
                        DesctoSinImpuesto = promociondet.discount_without_taxes == null ? 0 : (decimal)promociondet.discount_without_taxes,
                        Iva = promociondet.vat_tax == null ? 0 : (decimal)promociondet.vat_tax,
                        Ieps = promociondet.ieps == null ? 0 : (decimal)promociondet.ieps,
                        IepsCuota = promociondet.ieps_fee == null ? 0 : (decimal)promociondet.ieps_fee,
                        IepsBotana = promociondet.ieps_snack == null ? 0 : (decimal)promociondet.ieps_snack,
                        TasaIva = promociondet.vat_tax_rate == null ? 0 : (float)promociondet.vat_tax_rate,
                        TasaIeps = promociondet.ieps_rate == null ? 0 : (decimal)promociondet.ieps_rate,
                        TasaIepsCuota = promociondet.ieps_fee_rate == null ? 0 : (decimal)promociondet.ieps_fee_rate,
                        TasaIepsBotana = promociondet.ieps_snack_rate == null ? 0 : (decimal)promociondet.ieps_snack_rate,
                        Litros = promociondet.liters == null ? 0 : (int)promociondet.liters

                    });
                }

            }

            return promociones;
        }

        public List<Visita> CreateVisita(List<so_binnacle_visit> binnacleVisits, int routeCode)
        {
            List<Visita> visitas = new List<Visita>();
            foreach (var binnacle in binnacleVisits)
                visitas.Add(new Visita()
                {
                    BinnacleVisitId = binnacle.binnacleId,
                    BranchCode = int.Parse(binnacle.so_user.so_branch.code),
                    RouteCode = routeCode,
                    CustomerCode = int.Parse(binnacle.so_customer.code),
                    CheckIn = binnacle.checkin,
                    CheckOut = binnacle.checkout,
                    Latitude = binnacle.latitudeout,
                    Longitude = binnacle.longitudeout,
                    Scanned = binnacle.scanned,
                    UserCode = int.Parse(binnacle.so_user.code),
                    CreatedOn = binnacle.createdon.Value
                });

            return visitas;
        }
    }
}