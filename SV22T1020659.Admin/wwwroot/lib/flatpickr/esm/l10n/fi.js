var fp = typeof window !== "undefined" && window.flatpickr !== undefined
    ? window.flatpickr
    : {
        l10ns: {},
    };
export var Finnish = {
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
export default fp.l10ns;
