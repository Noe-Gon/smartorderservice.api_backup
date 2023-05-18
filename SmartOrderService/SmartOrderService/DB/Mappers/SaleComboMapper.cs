using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Text;
using SmartOrderService.DB;

namespace SmartOrderService.DB.Mappers
{
    public class SaleComboMapper : EntityTypeConfiguration<so_sale_combo>
    {
        public SaleComboMapper()
        {
            this.ToTable("so_sale_combo");
            this.HasKey(x => x.saleComboId);
            this.Property(x => x.saleComboId)
                .HasColumnName("saleComboId")
                .IsRequired();
            this.Property(x => x.saleId)
                .HasColumnName("saleId")
                .IsRequired();
            this.Property(x => x.amount)
                .HasColumnName("amount")
                .IsRequired();
            this.Property(x => x.promotionReferenceId)
                .HasColumnName("promotionReferenceId");
            this.Property(x => x.code)
                .HasColumnName("code")
                .IsRequired();
            this.Property(x => x.name)
                .HasColumnName("name")
                .IsRequired();
            this.Property(x => x.createdby)
                .HasColumnName("createdby")
                .IsRequired();
            this.Property(x => x.createdon)
                .HasColumnName("createdon")
                .IsRequired();
            this.Property(x => x.modifiedby)
                .HasColumnName("modifiedby")
                .IsRequired();
            this.Property(x => x.modifiedby)
                .HasColumnName("modifiedby")
                .IsRequired();
            this.HasRequired(x => x.so_sale)
                .WithMany(x => x.so_sale_combos)
                .HasForeignKey(d => d.saleId);
            this.HasMany(x => x.so_sale_combo_details)
                .WithRequired(x => x.so_sale_combo)
                .HasForeignKey(x => x.saleComboId);
        }
    }
}