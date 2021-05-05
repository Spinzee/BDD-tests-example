namespace Products.WebModel.ViewModels.TariffChange
{
    using System.Collections.Generic;
    using System.Linq;
    using Products.Model.TariffChange.Enums;

    public class ProgressBarViewModel
    {
        public IList<ProgressBarSection> Sections { get; set; }

        public double SectionCount => Sections.Count;

        public int CurrentStep => Sections.IndexOf(CurrentSection) + 1;

        public ProgressBarSection CurrentSection
        {
            get { return Sections.Last(x => x.Status == ProgressBarStatus.Active); }
        }
    }
}
