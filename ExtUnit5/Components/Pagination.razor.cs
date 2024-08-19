using ExtUnit5.Entities;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using static Plotly.NET.StyleParam.BackOff;

namespace ExtUnit5.Components
{
    public partial class Pagination<TItem> : ComponentBase
    {
        [Parameter] public List<TItem> Items { get; set; } = null!;
        [Parameter] public int PageSize { get; set; }
        [Parameter] public int PageNumber { get; set; }
        [Parameter] public EventCallback<int> OnPageChanged { get; set; }

        private int TotalItems => Items.Count;
        private int TotalPages
        {
            get
            {
                var totalPages = (int)Math.Ceiling((double)TotalItems / PageSize);

                if (totalPages < PageNumber)
                {
                    PageNumber = totalPages;
                    OnPageChanged.InvokeAsync(PageNumber);
                }
                return totalPages;
            }
        }

        private bool IsFirstPage => PageNumber == 1;
        private bool IsLastPage => PageNumber == TotalPages;
        private IEnumerable<int> PageNumbers
        {
            get
            {
                int startPage = Math.Max(1, PageNumber - 5);        //Centered
                int endPage = Math.Min(TotalPages, startPage + 9);

                if (endPage == TotalPages && endPage - startPage < 9)
                {
                    startPage = Math.Max(1, endPage - 9);
                }

                return Enumerable.Range(startPage, endPage - startPage + 1);
            }
        }

        private void ChangePage(int page)
        {
            if (page < 1 || page > TotalPages || page == PageNumber)
            {
                return;
            }

            PageNumber = page;
            OnPageChanged.InvokeAsync(PageNumber);
        }
    }
}