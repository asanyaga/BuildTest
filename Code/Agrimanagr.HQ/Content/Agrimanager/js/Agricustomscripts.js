$(document).ready(function() {
    $("#ddlItemsPerPage").change(function(e) {

        window.location = window.location.href.substring(0, window.location.href.indexOf('?')) + "?itemsperpage=" + $(this).val();
    });
});

