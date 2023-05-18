using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Text;
using SmartOrderService.DB;

namespace SmartOrderService.DB.Mappers
{
    public class DeliveryComboMapper : EntityTypeConfiguration<so_delivery_combo>
    {
        public DeliveryComboMapper()
        {
            this.ToTable("so_delivery_combo");
            this.HasKey(x => x.deliveryComboId);
            this.Property(x => x.deliveryComboId)
                .HasColumnName("deliveryComboId")
                .IsRequired();
            this.Property(x => x.deliveryId)
                .HasColumnName("deliveryId")
                .IsRequired();
            this.Property(x => x.promotionReferenceId)
                .HasColumnName("promotionReferenceId");
            this.Property(x => x.amount)
                .HasColumnName("amount")
                .IsRequired();
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
            this.HasRequired(x => x.so_delivery)
                .WithMany(x => x.so_delivery_combos)
                .HasForeignKey(d => d.deliveryId);
            this.HasMany(x => x.so_delivery_combo_details)
                .WithRequired(x => x.so_delivery_combo)
                .HasForeignKey(x => x.deliveryComboId);
        }
    }
}