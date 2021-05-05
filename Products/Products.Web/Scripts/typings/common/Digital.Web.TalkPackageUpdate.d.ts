declare module Digital.Web.Common {
    class TalkPackageUpdate {
        private sourceUrl;
        private selectTalkPackageUrl;
        private basketConfig;
        private triggerElement;
        private targetElement;
        private basket;
        private triggerElementName;
        private targetElementId;
        private targetUrl;
        constructor(sourceUrl: string, selectTalkPackageUrl: string, basketConfig: Web.Energy.BasketConfig);
        bindTalkPackageChange(): void;
        removeUpgradeCallback(scope: any): void;
        private handlePhonePackageChange;
        private updateTalkPackage;
    }
}