declare module Digital.Web.Energy {
    class Basket {
        trashList: JQuery;
        targetBasketUrl: string;
        sourceUrl: string;
        scope: any;
        removeUpgradeCallback: string;
        constructor(config: BasketConfig);
        bindTrashListItems(doParentCallBack: boolean): void;
        removeUpgrade(eventArgs: JQueryEventObject): void;
        private executeFunctionByName;
    }
    class BasketConfig {
        targetBasketUrl: string;
        sourceUrl: string;
        scope: any;
        callBackFunctionName: string;
        bindOnConstruction: boolean;
        constructor(targetBasketUrl: string, sourceUrl: string, bindOnConstruction?: boolean);
    }
}