﻿// <auto-generated />
using System;
using DDDEfCore.ProductCatalog.Infrastructure.EfCore.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DDDEfCore.ProductCatalog.Infrastructure.EfCore.Migrations
{
    [DbContext(typeof(ProductCatalogDbContext))]
    partial class ProductCatalogDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs.Catalog", b =>
                {
                    b.Property<Guid>("CatalogId")
                        .HasColumnName("Id");

                    b.Property<string>("DisplayName");

                    b.HasKey("CatalogId");

                    b.ToTable("Catalog");
                });

            modelBuilder.Entity("DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs.CatalogCategory", b =>
                {
                    b.Property<Guid>("CatalogCategoryId");

                    b.Property<Guid?>("CatalogCategoryParentId");

                    b.Property<Guid>("CatalogId");

                    b.Property<Guid>("CategoryId");

                    b.Property<string>("DisplayName");

                    b.HasKey("CatalogCategoryId");

                    b.HasIndex("CatalogCategoryParentId");

                    b.HasIndex("CatalogId");

                    b.ToTable("CatalogCategory");
                });

            modelBuilder.Entity("DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs.CatalogProduct", b =>
                {
                    b.Property<Guid>("CatalogProductId")
                        .HasColumnName("Id");

                    b.Property<Guid>("CatalogCategoryId");

                    b.Property<string>("DisplayName");

                    b.Property<Guid>("ProductId");

                    b.HasKey("CatalogProductId");

                    b.HasIndex("CatalogCategoryId");

                    b.ToTable("CatalogProduct");
                });

            modelBuilder.Entity("DDDEfCore.ProductCatalog.Core.DomainModels.Categories.Category", b =>
                {
                    b.Property<Guid>("CategoryId")
                        .HasColumnName("Id");

                    b.Property<string>("DisplayName");

                    b.HasKey("CategoryId");

                    b.ToTable("Category");
                });

            modelBuilder.Entity("DDDEfCore.ProductCatalog.Core.DomainModels.Products.Product", b =>
                {
                    b.Property<Guid>("ProductId")
                        .HasColumnName("Id");

                    b.Property<string>("Name");

                    b.HasKey("ProductId");

                    b.ToTable("Product");
                });

            modelBuilder.Entity("DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs.CatalogCategory", b =>
                {
                    b.HasOne("DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs.CatalogCategory", "Parent")
                        .WithMany()
                        .HasForeignKey("CatalogCategoryParentId");

                    b.HasOne("DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs.Catalog")
                        .WithMany("Categories")
                        .HasForeignKey("CatalogId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs.CatalogProduct", b =>
                {
                    b.HasOne("DDDEfCore.ProductCatalog.Core.DomainModels.Catalogs.CatalogCategory", "CatalogCategory")
                        .WithMany("Products")
                        .HasForeignKey("CatalogCategoryId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
