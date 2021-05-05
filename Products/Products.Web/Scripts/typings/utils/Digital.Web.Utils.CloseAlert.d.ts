declare module Digital.Web.Utils {
    class CloseAlert {
        constructor();
        close: (e: any, selector: string) => void;
        remove: (e: JQuery) => void;
    }
}
