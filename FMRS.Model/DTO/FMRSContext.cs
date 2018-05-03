using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace FMRS.Model.DTO
{
    public partial class FMRSContext : DbContext
    {
        public virtual DbSet<AccessRight> AccessRight { get; set; }
        public virtual DbSet<AccessRightD> AccessRightD { get; set; }
        public virtual DbSet<AccessRightM> AccessRightM { get; set; }
        public virtual DbSet<AccessRightY> AccessRightY { get; set; }
        public virtual DbSet<CwrfAccessControl> CwrfAccessControl { get; set; }
        public virtual DbSet<DonationBalance> DonationBalance { get; set; }
        public virtual DbSet<DonationDetail> DonationDetail { get; set; }
        public virtual DbSet<DonCat> DonCat { get; set; }
        public virtual DbSet<DonDonor> DonDonor { get; set; }
        public virtual DbSet<DonPurpose> DonPurpose { get; set; }
        public virtual DbSet<DonSubcat> DonSubcat { get; set; }
        public virtual DbSet<DonSubsubcat> DonSubsubcat { get; set; }
        public virtual DbSet<DonSupercat> DonSupercat { get; set; }
        public virtual DbSet<DonType> DonType { get; set; }
        public virtual DbSet<Hospital> Hospital { get; set; }
        public virtual DbSet<SuppUserGroup> SuppUserGroup { get; set; }
        public virtual DbSet<UserGroup> UserGroup { get; set; }
        public virtual DbSet<UserGroupHosp> UserGroupHosp { get; set; }
        public virtual DbSet<UserInfo> UserInfo { get; set; }
        public virtual DbSet<UserSpecialty> UserSpecialty { get; set; }

        //        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //        {
        //            if (!optionsBuilder.IsConfigured)
        //            {
        //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
        //                optionsBuilder.UseSqlServer(@"Server=HAS-FIN-DB;Database=FMRS;user id=sa;password=devadmin;persist security info=True;");
        //            }
        //        }
        public FMRSContext(DbContextOptions<FMRSContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AccessRight>(entity =>
            {
                entity.HasKey(e => new { e.LoginId, e.AccessType, e.Privilege })
                    .ForSqlServerIsClustered(false);

                entity.ToTable("access_right");

                entity.HasIndex(e => new { e.LoginId, e.AccessType })
                    .HasName("IDX_access_right")
                    .IsUnique()
                    .ForSqlServerIsClustered();

                entity.Property(e => e.LoginId)
                    .HasColumnName("login_id")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.AccessType)
                    .HasColumnName("access_type")
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.Privilege)
                    .HasColumnName("privilege")
                    .HasColumnType("char(1)");
            });

            modelBuilder.Entity<AccessRightD>(entity =>
            {
                entity.HasKey(e => e.LoginId);

                entity.ToTable("access_right_d");

                entity.Property(e => e.LoginId)
                    .HasColumnName("login_id")
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.AdminD)
                    .HasColumnName("admin_D")
                    .HasColumnType("char(1)");

                entity.Property(e => e.AsoiRpt)
                    .HasColumnName("asoi_rpt")
                    .HasColumnType("char(1)");

                entity.Property(e => e.Closing)
                    .HasColumnName("closing")
                    .HasColumnType("char(1)");

                entity.Property(e => e.Donation)
                    .HasColumnName("donation")
                    .HasColumnType("char(1)");

                entity.Property(e => e.InputBy)
                    .IsRequired()
                    .HasColumnName("input_by")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.InputDate)
                    .HasColumnName("input_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.NonPjtReport)
                    .HasColumnName("non_pjt_report")
                    .HasColumnType("char(1)");

                entity.Property(e => e.ReportD)
                    .HasColumnName("report_D")
                    .HasColumnType("char(1)");

                entity.Property(e => e.UpdateBy)
                    .IsRequired()
                    .HasColumnName("update_by")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UpdateDate)
                    .HasColumnName("update_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.UserGroup)
                    .IsRequired()
                    .HasColumnName("user_group")
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<AccessRightM>(entity =>
            {
                entity.HasKey(e => e.LoginId);

                entity.ToTable("access_right_m");

                entity.Property(e => e.LoginId)
                    .HasColumnName("login_id")
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.AdminM)
                    .HasColumnName("admin_M")
                    .HasColumnType("char(1)");

                entity.Property(e => e.AsoiRpt)
                    .HasColumnName("asoi_rpt")
                    .HasColumnType("char(1)");

                entity.Property(e => e.Cbv)
                    .HasColumnName("cbv")
                    .HasColumnType("char(1)");

                entity.Property(e => e.CbvFunding)
                    .HasColumnName("cbv_funding")
                    .HasColumnType("char(1)");

                entity.Property(e => e.CbvOriUpdate)
                    .HasColumnName("cbv_ori_update")
                    .HasColumnType("char(1)");

                entity.Property(e => e.Closing)
                    .HasColumnName("closing")
                    .HasColumnType("char(1)");

                entity.Property(e => e.ClusterAdminM)
                    .HasColumnName("cluster_admin_M")
                    .HasColumnType("char(1)");

                entity.Property(e => e.Cwrf)
                    .HasColumnName("cwrf")
                    .HasColumnType("char(1)");

                entity.Property(e => e.CwrfCwd)
                    .HasColumnName("cwrf_cwd")
                    .HasColumnType("char(1)");

                entity.Property(e => e.CwrfFunding)
                    .HasColumnName("cwrf_funding")
                    .HasColumnType("char(1)");

                entity.Property(e => e.CwrfHo)
                    .HasColumnName("cwrf_ho")
                    .HasColumnType("char(1)");

                entity.Property(e => e.CwrfHpd)
                    .HasColumnName("cwrf_hpd")
                    .HasColumnType("char(1)");

                entity.Property(e => e.CwrfStatus)
                    .HasColumnName("cwrf_status")
                    .HasColumnType("char(1)");

                entity.Property(e => e.CwrfSubmenu)
                    .HasColumnName("cwrf_submenu")
                    .HasColumnType("char(1)");

                entity.Property(e => e.InputBy)
                    .IsRequired()
                    .HasColumnName("input_by")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.InputDate)
                    .HasColumnName("input_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.NonPjtReport)
                    .HasColumnName("non_pjt_report")
                    .HasColumnType("char(1)");

                entity.Property(e => e.Project)
                    .HasColumnName("project")
                    .HasColumnType("char(1)");

                entity.Property(e => e.ReportM)
                    .HasColumnName("report_M")
                    .HasColumnType("char(1)");

                entity.Property(e => e.UpdateBy)
                    .IsRequired()
                    .HasColumnName("update_by")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UpdateDate)
                    .HasColumnName("update_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.UserGroup)
                    .IsRequired()
                    .HasColumnName("user_group")
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<AccessRightY>(entity =>
            {
                entity.HasKey(e => e.LoginId);

                entity.ToTable("access_right_y");

                entity.Property(e => e.LoginId)
                    .HasColumnName("login_id")
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.AdminY)
                    .HasColumnName("admin_Y")
                    .HasColumnType("char(1)");

                entity.Property(e => e.AsoiInput)
                    .HasColumnName("asoi_input")
                    .HasColumnType("char(1)");

                entity.Property(e => e.AsoiRpt)
                    .HasColumnName("asoi_rpt")
                    .HasColumnType("char(1)");

                entity.Property(e => e.Closing)
                    .HasColumnName("closing")
                    .HasColumnType("char(1)");

                entity.Property(e => e.FarAccess)
                    .HasColumnName("far_access")
                    .HasColumnType("char(1)");

                entity.Property(e => e.FvCluster)
                    .HasColumnName("fv_cluster")
                    .HasColumnType("char(1)");

                entity.Property(e => e.FvInput)
                    .HasColumnName("fv_input")
                    .HasColumnType("char(1)");

                entity.Property(e => e.FvUserAdmin)
                    .HasColumnName("fv_user_admin")
                    .HasColumnType("char(1)");

                entity.Property(e => e.InputBy)
                    .IsRequired()
                    .HasColumnName("input_by")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.InputDate)
                    .HasColumnName("input_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.NonPjtReport)
                    .HasColumnName("non_pjt_report")
                    .HasColumnType("char(1)");

                entity.Property(e => e.ReportY)
                    .HasColumnName("report_Y")
                    .HasColumnType("char(1)");

                entity.Property(e => e.UpdateBy)
                    .IsRequired()
                    .HasColumnName("update_by")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UpdateDate)
                    .HasColumnName("update_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.UserGroup)
                    .IsRequired()
                    .HasColumnName("user_group")
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<CwrfAccessControl>(entity =>
            {
                entity.HasKey(e => new { e.UserName, e.TranId })
                    .ForSqlServerIsClustered(false);

                entity.ToTable("cwrf_access_control");

                entity.HasIndex(e => new { e.UserName, e.TranId })
                    .HasName("IDX_cwrf_access_control")
                    .IsUnique()
                    .ForSqlServerIsClustered();

                entity.Property(e => e.UserName)
                    .HasColumnName("user_name")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TranId).HasColumnName("tran_id");
            });

            modelBuilder.Entity<DonationBalance>(entity =>
            {
                entity.HasKey(e => new { e.Hospital, e.Fund, e.Section, e.Analytical, e.InputFor });

                entity.ToTable("donation_balance");

                entity.Property(e => e.Hospital)
                    .HasColumnName("hospital")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Fund)
                    .HasColumnName("fund")
                    .HasColumnType("char(2)");

                entity.Property(e => e.Section)
                    .HasColumnName("section")
                    .HasColumnType("char(7)");

                entity.Property(e => e.Analytical)
                    .HasColumnName("analytical")
                    .HasColumnType("char(5)");

                entity.Property(e => e.InputFor)
                    .HasColumnName("input_for")
                    .HasColumnType("datetime");

                entity.Property(e => e.CrBy)
                    .IsRequired()
                    .HasColumnName("cr_by")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CrDt)
                    .HasColumnName("cr_dt")
                    .HasColumnType("datetime");

                entity.Property(e => e.Expenditure)
                    .HasColumnName("expenditure")
                    .HasColumnType("money");

                entity.Property(e => e.Income)
                    .HasColumnName("income")
                    .HasColumnType("money");

                entity.Property(e => e.ReserveBalBegin)
                    .HasColumnName("reserve_bal_begin")
                    .HasColumnType("money");

                entity.Property(e => e.ReserveBalEnd)
                    .HasColumnName("reserve_bal_end")
                    .HasColumnType("money");

                entity.Property(e => e.UpdtBy)
                    .IsRequired()
                    .HasColumnName("updt_by")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UpdtDt)
                    .HasColumnName("updt_dt")
                    .HasColumnType("datetime");
            });

            modelBuilder.Entity<DonationDetail>(entity =>
            {
                entity.ToTable("donation_detail");

                entity.HasIndex(e => new { e.DonType, e.Id, e.Hospital, e.Trust, e.Fund, e.DonIncExp, e.DonPurpose, e.DonSupercat })
                    .HasName("idx_donation_detail");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Analytical)
                    .IsRequired()
                    .HasColumnName("analytical")
                    .HasColumnType("char(5)");

                entity.Property(e => e.DonCat).HasColumnName("don_cat");

                entity.Property(e => e.DonCurMth)
                    .HasColumnName("don_cur_mth")
                    .HasColumnType("money");

                entity.Property(e => e.DonIncExp)
                    .IsRequired()
                    .HasColumnName("don_inc_exp")
                    .HasColumnType("char(1)");

                entity.Property(e => e.DonKindDesc)
                    .HasColumnName("don_kind_desc")
                    .HasMaxLength(1000);

                entity.Property(e => e.DonPurpose).HasColumnName("don_purpose");

                entity.Property(e => e.DonSpecific)
                    .HasColumnName("don_specific")
                    .HasMaxLength(200);

                entity.Property(e => e.DonSubcat).HasColumnName("don_subcat");

                entity.Property(e => e.DonSubsubcat).HasColumnName("don_subsubcat");

                entity.Property(e => e.DonSupercat).HasColumnName("don_supercat");

                entity.Property(e => e.DonType)
                    .IsRequired()
                    .HasColumnName("don_type")
                    .HasColumnType("char(1)");

                entity.Property(e => e.DonorId).HasColumnName("donor_id");

                entity.Property(e => e.DonorName)
                    .HasColumnName("donor_name")
                    .HasMaxLength(500);

                entity.Property(e => e.DonorType)
                    .HasColumnName("donor_type")
                    .HasColumnType("char(1)");

                entity.Property(e => e.EqtDesc).HasColumnName("eqt_desc");

                entity.Property(e => e.Fund)
                    .IsRequired()
                    .HasColumnName("fund")
                    .HasColumnType("char(2)");

                entity.Property(e => e.Hospital)
                    .IsRequired()
                    .HasColumnName("hospital")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.InputAt)
                    .HasColumnName("input_at")
                    .HasColumnType("datetime");

                entity.Property(e => e.InputBy)
                    .IsRequired()
                    .HasColumnName("input_by")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.LinkId).HasColumnName("link_id");

                entity.Property(e => e.MajDon1)
                    .HasColumnName("maj_don1")
                    .HasColumnType("char(1)");

                entity.Property(e => e.MajDon2)
                    .HasColumnName("maj_don2")
                    .HasColumnType("char(1)");

                entity.Property(e => e.MajDon3)
                    .HasColumnName("maj_don3")
                    .HasColumnType("char(1)");

                entity.Property(e => e.RecurrentCon)
                    .HasColumnName("recurrent_con")
                    .HasColumnType("char(1)");

                entity.Property(e => e.RecurrentCost)
                    .HasColumnName("recurrent_cost")
                    .HasColumnType("money");

                entity.Property(e => e.Reimb)
                    .HasColumnName("reimb")
                    .HasColumnType("char(1)");

                entity.Property(e => e.Section)
                    .IsRequired()
                    .HasColumnName("section")
                    .HasColumnType("char(7)");

                entity.Property(e => e.Trust).HasColumnName("trust");

                entity.Property(e => e.UpdateAt)
                    .HasColumnName("update_at")
                    .HasColumnType("datetime");

                entity.Property(e => e.UpdateBy)
                    .IsRequired()
                    .HasColumnName("update_by")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<DonCat>(entity =>
            {
                entity.ToTable("don_cat");

                entity.HasIndex(e => e.Id)
                    .HasName("IDX_don_cat")
                    .IsUnique()
                    .ForSqlServerIsClustered();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.Designated)
                    .HasColumnName("designated")
                    .HasColumnType("char(1)");

                entity.Property(e => e.DisplayOrder).HasColumnName("display_order");

                entity.Property(e => e.SupercatId).HasColumnName("supercat_id");
            });

            modelBuilder.Entity<DonDonor>(entity =>
            {
                entity.ToTable("don_donor");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasMaxLength(255);

                entity.Property(e => e.DisplayOrder).HasColumnName("display_order");
            });

            modelBuilder.Entity<DonPurpose>(entity =>
            {
                entity.ToTable("don_purpose");

                entity.HasIndex(e => e.Id)
                    .HasName("IDX_don_purpose")
                    .IsUnique()
                    .ForSqlServerIsClustered();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.DisplayOrder).HasColumnName("display_order");
            });

            modelBuilder.Entity<DonSubcat>(entity =>
            {
                entity.ToTable("don_subcat");

                entity.HasIndex(e => e.Id)
                    .HasName("IDX_don_subcat")
                    .IsUnique()
                    .ForSqlServerIsClustered();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CatId).HasColumnName("cat_id");

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.DisplayOrder).HasColumnName("display_order");

                entity.Property(e => e.Specify).HasColumnName("specify");
            });

            modelBuilder.Entity<DonSubsubcat>(entity =>
            {
                entity.ToTable("don_subsubcat");

                entity.HasIndex(e => e.Id)
                    .HasName("IDX_don_subsubcat")
                    .IsUnique()
                    .ForSqlServerIsClustered();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.DisplayOrder).HasColumnName("display_order");

                entity.Property(e => e.Specify).HasColumnName("specify");

                entity.Property(e => e.SubcatId).HasColumnName("subcat_id");
            });

            modelBuilder.Entity<DonSupercat>(entity =>
            {
                entity.HasKey(e => e.SupercatId);

                entity.ToTable("don_supercat");

                entity.HasIndex(e => e.SupercatId)
                    .HasName("idx_don_supercat");

                entity.Property(e => e.SupercatId)
                    .HasColumnName("supercat_id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.DisplayOrder).HasColumnName("display_order");
            });

            modelBuilder.Entity<DonType>(entity =>
            {
                entity.ToTable("don_type");

                entity.HasIndex(e => e.Id)
                    .HasName("IDX_don_type")
                    .IsUnique()
                    .ForSqlServerIsClustered();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.DisplayOrder).HasColumnName("display_order");
            });

            modelBuilder.Entity<Hospital>(entity =>
            {
                entity.HasKey(e => e.HospitalCode)
                    .ForSqlServerIsClustered(false);

                entity.ToTable("hospital");

                entity.HasIndex(e => e.HospitalCode)
                    .HasName("IDX_hospital")
                    .IsUnique()
                    .ForSqlServerIsClustered();

                entity.Property(e => e.HospitalCode)
                    .HasColumnName("hospital_code")
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.AnnualCostingInd)
                    .HasColumnName("annual_costing_ind")
                    .HasColumnType("char(1)");

                entity.Property(e => e.Cce)
                    .HasColumnName("cce")
                    .HasColumnType("char(1)");

                entity.Property(e => e.Cluster)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.DdGp)
                    .HasColumnName("DD_gp")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FlashPgNo).HasColumnName("flash_pg_no");

                entity.Property(e => e.HeadofficeInd)
                    .HasColumnName("headoffice_ind")
                    .HasColumnType("char(1)");

                entity.Property(e => e.PeBySpecInd)
                    .HasColumnName("pe_by_spec_ind")
                    .HasColumnType("char(1)");

                entity.Property(e => e.ShowHpsInd)
                    .HasColumnName("show_hps_ind")
                    .HasColumnType("char(1)");
            });

            modelBuilder.Entity<SuppUserGroup>(entity =>
            {
                entity.HasKey(e => e.LoginId)
                    .ForSqlServerIsClustered(false);

                entity.ToTable("supp_user_group");

                entity.HasIndex(e => new { e.LoginId, e.FmrsSystem })
                    .HasName("IDX_supp_user_group")
                    .IsUnique()
                    .ForSqlServerIsClustered();

                entity.Property(e => e.LoginId)
                    .HasColumnName("login_id")
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.FmrsSystem)
                    .IsRequired()
                    .HasColumnName("fmrs_system")
                    .HasColumnType("char(1)");

                entity.Property(e => e.InstCode)
                    .IsRequired()
                    .HasColumnName("inst_code")
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.UserGroup)
                    .IsRequired()
                    .HasColumnName("user_group")
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<UserGroup>(entity =>
            {
                entity.HasKey(e => e.UserGroup1)
                    .ForSqlServerIsClustered(false);

                entity.ToTable("user_group");

                entity.HasIndex(e => e.UserGroup1)
                    .HasName("IDX_user_group")
                    .IsUnique()
                    .ForSqlServerIsClustered();

                entity.Property(e => e.UserGroup1)
                    .HasColumnName("user_group")
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.AllInstCodeInd)
                    .IsRequired()
                    .HasColumnName("all_inst_code_ind")
                    .HasColumnType("char(1)");

                entity.Property(e => e.ConsolidationDesc)
                    .IsRequired()
                    .HasColumnName("consolidation_desc")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnName("description")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.HeadofficeInd)
                    .HasColumnName("headoffice_ind")
                    .HasColumnType("char(1)");
            });

            modelBuilder.Entity<UserGroupHosp>(entity =>
            {
                entity.HasKey(e => new { e.UserGroup, e.HospitalCode })
                    .ForSqlServerIsClustered(false);

                entity.ToTable("user_group_hosp");

                entity.HasIndex(e => new { e.HospitalCode, e.UserGroup })
                    .HasName("IDX2_user_group_hosp");

                entity.HasIndex(e => new { e.UserGroup, e.HospitalCode })
                    .HasName("IDX_user_group_hosp")
                    .IsUnique()
                    .ForSqlServerIsClustered();

                entity.Property(e => e.UserGroup)
                    .HasColumnName("user_group")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.HospitalCode)
                    .HasColumnName("hospital_code")
                    .HasMaxLength(10)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<UserInfo>(entity =>
            {
                entity.HasKey(e => e.LoginId)
                    .ForSqlServerIsClustered(false);

                entity.ToTable("user_info");

                entity.HasIndex(e => e.LoginId)
                    .HasName("user_info")
                    .IsUnique()
                    .ForSqlServerIsClustered();

                entity.Property(e => e.LoginId)
                    .HasColumnName("login_id")
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.DomainInst)
                    .HasColumnName("domain_inst")
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.DomainUser)
                    .HasColumnName("domain_user")
                    .HasColumnType("char(1)");

                entity.Property(e => e.Donation)
                    .HasColumnName("donation")
                    .HasColumnType("char(1)");

                entity.Property(e => e.FinancialClosing)
                    .HasColumnName("financial_closing")
                    .HasColumnType("char(1)");

                entity.Property(e => e.HoUser)
                    .HasColumnName("ho_user")
                    .HasColumnType("char(1)");

                entity.Property(e => e.InputBy)
                    .HasColumnName("input_by")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.InputDate)
                    .HasColumnName("input_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.InstCode)
                    .IsRequired()
                    .HasColumnName("inst_code")
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.LastLogin)
                    .HasColumnName("last_login")
                    .HasColumnType("datetime");

                entity.Property(e => e.Password)
                    .HasColumnName("password")
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.ProjectManagement)
                    .HasColumnName("project_management")
                    .HasColumnType("char(1)");

                entity.Property(e => e.PwdExpiry)
                    .IsRequired()
                    .HasColumnName("pwd_expiry")
                    .HasColumnType("char(1)");

                entity.Property(e => e.UpdateBy)
                    .HasColumnName("update_by")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UpdateDate)
                    .HasColumnName("update_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.UserGroup)
                    .IsRequired()
                    .HasColumnName("user_group")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.UserName)
                    .HasColumnName("user_name")
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<UserSpecialty>(entity =>
            {
                entity.HasKey(e => e.LoginId);

                entity.ToTable("user_specialty");

                entity.Property(e => e.LoginId)
                    .HasColumnName("login_id")
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.Specialty)
                    .IsRequired()
                    .HasColumnName("specialty")
                    .HasMaxLength(24)
                    .IsUnicode(false);
            });
        }
    }
}
