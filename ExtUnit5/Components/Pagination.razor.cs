using ExtUnit5.Entities;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;

namespace ExtUnit5.Components
{
    public partial class Pagination<TItem> : ComponentBase
    {
        [Parameter] public List<TItem> Items { get; set; } = null!;
        [Parameter] public int PageSize { get; set; }
        [Parameter] public int PageNumber { get; set; }
        [Parameter] public EventCallback<int> OnPageChanged { get; set; }

        private int TotalItems => Items.Count;
        private int TotalPages => TotalItems / PageSize;
        private bool _isFirstPage => PageNumber == 1;
        private bool _isLastPage => PageNumber == TotalPages;

        private IEnumerable<int> PageNumbers => Enumerable.Range(1, TotalPages);

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