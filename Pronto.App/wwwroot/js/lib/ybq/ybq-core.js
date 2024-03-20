///////////////////////////////////////////////////////////////////
//
// Youbiquitous Web Assets
// Copyright (c) Youbiquitous 2022
//
// Author: Youbiquitous Team
// v2.0.0  (May 5, 2022)
//


String.prototype.capitalize = function () {
    return this.replace(/(?:^|\s)\S/g, function (a) { return a.toUpperCase(); });
};

// **************************************************************************************************//

// <summary>
// Root object for any script function used throughout the application
// </summary>
var Ybq = Ybq || {};
Ybq.Internal = {};
Ybq.RootServer = "";        // Should be set to /vdir when deployed


// <summary>
// Return a root-based path
// </summary>
Ybq.fromServer = function (relativeUrl) {
    return Ybq.RootServer + relativeUrl;
};

// <summary>
// Helper function to call a remote URL (GET)
// </summary>
Ybq.invoke = function (url, success, error) {
    $.ajax({
        cache: false,
        url: Ybq.fromServer(url),
        success: success,
        error: error
    });
};

// <summary>
// Jump to the given ABSOLUTE URL (no transformation made on the URL)
// </summary>
Ybq.goto = function(url) {
    window.location = url;
};

// <summary>
// Jump to the given RELATIVE URL (modified with ROOTSERVER)
// </summary>
Ybq.gotoRelative = function(url) {
    window.location = Ybq.fromServer(url);
};

// <summary>
// Helper function to call a remote URL (POST)
// </summary>
Ybq.post = function (url, data, success, error) {
    var defer = $.Deferred();
    $.ajax({
        cache: false,
        url: Ybq.fromServer(url),
        type: 'post',
        data: data,
        success: success,
        error: error
    });
    defer.resolve("true");
    return defer.promise();
};




// <summary>
// Custom plugins for (animated) messages in UI
// </summary>
(function($) {
    // Add a rotating spin to the element
    $.fn.spin = function() {
        var fa = "<i class='ybq-spin ms-1 ml-1 fas fa-spinner fa-pulse'></i>";
        $(this).append(fa);
        return $(this);
    }

    // Remove a rotating spin from the element
    $.fn.unspin = function() {
        $(this).find("i.ybq-spin").remove();
        return $(this);
    }

    // Add a hiding timer for hiding the element
    $.fn.clearAfter = function(secs) {
        secs = (typeof secs !== 'undefined') ? secs : 3;
        var item = $(this);
        window.setTimeout(function () {
            $(item).addClass("d-none");
        }, secs * 1000);
        return $(this);
    }

    // Add a cleaning timer for the HTML content of the element
    $.fn.clearAfter = function(secs) {
        secs = (typeof secs !== 'undefined') ? secs : 3;
        var item = $(this);
        window.setTimeout(function () {
            $(item).html("");
        }, secs * 1000);
        return $(this);
    }

    // Add a reload timer for the current page
    $.fn.reloadAfter = function(secs) {
        secs = (typeof secs !== 'undefined') ? secs : 3;
        var item = $(this);
        window.setTimeout(function () {
            window.location.reload();
        }, secs * 1000);
        return $(this);
    }

    // Add a goto timer to navigate away
    $.fn.gotoAfter = function(url, secs) {
        secs = (typeof secs !== 'undefined') ? secs : 3;
        var item = $(this);
        window.setTimeout(function () {
            window.location.href = url;
        }, secs * 1000);
        return $(this);
    }

    // HTML writer context-sensitive
    $.fn.setMsg = function(text, success) {
        var css = success ? "text-success" : "text-danger";
        $(this).html(text).removeClass("text-success text-danger").addClass(css);
        return $(this);
    }

    // HTML writer (error message) 
    $.fn.fail = function(text) {
        return $(this).setMsg(text, false);
    }

    // Show/Hide via d-none (mostly for form overlays)
    $.fn.overlayOn = function() {
        return $(this).removeClass("d-none");
    }
    $.fn.overlayOff = function() {
        return $(this).addClass("d-none");
    }

}(jQuery));
