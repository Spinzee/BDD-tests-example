namespace Products.WebModel.ViewModels.TariffChange
{
    using Products.Model.TariffChange.Enums;

    public class ProgressBarSection
    {
        public string Text { get; set; }

        public ProgressBarStatus Status { get; set; }

        public int StepsToComplete { get; set; }

        public int CompletedStep { get; set; }

        public int DisplayCompletedStep => CompletedStep + 1;
    }
}
