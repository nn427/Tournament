using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace TournamentAPI.Models;

public partial class LocalDBContext : DbContext
{
    public LocalDBContext(DbContextOptions<LocalDBContext> options)
        : base(options)
    {
    }

    public virtual DbSet<distributor> distributor { get; set; }

    public virtual DbSet<machine> machine { get; set; }

    public virtual DbSet<store> store { get; set; }

    public virtual DbSet<tournament> tournament { get; set; }

    public virtual DbSet<tournament_format> tournament_format { get; set; }

    public virtual DbSet<tournament_machine> tournament_machine { get; set; }

    public virtual DbSet<tournament_registration> tournament_registration { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<distributor>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK_distributor_id");

            entity.Property(e => e.id)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasDefaultValueSql("(newid())", "DF_distributor1_id");
            entity.Property(e => e.name).HasMaxLength(50);
            entity.Property(e => e.nation)
                .HasMaxLength(2)
                .IsUnicode(false);
        });

        modelBuilder.Entity<machine>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK_machine_id");

            entity.HasIndex(e => e.storeId, "IX_machine_storeId");

            entity.Property(e => e.id)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasDefaultValueSql("(newid())", "DF_machine_id");
            entity.Property(e => e.macAddress)
                .HasMaxLength(12)
                .IsUnicode(false);
            entity.Property(e => e.storeId)
                .HasMaxLength(36)
                .IsUnicode(false);
        });

        modelBuilder.Entity<store>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK_store_id");

            entity.HasIndex(e => e.distributorId, "IX_store_distributorId");

            entity.Property(e => e.id)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasDefaultValueSql("(newid())", "DF_store_id");
            entity.Property(e => e.distributorId)
                .HasMaxLength(36)
                .IsUnicode(false);
            entity.Property(e => e.name).HasMaxLength(50);
        });

        modelBuilder.Entity<tournament>(entity =>
        {
            entity.HasIndex(e => e.distributorId, "IX_house_tournament_distributorId");

            entity.HasIndex(e => e.storeId, "IX_house_tournament_storeId");

            entity.Property(e => e.id)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasDefaultValueSql("(newid())", "DF_tournament_id");
            entity.Property(e => e.createdAt)
                .HasPrecision(0)
                .HasComment("比賽建立時間UTC")
                .HasDefaultValueSql("(sysutcdatetime())", "DF_tournament_createdAt");
            entity.Property(e => e.dailySequence).HasComment("同一天同一店家建立比賽的序號, 從1開始");
            entity.Property(e => e.dartType)
                .HasComment("1->硬靶, 2->軟靶")
                .HasDefaultValue((byte)1, "DF_tournament_dartType");
            entity.Property(e => e.displayName).HasMaxLength(50);
            entity.Property(e => e.distributorId)
                .HasMaxLength(36)
                .IsUnicode(false);
            entity.Property(e => e.endDate)
                .HasPrecision(0)
                .HasComment("比賽結束時間UTC->冠軍出來才算比賽結束");
            entity.Property(e => e.status).HasComment("0->已建立但未報名完成，1->比賽期間，2->比賽結束");
            entity.Property(e => e.storeId)
                .HasMaxLength(36)
                .IsUnicode(false);
            entity.Property(e => e.tournamentDate)
                .HasMaxLength(16)
                .IsUnicode(false)
                .HasComment("比賽建立的local日期");
            entity.Property(e => e.tournamentName).HasMaxLength(50);
        });

        modelBuilder.Entity<tournament_format>(entity =>
        {
            entity.HasKey(e => e.tournamentId).HasName("PK_tournament_format_tournamentId");

            entity.Property(e => e.tournamentId).HasMaxLength(36);
            entity.Property(e => e.checkoutType).HasComment("0->OIOO, 1->OIDO, 2->DIDO...");
            entity.Property(e => e.createdAt).HasPrecision(0);
            entity.Property(e => e.game01Score).HasComment("01遊戲的分數");
            entity.Property(e => e.gameTypes).HasColumnType("json");
            entity.Property(e => e.isProMode).HasComment("01遊戲專屬, true>三鏢更新分數 false>每鏢更新分數");
            entity.Property(e => e.setLegType).HasComment("1=>盤，2=>局");
            entity.Property(e => e.teamPlayer)
                .HasComment("每隊幾個玩家")
                .HasDefaultValue((byte)1, "DF_tournament_format_teamPlayer");
        });

        modelBuilder.Entity<tournament_machine>(entity =>
        {
            entity.HasKey(e => new { e.tournamentId, e.machineId }).HasName("PK_tournament_machine_id");

            entity.HasIndex(e => e.machineId, "IX_machineId");

            entity.HasIndex(e => new { e.tournamentId, e.sequence }, "UQ_tournamentId_machineId").IsUnique();

            entity.Property(e => e.tournamentId)
                .HasMaxLength(36)
                .IsUnicode(false);
            entity.Property(e => e.machineId)
                .HasMaxLength(36)
                .IsUnicode(false);
            entity.Property(e => e.sequence).HasComment("機台指定的順序,從0開始");
        });

        modelBuilder.Entity<tournament_registration>(entity =>
        {
            entity.HasNoKey();

            entity.HasIndex(e => new { e.tournamentId, e.sequence }, "PK_tournament_registration_tournamentId_sequence").IsUnique();

            entity.HasIndex(e => new { e.tournamentId, e.playerId }, "UQ_tournamentId_playerId").IsUnique();

            entity.Property(e => e.displayName).HasMaxLength(80);
            entity.Property(e => e.isGuest).HasComment("參加者是否是透過網頁訪客報名");
            entity.Property(e => e.playerId)
                .HasMaxLength(36)
                .IsUnicode(false);
            entity.Property(e => e.sequence).HasComment("sequence");
            entity.Property(e => e.teamId)
                .HasMaxLength(36)
                .IsUnicode(false);
            entity.Property(e => e.tournamentId)
                .HasMaxLength(36)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
