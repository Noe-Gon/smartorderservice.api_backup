namespace SmartOrderService.DB
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using SmartOrderService.Controllers;

    public partial class SmartOrderModel : DbContext
    {
        public SmartOrderModel()
            : base("name=SmartOrderModel")
        {
        }

        public virtual DbSet<so_survey_customer> so_survey_customer { set; get; }
        public virtual DbSet<factdet> factdets { get; set; }
        public virtual DbSet<factura> facturas { get; set; }
        public virtual DbSet<facturas_so_sale> facturas_so_sale { get; set; }
        public virtual DbSet<so_api_version> so_api_version { get; set; }
        public virtual DbSet<so_application> so_application { get; set; }
        public virtual DbSet<so_article> so_article { get; set; }
        public virtual DbSet<so_billing_data> so_billing_data { get; set; }
        public virtual DbSet<so_binnacle_reason_failed> so_binnacle_reason_failed { get; set; }
        public virtual DbSet<so_binnacle_visit> so_binnacle_visit { get; set; }
        public virtual DbSet<so_bottle> so_bottle { get; set; }
        public virtual DbSet<so_branch> so_branch { get; set; }
        public virtual DbSet<so_branch_articles> so_branch_articles { get; set; }
        public virtual DbSet<so_branch_config> so_branch_config { get; set; }
        public virtual DbSet<so_branch_products_discount_list> so_branch_products_discount_list { get; set; }
        public virtual DbSet<so_branch_reason_devolution> so_branch_reason_devolution { get; set; }
        public virtual DbSet<so_branch_reason_failed_transaction> so_branch_reason_failed_transaction { get; set; }
        public virtual DbSet<so_branch_reason_replacement> so_branch_reason_replacement { get; set; }
        public virtual DbSet<so_branch_replacement> so_branch_replacement { get; set; }
        public virtual DbSet<so_branch_tax> so_branch_tax { get; set; }
        public virtual DbSet<so_brand> so_brand { get; set; }
        public virtual DbSet<so_category> so_category { get; set; }
        public virtual DbSet<so_category_billing_data> so_category_billing_data { get; set; }
        public virtual DbSet<so_category_restriction> so_category_restriction { get; set; }
        public virtual DbSet<so_cellar_notice> so_cellar_notice { get; set; }
        public virtual DbSet<so_collect_bottle> so_collect_bottle { get; set; }
        public virtual DbSet<so_collect_bottle_detail> so_collect_bottle_detail { get; set; }
        public virtual DbSet<so_company> so_company { get; set; }
        public virtual DbSet<so_configuration_customer> so_configuration_customer { get; set; }
        public virtual DbSet<so_container> so_container { get; set; }
        public virtual DbSet<so_container_customer> so_container_customer { get; set; }
        public virtual DbSet<so_container_detail> so_container_detail { get; set; }
        public virtual DbSet<so_container_user> so_container_user { get; set; }
        public virtual DbSet<so_control_download> so_control_download { get; set; }
        public virtual DbSet<so_control_send_mail> so_control_send_mail { get; set; }
        public virtual DbSet<so_customer> so_customer { get; set; }
        public virtual DbSet<so_customer_data> so_customer_data { get; set; }
        public virtual DbSet<so_customer_products_price_list> so_customer_products_price_list { get; set; }
        public virtual DbSet<so_customer_promotion_config> so_customer_promotion_config { get; set; }
        public virtual DbSet<so_data_input> so_data_input { get; set; }
        public virtual DbSet<so_data_out> so_data_out { get; set; }
        public virtual DbSet<so_delivery> so_delivery { get; set; }
        public virtual DbSet<so_delivery_detail> so_delivery_detail { get; set; }
        public virtual DbSet<so_delivery_devolution> so_delivery_devolution { get; set; }
        public virtual DbSet<so_delivery_promotion> so_delivery_promotion { get; set; }
        public virtual DbSet<so_delivery_promotion_article_detail> so_delivery_promotion_article_detail { get; set; }
        public virtual DbSet<so_delivery_promotion_detail> so_delivery_promotion_detail { get; set; }
        public virtual DbSet<so_delivery_references> so_delivery_references { get; set; }
        public virtual DbSet<so_delivery_replacement> so_delivery_replacement { get; set; }
        public virtual DbSet<so_delivery_sale> so_delivery_sale { get; set; }
        public virtual DbSet<so_device> so_device { get; set; }
        public virtual DbSet<so_equivalence_article> so_equivalence_article { get; set; }
        public virtual DbSet<so_equivalence_product> so_equivalence_product { get; set; }
        public virtual DbSet<so_fee> so_fee { get; set; }
        public virtual DbSet<so_global_promotion> so_global_promotion { get; set; }
        public virtual DbSet<so_inventory> so_inventory { get; set; }
        public virtual DbSet<so_inventory_container> so_inventory_container { get; set; }
        public virtual DbSet<so_inventory_detail> so_inventory_detail { get; set; }
        public virtual DbSet<so_inventory_detail_article> so_inventory_detail_article { get; set; }
        public virtual DbSet<so_inventory_process> so_inventory_process { get; set; }
        public virtual DbSet<so_inventory_product_container> so_inventory_product_container { get; set; }
        public virtual DbSet<so_inventory_replacement_detail> so_inventory_replacement_detail { get; set; }
        public virtual DbSet<so_inventory_summary> so_inventory_summary { get; set; }
        public virtual DbSet<so_loan_order> so_loan_order { get; set; }
        public virtual DbSet<so_loan_sale> so_loan_sale { get; set; }
        public virtual DbSet<so_log_errors> so_log_errors { get; set; }
        public virtual DbSet<so_price_list_products_detail> so_price_list_products_detail { get; set; }
        public virtual DbSet<so_process> so_process { get; set; }
        public virtual DbSet<so_process_branch> so_process_branch { get; set; }
        public virtual DbSet<so_process_user> so_process_user { get; set; }
        public virtual DbSet<so_product> so_product { get; set; }
        public virtual DbSet<so_product_article> so_product_article { get; set; }
        public virtual DbSet<so_product_bottle> so_product_bottle { get; set; }
        public virtual DbSet<so_product_category_branch> so_product_category_branch { get; set; }
        public virtual DbSet<so_product_company> so_product_company { get; set; }
        public virtual DbSet<so_product_tax> so_product_tax { get; set; }
        public virtual DbSet<so_products_discount_list> so_products_discount_list { get; set; }
        public virtual DbSet<so_products_discount_list_detail> so_products_discount_list_detail { get; set; }
        public virtual DbSet<so_products_price_list> so_products_price_list { get; set; }
        public virtual DbSet<so_promotion> so_promotion { get; set; }
        public virtual DbSet<so_promotion_detail_article> so_promotion_detail_article { get; set; }
        public virtual DbSet<so_promotion_detail_product> so_promotion_detail_product { get; set; }
        public virtual DbSet<so_promotion_discount_list> so_promotion_discount_list { get; set; }
        public virtual DbSet<so_reason_devolution> so_reason_devolution { get; set; }
        public virtual DbSet<so_reason_failed_transaction> so_reason_failed_transaction { get; set; }
        public virtual DbSet<so_reason_replacement> so_reason_replacement { get; set; }
        public virtual DbSet<so_reception_bottle> so_reception_bottle { get; set; }
        public virtual DbSet<so_reception_bottle_detail> so_reception_bottle_detail { get; set; }
        public virtual DbSet<so_reception_collect_bottle> so_reception_collect_bottle { get; set; }
        public virtual DbSet<so_replacement> so_replacement { get; set; }
        public virtual DbSet<so_revision_states> so_revision_states { get; set; }
        public virtual DbSet<so_revision_types> so_revision_types { get; set; }
        public virtual DbSet<so_route> so_route { get; set; }
        public virtual DbSet<so_route_category> so_route_category { get; set; }
        public virtual DbSet<so_route_customer> so_route_customer { get; set; }
        public virtual DbSet<so_inventory_revisions> so_inventory_revisions { get; set; }
        public virtual DbSet<so_sale> so_sale { get; set; }
        public virtual DbSet<so_sale_aditional_data> so_sale_aditional_data { get; set; }
        public virtual DbSet<so_sale_detail> so_sale_detail { get; set; }
        public virtual DbSet<so_sale_inventory> so_sale_inventory { get; set; }
        public virtual DbSet<so_sale_promotion> so_sale_promotion { get; set; }
        public virtual DbSet<so_sale_promotion_detail> so_sale_promotion_detail { get; set; }
        public virtual DbSet<so_sale_promotion_detail_article> so_sale_promotion_detail_article { get; set; }
        public virtual DbSet<so_sale_replacement> so_sale_replacement { get; set; }
        public virtual DbSet<so_sale_send_mail> so_sale_send_mail { get; set; }
        public virtual DbSet<so_site> so_site { get; set; }
        public virtual DbSet<so_summary> so_summary { get; set; }
        public virtual DbSet<so_tag> so_tag { get; set; }
        public virtual DbSet<so_user> so_user { get; set; }
        public virtual DbSet<so_user_customer_ribe> so_user_customer_ribe { get; set; }
        public virtual DbSet<so_user_devolutions> so_user_devolutions { get; set; }
        public virtual DbSet<so_user_notice_recharge> so_user_notice_recharge { get; set; }
        public virtual DbSet<so_user_notice_recharge_route> so_user_notice_recharge_route { get; set; }
        public virtual DbSet<so_user_portal> so_user_portal { get; set; }
        public virtual DbSet<so_user_portal_branch> so_user_portal_branch { get; set; }
        public virtual DbSet<so_user_promotion_config> so_user_promotion_config { get; set; }
        public virtual DbSet<so_user_reason_devolutions> so_user_reason_devolutions { get; set; }
        public virtual DbSet<so_tracking> so_tracking { get; set; }
        public virtual DbSet<so_tracking_configuration> so_tracking_configuration { get; set; }
        public virtual DbSet<so_user_route> so_user_route { get; set; }
        public virtual DbSet<so_work_day> so_work_day { get; set; }
        public virtual DbSet<ribe_producto> ribe_producto { get; set; }
        public virtual DbSet<SO_CAT_TYPE_USER_VISIT> SO_CAT_TYPE_USER_VISIT { get; set; }
        public virtual DbSet<DEVOLUTIONS_VIEW> DEVOLUTIONS_VIEW { get; set; }
        public virtual DbSet<so_autoventa_View> so_autoventa_View { get; set; }
        public virtual DbSet<so_preventa_View> so_preventa_View { get; set; }
        public virtual DbSet<so_venta_View> so_venta_View { get; set; }
        public virtual DbSet<User_Visit_Plan> User_Visit_Plan { get; set; }
        public virtual DbSet<so_invoice_opefactura> so_invoice_opefactura { set; get; }

        public virtual DbSet<so_role_team> so_role_team { get; set; }
        public virtual DbSet<so_route_team> so_route_team { get; set; }
        public virtual DbSet<so_route_team_travels> so_route_team_travels { get; set; }

        public virtual DbSet<so_route_team_inventory_available> so_route_team_inventory_available { get; set; }

        public virtual DbSet<so_route_team_travels_visit> so_route_team_travels_visits { get; set; }

        public virtual DbSet<so_customer_additional_data> so_customerr_additional_data { get; set; }
        public virtual DbSet<so_customer_removal_request> so_customer_romoval_requests { get; set; }
        public virtual DbSet<so_portal_links_log> so_portal_links_logs { get; set; }
        public virtual DbSet<so_code_place> so_code_places { get; set; }
        public virtual DbSet<so_route_team_travels_employees> so_route_team_travels_employees { get; set; }
        public virtual DbSet<so_route_team_travels_customer_blocked> so_route_team_travel_customer_blockeds { get; set; }
        public virtual DbSet<so_leader_authorization_code> so_leader_authorization_codes { get; set; }
        public virtual DbSet<so_authentication_log> so_authentication_logs { get; set; }

        public virtual DbSet<so_sale_detail_article> so_sale_detail_article { get; set; }

        public virtual DbSet<so_promotion_article_movement> so_promotion_article_movement { get; set; }

        public virtual DbSet<so_article_promotional_route> so_article_promotional_route { get; set; }
        public virtual DbSet<so_delivery_status> so_delivery_status { get; set; }
        public virtual DbSet<so_order> so_order { get; set; }
        public virtual DbSet<so_order_detail> so_order_detail { get; set; }
        public virtual DbSet<so_delivery_additional_data> so_delivery_additional_data { get; set; }
        public virtual DbSet<so_synchronized_consumer> so_synchronized_consumer { get; set; }
        public virtual DbSet<so_synchronized_consumer_detail> so_synchronized_consumer_detail { get; set; }
        public virtual DbSet<Configuracion_WorkByCloud> Configuracion_WorkByCloud { get; set; }
        public virtual DbSet<so_promotion_type_catalog> so_promotion_type_catalog { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<so_role_team>()
                .Property(e => e.description)
                .IsFixedLength();

            modelBuilder.Entity<so_role_team>()
                .HasMany(e => e.so_route_team)
                .WithRequired(e => e.so_role_team)
                .WillCascadeOnDelete(false);

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

            modelBuilder.Entity<factura>()
                .Property(e => e.serie)
                .IsUnicode(false);

            modelBuilder.Entity<factura>()
                .Property(e => e.cliente)
                .IsUnicode(false);

            modelBuilder.Entity<factura>()
                .Property(e => e.ruta)
                .IsUnicode(false);

            modelBuilder.Entity<factura>()
                .Property(e => e.facturaref)
                .IsUnicode(false);

            modelBuilder.Entity<factura>()
                .Property(e => e.rfc)
                .IsUnicode(false);

            modelBuilder.Entity<factura>()
                .Property(e => e.total)
                .HasPrecision(25, 8);

            modelBuilder.Entity<factura>()
                .Property(e => e.importeiva)
                .HasPrecision(25, 8);

            modelBuilder.Entity<factura>()
                .Property(e => e.importeiesp)
                .HasPrecision(25, 8);

            modelBuilder.Entity<factura>()
                .Property(e => e.importeiespcuota)
                .HasPrecision(25, 8);

            modelBuilder.Entity<factura>()
                .Property(e => e.importeiespbotana)
                .HasPrecision(25, 8);

            modelBuilder.Entity<factura>()
                .Property(e => e.sitio)
                .IsUnicode(false);

            modelBuilder.Entity<factura>()
                .Property(e => e.observ)
                .IsUnicode(false);

            modelBuilder.Entity<factura>()
                .Property(e => e.razonsocial)
                .IsUnicode(false);

            modelBuilder.Entity<factura>()
                .Property(e => e.direccioncliente)
                .IsUnicode(false);

            modelBuilder.Entity<factura>()
                .Property(e => e.tipodecambio)
                .HasPrecision(32, 16);

            modelBuilder.Entity<factura>()
                .HasMany(e => e.factdets)
                .WithRequired(e => e.factura)
                .HasForeignKey(e => e.folio)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<factura>()
                .HasMany(e => e.facturas_so_sale)
                .WithRequired(e => e.factura1)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<facturas_so_sale>()
                .Property(e => e.serie)
                .IsUnicode(false);

            modelBuilder.Entity<so_application>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<so_application>()
                .Property(e => e.name_installer)
                .IsUnicode(false);

            modelBuilder.Entity<so_application>()
                .Property(e => e.version)
                .IsUnicode(false);

            modelBuilder.Entity<so_application>()
                .Property(e => e.download_url)
                .IsUnicode(false);

            modelBuilder.Entity<so_application>()
                .Property(e => e.ws_url)
                .IsUnicode(false);

            modelBuilder.Entity<so_application>()
                .Property(e => e.package)
                .IsUnicode(false);

            modelBuilder.Entity<so_article>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<so_article>()
                .Property(e => e.code)
                .IsUnicode(false);

            modelBuilder.Entity<so_article>()
                .HasMany(e => e.so_branch_articles)
                .WithRequired(e => e.so_article)
                .WillCascadeOnDelete(false);

            /*modelBuilder.Entity<so_article>()
               .HasMany(e => e.so_sale_promotion_detail_article)
               .WithRequired(e => e.so_article)
               .WillCascadeOnDelete(false);*/

            modelBuilder.Entity<so_article_promotional_route>()
                .HasMany(e => e.so_promotion_article_movement)
                .WithRequired(e => e.so_article_promotional_route)
                .WillCascadeOnDelete(false);

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

            modelBuilder.Entity<so_billing_data>()
                .HasMany(e => e.so_category_billing_data)
                .WithRequired(e => e.so_billing_data)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_binnacle_visit>()
                .HasMany(e => e.so_binnacle_reason_failed)
                .WithRequired(e => e.so_binnacle_visit)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_bottle>()
                .Property(e => e.code)
                .IsUnicode(false);

            modelBuilder.Entity<so_bottle>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<so_branch>()
                .Property(e => e.code)
                .IsUnicode(false);

            modelBuilder.Entity<so_branch>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<so_branch>()
                .HasMany(e => e.so_collect_bottle)
                .WithRequired(e => e.so_branch)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_branch>()
                .HasMany(e => e.so_products_price_list)
                .WithRequired(e => e.so_branch)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_branch>()
                .HasMany(e => e.so_branch_articles)
                .WithRequired(e => e.so_branch)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_branch>()
                .HasMany(e => e.so_branch_products_discount_list)
                .WithRequired(e => e.so_branch)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_branch>()
                .HasMany(e => e.so_branch_reason_devolution)
                .WithRequired(e => e.so_branch)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_branch>()
                .HasMany(e => e.so_branch_reason_failed_transaction)
                .WithRequired(e => e.so_branch)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_branch>()
                .HasMany(e => e.so_branch_reason_replacement)
                .WithRequired(e => e.so_branch)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_branch>()
                .HasMany(e => e.so_customer_promotion_config)
                .WithRequired(e => e.so_branch)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_branch>()
                .HasMany(e => e.so_global_promotion)
                .WithRequired(e => e.so_branch)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_branch>()
                .HasMany(e => e.so_process_branch)
                .WithRequired(e => e.so_branch)
                .WillCascadeOnDelete(false);


            modelBuilder.Entity<so_branch>()
                .HasMany(e => e.so_product_category_branch)
                .WithRequired(e => e.so_branch)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_branch>()
                .HasMany(e => e.so_route)
                .WithRequired(e => e.so_branch)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_branch>()
                .HasMany(e => e.so_user)
                .WithRequired(e => e.so_branch)
                .WillCascadeOnDelete(false);
            

            modelBuilder.Entity<so_branch>()
                .HasMany(e => e.so_user_portal_branch)
                .WithRequired(e => e.so_branch)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_branch>()
                .HasOptional(e => e.so_branch_tax)
                .WithRequired(e => e.so_branch);

            modelBuilder.Entity<so_branch>()
                .HasMany(e => e.so_tag)
                .WithRequired(e => e.so_branch)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_branch>()
                .HasMany(e => e.so_user_notice_recharge)
                .WithRequired(e => e.so_branch)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_branch_tax>()
                .Property(e => e.vat)
                .HasPrecision(10, 4);

            modelBuilder.Entity<so_branch_tax>()
                .Property(e => e.stps)
                .HasPrecision(10, 4);

            modelBuilder.Entity<so_branch_tax>()
                .Property(e => e.stps_fee)
                .HasPrecision(10, 4);

            modelBuilder.Entity<so_branch_tax>()
                .Property(e => e.stps_snack)
                .HasPrecision(10, 4);

            modelBuilder.Entity<so_brand>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<so_brand>()
                .Property(e => e.image_name)
                .IsUnicode(false);

            modelBuilder.Entity<so_brand>()
                .Property(e => e.code)
                .IsUnicode(false);

            modelBuilder.Entity<so_brand>()
                .HasMany(e => e.so_product)
                .WithRequired(e => e.so_brand)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_category>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<so_category>()
                .Property(e => e.code)
                .IsUnicode(false);

            modelBuilder.Entity<so_category>()
                .HasMany(e => e.so_category_billing_data)
                .WithRequired(e => e.so_category)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_category>()
                .HasMany(e => e.so_category_restriction)
                .WithRequired(e => e.so_category)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_category>()
                .HasMany(e => e.so_product_category_branch)
                .WithRequired(e => e.so_category)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_category>()
                .HasMany(e => e.so_route_category)
                .WithRequired(e => e.so_category)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_collect_bottle>()
                .HasMany(e => e.so_collect_bottle_detail)
                .WithRequired(e => e.so_collect_bottle)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_collect_bottle>()
                .HasMany(e => e.so_reception_collect_bottle)
                .WithRequired(e => e.so_collect_bottle)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_company>()
                .Property(e => e.code)
                .IsUnicode(false);

            modelBuilder.Entity<so_company>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<so_company>()
                .HasMany(e => e.so_branch)
                .WithRequired(e => e.so_company)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_company>()
                .HasMany(e => e.so_process)
                .WithRequired(e => e.so_company)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_company>()
                .HasMany(e => e.so_user_portal)
                .WithRequired(e => e.so_company)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_company>()
                .HasMany(e => e.so_product_company)
                .WithRequired(e => e.so_company)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_configuration_customer>()
                .Property(e => e.payment_method)
                .IsUnicode(false);

            modelBuilder.Entity<so_configuration_customer>()
                .Property(e => e.payment_account)
                .IsUnicode(false);

            modelBuilder.Entity<so_container>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<so_container>()
                .Property(e => e.code)
                .IsUnicode(false);

            modelBuilder.Entity<so_container>()
                .HasMany(e => e.so_container_customer)
                .WithRequired(e => e.so_container)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_container>()
                .HasMany(e => e.so_container_detail)
                .WithRequired(e => e.so_container)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_container_customer>()
                .Property(e => e.name)
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
                .HasMany(e => e.so_binnacle_visit)
                .WithRequired(e => e.so_customer)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_customer>()
                .HasMany(e => e.so_category_restriction)
                .WithRequired(e => e.so_customer)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_customer>()
                .HasMany(e => e.so_collect_bottle)
                .WithRequired(e => e.so_customer)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_customer>()
                .HasMany(e => e.so_container_customer)
                .WithRequired(e => e.so_customer)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_customer>()
                .HasMany(e => e.so_customer_data)
                .WithRequired(e => e.so_customer)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_customer>()
                .HasMany(e => e.so_customer_products_price_list)
                .WithRequired(e => e.so_customer)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_customer>()
                .HasMany(e => e.so_customer_promotion_config)
                .WithRequired(e => e.so_customer)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_customer>()
                .HasMany(e => e.so_route_customer)
                .WithRequired(e => e.so_customer)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_customer>()
                .HasMany(e => e.so_sale)
                .WithRequired(e => e.so_customer)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_customer>()
                .HasMany(e => e.so_reception_bottle)
                .WithRequired(e => e.so_customer)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_customer>()
                .HasMany(e => e.so_tag)
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

            modelBuilder.Entity<so_data_input>()
                .Property(e => e.data)
                .IsUnicode(false);

            modelBuilder.Entity<so_data_input>()
                .Property(e => e.checksum)
                .IsUnicode(false);

            modelBuilder.Entity<so_data_input>()
                .Property(e => e.result)
                .IsUnicode(false);

            modelBuilder.Entity<so_data_out>()
                .Property(e => e.complete_data)
                .IsUnicode(false);

            modelBuilder.Entity<so_data_out>()
                .Property(e => e.partial_data)
                .IsUnicode(false);

            modelBuilder.Entity<so_delivery>()
                .Property(e => e.code)
                .IsUnicode(false);

            modelBuilder.Entity<so_delivery>()
                .HasMany(e => e.so_delivery_detail)
                .WithRequired(e => e.so_delivery)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_delivery>()
                .HasOptional(e => e.so_delivery_devolution)
                .WithRequired(e => e.so_delivery);

            modelBuilder.Entity<so_delivery>()
                .HasMany(e => e.so_delivery_promotion)
                .WithRequired(e => e.so_delivery)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_delivery>()
                .HasMany(e => e.so_delivery_replacement)
                .WithRequired(e => e.so_delivery)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_delivery>()
                .HasOptional(e => e.so_delivery_sale)
                .WithRequired(e => e.so_delivery);

            modelBuilder.Entity<so_delivery_promotion>()
                .HasMany(e => e.so_delivery_promotion_article_detail)
                .WithRequired(e => e.so_delivery_promotion)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_delivery_promotion>()
                .HasMany(e => e.so_delivery_promotion_detail)
                .WithRequired(e => e.so_delivery_promotion)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_delivery_references>()
                .Property(e => e.description)
                .IsUnicode(false);

            modelBuilder.Entity<so_device>()
                .Property(e => e.code)
                .IsUnicode(false);

            modelBuilder.Entity<so_fee>()
                .Property(e => e.tendency)
                .IsUnicode(false);

            modelBuilder.Entity<so_fee>()
                .Property(e => e.vsGoal)
                .IsUnicode(false);

            modelBuilder.Entity<so_fee>()
                .Property(e => e.vsPreviousGoal)
                .IsUnicode(false);

            modelBuilder.Entity<so_inventory>()
                .Property(e => e.code)
                .IsUnicode(false);

            modelBuilder.Entity<so_inventory>()
                .HasMany(e => e.so_delivery)
                .WithRequired(e => e.so_inventory)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_inventory>()
                .HasMany(e => e.so_inventory_detail_article)
                .WithRequired(e => e.so_inventory)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_inventory>()
                .HasMany(e => e.so_inventory_detail)
                .WithRequired(e => e.so_inventory)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_inventory>()
                .HasMany(e => e.so_inventory_replacement_detail)
                .WithRequired(e => e.so_inventory)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_inventory>()
                .HasOptional(e => e.so_inventory_summary)
                .WithRequired(e => e.so_inventory);

            modelBuilder.Entity<so_inventory>()
                .HasMany(e => e.so_sale_inventory)
                .WithRequired(e => e.so_inventory)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_inventory>()
                .HasMany(e => e.so_user_devolutions)
                .WithRequired(e => e.so_inventory)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_inventory_container>()
                .HasMany(e => e.so_inventory_product_container)
                .WithRequired(e => e.so_inventory_container)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_inventory_process>()
                .Property(e => e.description)
                .IsUnicode(false);

            modelBuilder.Entity<so_invoice_opefactura>()
                .Property(e => e.total);
                //.HasPrecision(10, 4);

            modelBuilder.Entity<so_log_errors>()
                .Property(e => e.description)
                .IsUnicode(false);

            modelBuilder.Entity<so_log_errors>()
                .Property(e => e.branch_code)
                .IsUnicode(false);

            modelBuilder.Entity<so_process>()
                .Property(e => e.description)
                .IsUnicode(false);

            modelBuilder.Entity<so_process>()
                .HasMany(e => e.so_log_errors)
                .WithRequired(e => e.so_process)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_process>()
                .HasMany(e => e.so_process_user)
                .WithRequired(e => e.so_process)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_process>()
                .HasMany(e => e.so_process_branch)
                .WithRequired(e => e.so_process)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_process>()
                .HasMany(e => e.so_summary)
                .WithRequired(e => e.so_process)
                .WillCascadeOnDelete(false);

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
                .HasMany(e => e.so_collect_bottle_detail)
                .WithRequired(e => e.so_product)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_product>()
                .HasMany(e => e.so_container_detail)
                .WithRequired(e => e.so_product)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_product>()
                .HasMany(e => e.so_delivery_detail)
                .WithRequired(e => e.so_product)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_product>()
                .HasMany(e => e.so_delivery_promotion_article_detail)
                .WithRequired(e => e.so_product)
                .HasForeignKey(e => e.articleId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_product>()
                .HasMany(e => e.so_delivery_promotion_detail)
                .WithRequired(e => e.so_product)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_product>()
                .HasMany(e => e.so_equivalence_article)
                .WithRequired(e => e.so_product)
                .HasForeignKey(e => e.articleId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_product>()
                .HasMany(e => e.so_equivalence_product)
                .WithRequired(e => e.so_product)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_product>()
                .HasMany(e => e.so_inventory_detail)
                .WithRequired(e => e.so_product)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_product>()
                .HasMany(e => e.so_inventory_detail_article)
                .WithRequired(e => e.so_product)
                .HasForeignKey(e => e.articleId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_product>()
                .HasMany(e => e.so_inventory_product_container)
                .WithRequired(e => e.so_product)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_product>()
                .HasMany(e => e.so_loan_order)
                .WithRequired(e => e.so_product)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_product>()
                .HasMany(e => e.so_loan_sale)
                .WithRequired(e => e.so_product)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_product>()
                .HasMany(e => e.so_price_list_products_detail)
                .WithRequired(e => e.so_product)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_product>()
                .HasMany(e => e.so_user_devolutions)
                .WithRequired(e => e.so_product)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_product>()
                .HasMany(e => e.so_product_article)
                .WithRequired(e => e.so_product)
                .HasForeignKey(e => e.articleId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_product>()
                .HasMany(e => e.so_promotion_detail_article)
                .WithRequired(e => e.so_product)
                .HasForeignKey(e => e.articleId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_product>()
                .HasMany(e => e.so_product_bottle)
                .WithRequired(e => e.so_product)
                .HasForeignKey(e => e.bottleId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_product>()
                .HasMany(e => e.so_product_company)
                .WithRequired(e => e.so_product)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_product>()
                .HasMany(e => e.so_product_article1)
                .WithRequired(e => e.so_product1)
                .HasForeignKey(e => e.productId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_product>()
                .HasMany(e => e.so_product_bottle1)
                .WithRequired(e => e.so_product1)
                .HasForeignKey(e => e.productId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_product>()
                .HasMany(e => e.so_product_category_branch)
                .WithRequired(e => e.so_product)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_product>()
                .HasMany(e => e.so_products_discount_list_detail)
                .WithRequired(e => e.so_product)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_product>()
                .HasMany(e => e.so_promotion_detail_product)
                .WithRequired(e => e.so_product)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_product>()
                .HasMany(e => e.so_sale_detail)
                .WithRequired(e => e.so_product)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_product>()
                .HasMany(e => e.so_sale_promotion_detail)
                .WithRequired(e => e.so_product)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_product>()
                .HasOptional(e => e.so_product_tax)
                .WithRequired(e => e.so_product);

            modelBuilder.Entity<so_product>()
                .HasMany(e => e.so_reception_bottle_detail)
                .WithRequired(e => e.so_product)
                .WillCascadeOnDelete(false);

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

            modelBuilder.Entity<so_products_discount_list>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<so_products_discount_list>()
                .Property(e => e.code)
                .IsUnicode(false);

            modelBuilder.Entity<so_products_discount_list>()
                .HasMany(e => e.so_branch_products_discount_list)
                .WithRequired(e => e.so_products_discount_list)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_products_discount_list>()
                .HasMany(e => e.so_products_discount_list_detail)
                .WithRequired(e => e.so_products_discount_list)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_products_discount_list>()
                .HasMany(e => e.so_promotion_discount_list)
                .WithRequired(e => e.so_products_discount_list)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_products_price_list>()
                .Property(e => e.code)
                .IsUnicode(false);

            modelBuilder.Entity<so_products_price_list>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<so_products_price_list>()
                .HasMany(e => e.so_customer_products_price_list)
                .WithRequired(e => e.so_products_price_list)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_products_price_list>()
                .HasMany(e => e.so_price_list_products_detail)
                .WithRequired(e => e.so_products_price_list)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_promotion>()
                .Property(e => e.code)
                .IsUnicode(false);

            modelBuilder.Entity<so_promotion>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<so_promotion>()
                .HasMany(e => e.so_customer_promotion_config)
                .WithRequired(e => e.so_promotion)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_promotion>()
                .HasMany(e => e.so_global_promotion)
                .WithRequired(e => e.so_promotion)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_promotion>()
                .HasMany(e => e.so_promotion_detail_article)
                .WithRequired(e => e.so_promotion)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_promotion>()
                .HasMany(e => e.so_promotion_detail_product)
                .WithRequired(e => e.so_promotion)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_promotion>()
                .HasMany(e => e.so_promotion_discount_list)
                .WithRequired(e => e.so_promotion)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_promotion>()
                .HasMany(e => e.so_sale_promotion)
                .WithRequired(e => e.so_promotion)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_promotion>()
                .HasMany(e => e.so_user_promotion_config)
                .WithRequired(e => e.so_promotion)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_promotion_detail_article>()
                .HasMany(e => e.so_equivalence_article)
                .WithRequired(e => e.so_promotion_detail_article)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_promotion_detail_product>()
                .HasMany(e => e.so_equivalence_product)
                .WithRequired(e => e.so_promotion_detail_product)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_reason_devolution>()
                .Property(e => e.code)
                .IsUnicode(false);

            modelBuilder.Entity<so_reason_devolution>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<so_reason_devolution>()
                .Property(e => e.description)
                .IsUnicode(false);

            modelBuilder.Entity<so_reason_devolution>()
                .HasMany(e => e.so_branch_reason_devolution)
                .WithRequired(e => e.so_reason_devolution)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_reason_devolution>()
                .HasMany(e => e.so_delivery_devolution)
                .WithRequired(e => e.so_reason_devolution)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_reason_failed_transaction>()
                .Property(e => e.code)
                .IsUnicode(false);

            modelBuilder.Entity<so_reason_failed_transaction>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<so_reason_failed_transaction>()
                .Property(e => e.description)
                .IsUnicode(false);

            modelBuilder.Entity<so_reason_failed_transaction>()
                .HasMany(e => e.so_binnacle_reason_failed)
                .WithRequired(e => e.so_reason_failed_transaction)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_reason_failed_transaction>()
                .HasMany(e => e.so_branch_reason_failed_transaction)
                .WithRequired(e => e.so_reason_failed_transaction)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_reason_replacement>()
                .Property(e => e.code)
                .IsUnicode(false);

            modelBuilder.Entity<so_reason_replacement>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<so_reason_replacement>()
                .Property(e => e.description)
                .IsUnicode(false);

            modelBuilder.Entity<so_reason_replacement>()
                .HasMany(e => e.so_branch_reason_replacement)
                .WithRequired(e => e.so_reason_replacement)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_reception_bottle>()
                .HasMany(e => e.so_reception_bottle_detail)
                .WithRequired(e => e.so_reception_bottle)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_reception_bottle>()
                .HasMany(e => e.so_reception_collect_bottle)
                .WithRequired(e => e.so_reception_bottle)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_replacement>()
                .Property(e => e.code)
                .IsUnicode(false);

            modelBuilder.Entity<so_replacement>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<so_replacement>()
                .HasMany(e => e.so_delivery_replacement)
                .WithRequired(e => e.so_replacement)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_replacement>()
                .HasMany(e => e.so_inventory_replacement_detail)
                .WithRequired(e => e.so_replacement)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_replacement>()
                .HasMany(e => e.so_sale_replacement)
                .WithRequired(e => e.so_replacement)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_revision_states>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<so_revision_states>()
                .Property(e => e.value)
                .IsFixedLength();

            modelBuilder.Entity<so_revision_states>()
                .HasMany(e => e.so_route_revisions)
                .WithRequired(e => e.so_revision_states)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_revision_types>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<so_revision_types>()
                .HasMany(e => e.so_route_revisions)
                .WithRequired(e => e.so_revision_types)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_route>()
                .Property(e => e.code)
                .IsUnicode(false);

            modelBuilder.Entity<so_route>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<so_route>()
                .HasMany(e => e.so_cellar_notice)
                .WithRequired(e => e.so_route)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_route>()
                .HasMany(e => e.so_route_revisions)
                .WithRequired(e => e.so_route)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_route>()
                .HasMany(e => e.so_route_category)
                .WithRequired(e => e.so_route)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_route>()
                .HasMany(e => e.so_route_customer)
                .WithRequired(e => e.so_route)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_route>()
                .HasMany(e => e.so_user_route)
                .WithRequired(e => e.so_route)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_route>()
                .HasMany(e => e.so_user_notice_recharge_route)
                .WithRequired(e => e.so_route)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_sale>()
                .Property(e => e.tag)
                .IsUnicode(false);

            modelBuilder.Entity<so_sale>()
                .HasMany(e => e.facturas_so_sale)
                .WithRequired(e => e.so_sale)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_sale>()
                .HasMany(e => e.so_invoice_opefactura)
                .WithRequired(e => e.so_sale)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_sale>()
                .HasMany(e => e.so_delivery_sale)
                .WithRequired(e => e.so_sale)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_sale>()
                .HasMany(e => e.so_loan_sale)
                .WithRequired(e => e.so_sale)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_sale>()
                .HasMany(e => e.so_sale_inventory)
                .WithRequired(e => e.so_sale)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_sale>()
                .HasMany(e => e.so_sale_replacement)
                .WithRequired(e => e.so_sale)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_sale>()
                .HasMany(e => e.so_sale_detail)
                .WithRequired(e => e.so_sale)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_sale>()
                .HasMany(e => e.so_sale_promotion)
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

            modelBuilder.Entity<so_sale_promotion>()
                .HasMany(e => e.so_sale_promotion_detail_article)
                .WithRequired(e => e.so_sale_promotion)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_sale_promotion>()
                .HasMany(e => e.so_sale_promotion_detail)
                .WithRequired(e => e.so_sale_promotion)
                .WillCascadeOnDelete(false);

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

            modelBuilder.Entity<so_summary>()
                .Property(e => e.table_name)
                .IsUnicode(false);

            modelBuilder.Entity<so_summary>()
                .Property(e => e.branch_code)
                .IsUnicode(false);

            modelBuilder.Entity<so_tag>()
                .Property(e => e.tag)
                .IsUnicode(false);

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
                .HasMany(e => e.so_binnacle_visit)
                .WithRequired(e => e.so_user)
                .WillCascadeOnDelete(false);

            

            modelBuilder.Entity<so_user>()
               .HasMany(e => e.so_tracking)
               .WithRequired(e => e.so_user)
               .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_user>()
                .HasMany(e => e.so_category_restriction)
                .WithRequired(e => e.so_user)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_user>()
                .HasMany(e => e.so_cellar_notice)
                .WithRequired(e => e.so_user)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_user>()
                .HasMany(e => e.so_container_user)
                .WithRequired(e => e.so_user)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_user>()
                .HasMany(e => e.so_control_download)
                .WithRequired(e => e.so_user)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_user>()
                .HasMany(e => e.so_data_input)
                .WithRequired(e => e.so_user)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_user>()
                .HasMany(e => e.so_data_out)
                .WithRequired(e => e.so_user)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_user>()
                .HasMany(e => e.so_device)
                .WithRequired(e => e.so_user)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_user>()
                .HasMany(e => e.so_fee)
                .WithRequired(e => e.so_user)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_user>()
                .HasMany(e => e.so_inventory)
                .WithRequired(e => e.so_user)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_user>()
                .HasMany(e => e.so_process_user)
                .WithRequired(e => e.so_user)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_user>()
                .HasMany(e => e.so_reception_bottle)
                .WithRequired(e => e.so_user)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_user>()
                .HasMany(e => e.so_route_revisions)
                .WithRequired(e => e.so_user)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_user>()
                .HasMany(e => e.so_sale)
                .WithRequired(e => e.so_user)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_user>()
                .HasMany(e => e.so_user_promotion_config)
                .WithRequired(e => e.so_user)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_user>()
                .HasMany(e => e.so_user_route)
                .WithRequired(e => e.so_user)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_user_customer_ribe>()
                .Property(e => e.userCode)
                .IsUnicode(false);

            modelBuilder.Entity<so_user_customer_ribe>()
                .Property(e => e.customerCode)
                .IsUnicode(false);

            modelBuilder.Entity<so_user_notice_recharge>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<so_user_notice_recharge>()
                .Property(e => e.mail)
                .IsUnicode(false);

            modelBuilder.Entity<so_user_notice_recharge>()
                .Property(e => e.phone_number)
                .IsUnicode(false);

            modelBuilder.Entity<so_user_notice_recharge>()
                .HasMany(e => e.so_user_notice_recharge_route)
                .WithRequired(e => e.so_user_notice_recharge)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_user_portal>()
                .Property(e => e.name)
                .IsUnicode(false);

            modelBuilder.Entity<so_user_portal>()
                .Property(e => e.nickname)
                .IsUnicode(false);

            modelBuilder.Entity<so_user_portal>()
                .Property(e => e.password)
                .IsUnicode(false);

            modelBuilder.Entity<so_user_portal>()
                .HasMany(e => e.so_user_portal_branch)
                .WithRequired(e => e.so_user_portal)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<so_user_reason_devolutions>()
                .Property(e => e.description)
                .IsUnicode(false);

            modelBuilder.Entity<so_user_reason_devolutions>()
                .HasMany(e => e.so_user_devolutions)
                .WithRequired(e => e.so_user_reason_devolutions)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<SO_CAT_TYPE_USER_VISIT>()
                .Property(e => e.desc_type_visit)
                .IsUnicode(false);

            modelBuilder.Entity<DEVOLUTIONS_VIEW>()
                .Property(e => e.product_code)
                .IsUnicode(false);

            modelBuilder.Entity<DEVOLUTIONS_VIEW>()
                .Property(e => e.user_code)
                .IsUnicode(false);

            modelBuilder.Entity<DEVOLUTIONS_VIEW>()
                .Property(e => e.branch_code)
                .IsUnicode(false);

            modelBuilder.Entity<DEVOLUTIONS_VIEW>()
                .Property(e => e.customer_code)
                .IsUnicode(false);

            modelBuilder.Entity<DEVOLUTIONS_VIEW>()
                .Property(e => e.inventory_code)
                .IsUnicode(false);

            modelBuilder.Entity<DEVOLUTIONS_VIEW>()
                .Property(e => e.reason_code)
                .IsUnicode(false);

            modelBuilder.Entity<so_autoventa_View>()
                .Property(e => e.code_user)
                .IsUnicode(false);

            modelBuilder.Entity<so_autoventa_View>()
                .Property(e => e.code_branch_user)
                .IsUnicode(false);

            modelBuilder.Entity<so_autoventa_View>()
                .Property(e => e.code_customer)
                .IsUnicode(false);

            modelBuilder.Entity<so_preventa_View>()
                .Property(e => e.code_user)
                .IsUnicode(false);

            modelBuilder.Entity<so_preventa_View>()
                .Property(e => e.code_branch_user)
                .IsUnicode(false);

            modelBuilder.Entity<so_preventa_View>()
                .Property(e => e.code_customer)
                .IsUnicode(false);

            modelBuilder.Entity<so_venta_View>()
                .Property(e => e.code_user)
                .IsUnicode(false);

            modelBuilder.Entity<so_venta_View>()
                .Property(e => e.code_branch_user)
                .IsUnicode(false);

            modelBuilder.Entity<so_venta_View>()
                .Property(e => e.code_customer)
                .IsUnicode(false);

            modelBuilder.Entity<User_Visit_Plan>()
                .Property(e => e.code_user)
                .IsUnicode(false);

            modelBuilder.Entity<User_Visit_Plan>()
                .Property(e => e.code_branch_user)
                .IsUnicode(false);

            modelBuilder.Entity<User_Visit_Plan>()
                .Property(e => e.code_customer)
                .IsUnicode(false);

            modelBuilder.Entity<so_route_team_travels_visit>()
                .HasKey(x => new { x.binnacleId, x.routeId, x.inventoryId, x.workDayId });

            modelBuilder.Entity<so_route_team_travels_visit>()
                .HasRequired(x => x.So_Binnacle_Visit)
                .WithMany(x => x.so_route_team_travels_visits)
                .HasForeignKey(x => x.binnacleId);

            modelBuilder.Entity<so_customer_additional_data>()
                .HasKey(x => x.Id)
                .HasRequired(x => x.Customer)
                .WithMany(x => x.CustomerAdditionalData)
                .HasForeignKey(x => x.CustomerId);

            modelBuilder.Entity<so_customer_additional_data>()
                .Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<so_customer_additional_data>()
                .HasOptional(x => x.CodePlace)
                .WithMany(x => x.CustomerAdditionalData)
                .HasForeignKey(x => x.CodePlaceId);

            modelBuilder.Entity<so_customer_removal_request>()
                .HasKey(x => x.Id)
                .HasRequired(x => x.Customer)
                .WithMany(x => x.CustomerRemovalRequests)
                .HasForeignKey(x => x.CustomerId);

            modelBuilder.Entity<so_customer_removal_request>()
                .HasRequired(x => x.User)
                .WithMany(x => x.CustomerRemovalRequests)
                .HasForeignKey(x => x.UserId);

            modelBuilder.Entity<so_portal_links_log>()
                .HasKey(x => x.Id)
                .HasRequired(x => x.Customer)
                .WithMany(x => x.PortalLinksLog)
                .HasForeignKey(x => x.CustomerId);

            var codePlace = modelBuilder.Entity<so_code_place>();
            codePlace.HasKey(x => x.Id);
            codePlace.Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<so_route_team_travels_employees>()
                .HasKey(x => new { x.routeId, x.inventoryId, x.work_dayId, x.userId });

            modelBuilder.Entity<so_route_team_travels_customer_blocked>()
                .HasKey(x => new { x.CustomerId, x.InventoryId, x.WorkDayId, x.UserId });

            modelBuilder.Entity<so_route_team_travels_customer_blocked>()
                .HasRequired(x => x.User)
                .WithMany(x => x.RouteTeamTravelsCustomerBlockeds)
                .HasForeignKey(x => x.UserId);

            modelBuilder.Entity<so_route_team_travels_customer_blocked>()
                .HasRequired(x => x.WorkDay)
                .WithMany(x => x.RouteTeamTravelsCustomerBlockeds)
                .HasForeignKey(x => x.WorkDayId);

            modelBuilder.Entity<so_route_team_travels_customer_blocked>()
                .HasRequired(x => x.Inventory)
                .WithMany(x => x.RouteTeamTravelsCustomerBlockeds)
                .HasForeignKey(x => x.InventoryId);

            modelBuilder.Entity<so_route_team_travels_customer_blocked>()
                .HasRequired(x => x.Customer)
                .WithMany(x => x.RouteTeamTravelsCustomerBlockeds)
                .HasForeignKey(x => x.CustomerId);

            modelBuilder.Entity<so_leader_authorization_code>()
                .HasKey(x => x.Id)
                .Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            var authenticationLog = modelBuilder.Entity<so_authentication_log>();
            authenticationLog.HasKey(x => x.Id);
            authenticationLog.Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            authenticationLog.HasOptional(x => x.User)
                .WithMany(x => x.AuthenticationLogs)
                .HasForeignKey(x => x.UserId);
            authenticationLog.HasOptional(x => x.Route)
                .WithMany(x => x.AuthenticationLogs)
                .HasForeignKey(x => x.RouteId);
            authenticationLog.HasOptional(x => x.LeaderAuthorizationCode)
                .WithMany(x => x.AuthenticationLogs)
                .HasForeignKey(x => x.LeaderAuthenticationCodeId);

            var routeCustomerVario = modelBuilder.Entity<so_route_customer_vario>();
            routeCustomerVario.HasKey(x => x.Id);
            routeCustomerVario.Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            routeCustomerVario.HasRequired(x => x.Customer)
                .WithMany(x => x.RouteCustomerVario)
                .HasForeignKey(x => x.CustomerId);
            routeCustomerVario.HasRequired(x => x.Route)
                .WithMany(x => x.RouteCustomerVario)
                .HasForeignKey(x => x.RouteId);

            var saleAdditionalData = modelBuilder.Entity<so_sale_aditional_data>();
            saleAdditionalData.HasKey(x => x.saleAdicionalDataId);
            saleAdditionalData.Property(x => x.saleAdicionalDataId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            saleAdditionalData.HasRequired(x => x.so_sale)
                .WithMany(x => x.so_sale_aditional_data)
                .HasForeignKey(x => x.saleId);

            var deliveryStatus = modelBuilder.Entity<so_delivery_status>();
            deliveryStatus.HasKey(x => x.deliveryStatusId);
            deliveryStatus.Property(x => x.deliveryStatusId).IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<so_delivery>()
                .HasOptional(x => x.so_delivery_additional_data)
                .WithRequired(x => x.Delivery);

            var deliveryAdditionalData = modelBuilder.Entity<so_delivery_additional_data>();
            deliveryAdditionalData.HasKey(x => x.deliveryId);

            var liquidationLogStatus = modelBuilder.Entity<so_liquidation_log_status>();
            liquidationLogStatus.HasKey(x => x.Id);
            liquidationLogStatus.Property(x => x.Id).IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            var liquidationLog = modelBuilder.Entity<so_liquidation_log>();
            liquidationLog.HasKey(x => x.Id);
            liquidationLog.Property(x => x.Id).IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            liquidationLog.HasRequired(x => x.LiquidationStatus)
                .WithMany(x => x.LiquidationLogs)
                .HasForeignKey(x => x.LiquidationStatusId);

            modelBuilder.Entity<so_delivery>()
                .HasOptional(x => x.so_delivery_additional_data)
                .WithRequired(x => x.Delivery);

            modelBuilder.Entity<Configuracion_WorkByCloud>()
                .HasKey(x => x.wbcConfId);

            modelBuilder.Entity<so_promotion_type_catalog>()
                .HasKey(x => x.id);
        }
    }
}
