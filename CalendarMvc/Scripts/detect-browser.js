/// <reference path="../jquery-2.1.3.js" />
/// <reference path="../jquery-2.1.3.intellisense.js" />
/**
 * Written by 
 * Alim Ul Karim
 * devorg.bd@gmail.com
 */
$.detectBrowser = {
    browserName: null,
    browserVersion: 0,
    isBrowsersDetected: false,
    isOpera: null,
    isInternetExplorer: null,
    isSafari: null,
    isChrome: null,
    isFirefox: null,
    getBrowserName: function () {
        /// <summary>
        /// return $.detectBrowser.browserName
        /// </summary>
        return $.detectBrowser.browserName;
    },
    getBrowserVersion: function () {
        /// <summary>
        /// return $.detectBrowser.browserVersion
        /// </summary>
        return $.detectBrowser.browserVersion;
    },
    isMinimumBrowser: function (chormeVersion, firefoxVersion, IEVersion, safariVersion, operaVersion) {
        if ($.detectBrowser.isChrome && $.detectBrowser.browserVersion >= chormeVersion) {
            return true;
        } else if ($.detectBrowser.isFirefox && $.detectBrowser.browserVersion >= firefoxVersion) {
            return true;
        } else if ($.detectBrowser.isInternetExplorer && $.detectBrowser.browserVersion >= IEVersion) {
            return true;
        } else if ($.detectBrowser.isSafari && $.detectBrowser.browserVersion >= safariVersion) {
            return true;
        } else if ($.detectBrowser.isOpera && $.detectBrowser.browserVersion >= operaVersion) {
            return true;
        }
        return false;
    },
    printBrowser: function () {
        var name = $.detectBrowser.browserName + " " + $.detectBrowser.browserVersion;
        console.log(name);
        return name;
    },

    setBrowserDetectedFlags: function () {
        $.detectBrowser.isBrowsersDetected = true;
        $.detectBrowser.isChrome = false;
        $.detectBrowser.isOpera = false;
        $.detectBrowser.isSafari = false;
        $.detectBrowser.isFirefox = false;
        $.detectBrowser.isInternetExplorer = false;

        if ($.detectBrowser.browserName === "Chrome") {
            $.detectBrowser.isChrome = true;
        } else if ($.detectBrowser.browserName === "Firefox") {
            $.detectBrowser.isFirefox = true;
        } else if ($.detectBrowser.browserName === "IE" || $.detectBrowser.browserName === "MSIE") {
            $.detectBrowser.isInternetExplorer = true;
        } else if ($.detectBrowser.browserName === "Safari") {
            $.detectBrowser.isSafari = true;
        } else if ($.detectBrowser.browserName === "Opera") {
            $.detectBrowser.isOpera = true;
        }

        //console.log("Chrome : " + $.detectBrowser.isChrome);
        //console.log("Firefox : " + $.detectBrowser.isFirefox);
        //console.log("IE : " + $.detectBrowser.isInternetExplorer);
        //console.log("Safari : " + $.detectBrowser.isSafari);
        //console.log("Opera : " + $.detectBrowser.isOpera);

    },
    execute: function () {
        if ($.detectBrowser.isBrowsersDetected) {
            return $.detectBrowser.printBrowser();
        }
        var ua = navigator.userAgent, tem,
        M = ua.match(/(opera|chrome|safari|firefox|msie|trident(?=\/))\/?\s*(\d+)/i) || [];
        if (/trident/i.test(M[1])) {
            tem = /\brv[ :]+(\d+)/g.exec(ua) || [];
            //internet explorer
            $.detectBrowser.browserName = "IE";
            $.detectBrowser.browserVersion = tem[1] || '';
            //return 'IE ' + (tem[1] || '');

            // all browser flags : isOpera, isChrome..
            $.detectBrowser.setBrowserDetectedFlags();
            return $.detectBrowser.printBrowser();
        }
        if (M[1] === 'Chrome') {
            tem = ua.match(/\b(OPR|Edge)\/(\d+)/);
            if (tem != null) {
                //opera
                // slice example 
                /**
                 * var fruits = ['Banana', 'Orange', 'Lemon', 'Apple', 'Mango'];
                 * var citrus = fruits.slice(1, 3);
                 * citrus contains ['Orange','Lemon']
                 */
                var simpleBrowserName = tem.slice(1); // get browser name [0] = browser and [1] = version
                $.detectBrowser.browserName = simpleBrowserName[0].replace('OPR', 'Opera');
                $.detectBrowser.browserVersion = simpleBrowserName[1];
                // all browser flags : isOpera, isChrome..
                $.detectBrowser.setBrowserDetectedFlags();

                return $.detectBrowser.printBrowser();
            }
        }
        M = M[2] ? [M[1], M[2]] : [navigator.appName, navigator.appVersion, '-?'];
        if ((tem = ua.match(/version\/(\d+)/i)) != null) M.splice(1, 1, tem[1]);
        var simpleBrowserName2 = M.join(' ').split(" ");
        $.detectBrowser.browserName = simpleBrowserName2[0];
        $.detectBrowser.browserVersion = simpleBrowserName2[1];
        // all browser flags : isOpera, isChrome..
        $.detectBrowser.setBrowserDetectedFlags();
        return $.detectBrowser.printBrowser();

    }

}
$(function () {
    $.detectBrowser.execute();

});
//$.detectBrowser.printBrowser();