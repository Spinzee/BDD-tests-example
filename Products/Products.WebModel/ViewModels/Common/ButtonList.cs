using System.Collections.Generic;

namespace Products.WebModel.ViewModels.Common
{
    public class ButtonList
    {
        public IList<RadioButton> Items { get; set; }

        public string SelectedValue { get; set; }
    }
}
