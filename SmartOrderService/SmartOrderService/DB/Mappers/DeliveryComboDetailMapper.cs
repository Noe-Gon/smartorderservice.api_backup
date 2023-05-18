using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Text;
using SmartOrderService.DB;

namespace SmartOrderService.DB.Mappers
{
    public class DeliveryComboDetailMapper : EntityTypeConfiguration<so_delivery_combo_detail>
    {
        public DeliveryComboDetailMapper()
        {
            this.ToTable("so_delivery_combo_detail"); 
            this.HasKey(x => x.deliveryComboDetailId);
            this.Property(x => x.deliveryComboDetailId)
                .HasColumnName("deliveryComboDetailId")
                .IsRequired();
            this.Property(x => x.deliveryComboId)
                .HasColumnName("deliveryComboId")
                .IsRequired();
            this.Property(x => x.amount)
                .HasColumnName("amount")
                .IsRequired();
            this.Property(x => x.productId)
                .HasColumnName("productId")
                .IsRequired();
            this.Property(x => x.is_gift)
                .HasColumnName("is_gift")
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
            this.HasRequired(x => x.so_delivery_combo)
                .WithMany(x => x.so_delivery_combo_details)
                .HasForeignKey(d => d.deliveryComboId);
            this.HasRequired(x => x.so_product)
                .WithMany(x => x.so_delivery_combo_details)
                .HasForeignKey(d => d.productId);
        }
    }
}