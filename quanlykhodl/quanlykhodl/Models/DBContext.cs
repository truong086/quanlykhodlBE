﻿using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Numerics;
using System.Security.Principal;

namespace quanlykhodl.Models
{
	public class DBContext : DbContext
	{
		public DBContext(DbContextOptions options) : base(options) { }

		#region DBSet
		public DbSet<Account> accounts { get; set; }
		public DbSet<Area> areas { get; set; }
		public DbSet<category> categories { get; set; }
		public DbSet<Deliverynote> deliverynotes { get; set; }
		public DbSet<Floor> floors { get; set; }
		public DbSet<ImageProduct> imageProducts { get; set; }
		public DbSet<imagestatusitem> imagestatusitems { get; set; }
		public DbSet<Importform> importforms { get; set; }
		public DbSet<Plan> plans { get; set; }
		public DbSet<product> products1 { get; set; }
		public DbSet<productDeliverynote> productDeliverynotes { get; set; }
		public DbSet<productImportform> productImportforms { get; set; }
		public DbSet<Retailcustomers> Retailcustomers { get; set; }
		public DbSet<role> roles { get; set; }
		public DbSet<StatusItem> statusItems { get; set; }
		public DbSet<Supplier> suppliers { get; set; }
		public DbSet<Warehouse> warehouses { get; set; }
		public DbSet<Token> tokens { get; set; }
		public DbSet<Warehousetransferstatus> warehousetransferstatuses { get; set; }
		#endregion

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<category>()
				.HasMany(c => c.products)
				.WithOne(p => p.categoryid123) // Trường "categoryid" trong product liên kết đến id của category
				.HasForeignKey(p => p.category_map)
				.OnDelete(DeleteBehavior.Restrict); // Cấm xóa hoặc cập nhật dữ liệu liên quan

			modelBuilder.Entity<Token>()
				.HasOne(c => c.account)
				.WithMany(p => p.tokens) // Trường "categoryid" trong product liên kết đến id của category
				.HasForeignKey(p => p.account_id)
				.OnDelete(DeleteBehavior.Restrict); // Cấm xóa hoặc cập nhật dữ liệu liên quan

			modelBuilder.Entity<product>()
				.HasMany(c => c.productDeliverynotes)
				.WithOne(p => p.product) // Trường "categoryid" trong product liên kết đến id của category
				.HasForeignKey(p => p.product_map)
				 .OnDelete(DeleteBehavior.Restrict); // Cấm xóa hoặc cập nhật dữ liệu liên quan

			modelBuilder.Entity<product>()
				.HasMany(c => c.productImportforms)
				.WithOne(p => p.products) // Trường "categoryid" trong product liên kết đến id của category
				.HasForeignKey(p => p.product)
				.OnDelete(DeleteBehavior.Restrict); // Xóa liên quan khi sản phẩm bị xóa

			modelBuilder.Entity<product>()
				.HasMany(c => c.plans)
				.WithOne(p => p.productid123) // Trường "categoryid" trong product liên kết đến id của category
				.HasForeignKey(p => p.product_map)
				 .OnDelete(DeleteBehavior.Restrict);// Map cho trường "categoryId" trong product

			modelBuilder.Entity<Account>()
				.HasMany(c => c.areas)
				.WithOne(p => p.account_id) // Trường "categoryid" trong product liên kết đến id của category
				.HasForeignKey(p => p.account)
				 .OnDelete(DeleteBehavior.Restrict);// Map cho trường "categoryId" trong product

			modelBuilder.Entity<Account>()
				.HasMany(c => c.products)
				.WithOne(p => p.account) // Trường "categoryid" trong product liên kết đến id của category
				.HasForeignKey(p => p.account_map)
				 .OnDelete(DeleteBehavior.Restrict);// Map cho trường "categoryId" trong product

            modelBuilder.Entity<Account>()
                .HasMany(c => c.categories)
                .WithOne(p => p.account) // Trường "categoryid" trong product liên kết đến id của category
                .HasForeignKey(p => p.account_Id)
                 .OnDelete(DeleteBehavior.Restrict);// Map cho trường "categoryId" trong product

            modelBuilder.Entity<Account>()
                .HasMany(c => c.suppliers)
                .WithOne(p => p.accounts) // Trường "categoryid" trong product liên kết đến id của category
                .HasForeignKey(p => p.account_id)
                 .OnDelete(DeleteBehavior.Restrict);// Map cho trường "categoryId" trong product

            modelBuilder.Entity<Account>()
                .HasMany(c => c.floors)
                .WithOne(p => p.accounts) // Trường "categoryid" trong product liên kết đến id của category
                .HasForeignKey(p => p.account_id)
                 .OnDelete(DeleteBehavior.Restrict);// Map cho trường "categoryId" trong product

            modelBuilder.Entity<Account>()
                .HasMany(c => c.warehouses)
                .WithOne(p => p.account) // Trường "categoryid" trong product liên kết đến id của category
                .HasForeignKey(p => p.account_map)
                 .OnDelete(DeleteBehavior.Restrict);// Map cho trường "categoryId" trong product

            modelBuilder.Entity<Account>()
				.HasMany(c => c.importforms)
				.WithOne(p => p.account_id) // Trường "categoryid" trong product liên kết đến id của category
				.HasForeignKey(p => p.account_idMap)
				 .OnDelete(DeleteBehavior.Restrict);// Map cho trường "categoryId" trong product

			modelBuilder.Entity<Account>()
				.HasMany(c => c.deliverynotes)
				.WithOne(p => p.account) // Trường "categoryid" trong product liên kết đến id của category
				.HasForeignKey(p => p.accountmap)
				 .OnDelete(DeleteBehavior.Restrict);// Map cho trường "categoryId" trong product

			modelBuilder.Entity<Account>()
				.HasMany(c => c.plans)
				.WithOne(p => p.Receiver_id) // Trường "categoryid" trong product liên kết đến id của category
				.HasForeignKey(p => p.Receiver)
				 .OnDelete(DeleteBehavior.Restrict);// Map cho trường "categoryId" trong product

			modelBuilder.Entity<Area>()
				.HasMany(c => c.products)
				.WithOne(p => p.area_id) // Trường "categoryid" trong product liên kết đến id của category
				.HasForeignKey(p => p.area_map)
				 .OnDelete(DeleteBehavior.Restrict);// Map cho trường "categoryId" trong product

			modelBuilder.Entity<Deliverynote>()
				.HasMany(c => c.productDeliverynotes)
				.WithOne(p => p.deliverynote_id1) // Trường "categoryid" trong product liên kết đến id của category
				.HasForeignKey(p => p.deliverynote)
				 .OnDelete(DeleteBehavior.Restrict);// Map cho trường "categoryId" trong product

			modelBuilder.Entity<Deliverynote>()
				.HasMany(c => c.retailcustomers)
				.WithOne(p => p.deliverynote_id1) // Trường "categoryid" trong product liên kết đến id của category
				.HasForeignKey(p => p.deliverynote)
				 .OnDelete(DeleteBehavior.Restrict);// Map cho trường "categoryId" trong product

			modelBuilder.Entity<Floor>()
				.HasMany(c => c.products)
				.WithOne(p => p.floor_id1) // Trường "categoryid" trong product liên kết đến id của category
				.HasForeignKey(p => p.floor_map)
				 .OnDelete(DeleteBehavior.Restrict);// Map cho trường "categoryId" trong product

			modelBuilder.Entity<Floor>()
				.HasMany(c => c.areas)
				.WithOne(p => p.floor_id) // Trường "categoryid" trong product liên kết đến id của category
				.HasForeignKey(p => p.floor)
				 .OnDelete(DeleteBehavior.Restrict);// Map cho trường "categoryId" trong product

			modelBuilder.Entity<Importform>()
				.HasMany(c => c.productImportforms)
				.WithOne(p => p.importform_id1) // Trường "categoryid" trong product liên kết đến id của category
				.HasForeignKey(p => p.importform)
				 .OnDelete(DeleteBehavior.Restrict);// Map cho trường "categoryId" trong product

			modelBuilder.Entity<Plan>()
				.HasMany(c => c.warehousetransferstatuses)
				.WithOne(p => p.plan_id) // Trường "categoryid" trong product liên kết đến id của category
				.HasForeignKey(p => p.plan)
				 .OnDelete(DeleteBehavior.Restrict);// Map cho trường "categoryId" trong product

			modelBuilder.Entity<role>()
				.HasMany(c => c.account)
				.WithOne(p => p.role) // Trường "categoryid" trong product liên kết đến id của category
				.HasForeignKey(p => p.role_id)
				.OnDelete(DeleteBehavior.Restrict); // Xóa liên quan khi sản phẩm bị xóa

			modelBuilder.Entity<StatusItem>()
				.HasMany(c => c.imagestatusitems)
				.WithOne(p => p.statusItem_id) // Trường "categoryid" trong product liên kết đến id của category
				.HasForeignKey(p => p.statusItemMap)
				.OnDelete(DeleteBehavior.Restrict); // Xóa liên quan khi sản phẩm bị xóa

			modelBuilder.Entity<Supplier>()
				.HasMany(c => c.importforms)
				.WithOne(p => p.supplier_id) // Trường "categoryid" trong product liên kết đến id của category
				.HasForeignKey(p => p.supplier)
				.OnDelete(DeleteBehavior.Restrict); // Xóa liên quan khi sản phẩm bị xóa

			modelBuilder.Entity<Supplier>()
				.HasMany(c => c.products)
				.WithOne(p => p.supplier_id) // Trường "categoryid" trong product liên kết đến id của category
				.HasForeignKey(p => p.suppliers)
				.OnDelete(DeleteBehavior.Restrict); // Xóa liên quan khi sản phẩm bị xóa

			modelBuilder.Entity<Warehouse>()
				.HasMany(c => c.floors)
				.WithOne(p => p.warehouse_id) // Trường "categoryid" trong product liên kết đến id của category
				.HasForeignKey(p => p.warehouse)
				.OnDelete(DeleteBehavior.Restrict); // Xóa liên quan khi sản phẩm bị xóa

			modelBuilder.Entity<Warehousetransferstatus>()
				.HasMany(c => c.statusItems)
				.WithOne(p => p.Warehousetransferstatus_id) // Trường "categoryid" trong product liên kết đến id của category
				.HasForeignKey(p => p.warehousetransferstatus)
				.OnDelete(DeleteBehavior.Restrict); // Xóa liên quan khi sản phẩm bị xóa
		}
	}
}