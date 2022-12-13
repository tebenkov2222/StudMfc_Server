namespace ISTU_MFC.ViewModels
{
    public class SelectListViewModel
    {
        public string Name { get; set; }
        public string View { get; set; }

        public SelectListViewModel(string name, string view)
        {
            Name = name;
            View = view;
        }
    }
}