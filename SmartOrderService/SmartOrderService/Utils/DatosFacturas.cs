using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartOrderService.Utils.DB;
using LibreriaCFDI;
using LibreriaCFDI.Clases;
using LibreriaCFDI.Enums;
using SmartOrderService.Utils;

namespace Facturacion
{
    public class DatosFacturas
    {
        private ModelFacturacion db = new ModelFacturacion();

        public Factura Llenarfactura(double folio)
        {
            //Variables predefinidas
            //String FormaDePago = "PAGO EN UNA SOLA EXHIBICIÓN";
            String Version = "3.2";
            String RegimenFiscal = "RÉGIMEN OPCIONAL PARA GRUPOS DE SOCIEDADES";

            //Variables generales
            DateTime Fecha = DateTime.Today;
            String LugarExpedicion = "";
            String MetodoDePago = "";
            String NumeroCuentaPago = "";
            string CondicionDePago = "";
            string[] Emails = { };
            //String MetodoDePagoDefault = "NA";
            //String NumeroCuentaPagoDefaul = "NA";
            

            TipoComprobante TipoComprobante = TipoComprobante.Ingreso;

            var model = new ModelFacturacion();

            var facs = from table in db.facturas where table.foliointerno == folio
                           select table;

            //Timbrado timbrado = new Timbrado();
            Factura factura = new Factura();
            foreach (facturas fac in facs)
            {
                TipoComprobante = (fac.tipodoc == 1 ? TipoComprobante.Ingreso : TipoComprobante.Egreso);
                //var configuration_customer = (from table in db.so_configuration_customer
                //           select table).FirstOrDefault();
                //MetodoDePagoDefault = configuration_customer.payment_method;
                //NumeroCuentaPagoDefaul = configuration_customer.payment_account;

                var cias = from table in db.so_billing_data
                           where table.code == fac.cia.ToString()
                           select table;
                Emisor emisor = new Emisor();

                foreach (so_billing_data cia in cias)
                {
                    #region Emisor
                    emisor.Nombre = cia.name;
                    //emisor.referencia = fac.serie + fac.factura;
                    emisor.RFC = cia.ftr;
                    emisor.RegimenFiscal.Add(RegimenFiscal);
                    emisor.DomicilioFiscal.Calle = cia.address_street;
                    emisor.DomicilioFiscal.CodigoPostal = cia.postal_code;
                    emisor.DomicilioFiscal.Colonia = cia.suburb;
                    emisor.DomicilioFiscal.Estado = cia.state;
                    emisor.DomicilioFiscal.Localidad = "";
                    emisor.DomicilioFiscal.Municipio = cia.town;
                    emisor.DomicilioFiscal.NumeroExterior = cia.address_number;
                    emisor.DomicilioFiscal.NumeroInterior = "";
                    emisor.DomicilioFiscal.Pais = cia.country;
                    emisor.DomicilioFiscal.Referencia = fac.serie + fac.factura;
                    
                }

                var sites = from table in db.so_site
                           where table.code == fac.sitio
                           select table;
                foreach (so_site site in sites)
                {
                    emisor.LugarDeExpedicion.Calle = site.address;
                    emisor.LugarDeExpedicion.CodigoPostal = site.postal_code;
                    emisor.LugarDeExpedicion.Colonia = "";
                    emisor.LugarDeExpedicion.Estado = site.state;
                    emisor.LugarDeExpedicion.Localidad = site.city;
                    emisor.LugarDeExpedicion.Municipio = site.town;
                    emisor.LugarDeExpedicion.NumeroInterior = "";
                    emisor.LugarDeExpedicion.NumeroExterior = "";
                    emisor.LugarDeExpedicion.Pais = site.country;
                    emisor.LugarDeExpedicion.Referencia = "";
                    emisor.LugarDeExpedicion.NombreSucursal = site.name;
                    LugarExpedicion = site.town.Trim() + ", " + site.state.Trim();
                }
                #endregion

                #region Receptor
                Receptor receptor = new Receptor();

                var customers = from table in db.so_customer
                            where table.code == fac.cliente
                            select table;
                receptor.Nombre = fac.razonsocial;
                receptor.RFC = fac.rfc;
                receptor.Domicilio.Calle = fac.direccioncliente;
                var sale = fac.facturas_so_sale.FirstOrDefault().so_sale;
                
                var branch = int.Parse(sale.so_user.so_branch.code);
                var route = int.Parse(sale.so_user.code);

                foreach (so_customer customer in customers)
                {
                    var cds = customer.so_customer_data;
                    var customer_data = cds.FirstOrDefault(cd => cd.branch_code == branch && cd.route_code == route);

                    receptor.Domicilio.CodigoPostal = customer_data.postal_code;
                    receptor.Domicilio.Colonia = "";// customer_data.suburb;
                    receptor.Domicilio.Estado = "";// customer_data.state;
                    receptor.Domicilio.Localidad = "";
                    receptor.Domicilio.Municipio = "";//customer_data.town;
                    receptor.Domicilio.NumeroInterior = customer_data.address_number_int;
                    receptor.Domicilio.NumeroExterior = customer_data.address_number;
                    receptor.Domicilio.Pais = customer_data.country;
                    receptor.Domicilio.Referencia = "";
                    receptor.NombreComercial = customer_data.trade_name;
                    MetodoDePago = ((customer_data.payment_method == null || customer_data.payment_method == String.Empty) ? "01" : customer_data.payment_method.Trim());
                    NumeroCuentaPago = ((customer_data.account_ended == null || customer_data.account_ended == String.Empty) ? "" : customer_data.account_ended.Trim());
                    CondicionDePago = customer_data.payment_condition;
                    Emails = customer.email.Trim().Split(new Char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                }
                #endregion

                #region Concepto
                var lineas = fac.factdet;
                List<Concepto> conceptos = new List<Concepto>();
                decimal IvaTot = 0;
                decimal IepsTot = 0;
                decimal IepscuotaTot = 0;
                decimal IepsbotanaTot = 0;
                decimal SubTotal = 0;
                decimal Total = 0;
                decimal TasaIva = 0;
                decimal TasaIeps = 0;
                decimal TasaIepsCuota = 0;
                decimal TasaIepsBotana = 0;

                foreach (factdet linea in lineas)
                {
                    decimal ValorUnitario = 0;
                    decimal Importe = 0;
                    decimal precio = 0;
                    decimal descuento = 0;
                    decimal cantidad = 0;
                    decimal ivaimporte = 0;
                    decimal ivatotal = 0;
                    decimal iepsimporte = 0;
                    decimal iepscuota = 0;
                    decimal iepsbotanaimporte = 0;

                    //Redondear valores unitarios
                    precio = Math.Round(linea.precio,2);
                    descuento = Math.Round(linea.descuento,2);
                    ivaimporte = Math.Round(linea.ivaimporte, 2);
                    ivatotal = Math.Round(linea.ivatotal, 2);
                    iepsimporte = Math.Round(linea.iepsimporte, 2);
                    iepscuota = Math.Round(linea.iepscuota, 2);
                    iepsbotanaimporte = Math.Round(linea.iepsbotanaimporte, 2);

                    cantidad = linea.cantidad;

                    ValorUnitario = (precio);

                    if (fac.imprimeiva == false)
                    {
                        ValorUnitario = ValorUnitario + ivaimporte;
                    }
                    else
                    {                        
                        IvaTot = IvaTot + ivatotal;
                    }

                    if (fac.imprimeieps == false)
                    {
                        ValorUnitario = ValorUnitario + iepsimporte;
                    }
                    else
                    {
                        IepsTot = IepsTot + (iepsimporte * cantidad);
                    }

                    if (fac.imprimeiepscuota == false)
                    {
                        ValorUnitario = ValorUnitario + iepscuota;
                    }
                    else
                    {
                        IepscuotaTot = IepscuotaTot + (iepscuota * cantidad);
                    }

                    if (fac.imprimeiepsbotana == false)
                    {
                        ValorUnitario = ValorUnitario + iepsbotanaimporte;
                    }
                    else
                    {
                        IepsbotanaTot = IepsbotanaTot + (iepsbotanaimporte * cantidad);
                    }

                    Importe = (ValorUnitario - descuento) * linea.cantidad;
                    SubTotal = SubTotal + Importe;

                    if (linea.ivaimporte > 0 && TasaIva < linea.tasaiva)
                        TasaIva = linea.tasaiva;

                    if (linea.iepsimporte > 0 && TasaIeps < linea.tasaieps)
                        TasaIeps = linea.tasaieps;

                    //if (linea.iepscuota > 0 && TasaIepsCuota < linea.iepscuota)
                    //    TasaIepsCuota = linea.iepscuota;

                    if (linea.iepsbotanaimporte > 0 && TasaIepsBotana < linea.iepsbotana)
                        TasaIepsBotana = linea.iepsbotana;

                    Concepto concepto = new Concepto();
                    concepto.Cantidad = Math.Round(linea.cantidad,2);
                    concepto.Descripcion = linea.descriparticulo;
                    concepto.Importe = Math.Round(Importe,2);
                    concepto.NumeroIdentificacion = linea.codeupc;
                    concepto.Unidad = linea.unidad;
                    concepto.ValorUnitario = Math.Round(ValorUnitario,2);
                    concepto.Codigo = linea.codope;
                    concepto.TasaIVA = linea.tasaiva; ;
                    concepto.Descuento = linea.descuento;
                    conceptos.Add(concepto);
                }

                //Redondear valores totales
                SubTotal = Math.Round(SubTotal, 2);
                IvaTot = Math.Round(IvaTot, 2);
                IepsTot = Math.Round(IepsTot, 2);
                IepscuotaTot = Math.Round(IepscuotaTot, 2);
                IepsbotanaTot = Math.Round(IepsbotanaTot, 2);

                Total = SubTotal + IvaTot + IepsTot + IepscuotaTot + IepsbotanaTot;

                #endregion

                #region Impuestos
                Impuestos impuestos = new Impuestos();
                impuestos.IncluyeImpuestosRetenidos = false;
                impuestos.IncluyeImpuestosTraslado = true;//(IvaTot > 0 || IepsTot > 0 || IepscuotaTot > 0 || IepsbotanaTot > 0);

                //if (IvaTot > 0)
                impuestos.Traslados.Add(new Traslado { Importe = IvaTot, Tasa = TasaIva, Impuesto = ImpuestosTraslado.IVA });

                if (IepsTot>0)
                    impuestos.Traslados.Add(new Traslado { Importe = IepsTot, Tasa = TasaIeps, Impuesto = ImpuestosTraslado.IEPS });

                if (IepscuotaTot>0)
                    impuestos.Traslados.Add(new Traslado { Importe = IepscuotaTot, Tasa = TasaIepsCuota, Impuesto = ImpuestosTraslado.IEPS });

                if (IepsbotanaTot>0)
                    impuestos.Traslados.Add(new Traslado { Importe = IepsbotanaTot, Tasa = TasaIepsBotana, Impuesto = ImpuestosTraslado.IEPS });

                impuestos.TotalImpuestosRetenidos = 0;
                impuestos.TotalImpuestosTraslado = IvaTot + IepsTot + IepscuotaTot + IepsbotanaTot;

                #endregion

                #region Factura
                //Factura factura = new Factura();
                factura.Certificado = "";
                factura.CondicionesDePago = CondicionDePago;
                factura.Descuento = 0;
                factura.IncluyeDescuento = true;
             /*   if (DateTime.Parse(fac.fechafac.ToString("yyyy-MM-dd")) < DateTime.Parse(fac.fechacap.ToString("yyyy-MM-dd")))
                {
                    Fecha = DateTime.Parse(fac.fechafac.ToString("yyyy-MM-dd") + " 23:59");
                }
                else
                {
                    Fecha = fac.fechacap;
                }*/
                factura.Fecha = fac.fechacap.ToString("yyyy-MM-ddTHH:mm:ss");
                factura.FechaFolioFiscalOriginal = default(DateTime);
                factura.Folio = fac.factura.ToString();
                factura.FolioFiscalOriginal = "";
                factura.FormaDePago = fac.formapago == 0 ? "Contado" : "Crédito";// FormaDePago;
                factura.IncluyeMontonFolioFiscalOriginal = false;
                factura.IncluyeMontonFolioFiscalOriginal = false;
                factura.IncluyeFechaFolioFiscalOriginal = false;
                factura.LugarExpedicion = LugarExpedicion;
                
                factura.Moneda = fac.moneda;
                factura.MotivoDescuento = "";
                factura.NoCertificado = "";

                /*if (MetodoDePago.Trim().ToUpper().Length > 0)
                {
                    if (MetodoDePago.Trim().ToUpper() == "EFECTIVO")
                    {
                        MetodoDePago = MetodoDePago.Trim();
                        NumeroCuentaPago = NumeroCuentaPagoDefaul;
                    }
                    else
                    {
                        if (NumeroCuentaPago.Trim().Length == 4)
                        {
                            MetodoDePago = MetodoDePago.Trim();
                            NumeroCuentaPago = NumeroCuentaPago.Trim();
                        }
                        else
                        {
                            MetodoDePago = MetodoDePagoDefault;
                            NumeroCuentaPago = NumeroCuentaPagoDefaul;
                        }
                    }
                }
                else
                {
                    MetodoDePago = MetodoDePagoDefault;
                    NumeroCuentaPago = NumeroCuentaPagoDefaul;
                }*/

                factura.MetodoDePago = MetodoDePago;
                factura.NumeroCuentaPago = NumeroCuentaPago;

                factura.SerieFolioFiscalOriginal = "";
                factura.Sello = "";
                factura.Sello = "[Addenda][DIREMI " + emisor.DomicilioFiscal.Calle + " " + emisor.DomicilioFiscal.NumeroExterior + " " + emisor.DomicilioFiscal.Colonia;
                factura.Sello += " C.P. " + emisor.DomicilioFiscal.CodigoPostal + " " + emisor.DomicilioFiscal.Municipio + " " + emisor.DomicilioFiscal.Estado;
                factura.Sello += "][NOMEMI " + emisor.Nombre + "][NUMEOC " + fac.notaventa + "-1][NUMCLI ";
                factura.Sello += fac.cliente + "][NOMCLI " + receptor.NombreComercial + " (" + fac.ruta + ")]";
                factura.Sello += "[NOTAS3 " + NumberUtil.enletras(fac.total.ToString()) + " M.N.]";

                if (Emails.Any())
                {
                    factura.Sello += "[EMAIL ";
                    for (int i = 0; i < Emails.Count(); i++)
                    {
                        if (i != 0)
                            factura.Sello += "; ";
                        factura.Sello += Emails[i].Trim();
                    }
                    factura.Sello += "]";
                }

                factura.Sello += "[DIRREC " + receptor.Domicilio.Calle + "]";
                factura.Sello += "[DIREXP " + emisor.LugarDeExpedicion.NombreSucursal + ":" + emisor.LugarDeExpedicion.Calle + "]";

                for (int i = 0; i < conceptos.Count(); i++)
                    factura.Sello += "[ESTILV_D" + (i + 1) + " " + conceptos[i].Codigo + "]";

                for (int i = 0; i < conceptos.Count(); i++)
                    factura.Sello += "[TASIPE_D" + (i + 1) + " " + conceptos[i].TasaIVA + "]";

                for (int i = 0; i < conceptos.Count(); i++)
                    factura.Sello += "[MDECON_D" + (i + 1) + " " + conceptos[i].Descuento + "]";
                
                factura.Serie = fac.serie;
                factura.SubTotal = SubTotal;
                factura.TipoCambio = fac.tipodecambio.ToString();
                factura.TipoComprobante = TipoComprobante;
                factura.Total = Total;
                factura.Version = Version;

                //Este uso es solo para uso de pruebas, se puede implementar directo sobre el objeto factura.X el llenado de la //información.
                factura.Emisor = emisor;
                factura.Receptor = receptor;
                factura.Impuestos = impuestos;
                factura.Conceptos = conceptos;
                #endregion
            }
            return factura;
        }

    }
}
