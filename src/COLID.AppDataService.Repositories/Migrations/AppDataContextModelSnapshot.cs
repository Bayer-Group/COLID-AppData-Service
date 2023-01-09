﻿// <auto-generated />
using System;
using COLID.AppDataService.Repositories.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Repositories.Migrations
{
    [DbContext(typeof(AppDataContext))]
    partial class AppDataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("COLID.AppDataService.Common.DataModel.ColidEntrySubscription", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("int");

                    b.Property<string>("ColidPidUri")
                        .HasColumnName("colid_pid_uri")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnName("created_at")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnName("modified_at")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Note")
                        .HasColumnName("note")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<Guid?>("UserId")
                        .HasColumnName("user_id")
                        .HasColumnType("char(36)");

                    b.HasKey("Id")
                        .HasName("pk_colid_entry_subscriptions");

                    b.HasIndex("UserId")
                        .HasName("ix_colid_entry_subscriptions_user_id");

                    b.ToTable("colid_entry_subscriptions");
                });

            modelBuilder.Entity("COLID.AppDataService.Common.DataModel.ConsumerGroup", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("int");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnName("created_at")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnName("modified_at")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Uri")
                        .IsRequired()
                        .HasColumnName("uri")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("Id")
                        .HasName("pk_consumer_groups");

                    b.ToTable("consumer_groups");
                });

            modelBuilder.Entity("COLID.AppDataService.Common.DataModel.FavoritesList", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("int");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnName("created_at")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnName("modified_at")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Name")
                        .HasColumnName("name")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<Guid?>("UserId")
                        .HasColumnName("user_id")
                        .HasColumnType("char(36)");

                    b.HasKey("Id")
                        .HasName("pk_favorites_lists");

                    b.HasIndex("Id")
                        .IsUnique()
                        .HasName("ix_favorites_lists_id");

                    b.HasIndex("UserId")
                        .HasName("ix_favorites_lists_user_id");

                    b.ToTable("favorites_lists");
                });

            modelBuilder.Entity("COLID.AppDataService.Common.DataModel.FavoritesListEntry", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("int");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnName("created_at")
                        .HasColumnType("datetime(6)");

                    b.Property<int?>("FavoritesListId")
                        .HasColumnName("favorites_list_id")
                        .HasColumnType("int");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnName("modified_at")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("PIDUri")
                        .HasColumnName("piduri")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("PersonalNote")
                        .HasColumnName("personal_note")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("Id")
                        .HasName("pk_favorites_list_entries");

                    b.HasIndex("FavoritesListId")
                        .HasName("ix_favorites_list_entries_favorites_list_id");

                    b.ToTable("favorites_list_entries");
                });

            modelBuilder.Entity("COLID.AppDataService.Common.DataModel.Message", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("int");

                    b.Property<string>("AdditionalInfo")
                        .HasColumnName("additional_info")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Body")
                        .HasColumnName("body")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnName("created_at")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("DeleteOn")
                        .HasColumnName("delete_on")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnName("modified_at")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("ReadOn")
                        .HasColumnName("read_on")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("SendOn")
                        .HasColumnName("send_on")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Subject")
                        .HasColumnName("subject")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<Guid?>("UserId")
                        .HasColumnName("user_id")
                        .HasColumnType("char(36)");

                    b.HasKey("Id")
                        .HasName("pk_messages");

                    b.HasIndex("UserId")
                        .HasName("ix_messages_user_id");

                    b.ToTable("messages");
                });

            modelBuilder.Entity("COLID.AppDataService.Common.DataModel.MessageConfig", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("int");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnName("created_at")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("DeleteInterval")
                        .HasColumnName("delete_interval")
                        .HasColumnType("int");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnName("modified_at")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("SendInterval")
                        .HasColumnName("send_interval")
                        .HasColumnType("int");

                    b.Property<Guid?>("UserId")
                        .HasColumnName("user_id")
                        .HasColumnType("char(36)");

                    b.HasKey("Id")
                        .HasName("pk_message_configs");

                    b.HasIndex("UserId")
                        .IsUnique()
                        .HasName("ix_message_configs_user_id");

                    b.ToTable("message_configs");
                });

            modelBuilder.Entity("COLID.AppDataService.Common.DataModel.MessageTemplate", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("int");

                    b.Property<string>("Body")
                        .HasColumnName("body")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnName("created_at")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnName("modified_at")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Subject")
                        .HasColumnName("subject")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int>("Type")
                        .HasColumnName("type")
                        .HasColumnType("int");

                    b.HasKey("Id")
                        .HasName("pk_message_templates");

                    b.HasIndex("Type")
                        .IsUnique()
                        .HasName("ix_message_templates_type");

                    b.ToTable("message_templates");
                });

            modelBuilder.Entity("COLID.AppDataService.Common.DataModel.SearchFilterDataMarketplace", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("int");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnName("created_at")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("FilterJson")
                        .HasColumnName("filter_json")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnName("modified_at")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Name")
                        .HasColumnName("name")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("SearchTerm")
                        .HasColumnName("search_term")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int?>("StoredQueryId")
                        .HasColumnName("stored_query_id")
                        .HasColumnType("int");

                    b.Property<Guid?>("UserId")
                        .HasColumnName("user_id")
                        .HasColumnType("char(36)");

                    b.HasKey("Id")
                        .HasName("pk_search_filter_data_marketplace");

                    b.HasIndex("StoredQueryId")
                        .IsUnique()
                        .HasName("ix_search_filter_data_marketplace_stored_query_id");

                    b.HasIndex("UserId")
                        .HasName("ix_search_filter_data_marketplace_user_id");

                    b.ToTable("search_filter_data_marketplace");
                });

            modelBuilder.Entity("COLID.AppDataService.Common.DataModel.SearchFilterEditor", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("int");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnName("created_at")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("FilterJson")
                        .HasColumnName("filter_json")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnName("modified_at")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id")
                        .HasName("pk_search_filters_editor");

                    b.ToTable("search_filters_editor");
                });

            modelBuilder.Entity("COLID.AppDataService.Common.DataModel.StoredQuery", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("int");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnName("created_at")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("ExecutionInterval")
                        .HasColumnName("execution_interval")
                        .HasColumnType("int");

                    b.Property<DateTime?>("LatestExecutionDate")
                        .HasColumnName("latest_execution_date")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnName("modified_at")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("NumberSearchResults")
                        .HasColumnName("number_search_results")
                        .HasColumnType("int");

                    b.Property<string>("SearchResultHash")
                        .HasColumnName("search_result_hash")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("Id")
                        .HasName("pk_stored_queries");

                    b.ToTable("stored_queries");
                });

            modelBuilder.Entity("COLID.AppDataService.Common.DataModel.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("char(36)");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnName("created_at")
                        .HasColumnType("datetime(6)");

                    b.Property<int?>("DefaultConsumerGroupId")
                        .HasColumnName("default_consumer_group_id")
                        .HasColumnType("int");

                    b.Property<int?>("DefaultSearchFilterDataMarketplace")
                        .HasColumnName("default_search_filter_data_marketplace")
                        .HasColumnType("int");

                    b.Property<string>("EmailAddress")
                        .HasColumnName("email_address")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<DateTime?>("LastLoginDataMarketplace")
                        .HasColumnName("last_login_data_marketplace")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("LastLoginEditor")
                        .HasColumnName("last_login_editor")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("LastTimeChecked")
                        .HasColumnName("last_time_checked")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnName("modified_at")
                        .HasColumnType("datetime(6)");

                    b.Property<int?>("SearchFilterEditorId")
                        .HasColumnName("search_filter_editor_id")
                        .HasColumnType("int");

                    b.HasKey("Id")
                        .HasName("pk_users");

                    b.HasIndex("DefaultConsumerGroupId")
                        .HasName("ix_users_default_consumer_group_id");

                    b.HasIndex("Id")
                        .IsUnique()
                        .HasName("ix_users_id");

                    b.HasIndex("SearchFilterEditorId")
                        .HasName("ix_users_search_filter_editor_id");

                    b.ToTable("users");
                });

            modelBuilder.Entity("COLID.AppDataService.Common.DataModel.WelcomeMessage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("int");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnName("content")
                        .HasColumnType("text");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnName("created_at")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnName("modified_at")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("Type")
                        .HasColumnName("type")
                        .HasColumnType("int");

                    b.HasKey("Id")
                        .HasName("pk_welcome_messages");

                    b.HasIndex("Type")
                        .IsUnique()
                        .HasName("ix_welcome_messages_type");

                    b.ToTable("welcome_messages");
                });

            modelBuilder.Entity("COLID.AppDataService.Common.DataModel.ColidEntrySubscription", b =>
                {
                    b.HasOne("COLID.AppDataService.Common.DataModel.User", "User")
                        .WithMany("ColidEntrySubscriptions")
                        .HasForeignKey("UserId")
                        .HasConstraintName("fk_colid_entry_subscriptions_users_user_id")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("COLID.AppDataService.Common.DataModel.FavoritesList", b =>
                {
                    b.HasOne("COLID.AppDataService.Common.DataModel.User", "User")
                        .WithMany("FavoritesLists")
                        .HasForeignKey("UserId")
                        .HasConstraintName("fk_favorites_lists_users_user_id")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("COLID.AppDataService.Common.DataModel.FavoritesListEntry", b =>
                {
                    b.HasOne("COLID.AppDataService.Common.DataModel.FavoritesList", "FavoritesLists")
                        .WithMany("FavoritesListEntries")
                        .HasForeignKey("FavoritesListId")
                        .HasConstraintName("fk_favorites_list_entries_favorites_lists_favorites_list_id")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("COLID.AppDataService.Common.DataModel.Message", b =>
                {
                    b.HasOne("COLID.AppDataService.Common.DataModel.User", "User")
                        .WithMany("Messages")
                        .HasForeignKey("UserId")
                        .HasConstraintName("fk_messages_users_user_id")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("COLID.AppDataService.Common.DataModel.MessageConfig", b =>
                {
                    b.HasOne("COLID.AppDataService.Common.DataModel.User", "User")
                        .WithOne("MessageConfig")
                        .HasForeignKey("COLID.AppDataService.Common.DataModel.MessageConfig", "UserId")
                        .HasConstraintName("fk_message_configs_users_user_id");
                });

            modelBuilder.Entity("COLID.AppDataService.Common.DataModel.SearchFilterDataMarketplace", b =>
                {
                    b.HasOne("COLID.AppDataService.Common.DataModel.StoredQuery", "StoredQuery")
                        .WithOne("SearchFilterDataMarketplace")
                        .HasForeignKey("COLID.AppDataService.Common.DataModel.SearchFilterDataMarketplace", "StoredQueryId")
                        .HasConstraintName("fk_search_filter_data_marketplace_stored_queries_stored_query_id")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("COLID.AppDataService.Common.DataModel.User", "User")
                        .WithMany("SearchFiltersDataMarketplace")
                        .HasForeignKey("UserId")
                        .HasConstraintName("fk_search_filter_data_marketplace_users_user_id")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("COLID.AppDataService.Common.DataModel.User", b =>
                {
                    b.HasOne("COLID.AppDataService.Common.DataModel.ConsumerGroup", "DefaultConsumerGroup")
                        .WithMany("Users")
                        .HasForeignKey("DefaultConsumerGroupId")
                        .HasConstraintName("fk_users_consumer_groups_default_consumer_group_id")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("COLID.AppDataService.Common.DataModel.SearchFilterEditor", "SearchFilterEditor")
                        .WithMany("Users")
                        .HasForeignKey("SearchFilterEditorId")
                        .HasConstraintName("fk_users_search_filters_editor_search_filter_editor_id")
                        .OnDelete(DeleteBehavior.SetNull);
                });
#pragma warning restore 612, 618
        }
    }
}
