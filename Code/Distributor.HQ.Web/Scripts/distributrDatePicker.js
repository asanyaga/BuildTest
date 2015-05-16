$(function () {
    $("#datepicker").datepicker({
       dateFormat:"d-MM-yy",
        changeMonth: true,
        buttonImage:"/Content/images/cal.gif",
        showOn: "button",
        inline: true,
        showStatus: true,
        highlightWeek: true,
        showAnim: 'scale',
        changeYear: true
    });
});


//Second Date picker
$(function () {
    $("#datepicker2").datepicker({
        dateFormat: "d-MM-yy",
        changeMonth: true,
        buttonImage: "/Content/images/cal.gif",
        showOn: "button",
        inline: true,
        showStatus: true,
        highlightWeek: true,
        showAnim: 'scale',
        changeYear: true
    });
});