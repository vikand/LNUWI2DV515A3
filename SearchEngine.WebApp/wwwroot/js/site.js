// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

$(function () {
    $("button").click(function () {
        $(".waiter").css("display", "inline-block");
        $("button").css("display", "none");
    });
});
