using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Numerics;
using System.Security.Principal;

namespace quanlykhodl.Models
{
	public class DBContext : DbContext
	{
		public DBContext(DbContextOptions<DBContext> options) : base(options) { }

		#region DBSet
		public DbSet<accounts> accounts { get; set; }
		public DbSet<Shelf> shelfs { get; set; }
		public DbSet<areas> areas { get; set; }
		public DbSet<Codelocation> codelocations { get; set; }
		public DbSet<categories> categories { get; set; }
		public DbSet<Deliverynote> deliverynotes { get; set; }
		public DbSet<Floor> floors { get; set; }
		public DbSet<ImageProduct> imageproducts { get; set; }
		public DbSet<LocationException> locationexceptions { get; set; }
		public DbSet<imagestatusitem> imagestatusitems { get; set; }
		//public DbSet<PrepareToExport> prepareToExports { get; set; }
		public DbSet<Importform> importforms { get; set; }
		public DbSet<Plan> plans { get; set; }
		public DbSet<product> products1 { get; set; }
		public DbSet<productDeliverynote> productdeliverynotes { get; set; }
		public DbSet<DeliverynotePrepareToExport> deliverynotepreparetoes { get; set; }
		public DbSet<productImportform> productimportforms { get; set; }
		public DbSet<Retailcustomers> retailcustomers { get; set; }
		public DbSet<role> roles { get; set; }
		public DbSet<StatusItem> statusitems { get; set; }
		public DbSet<Supplier> suppliers { get; set; }
		public DbSet<Warehouse> warehouses { get; set; }
		public DbSet<Token> tokens { get; set; }
		public DbSet<Warehousetransferstatus> warehousetransferstatuses { get; set; }
		public DbSet<productlocation> productlocations { get; set; }
        public DbSet<Message> messages { get; set; }
        public DbSet<Conversation> conversations { get; set; }
        public DbSet<UserConversation> userconversations { get; set; }
        public DbSet<OnlineUsers> onlineusersuser { get; set; }
        public DbSet<UserTokenApp> usertokenapps { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
            modelBuilder.Entity<UserConversation>()
				.HasOne(uc => uc.User)
				.WithMany()
				.HasForeignKey(uc => uc.userid)
				.OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserConversation>()
                .HasOne(uc => uc.Conversation)
                .WithMany()
                .HasForeignKey(uc => uc.conversationid)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Message>()
			   .HasOne(m => m.Sender)
			   .WithMany(u => u.SentMessages)
			   .HasForeignKey(m => m.senderid)
			   .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Receiver)
                .WithMany(u => u.ReceivedMessages)
                .HasForeignKey(m => m.receiverid)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Conversation>()
			   .HasOne(c => c.User1)
			   .WithMany() // "WithMany()" để rỗng là không ánh xạ ngược
               .HasForeignKey(c => c.user1id)
			   .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Conversation>()
                .HasOne(c => c.User2)
                .WithMany()
                .HasForeignKey(c => c.user2id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Conversation>()
                .HasOne(c => c.LastMessage)
                .WithMany()
                .HasForeignKey(c => c.lastmessageid)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<categories>()
				.HasMany(c => c.products)
				.WithOne(p => p.categoryid123) // Trường "categoryid" trong product liên kết đến id của categories
				.HasForeignKey(p => p.category_map)
				.OnDelete(DeleteBehavior.Restrict); // Cấm xóa hoặc cập nhật dữ liệu liên quan

			modelBuilder.Entity<Token>()
				.HasOne(c => c.account)
				.WithMany(p => p.tokens) // Trường "categoryid" trong product liên kết đến id của categories
				.HasForeignKey(p => p.account_id)
				.OnDelete(DeleteBehavior.Restrict); // Cấm xóa hoặc cập nhật dữ liệu liên quan

			modelBuilder.Entity<product>()
				.HasMany(c => c.Productlocations)
				.WithOne(p => p.products) // Trường "categoryid" trong product liên kết đến id của categories
				.HasForeignKey(p => p.id_product)
				 .OnDelete(DeleteBehavior.Restrict); // Cấm xóa hoặc cập nhật dữ liệu liên quan

            modelBuilder.Entity<product>()
                .HasMany(c => c.imageProducts)
                .WithOne(p => p.products_id) // Trường "categoryid" trong product liên kết đến id của categories
                .HasForeignKey(p => p.productmap)
                 .OnDelete(DeleteBehavior.Restrict); // Cấm xóa hoặc cập nhật dữ liệu liên quan

            modelBuilder.Entity<PrepareToExport>()
                .HasMany(c => c.DeliverynotePrepareToExports)
                .WithOne(p => p.PreparetoExports) // Trường "categoryid" trong product liên kết đến id của categories
                .HasForeignKey(p => p.id_preparetoexport)
                 .OnDelete(DeleteBehavior.Restrict); // Cấm xóa hoặc cập nhật dữ liệu liên quan

            modelBuilder.Entity<product>()
				.HasMany(c => c.productImportforms)
				.WithOne(p => p.products) // Trường "categoryid" trong product liên kết đến id của categories
				.HasForeignKey(p => p.product)
				.OnDelete(DeleteBehavior.Restrict); // Xóa liên quan khi sản phẩm bị xóa

            modelBuilder.Entity<product>()
                .HasMany(c => c.productDeliverynotes)
                .WithOne(p => p.product) // Trường "categoryid" trong product liên kết đến id của categories
                .HasForeignKey(p => p.product_map)
                .OnDelete(DeleteBehavior.Restrict); // Xóa liên quan khi sản phẩm bị xóa

            modelBuilder.Entity<productlocation>()
				.HasMany(c => c.plans)
				.WithOne(p => p.productidlocation) // Trường "categoryid" trong product liên kết đến id của categories
				.HasForeignKey(p => p.productlocation_map)
				 .OnDelete(DeleteBehavior.Restrict);// Map cho trường "categoryId" trong product

            modelBuilder.Entity<productlocation>()
                .HasMany(c => c.ProductDeliverynotes)
                .WithOne(p => p.productlocations) // Trường "categoryid" trong product liên kết đến id của categories
                .HasForeignKey(p => p.productlocation_id)
                 .OnDelete(DeleteBehavior.Restrict);// Map cho trường "categoryId" trong product

            modelBuilder.Entity<productlocation>()
                .HasMany(c => c.ProductImportforms)
                .WithOne(p => p.productlocations) // Trường "categoryid" trong product liên kết đến id của categories
                .HasForeignKey(p => p.productlocation_id)
                 .OnDelete(DeleteBehavior.Restrict);// Map cho trường "categoryId" trong product

            modelBuilder.Entity<accounts>()
				.HasMany(c => c.shelfs)
				.WithOne(p => p.account_id) // Trường "categoryid" trong product liên kết đến id của categories
				.HasForeignKey(p => p.account)
				 .OnDelete(DeleteBehavior.Restrict);// Map cho trường "categoryId" trong product

			modelBuilder.Entity<accounts>()
				.HasMany(c => c.products)
				.WithOne(p => p.account) // Trường "categoryid" trong product liên kết đến id của categories
				.HasForeignKey(p => p.account_map)
				 .OnDelete(DeleteBehavior.Restrict);// Map cho trường "categoryId" trong product

            modelBuilder.Entity<accounts>()
                .HasMany(c => c.areas)
                .WithOne(p => p.account) // Trường "categoryid" trong product liên kết đến id của categories
                .HasForeignKey(p => p.account_id)
                 .OnDelete(DeleteBehavior.Restrict);// Map cho trường "categoryId" trong product

            modelBuilder.Entity<accounts>()
                .HasMany(c => c.categories)
                .WithOne(p => p.account) // Trường "categoryid" trong product liên kết đến id của categories
                .HasForeignKey(p => p.account_id)
                 .OnDelete(DeleteBehavior.Restrict);// Map cho trường "categoryId" trong product

            modelBuilder.Entity<accounts>()
                .HasMany(c => c.suppliers)
                .WithOne(p => p.accounts) // Trường "categoryid" trong product liên kết đến id của categories
                .HasForeignKey(p => p.account_id)
                 .OnDelete(DeleteBehavior.Restrict);// Map cho trường "categoryId" trong product

            modelBuilder.Entity<accounts>()
                .HasMany(c => c.floors)
                .WithOne(p => p.accounts) // Trường "categoryid" trong product liên kết đến id của categories
                .HasForeignKey(p => p.account_id)
                 .OnDelete(DeleteBehavior.Restrict);// Map cho trường "categoryId" trong product

            modelBuilder.Entity<accounts>()
                .HasMany(c => c.warehouses)
                .WithOne(p => p.account) // Trường "categoryid" trong product liên kết đến id của categories
                .HasForeignKey(p => p.account_map)
                 .OnDelete(DeleteBehavior.Restrict);// Map cho trường "categoryId" trong product

            modelBuilder.Entity<accounts>()
				.HasMany(c => c.importforms)
				.WithOne(p => p.account_id) // Trường "categoryid" trong product liên kết đến id của categories
				.HasForeignKey(p => p.account_idmap)
				 .OnDelete(DeleteBehavior.Restrict);// Map cho trường "categoryId" trong product

			modelBuilder.Entity<accounts>()
				.HasMany(c => c.deliverynotes)
				.WithOne(p => p.account) // Trường "categoryid" trong product liên kết đến id của categories
				.HasForeignKey(p => p.accountmap)
				 .OnDelete(DeleteBehavior.Restrict);// Map cho trường "categoryId" trong product

			modelBuilder.Entity<accounts>()
				.HasMany(c => c.plans)
				.WithOne(p => p.Receiver_id) // Trường "categoryid" trong product liên kết đến id của categories
				.HasForeignKey(p => p.Receiver)
				 .OnDelete(DeleteBehavior.Restrict);// Map cho trường "categoryId" trong product

            modelBuilder.Entity<accounts>()
                .HasMany(c => c.onlineUsers)
                .WithOne(p => p.account) // Trường "categoryid" trong product liên kết đến id của categories
                .HasForeignKey(p => p.account_id)
                 .OnDelete(DeleteBehavior.Restrict);// Map cho trường "categoryId" trong product

            modelBuilder.Entity<areas>()
                .HasMany(c => c.plans)
                .WithOne(p => p.area_id) // Trường "categoryid" trong product liên kết đến id của categories
                .HasForeignKey(p => p.area)
                 .OnDelete(DeleteBehavior.Restrict);// Map cho trường "categoryId" trong product

            modelBuilder.Entity<Shelf>()
				.HasMany(c => c.Productlocations)
				.WithOne(p => p.shelfs) // Trường "categoryid" trong product liên kết đến id của categories
				.HasForeignKey(p => p.id_shelf)
				 .OnDelete(DeleteBehavior.Restrict);// Map cho trường "categoryId" trong product

            modelBuilder.Entity<Shelf>()
                .HasMany(c => c.LocationExceptions)
                .WithOne(p => p.shelf) // Trường "categoryid" trong product liên kết đến id của categories
                .HasForeignKey(p => p.id_shelf)
                 .OnDelete(DeleteBehavior.Restrict);// Map cho trường "categoryId" trong product

            modelBuilder.Entity<Shelf>()
                .HasMany(c => c.Codelocations)
                .WithOne(p => p.shelf) // Trường "categoryid" trong product liên kết đến id của categories
                .HasForeignKey(p => p.id_helf)
                 .OnDelete(DeleteBehavior.Restrict);// Map cho trường "categoryId" trong product

            modelBuilder.Entity<Shelf>()
                .HasMany(c => c.ProductImportforms)
                .WithOne(p => p.shelfs) // Trường "categoryid" trong product liên kết đến id của categories
                .HasForeignKey(p => p.shelf_id)
                 .OnDelete(DeleteBehavior.Restrict);// Map cho trường "categoryId" trong product

            modelBuilder.Entity<areas>()
                .HasMany(c => c.shelfsl)
                .WithOne(p => p.area_id) // Trường "categoryid" trong product liên kết đến id của categories
                .HasForeignKey(p => p.area)
                 .OnDelete(DeleteBehavior.Restrict);// Map cho trường "categoryId" trong product

            modelBuilder.Entity<Deliverynote>()
				.HasMany(c => c.productDeliverynotes)
				.WithOne(p => p.deliverynote_id1) // Trường "categoryid" trong product liên kết đến id của categories
				.HasForeignKey(p => p.deliverynote)
				 .OnDelete(DeleteBehavior.Restrict);// Map cho trường "categoryId" trong product

			//modelBuilder.Entity<Deliverynote>()
			//    .HasMany(c => c.deliverynotePrepareToExports)
			//    .WithOne(p => p.deliverynotes) // Trường "categoryid" trong product liên kết đến id của categories
			//    .HasForeignKey(p => p.id_delivenote)
			//     .OnDelete(DeleteBehavior.Restrict);// Map cho trường "categoryId" trong product

			modelBuilder.Entity<Retailcustomers>()
				.HasMany(c => c.deliverynotes)
				.WithOne(p => p.retailcustomers_id) // Trường "categoryid" trong product liên kết đến id của categories
				.HasForeignKey(p => p.retailcustomers)
				 .OnDelete(DeleteBehavior.Restrict);// Map cho trường "categoryId" trong product

			modelBuilder.Entity<Floor>()
				.HasMany(c => c.areas)
				.WithOne(p => p.floor_id) // Trường "categoryid" trong product liên kết đến id của categories
				.HasForeignKey(p => p.floor)
				 .OnDelete(DeleteBehavior.Restrict);// Map cho trường "categoryId" trong product

			modelBuilder.Entity<Importform>()
				.HasMany(c => c.productImportforms)
				.WithOne(p => p.importform_id1) // Trường "categoryid" trong product liên kết đến id của categories
				.HasForeignKey(p => p.importform)
				 .OnDelete(DeleteBehavior.Restrict);// Map cho trường "categoryId" trong product

			modelBuilder.Entity<Plan>()
				.HasMany(c => c.warehousetransferstatuses)
				.WithOne(p => p.plan_id) // Trường "categoryid" trong product liên kết đến id của categories
				.HasForeignKey(p => p.plan)
				 .OnDelete(DeleteBehavior.Restrict);// Map cho trường "categoryId" trong product

			modelBuilder.Entity<role>()
				.HasMany(c => c.account)
				.WithOne(p => p.role) // Trường "categoryid" trong product liên kết đến id của categories
				.HasForeignKey(p => p.role_id)
				.OnDelete(DeleteBehavior.Restrict); // Xóa liên quan khi sản phẩm bị xóa

			modelBuilder.Entity<StatusItem>()
				.HasMany(c => c.imagestatusitems)
				.WithOne(p => p.statusItem_id) // Trường "categoryid" trong product liên kết đến id của categories
				.HasForeignKey(p => p.statusitemmap)
				.OnDelete(DeleteBehavior.Restrict); // Xóa liên quan khi sản phẩm bị xóa

			modelBuilder.Entity<Supplier>()
				.HasMany(c => c.productimportforms)
				.WithOne(p => p.supplier_id) // Trường "categoryid" trong product liên kết đến id của categories
				.HasForeignKey(p => p.supplier)
				.OnDelete(DeleteBehavior.Restrict); // Xóa liên quan khi sản phẩm bị xóa

			modelBuilder.Entity<Supplier>()
				.HasMany(c => c.products)
				.WithOne(p => p.supplier_id) // Trường "categoryid" trong product liên kết đến id của categories
				.HasForeignKey(p => p.suppliers)
				.OnDelete(DeleteBehavior.Restrict); // Xóa liên quan khi sản phẩm bị xóa

			modelBuilder.Entity<Warehouse>()
				.HasMany(c => c.floors)
				.WithOne(p => p.warehouse_id) // Trường "categoryid" trong product liên kết đến id của categories
				.HasForeignKey(p => p.warehouse)
				.OnDelete(DeleteBehavior.Restrict); // Xóa liên quan khi sản phẩm bị xóa

			modelBuilder.Entity<Warehousetransferstatus>()
				.HasMany(c => c.statusItems)
				.WithOne(p => p.Warehousetransferstatus_id) // Trường "categoryid" trong product liên kết đến id của categories
				.HasForeignKey(p => p.warehousetransferstatus)
				.OnDelete(DeleteBehavior.Restrict); // Xóa liên quan khi sản phẩm bị xóa
        }
	}
}