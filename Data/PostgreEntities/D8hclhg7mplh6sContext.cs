using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace FindingPets.Data.PostgreEntities;

public partial class D8hclhg7mplh6sContext : DbContext
{
    public D8hclhg7mplh6sContext()
    {
    }

    public D8hclhg7mplh6sContext(DbContextOptions<D8hclhg7mplh6sContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Authenuser> Authenusers { get; set; }

    public virtual DbSet<Post> Posts { get; set; }

    public virtual DbSet<Postimage> Postimages { get; set; }

    public virtual DbSet<Userrole> Userroles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasPostgresExtension("heroku_ext", "pg_stat_statements")
            .HasPostgresExtension("uuid-ossp");

        modelBuilder.Entity<Authenuser>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("authenuser_pkey");

            entity.ToTable("authenuser");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v1()")
                .HasColumnName("id");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasColumnName("email");
            entity.Property(e => e.Fullname)
                .HasMaxLength(50)
                .HasColumnName("fullname");
            entity.Property(e => e.Imageurl)
                .HasColumnType("character varying")
                .HasColumnName("imageurl");
            entity.Property(e => e.Isactive)
                .HasDefaultValueSql("false")
                .HasColumnName("isactive");
            entity.Property(e => e.Phone)
                .HasMaxLength(12)
                .HasColumnName("phone");
            entity.Property(e => e.Userrole).HasColumnName("userrole");

            entity.HasOne(d => d.UserroleNavigation).WithMany(p => p.Authenusers)
                .HasForeignKey(d => d.Userrole)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_userrole");
        });

        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("post_pkey");

            entity.ToTable("post");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v1()")
                .HasColumnName("id");
            entity.Property(e => e.Contact)
                .HasColumnType("character varying")
                .HasColumnName("contact");
            entity.Property(e => e.Created)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created");
            entity.Property(e => e.Description)
                .HasColumnType("character varying")
                .HasColumnName("description");
            entity.Property(e => e.Isbanned)
                .HasDefaultValueSql("false")
                .HasColumnName("isbanned");
            entity.Property(e => e.Isclosed)
                .HasDefaultValueSql("false")
                .HasColumnName("isclosed");
            entity.Property(e => e.Ownerid).HasColumnName("ownerid");
            entity.Property(e => e.Title)
                .HasMaxLength(200)
                .HasColumnName("title");

            entity.HasOne(d => d.Owner).WithMany(p => p.Posts)
                .HasForeignKey(d => d.Ownerid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_authenuser");
        });

        modelBuilder.Entity<Postimage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("postimages_pkey");

            entity.ToTable("postimages");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v1()")
                .HasColumnName("id");
            entity.Property(e => e.Imagebase64)
                .HasColumnType("character varying")
                .HasColumnName("imagebase64");
            entity.Property(e => e.Postid).HasColumnName("postid");

            entity.HasOne(d => d.Post).WithMany(p => p.Postimages)
                .HasForeignKey(d => d.Postid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_post");
        });

        modelBuilder.Entity<Userrole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("userrole_pkey");

            entity.ToTable("userrole");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Rolename)
                .HasMaxLength(50)
                .HasColumnName("rolename");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
