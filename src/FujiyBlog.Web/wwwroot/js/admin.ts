$(function () {
    $('.datetime-picker').datetimepicker({
        format: 'YYYY/MM/DD HH:mm'
    });

    $("#notify-bar").slideDown(500).delay(5000).slideUp(500);
});