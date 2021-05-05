declare module Digital.Web.Energy {
    class DirectDebitAmountUpdate {
        private updateDirectDebitAmountUrl;
        private sourceUrl;
        private basketConfig;
        private basket;
        constructor(updateDirectDebitAmountUrl: string, sourceUrl: string, basketConfig: Energy.BasketConfig);
        private updateDirectDebitAmountCallback;
    }
}
