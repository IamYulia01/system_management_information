using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace system_management_information;

public partial class VisitCenterContext : DbContext
{
    public VisitCenterContext()
    {
    }

    public VisitCenterContext(DbContextOptions<VisitCenterContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Catering> Caterings { get; set; }

    public virtual DbSet<CateringModeOperationCatering> CateringModeOperationCaterings { get; set; }

    public virtual DbSet<CateringTypeKitchen> CateringTypeKitchens { get; set; }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<Hotel> Hotels { get; set; }

    public virtual DbSet<ModeOperationCatering> ModeOperationCaterings { get; set; }

    public virtual DbSet<OperatingMode> OperatingModes { get; set; }

    public virtual DbSet<PhotoHotel> PhotoHotels { get; set; }

    public virtual DbSet<PhotoSight> PhotoSights { get; set; }

    public virtual DbSet<Route> Routes { get; set; }

    public virtual DbSet<RouteCateringHotel> RouteCateringHotels { get; set; }

    public virtual DbSet<RouteEventSight> RouteEventSights { get; set; }

    public virtual DbSet<Sight> Sights { get; set; }

    public virtual DbSet<SightOperatingMode> SightOperatingModes { get; set; }

    public virtual DbSet<Souvenir> Souvenirs { get; set; }

    public virtual DbSet<SpecialDayCatering> SpecialDayCaterings { get; set; }

    public virtual DbSet<SpecialDaySight> SpecialDaySights { get; set; }

    public virtual DbSet<Ticket> Tickets { get; set; }

    public virtual DbSet<TypeKitchen> TypeKitchens { get; set; }

    public virtual DbSet<Userbot> Userbots { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=VisitCenter;Username=postgres;Password=kasko");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Catering>(entity =>
        {
            entity.HasKey(e => e.IdCatering).HasName("catering_pkey");

            entity.ToTable("catering");

            entity.Property(e => e.IdCatering).HasColumnName("id_catering");
            entity.Property(e => e.CateringUrl)
                .HasMaxLength(100)
                .HasColumnName("catering_url");
            entity.Property(e => e.EstablishmentCategory)
                .HasMaxLength(50)
                .HasColumnName("establishment_category");
            entity.Property(e => e.EstablishmentHouse)
                .HasMaxLength(10)
                .HasColumnName("establishment_house");
            entity.Property(e => e.EstablishmentName)
                .HasMaxLength(100)
                .HasColumnName("establishment_name");
            entity.Property(e => e.EstablishmentPhone)
                .HasMaxLength(15)
                .HasColumnName("establishment_phone");
            entity.Property(e => e.EstablishmentStreet)
                .HasMaxLength(50)
                .HasColumnName("establishment_street");
        });

        modelBuilder.Entity<CateringModeOperationCatering>(entity =>
        {
            entity.HasKey(e => e.IdCateringModeOperationCatering).HasName("catering_mode_operation_catering_pkey");

            entity.ToTable("catering_mode_operation_catering");

            entity.HasIndex(e => e.IdCatering, "IX_catering_mode_operation_catering_id_catering");

            entity.HasIndex(e => e.IdModeOperationCatering, "IX_catering_mode_operation_catering_id_mode_operation_catering");

            entity.Property(e => e.IdCateringModeOperationCatering)
                .HasDefaultValueSql("nextval('catering_mode_operation_catering_id_catering_mode_operation_cat'::regclass)")
                .HasColumnName("id_catering_mode_operation_catering");
            entity.Property(e => e.DayWeek).HasColumnName("day_week");
            entity.Property(e => e.IdCatering).HasColumnName("id_catering");
            entity.Property(e => e.IdModeOperationCatering).HasColumnName("id_mode_operation_catering");
            entity.Property(e => e.IdSpecialDayCatering).HasColumnName("id_special_day_catering");

            entity.HasOne(d => d.IdCateringNavigation).WithMany(p => p.CateringModeOperationCaterings)
                .HasForeignKey(d => d.IdCatering)
                .HasConstraintName("fk_catering_mode_operation_catering");

            entity.HasOne(d => d.IdModeOperationCateringNavigation).WithMany(p => p.CateringModeOperationCaterings)
                .HasForeignKey(d => d.IdModeOperationCatering)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_catering_mode_operation_mode");

            entity.HasOne(d => d.IdSpecialDayCateringNavigation).WithMany(p => p.CateringModeOperationCaterings)
                .HasForeignKey(d => d.IdSpecialDayCatering)
                .HasConstraintName("fk_special_day_catering");
        });

        modelBuilder.Entity<CateringTypeKitchen>(entity =>
        {
            entity.HasKey(e => e.IdCateringTypeKitchen).HasName("catering_type_kitchen_pkey");

            entity.ToTable("catering_type_kitchen");

            entity.HasIndex(e => e.IdCatering, "IX_catering_type_kitchen_id_catering");

            entity.HasIndex(e => e.IdTypeKitchen, "IX_catering_type_kitchen_id_type_kitchen");

            entity.Property(e => e.IdCateringTypeKitchen).HasColumnName("id_catering_type_kitchen");
            entity.Property(e => e.IdCatering).HasColumnName("id_catering");
            entity.Property(e => e.IdTypeKitchen).HasColumnName("id_type_kitchen");

            entity.HasOne(d => d.IdCateringNavigation).WithMany(p => p.CateringTypeKitchens)
                .HasForeignKey(d => d.IdCatering)
                .HasConstraintName("fk_catering_type_kitchen_catering");

            entity.HasOne(d => d.IdTypeKitchenNavigation).WithMany(p => p.CateringTypeKitchens)
                .HasForeignKey(d => d.IdTypeKitchen)
                .HasConstraintName("fk_catering_type_kitchen_type");
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.IdEvent).HasName("event__pkey");

            entity.ToTable("event_");

            entity.Property(e => e.IdEvent)
                .HasDefaultValueSql("nextval('event_id_event_seq'::regclass)")
                .HasColumnName("id_event");
            entity.Property(e => e.AgeLimit)
                .HasMaxLength(10)
                .HasColumnName("age_limit");
            entity.Property(e => e.DateEvent).HasColumnName("date_event");
            entity.Property(e => e.HouseEvent)
                .HasMaxLength(10)
                .HasColumnName("house_event");
            entity.Property(e => e.NameEvent)
                .HasMaxLength(70)
                .HasColumnName("name_event");
            entity.Property(e => e.StreetEvent)
                .HasMaxLength(50)
                .HasColumnName("street_event");
            entity.Property(e => e.TimeBeginningEvent).HasColumnName("time_beginning_event");
            entity.Property(e => e.TypeEvent)
                .HasMaxLength(50)
                .HasColumnName("type_event");
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.IdFeedback).HasName("feedback_pkey");

            entity.ToTable("feedback");

            entity.HasIndex(e => e.IdUser, "IX_feedback_id_user");

            entity.Property(e => e.IdFeedback).HasColumnName("id_feedback");
            entity.Property(e => e.ContactCommunicationNumber)
                .HasMaxLength(11)
                .IsFixedLength()
                .HasColumnName("contact_communication_number");
            entity.Property(e => e.IdUser).HasColumnName("id_user");
            entity.Property(e => e.MessageSubject)
                .HasMaxLength(30)
                .HasColumnName("message_subject");
            entity.Property(e => e.TextMessage)
                .HasMaxLength(100)
                .HasColumnName("text_message");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.IdUser)
                .HasConstraintName("fk_feedback_user");
        });

        modelBuilder.Entity<Hotel>(entity =>
        {
            entity.HasKey(e => e.IdHotel).HasName("hotel_pkey");

            entity.ToTable("hotel");

            entity.Property(e => e.IdHotel).HasColumnName("id_hotel");
            entity.Property(e => e.ContactNumberHotel)
                .HasMaxLength(15)
                .HasColumnName("contact_number_hotel");
            entity.Property(e => e.HotelHouse)
                .HasMaxLength(10)
                .HasColumnName("hotel_house");
            entity.Property(e => e.HotelName)
                .HasMaxLength(50)
                .HasColumnName("hotel_name");
            entity.Property(e => e.HotelStreet)
                .HasMaxLength(50)
                .HasColumnName("hotel_street");
            entity.Property(e => e.HotelUrl)
                .HasMaxLength(100)
                .HasColumnName("hotel_url");
            entity.Property(e => e.TypeHotel)
                .HasMaxLength(50)
                .HasColumnName("type_hotel");
        });

        modelBuilder.Entity<ModeOperationCatering>(entity =>
        {
            entity.HasKey(e => e.IdModeOperationCatering).HasName("mode_operation_catering_pkey");

            entity.ToTable("mode_operation_catering");

            entity.Property(e => e.IdModeOperationCatering).HasColumnName("id_mode_operation_catering");
            entity.Property(e => e.Beginning).HasColumnName("beginning");
            entity.Property(e => e.EndDay).HasColumnName("end_day");
        });

        modelBuilder.Entity<OperatingMode>(entity =>
        {
            entity.HasKey(e => e.IdOperatingMode).HasName("operating_mode_pkey");

            entity.ToTable("operating_mode");

            entity.Property(e => e.IdOperatingMode).HasColumnName("id_operating_mode");
            entity.Property(e => e.EndTime).HasColumnName("end_time");
            entity.Property(e => e.StartTime).HasColumnName("start_time");
        });

        modelBuilder.Entity<PhotoHotel>(entity =>
        {
            entity.HasKey(e => e.IdPhotoHotel).HasName("photo_hotel_pkey");

            entity.ToTable("photo_hotel");

            entity.Property(e => e.IdPhotoHotel).HasColumnName("id_photo_hotel");
            entity.Property(e => e.DescriptionPhoto)
                .HasMaxLength(100)
                .HasColumnName("description_photo");
            entity.Property(e => e.IdHotel).HasColumnName("id_hotel");
            entity.Property(e => e.NameFile)
                .HasMaxLength(40)
                .HasColumnName("name_file");

            entity.HasOne(d => d.IdHotelNavigation).WithMany(p => p.PhotoHotels)
                .HasForeignKey(d => d.IdHotel)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_photo_hotel_hotel");
        });

        modelBuilder.Entity<PhotoSight>(entity =>
        {
            entity.HasKey(e => e.IdPhotoSight).HasName("photo_sight_pkey");

            entity.ToTable("photo_sight");

            entity.HasIndex(e => e.IdSight, "IX_photo_sight_id_sight");

            entity.Property(e => e.IdPhotoSight).HasColumnName("id_photo_sight");
            entity.Property(e => e.IdSight).HasColumnName("id_sight");
            entity.Property(e => e.LinkPhoto)
                .HasMaxLength(100)
                .HasColumnName("link_photo");
            entity.Property(e => e.ShortDescription)
                .HasMaxLength(50)
                .HasColumnName("short_description");

            entity.HasOne(d => d.IdSightNavigation).WithMany(p => p.PhotoSights)
                .HasForeignKey(d => d.IdSight)
                .HasConstraintName("fk_photo_sight_sight");
        });

        modelBuilder.Entity<Route>(entity =>
        {
            entity.HasKey(e => e.IdRoute).HasName("route_pkey");

            entity.ToTable("route");

            entity.HasIndex(e => e.IdUser, "IX_route_id_user");

            entity.Property(e => e.IdRoute).HasColumnName("id_route");
            entity.Property(e => e.DateCreation).HasColumnName("date_creation");
            entity.Property(e => e.IdUser).HasColumnName("id_user");
            entity.Property(e => e.NameRoute)
                .HasMaxLength(50)
                .HasColumnName("name_route");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.Routes)
                .HasForeignKey(d => d.IdUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_route_userbot");
        });

        modelBuilder.Entity<RouteCateringHotel>(entity =>
        {
            entity.HasKey(e => e.IdRouteCateringHotel).HasName("route_catering_hotel_pkey");

            entity.ToTable("route_catering_hotel");

            entity.HasIndex(e => e.IdCatering, "IX_route_catering_hotel_id_catering");

            entity.HasIndex(e => e.IdHotel, "IX_route_catering_hotel_id_hotel");

            entity.HasIndex(e => e.IdRoute, "IX_route_catering_hotel_id_route");

            entity.Property(e => e.IdRouteCateringHotel).HasColumnName("id_route_catering_hotel");
            entity.Property(e => e.IdCatering).HasColumnName("id_catering");
            entity.Property(e => e.IdHotel).HasColumnName("id_hotel");
            entity.Property(e => e.IdRoute).HasColumnName("id_route");

            entity.HasOne(d => d.IdCateringNavigation).WithMany(p => p.RouteCateringHotels)
                .HasForeignKey(d => d.IdCatering)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_route_catering_hotel_catering");

            entity.HasOne(d => d.IdHotelNavigation).WithMany(p => p.RouteCateringHotels)
                .HasForeignKey(d => d.IdHotel)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_route_catering_hotel_hotel");

            entity.HasOne(d => d.IdRouteNavigation).WithMany(p => p.RouteCateringHotels)
                .HasForeignKey(d => d.IdRoute)
                .HasConstraintName("fk_route_catering_hotel_route");
        });

        modelBuilder.Entity<RouteEventSight>(entity =>
        {
            entity.HasKey(e => e.IdRouteEventSight).HasName("route_event_sight_pkey");

            entity.ToTable("route_event_sight");

            entity.HasIndex(e => e.IdEvent, "IX_route_event_sight_id_event");

            entity.HasIndex(e => e.IdRoute, "IX_route_event_sight_id_route");

            entity.HasIndex(e => e.IdSight, "IX_route_event_sight_id_sight");

            entity.Property(e => e.IdRouteEventSight).HasColumnName("id_route_event_sight");
            entity.Property(e => e.IdEvent).HasColumnName("id_event");
            entity.Property(e => e.IdRoute).HasColumnName("id_route");
            entity.Property(e => e.IdSight).HasColumnName("id_sight");

            entity.HasOne(d => d.IdEventNavigation).WithMany(p => p.RouteEventSights)
                .HasForeignKey(d => d.IdEvent)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_route_event_sight_event");

            entity.HasOne(d => d.IdRouteNavigation).WithMany(p => p.RouteEventSights)
                .HasForeignKey(d => d.IdRoute)
                .HasConstraintName("fk_route_event_sight_route");

            entity.HasOne(d => d.IdSightNavigation).WithMany(p => p.RouteEventSights)
                .HasForeignKey(d => d.IdSight)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_route_event_sight_sight");
        });

        modelBuilder.Entity<Sight>(entity =>
        {
            entity.HasKey(e => e.IdSight).HasName("sight_pkey");

            entity.ToTable("sight");

            entity.Property(e => e.IdSight).HasColumnName("id_sight");
            entity.Property(e => e.ContactNumber)
                .HasMaxLength(15)
                .HasColumnName("contact_number");
            entity.Property(e => e.Description)
                .HasMaxLength(900)
                .HasColumnName("description");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasColumnName("email");
            entity.Property(e => e.LocationHouse)
                .HasMaxLength(10)
                .HasColumnName("location_house");
            entity.Property(e => e.LocationStreet)
                .HasMaxLength(50)
                .HasColumnName("location_street");
            entity.Property(e => e.NameSight)
                .HasMaxLength(70)
                .HasColumnName("name_sight");
            entity.Property(e => e.SightUrl)
                .HasMaxLength(100)
                .HasColumnName("sight_url");
            entity.Property(e => e.TypeSight)
                .HasMaxLength(70)
                .HasColumnName("type_sight");
        });

        modelBuilder.Entity<SightOperatingMode>(entity =>
        {
            entity.HasKey(e => e.IdSightOperatingMode).HasName("sight_operating_mode_pkey");

            entity.ToTable("sight_operating_mode");

            entity.HasIndex(e => e.IdOperatingMode, "IX_sight_operating_mode_id_operating_mode");

            entity.HasIndex(e => e.IdSight, "IX_sight_operating_mode_id_sight");

            entity.Property(e => e.IdSightOperatingMode).HasColumnName("id_sight_operating_mode");
            entity.Property(e => e.IdOperatingMode).HasColumnName("id_operating_mode");
            entity.Property(e => e.IdSight).HasColumnName("id_sight");
            entity.Property(e => e.IdSpecialDaySight).HasColumnName("id_special_day_sight");
            entity.Property(e => e.WorkingDayWeek).HasColumnName("working_day_week");

            entity.HasOne(d => d.IdOperatingModeNavigation).WithMany(p => p.SightOperatingModes)
                .HasForeignKey(d => d.IdOperatingMode)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_sight_operating_mode_operating");

            entity.HasOne(d => d.IdSightNavigation).WithMany(p => p.SightOperatingModes)
                .HasForeignKey(d => d.IdSight)
                .HasConstraintName("fk_sight_operating_mode_sight");

            entity.HasOne(d => d.IdSpecialDaySightNavigation).WithMany(p => p.SightOperatingModes)
                .HasForeignKey(d => d.IdSpecialDaySight)
                .HasConstraintName("fk_special_day_sight");
        });

        modelBuilder.Entity<Souvenir>(entity =>
        {
            entity.HasKey(e => e.IdSouvenir).HasName("souvenir_pkey");

            entity.ToTable("souvenir");

            entity.Property(e => e.IdSouvenir).HasColumnName("id_souvenir");
            entity.Property(e => e.NameSouvenir)
                .HasMaxLength(50)
                .HasColumnName("name_souvenir");
            entity.Property(e => e.Product)
                .HasMaxLength(30)
                .HasColumnName("product");
            entity.Property(e => e.Tastes)
                .HasMaxLength(50)
                .HasColumnName("tastes");
            entity.Property(e => e.Weight)
                .HasMaxLength(50)
                .HasColumnName("weight");
        });

        modelBuilder.Entity<SpecialDayCatering>(entity =>
        {
            entity.HasKey(e => e.IdSpecialDayCatering).HasName("special_day_catering_pkey");

            entity.ToTable("special_day_catering");

            entity.Property(e => e.IdSpecialDayCatering).HasColumnName("id_special_day_catering");
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.StatusDay)
                .HasMaxLength(200)
                .HasColumnName("status_day");
        });

        modelBuilder.Entity<SpecialDaySight>(entity =>
        {
            entity.HasKey(e => e.IdSpecialDaySight).HasName("special_day_sight_pkey");

            entity.ToTable("special_day_sight");

            entity.Property(e => e.IdSpecialDaySight).HasColumnName("id_special_day_sight");
            entity.Property(e => e.SpecialDayDate).HasColumnName("special_day_date");
            entity.Property(e => e.SpecialDayStatus)
                .HasMaxLength(200)
                .HasColumnName("special_day_status");
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(e => e.IdTicket).HasName("ticket_pkey");

            entity.ToTable("ticket");

            entity.HasIndex(e => e.IdEvent, "IX_ticket_id_event");

            entity.Property(e => e.IdTicket).HasColumnName("id_ticket")
    .HasDefaultValueSql("nextval('ticket_id_ticket_seq')")
    .ValueGeneratedOnAdd();
            entity.Property(e => e.CountPeople).HasColumnName("count_people");
            entity.Property(e => e.IdEvent).HasColumnName("id_event");
            entity.Property(e => e.MaximumAge).HasColumnName("maximum_age");
            entity.Property(e => e.MinimumAge).HasColumnName("minimum_age");
            entity.Property(e => e.Price)
                .HasColumnType("money")
                .HasColumnName("price");

            entity.HasOne(d => d.IdEventNavigation).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.IdEvent)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ticket_event");
        });

        modelBuilder.Entity<TypeKitchen>(entity =>
        {
            entity.HasKey(e => e.IdTypeKitchen).HasName("type_kitchen_pkey");

            entity.ToTable("type_kitchen");

            entity.HasIndex(e => e.NameTypeKitchen, "type_kitchen_name_type_kitchen_key").IsUnique();

            entity.Property(e => e.IdTypeKitchen).HasColumnName("id_type_kitchen");
            entity.Property(e => e.NameTypeKitchen)
                .HasMaxLength(50)
                .HasColumnName("name_type_kitchen");
        });

        modelBuilder.Entity<Userbot>(entity =>
        {
            entity.HasKey(e => e.IdUser).HasName("userbot_pkey");

            entity.ToTable("userbot");

            entity.Property(e => e.IdUser)
                .ValueGeneratedNever()
                .HasColumnName("id_user");
            entity.Property(e => e.DateBirth).HasColumnName("date_birth");
            entity.Property(e => e.Gender)
                .HasMaxLength(10)
                .HasColumnName("gender");
            entity.Property(e => e.LastName)
                .HasMaxLength(30)
                .HasColumnName("last_name");
            entity.Property(e => e.NamePerson)
                .HasMaxLength(30)
                .HasColumnName("name_person");
            entity.Property(e => e.Patronymic)
                .HasMaxLength(30)
                .HasColumnName("patronymic");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(11)
                .IsFixedLength()
                .HasColumnName("phone_number");
            entity.Property(e => e.UserName)
                .HasMaxLength(30)
                .HasColumnName("user_name");
        });
        modelBuilder.HasSequence("catering_mode_operation_catering_id_catering_mode_operation_cat");
        modelBuilder.HasSequence("catering_type_kitchen_id_catering_type_kitchen_seq");
        modelBuilder.HasSequence("event_id_event_seq");
        modelBuilder.HasSequence("feedback_id_feedback_seq");
        modelBuilder.HasSequence("mode_operation_catering_id_mode_operation_catering_seq");
        modelBuilder.HasSequence("operating_mode_id_operating_mode_seq");
        modelBuilder.HasSequence("photo_sight_id_photo_sight_seq");
        modelBuilder.HasSequence("route_catering_hotel_id_route_catering_hotel_seq");
        modelBuilder.HasSequence("route_event_sight_id_route_event_sight_seq");
        modelBuilder.HasSequence("route_id_route_seq");
        modelBuilder.HasSequence("sight_id_sight_seq");
        modelBuilder.HasSequence("sight_operating_mode_id_sight_operating_mode_seq");
        modelBuilder.HasSequence("souvenir_id_souvenir_seq");
        modelBuilder.HasSequence("special_day_catering_id_special_day_catering_seq");
        modelBuilder.HasSequence("special_day_sight_id_special_day_sight_seq");
        modelBuilder.HasSequence("ticket_id_ticket_seq");
        modelBuilder.HasSequence("type_kitchen_id_type_kitchen_seq");

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
