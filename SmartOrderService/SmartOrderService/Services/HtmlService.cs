using LibreriaCFDI.Clases;
using SmartOrderService.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Services
{
    public class HtmlService
    {

        public  const int IMAGE=1;
        public  const int PDF=2;
        public  const int HTML=3;

        public String createHtml(int outputFormat,Factura factura,string pathQR) {

            string Html = null;

            switch (outputFormat)
            {

                case IMAGE:
                    Html = createHtmlForImage(factura,pathQR);
                    break;

                case PDF:

                    break;

                case HTML:

                    break;
            }

            return Html;
        }

        private string getHeaderForImage() {
            string head = "<head>" +
            "<link rel = \"Stylesheet\" href = \"StyleSheet\" />" +
            "<meta http-equiv = \"Content-Type\" content = \"text/html; charset=UTF-8\" >" +
            "</head>";

            return head;
        }

        private string getProductsForImage(Factura factura) {
            string tableConceptos =
           "<h3>Productos</h3>\n" +

           "<table width=\"380px\" cellspacing=\"5\">\n" +

           "<tr>\n" +
           "<td> Precio Unitario</td>\n" +
           "<td>Cantidad</td>\n" +
           "<td>Importe</td>\n" +
           "</tr>\n";

            var conceptos = factura.Conceptos;

            foreach (var concepto in conceptos) {
                tableConceptos +=
                "<tr>" +
                "<td colspan = \"3\" align= \"left\" valign= \"middle\" >\n" +
                "<font size = 6> "+concepto.Descripcion+ "</font>\n" +
                "</td>\n" +
                "</tr>\n" +
                "<tr>\n" +
                "<td>$ "+concepto.ValorUnitario+ "</td>\n" +
                "<td>"+concepto.Cantidad+ "</td>\n" +
                "<td>$ "+concepto.Importe+ "</td>\n" +
                "</tr>\n";
                }


            tableConceptos += "<tr>"+
                            "<td colspan = \"2\", valign = \"middle\" ><font size = 6> SubTotal </font></td>"+
                            "<td >$ "+factura.SubTotal+" </td ></tr>\n"+
         
                            "<tr>"+
                            "<td colspan = \"2\",valign = \"middle\" ><font size = 6> IVA 16 %</font></td>"+
                            "<td>$ "+factura.Impuestos.TotalImpuestosTraslado+" </td></tr>\n"+
                  

                            "<tr>"+
                            "<td colspan = \"2\",valign = \"middle\" ><font size = 6> Total </font></td>"+
                            "<td>$ "+factura.Total+" </td></tr>\n"+

                            "<tr>" +
                            "<td colspan = \"2\",valign = \"middle\" ><font size = 6>  Con Letra: "+ NumberUtil.enletras(factura.Total.ToString()) 
                            +" "+  
                            factura.Moneda+ ". </font></td></tr>\n" +


            "</table>\n" +
            "</br>\n";

            return tableConceptos;
        }

        private string getStringSplited(string chain) {
            string value = "";
            int maxLnegth = 40;
            int length = chain.Length;

            while (maxLnegth < length) {
                value += chain.Substring(0, maxLnegth)+"</br>\n";
                chain = chain.Substring(maxLnegth + 1);
                length = chain.Length; 
            }
            
            value += chain;

            return value;
        }

        private string getFooterForImage(Factura factura, string pathQR) {
            string selloEmisor = getStringSplited( factura.TimbreFiscalDigital.SelloCFD);
            string selloSat = getStringSplited( factura.TimbreFiscalDigital.SelloSAT);
            string cadenaComplemento = getStringSplited(factura.CadenaOriginal);


            string footer = "<table width=\"380px\" style=\"font-weight: bold;\" >" +
                "<tr><td><h4>Sello del Emisor</h4></td></tr>" +
                "<tr><td>" + selloEmisor+ " </td></tr>" +
                "<tr><td><h4> Sello digital del SAT</h4></td></tr>" +
                "<tr><td> " + selloSat + "</td></tr>" +
                "<tr><td><h4>Cadena original de certificación del sat</h4></td></tr>" +
                "<tr><td> " + cadenaComplemento + "</td></tr>" 
                + "</table>"+
          "\n</div>\n"+
          "<img src=\""+pathQR+ "\" width = \"380px\" height=\"380px\">\n" +
          "</br>"+
          "<p>Este documento es una representación impresa de un CFDI</p>"; 
            
            
              

            return footer;
        }

        private string getBodyForImage(Factura factura) {
            string body = "<body width=\"380px\" style=\"font-weight: bold;\">\n" +
                "<h1> Factura </h1>\n" +
                "<h2> " + factura.Emisor.Nombre + "</h2>\n";

            string fiscalData = "<table width=\"380px\" cellspacing=\"5\">" +
                "<tr><td>RFC</td><td>  " + factura.Emisor.RFC + "</td></tr>"+
                "<tr><td>Régimen Fiscal</td><td> "  + factura.Emisor.RegimenFiscal.FirstOrDefault() + " </td></tr>"+
                "<tr><td>Factura</td><td> " + factura.Serie + "-" + factura.Folio + "</td></tr>" +
                "<tr><td>Lugar</td><td> " + factura.LugarExpedicion + "</td></tr>"+
                "<tr><td>Fecha y hora de Certificación</td><td> " + factura.TimbreFiscalDigital.FechaTimbrado + "</td></tr>"+
                "<tr><td>NoCertificado Emisor</td><td> " + factura.NoCertificado + "</td></tr>" +
                "<tr><td>NoCertificado SAT</td><td> " + factura.TimbreFiscalDigital.NoCertificadoSAT + "</td></tr>"+
                "<tr><td>Cliente</td><td> " + factura.Receptor.Nombre + "</td></tr>"+
                "<tr><td>RFC Cliente</td><td> " + factura.Receptor.RFC + "</td></tr>"+
                "<tr><td>Domicilio</td><td> " + factura.Receptor.Domicilio.Calle + "</td></tr>"+
                "<tr><td>Forma de pago</td><td> " + factura.FormaDePago + "</td></tr>"+
                "<tr><td>Método de pago</td><td> " + factura.MetodoDePago + "</td></tr>"
                + "</table>";

            body += fiscalData + getProductsForImage(factura)+ "</body>\n";          

            return body;
        }
        public string createHtmlForImage(Factura factura,string pathQR) {

            string Html =

            "<html >" +
            getHeaderForImage() +
            getBodyForImage(factura)+
            getFooterForImage(factura, pathQR)+
            "</html>";

            return Html;
        }


    }
}