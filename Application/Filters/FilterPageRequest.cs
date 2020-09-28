using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Filters
{
    public class FilterPageRequest
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public FilterPageRequest()
        {
            this.PageNumber = 1;
            this.PageSize = 10;
        }
        public FilterPageRequest(int pageNumber, int pageSize)
        {
            this.PageNumber = pageNumber < 1 ? 1 : pageNumber;
            this.PageSize = pageSize > 10 ? 10 : pageSize;
        }
    }
}
