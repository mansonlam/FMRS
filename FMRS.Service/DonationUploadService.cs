using FMRS.DAL.Repository;
using FMRS.Common.Resources;
using System;
using System.Collections.Generic;
using System.Text;
using FMRS.Model.DTO;
using System.Linq;
using System.Globalization;
using CsvHelper.Configuration;
using System.IO;

namespace FMRS.Service
{
    public interface IDonationUploadService
    {
        string GetErrorMsg(int record_error);
        string Go_end_line(string remark);
        string GetRecordArray(string remark);
        string[] GetRecordItem(string row);
        List<DonationUploadRecord> GetRecordList(string remark, DateTime current_date, string value_date, out int rec_cnt, out int blank_line);
        List<DonationUploadRecord> RecordValidation(List<DonationUploadRecord> record_list, string user_group, string user_inst_code, DateTime current_date, string value_date, int financial_year, out int valid_rec_cnt, out int invalid_rec_cnt);
        string GetLine(List<DonationUploadRecord> record_list, string function_mode, int show_valid_only, string login_id, string value_date);
        void ExecuteUploadSQL(string upload_strsql);
    }

    public class DonationUploadService : IDonationUploadService
    {
        private IDonationBalanceRepository DonationBalanceRepository;
        private IDonationDetailRepository DonationDetailRepository;
        private IDonationHistoryRepository DonationHistoryRepository;
        private IDonationReserveRepository DonationReserveRepository;
        private IDonCatRepository DonCatRepository;
        private IDonDonorRepository DonDonorRepository;
        private IDonPurposeRepository DonPurposeRepository;
        private IDonSubcatRepository DonSubcatRepository;
        private IDonSubsubcatRepository DonSubsubcatRepository;
        private IDonSupercatRepository DonSupercatRepository;
        private IDonTypeRepository DonTypeRepository;
        private IUserGroupHospRespository UserGroupHospRespository;
        

        public DonationUploadService(IDonationBalanceRepository _donationBalanceRepository, IDonationDetailRepository _donationDetailRepository,
                                      IDonationHistoryRepository _donationHistoryRepository, IDonationReserveRepository _donationReserveRepository,
                                      IDonCatRepository _donCatRepository, IDonDonorRepository _donDonorRepository,
                                      IDonPurposeRepository _donPurposeRepository, IDonSubcatRepository _donSubcatRepository,
                                      IDonSubsubcatRepository _donSubsubcatRepository, IDonSupercatRepository _donSupercatRepository,
                                      IDonTypeRepository _donTypeRepository, IUserGroupHospRespository _userGroupHospRespository
                                     )
        {
            DonationBalanceRepository = _donationBalanceRepository;
            DonationDetailRepository = _donationDetailRepository;
            DonationHistoryRepository = _donationHistoryRepository;
            DonationReserveRepository = _donationReserveRepository;
            DonCatRepository = _donCatRepository;
            DonDonorRepository = _donDonorRepository;
            DonPurposeRepository = _donPurposeRepository;
            DonSubcatRepository = _donSubcatRepository;
            DonSubsubcatRepository = _donSubsubcatRepository;
            DonSupercatRepository = _donSupercatRepository;
            DonTypeRepository = _donTypeRepository;
            UserGroupHospRespository = _userGroupHospRespository;
        }

        public string GetErrorMsg(int record_error)
        {
            string message = "";
            switch (record_error)
            {
                case 1:
                    message = Resource.InvalidSubCat1;
                    break;
                case 2:
                    message = Resource.InvalidSubCat2;
                    break;
                case 3:
                    message = Resource.InvalidSubCat2;
                    break;
                case 4:
                    message = Resource.InvalidSubCat3;
                    break;
                case 5:
                    message = Resource.InvalidDonType;
                    break;
                case 6:
                    message = Resource.InvalidDonPurpose;
                    break;
                case 7:
                    message = Resource.InvalidRecurrentCost;
                    break;
                case 8:
                    message = Resource.InvalidHospital;
                    break;
                case 9:
                    message = Resource.HospitalNotHaveTrust;
                    break;
                case 10:
                    message = Resource.InvalidDonationCM;
                    break;
                case 11:
                    message = Resource.InvalidOutstandCommitment;
                    break;
                case 12:
                    message = Resource.InvalidDonationDate;
                    break;
                case 13:
                    message = Resource.DonationDateFutureNotAllowed;
                    break;
                case 14:
                    message = Resource.DonationNotInCurYear;
                    break;
                case 15:
                    message = Resource.DonationNotInCurMon;
                    break;
                case 16:
                    message = Resource.DonationDescMustNotEmpty;
                    break;
                case 17:
                    message = Resource.ReceiverAndDonorAreTrust;
                    break;
                case 18:
                    message = Resource.FundShouldBe212252;
                    break;
                case 19:
                    message = Resource.InvalidSpecificDonation;
                    break;
                case 20:
                    message = Resource.InvalidSection;
                    break;
                case 21:
                    message = Resource.InvalidAnalytical;
                    break;
                case 22:
                    message = Resource.InvalidMajorDonation1;
                    break;
                case 23:
                    message = Resource.InvalidMajorDonation2;
                    break;
                case 24:
                    message = Resource.InvalidMajorDonation3;
                    break;
                case 25:
                    message = Resource.COANotInDonBalInCurMon;
                    break;
                case 26:
                    message = Resource.InvalidIncomeExpenditure;
                    break;
                case 27:
                    message = Resource.DonRecOrExpNotLessThanOutstandingAmtForCurMon;
                    break;
                case 28:
                    message = Resource.DonationInCurMonNotNegative;
                    break;
                case 29:
                    message = Resource.ExpenditureInCurMonNotPositive;
                    break;
            }
            return message;
        }
        public string Go_end_line(string remark)
        {
            int start_po = remark.IndexOf((char)13);
            remark = remark.Substring(start_po + 2);
            return remark;
        }

        public string GetRecordArray(string remark)
        {
            return Go_end_line(remark).Replace("\r\n","******"); 
        }

        public string[] GetRecordItem(string row)
        {
            string[] item = new string[20];

            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(row));
            TextReader tr = new StreamReader(stream);
            var csv = new CsvHelper.CsvReader(tr);
            csv.Read();
            csv.Configuration.MissingFieldFound = null;
            //var record = csv.GetRecord<DonationUploadRecord>();
            for (int i = 0; i < 20; i++)
            { item[i] = csv[i]??""; }

            return item;
        }

        public List<DonationUploadRecord> GetRecordList(string remark, DateTime current_date, string value_date, out int rec_cnt, out int blank_line)
        {
            rec_cnt = 0;
            blank_line = 0;
            string[] row_list = remark.Split("******");
            List<DonationUploadRecord> result_list = new List<DonationUploadRecord>();
            for (int i = 0; i < row_list.Length; i++)
            {
                if(row_list[i].Length > 0) {
                    string endline = "N";
                    DonationUploadRecord model = new DonationUploadRecord();
                    string[] item = GetRecordItem(row_list[i]);
                    model.Hospital = item[0];           //A
                    if (model.Hospital != "")
                    {
                        model.Fund = item[1];               //B
                        model.Section = item[2];            //C
                        model.Analytical = item[3];         //D
                        model.Trust = model.Fund == "52" ? 1 : 0;
                        model.Donor_name = endline == "N" ? item[4] : "";         //E
                        if (endline == "N")
                        {
                            if (DonDonorRepository.GetDonorCnt(model.Donor_name) <= 0)
                                model.Donor_id = 0;
                            else
                                model.Donor_id = DonDonorRepository.GetDonorByDesc(model.Donor_name).Id;
                        }
                        if (endline == "N")
                        {
                            model.Don_inc_exp = item[5];        //F
                            if (model.Don_inc_exp == "")
                                model.Don_inc_exp = "I";
                        }
                        else
                        {
                            model.Don_inc_exp = "I";
                        }
                        if (endline == "N")
                        {
                            model.Don_type_c = item[6];         //G
                            if (string.IsNullOrEmpty(model.Don_type_c))
                            {
                                model.Don_type_c = "2";
                                model.Don_type = 2;
                            }
                            else if (Int32.TryParse(model.Don_type_c, out int number))
                            {
                                model.Don_type = number;
                            }
                            else
                            {
                                model.Don_type = 2;
                            }

                        }
                        else
                        {
                            model.Don_type = 0;
                        }
                        if (endline == "N")
                        {
                            model.Don_date_c = item[7];         //H
                            if (model.Don_date_c == "")
                            {
                                model.Don_date = current_date;
                                model.Don_date_c = current_date.ToString("yyyy/MM/dd");
                            }
                            else
                            {
                                if (model.Don_date_c.Length == 8)
                                {
                                    string date = "20" + model.Don_date_c.Substring(6, 2) + model.Don_date_c.Substring(2, 2) + model.Don_date_c.Substring(0, 2);
                                    model.Don_date = DateTime.ParseExact(date, "yyyyMMdd", CultureInfo.InvariantCulture);
                                }
                                else if (model.Don_date_c.Length == 10)
                                {
                                    string date = model.Don_date_c.Substring(6, 4) + model.Don_date_c.Substring(3, 2) + model.Don_date_c.Substring(0, 2);
                                    model.Don_date = DateTime.ParseExact(date, "yyyyMMdd", CultureInfo.InvariantCulture);
                                }
                                else
                                {
                                    model.Don_date = current_date;
                                }
                            }
                        }
                        else
                        {
                            model.Don_date_c = "10/10/2014";
                            model.Don_date = DateTime.ParseExact("10102014", "ddMMyyyy", CultureInfo.InvariantCulture);
                        }
                        if (endline == "N")
                        {
                            model.Don_purpose_c = item[8];      //I
                            if (string.IsNullOrEmpty(model.Don_purpose_c))
                            {
                                model.Don_purpose_c = "2";
                                model.Don_purpose = 2;
                            }
                            else if (Int32.TryParse(model.Don_purpose_c, out int number))
                            {
                                model.Don_purpose = number;
                            }
                            else
                            {
                                model.Don_purpose = 2;
                            }
                        }
                        else
                        {
                            model.Don_purpose = 0;
                        }

                        model.Don_super_cat = 0;
                        model.Don_super_cat_desc = endline == "N" ? item[9] : ""; //J
                        model.Don_cat = 0;
                        model.Don_cat_desc = endline == "N" ? item[10] : "";       //K
                        model.Don_subcat = 0;
                        model.Don_subcat_desc = endline == "N" ? item[11] : "";   //L
                        model.Don_subsubcat = 0;
                        model.Don_subsubcat_desc = endline == "N" ? item[12] : ""; //M 
                        model.Don_specific = endline == "N" ? item[13] : "";      //N
                        if (endline == "N")
                        {
                            model.Maj_don1 = item[14];          //O
                            model.Maj_don2 = item[15];          //P
                            model.Maj_don3 = item[16];          //Q
                            model.Reimb = item[17];             //R
                        }
                        model.Don_kind_desc = endline == "N" ? item[18] : "";     //S
                        if (endline == "N")
                        {
                            model.Don_cur_mth_c = item[19];     //T
                            if (string.IsNullOrEmpty(model.Don_cur_mth_c))
                            {
                                model.Don_cur_mth_c = "0";
                                model.Don_cur_mth = 0;
                            }
                            else if (Decimal.TryParse(item[19], out decimal number))
                            {
                                model.Don_cur_mth = number;
                            }
                            else
                            {
                                model.Don_cur_mth = 0;
                            }
                        }
                        else
                        {
                            model.Don_cur_mth_c = "0";
                            model.Don_cur_mth = 0;
                        }

                        endline = "Y";

                        var ds = DonationBalanceRepository.GetReserveBal(model.Hospital, model.Fund, model.Section, model.Analytical, value_date);
                        if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count != 0)
                        {
                            model.R_begin = Convert.ToDecimal(ds.Tables[0].Rows[0]["reserve_bal_begin"]);
                            model.R_end = Convert.ToDecimal(ds.Tables[0].Rows[0]["reserve_bal_end"]);
                        }
                        else
                        {
                            model.R_begin = 0;
                        }

                        result_list.Add(model);
                        rec_cnt = rec_cnt + 1;
                    }
                    else
                    {
                        blank_line = blank_line + 1;
                    }

                }
            }
            return result_list;
        }

        public List<DonationUploadRecord> RecordValidation(List<DonationUploadRecord> record_list, string user_group, string user_inst_code, DateTime current_date, string value_date, int financial_year, out int valid_rec_cnt, out int invalid_rec_cnt)
        {
            valid_rec_cnt = 0;
            invalid_rec_cnt = 0;
            foreach (var r in record_list)
            {
                    if (r.Record_error == 0)
                    {
                        var ds = DonSupercatRepository.GetSuperCatByDesc(r.Don_super_cat_desc);
                        if (ds == null)
                            r.Record_error = 19;
                        else
                            r.Don_super_cat = ds.SupercatId;
                        if (r.Don_super_cat == 0)
                            r.Don_super_cat = 7;
                    }

                    if (r.Record_error == 0)
                    {
                        var ds = DonCatRepository.GetDonCatByDesc(r.Don_cat_desc);
                        if (ds == null)
                            r.Record_error = 1;
                        else
                            r.Don_cat = ds.Id;
                        if (r.Don_cat == 0)
                            r.Don_cat = 7;
                    }

                    if (r.Record_error == 0)
                    {
                        if (r.Don_subcat_desc != "")
                        {
                            var ds = DonSubcatRepository.GetDonSubcatByCatIdDesc(r.Don_subcat_desc, r.Don_cat);
                            if (ds == null)
                            {
                                if (r.Don_specific != "")
                                {
                                    var ds2 = DonSubcatRepository.GetDonSubcatByCatIdSpec(r.Don_cat);
                                    if (ds2 != null)
                                        r.Don_subcat = ds2.Id;
                                }
                                else
                                    r.Record_error = 2;
                            }
                            else
                            {
                                r.Don_subcat = ds.Id;
                            }
                        }
                        else
                        {
                            r.Don_subcat = 0;
                        }

                        if (r.Don_subcat == 0)
                        {
                            var ds = DonSubcatRepository.GetDonSubcatByCatId(r.Don_cat).Count();
                            if (ds > 0)
                                r.Record_error = 3;
                        }
                    }

                    if (r.Record_error == 0)
                    {
                        if (r.Don_subsubcat_desc != "")
                        {
                            var ds = DonSubsubcatRepository.GetDonSubsubcatBySubCatIdDesc(r.Don_subsubcat_desc, r.Don_subcat);
                            if (ds == null)
                            {
                                r.Record_error = 4;
                            }
                            else
                                r.Don_subsubcat = ds.Id;
                        }
                        else
                            r.Don_subsubcat = 0;
                    }

                    if (r.Record_error == 0)
                    {
                        if (Int32.TryParse(r.Don_type_c, out int number))
                        {
                            r.Don_type = number;
                        }
                        else
                        {
                            r.Don_type = 2;
                            r.Record_error = 5;
                        }
                        if (r.Record_error == 0)
                        {
                            var ds = DonTypeRepository.GetDonTypeById(r.Don_type).Count();
                            if (ds <= 0)
                                r.Record_error = 5;
                        }
                    }

                    if (r.Record_error == 0)
                    {
                        if (Int32.TryParse(r.Don_purpose_c, out int number))
                        {
                            r.Don_purpose = number;
                        }
                        else
                        {
                            r.Don_purpose = 2;
                            r.Record_error = 6;
                        }
                        if (r.Record_error == 0)
                        {
                            var ds = DonPurposeRepository.GetDonPurposeById(r.Don_purpose).Count();
                            if (ds <= 0)
                                r.Record_error = 6;
                        }
                    }

                    if (r.Record_error == 0)
                    {
                        if (r.Recurrent_con == "N")
                        {
                            r.Recurrent_cost = 0;
                        }
                        else if (String.IsNullOrEmpty(r.Recurrent_cost_c))
                        {
                            r.Recurrent_cost = 0;
                        }
                        else if (Int32.TryParse(r.Recurrent_cost_c, out int number))
                        {
                            r.Recurrent_cost = number;
                        }
                        else
                        {
                            r.Record_error = 7;
                            r.Recurrent_cost = 0;
                        }
                    }

                    if (r.Record_error == 0)
                    {
                        if (user_group != "HOSP")
                        {
                            var ds = UserGroupHospRespository.GetUserGpHospByGpHosp(user_group, r.Hospital).Count();
                            if (ds <= 0)
                                r.Record_error = 8;
                        }
                        else
                        {
                            if (r.Hospital != user_inst_code)
                                r.Record_error = 8;
                        }
                    }

                    if (r.Trust == 1 && r.Donor_type == "T")
                        r.Record_error = 17;

                    if (r.Record_error == 0)
                    {
                        if (string.IsNullOrEmpty(r.Don_cur_mth_c))
                        {
                            r.Don_cur_mth_c = "0";
                            r.Don_cur_mth = 0;
                        }
                        else if (Decimal.TryParse(r.Don_cur_mth_c, out decimal number))
                        {
                            r.Don_cur_mth = number;
                        }
                        else
                        {
                            r.Don_cur_mth = 0;
                            r.Record_error = 10;
                        }
                    }

                    if (r.Record_error == 0)
                    {
                        if (string.IsNullOrEmpty(r.Out_comm_c))
                        {
                            r.Out_comm_c = "0";
                            r.Out_comm = 0;
                        }
                        else if (Int32.TryParse(r.Out_comm_c, out int number))
                        {
                            r.Out_comm = number;
                        }
                        else
                        {
                            r.Out_comm = 0;
                            r.Record_error = 11;
                        }
                    }

                    if (r.Record_error == 0)
                    {
                        if (r.Don_date_c.Trim().Length != 8 && r.Don_date_c.Trim().Length != 10)
                            r.Record_error = 12;
                        //if (r.Record_error == 0)
                        //{
                        //    if (!Int32.TryParse(r.Don_date_c, out int number))
                        //        r.Record_error = 12;
                        //}
                        //if (r.Record_error == 0)
                        //{
                        //    bool don_date_result = Int32.TryParse(r.Don_date_c, out int number);
                        //    if (!(number >= 1000000 && number <= 9999999))
                        //        r.Record_error = 12;
                        //}
                    }

                    if (r.Record_error == 0)
                    {
                        if (r.Don_date > current_date)
                            r.Record_error = 13;
                        if (r.Don_date < DateTime.ParseExact(financial_year + "0401", "yyyyMMdd", CultureInfo.InvariantCulture))
                            r.Record_error = 14;
                        if (r.Don_date.ToString("yyyyMM") != value_date.Substring(0, 6))
                            r.Record_error = 15;
                    }

                    if (r.Record_error == 0)
                    {
                        if (r.Don_kind_desc.Trim() == "")
                            r.Record_error = 16;
                    }

                    if (r.Record_error == 0)
                    {
                        if (r.Fund != "21" && r.Fund != "22" && r.Fund != "52")
                            r.Record_error = 18;
                    }

                    if (r.Record_error == 0)
                    {
                        if (r.Section.Length != 7)
                            r.Record_error = 20;
                    }

                    if (r.Record_error == 0)
                    {
                        if (r.Analytical.Length != 5)
                            r.Record_error = 21;
                    }

                    if (r.Record_error == 0)
                    {
                        if (r.Maj_don1 != "Y" && r.Maj_don1 != "N")
                            r.Record_error = 22;
                    }

                    if (r.Record_error == 0)
                    {
                        if (r.Maj_don2 != "Y" && r.Maj_don2 != "N")
                            r.Record_error = 23;
                    }

                    if (r.Record_error == 0)
                    {
                        if (r.Maj_don3 != "Y" && r.Maj_don3 != "N")
                            r.Record_error = 24;
                    }

                    if (r.Record_error == 0)
                    {
                        var ds = DonationBalanceRepository.GetDonationBalance(r.Hospital, r.Fund, r.Section, r.Analytical, value_date).Count();
                        if (ds <= 0)
                            r.Record_error = 25;
                    }

                    if (r.Record_error == 0)
                    {
                        if (r.Don_inc_exp != "I" && r.Don_inc_exp != "E" && r.Don_inc_exp != "")
                            r.Record_error = 26;
                    }

                    if (r.Record_error == 0)
                    {
                        var donation_bal = DonationBalanceRepository.GetDonationBalance(r.Hospital, r.Fund, r.Section, r.Analytical, value_date).First();
                        if (r.Don_inc_exp == "I")
                        {
                            var income_bal = donation_bal.Income;
                            var income_input = DonationDetailRepository.GetIncomeInput(r.Hospital, r.Fund, r.Section, r.Analytical, value_date);
                            if (income_bal - income_input < r.Don_cur_mth)
                                r.Record_error = 27;
                        }
                        else
                        {
                            var exp_bal = donation_bal.Expenditure;
                            var exp_input = DonationDetailRepository.GetExpenditure(r.Hospital, r.Fund, r.Section, r.Analytical, value_date);
                            if (exp_bal - exp_input > r.Don_cur_mth)
                                r.Record_error = 27;
                        }
                    }

                    if (r.Record_error == 0)
                    {
                        var outstanding_SP = DonationDetailRepository.GetOutstanding(r.Hospital, r.Fund, r.Section, r.Analytical, value_date, r.Don_inc_exp);
                        if (r.Don_inc_exp == "I")
                        {
                            if (outstanding_SP - r.Don_cur_mth < 0)
                                r.Record_error = 28;
                        }
                        else
                        {
                            if (outstanding_SP - r.Don_cur_mth > 0)
                                r.Record_error = 29;
                        }

                    }

                    if (r.Record_error == 0)
                    {
                        valid_rec_cnt = valid_rec_cnt + 1;
                    }
                    else
                    {
                        invalid_rec_cnt = invalid_rec_cnt + 1;
                    }
                if (r.Record_error != 0)
                    r.Err_msg = GetErrorMsg(r.Record_error);
                
            }
            return record_list;
        }

        public string GetLine(List<DonationUploadRecord> record_list, string function_mode, int show_valid_only, string login_id, string value_date)
        {
            decimal ytd_inc = 0;
            decimal ytd_exp = 0;
            string upload_strsql = "";
            if (function_mode == "C" || function_mode == "U")
            {
                foreach(var r in record_list)
                { 
                    if (function_mode == "U" && r.Record_error == 0)
                    {
                        r.Donor_name = r.Donor_name.Replace("'", "''");
                        r.Don_specific = r.Don_specific.Replace("'", "''");
                        r.Don_kind_desc = r.Don_kind_desc.Replace("'", "''");
                        upload_strsql = upload_strsql + DonationDetailRepository.InsertDonationDetail(r.Hospital, r.Fund, r.Section, r.Analytical, r.Trust, r.Donor_id, r.Donor_name, r.Don_inc_exp, r.Don_type, r.Don_purpose, r.Don_super_cat, r.Don_cat, r.Don_subcat, r.Don_subsubcat, r.Don_specific, r.Maj_don1, r.Maj_don2, r.Maj_don3, r.Reimb, r.Don_kind_desc, r.Don_cur_mth, login_id);
                        upload_strsql = upload_strsql + DonationHistoryRepository.InsertDonationHistory(r.Don_cur_mth, r.Out_comm, r.Don_date, login_id);

                        if (r.Don_inc_exp == "I")
                        {
                            ytd_inc = r.Don_cur_mth;
                            ytd_exp = 0;
                        }
                        else
                        {
                            ytd_inc = 0;
                            ytd_exp = r.Don_cur_mth;
                        }
                        if (r.R_begin != 0 && r.Fund != "21" && r.Don_type != 1)
                        {
                            upload_strsql = upload_strsql + DonationReserveRepository.InsertDonationReserve(r.Hospital, r.Fund, r.Section, r.Analytical, r.Don_kind_desc, r.Donor_name,
                            value_date, r.Don_date, r.Don_super_cat, r.R_begin, ytd_inc, ytd_exp, r.Don_specific);

                            upload_strsql = upload_strsql + DonationReserveRepository.InsertDonationReserve(r.Hospital, r.Fund, r.Section, r.Analytical, r.Don_kind_desc, r.Donor_name,
                            value_date, r.Don_date, r.Don_super_cat, 0, ytd_inc, ytd_exp, r.Don_specific);
                        }
                    }
                }
            }


            return upload_strsql;

        }

        public void ExecuteUploadSQL(string upload_strsql)
        {
            DonationDetailRepository.ExecuteUploadSQL(upload_strsql);
        }
    }
}
