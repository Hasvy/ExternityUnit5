﻿using ExtUnit5.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExtUnit5.EntitiesConfigurations
{
    public class ProductConfigurationcs : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasOne(p => p.Category)
                .WithMany();
                //.HasForeignKey(c => c.Id);
        }
    }
}
