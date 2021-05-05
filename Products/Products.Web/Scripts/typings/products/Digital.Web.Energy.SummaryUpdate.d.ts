declare module Digital.Web.Energy {
    class SummaryUpdate {
        private targetUrl;
        private sourceUrl;
        trashListItems: JQuery;
        constructor(targetUrl: string, sourceUrl: string);
        bindRemoveUpgradeList(): void;
        removeUpgrade(eventArgs: JQueryEventObject): void;
    }
}
