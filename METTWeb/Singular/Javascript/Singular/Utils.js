
//#region  Dates 

/*
* Date Format 1.2.3
* (c) 2007-2009 Steven Levithan <stevenlevithan.com>
* MIT license
* Includes enhancements by Scott Trenda <scott.trenda.net> and Kris Kowal <cixar.com/~kris.kowal/>*/
var dateFormat = function () {
  var e = /d{1,4}|M{1,4}|yy(?:yy)?|([HhmsTt])\1?|[LloSZ]|"[^"]*"|'[^']*'/g, t = /\b(?:[PMCEA][SDP]T|(?:Pacific|Mountain|Central|Eastern|Atlantic) (?:Standard|Daylight|Prevailing) Time|(?:GMT|UTC)(?:[-+]\d{4})?)\b/g, n = /[^-+\dA-Z]/g, r = function (e, t) { e = String(e); t = t || 2; while (e.length < t) e = "0" + e; return e }; return function (i, s, o) {
    var u = dateFormat; if (i == null) { return "" } if (arguments.length == 1 && Object.prototype.toString.call(i) == "[object String]" && !/\d/.test(i)) { s = i; i = undefined } i = i ? new Date(i) : new Date; if (isNaN(i)) throw SyntaxError("invalid date"); s = String(u.masks[s] || s || u.masks["default"]); if (s.slice(0, 4) == "UTC:") { s = s.slice(4); o = true }
    var a = o ? "getUTC" : "get", f = i[a + "Date"](), l = i[a + "Day"](), c = i[a + "Month"](), h = i[a + "FullYear"](), p = i[a + "Hours"](), d = i[a + "Minutes"](), v = i[a + "Seconds"](), m = i[a + "Milliseconds"](), g = o ? 0 : i.getTimezoneOffset(), y = {
      d: f, dd: r(f), ddd: u.i18n.dayNames[l], dddd: u.i18n.dayNames[l + 7], M: c + 1, MM: r(c + 1), MMM: u.i18n.monthNames[c], MMMM: u.i18n.monthNames[c + 12], yy: String(h).slice(2), yyyy: h, h: p % 12 || 12, hh: r(p % 12 || 12), H: p, HH: r(p), m: d, mm: r(d), s: v, ss: r(v), l: r(m, 3), L: r(m > 99 ? Math.round(m / 10) : m), t: p < 12 ? "a" : "p", tt: p < 12 ? "am" : "pm", T: p < 12 ? "A" : "P", TT: p < 12 ? "AM" : "PM", Z: o ? "UTC" : (String(i).match(t) || [""]).pop().replace(n, ""), o: (g > 0 ? "-" : "+") +
      r(Math.floor(Math.abs(g) / 60) * 100 + Math.abs(g) % 60, 4), S: ["th", "st", "nd", "rd"][f % 10 > 3 ? 0 : (f % 100 - f % 10 != 10) * f % 10]
    }; return s.replace(e, function (e) { return e in y ? y[e] : e.slice(1, e.length - 1) })
  }
}();
dateFormat.masks = { "default": "ddd mmm dd yyyy HH:MM:ss", shortDate: "m/d/yy", mediumDate: "mmm d, yyyy", longDate: "mmmm d, yyyy", fullDate: "dddd, mmmm d, yyyy", shortTime: "h:MM TT", mediumTime: "h:MM:ss TT", longTime: "h:MM:ss TT Z", isoDate: "yyyy-mm-dd", isoTime: "HH:MM:ss", isoDateTime: "yyyy-mm-dd'T'HH:MM:ss", isoUtcDateTime: "UTC:yyyy-mm-dd'T'HH:MM:ss'Z'" };
dateFormat.i18n = { dayNames: ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"], monthNames: ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec", "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"] }

Date.prototype.format = function (mask, utc) {
  if (isNaN(this)) {
    return '';
  } else {
    return dateFormat(this, mask, utc);
  }
};
Date.prototype.AddDays = function (Days) {
  var newDate = new Date(this);
  newDate.setDate(newDate.getDate() + Days);
  return newDate;
};
Date.prototype.Date = function () {
  this.setHours(0, 0, 0, 0);
  return this;
}
Date.ParseRequiresAllParts = false; //If true, null will be returned if the date is missing a component, e.g. the year. Otherwise the current year / month is used.
Date.ParseReturnNullIfInvalid = false; //If true, null will be returned if the date cannot be parsed. Otherwise returns current date.

String.prototype.ParseDate = function (Format, Default) {
  if (this == '') return null;
  Format = Format ? Format : 'dd MMM yyyy';
  Default = Default != null ? Default : new Date();

  //Find the position of d m and y in the format string.
  var d = Format.indexOf('d'), m = Format.indexOf('M'), y = Format.indexOf('y'), Extra = 100;

  //Add missing d/m/y on the end
  d = d == -1 ? Extra++ : d;
  m = m == -1 ? Extra++ : m;
  y = y == -1 ? Extra++ : y;

  //Replace common seperators in the date string with /
  var str = this.replace(/-/g, '/').replace(/ /g, '/');

  //If the user doesnt put in seperators, e.g. 0103, then add them to make the string value 01/03
  if ((d == 0 || m == 0) && str.indexOf('/') == -1 && str.length >= 4) {
    str = str.substring(0, 2) + '/' + str.substring(2, 4) + (str.length == 8 ? '/' + str.substring(4, 8) : '');
  }

  var parts = str.split('/');
  if (Date.ParseRequiresAllParts && parts.length < 3) return null;

  //if format is dd mmm yyyy, and the user omits the dd, then add 1st day.
  if (d == 0 && parts[0].length > 2) parts.splice(0, 0, 1);

  var Year = y > m ? (y > d ? parts[2] : parts[1]) : (y > d ? parts[1] : parts[0]);
  var Month = m > d ? (m > y ? parts[2] : parts[1]) : (m > y ? parts[1] : parts[0]);
  var Day = d > m ? (d > y ? parts[2] : parts[1]) : (d > y ? parts[1] : parts[0]);

  if (Year && Year.length > 4 && Date.ParseReturnNullIfInvalid) return null;

  Year = Year == undefined ? new Date().getFullYear() : Year;
  Month = Month == undefined ? new Date().getMonth() + 1 : Month;
  Day = Day == undefined ? 1 : Day;
  if (parseInt(Month) == Month && Month <= 12) {
    Month = dateFormat.i18n.monthNames[Month - 1];
  }
  var ret = new Date(Day + ' ' + Month + ' ' + Year);
  if (ret == "Invalid Date" && Year.length == 2) {
    ret = new Date(Day + ' ' + Month + ' 20' + Year);
  }
  return isNaN(ret) ? (Date.ParseReturnNullIfInvalid ? null : Default) : ret;

}
/*
//Tests
var x = 'Mar 2013'.ParseDate('MMM yyyy');
x = '0103'.ParseDate('dd/MM/yyyy');
x = '5/6'.ParseDate('dd MMM yyyy');
x = '3/9'.ParseDate('dd/MM/yyyy');
x = '2 12'.ParseDate('dd/MM/yyyy');
x = '2 12'.ParseDate('MM/dd/yyyy');
x = '7'.ParseDate('dd/MM/yyyy');
x = '9 Jan'.ParseDate('dd/MM/yyyy');
x = '5 October'.ParseDate('dd MMM yyyy');
*/

var GetDTime = function (obj) {
  if (obj == null) {
    return 0;
  } else if (obj instanceof Date) {
    return obj.getTime();
  } else {
    return new Date(obj).getTime();
  }
}

//#endregion

//#region  Numbers 

// ForceNumericInput(Element, AllowDecimal, AllowNegative
function ForceNumericInput(e, t, n) {
  if (arguments.length == 1) {
    var r = e.value; var i = r.lastIndexOf("-"); if (i == -1) return; if (i != 0) e.value = r.substring(0, i) + r.substring(i + 1); return
  } if (this.event == undefined) return;
  var s = event.keyCode;
  if (event.ctrlKey) return true; //ctrl is allowed
  switch (s) { case 9: case 8: case 37: case 39: case 46: event.returnValue = true; return }
  if (s == 189 || s == 109) {
    if (n == false) { event.returnValue = false; return }
    setTimeout(function () { ForceNumericInput(document.getElementById(e.id)) }, 250); return
  } if (t && (s == 190 || s == 110)) { if (e.value.indexOf(".") >= 0) { event.returnValue = false; return } event.returnValue = true; return } if (s >= 48 && s <= 57 || s >= 96 && s <= 105) {
    event.returnValue = true; return
  } event.returnValue = false
}

//Format Numbers (cents, thousands, negative)
Number.prototype.formatMoney = function (c, t, neg) {

  //check if the first arg is in the format #,##00
  if (arguments.length == 1 && isNaN(parseInt(c))) {
    return this.formatDotNet(c);
  }
  t = t == undefined ? ' ' : t;
  t = t == ' ' ? '\xA0' : t;

  var d = '.';
  var n = this, c = isNaN(c = Math.abs(c)) ? 2 : c, d = d == undefined ? "," : d, s = n < 0 ? "-" : "", i = parseInt(n = Math.abs(+n || 0).toFixed(c)) + "", j = (j = i.length) > 3 ? j % 3 : 0;
  var result = s + (j ? i.substr(0, j) + t : "") + i.substr(j).replace(/(\d{3})(?=\d)/g, "$1" + t) + (c ? d + Math.abs(n - i).toFixed(c).slice(2) : "");

  return result[0] == "-" ? ((neg == undefined ? "(" : neg) + result.substr(1) + (neg == undefined ? ")" : "")) : result;
};
Number.ThousandsSeperator = '\xA0';//non breaking space.

Number.prototype.formatDotNet = function (m) {

  var v = this;
  if (!m || isNaN(+v)) {
    return v; //return as it is.
  }
  var isNegative = +v < 0;
  v = Math.abs(v);

  m = m.replace(/ /g, '\xA0'); //replace spaces with non-breaking spaces.

  //Check if the format string is a 2 part string
  var SepIdx = m.indexOf(';');
  if (SepIdx > 0) {
    if (isNegative) {
      //use the negative part
      m = m.substr(SepIdx + 1);
    } else {
      //use the positive part
      m = m.substr(0, SepIdx);
    }
  }
  var BeforeText = '', AfterText = '';
  var fmtStart = m.indexOf('#');
  var fmtEnd = Math.max(m.lastIndexOf('#'), m.lastIndexOf('0'));
  if (fmtStart > 0) {
    BeforeText = m.substr(0, fmtStart);
  }
  if (fmtEnd < m.length - 1) {
    AfterText = m.substr(fmtEnd + 1);
  }
  m = m.substr(fmtStart, fmtEnd - fmtStart + 1);

  //search for separator for grp & decimal, anything not digit, not +/- sign, not #.
  var result = m.match(/[^\d\-\+#]/g);
  var Decimal = (result && result[result.length - 1]) || '.'; //treat the right most symbol as decimal 
  var Group = (result && result[1] && result[0]) || ',';  //treat the left most symbol as group separator
  var GroupAppend = Group == ' ' ? '\xA0' : Group;
  if (Decimal == Group) Decimal = '.';

  //split the decimal for the format string if any.
  var m = m.split(Decimal);

  //Check if this is a percent
  v = AfterText.indexOf('%') < 0 ? v : v * 100;

  //Fix the decimal first, toFixed will auto fill trailing zero.
  v = v.toFixed(m[1] && m[1].length);
  v = +(v) + ''; //convert number to string to trim off *all* trailing decimal zero(es)

  //fill back any trailing zero according to format
  var pos_trail_zero = m[1] && m[1].lastIndexOf('0'); //look for last zero in format
  var part = v.split('.');

  //integer will get !part[1]
  if (!part[1] || part[1] && part[1].length <= pos_trail_zero) {
    v = (+v).toFixed(pos_trail_zero + 1);
  }

  var szSep = m[0].split(Group); //look for separator
  m[0] = szSep.join(''); //join back without separator for counting the pos of any leading 0.

  var pos_lead_zero = m[0] && m[0].indexOf('0');
  if (pos_lead_zero > -1) {
    while (part[0].length < (m[0].length - pos_lead_zero)) {
      part[0] = '0' + part[0];
    }
  }
  else if (+part[0] == 0) {
    part[0] = '';
  }

  v = v.split('.');
  v[0] = part[0];


  //process the first group separator from decimal (.) only, the rest ignore.
  //get the length of the last slice of split result.
  var pos_separator = (szSep[1] && szSep[szSep.length - 1].length);
  if (pos_separator) {
    var integer = v[0];
    var str = '';
    var offset = integer.length % pos_separator;
    for (var i = 0, l = integer.length; i < l; i++) {
      str += integer.charAt(i); //ie6 only support charAt for sz.
      //-pos_separator so that won't trail separator on full length
      if (!((i - offset + 1) % pos_separator) && i < l - pos_separator) {
        str += GroupAppend;
      }
    }
    v[0] = str;
  }

  v[1] = (m[1] && v[1]) ? Decimal + v[1] : "";
  v[0] = v[0].replace(/,/g, Number.ThousandsSeperator);
  return BeforeText + v[0] + v[1] + AfterText; //put back any negation and combine integer and fraction.

};


//The awesome vb CInt function, named incorrectly, because it converts to a number, not an integer.
function CInt(val) {
  return val ? parseFloat(val) : val;
}

function CompareSafe(val1, val2) {
  return val1 == val2;
}


Number.prototype.addOrdinal = function () {

  if (this <= 0) return this;

  switch (this % 100) {
    case 11: case 12: case 13: return this + "th";
  }

  switch (this % 10) {
    case 1: return this + "st";
    case 2: return this + "nd";
    case 3: return this + "rd";
  }

  return this + "th";
}

//Allow comparison to a certain precision.
Number.IgnorePrecision = 0.0000001;

function CompareGT  (x, y) {
  return x - y > Number.IgnorePrecision;
}
function CompareGTE (x, y) {
  return x - y > -Number.IgnorePrecision;
}
function CompareLT (x, y) {
  return CompareGT(y, x);
}
function CompareLTE (x, y) {
  return CompareGTE(y, x);
}
function CompareE (x, y) {
  return Math.abs(x - y) < Number.IgnorePrecision;
}
function CompareNE (x, y) {
  return !CompareE(x, y);
}

//Public Shared Function AddOrdinal(ToNumber As Integer) As String

//  If ToNumber <= 0 Then
//Return ToNumber.ToString()
//End If

//Select Case (ToNumber Mod 100)
//  Case 11, 12, 13
//    Return ToNumber.ToString() & "th"
//End Select

//Select Case (ToNumber Mod 10)
//  Case 1
//    Return ToNumber.ToString() & "st"
//Case 2
//Return ToNumber.ToString() & "nd"
//Case 3
//Return ToNumber.ToString() & "rd"
//Case Else
//Return ToNumber.ToString() & "th"
//End Select

//End Function

//#endregion

//#region  Strings 

// String.format. Allows: "Hello {0}".format("World")
String.prototype.format = function () {
  var args = arguments.length == 1 && arguments[0] instanceof Array ? arguments[0] : arguments;
  return this.replace(/{(\d+)}/g, function (match, number) {
    return typeof args[number] != 'undefined'
        ? args[number]
        : match
    ;
  });
};
if (!String.prototype.trim) {
  String.prototype.trim = function () { return this.replace(/^\s+|\s+$/g, ''); };
}

String.prototype.startsWith = function (search) {
  return this.lastIndexOf(search, 0) === 0;
}
String.prototype.StartsWith = String.prototype.startsWith;

//#endregion

//#region  Misc 

/*
Masked Input plugin for jQuery
Copyright (c) 2007-2013 Josh Bush (digitalbush.com)
Licensed under the MIT license (http://digitalbush.com/projects/masked-input-plugin/#license)
Version: 1.3.1
*/
(function (a) {
  var b, c = navigator.userAgent, d = /iphone/i.test(c), e = /chrome/i.test(c), f = /android/i.test(c);
  a.mask = {
    definitions: { 9: "[0-9]", a: "[A-Za-z]", "*": "[A-Za-z0-9]" },
    autoclear: !0,
    dataName: "rawMaskFn",
    placeholder: "_"
  }, a.fn.extend({
    caret: function (a, b) {
      var c;
      if (0 !== this.length && !this.is(":hidden")) return "number" == typeof a ? (b = "number" == typeof b ? b : a, this.each(function () { this.setSelectionRange ? this.setSelectionRange(a, b) : this.createTextRange && (c = this.createTextRange(), c.collapse(!0), c.moveEnd("character", b), c.moveStart("character", a), c.select()) })) : (this[0].setSelectionRange ? (a = this[0].selectionStart, b = this[0].selectionEnd) : document.selection && document.selection.createRange && (c = document.selection.createRange(), a = 0 - c.duplicate().moveStart("character", -1e5), b = a + c.text.length), { begin: a, end: b })
    },
    unmask: function () { return this.trigger("unmask") },
    mask: function (c, g) {
      var h, i, j, k, l, m, n, o;
      if (!c && this.length > 0) {
        h = a(this[0]);
        var p = h.data(a.mask.dataName);
        return p ? p() : void 0
      }
      return g = a.extend({ autoclear: a.mask.autoclear, placeholder: a.mask.placeholder, completed: null }, g), i = a.mask.definitions, j = [], k = n = c.length, l = null, a.each(c.split(""),
				function (a, b) {
				  "?" == b ? (n--, k = a) : i[b] ? (j.push(new RegExp(i[b])), null === l && (l = j.length - 1), k > a && (m = j.length - 1)) : j.push(null)
				}), this.trigger("unmask").each(function () {
				  function h() {
				    if (g.completed) {
				      for (var a = l; m >= a; a++)
				        if (j[a] && C[a] === p(a))
				          return;
				      g.completed.call(B)
				    }
				  } function p(a) {
				    return g.placeholder.charAt(a < g.placeholder.length ? a : 0)
				  } function q(a) {
				    for (; ++a < n && !j[a];); return a
				  } function r(a) {
				    for (; --a >= 0 && !j[a];); return a
				  } function s(a, b) {
				    var c, d;
				    if (!(0 > a)) {
				      for (c = a, d = q(b) ; n > c; c++)
				        if (j[c]) {
				          if (!(n > d && j[c].test(C[d])))
				            break;
				          C[c] = C[d], C[d] = p(d), d = q(d)
				        }
				      z(), B.caret(Math.max(l, a))
				    }
				  } function t(a) {
				    var b, c, d, e;
				    for (b = a, c = p(a) ; n > b; b++)
				      if (j[b]) {
				        if (d = q(b), e = C[b], C[b] = c, !(n > d && j[d].test(e)))
				          break;
				        c = e
				      }
				  } function u() {
				    var a = B.val(), b = B.caret();
				    if (o && o.length && o.length > a.length) {
				      for (A(!0) ; b.begin > 0 && !j[b.begin - 1];) b.begin--; if (0 === b.begin) for (; b.begin < l && !j[b.begin];) b.begin++; B.caret(b.begin, b.begin)
				    } else {
				      for (A(!0) ; b.begin < n && !j[b.begin];) b.begin++; B.caret(b.begin, b.begin)
				    } h()
				  } function v() {
				    A(), B.val() != E && B.change()
				  } function w(a) {
				    if (!B.prop("readonly")) {
				      var b, c, e, f = a.which || a.keyCode;
				      o = B.val(), 8 === f || 46 === f || d && 127 === f ? (b = B.caret(), c = b.begin, e = b.end, e - c === 0 && (c = 46 !== f ? r(c) : e = q(c - 1), e = 46 === f ? q(e) : e), y(c, e), s(c, e - 1), a.preventDefault()) : 13 === f ? v.call(this, a) : 27 === f && (B.val(E), B.caret(0, A()), a.preventDefault())
				    }
				  } function x(b) {
				    if (!B.prop("readonly")) {
				      var c, d, e, g = b.which || b.keyCode, i = B.caret(); if (!(b.ctrlKey || b.altKey || b.metaKey || 32 > g) && g && 13 !== g) { if (i.end - i.begin !== 0 && (y(i.begin, i.end), s(i.begin, i.end - 1)), c = q(i.begin - 1), n > c && (d = String.fromCharCode(g), j[c].test(d))) { if (t(c), C[c] = d, z(), e = q(c), f) { var k = function () { a.proxy(a.fn.caret, B, e)() }; setTimeout(k, 0) } else B.caret(e); i.begin <= m && h() } b.preventDefault() }
				    }
				  } function y(a, b) {
				    var c;
				    for (c = a; b > c && n > c; c++) j[c] && (C[c] = p(c))
				  } function z() {
				    B.val(C.join(""))
				  } function A(a) {
				    var b, c, d, e = B.val(), f = -1;
				    for (b = 0, d = 0; n > b; b++)
				      if (j[b]) {
				        for (C[b] = p(b) ; d++ < e.length;)
				          if (c = e.charAt(d - 1), j[b].test(c)) {
				            C[b] = c, f = b;
				            break
				          }
				        if (d > e.length) {
				          y(b + 1, n);
				          break
				        }
				      } else
				        C[b] === e.charAt(d) && d++, k > b && (f = b);
				    return a ? z() : k > f + 1 ? g.autoclear || C.join("") === D ? (B.val() && B.val(""), y(0, n)) : z() : (z(), B.val(B.val().substring(0, f + 1))), k ? b : l
				  } var B = a(this), C = a.map(c.split(""), function (a, b) { return "?" != a ? i[a] ? p(b) : a : void 0 }), D = C.join(""), E = B.val(); B.data(a.mask.dataName, function () { return a.map(C, function (a, b) { return j[b] && a != p(b) ? a : null }).join("") }), B.one("unmask", function () { B.off(".mask").removeData(a.mask.dataName) }).on("focus.mask", function () { if (!B.prop("readonly")) { clearTimeout(b); var a; E = B.val(), a = A(), b = setTimeout(function () { B.get(0) === document.activeElement && (z(), a == c.replace("?", "").length ? B.caret(0, a) : B.caret(a)) }, 10) } }).on("blur.mask", v).on("keydown.mask", w).on("keypress.mask", x).on("input.mask paste.mask", function () { B.prop("readonly") || setTimeout(function () { var a = A(!0); B.caret(a), h() }, 0) }), e && f && B.off("input.mask").on("input.mask", u), A()
				})
    }
  })
})(jQuery);

function GetIEVersion()
  // Returns the version of Internet Explorer or a 999
  // (indicating the use of another browser).
{
  var rv = 999;
  if (navigator.appName == 'Microsoft Internet Explorer') {
    var ua = navigator.userAgent;
    var re = new RegExp("MSIE ([0-9]{1,}[\.0-9]{0,})");
    if (re.exec(ua) != null)
      rv = parseFloat(RegExp.$1);
  }
  return rv;
}

var elementInDocument = function (element) {
  return elementIsParent(document, element);
}
//Checks if a child element is part of a parents child controls.
var elementIsParent = function (ParentElement, ChildElement) {
  do {
    if (ChildElement == ParentElement) {
      return true;
    }
    ChildElement = ChildElement.parentNode
  } while (ChildElement)
  return false;
}

function GetCookie(name) {
  var cookie = document.cookie.match('(^|;)\\s*' + name + '\\s*=\\s*([^;]+)');
  return cookie ? cookie.pop() : '';
}
function getQueryVariable(variable) {
  var query = window.location.search.substring(1);
  var vars = query.split("&");
  for (var i = 0; i < vars.length; i++) {
    var pair = vars[i].split("=");
    if (pair[0].toLowerCase() == variable.toLowerCase()) { return decodeURIComponent(pair[1]); }
  }
  return '';
}

function XOR(a, b) {
  return (a || b) && !(a && b);
}

function IsMobile() {
  return /Mobi|Android/i.test(navigator.userAgent);
}

//#endregion

"use strict";

//#region  Arrays 

//Fix ie < 9
if (!Array.prototype.indexOf) {
  Array.prototype.indexOf = function (obj, start) {
    for (var i = (start || 0), j = this.length; i < j; i++) {
      if (this[i] === obj) { return i; }
    }
    return -1;
  };
}
//Returns only the items that match the filter
Array.prototype.Filter = function (PropertyName, FilterValue) {
  return Singular.FilterList(this, PropertyName, FilterValue);
};
//Finds an item in the list
Array.prototype.Find = function (Property, Value) {
  for (var i = 0; i < this.length; i++) {
    if (ko.utils.unwrapObservable(this[i][Property]) == Value)
      return this[i];
  };
};
//Finds an item ignoring case.
Array.prototype.FindText = function (Property, Value) {
  Value = Value.toLowerCase();
  for (var i = 0; i < this.length; i++) {
    if (ko.utils.unwrapObservable(this[i][Property]).toLowerCase() == Value)
      return this[i];
  };
};

Array.prototype.Sum = function (PropertyName) {
  var Tot = 0;
  for (var i = 0; i < this.length; i++) Tot += ko.utils.unwrapObservable(this[i][PropertyName]);
  return Tot;
}
Array.prototype.Avg = function (PropertyName) {
  if (this.length == 0) return 0;
  return this.Sum(PropertyName) / this.length;
}
Array.prototype.AvgNZ = function (PropertyName) {
  var Tot = 0;
  var Items = 0;
  for (var i = 0; i < this.length; i++) {
    var Val = ko.utils.unwrapObservable(this[i][PropertyName]);
    if (Val) {
      Tot += Val;
      Items += 1;
    }
  }
  return Items == 0 ? 0 : Tot / Items;
}
//Finds an item in the list (goes down the tree) and returns the value of the specified ReturnValueProperty.
Array.prototype.FindValue = function (FindProperty, FindValue, ReturnValueProperty) {

  //Check if the property in not this list.
  if (this.length > 0 && this[0][FindProperty] == undefined) {
    //then check child lists.
    for (var name in this[0]) {
      if (this[0][name] instanceof Array && this[0][name].length > 0) {

        for (var i = 0; i < this.length; i++) {
          var FoundObj = this[i][name].FindValue(FindProperty, FindValue, ReturnValueProperty);
          if (!(FoundObj == undefined)) {
            if (FoundObj.Value == undefined) {
              return { Object: FoundObj.Object, Value: this[i][ReturnValueProperty] };
            } else {
              return FoundObj;
            };

          }
        };
        return;
      };
    };
  } else {
    var FoundObj = this.Find(FindProperty, FindValue);
    if (FoundObj == undefined) {
      return;
    } else {
      return { Object: FoundObj, Value: FoundObj[ReturnValueProperty] };
    }
  };

};

//Removes an item.
Array.prototype.RemoveItem = function (item) {
  var idx = this.indexOf(item);
  if (idx >= 0) this.splice(idx, 1);
};
//Empties the List.
Array.prototype.Clear = function () {
  this.length = 0;
};

//Loop through the list, and for each item in the list, run the function "codeToRun" on that item
Array.prototype.Iterate = function (callback) {
  for (var i = 0; i < this.length; i++) {
    callback(this[i], i);
  }
};
//Reverse iterate
Array.prototype.IterateR = function (callback) {
  for (var i = this.length - 1; i >= 0; i--) {
    callback(this[i], i);
  }
};

//Gets the distinct values of the specified property
Array.prototype.Distinct = function (Property, ExcludeNull) {
  var result = new Array(),
           i = 0,
           propVal = null;
  while (i < this.length) {
    propVal = this[i][Property]();
    if (result.indexOf(propVal) < 0) {
      if (ExcludeNull && !propVal) {
        //don't add the null value
      } else {
        result.push(propVal);
      }
    };
    i++;
  };
  //if (ExcludeNull) {
  //  var indexOfNull = result.indexOf(null);
  //  result.splice(indexOfNull, 1);
  //};
  return result;
};

Array.prototype.Last = function () {
  return this[this.length - 1];
};

Array.prototype.Where = function (callback) {
  var fList = [];
  this.Iterate(function (item) {
    if (callback(item)) fList.push(item);
  });
  return fList;
}

Array.prototype.FirstOrDefault = function (critFunction) {
  if (critFunction) {
    for (var i = 0; i < this.length; i++) {
      if (critFunction(this[i]) == true) {
        return this[i];
      }
    }
  } else if (this.length) {
    return this[0];
  }
  return null;
}

Array.prototype.Max = function (callback) {
  var max = null;
  if (this.length > 0) {
    if (typeof callback == 'function') {
      max = ko.utils.unwrapObservable(callback(this[0]));
      for (var i = 1; i < this.length; i++) {
        var newMax = ko.utils.unwrapObservable(callback(this[i]))
        if (max < newMax) {
          max = newMax;
        }
      }
    }
    if (typeof callback == 'string') {
      max = ko.utils.unwrapObservable(this[0][callback]);
      for (var i = 1; i < this.length; i++) {
        var newMax = ko.utils.unwrapObservable(this[i][callback])
        if (max < newMax) {
          max = newMax;
        }
      }
    }
  }
  return max;
}


//#endregion













