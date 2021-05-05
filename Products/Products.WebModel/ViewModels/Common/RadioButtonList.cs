using System.Collections.Generic;

namespace Products.WebModel.ViewModels.Common
{
    public class RadioButtonList
    {
        public IList<RadioButton> Items { get; set; }

        public string SelectedValue { get; set; }
    }
}
