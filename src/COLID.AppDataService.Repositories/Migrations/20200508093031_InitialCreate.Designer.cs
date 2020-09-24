﻿// <auto-generated />
using System;
using COLID.AppDataService.Repositories.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Repositories.Migrations
{
    [DbContext(typeof(AppDataContext))]
    [Migration("20200508093031_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("COLID.AppDataService.Common.DataModel.ColidEntrySubscription", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("int");

                    b.Property<string>("ColidEntry")
                        .HasColumnName("colid_entry")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnName("created_at")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnName("modified_at")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("NextExecutionAt")
                        .HasColumnName("next_execution_at")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("NotificationInterval")
                        .HasColumnName("notification_interval")
                        .HasColumnType("int");

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

            modelBuilder.Entity("COLID.AppDataService.Common.DataModel.Message", b =>
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

                    b.Property<DateTime?>("HasBeenReadAt")
                        .HasColumnName("has_been_read_at")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("HasBeenSentAt")
                        .HasColumnName("has_been_sent_at")
                        .HasColumnType("datetime(6)");

                    b.Property<int?>("MessageTemplateId")
                        .HasColumnName("message_template_id")
                        .HasColumnType("int");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnName("modified_at")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Subject")
                        .HasColumnName("subject")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<DateTime?>("ToBeDeletedAt")
                        .HasColumnName("to_be_deleted_at")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("Type")
                        .HasColumnName("type")
                        .HasColumnType("int");

                    b.Property<Guid?>("UserId")
                        .HasColumnName("user_id")
                        .HasColumnType("char(36)");

                    b.HasKey("Id")
                        .HasName("pk_messages");

                    b.HasIndex("MessageTemplateId")
                        .HasName("ix_messages_message_template_id");

                    b.HasIndex("UserId")
                        .HasName("ix_messages_user_id");

                    b.ToTable("messages");
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

                    b.Property<Guid?>("UserId")
                        .HasColumnName("user_id")
                        .HasColumnType("char(36)");

                    b.HasKey("Id")
                        .HasName("pk_search_filter_data_marketplace");

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

                    b.Property<string>("LastExecutionResult")
                        .HasColumnName("last_execution_result")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnName("modified_at")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("NextExecutionAt")
                        .HasColumnName("next_execution_at")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("QueryJson")
                        .HasColumnName("query_json")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("QueryName")
                        .HasColumnName("query_name")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<Guid?>("UserId")
                        .HasColumnName("user_id")
                        .HasColumnType("char(36)");

                    b.HasKey("Id")
                        .HasName("pk_stored_queries");

                    b.HasIndex("UserId")
                        .HasName("ix_stored_queries_user_id");

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

                    b.HasIndex("SearchFilterEditorId")
                        .HasName("ix_users_search_filter_editor_id");

                    b.ToTable("users");
                });

            modelBuilder.Entity("COLID.AppDataService.Common.DataModel.UserMessageConfig", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("int");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnName("created_at")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("DeleteReadMessagesAfter")
                        .HasColumnName("delete_read_messages_after")
                        .HasColumnType("int");

                    b.Property<int>("DeleteSentMessagesAfter")
                        .HasColumnName("delete_sent_messages_after")
                        .HasColumnType("int");

                    b.Property<int?>("MessageTemplateId")
                        .HasColumnName("message_template_id")
                        .HasColumnType("int");

                    b.Property<int>("MessagesType")
                        .HasColumnName("messages_type")
                        .HasColumnType("int");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnName("modified_at")
                        .HasColumnType("datetime(6)");

                    b.Property<Guid?>("UserId")
                        .HasColumnName("user_id")
                        .HasColumnType("char(36)");

                    b.HasKey("Id")
                        .HasName("pk_user_message_configs");

                    b.HasIndex("MessageTemplateId")
                        .HasName("ix_user_message_configs_message_template_id");

                    b.HasIndex("UserId")
                        .HasName("ix_user_message_configs_user_id");

                    b.ToTable("user_message_configs");
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

                    b.ToTable("welcome_messages");
                });

            modelBuilder.Entity("COLID.AppDataService.Common.DataModel.ColidEntrySubscription", b =>
                {
                    b.HasOne("COLID.AppDataService.Common.DataModel.User", null)
                        .WithMany("ColidEntrySubscriptions")
                        .HasForeignKey("UserId")
                        .HasConstraintName("fk_colid_entry_subscriptions_users_user_id");
                });

            modelBuilder.Entity("COLID.AppDataService.Common.DataModel.Message", b =>
                {
                    b.HasOne("COLID.AppDataService.Common.DataModel.MessageTemplate", "MessageTemplate")
                        .WithMany()
                        .HasForeignKey("MessageTemplateId")
                        .HasConstraintName("fk_messages_message_templates_message_template_id");

                    b.HasOne("COLID.AppDataService.Common.DataModel.User", "User")
                        .WithMany("Messages")
                        .HasForeignKey("UserId")
                        .HasConstraintName("fk_messages_users_user_id")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("COLID.AppDataService.Common.DataModel.SearchFilterDataMarketplace", b =>
                {
                    b.HasOne("COLID.AppDataService.Common.DataModel.User", "User")
                        .WithMany("SearchFiltersDataMarketplace")
                        .HasForeignKey("UserId")
                        .HasConstraintName("fk_search_filter_data_marketplace_users_user_id")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("COLID.AppDataService.Common.DataModel.StoredQuery", b =>
                {
                    b.HasOne("COLID.AppDataService.Common.DataModel.User", "User")
                        .WithMany("StoredQueries")
                        .HasForeignKey("UserId")
                        .HasConstraintName("fk_stored_queries_users_user_id")
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
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("COLID.AppDataService.Common.DataModel.UserMessageConfig", b =>
                {
                    b.HasOne("COLID.AppDataService.Common.DataModel.MessageTemplate", "MessagesTemplate")
                        .WithMany()
                        .HasForeignKey("MessageTemplateId")
                        .HasConstraintName("fk_user_message_configs_message_templates_message_template_id");

                    b.HasOne("COLID.AppDataService.Common.DataModel.User", "User")
                        .WithMany("UserMessageConfigs")
                        .HasForeignKey("UserId")
                        .HasConstraintName("fk_user_message_configs_users_user_id")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
