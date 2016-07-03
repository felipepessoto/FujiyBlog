using FujiyBlog.Core.Caching;
using FujiyBlog.Core.DomainObjects;
using FujiyBlog.Core.EntityFramework;
using FujiyBlog.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.PlatformAbstractions;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FujiyBlog.Web.ViewComponents
{
    public partial class PagerViewComponent : ViewComponent
    {
        public Task<IViewComponentResult> InvokeAsync(int currentPage, int totalPages, string nextPageText, string previousPageText)
        {
            totalPages = Math.Max(totalPages, 1);
            int numbersToShow = Math.Min(13, totalPages);

            int initialPage = Math.Max(currentPage - (numbersToShow / 2), 1);

            int suppostEndPage = initialPage + numbersToShow - 1;
            if (suppostEndPage > totalPages)
            {
                initialPage = initialPage - (suppostEndPage - totalPages);
            }

            bool showTwoFirst = initialPage > 1;

            if (showTwoFirst)
            {
                initialPage += 2;
                numbersToShow -= 2;
            }

            bool showTwoLasts = initialPage + numbersToShow - 1 < totalPages;

            if (showTwoLasts)
            {
                numbersToShow -= 2;
            }

            ViewBag.CurrentPage = currentPage;
            ViewBag.TotalPages = totalPages;
            ViewBag.NextPageText = nextPageText;
            ViewBag.PreviousPageText = previousPageText;
            ViewBag.InitialPage = initialPage;
            ViewBag.NumbersToShow = numbersToShow;
            ViewBag.ShowTwoFirst = showTwoFirst;
            ViewBag.ShowTwoLasts = showTwoLasts;

            return Task.FromResult<IViewComponentResult>(View("Links"));
        }
    }
}
