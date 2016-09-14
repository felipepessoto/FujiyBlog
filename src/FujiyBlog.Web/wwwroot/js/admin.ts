/// <reference path="jquery.d.ts" />
$(function () {
    $('.datetime-picker').datetimepicker({
        format: 'YYYY/MM/DD HH:mm'
    });

    $("#notify-bar").fadeIn(2000);
    $("#notify-bar").delay(5000).fadeOut(1000);
    $("#close-notify").click(function () {
        $("#notify-bar").stop(true).fadeOut('slow');
    });
});