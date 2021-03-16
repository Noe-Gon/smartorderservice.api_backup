namespace SmartOrderService.Utils.DB
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class ModelFacturacion : DbContext
    {
        public ModelFacturacion()
            : base("name=SmartOrderModel")
        {
        }

        public virtual DbSet<factdet> factdet { get; set; }
        public virtual DbSet<facturas> facturas { get; set; }
        public virtual DbSet<facturas_so_sale> facturas_so_sale { get; set; }
        public virtual DbSet<so_billing_data> so_billing_data { get; set; }
        public virtual DbSet<so_branch> so_branch { get; set; }
        public virtual DbSet<so_configuration_customer> so_configuration_customer { get; set; }
        public virtual DbSet<so_customer> so_customer { get; set; }
        public virtual DbSet<so_customer_data> so_customer_data { get; set; }
        public virtual DbSet<so_product> so_product { get; set; }
        public virtual DbSet<so_product_tax> so_product_tax { get; set; }
        public virtual DbSet<so_sale> so_sale { get; set; }
        public virtual DbSet<so_sale_detail> so_sale_detail { get; set; }
        public virtual DbSet<so_site> so_site { get; set; }
        public virtual DbSet<so_user> so_user { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<factdet>()
                .Property(e => e.sitio)
                .IsUnicode(false);

            modelBuilder.Entity<factdet>()
                .Property(e => e.articulo)
                .IsUnicode(false);

            modelBuilder.Entity<factdet>()
                .Property(e => e.codeupc)
                .IsUnicode(false);

            modelBuilder.Entity<factdet>()
                .Property(e => e.codope)
                .IsUnicode(false);

            modelBuilder.Entity<factdet>()
                .Property(e => e.tasaiva)
                .HasPrecision(14, 8);

            modelBuilder.Entity<factdet>()
                .Property(e => e.ivaimporte)
                .HasPrecision(28, 12);

            modelBuilder.Entity<factdet>()
                .Property(e => e.tasaieps)
                .HasPrecision(14, 8);

            modelBuilder.Entity<factdet>()
                .Property(e => e.iepsimporte)
                .HasPrecision(28, 12);

            modelBuilder.Entity<factdet>()
                .Property(e => e.iepscuota)
                .HasPrecision(28, 12);

            modelBuilder.Entity<factdet>()
                .Property(e => e.iepsbotana)
                .HasPrecision(14, 8);

            modelBuilder.Entity<factdet>()
                .Property(e => e.iepsbotanaimporte)
                .HasPrecision(28, 12);

            modelBuilder.Entity<factdet>()
                .Property(e => e.litros)
                .HasPrecision(28, 12);

            modelBuilder.Entity<factdet>()
                .Property(e => e.cantidad)
                .HasPrecision(25, 8);

            modelBuilder.Entity<factdet>()
                .Property(e => e.precio)
                .HasPrecision(25, 8);

            modelBuilder.Entity<factdet>()
                .Property(e => e.descuento)
                .HasPrecision(25, 8);

            modelBuilder.Entity<factdet>()
                .Property(e => e.ivatotal)
                .HasPrecision(28, 12);

            modelBuilder.Entity<facturas>()
                .Property(e => e.serie)
                .IsUnicode(false);

            modelBuilder.Entity<facturas>()
                .Property(e => e.cliente)
                .IsUnicode(false);

            modelBuilder.Entity<facturas>()
                .Property(e => e.ruta)
                .IsUnicode(false);

            modelBuilder.Entity<facturas>()
                .Property(e => e.facturaref)
                .IsUnicode(false);

            modelBuilder.Entity<facturas>()
                .Property(e => e.rfc)
                .IsUnicode(false);

            modelBuilder.Entity<facturas>()
                .Property(e => e.total)
                .HasPrecision(25, 8);

            modelBuilder.Entity<facturas>()
                .Property(e => e.importeiva)
                .HasPrecision(25, 8);

            modelBuilder.Entity<facturas>()
                .Property(e => e.importeiesp)
                .HasPrecision(25, 8);

            modelBuilder.Entity<facturas>()
                .Property(e => e.importeiespcuota)
                .HasPrecision(25, 8);

            modelBuilder.Entity<facturas>()
                .Property(e => e.importeiespbotana)
                .HasPrecision(25, 8);

            modelBuilder.Entity<facturas>()
                .Property(e => e.sitio)
                .IsUnicode(false);

            modelBuilder.Entity<facturas>()
                .Property(e => e.observ)
                .IsUnicode(false);

            modelBuilder.Entity<facturas>()
                .Property(e => e.razonsocial)
                .IsUnicode(false);

            modelBuilder.Entity<facturas>()
                .Property(e => e.direccioncliente)
                .IsUnicode(false);

            modelBuilder.Entity<facturas>()
                .Property(e => e.tipodecambio)
                .HasPrecision(32, 16);

            modelBuilder.Entity<facturas>()
                .HasMany(e => e.factdet)
                .WithRequired(e => e.facturas)
                .HasForeignKey(e => e.folio)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<facturas>()
                .HasMany(e => e.facturas_so_sale)
                .WithRequired(e => e.facturas)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<facturas_so_sale>()
                .Property(e => e.serie)
                .IsUnicode(false);

            modelBuilder.Entity<so_billing_data>()
                .Property(e => e.code)
                .IsUnicode(false);

            modelBuilder.Entity<so_billing_data>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<so_billing_data>()
                .Property(e => e.address_street)
                .IsUnicode(false);

            modelBuilder.Entity<so_billing_data>()
                .Property(e => e.address_number)
                .IsUnicode(false);

            modelBuilder.Entity<so_billing_data>()
                .Property(e => e.postal_code)
                .IsUnicode(false);

            modelBuilder.Entity<so_billing_data>()
                .Property(e => e.suburb)
                .IsUnicode(false);

            modelBuilder.Entity<so_billing_data>()
                .Property(e => e.town)
                .IsUnicode(false);

            modelBuilder.Entity<so_billing_data>()
                .Property(e => e.state)
                .IsUnicode(false);

            modelBuilder.Entity<so_billing_data>()
                .Property(e => e.country)
                .IsUnicode(false);

            modelBuilder.Entity<so_billing_data>()
                .Property(e => e.ftr)
                .IsUnicode(false);

            modelBuilder.Entity<so_billing_data>()
                .Property(e => e.phone)
                .IsUnicode(false);

            modelBuilder.Entity<so_branch>()
                .Property(e => e.code)
                .IsUnicode(false);

            modelBuilder.Entity<so_branch>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<so_branch>()
                .HasMany(e => e.so_user)
                .WithRequired(e => e.so_branch)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_configuration_customer>()
                .Property(e => e.payment_method)
                .IsUnicode(false);

            modelBuilder.Entity<so_configuration_customer>()
                .Property(e => e.payment_account)
                .IsUnicode(false);

            modelBuilder.Entity<so_customer>()
                .Property(e => e.code)
                .IsUnicode(false);

            modelBuilder.Entity<so_customer>()
                .Property(e => e.contact)
                .IsUnicode(false);

            modelBuilder.Entity<so_customer>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<so_customer>()
                .Property(e => e.address)
                .IsUnicode(false);

            modelBuilder.Entity<so_customer>()
                .Property(e => e.description)
                .IsUnicode(false);

            modelBuilder.Entity<so_customer>()
                .Property(e => e.email)
                .IsUnicode(false);

            modelBuilder.Entity<so_customer>()
                .HasMany(e => e.so_customer_data)
                .WithRequired(e => e.so_customer)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_customer>()
                .HasMany(e => e.so_sale)
                .WithRequired(e => e.so_customer)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_customer_data>()
                .Property(e => e.ftr)
                .IsUnicode(false);

            modelBuilder.Entity<so_customer_data>()
                .Property(e => e.business_name)
                .IsUnicode(false);

            modelBuilder.Entity<so_customer_data>()
                .Property(e => e.fiscal_address)
                .IsUnicode(false);

            modelBuilder.Entity<so_customer_data>()
                .Property(e => e.trade_name)
                .IsUnicode(false);

            modelBuilder.Entity<so_customer_data>()
                .Property(e => e.payment_method)
                .IsUnicode(false);

            modelBuilder.Entity<so_customer_data>()
                .Property(e => e.account_ended)
                .IsUnicode(false);

            modelBuilder.Entity<so_customer_data>()
                .Property(e => e.country)
                .IsUnicode(false);

            modelBuilder.Entity<so_customer_data>()
                .Property(e => e.state)
                .IsUnicode(false);

            modelBuilder.Entity<so_customer_data>()
                .Property(e => e.town)
                .IsUnicode(false);

            modelBuilder.Entity<so_customer_data>()
                .Property(e => e.suburb)
                .IsUnicode(false);

            modelBuilder.Entity<so_customer_data>()
                .Property(e => e.address_street)
                .IsUnicode(false);

            modelBuilder.Entity<so_customer_data>()
                .Property(e => e.address_number)
                .IsUnicode(false);

            modelBuilder.Entity<so_customer_data>()
                .Property(e => e.address_number_cross1)
                .IsUnicode(false);

            modelBuilder.Entity<so_customer_data>()
                .Property(e => e.address_number_cross2)
                .IsUnicode(false);

            modelBuilder.Entity<so_product>()
                .Property(e => e.code)
                .IsUnicode(false);

            modelBuilder.Entity<so_product>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<so_product>()
                .Property(e => e.barcode)
                .IsUnicode(false);

            modelBuilder.Entity<so_product>()
                .Property(e => e.reference)
                .IsUnicode(false);

            modelBuilder.Entity<so_product>()
                .HasMany(e => e.so_sale_detail)
                .WithRequired(e => e.so_product)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_product>()
                .HasOptional(e => e.so_product_tax)
                .WithRequired(e => e.so_product);

            modelBuilder.Entity<so_product_tax>()
                .Property(e => e.trade_volume)
                .HasPrecision(10, 6);

            modelBuilder.Entity<so_product_tax>()
                .Property(e => e.pieces)
                .HasPrecision(10, 4);

            modelBuilder.Entity<so_product_tax>()
                .Property(e => e.description_tax)
                .IsUnicode(false);

            modelBuilder.Entity<so_product_tax>()
                .Property(e => e.unit)
                .IsUnicode(false);

            modelBuilder.Entity<so_sale>()
                .Property(e => e.tag)
                .IsUnicode(false);

            modelBuilder.Entity<so_sale>()
                .HasMany(e => e.facturas_so_sale)
                .WithRequired(e => e.so_sale)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_sale>()
                .HasMany(e => e.so_sale_detail)
                .WithRequired(e => e.so_sale)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_sale_detail>()
                .Property(e => e.base_price_no_tax)
                .HasPrecision(10, 4);

            modelBuilder.Entity<so_sale_detail>()
                .Property(e => e.discount_no_tax)
                .HasPrecision(10, 4);

            modelBuilder.Entity<so_sale_detail>()
                .Property(e => e.vat)
                .HasPrecision(10, 4);

            modelBuilder.Entity<so_sale_detail>()
                .Property(e => e.vat_total)
                .HasPrecision(10, 4);

            modelBuilder.Entity<so_sale_detail>()
                .Property(e => e.stps)
                .HasPrecision(10, 4);

            modelBuilder.Entity<so_sale_detail>()
                .Property(e => e.stps_fee)
                .HasPrecision(10, 4);

            modelBuilder.Entity<so_sale_detail>()
                .Property(e => e.stps_snack)
                .HasPrecision(10, 4);

            modelBuilder.Entity<so_sale_detail>()
                .Property(e => e.net_content)
                .HasPrecision(10, 4);

            modelBuilder.Entity<so_sale_detail>()
                .Property(e => e.vat_rate)
                .HasPrecision(10, 4);

            modelBuilder.Entity<so_sale_detail>()
                .Property(e => e.stps_rate)
                .HasPrecision(10, 4);

            modelBuilder.Entity<so_sale_detail>()
                .Property(e => e.stps_fee_rate)
                .HasPrecision(10, 4);

            modelBuilder.Entity<so_sale_detail>()
                .Property(e => e.stps_snack_rate)
                .HasPrecision(10, 4);

            modelBuilder.Entity<so_site>()
                .Property(e => e.code)
                .IsUnicode(false);

            modelBuilder.Entity<so_site>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<so_site>()
                .Property(e => e.address)
                .IsUnicode(false);

            modelBuilder.Entity<so_site>()
                .Property(e => e.postal_code)
                .IsUnicode(false);

            modelBuilder.Entity<so_site>()
                .Property(e => e.city)
                .IsUnicode(false);

            modelBuilder.Entity<so_site>()
                .Property(e => e.town)
                .IsUnicode(false);

            modelBuilder.Entity<so_site>()
                .Property(e => e.state)
                .IsUnicode(false);

            modelBuilder.Entity<so_site>()
                .Property(e => e.country)
                .IsUnicode(false);

            modelBuilder.Entity<so_site>()
                .HasMany(e => e.so_branch)
                .WithRequired(e => e.so_site)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_user>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<so_user>()
                .Property(e => e.code)
                .IsUnicode(false);

            modelBuilder.Entity<so_user>()
                .Property(e => e.tag)
                .IsUnicode(false);

            modelBuilder.Entity<so_user>()
                .HasMany(e => e.so_sale)
                .WithRequired(e => e.so_user)
                .WillCascadeOnDelete(false);
        }
    }
}
