declare module Digital.Web.Cookies {
    class SetCookies {
        hideCookie: JQuery;
        constructor();
        static init(): void;
        handleHideCookie(eventArgs: JQueryEventObject): void;
        setCookie(key: any, value: any): void;
        getCookie(key: any): string;
    }
}
