declare module Digital.Web.Tariffs {
    class Tab {
        chosenTariff: string;
        constructor(displayType: string, chosenTariff: string, initialHideTariffs: string, initialShowTariffs: string);
        private handleTabClick;
        private showHideTariffs;
        private showHideTabReactiveContent;
    }
}
