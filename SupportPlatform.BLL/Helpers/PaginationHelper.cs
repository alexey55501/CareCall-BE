namespace SupportPlatform.BLL.Helpers
{
    public static class PaginationHelper
    {
        public static int PagesCount(int amountOnPage, int objsCount)
        {
            try
            {
                if (amountOnPage == 0) return 1;

                int pagesCount = 0;

                pagesCount = objsCount / amountOnPage;

                //return pagesCount + (objsCount % amountOnPage != 0 ? 1 : 0);
                return pagesCount + (objsCount % amountOnPage != 0 ? 1 : 0);
            }
            catch { }

            return 0;
        }
    }
}
