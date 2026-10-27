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

    public virtual DbSet<tournament_battle> tournament_battle { get; set; }

    public virtual DbSet<tournament_format> tournament_format { get; set; }

    public virtual DbSet<tournament_machine> tournament_machine { get; set; }

    public virtual DbSet<tournament_registration> tournament_registration { get; set; }

    public virtual DbSet<tournament_sets> tournament_sets { get; set; }

    public virtual DbSet<tournament_team> tournament_team { get; set; }

    public virtual DbSet<tournament_team_member> tournament_team_member { get; set; }

    public virtual DbSet<users> users { get; set; }

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
                .HasMaxLength(12)
                .IsUnicode(false);
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
            entity.Property(e => e.displayName)
                .HasMaxLength(50)
                .HasDefaultValue("", "DF_tournament_displayName");
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

        modelBuilder.Entity<tournament_battle>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK_tournament_battle_id");

            entity.HasIndex(e => new { e.tournamentId, e.sequence }, "UQ_tournament_battle_tournamentId_sequence").IsUnique();

            entity.Property(e => e.id)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasComment("battleId");
            entity.Property(e => e.abandonPlayerId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasComment("執行棄賽的玩家id, 與勝負無關, 只要是這場錦標賽的參加者都可以執行棄賽");
            entity.Property(e => e.awayScore).HasComment("away隊贏的set數");
            entity.Property(e => e.awayTeamId)
                .HasMaxLength(36)
                .IsUnicode(false);
            entity.Property(e => e.createdAt)
                .HasPrecision(0)
                .HasComment("battle建立的UTC時間");
            entity.Property(e => e.endDate).HasPrecision(0);
            entity.Property(e => e.homeScore).HasComment("home隊贏的set數");
            entity.Property(e => e.homeTeamId)
                .HasMaxLength(36)
                .IsUnicode(false);
            entity.Property(e => e.isAbandon).HasComment("是否棄賽");
            entity.Property(e => e.leftBattleId)
                .HasMaxLength(36)
                .IsUnicode(false);
            entity.Property(e => e.machineId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasComment("能進行對戰的唯一機台");
            entity.Property(e => e.rightBattleId)
                .HasMaxLength(36)
                .IsUnicode(false);
            entity.Property(e => e.round).HasComment("round數從1開始");
            entity.Property(e => e.roundName).HasMaxLength(50);
            entity.Property(e => e.startDate).HasPrecision(0);
            entity.Property(e => e.status).HasComment("0 => 等待中, 1 => 可以允許開打, 2 => 已結束");
            entity.Property(e => e.tournamentId)
                .HasMaxLength(36)
                .IsUnicode(false);
            entity.Property(e => e.winnerTeamId)
                .HasMaxLength(36)
                .IsUnicode(false);
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
            entity.HasKey(e => new { e.tournamentId, e.sequence }).HasName("PK_tournament_registration_tournamentId_sequence");

            entity.HasIndex(e => new { e.tournamentId, e.playerId }, "UQ_tournamentId_playerId").IsUnique();

            entity.Property(e => e.tournamentId)
                .HasMaxLength(36)
                .IsUnicode(false);
            entity.Property(e => e.sequence).HasComment("sequence");
            entity.Property(e => e.displayName).HasMaxLength(80);
            entity.Property(e => e.isGuest).HasComment("參加者是否是透過網頁訪客報名");
            entity.Property(e => e.playerId)
                .HasMaxLength(36)
                .IsUnicode(false);
            entity.Property(e => e.teamId)
                .HasMaxLength(36)
                .IsUnicode(false);
        });

        modelBuilder.Entity<tournament_sets>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK_tournament_sets_id");

            entity.HasIndex(e => new { e.battleId, e.sequence }, "UQ_tournament_sets_battleId_sequence").IsUnique();

            entity.Property(e => e.id)
                .HasMaxLength(36)
                .IsUnicode(false);
            entity.Property(e => e.awayWinLeg).HasComment("away隊這個set贏的leg數量");
            entity.Property(e => e.battleId)
                .HasMaxLength(36)
                .IsUnicode(false);
            entity.Property(e => e.createdAt)
                .HasPrecision(0)
                .HasComment("set建立的UTC時間");
            entity.Property(e => e.endDate).HasPrecision(0);
            entity.Property(e => e.firstAttackTeamIndex)
                .HasComment("先攻home/away的index, -1->未決定, 0->home, 1->away")
                .HasDefaultValue(-1, "DF_tournament_sets_firstAttackTeamIndex");
            entity.Property(e => e.homeWinLeg).HasComment("home隊這個set贏的leg數量");
            entity.Property(e => e.startDate).HasPrecision(0);
            entity.Property(e => e.winnerTeamId)
                .HasMaxLength(36)
                .IsUnicode(false);
        });

        modelBuilder.Entity<tournament_team>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK_tournament_team_id");

            entity.HasIndex(e => new { e.tournamentId, e.sequence }, "UQ_tournament_team_tournamentId_sequence").IsUnique();

            entity.Property(e => e.id)
                .HasMaxLength(36)
                .IsUnicode(false);
            entity.Property(e => e.createdAt).HasPrecision(0);
            entity.Property(e => e.sequence).HasComment("報名的順序, 和比賽排序無關, 從0開始");
            entity.Property(e => e.tournamentId)
                .HasMaxLength(36)
                .IsUnicode(false);
        });

        modelBuilder.Entity<tournament_team_member>(entity =>
        {
            entity.HasKey(e => new { e.tournamentId, e.teamId, e.playerId }).HasName("PK_tournament_team_member_tournamentId_teamId_playerId");

            entity.HasIndex(e => new { e.tournamentId, e.sequence }, "UQ_tournament_team_member_tournamentId_sequence").IsUnique();

            entity.Property(e => e.tournamentId)
                .HasMaxLength(36)
                .IsUnicode(false);
            entity.Property(e => e.teamId)
                .HasMaxLength(36)
                .IsUnicode(false);
            entity.Property(e => e.playerId)
                .HasMaxLength(36)
                .IsUnicode(false);
            entity.Property(e => e.displayName)
                .HasMaxLength(50)
                .HasComment("玩家參加比賽顯示的名稱, 可能是玩家報名時的nickname或是網頁報名自己填入的名稱");
            entity.Property(e => e.isGuest).HasComment("參加者是否是透過網頁訪客報名");
            entity.Property(e => e.sequence).HasComment("報名順序");
        });

        modelBuilder.Entity<users>(entity =>
        {
            entity.HasIndex(e => e.email, "IX_users_email").IsUnique();

            entity.Property(e => e.id)
                .HasMaxLength(36)
                .IsUnicode(false)
                .HasDefaultValueSql("(newid())", "DF_users_id");
            entity.Property(e => e.createdAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())", "DF_users_createdAt");
            entity.Property(e => e.displayId)
                .HasMaxLength(8)
                .IsUnicode(false);
            entity.Property(e => e.email)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.name)
                .HasMaxLength(50)
                .HasDefaultValue("", "DF_users_name");
            entity.Property(e => e.nation)
                .HasMaxLength(2)
                .IsUnicode(false);
            entity.Property(e => e.nickname)
                .HasMaxLength(50)
                .HasDefaultValue("", "DF_users_nickname");
            entity.Property(e => e.password)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.photoURL)
                .HasMaxLength(128)
                .IsUnicode(false)
                .HasDefaultValue("", "DF_users_photoURL");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
