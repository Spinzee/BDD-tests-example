
namespace Products.WebModel.ViewModels.Common
{
    public class BackChevronViewModel
    {
        public string ActionName { get; set; }

        public string ControllerName { get; set; }

        public object RouteValues { get; set; }

        /// <summary>
        /// Text for the title attribute -- if you do not specify then a default value will be used.
        /// </summary>
        public string TitleAttributeText { get; set; } = "Back to previous page";

        /// <summary>
        /// Url will have a preference over ActionName and ControllerName to generate url in the partial view.
        /// </summary>
        public string Url { get; set; }
    }
}
