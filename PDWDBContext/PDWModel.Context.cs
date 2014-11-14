﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Objects;

namespace PDWDBContext
{
    public partial class PaoliPDWEntities : DbContext
    {
        public PaoliPDWEntities()
            : base("name=PaoliPDWEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<AttributeOption> AttributeOptions { get; set; }
        public DbSet<Attribute> Attributes { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Series> Serieses { get; set; }
        public DbSet<SeriesIntAttribute> SeriesIntAttributes { get; set; }
        public DbSet<SeriesOptionAttribute> SeriesOptionAttributes { get; set; }
        public DbSet<SeriesTextAttribute> SeriesTextAttributes { get; set; }
        public DbSet<ImageFile> ImageFiles { get; set; }
        public DbSet<SeriesImageFile> SeriesImageFiles { get; set; }
        public DbSet<TAttributeOption> TAttributeOptions { get; set; }
        public DbSet<TAttribute> TAttributes { get; set; }
        public DbSet<TypicalImageFile> TypicalImageFiles { get; set; }
        public DbSet<TypicalIntAttribute> TypicalIntAttributes { get; set; }
        public DbSet<TypicalOptionAttribute> TypicalOptionAttributes { get; set; }
        public DbSet<Typical> Typicals { get; set; }
        public DbSet<TypicalTextAttribute> TypicalTextAttributes { get; set; }
        public DbSet<SeriesTypical> SeriesTypicals { get; set; }
        public DbSet<SearchResultsLog> SearchResultsLogs { get; set; }
        public DbSet<PublicationImage> PublicationImages { get; set; }
        public DbSet<Publication> Publications { get; set; }
        public DbSet<Dealer> Dealers { get; set; }
        public DbSet<PageLink> PageLinks { get; set; }
        public DbSet<VideoLink> VideoLinks { get; set; }
        public DbSet<DealerFeaturedProduct> DealerFeaturedProducts { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<SpecRequestFile> SpecRequestFiles { get; set; }
        public DbSet<SpecRequest> SpecRequests { get; set; }
        public DbSet<Territory> Territories { get; set; }
        public DbSet<UserSubscription> UserSubscriptions { get; set; }
        public DbSet<Showroom> Showrooms { get; set; }
        public DbSet<ZipCodeInfo> ZipCodeInfoes { get; set; }
        public DbSet<CollateralItem> CollateralItems { get; set; }
        public DbSet<CollateralType> CollateralTypes { get; set; }
        public DbSet<CollateralGroupItem> CollateralGroupItems { get; set; }
        public DbSet<CollateralOrderDetail> CollateralOrderDetails { get; set; }
        public DbSet<CollateralOrder> CollateralOrders { get; set; }
        public DbSet<CollateralOrderShipmentDetail> CollateralOrderShipmentDetails { get; set; }
        public DbSet<CollateralOrderShipment> CollateralOrderShipments { get; set; }
        public DbSet<GSAContract> GSAContracts { get; set; }
        public DbSet<UserLogin> UserLogins { get; set; }
        public DbSet<HomePageContent> HomePageContents { get; set; }
        public DbSet<ScoopArticle> ScoopArticles { get; set; }
        public DbSet<eCollateralItem> eCollateralItems { get; set; }
        public DbSet<eCollateralSection> eCollateralSections { get; set; }
        public DbSet<SpecRequestEvent> SpecRequestEvents { get; set; }
        public DbSet<LeadTimeDetail> LeadTimeDetails { get; set; }
    
        public virtual ObjectResult<Showroom> ClosestShowroom(Nullable<double> lat, Nullable<double> @long)
        {
            ((IObjectContextAdapter)this).ObjectContext.MetadataWorkspace.LoadFromAssembly(typeof(Showroom).Assembly);
    
            var latParameter = lat.HasValue ?
                new ObjectParameter("lat", lat) :
                new ObjectParameter("lat", typeof(double));
    
            var longParameter = @long.HasValue ?
                new ObjectParameter("long", @long) :
                new ObjectParameter("long", typeof(double));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Showroom>("ClosestShowroom", latParameter, longParameter);
        }
    
        public virtual ObjectResult<Showroom> ClosestShowroom(Nullable<double> lat, Nullable<double> @long, MergeOption mergeOption)
        {
            ((IObjectContextAdapter)this).ObjectContext.MetadataWorkspace.LoadFromAssembly(typeof(Showroom).Assembly);
    
            var latParameter = lat.HasValue ?
                new ObjectParameter("lat", lat) :
                new ObjectParameter("lat", typeof(double));
    
            var longParameter = @long.HasValue ?
                new ObjectParameter("long", @long) :
                new ObjectParameter("long", typeof(double));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Showroom>("ClosestShowroom", mergeOption, latParameter, longParameter);
        }
    }
}