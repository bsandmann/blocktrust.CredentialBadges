﻿// <auto-generated />
using System;
using Blocktrust.CredentialBadges.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Blocktrust.CredentialBadges.Web.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20250306112551_RemovedNameDescription")]
    partial class RemovedNameDescription
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Blocktrust.CredentialBadges.Web.Entities.VerifiedCredentialEntity", b =>
                {
                    b.Property<Guid>("StoredCredentialId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Claims")
                        .HasMaxLength(5000)
                        .HasColumnType("text");

                    b.Property<string>("Credential")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Domain")
                        .HasMaxLength(253)
                        .HasColumnType("character varying(253)");

                    b.Property<string>("Image")
                        .HasColumnType("text");

                    b.Property<string>("Issuer")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.Property<string>("TemplateId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("ValidFrom")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("ValidUntil")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("StoredCredentialId");

                    b.ToTable("VerifiedCredentials");
                });
#pragma warning restore 612, 618
        }
    }
}
