///////////////////////////////////////////////////////////////////
//
// Youbiquitous Web Assets
// Copyright (c) Youbiquitous 2022
//
// Author: Youbiquitous Team
// v2.0.0  (April 22, 2022)
//

//var YbqForms = YbqForms || {};


//////////////////////////////////////////////////////////////////
//
// YBQ FORMS
// General text 
//
$("input[type=text]")
    .on("blur",
        function() {
            // UI elements for showing errors
            var message = $(this).data("error");
            var uiElem = $(this).data("error-ui");

            var text = $.trim($(this).val());
            var required = $(this).attr("required") !== undefined;

            // If empty but not required => valid input
            var invalid = (text.length === 0 && required);
            if (invalid) {
                $(this).addClass("is-invalid");
                if (uiElem !== null)
                    $(uiElem).html(message);
            } else {
                // Remote URL check (expect boolean reply)
                var url = $(this).data("remote-url");
                if (url !== undefined && url !== null && url.length > 0) {
                    url = url.endsWith("/") ? url : url + "/";
                    var output = $.ajax({
                        type: "POST",
                        url: url + text,
                        cache: false,
                        async: false
                    }).responseText;
                    var response = JSON.parse(output);
                    $(uiElem).html(response.success);
                    if (!response.success) {
                        $(this).addClass("is-invalid");
                        if (uiElem !== null)
                            $(uiElem).html(response.message);
                        return;
                    }
                }
                
                $(this).removeClass("is-invalid");
                if (uiElem !== null)
                    $(uiElem).html("");
            }
        })
    .on("input", 
        function() {
        // UI elements for showing errors
        var uiElem = $(this).data("error-ui");
        if (uiElem !== null)
            $(uiElem).html("");
    });


//////////////////////////////////////////////////////////////////
//
// YBQ FORMS
// Number (min/max, decimals)
//
$("input[type=number]")
    .on("keypress", 
        function (event) {
            var chr = event.charCode;
            var decimalSep = (1.1).toLocaleString().match(/\d(.*?)\d/)[1];
            if (chr === decimalSep.charCodeAt(0))
                return true;
            if (event.charCode < 48 || event.charCode > 57) {
                event.preventDefault();
                return false;
            }
            return true;
        })
    .on("keyup", 
        function () {
        var buffer = $(this).val();
        var maxLength = parseInt($(this).attr("maxlength"));
        if (buffer.length > maxLength) {
            $(this).val("");
            return false;
        }
        var minVal = parseFloat($(this).attr("min"));
        var maxVal = parseFloat($(this).attr("max"));
        if (isNaN(minVal)) {
            minVal = 0;
        }
        if (isNaN(maxVal)) {
            maxVal = 1000000;
        }
        var number = parseFloat(buffer);
        if (number < minVal || number > maxVal) {
            $(this).val("");
            return false;
        }
        return true;
    })
    .on("change",
        function() {
        var decimals = parseInt($(this).data("decimals"));
        if (isNaN(decimals))
            decimals = 0;
        var val = parseFloat($(this).val());
        $(this).val(val.toFixed(decimals));
    });

//////////////////////////////////////////////////////////////////
//
// YBQ FORMS
// Number -- autoformatting for thousands, decimals, min/max
//
$("input[type=numeric]")
    .on('keyup',
        function() {
            var requestedDecimals = parseInt($(this).data("decimals"));
            var buddy = "#" + $(this).data("ref");
            if (isNaN(requestedDecimals))
                requestedDecimals = 0;

            var raw = $(this).val();
            var num = parseFloat(raw.replace(/[^\d]/g, '')).toFixed(requestedDecimals);
            $("#amount").val(num);

            var splits = raw.split('.');
            var intPart = splits[0];
            var decPart = splits[1];

            // Handle int part
            var x = intPart.replace(/[^\d]/g, '');
            var i = parseFloat(x);
            if (isNaN(i)) {
                $(this).val("0");
                $(buddy).val("0");
                return true;
            }

            // Handle decimal part
            var intPartFmt = i.toLocaleString();
            if (requestedDecimals === 0) {
                $(this).val(intPartFmt);
                $(buddy).val(i);
                return true;
            }

            // Format
            if (decPart === undefined) {
                $(this).val(intPartFmt);
                $(buddy).val(i);
                return true;
            }
            if (decPart == null || decPart.length === 0) {
                $(this).val(intPartFmt + ".");
                $(buddy).val(i);
                return true;
            }

            var digits = decPart.substr(0, requestedDecimals);
            $(this).val(intPartFmt + "." + digits);
            $(buddy).val(i + parseFloat("0." + digits));
            return true;
        })
    .on("blur", function() {
        var buddy = "#" + $(this).data("ref");
        var requestedDecimals = parseInt($(this).data("decimals"));
        var num = parseFloat(parseFloat($(buddy).val()).toFixed(requestedDecimals));
        if (isNaN(num))
            return;
        var min = parseFloat(parseFloat($(this).attr("min")).toFixed(requestedDecimals));
        var max = parseFloat(parseFloat($(this).attr("max")).toFixed(requestedDecimals));
        if (isNaN(min)) {
            min = -1000000000000;
        }
        if (isNaN(max)) {
            max = 1000000000000;
        }

        // If empty but not required => valid input
        var text = $.trim($(this).val());
        var required = $(this).attr("required") !== undefined;
        var invalid = (text.length === 0 && required);

        // UI elements for showing errors
        var message = $(this).data("error");
        var uiElem = $(this).data("error-ui");

        if (invalid || (num < min || num > max)) {
            $(this).addClass("is-invalid");
            if (uiElem !== null)
                $(uiElem).html(message);
        } else {
            $(this).removeClass("is-invalid");
            if (uiElem !== null)
                $(uiElem).html("");
        }
    });


//////////////////////////////////////////////////////////////////
//
// YBQ FORMS
// Email 
//
$("input[type=email]").on("blur",
    function () {
        // UI elements for showing errors
        var message = $(this).data("error");
        var uiElem = $(this).data("error-ui");

        var email = $.trim($(this).val());
        var required = $(this).attr("required") !== undefined;

        // If empty but not required => valid input
        if (email.length === 0 && !required) {
            if (uiElem !== null)
                $(uiElem).html("");
            return;
        }

        // If email is required or non-blank content provided, use RE to check
        var success = __validateEmail(email);
        if (success) {
            $(this).removeClass("is-invalid");
            if (uiElem !== null)
                $(uiElem).html("");
        } else {
            $(this).addClass("is-invalid");
            if (uiElem !== null)
                $(uiElem).html(message);
        }
    })
    .on("input", function() {
        // UI elements for showing errors
        var uiElem = $(this).data("error-ui");
        if (uiElem !== null)
            $(uiElem).html("");
    });

Ybq.validateEmail = function(email) {
    return __validateEmail(email);
};

Ybq.isFormContentValid = function(selector) {
    $(selector).blur();
    var invalidCount = $(".is-invalid").length;
    return invalidCount === 0;
}


//////////////////////////////////////////////////////////////////
//
// YBQ FORMS
// URL 
//
$("input[type=url]").on("blur",
    function () {
        var url = $(this).val();
        var required = $(this).attr("required") != null;
        if (url.length === 0 && !required)
            return;

        // If URL is required or non-blank content provided, use RE to check
        var re =
            /((([A-Za-z]{3,9}:(?:\/\/)?)(?:[-;:&=\+\$,\w]+@)?[A-Za-z0-9.-]+|(?:www.|[-;:&=\+\$,\w]+@)[A-Za-z0-9.-]+)((?:\/[\+~%\/.\w-_]*)?\??(?:[-\+=&;%@.\w_]*)#?(?:[\w]*))?)/;
        var pattern = $(this).attr("pattern");
        if (pattern != null) {
            re = new RegExp($(this).attr("pattern"));
        }

        var success = re.test(url);
        if (success)
            $(this).removeClass("is-invalid");
        else
            $(this).addClass("is-invalid");
    });


//////////////////////////////////////////////////////////////////
//
// YBQ FORMS
// Text - alphanumeric only 
//
$("input[type=text][data-alphanumeric]").on("keypress",
    function (event) {
        var re = /[a-zA-Z0-9-_]/;
        var code = event.charCode || event.keyCode;    
        var chr = String.fromCharCode(code);          
        var success = re.test(chr);
        if (success) {
            return true;
        } else {
            event.preventDefault();
            return false;
        }
    });



//////////////////////////////////////////////////////////////////
//
// YBQ FORMS
// Password
//
$("input[type=password]").each(function() {
    var noClearText = $(this).data("cleartext") === "off";
    if (!noClearText) {
        $(this)
            .add(
                "<div class='input-group-append'>" +
                "<button type='button' class='btn btn-square btn-primary' onclick='__togglePswdView(this)'>" +
                "<i class='fa fa-eye-slash'></i></button></div>")
            .wrapAll("<div class='input-group' />");
    }
}).on("blur",
    function() {
        var pswd = $.trim($(this).val());
        var minLength = parseInt($(this).attr("minlength"));
        var maxLength = parseInt($(this).attr("maxlength"));
        if (isNaN(minLength)) {
            minLength = 0;
        }
        if (isNaN(maxLength)) {
            maxLength = 100;
        }
        if (pswd.length < minLength ||
            pswd.length > maxLength)
            $(this).addClass("is-invalid");
        else
            $(this).removeClass("is-invalid");
    });


//////////////////////////////////////////////////////////////////
//
// YBQ FORMS
// File - uploads
//
$(".ybq-inputfile").each(function () {
    var elem = $(this);
    __initializeInputFile(elem);
});

//YbqForms.imgLoadError = function (img) {
function __imgLoadError(img) {
    var placeholderId = $(img).data("fileid") + "-placeholder";
    $(img).hide();
    $(placeholderId).show();
    var removerId = $(img).data("fileid") + "-remover";
    $(removerId).hide();
};

// <summary>
// Sets up custom INPUT file
// </summary>
function __initializeInputFile(container) {
    var inputFile = container.find("input[type=file]").first();
    inputFile.hide();

    // Sets references to internal components
    var baseId = "#" + $(inputFile).attr("id");
    var isDefinedId = baseId + "-isdefined";
    var previewId = baseId + "-preview";
    var removerId = baseId + "-remover";
    var placeholderId = baseId + "-placeholder";
    var isAnyImageLinked = ($(isDefinedId).val() === "true");

    // Preserves original values of ISDEFINED and PREVIEW
    $(isDefinedId).data("orig", isAnyImageLinked);
    $(previewId).data("orig", $(previewId).attr("src"));

    __applyInternalState(inputFile);

    // Sets up the remover
    if (isAnyImageLinked)
        $(removerId).show();
    else
        $(removerId).hide();

    // Sets up CLICK handler for the photo placeholder
    $(placeholderId).click(function () {
        inputFile.click();
    });

    // Sets up CLICK handler for the remover
    $(removerId).click(function () {
        inputFile.val("");
        inputFile.trigger("change");
        $(previewId).removeAttr("src").removeAttr("title");
        $(previewId).hide();
        $(placeholderId).show();
        $(this).hide();
        $(isDefinedId).val("false");
        return false;
    });

    // Sets up CHANGE handler for INPUT file
    var sizeErrorMsg = inputFile.data("size-error");
    inputFile.change(function (evt) {
        var files = evt.target.files;
        if (files && files[0]) {
            var reader = new FileReader();
            reader.onload = function (e) {
                $(previewId).attr("src", e.target.result);
                $(previewId).show();
                $(placeholderId).hide();
                $(removerId).show();
                $(isDefinedId).val("true");
            };
            if (files[0].size > 2097152) { // 2MB
                var msg = sizeErrorMsg.replace("$", (files[0].size / 1024 / 1024).toFixed(2) + " MB");
                Ybq.alert(msg, false);
                $(removerId).click();
                return;
            }
            reader.readAsDataURL(files[0]);
        }
    });

    inputFile.click(function (ev) {
        return ev.stopPropagation();
    });
};

// <summary>
// Makes internal changes based on the state of INPUT elements
// </summary>
//YbqForms.applyInternalState = function (inputFile) {
function __applyInternalState(inputFile) {
    // Get further references
    var baseId = "#" + $(inputFile).attr("id");
    var isDefinedId = baseId + "-isdefined";
    var previewId = baseId + "-preview";
    var removerId = baseId + "-remover";
    var placeholderId = baseId + "-placeholder";
    var isAnyImageLinked = ($(isDefinedId).val() === "true");

    // Sets up the image placeholder  
    if (isAnyImageLinked) {
        $(placeholderId).hide();
    } else {
        $(placeholderId).show();
    }

    // Sets up the image preview
    $(previewId).data("fileid", baseId);
    if (isAnyImageLinked)
        $(previewId).show();
    else
        $(previewId).hide();
    $(previewId).click(function () {
        inputFile.click();
    });

    // Sets up the remover
    if (isAnyImageLinked)
        $(removerId).show();
    else
        $(removerId).hide();
};

// <summary>
// Reset custom INPUT file to original configuration
// </summary>
//YbqForms.resetInternalState = function (inputFile) {
function __resetInternalState(inputFile) {
    var isDefinedId = "#" + inputFile.attr("id") + "-isdefined";
    $(isDefinedId).val($(isDefinedId).data("orig"));

    var previewId = "#" + inputFile.attr("id") + "-preview";
    $(previewId).attr("src", $(previewId).data("orig"));
};

// <summary>
// Set SRC in case of missing images
// </summary>
//YbqForms.defaultImage = function(img, defaultImg) {
function __defaultImage(img, defaultImg) {
    img.onerror = "";
    img.src = defaultImg;
};


//////////////////////////////////////////////////////////////////
//
// YBQ FORMS
// Any input with focus - click specified button ID
//
$("input[data-click-on-enter]").each(function () {
    $(this).attr("onkeyup",
        "__clickOnEnter(event, '" + $(this).data("click-on-enter") + "')");
});

// <summary>
// Helper function to post the content of a HTML form
// </summary>
Ybq.postForm = function (formSelector, success, error) {
    var form = $(formSelector);
    var formData = new FormData(form[0]);
    form.find("input[type=file]").each(function () {
        formData.append($(this).attr("name"), $(this)[0].files[0]);
    });

    Ybq.notifyBeginOfOperation(formSelector);
    $.ajax({
        cache: false,
        url: form.attr("action"),
        type: form.attr("method"),
        dataType: "html",
        data: formData,
        processData: false,
        contentType: false,
        success: function (data) { Ybq.notifyEndOfOperation(formSelector); success(data); },
        error: function (data) { Ybq.notifyEndOfOperation(formSelector); error(data); }
    });
};

// <summary>
// Helper function to disable controls in the form and show a LOADING message
// </summary>
Ybq.notifyBeginOfOperation = function (formSelector) {
    $(formSelector + " input").attr("disabled", "disabled");
    $(formSelector + " button").attr("disabled", "disabled");
    $(formSelector + " a").attr("disabled", "disabled");
    $(formSelector + "-loader").show();
};

// <summary>
// Helper function to re-enable controls in the form and hide any LOADING message
// </summary>
Ybq.notifyEndOfOperation = function (formSelector) {
    $(formSelector + "-loader").hide();
    $(formSelector + " button").removeAttr("disabled");
    $(formSelector + " a").removeAttr("disabled");
    $(formSelector + " input").removeAttr("disabled");
};


// <summary>
// Greatly simplified helper function to post the content of a HTML form
// </summary>
Ybq.postFormAuto = function(formSelector, redirectUrl, clearForm, genericErrorMessage) {

    if (clearForm == null) {
        clearForm = false;
    }
    if (genericErrorMessage == null) {
        genericErrorMessage = "Oops|Something went unexpectedly wrong while posting the content";
    }
    Ybq.postForm(formSelector, 
        function (data) {
            var response = "";
            try {
                response = JSON.parse(data);
            } catch(e) {
                Ybq.alert(genericErrorMessage, false, false, 0);
                return;
            };
            var title = response.success ? "All done|" : "It didn't work|";
            Ybq.alert(title + response.message, response.success)
                .then(function () {
                    if (response.success)
                        Ybq.goto(redirectUrl);
                    if (clearForm) {
                        var form = $(formSelector);
                        form.find("input[type=text]").val("");
                        form.find("input[type=password]").val("");
                        form.find("input[type=email]").val("");
                        form.find("input[type=url]").val("");
                        form.find("input[type=number]").val("");
                        form.find("textarea").val("");
                    }
                });
        });
};



// Internal
function __clickOnEnter(event, selector) {
    if (event.keyCode === 13) {
        $(selector).click();
    }
}

function __validateEmail(email) {
    var re =
        /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    return re.test(email);
}

function __togglePswdView(elem) {
    var pswd = $(elem).closest("div[class='input-group'").find("input");
    var type = $(pswd).attr("type");
    if (type === "password") {
        pswd.attr("type", "text");
        $(elem).html("<i class='fa fa-eye'></i>");
        $(elem).removeClass("btn-primary");
    } else {
        pswd.attr("type", "password");
        $(elem).html("<i class='fa fa-eye-slash'></i>");
        $(elem).addClass("btn-primary");
    }
    pswd.focus();
}
