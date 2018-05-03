using System;
using System.Collections.Generic;
using System.Text;

namespace FMRS.Model.DTO
{
    public class ProjectReportModel
    {
        public string Report_id { get; set; }
        public string Rpt_name { get; set; }
        public string Show_project_item_ind { get; set; }
        public string Cwrf_recur { get; set; }
        public int Cnt { get; set; }
        public List<ProjectReportDetailRightModel> Project_right { get; set; }
    }

    public class ProjectReportViewModel
    {
        public string Admin_login_id { get; set; }
        public int Group_id { get; set; }
        public string Modules { get; set; }
        public List<ProjectReportModel> Report_list { get; set; }
        public string Access_right { get; set; } //ID:Val,ID:Val,ID:Val,ID:Val,
    }

    public class ProjectReportDetailViewModel
    {
        public string Admin_login_id { get; set; }
        public int Group_id { get; set; }
        public string Modules { get; set; }
        public string Detail_type { get; set; }
        public ProjectReportModel Report { get; set; }
        public string Access_right { get; set; } //ID:Val,ID:Val,ID:Val,ID:Val,
    }

    public class ProjectReportDetailRightModel
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public string Id_type { get; set; }
        public int Cnt { get; set; } 
    }
}
