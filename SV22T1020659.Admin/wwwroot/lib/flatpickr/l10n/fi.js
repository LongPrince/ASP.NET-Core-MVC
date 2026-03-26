(function (global, factory) {
  typeof exports === 'object' && typeof module !== 'undefined' ? factory(exports) :
  typeof define === 'function' && define.amd ? define(['exports'], factory) :
  (global = typeof globalThis !== 'undefined' ? globalThis : global || self, factory(global.fi = {}));
}(this, (function (exports) { 'use strict';

  var fp = typeof window !== "undefined" && window.flatpickr !== undefined
      ? window.flatpickr
      : {
          l10ns: {},
      };
  var Finnish = {
      firstDayOfWeek: 1,
      weekdays: {
          shorthand: ["su", "ma", "ti", "ke", "to", "pe", "la"],
          longhand: [
              "sunnuntai",
              "maanantai",
              "tiistai",
              "keskiviikko",
              "torstai",
              "perjantai",
              "lauantai",
          ],
      },
      months: {
          shorthand: [
              "tammi",
              "helmi",
              "maalis",
              "huhti",
              "touko",
              "kesä",
              "heinä",
              "elo",
              "syys",
              "loka",
              "marras",
              "joulu",
          ],
          longhand: [
              "tammikuu",
              "helmikuu",
              "maaliskuu",
              "huhtikuu",
              "toukokuu",
              "kesäkuu",
              "heinäkuu",
              "elokuu",
              "syyskuu",
              "lokakuu",
              "marraskuu",
              "joulukuu",
          ],
      },
      ordinal: function () {
          return ".";
      },
      time_24hr: true,
  };
  fp.l10ns.fi = Finnish;
  var fi = fp.l10ns;

  exports.Finnish = Finnish;
  exports.default = fi;

  Object.defineProperty(exports, '__esModule', { value: true });

})));
