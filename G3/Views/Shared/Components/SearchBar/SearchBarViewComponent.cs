using Microsoft.AspNetCore.Mvc;

namespace G3.Views.Shared.Components.SearchBar
{
	public class SearchBarViewComponent : ViewComponent
	{
        public SearchBarViewComponent()
        {
            
        }
        
        public IViewComponentResult Invoke(SPager searchPager)
        {
            return View("Default", searchPager);
        }
    }
}
