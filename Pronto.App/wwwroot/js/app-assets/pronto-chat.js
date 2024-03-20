///////////////////////////////////////////////////////////////////
//
// PRONTO: GPT-booking proof-of-concept
// Copyright (c) Youbiquitous
//
// Author: Youbiquitous Team
//


///////////////////////////////////////////////////////////////////
// 
//  Handler responsible for sending questions over for the 
//  GPT engine to reply based on the prompt
// 
$("#trigger-translation-send").click(function () {
    if ($("#email").val().length === 0)
        alert("Please enter the original email");
    __prontoProcessTranslation($(this), $("#email"), $("#my-language").val());
});

///////////////////////////////////////////////////////////////////
// 
//  Handler responsible for clearing input
// 
$("#trigger-email-clear").click(function () {
    $("#email").val("");
    __prontoClearConversation();
});

///////////////////////////////////////////////////////////////////
// 
//  Handler responsible for triggering send action on on-enter button click
//
$('#email').on("keydown", function (e) {
    if (e.which == 13) {
        $("#trigger-translation-send").click();
        return false;
    }
    return true;
});

///////////////////////////////////////////////////////////////////
// 
//  Handler responsible for triggering paste action
//
$('#trigger-email-paste').click(function (e) {
    if (navigator.clipboard) {
        navigator.clipboard.readText().then(function (clipboardText) {
            $("#email").val(clipboardText);
            if (clipboardText.length > 0)
            {
                $("#trigger-translation-send").click();
                $("#message").focus();
            }
        });
    }
});


///////////////////////////////////////////////////////////////////
// 
//  Handler responsible for triggering copy action
//
$('.btn-copy').click(function (e) {
    CopyText(e);
});

///////////////////////////////////////////////////////////////////
//
//  Copy action
//
function CopyText(e) {
    var copyText = $(e).parent().text();
    navigator.clipboard.writeText(copyText);
}
///////////////////////////////////////////////////////////////////
// 
//  Handler responsible for sending messages over for the 
//  GPT engine to reply based on the prompt
// 
$("#trigger-send").click(function () {
    var orig = $("#email").val();

    if (orig.length === 0)
        alert("Please enter the original email");

    var question = $("#message").val();
    $('#message').val("");
    __prontoProcessQuestion($(this), orig, question, $("#chat-container"));
});
///////////////////////////////////////////////////////////////////
// 
//  Handler responsible for clearing conversation
// 
$("#trigger-clear").click(function () {
    __prontoClearConversation();
});

///////////////////////////////////////////////////////////////////
// 
//  Handler responsible for triggering send action on on-enter button click
//
$('#message').on("keydown", function (e) {
    if (e.which == 13) {
        $("#trigger-send").click();
        return false;
    }
    return true;
});