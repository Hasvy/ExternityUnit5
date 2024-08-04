using Microsoft.AspNetCore.Components;
using System.Collections.Generic;

namespace ExtUnit5.Components
{
    public partial class Pagination<TItem> : ComponentBase
    {
        [Parameter] public List<TItem> Items { get; set; } = null!;
        [Parameter] public int ItemsOnPage { get; set; }
        [Parameter] public int CurrentPage { get; set; }
        [Parameter] public EventCallback<int> OnPageChanged { get; set; }

        private int TotalItems => Items.Count;
        private int TotalPages => TotalItems / ItemsOnPage;
        private bool _isFirstPage => CurrentPage == 1;
        private bool _isLastPage => CurrentPage == TotalPages;

        private IEnumerable<int> PageNumbers => Enumerable.Range(1, TotalPages);

        private void ChangePage(int page)
        {
            if (page < 1 || page > TotalPages || page == CurrentPage)
            {
                return;
            }

            CurrentPage = page;
            OnPageChanged.InvokeAsync(CurrentPage);
        }
    }
}