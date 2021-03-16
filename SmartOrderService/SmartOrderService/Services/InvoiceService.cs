using LibreriaCFDI;
using LibreriaCFDI.Clases;
using LibreriaCFDI.Enums;
using SmartOrderService.DB;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using SmartOrderService.Models.DTO;
using SmartOrderService.CustomExceptions;
using SmartOrderService.Utils;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Formatters.Soap;
using System.Xml.Serialization;
using SmartOrderService.Templates;
using System.Xml;
using SmartOrderService.Models.DTO.Invoice;
using SmartOrderService.Models.Responses;
using System.Net.Http;
using AutoMapper;

namespace SmartOrderService.Services
{
    public class InvoiceService
    {
        private SmartOrderModel db = new SmartOrderModel();
        public bool Stamp2()
        {

            Timbrado timbrado = new Timbrado();
            #region Emisor
            Emisor emisor = new Emisor();
            emisor.Nombre = "Embotelladora Bepensa SA de CV";
            emisor.RFC = "EBE7711037Y5";
            emisor.RegimenFiscal.Add("REGIMEN GENERAL DE LEY PERSONAS MORALES");
            emisor.LugarDeExpedicion.Calle = "Calle 29";
            emisor.LugarDeExpedicion.CodigoPostal = "97320";
            emisor.LugarDeExpedicion.Colonia = "Agricola San Antonio";
            emisor.LugarDeExpedicion.Estado = "Yucatan";
            emisor.LugarDeExpedicion.Localidad = "Progreso";
            emisor.LugarDeExpedicion.Municipio = "Nueva Yucalpeten";
            emisor.LugarDeExpedicion.NumeroInterior = "";
            emisor.LugarDeExpedicion.NumeroExterior = "1501";
            emisor.LugarDeExpedicion.Pais = "México";
            emisor.LugarDeExpedicion.Referencia = "";
            emisor.DomicilioFiscal.Calle = "Calle 29";
            emisor.DomicilioFiscal.CodigoPostal = "97320";
            emisor.DomicilioFiscal.Colonia = "Revolucion";
            emisor.DomicilioFiscal.Estado = "Yucatan";
            emisor.DomicilioFiscal.Localidad = "Nueva Yucalpeten";
            emisor.DomicilioFiscal.Municipio = "Progreso";
            emisor.DomicilioFiscal.NumeroExterior = "1501";
            emisor.DomicilioFiscal.NumeroInterior = "";
            emisor.DomicilioFiscal.Pais = "México";
            emisor.DomicilioFiscal.Referencia = "";
            #endregion

            #region Receptor
            Receptor receptor = new Receptor();
            receptor.Nombre = "ArkusNexus";
            receptor.RFC = "AICB850915N42";
            receptor.Domicilio.Calle = "Rio Suchiate";
            receptor.Domicilio.CodigoPostal = "22150";
            receptor.Domicilio.Colonia = "Revolucion";
            receptor.Domicilio.Estado = "Baja California";
            receptor.Domicilio.Localidad = "Tijuana";
            receptor.Domicilio.Municipio = "Tijuana";
            receptor.Domicilio.NumeroInterior = "";
            receptor.Domicilio.NumeroExterior = "1246";
            receptor.Domicilio.Pais = "México";
            receptor.Domicilio.Referencia = "";
            #endregion

            #region Concepto
            Concepto concepto = new Concepto();
            concepto.Cantidad = 1;
            concepto.Descripcion = "COCA-COLA 3L PET 6";
            concepto.Importe = decimal.Parse("132.9");
            concepto.NumeroIdentificacion = "34";
            concepto.Unidad = "CJ";
            concepto.ValorUnitario = decimal.Parse("132.9");
            List<Concepto> conceptos = new List<Concepto> { concepto };
            #endregion
            #region Impuestos
            Impuestos impuestos = new Impuestos();
            impuestos.IncluyeImpuestosRetenidos = false;
            impuestos.IncluyeImpuestosTraslado = true;
            impuestos.Traslados.Add(new Traslado { Importe = decimal.Parse("21.3"), Tasa = decimal.Parse("16.00"), Impuesto = ImpuestosTraslado.IVA });
            impuestos.TotalImpuestosRetenidos = 0;
            impuestos.TotalImpuestosTraslado = decimal.Parse("21.3");
            #endregion

            #region Factura
            Factura factura = new Factura();
            factura.Certificado = "";
            factura.CondicionesDePago = "";
            factura.Descuento = 0;
            factura.Fecha = DateTime.Today.ToString();
            factura.FechaFolioFiscalOriginal = default(DateTime);
            factura.Folio = (999999).ToString();
            factura.FolioFiscalOriginal = "";
            factura.FormaDePago = "EL TOTAL DE ESTA FACTURA SERA PAGADA EN UNA SOLA EXHIBICIÓN";
            factura.IncluyeMontonFolioFiscalOriginal = false;
            factura.IncluyeMontonFolioFiscalOriginal = false;
            factura.IncluyeFechaFolioFiscalOriginal = false;
            factura.LugarExpedicion = "Progreso Yucatan";
            factura.MetodoDePago = "No Identificado";
            factura.Moneda = "MXN";
            factura.MotivoDescuento = "";
            factura.NoCertificado = "";
            factura.NumeroCuentaPago = "No Identificado";
            factura.SerieFolioFiscalOriginal = "";
            factura.Sello = "";
            factura.Serie = "PB";
            factura.SubTotal = decimal.Parse("132.9");
            factura.TipoCambio = "1";
            factura.TipoComprobante = TipoComprobante.Ingreso;
            factura.Total = decimal.Parse("154.1");
            factura.Version = "3.2";

            //Este uso es solo para uso de pruebas, se puede implementar directo sobre el objeto factura.X el llenado de la //información.
            factura.Emisor = emisor;
            factura.Receptor = receptor;
            factura.Impuestos = impuestos;
            factura.Conceptos = conceptos;
            #endregion
            bool result = true;

            if (timbrado.CrearFactura(factura))
            {
                factura = timbrado.GeneraTimbrado("MFW-11373", "bepensa324@", "http://72.3.203.137/CFDI/wsCFDI.asmx");

                //Existio un error en la facturacion electronica
                if (timbrado.Errores.Count > 0)
                {
                    result = false;
                }
            }
            else
            {
                if (timbrado.Errores.Count > 0)
                {
                    result = false;
                }
            }


            return result;
        }


        [Obsolete]
        public Factura Stamp(int InvoiceId)
        {
            InvoiceData data = new InvoiceData();

            var factura = data.Stamp(InvoiceId);


            var timbrado = new Timbrado();

            string User, Password, URL;

            var IsDefaultConfig = db.facturas.Where(f => f.foliointerno == InvoiceId).Select(x => x.cia).FirstOrDefault() == 37;


            if (IsDefaultConfig)
            {
                User = ConfigurationManager.AppSettings["USER_BEPENSA1"];
                Password = ConfigurationManager.AppSettings["PASS_BEPENSA1"];
                URL = ConfigurationManager.AppSettings["WS_MASTEREDI"];
            }
            else
            {
                User = ConfigurationManager.AppSettings["USER_BEPENSA2"];
                Password = ConfigurationManager.AppSettings["PASS_BEPENSA2"];
                URL = ConfigurationManager.AppSettings["WS_MASTEREDI"];
            }



            if (timbrado.CrearFactura(factura))
            {

                factura = timbrado.GeneraTimbrado(User, Password, URL);

            }

            if (timbrado.Errores.Count > 0)
            {
                throw new InvoiceException(timbrado.Errores.FirstOrDefault());
            }


            registerBilling(InvoiceId);

            return factura;

        }

        public HttpStatusCode StampInOpeFactura(int InvoiceId)
        {

            string URL = ConfigurationManager.AppSettings["WS_INVOICE"];
            return HttpUtil.StampInOpeFactura(URL, InvoiceId);
        }

        public void registerBilling(int invoiceId)
        {
            var fact = db.facturas_so_sale.Where(fs => fs.foliointerno == invoiceId).FirstOrDefault();

            if (fact != null)
            {

                RegisterCustomerInvoice(fact.so_sale.customerId, fact.so_sale.so_user.so_branch.code, fact.so_sale.so_user.code);

            }
        }

        public bool existsInvoiceStamp(int invoiceId)
        {
            bool result;
            try
            {
                result = getInvoiceFromFile(invoiceId) != null;
            }
            catch (EntityNotFoundException)
            {
                result = false;
            }

            return result;

        }

        public bool saveInvoice(Factura invoice)
        {

            string fileName = invoice.Serie + "_" + invoice.Folio;

            string path = createPath(fileName);

            bool SaveQR = saveQR(invoice.ImagenQR, path, fileName);
            bool SaveDocument = saveInvoiceObject(invoice, path, fileName);

            bool result = SaveQR && SaveDocument;

            return result;
        }


        public bool saveInvoiceObject(Factura factura, string path, string filename)
        {

            factura.ImagenQR = null;
            factura.QR = "";
            string json = JsonConvert.SerializeObject(factura);
            saveFile(json, path, filename, ".txt");
            return true;
        }

        public string createPath(string fileName)
        {

            string location = ConfigurationManager.AppSettings["PathInvoices"];//@"C:\invoices\";

            string pathString = System.IO.Path.Combine(location, fileName);

            System.IO.Directory.CreateDirectory(pathString);

            return pathString;
        }

        [Obsolete]
        public Image createImageForInvoice(int id)
        {
            var invoice = getInvoiceFromFile(id);
            var service = new HtmlService();
            var path = getPathInvoice(invoice.Serie, invoice.Folio);
            var fileName = invoice.Serie + "-" + invoice.Folio + ".png";
            var pathQR = getPathQR(invoice.Serie, invoice.Folio);

            var html = service.createHtml(HtmlService.IMAGE, invoice, pathQR);

            var image = new ImageCreatorService().createImage(html);

            path = System.IO.Path.Combine(path, fileName);
            image.Save(path, ImageFormat.Png);

            return image;

        }

        public byte[] getImageFromOpeFactura(int id)
        {
            string URL = ConfigurationManager.AppSettings["WS_INVOICE"];
            return HttpUtil.GetImageOpeFactura(URL, id);
        }

        public string getPathInvoice(string serie, string folio)
        {
            string path = ConfigurationManager.AppSettings["PathInvoices"];
            string subfolder = serie + "_" + folio;
            path = System.IO.Path.Combine(path, subfolder);

            return path;
        }

        public string getPathQR(string serie, string folio)
        {
            string path = ConfigurationManager.AppSettings["PathInvoices"];
            string subfolder = serie + "_" + folio;
            string fileName = subfolder + "_qr.png";

            path = System.IO.Path.Combine(path, subfolder, fileName);

            return path;
        }

        private Factura getInvoiceFromFile(int id)
        {
            var invoice = getInvoice(id);

            string fileName = "", subfolder = "";


            string location = ConfigurationManager.AppSettings["PathInvoices"];
            Factura factura = null;
            if (invoice != null)
            {
                subfolder = invoice.serie + "_" + invoice.factura1;
                string pathString = System.IO.Path.Combine(location, subfolder);

                fileName = subfolder + ".txt";

                pathString = System.IO.Path.Combine(pathString, fileName);

                if (File.Exists(pathString))
                {
                    var file = File.OpenText(pathString);
                    var json = file.ReadToEnd();
                    factura = JsonConvert.DeserializeObject<Factura>(json);
                }
                else throw new EntityNotFoundException();
            }

            return factura;
        }

        private factura getInvoice(int invoiceId)
        {
            var invoice = db.facturas.Where(i => i.foliointerno == invoiceId).FirstOrDefault();

            return invoice;
        }

        public bool saveFile(string data, string path, string filename, string extension)
        {
            if (data == null || data.Length == 0)
                data = "peor es nada";

            path = System.IO.Path.Combine(path, filename + extension);

            System.IO.File.WriteAllText(path, data);

            return true;
        }


        public bool saveQR(Bitmap qr, string path, string fileName)
        {
            bool result;
            try
            {
                path = System.IO.Path.Combine(path, fileName + "_qr.png");
                qr.Save(path, ImageFormat.Png);
                result = true;
            }
            catch (Exception)
            {
                result = false;
            }

            return result;

        }

        public List<string> CreateLayout(Factura factura)
        {

            List<string> lines = new List<string>();

            lines.Add("Factura");

            lines.Add(factura.Emisor.Nombre);

            lines.Add("RFC: " + factura.Emisor.RFC);

            lines.Add("Factura: " + factura.Serie + factura.Folio);

            lines.Add("Fecha: " + factura.Fecha.ToString());

            lines.Add("Cliente: " + factura.Receptor.Nombre);


            lines.Add("RFC: " + factura.Receptor.RFC);

            var domicilio = factura.Receptor.Domicilio;

            var sDomicilio = domicilio.Calle;

            lines.Add("Domicilio: " + sDomicilio);

            lines.Add("");

            lines.Add("==============================");

            lines.Add("Productos");


            lines.Add("Cant".PadRight(10) + "Unid".PadRight(10) + "IVA".PadRight(10));
            lines.Add("PU".PadRight(10) + "Descto".PadRight(10) + "Importe".PadRight(10));

            lines.Add("==============================");

            var conceptos = factura.Conceptos;

            lines.Add(" ");

            foreach (var concepto in conceptos)
            {

                lines.Add(concepto.Descripcion);

                var cantidad = concepto.Cantidad.ToString().PadRight(10);
                var unidad = concepto.Unidad.ToString().PadRight(10);
                var impuesto = "".PadRight(10);


                var precioUnitario = ("$" + concepto.ValorUnitario).PadRight(10);
                var descuento = "$0.00".PadRight(10);
                var importe = ("$" + concepto.Importe).PadRight(10);

                lines.Add(cantidad + unidad + impuesto);
                lines.Add(precioUnitario + descuento + importe);

            }

            lines.Add(" ");

            lines.Add("Subtotal:".PadRight(15) + "$" + factura.SubTotal);
            lines.Add("I.V.A 16%:".PadRight(15) + "$" + factura.Impuestos.TotalImpuestosTraslado);
            lines.Add("Total:".PadRight(15) + "$" + factura.Total);

            lines.Add(" ");


            lines.Add("Sello digital del emisor: ");
            lines.Add(factura.TimbreFiscalDigital.SelloCFD);

            lines.Add(" ");


            lines.Add("Sello digital del SAT: ");
            lines.Add(factura.TimbreFiscalDigital.SelloSAT);

            lines.Add(" ");


            lines.Add("Cadena original del complemento de certificacion digital del sat: ");
            lines.Add(factura.CadenaOriginal);


            lines.Add(" ");

            lines.Add(" ");

            return lines;


        }

        private bool existsInvoice(int SaleId)
        {
            return db.facturas_so_sale.Where(f => f.saleId == SaleId).Count() > 0;

        }

        private List<InvoiceDto> getInvoices(int SaleId)
        {
            List<InvoiceDto> Invoices = new List<InvoiceDto>();


            var nativeInvoices = db.facturas_so_sale.Where(r => r.saleId == SaleId)
                       .Select(r => r.factura1)
                       .ToList();


            var billingDatas = db.so_billing_data.ToList();

            foreach (var ninvoice in nativeInvoices)
            {
                string name = billingDatas.Where(b => b.code.Equals(ninvoice.cia.ToString())).Select(x => x.name).FirstOrDefault();

                InvoiceDto dto = new InvoiceDto()
                {
                    InvoiceId = ninvoice.foliointerno,
                    Cia = name,
                    Folio = ninvoice.serie + "-" + ninvoice.factura1,
                    Total = Double.Parse(ninvoice.total.ToString())
                };

                Invoices.Add(dto);
            }

            return Invoices;

        }

        [Obsolete]
        public List<InvoiceDto> createInvoice(int saleId)
        {
            List<InvoiceDto> Invoices = null;

            if (!CustomerBillingDataAvailabe(saleId))
                throw new InvoiceException("Datos de facturacion no disponible");

            //if (CustomerInvoicedInventory(saleId))
            //    throw new InvoiceException("No se permite facturar al mismo cliente en el mismo viaje");

            if (!existsInvoice(saleId))
            {
                var saleIdParameter = new SqlParameter("@saleId", saleId);


                var result = db.Database
                    .SqlQuery<InvoiceResult>("sp_facturasestandar_fe @saleId", saleIdParameter)
                    .ToList();

                if (result.Any())
                {
                    var data = result.FirstOrDefault();

                    if (data.mensaje_tipo == 1)
                    {
                        Invoices = getInvoices(saleId);

                    }
                    else
                    {
                        throw new InvoiceException();
                    }

                }
            }
            else
                Invoices = getInvoices(saleId);

            return Invoices;

        }

        public InvoiceOpeFacturaDto createOpeFacturaDto(int saleId, InvoiceDataDto invoiceData)
        {
            var sale = db.so_sale.Where(s => s.saleId == saleId).FirstOrDefault();

            return createOpeFacturaDto(sale, invoiceData);
        }

        public InvoiceOpeFacturaDto createOpeFacturaDtoV2(int saleId, InvoiceDataDto invoiceData)
        {
            var sale = db.so_sale.Where(s => s.saleId == saleId).FirstOrDefault();
            OpeFacturaService service = new OpeFacturaService();
            return service.createDto(sale, invoiceData);
        }

        public InvoiceOpeFacturaDto createOpeFacturaDto(so_sale so_sale, so_customer_data so_customer_data, InvoiceDataDto invoiceData)
        {
            InvoiceOpeFacturaDto InvoiceOpeFacturaDto = new InvoiceOpeFacturaDto();

            InvoiceOpeFacturaDto.header = createHeader(so_sale, invoiceData);
            Dictionary<Int32, InvoiceLineOpeFacturaDto> lines = new Dictionary<Int32, InvoiceLineOpeFacturaDto>();

            int codigo_ope = 0;
            if (invoiceData != null)
                codigo_ope = Convert.ToInt32(invoiceData.Codope);

            int typePay = 1;
            if(invoiceData != null)
            {
                typePay = invoiceData.CreditApply ? 2 : 1;
            }
            else
            {
                typePay = (bool)so_customer_data.credit_apply ? 2 : 1;
            }

            foreach (so_sale_detail saleDetail in so_sale.so_sale_detail)
            {
                lines.Add(saleDetail.productId, createLine(saleDetail, so_sale, typePay));
            }

            List<so_sale_promotion> promotions = so_sale.so_sale_promotion.Where(sp => sp.so_promotion.type == 3).ToList();

            foreach (so_sale_promotion salePromotion in promotions)
            {
                foreach (so_sale_promotion_detail saleDetailPromotion in salePromotion.so_sale_promotion_detail)
                {
                    if (lines.ContainsKey(saleDetailPromotion.productId))
                    {
                        InvoiceLineOpeFacturaDto line = lines[saleDetailPromotion.productId];
                        line.cantidad += saleDetailPromotion.amount;
                        line.tipo_promocion = Convert.ToInt32(saleDetailPromotion.so_sale_promotion.so_promotion.code);
                        line.cantidad_promocion = saleDetailPromotion.amount;
                        
                        line.iva_total += Decimal.Round((decimal)saleDetailPromotion.vat_total, 2);

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
                        lines.Add(saleDetailPromotion.productId, (createLine(saleDetailPromotion, typePay, so_sale)));
                    }

                }

            }

            var products = so_sale.so_sale_detail.Select(sd => sd.productId).ToList();

            List<so_delivery_detail> detailsNoSaled = db.so_delivery_detail.Where(dd => !products.Contains(dd.productId)
                && dd.so_delivery.deliveryId == so_sale.deliveryId).ToList();

            var masterPrice = db.so_products_price_list.Where(ppl => ppl.is_master && ppl.status).FirstOrDefault();

            so_products_price_list price_list = db.so_products_price_list.
                                            Join(db.so_customer_products_price_list,
                                                PL => PL.products_price_listId,
                                                CPL => CPL.products_price_listId,
                                                (PL, CPL) => new { PL, CPL }).
                                            Where(r => r.PL.branchId == so_sale.so_user.branchId && r.PL.status && r.CPL.customerId == so_sale.customerId && r.CPL.status).
                                            Select(x => x.PL).FirstOrDefault();


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
                        InvoiceLineOpeFacturaDto line = lines[detail.productId];
                        line.devolucion = detail.amount;
                    }
                    else
                    {
                        lines.Add(detail.productId, createDevolutionLine(detail, typePay, so_sale.so_user.so_branch, masterPrice, price_list));
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
                                InvoiceLineOpeFacturaDto line = lines[deliveryPromotionDetail.productId];
                                line.devolucion = deliveryPromotionDetail.amount;
                            }
                            else
                            {
                                lines.Add(deliveryPromotionDetail.productId, createDevolutionLine(deliveryPromotionDetail, typePay, 
                                    so_sale.so_user.so_branch, masterPrice, price_list));
                            }
                        }
                    }


                }

            InvoiceOpeFacturaDto.lines.AddRange(lines.Values);
            return InvoiceOpeFacturaDto;
        }

        /**
         * Crea una factura apartir de los nuevos campos de la venta
         */ 
        public InvoiceOpeFacturaDto createOpeFacturaDto(so_sale so_sale, InvoiceDataDto invoiceData)
        {
            InvoiceOpeFacturaDto InvoiceOpeFacturaDto = new InvoiceOpeFacturaDto();

            InvoiceOpeFacturaDto.header = createHeader(so_sale, invoiceData);
            Dictionary<Int32, InvoiceLineOpeFacturaDto> lines = new Dictionary<Int32, InvoiceLineOpeFacturaDto>();

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
                        InvoiceLineOpeFacturaDto line = lines[saleDetailPromotion.productId];
                        line.cantidad += saleDetailPromotion.amount;
                        line.tipo_promocion = Convert.ToInt32(saleDetailPromotion.so_sale_promotion.so_promotion.code);
                        line.cantidad_promocion = saleDetailPromotion.amount;

                        line.iva_total += Decimal.Round((decimal)saleDetailPromotion.vat_tax * saleDetailPromotion.amount, 2);

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
                        InvoiceLineOpeFacturaDto line = lines[detail.productId];
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
                                InvoiceLineOpeFacturaDto line = lines[deliveryPromotionDetail.productId];
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



        public InvoiceOpeFacturaDto getInvoiceOpeFacturaDto(int saleId, InvoiceDataDto invoiceData)
        {
            var so_sale = db.so_sale.Where(s => s.saleId == saleId).FirstOrDefault();

            if (so_sale == null)
                throw new SaleNotFoundException();

            var so_customer_data = db.so_customer_data.Where(c => c.customerId == so_sale.so_customer.customerId).FirstOrDefault();

            if (so_customer_data == null)
                throw new InvoiceException("Datos de facturacion no disponible");

            InvoiceOpeFacturaDto InvoiceOpeFacturaDto = createOpeFacturaDto(so_sale, so_customer_data, invoiceData);

            return InvoiceOpeFacturaDto;
        }

        public InvoiceOpeFacturaDto getInvoiceOpeFacturaDtoNew(int saleId, InvoiceDataDto invoiceData)
        {
            var so_sale = db.so_sale.Where(s => s.saleId == saleId).FirstOrDefault();

            if (so_sale == null)
                throw new SaleNotFoundException();
            
            InvoiceOpeFacturaDto InvoiceOpeFacturaDto = createOpeFacturaDto(so_sale, invoiceData);

            return InvoiceOpeFacturaDto;
        }

        public List<InvoiceDto> createInvoiceInOpeFactura(int saleId, InvoiceDataDto invoiceData)
        {


            var so_sale = db.so_sale.Where(s => s.saleId == saleId).FirstOrDefault();

            if (so_sale == null)
                throw new SaleNotFoundException();

            int routeCode = Convert.ToInt32(so_sale.so_user.code);
            int branchCode = Convert.ToInt32(so_sale.so_user.so_branch.code);

            var so_customer_data = db.so_customer_data.Where(c => c.customerId == so_sale.so_customer.customerId && c.status
                && c.route_code == routeCode && c.branch_code == branchCode).FirstOrDefault();

            if (so_customer_data == null && invoiceData == null)
                throw new InvoiceException("Datos de facturacion no disponible");

            InvoiceOpeFacturaDto InvoiceOpeFacturaDto = createOpeFacturaDto(so_sale, so_customer_data, invoiceData);




            string URL = ConfigurationManager.AppSettings["WS_INVOICE"];


            List<InvoiceOpeFacturaResponse> invoicesOpeFactura = HttpUtil.CreateInvoicesInOpeFactura(URL, InvoiceOpeFacturaDto);

            if (invoicesOpeFactura == null)
                invoicesOpeFactura = new List<InvoiceOpeFacturaResponse>();

            List<InvoiceDto> invoicesDto = new List<InvoiceDto>();

            var billingsData = db.so_billing_data.ToList();

            foreach (InvoiceOpeFacturaResponse response in invoicesOpeFactura)
            {


                InvoiceDto dto = new InvoiceDto();

                dto.Folio = response.serie + response.folio;
                dto.Cia = response.cia_nombre;
                dto.Total = response.total;
                dto.InvoiceId = response.folio_interno;

                invoicesDto.Add(dto);

                var existsInvoice = db.so_invoice_opefactura.Where(iop => iop.folio_interno == response.folio_interno).Count() > 0;

                if (!existsInvoice)
                {

                    string ciaCode = Convert.ToString(response.cia);
                    var billingData = billingsData.Where(bd => bd.code == ciaCode).FirstOrDefault();

                    if (billingData == null)
                        throw new BillingDataException(ciaCode);


                    DateTime time = DateTime.Now;
                    so_invoice_opefactura so_invoice_opefactura = Mapper.Map<so_invoice_opefactura>(response);
                    so_invoice_opefactura.createdby = so_sale.userId;
                    so_invoice_opefactura.createdon = time;
                    so_invoice_opefactura.modifiedon = time;
                    so_invoice_opefactura.modifiedby = so_sale.userId;
                    so_invoice_opefactura.saleId = so_sale.saleId;
                    so_invoice_opefactura.billing_dataId = billingData.billing_dataId;
                    so_invoice_opefactura.status = true;

                    using (var dbContextTransaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            db.so_invoice_opefactura.Add(so_invoice_opefactura);
                            db.SaveChanges();


                            dbContextTransaction.Commit();
                        }
                        catch (Exception e)
                        {
                            dbContextTransaction.Rollback();
                        }

                    }

                }
            }

            return invoicesDto;
        }

        public List<InvoiceDto> createInvoiceInOpeFactura2(int saleId, InvoiceDataDto invoiceData)
        {
           

            var so_sale = db.so_sale.Where(s => s.saleId == saleId).FirstOrDefault();

            if (so_sale == null)
                throw new SaleNotFoundException();

            var config = db.so_branch_config.Where(bc => bc.status 
            && bc.branchId == so_sale.so_user.branchId 
            && bc.key.ToLower() == "invoice".ToLower()).FirstOrDefault();

            if(config != null && config.value.ToLower() == "v2".ToLower())
            {
                return createInvoiceInOpeFacturaV2(saleId, invoiceData);
            }
            else
            {
                InvoiceOpeFacturaDto InvoiceOpeFacturaDto = createOpeFacturaDto(so_sale, invoiceData);

                string URL = ConfigurationManager.AppSettings["WS_INVOICE"];


                List<InvoiceOpeFacturaResponse> invoicesOpeFactura = HttpUtil.CreateInvoicesInOpeFactura(URL, InvoiceOpeFacturaDto);

                if (invoicesOpeFactura == null)
                    invoicesOpeFactura = new List<InvoiceOpeFacturaResponse>();

                List<InvoiceDto> invoicesDto = new List<InvoiceDto>();

                var billingsData = db.so_billing_data.ToList();

                foreach (InvoiceOpeFacturaResponse response in invoicesOpeFactura)
                {


                    InvoiceDto dto = new InvoiceDto();

                    dto.Folio = response.serie + response.folio;
                    dto.Cia = response.cia_nombre;
                    dto.Total = response.total;
                    dto.InvoiceId = response.folio_interno;

                    invoicesDto.Add(dto);

                    var existsInvoice = db.so_invoice_opefactura.Where(iop => iop.folio_interno == response.folio_interno).Count() > 0;

                    if (!existsInvoice)
                    {

                        string ciaCode = Convert.ToString(response.cia);
                        var billingData = billingsData.Where(bd => bd.code == ciaCode).FirstOrDefault();

                        if (billingData == null)
                            throw new BillingDataException(ciaCode);


                        DateTime time = DateTime.Now;
                        so_invoice_opefactura so_invoice_opefactura = Mapper.Map<so_invoice_opefactura>(response);
                        so_invoice_opefactura.createdby = so_sale.userId;
                        so_invoice_opefactura.createdon = time;
                        so_invoice_opefactura.modifiedon = time;
                        so_invoice_opefactura.modifiedby = so_sale.userId;
                        so_invoice_opefactura.saleId = so_sale.saleId;
                        so_invoice_opefactura.billing_dataId = billingData.billing_dataId;
                        so_invoice_opefactura.status = true;

                        using (var dbContextTransaction = db.Database.BeginTransaction())
                        {
                            try
                            {
                                db.so_invoice_opefactura.Add(so_invoice_opefactura);
                                db.SaveChanges();


                                dbContextTransaction.Commit();
                            }
                            catch (Exception e)
                            {
                                dbContextTransaction.Rollback();
                            }

                        }

                    }
                }

                return invoicesDto;
            }

            
        }

        public InvoiceLineOpeFacturaDto createDevolutionLine(so_delivery_promotion_detail deliveryPromotionDetail, int typePay, 
            so_branch branch, so_products_price_list master_price_list, so_products_price_list price_list)
        {
            InvoiceLineOpeFacturaDto line = new InvoiceLineOpeFacturaDto();

           


            var saleService = new SaleService();

            var productTax = deliveryPromotionDetail.so_product.so_product_tax;

            var detail = new so_sale_detail();
            detail.productId = deliveryPromotionDetail.productId;
            detail.amount = 0;

            saleService.SetSaleTax(detail, branch.so_branch_tax, master_price_list, price_list);

            var product = db.so_product_tax.Where(p => p.productId == detail.productId).FirstOrDefault();

            line.codigo_ope = Convert.ToInt32(deliveryPromotionDetail.so_product.code);
            line.tipo_credito = typePay;
            line.unidad_venta = product != null ? product.unit : "";
            line.devolucion = deliveryPromotionDetail.amount;
            line.cantidad = 0;
            line.tipo_promocion = Convert.ToInt32(deliveryPromotionDetail.so_delivery_promotion.so_promotion.code);
            line.precio = Convert.ToSingle(detail.base_price_no_tax);
            line.descuento = Convert.ToSingle(detail.discount_no_tax);
            line.litros_gravables = (decimal)detail.net_content;
            line.porcentaje_iva = Convert.ToSingle(detail.vat_rate);
            line.importe_iva = Decimal.Round((decimal)detail.vat, 2);
            line.iva_total = Decimal.Round((decimal)detail.vat_total, 2);
            line.tasa_ieps = (decimal)detail.stps_rate;
            line.importe_ieps = Decimal.Round((decimal)detail.stps, 2);
            line.importe_ieps_cuota = Decimal.Round((decimal)detail.stps_fee, 2);
            line.tasa_ieps_botana = (decimal)detail.stps_snack_rate;
            line.importe_ieps_botana = Decimal.Round((decimal)detail.stps_snack, 2);





            return line;
        }

        /**
         * Crear una linea de devolucion apartir de los datos de la entrega-promocion
         */
        public InvoiceLineOpeFacturaDto createDevolutionLine(so_delivery_promotion_detail deliveryPromotionDetail, int typePay)
        {
            InvoiceLineOpeFacturaDto line = new InvoiceLineOpeFacturaDto();

            var product = db.so_product_tax.Where(p => p.productId == deliveryPromotionDetail.so_product.productId).FirstOrDefault();

            line.codigo_ope = Convert.ToInt32(deliveryPromotionDetail.so_product.code);
            line.tipo_credito = typePay;
            line.unidad_venta = product != null ? product.unit : "";
            line.devolucion = deliveryPromotionDetail.amount;
            line.cantidad = 0;
            line.tipo_promocion = Convert.ToInt32(deliveryPromotionDetail.so_delivery_promotion.so_promotion.code);
            line.precio = Convert.ToSingle(deliveryPromotionDetail.price_without_taxes ?? 0);
            line.descuento = Convert.ToSingle(deliveryPromotionDetail.discount_without_taxes ?? 0);
            line.litros_gravables = (decimal)(deliveryPromotionDetail.liters ?? 0);
            line.porcentaje_iva = Convert.ToSingle(deliveryPromotionDetail.vat_rate ?? 0);
            line.importe_iva = Decimal.Round((decimal)(deliveryPromotionDetail.vat ?? 0), 2);
            line.iva_total = Decimal.Round((decimal)(deliveryPromotionDetail.vat ?? 0) * deliveryPromotionDetail.amount, 2);
            line.tasa_ieps = (decimal)(deliveryPromotionDetail.ieps_rate ?? 0);
            line.importe_ieps = Decimal.Round((decimal)(deliveryPromotionDetail.ieps ?? 0), 2);
            line.importe_ieps_cuota = Decimal.Round((decimal)(deliveryPromotionDetail.ieps_fee ?? 0), 2);
            line.tasa_ieps_botana = (decimal)(deliveryPromotionDetail.ieps_snack_rate ?? 0);
            line.importe_ieps_botana = Decimal.Round((decimal)(deliveryPromotionDetail.ieps_snack ?? 0), 2);





            return line;
        }


        public InvoiceLineOpeFacturaDto createDevolutionLine(so_delivery_detail deliveryDetail, int typePay, so_branch branch, 
            so_products_price_list master_price_list, so_products_price_list price_list)
        {
            InvoiceLineOpeFacturaDto line = new InvoiceLineOpeFacturaDto();

            var saleService = new SaleService();

            var productTax = deliveryDetail.so_product.so_product_tax;

            var detail = new so_sale_detail();
            detail.productId = deliveryDetail.productId;
            detail.amount = 0;

            saleService.SetSaleTax(detail, branch.so_branch_tax, master_price_list, price_list);

            var product = db.so_product_tax.Where(p => p.productId == detail.productId).FirstOrDefault();



            line.codigo_ope = Convert.ToInt32(deliveryDetail.so_product.code);
            line.tipo_credito = typePay;
            line.unidad_venta = product != null ? product.unit : "";
            line.devolucion = deliveryDetail.amount;
            line.cantidad = 0;
            line.precio = Convert.ToSingle(detail.base_price_no_tax);
            line.descuento = Convert.ToSingle(detail.discount_no_tax);
            line.litros_gravables = (decimal)detail.net_content;
            line.porcentaje_iva = Convert.ToSingle(detail.vat_rate);
            line.importe_iva = Decimal.Round((decimal)detail.vat, 2);
            line.iva_total = Decimal.Round((decimal)detail.vat_total, 2);
            line.tasa_ieps = (decimal)detail.stps_rate;
            line.importe_ieps = Decimal.Round((decimal)detail.stps, 2);
            line.importe_ieps_cuota = Decimal.Round((decimal)detail.stps_fee, 2);
            line.tasa_ieps_botana = (decimal)detail.stps_snack_rate;
            line.importe_ieps_botana = Decimal.Round((decimal)detail.stps_snack, 2);

            
            return line;
        }


        public InvoiceLineOpeFacturaDto createDevolutionLine(so_delivery_detail deliveryDetail, int typePay)
        {
            InvoiceLineOpeFacturaDto line = new InvoiceLineOpeFacturaDto();
            
            var product = db.so_product_tax.Where(p => p.productId == deliveryDetail.so_product.productId).FirstOrDefault();
            
            line.codigo_ope = Convert.ToInt32(deliveryDetail.so_product.code);
            line.tipo_credito = typePay;
            line.unidad_venta = product != null ? product.unit : "";
            line.devolucion = deliveryDetail.amount;
            line.cantidad = 0;
            line.precio = Convert.ToSingle(deliveryDetail.price_without_taxes ?? 0);
            line.descuento = Convert.ToSingle(deliveryDetail.discount_without_taxes ?? 0);
            line.litros_gravables = (decimal)(deliveryDetail.liters ?? 0);
            line.porcentaje_iva = Convert.ToSingle(deliveryDetail.vat_rate ?? 0);
            line.importe_iva = Decimal.Round((decimal)(deliveryDetail.vat ?? 0), 2);
            line.iva_total = Decimal.Round((decimal)(deliveryDetail.vat ?? 0) * deliveryDetail.amount, 2);
            line.tasa_ieps = (decimal)(deliveryDetail.ieps_rate ?? 0);
            line.importe_ieps = Decimal.Round((decimal)(deliveryDetail.ieps ?? 0), 2);
            line.importe_ieps_cuota = Decimal.Round((decimal)(deliveryDetail.ieps_fee ?? 0), 2);
            line.tasa_ieps_botana = (decimal)(deliveryDetail.ieps_snack_rate ?? 0);
            line.importe_ieps_botana = Decimal.Round((decimal)(deliveryDetail.ieps_snack ?? 0), 2);


            return line;
        }

        public InvoiceLineOpeFacturaDto createLine(so_sale_detail detail, so_sale sale, int typePay)
        {
            InvoiceLineOpeFacturaDto line = new InvoiceLineOpeFacturaDto();

           

            int amountDevolution = 0;

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
            line.precio = Convert.ToSingle(detail.base_price_no_tax);
            line.descuento = Convert.ToSingle(detail.discount_no_tax);
            line.litros_gravables = (decimal)detail.net_content;
            line.porcentaje_iva = Convert.ToSingle(detail.vat_rate);
            line.importe_iva = Decimal.Round((decimal)detail.vat, 2);
            line.iva_total = Decimal.Round((decimal)detail.vat_total, 2);
            line.tasa_ieps = (decimal)detail.stps_rate;
            line.importe_ieps = Decimal.Round((decimal)detail.stps, 2);
            line.importe_ieps_cuota = Decimal.Round((decimal)detail.stps_fee, 2);
            line.tasa_ieps_botana = (decimal)detail.stps_snack_rate;
            line.importe_ieps_botana = Decimal.Round((decimal)detail.stps_snack, 2);

            return line;
        }

        /**
         * Crea una linea apartir de los nuevos campos
         */ 
        public InvoiceLineOpeFacturaDto createLine(so_sale_detail detail, int typePay)
        {
            InvoiceLineOpeFacturaDto line = new InvoiceLineOpeFacturaDto();

           

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
            line.precio = Convert.ToSingle( detail.price_without_taxes);
            line.descuento = Convert.ToSingle(detail.discount_without_taxes);
            line.litros_gravables = (decimal)detail.liters;
            line.porcentaje_iva = Convert.ToSingle(detail.vat_tax_rate);
            line.importe_iva = Decimal.Round((decimal)detail.vat_tax, 2);
            line.iva_total = Decimal.Round((decimal)detail.vat_tax * detail.amount, 2);
            line.tasa_ieps = (decimal)detail.ieps_rate;
            line.importe_ieps = Decimal.Round((decimal)detail.ieps, 2);
            line.importe_ieps_cuota = Decimal.Round((decimal)detail.ieps_fee, 2);
            line.tasa_ieps_botana = (decimal)detail.ieps_snack_rate;
            line.importe_ieps_botana = Decimal.Round((decimal)detail.ieps_snack, 2);

            return line;
        }

        public InvoiceLineOpeFacturaDto createLine(so_sale_promotion_detail detail, int typePay, so_sale sale)
        {
            InvoiceLineOpeFacturaDto line = new InvoiceLineOpeFacturaDto();

          
            int amountDevolution = 0;

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
            line.precio = Convert.ToSingle(detail.base_price_no_tax);
            line.descuento = Convert.ToSingle(detail.discount_no_tax);
            line.litros_gravables = (decimal)detail.net_content;
            line.porcentaje_iva = Convert.ToSingle(detail.vat_rate);
            line.importe_iva = Decimal.Round((decimal)detail.vat, 2);
            line.iva_total = Decimal.Round((decimal)detail.vat_total, 2);
            line.tasa_ieps = (decimal)detail.stps_rate;
            line.importe_ieps = Decimal.Round((decimal)detail.stps, 2);
            line.importe_ieps_cuota = Decimal.Round((decimal)detail.stps_fee, 2);
            line.tasa_ieps_botana = (decimal)detail.stps_snack_rate;
            line.importe_ieps_botana = Decimal.Round((decimal)detail.stps_snack, 2);
            line.cantidad_promocion = detail.amount;
            line.tipo_promocion = Convert.ToInt32(detail.so_sale_promotion.so_promotion.code);
            return line;
        }

        public InvoiceLineOpeFacturaDto createLine(so_sale_promotion_detail detail, int typePay)
        {
            InvoiceLineOpeFacturaDto line = new InvoiceLineOpeFacturaDto();


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
            line.precio = Convert.ToSingle(detail.price_without_taxes);
            line.descuento = Convert.ToSingle(detail.discount_without_taxes);
            line.litros_gravables = (decimal)detail.liters;
            line.porcentaje_iva = Convert.ToSingle(detail.vat_tax_rate);
            line.importe_iva = Decimal.Round((decimal)detail.vat_tax, 2);
            line.iva_total = Decimal.Round((decimal)detail.vat_tax * detail.amount, 2);
            line.tasa_ieps = (decimal)detail.ieps_rate;
            line.importe_ieps = Decimal.Round((decimal)detail.ieps, 2);
            line.importe_ieps_cuota = Decimal.Round((decimal)detail.ieps_fee, 2);
            line.tasa_ieps_botana = (decimal)detail.ieps_snack_rate;
            line.importe_ieps_botana = Decimal.Round((decimal)detail.ieps_snack, 2);
            line.cantidad_promocion = detail.amount;
            line.tipo_promocion = Convert.ToInt32(detail.so_sale_promotion.so_promotion.code);
            return line;
        }

        public InvoiceHeaderOpeFacturaDto createHeader(so_sale sale, InvoiceDataDto invoiceData)
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

        public string createOpeCdId(so_sale sale)
        {

            string opecdid = "WBC-" + sale.saleId;

            return opecdid;
        }

        private bool CustomerInvoicedInventory(int saleId)
        {
            var currentSale = db.so_sale.Where(s => s.saleId == saleId).FirstOrDefault();
            if (currentSale != null)
            {
                var sales = db.so_sale.Where(s => s.inventoryId == currentSale.inventoryId
                                                && s.customerId == currentSale.customerId
                                                && s.saleId != saleId
                                                && s.facturas_so_sale.Any());
                if (sales.Any())
                    return true;
            }
            return false;
        }

        private bool CustomerBillingDataAvailabe(int saleId)
        {
            bool isAvailable = false;
            var sale = db.so_sale.Where(s => s.saleId == saleId).FirstOrDefault();

            if (sale != null)
            {
                var branch_code = int.Parse(sale.so_user.so_branch.code);
                var route_code = int.Parse(sale.so_user.code);

                var customerData = db.so_customer_data.Where(cd => cd.customerId == sale.customerId && cd.branch_code == branch_code && cd.route_code == route_code && cd.status);
                if (customerData.Any())
                {
                    isAvailable = true;
                }
            }

            return isAvailable;
        }

        public void RegisterCustomerInvoice(int CustomerId, string BranchCode, string Routecode)
        {
            var Customer = db.so_customer.Where(customer => customer.customerId == CustomerId).FirstOrDefault();

            if (Customer == null)
                throw new CustomerNotFoundException(CustomerId);

            int iBRanchCode = Convert.ToInt32(BranchCode);
            int iRouteCode = Convert.ToInt32(Routecode);

            var CustomersData = db.so_customer_data.Where(cdata => cdata.customerId == CustomerId && cdata.branch_code == iBRanchCode && cdata.route_code == iRouteCode
                && cdata.status).ToList();

            if (CustomersData == null || CustomersData.Count == 0)
                throw new CustomerException("No hay información del cliente");

            foreach (var CustomerData in CustomersData)
            {

                if (CustomerData.registered_bebidas && CustomerData.registered_embe)
                    continue;


                var Branch = db.so_branch.Where(b => b.code == CustomerData.branch_code.ToString()).FirstOrDefault();


                if (String.IsNullOrEmpty(CustomerData.state))
                    CustomerData.state = "Sin información";

                string URL = ConfigurationManager.AppSettings["WS_MASTEREDI_REGISTER"];

                if (!CustomerData.registered_bebidas)
                {


                    string User = ConfigurationManager.AppSettings["USER_BEPENSA1"];
                    string Password = ConfigurationManager.AppSettings["PASS_BEPENSA1"];


                    RegisterCustomerDto dto = new RegisterCustomerDto();
                    dto.User = User;
                    dto.Password = Password;

                    RegisterCustomerDataDto dataDto = new RegisterCustomerDataDto();
                    dataDto.Name = Customer.name;


                    InsertCliente c = new InsertCliente();
                    c.Session = new Dictionary<string, object>();
                    c.Session.Add("Customer", Customer);
                    c.Session.Add("CustomerData", CustomerData);
                    c.Session.Add("User", User);
                    c.Session.Add("Password", Password);
                    c.Session.Add("BranchName", Branch.name);
                    c.Session.Add("IsVirtual", "0");
                    c.Session.Add("Observations", "");
                    c.Initialize();

                    string text = c.TransformText();
                    sendPost(URL, text);
                    CustomerData.registered_bebidas = true;

                    using (var dbContextTransaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            db.Entry(CustomerData).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();

                            dbContextTransaction.Commit();
                        }
                        catch (Exception e)
                        {
                            dbContextTransaction.Rollback();
                        }

                    }
                }

                if (!CustomerData.registered_embe)
                {
                    string User = ConfigurationManager.AppSettings["USER_BEPENSA2"];
                    string Password = ConfigurationManager.AppSettings["PASS_BEPENSA2"];

                    RegisterCustomerDto dto = new RegisterCustomerDto();
                    dto.User = User;
                    dto.Password = Password;

                    RegisterCustomerDataDto dataDto = new RegisterCustomerDataDto();
                    dataDto.Name = Customer.name;


                    InsertCliente c = new InsertCliente();
                    c.Session = new Dictionary<string, object>();
                    c.Session.Add("Customer", Customer);
                    c.Session.Add("CustomerData", CustomerData);
                    c.Session.Add("User", User);
                    c.Session.Add("Password", Password);
                    c.Session.Add("BranchName", Branch.name);
                    c.Session.Add("IsVirtual", "0");
                    c.Session.Add("Observations", "");
                    c.Initialize();

                    string text = c.TransformText();
                    sendPost(URL, text);
                    CustomerData.registered_embe = true;

                    using (var dbContextTransaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            db.Entry(CustomerData).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();

                            dbContextTransaction.Commit();
                        }
                        catch (Exception e)
                        {
                            dbContextTransaction.Rollback();
                        }

                    }
                }
            }
        }

        private void sendPost(string URL, string body)
        {
            var uri = new Uri(URL);

            var req = (HttpWebRequest)WebRequest.CreateDefault(uri);
            req.ContentType = "text/xml; charset=utf-8";
            req.Method = "POST";
            req.Accept = "text/xml";
            req.Timeout = 20000;
            req.Headers.Add("SOAPAction", "http://tempuri.org/InsertCliente");

            System.IO.StreamWriter swriter = new System.IO.StreamWriter(req.GetRequestStream());
            swriter.Write(body);
            swriter.Close();

            try
            {
                using (HttpWebResponse response = (HttpWebResponse)req.GetResponse())
                {
                    var statusCode = response.StatusCode;
                    if (statusCode == HttpStatusCode.OK)
                    {
                        using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                        {
                            string resp = reader.ReadToEnd();
                            XmlDocument xmldoc = new XmlDocument();
                            xmldoc.LoadXml(resp);
                            XmlNodeList node = xmldoc.GetElementsByTagName("InsertClienteResult");
                            if (node == null || node.Count == 0)
                            {
                                throw new RegisterCustomerBillingException("No se encontro la respuesta del servidor de registro de cliente. \n " + resp);
                            }

                            XmlNode n = node[0];


                            string responseValue = n.InnerText;

                            if (!responseValue.Equals("correcto", StringComparison.InvariantCultureIgnoreCase))
                            {
                                throw new RegisterCustomerBillingException(responseValue);
                            }

                        }
                    }
                    else
                    {
                        throw new RegisterCustomerBillingException("El servidor de registro de facturación devolvio error " + statusCode);
                    }
                }
            }
            catch (WebException e)
            {
                if (e.Status == WebExceptionStatus.Timeout)
                {
                    throw new RegisterCustomerBillingException("Se agoto el tiempo de espera para el registro del cliente");
                }
                else throw;
            }
        }

        public static string Right(string original, int numberCharacters)
        {
            return original.Substring(numberCharacters > original.Length ? 0 : original.Length - numberCharacters);
        }

        public List<InvoiceDto> createInvoiceInOpeFacturaV2(int saleId, InvoiceDataDto invoiceData)
        {


            var so_sale = db.so_sale.Where(s => s.saleId == saleId).FirstOrDefault();

            if (so_sale == null)
                throw new SaleNotFoundException();

            OpeFacturaService service = new OpeFacturaService();
            InvoiceOpeFacturaDto InvoiceOpeFacturaDto = service.createDto(so_sale, invoiceData);

            string URL = ConfigurationManager.AppSettings["WS_INVOICE"];


            List<InvoiceOpeFacturaResponse> invoicesOpeFactura = HttpUtil.CreateInvoicesInOpeFacturaV2(URL, InvoiceOpeFacturaDto);

            if (invoicesOpeFactura == null)
                invoicesOpeFactura = new List<InvoiceOpeFacturaResponse>();

            List<InvoiceDto> invoicesDto = new List<InvoiceDto>();

            var billingsData = db.so_billing_data.ToList();

            foreach (InvoiceOpeFacturaResponse response in invoicesOpeFactura)
            {


                InvoiceDto dto = new InvoiceDto();

                dto.Folio = response.serie + response.folio;
                dto.Cia = response.cia_nombre;
                dto.Total = response.total;
                dto.InvoiceId = response.folio_interno;

                invoicesDto.Add(dto);

                var existsInvoice = db.so_invoice_opefactura.Where(iop => iop.folio_interno == response.folio_interno).Count() > 0;

                if (!existsInvoice)
                {

                    string ciaCode = Convert.ToString(response.cia);
                    var billingData = billingsData.Where(bd => bd.code == ciaCode).FirstOrDefault();

                    if (billingData == null)
                        throw new BillingDataException(ciaCode);


                    DateTime time = DateTime.Now;
                    so_invoice_opefactura so_invoice_opefactura = Mapper.Map<so_invoice_opefactura>(response);
                    so_invoice_opefactura.createdby = so_sale.userId;
                    so_invoice_opefactura.createdon = time;
                    so_invoice_opefactura.modifiedon = time;
                    so_invoice_opefactura.modifiedby = so_sale.userId;
                    so_invoice_opefactura.saleId = so_sale.saleId;
                    so_invoice_opefactura.billing_dataId = billingData.billing_dataId;
                    so_invoice_opefactura.status = true;

                    using (var dbContextTransaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            db.so_invoice_opefactura.Add(so_invoice_opefactura);
                            db.SaveChanges();


                            dbContextTransaction.Commit();
                        }
                        catch (Exception e)
                        {
                            dbContextTransaction.Rollback();
                        }

                    }

                }
            }

            return invoicesDto;
        }

    }
}