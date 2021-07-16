using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Diary.WebUI.Models
{
    public class FilterQuery
    {
        public int Page { get; set; } = 0;

        public int PageSize { get; set; } = 5;

        public string SearchedText { get; set; }

        public DateTime SelectedStartDate { get; set; }
        public DateTime SelectedEndDate { get; set; }
    }
}
