using SmartOrderService.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SmartOrderService.Models.DTO.Invoice;
using SmartOrderService.Models.DTO;
using SmartOrderService.CustomExceptions;

namespace SmartOrderService.Services
{
    public class OpeFacturaService
    {
        private SmartOrderModel db = new SmartOrderModel();

        public InvoiceOpeFacturaDto createDto(so_sale so_sale, InvoiceDataDto invoiceData)
        {
            InvoiceOpeFacturaDto InvoiceOpeFacturaDto = new InvoiceOpeFacturaDto();

            InvoiceOpeFacturaDto.header = createHeader(so_sale, invoiceData);
            Dictionary<Int32, SimpleInvoiceLineOpeFactura> lines = new Dictionary<Int32, SimpleInvoiceLineOpeFactura>();

            int codigo_ope = 0;
            if (invoiceData != null)
                codigo_ope = Convert.ToInt32(invoiceData.Codope);

            int typePay = 1;
            if (invoiceData != null)
            {
                typePay = invoiceData.CreditApply ? 2 : 1;
            }


            foreach (so_sale_detail saleDetail in so_sale.so_sale_detail)
            {
                lines.Add(saleDetail.productId, createLine(saleDetail, typePay));
            }

            List<so_sale_promotion> promotions = so_sale.so_sale_promotion.Where(sp => sp.so_promotion.type == 3).ToList();

            foreach (so_sale_promotion salePromotion in promotions)
            {
                foreach (so_sale_promotion_detail saleDetailPromotion in salePromotion.so_sale_promotion_detail)
                {
                    if (lines.ContainsKey(saleDetailPromotion.productId))
                    {
                        SimpleInvoiceLineOpeFactura line = lines[saleDetailPromotion.productId];
                        line.cantidad += saleDetailPromotion.amount;
                        line.tipo_promocion = Convert.ToInt32(saleDetailPromotion.so_sale_promotion.so_promotion.code);
                        line.cantidad_promocion = saleDetailPromotion.amount;


                        int amountDevolution = 0;

                        if (so_sale.deliveryId != null && so_sale.deliveryId > 0)
                        {
                            so_delivery_promotion_detail deliveryPromotionDetail = db.so_delivery_promotion_detail.Where(dpd => dpd.productId == saleDetailPromotion.productId
                             && dpd.so_delivery_promotion.promotionId == saleDetailPromotion.so_sale_promotion.promotionId
                             && dpd.so_delivery_promotion.so_delivery.deliveryId == saleDetailPromotion.so_sale_promotion.so_sale.deliveryId).FirstOrDefault();
                            amountDevolution = deliveryPromotionDetail.amount;
                        }

                        amountDevolution -= saleDetailPromotion.amount;
                        if (amountDevolution < 0)
                            amountDevolution = 0;


                        line.devolucion += amountDevolution;

                    }
                    else
                    {
                        lines.Add(saleDetailPromotion.productId, createLine(saleDetailPromotion, typePay));
                    }

                }

            }

            var products = so_sale.so_sale_detail.Select(sd => sd.productId).ToList();

            List<so_delivery_detail> detailsNoSaled = db.so_delivery_detail.Where(dd => !products.Contains(dd.productId)
                && dd.so_delivery.deliveryId == so_sale.deliveryId).ToList();

            if (detailsNoSaled.Count > 0)
            {

                foreach (so_delivery_detail detail in detailsNoSaled)
                {
                    var billingId = detail.so_product.billing_dataId;
                    if (billingId == null)
                    {
                        throw new ProductNotFoundBillingException();
                    }

                    if (lines.ContainsKey(detail.productId))
                    {
                        SimpleInvoiceLineOpeFactura line = lines[detail.productId];
                        line.devolucion = detail.amount;
                    }
                    else
                    {
                        lines.Add(detail.productId, createDevolutionLine(detail, typePay));
                    }


                }
            }

            if (so_sale.so_delivery != null)

                foreach (so_delivery_promotion deliveryPromotion in so_sale.so_delivery.so_delivery_promotion)
                {
                    var salePromotion = so_sale.so_sale_promotion.Where(sp => sp.promotionId == deliveryPromotion.promotionId).FirstOrDefault();

                    if (salePromotion == null)
                    {
                        foreach (so_delivery_promotion_detail deliveryPromotionDetail in deliveryPromotion.so_delivery_promotion_detail)
                        {
                            var billingId = deliveryPromotionDetail.so_product.billing_dataId;
                            if (billingId == null)
                            {
                                throw new ProductNotFoundBillingException();
                            }

                            if (lines.ContainsKey(deliveryPromotionDetail.productId))
                            {
                                SimpleInvoiceLineOpeFactura line = lines[deliveryPromotionDetail.productId];
                                line.devolucion = deliveryPromotionDetail.amount;
                            }
                            else
                            {
                                lines.Add(deliveryPromotionDetail.productId, createDevolutionLine(deliveryPromotionDetail, typePay));
                            }
                        }
                    }


                }

            InvoiceOpeFacturaDto.lines.AddRange(lines.Values);
            return InvoiceOpeFacturaDto;
        }

        private InvoiceHeaderOpeFacturaDto createHeader(so_sale sale, InvoiceDataDto invoiceData)
        {
            InvoiceHeaderOpeFacturaDto InvoiceHeaderOpeFacturaDto = new InvoiceHeaderOpeFacturaDto();

            var route = db.so_user_route.Where(ur => ur.userId == sale.userId).FirstOrDefault();
            var delivery = sale.so_delivery;

            int puntoVenta = 0;
            int.TryParse(sale.so_user.so_branch.code, out puntoVenta);

            InvoiceHeaderOpeFacturaDto.id = sale.saleId;
            InvoiceHeaderOpeFacturaDto.opecdid = createOpeCdId(sale);
            InvoiceHeaderOpeFacturaDto.nota_venta = delivery == null ? "0" : delivery.sale_note.ToString();
            InvoiceHeaderOpeFacturaDto.cuc = sale.so_customer.code;
            InvoiceHeaderOpeFacturaDto.fecha = sale.date.ToString("yyyy-MM-dd");
            InvoiceHeaderOpeFacturaDto.carga = sale.so_inventory.order;
            InvoiceHeaderOpeFacturaDto.ruta = route.so_route.code;
            InvoiceHeaderOpeFacturaDto.punto_venta = puntoVenta;
            if (invoiceData != null)
            {
                InvoiceHeaderOpeFacturaDto.codope = invoiceData.Codope;
                InvoiceHeaderOpeFacturaDto.direccion_fiscal = invoiceData.Address;
                InvoiceHeaderOpeFacturaDto.razon_social = invoiceData.BussinessName;
                InvoiceHeaderOpeFacturaDto.rfc = invoiceData.CustomerRFC;
                InvoiceHeaderOpeFacturaDto.inventsite_id = invoiceData.InventSiteId;
                InvoiceHeaderOpeFacturaDto.siteId = invoiceData.SiteId;
                InvoiceHeaderOpeFacturaDto.supervisor = invoiceData.SalesResponsible;
                InvoiceHeaderOpeFacturaDto.usoCFDI = invoiceData.UseCFDI;

            }

            return InvoiceHeaderOpeFacturaDto;
        }

        private string createOpeCdId(so_sale sale)
        {
            if(sale.so_delivery == null)
            {
                return "WBC-DS." + sale.saleId; 
            }
            long code = 0;
            string deliveryCode = sale.so_delivery.code;
            if(Int64.TryParse(deliveryCode, out code))
            {
                return code.ToString();
            }
            return "WBC-" + sale.saleId;

        }

        private SimpleInvoiceLineOpeFactura createLine(so_sale_detail detail, int typePay)
        {
            SimpleInvoiceLineOpeFactura line = new SimpleInvoiceLineOpeFactura();



            int amountDevolution = 0;

            var sale = detail.so_sale;

            if (sale.deliveryId != null && sale.deliveryId > 0)
            {
                so_delivery_detail deliveryDetail = db.so_delivery_detail.Where(dd => dd.so_delivery.deliveryId == sale.deliveryId
                && dd.so_product.productId == detail.productId).FirstOrDefault();
                if (deliveryDetail != null)
                    amountDevolution = deliveryDetail.amount;
            }

            amountDevolution -= detail.amount;
            if (amountDevolution < 0)
                amountDevolution = 0;
            
            line.codigo_ope = Convert.ToInt32(detail.so_product.code); ;
            line.tipo_credito = typePay;
            line.unidad_venta = detail.so_product.so_product_tax.unit;
            line.devolucion = amountDevolution;
            line.cantidad = detail.amount;
           
                line.precio = detail.sale_price ?? 0;
            line.descuento = detail.discount ?? 0;
            line.porcentaje_iva = detail.vat_tax_rate ?? 0;

            return line;
        }

        private SimpleInvoiceLineOpeFactura createLine(so_sale_promotion_detail detail, int typePay)
        {
            SimpleInvoiceLineOpeFactura line = new SimpleInvoiceLineOpeFactura();


            int amountDevolution = 0;

            var sale = detail.so_sale_promotion.so_sale;

            if (sale.deliveryId != null && sale.deliveryId > 0)
            {
                so_delivery_promotion_detail deliveryPromotionDetail = db.so_delivery_promotion_detail.Where(dpd => dpd.productId == detail.productId
                 && dpd.so_delivery_promotion.promotionId == detail.so_sale_promotion.promotionId
                 && dpd.so_delivery_promotion.so_delivery.deliveryId == detail.so_sale_promotion.so_sale.deliveryId).FirstOrDefault();
                amountDevolution = deliveryPromotionDetail.amount;
            }

            amountDevolution -= detail.amount;
            if (amountDevolution < 0)
                amountDevolution = 0;

            line.codigo_ope = Convert.ToInt32(detail.so_product.code);
            line.tipo_credito = typePay;
            line.unidad_venta = detail.so_product.so_product_tax.unit;
            line.devolucion = amountDevolution;
            line.cantidad = detail.amount;
            line.precio = detail.sale_price ?? 0;
            line.descuento = detail.discount ?? 0;
            line.porcentaje_iva = detail.vat_tax_rate ?? 0;
           
            line.cantidad_promocion = detail.amount;
            line.tipo_promocion = Convert.ToInt32(detail.so_sale_promotion.so_promotion.code);
            return line;
        }

        private SimpleInvoiceLineOpeFactura createDevolutionLine(so_delivery_detail deliveryDetail, int typePay)
        {
            SimpleInvoiceLineOpeFactura line = new SimpleInvoiceLineOpeFactura();

            var product = db.so_product_tax.Where(p => p.productId == deliveryDetail.so_product.productId).FirstOrDefault();

            line.codigo_ope = Convert.ToInt32(deliveryDetail.so_product.code);
            line.tipo_credito = typePay;
            line.unidad_venta = product != null ? product.unit : "";
            line.devolucion = deliveryDetail.amount;
            line.cantidad = 0;
            line.precio = deliveryDetail.price ?? 0;
            line.descuento = deliveryDetail.discount ?? 0;
            line.porcentaje_iva = deliveryDetail.vat_rate ?? 0;
           


            return line;
        }

        /**
         * Crear una linea de devolucion apartir de los datos de la entrega-promocion
         */
        private SimpleInvoiceLineOpeFactura createDevolutionLine(so_delivery_promotion_detail deliveryPromotionDetail, int typePay)
        {
            SimpleInvoiceLineOpeFactura line = new SimpleInvoiceLineOpeFactura();

            var product = db.so_product_tax.Where(p => p.productId == deliveryPromotionDetail.so_product.productId).FirstOrDefault();

            line.codigo_ope = Convert.ToInt32(deliveryPromotionDetail.so_product.code);
            line.tipo_credito = typePay;
            line.unidad_venta = product != null ? product.unit : "";
            line.devolucion = deliveryPromotionDetail.amount;
            line.cantidad = 0;
            line.tipo_promocion = Convert.ToInt32(deliveryPromotionDetail.so_delivery_promotion.so_promotion.code);
            line.precio = deliveryPromotionDetail.price;
            line.descuento = deliveryPromotionDetail.discount ?? 0;
            line.porcentaje_iva = deliveryPromotionDetail.vat_rate ?? 0;
            return line;
        }

    }
}