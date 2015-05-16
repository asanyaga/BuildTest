$(function () {
    // Deviation from planned route logic
    // This script is responsible for the map deviation reason not sold
    // convention dev_rns_xxxxx

    $("#btnMap").change(function () {
        $sel = $('#btnMap').find(":selected").val();

        if ($sel == 'dev_ZS') {
            dev_zs_Initialize();
        }
    });

    function dev_zs_Initialize() {
        LoadFilter_dev_zs();
    }

    function LoadFilter_dev_zs() {
        $('#filter-d').hide();
        $('#filter-a').empty();

        $('#filter-cd').append('<label for="dev_zs_txtFromDate">Start Date</label>');
        $('#filter-cd').append('<input id="dev_zs_txtFromDate">');
        $("#dev_zs_txtFromDate").datepicker({
            defaultDate: "+1w",
            changeMonth: true,
            changeYear: true,
            dateFormat: 'yy-mm-dd',
            defaultDate: new Date(),
            numberOfMonths: 1,
            onClose: function (selectedDate) {
                $("#dev_zs_txtToDate").datepicker("option", "minDate", selectedDate);
            }
        });
        $("#dev_zs_txtFromDate").val(getToday());
        $("#dev_zs_txtFromDate").change(function () {
            dev_zs_GetDistributors();
            dev_zs_GetPoints();
        });

        $('#filter-cd').append('<label for="dev_zs_txtToDate">End Date</label>');
        $('#filter-cd').append('<input id="dev_zs_txtToDate">');
        $("#dev_zs_txtToDate").datepicker({
            defaultDate: "+1w",
            changeMonth: true,
            changeYear: true,
            dateFormat: 'yy-mm-dd',
            defaultDate: new Date(),
            numberOfMonths: 1,
            onClose: function (selectedDate) {
                $("#dev_zs_txtFromDate").datepicker("option", "maxDate", selectedDate);
            }
        });
        $("#dev_zs_txtToDate").val(getToday());
        $("#dev_zs_txtToDate").change(function () {
            dev_zs_GetDistributors();
            dev_zs_GetPoints();
        });

        // ==================================================================================================================

        $('#filter-a').append('<label for="dev_zs_btnDistributor">Distributor</label>');

        var xa = '<select id="dev_zs_btnDistributor">';
        xa = xa + '<option selected="selected" value="ALL">ALL</option>';
        xa = xa + '</select>';
        $('#filter-a').append(xa);

        $("#dev_zs_btnDistributor").on("change", function () {
            dev_zs_GetSalesman();
            dev_zs_GetPoints();
        });

        var xb = '<label for="dev_zs_btnSalesman">SalesMan</label>';
        xb = xb + '<select id="dev_zs_btnSalesman">';
        xb = xb + '<option selected="selected" value="ALL">ALL</option>';
        xb = xb + '</select>';
        $('#filter-a').append(xb);

        $("#dev_zs_btnSalesman").on("change", function () {
            dev_zs_GetRoute();
            dev_zs_GetPoints();
        });

        var xc = '<label for="dev_zs_btnRoute">Route</label>';
        xc = xc + '<select id="dev_zs_btnRoute">';
        xc = xc + '<option selected="selected" value="ALL">ALL</option>';
        xc = xc + '</select>';
        $('#filter-a').append(xc);

        $("#dev_zs_btnRoute").on("change", function () {
            dev_zs_GetOutlets();
            dev_zs_GetPoints();
        });

        var xd = '<label for="dev_zs_btnOutlet">Outlet</label>';
        xd = xd + '<select id="dev_zs_btnOutlet">';
        xd = xd + '<option selected="selected" value="ALL">ALL</option>';
        xd = xd + '</select>';
        $('#filter-a').append(xd);

        $("#dev_zs_btnOutlet").on("change", function () {
            dev_zs_GetPoints();
        });

        dev_zs_GetDistributors();

    }

    function dev_zs_GetDistributors() {
        LoadingStart();
        var qid = load_index();
		
        // reset all to ALL
        $('#dev_zs_btnDistributor').find('option').remove();
        $("#dev_zs_btnDistributor").append("<option selected=selected value='ALL'>ALL</option>")

        $('#dev_zs_btnSalesman').find('option').remove();
        $("#dev_zs_btnSalesman").append("<option selected=selected value='ALL'>ALL</option>")

        $('#dev_zs_btnRoute').find('option').remove();
        $("#dev_zs_btnRoute").append("<option selected=selected value='ALL'>ALL</option>")

        $('#dev_zs_btnOutlet').find('option').remove();
        $("#dev_zs_btnOutlet").append("<option selected=selected value='ALL'>ALL</option>")

        var sQuery = "Dev_ZS_GetDistributors";

        var sConditions = "sDate:" + $("#dev_zs_txtFromDate").val();
        sConditions = sConditions + "|eDate:" + $("#dev_zs_txtToDate").val();

        $.ajax({
            url: window.distributr_maps_baseurl + '/map/dev_zs',
            type: 'GET',
            data: {
                Query: sQuery,
                Conditions: sConditions
            },
            dataType: "json",
            contentType: 'application/json; charset=utf-8',
            success: function (response) {
                var json = JSON.parse(response);
                $.each(json, function (i, post) {
                    //alert(post.Name)
                    $("#dev_zs_btnDistributor").append("<option value='" + post.ID + "'>" + post.Name + "</option>")
                });
            },
            error: function () {
                //your error code
            },
            complete: function () {
                if (qid == load_index()) {
                    LoadingComplete();
                }
            }
        });
    }

    function dev_zs_GetSalesman() {
        LoadingStart();
        var qid = load_index();
		
        // reset all to ALL
        $('#dev_zs_btnSalesman').find('option').remove();
        $("#dev_zs_btnSalesman").append("<option selected=selected value='ALL'>ALL</option>")

        $('#dev_zs_btnRoute').find('option').remove();
        $("#dev_zs_btnRoute").append("<option selected=selected value='ALL'>ALL</option>")

        $('#dev_zs_btnOutlet').find('option').remove();
        $("#dev_zs_btnOutlet").append("<option selected=selected value='ALL'>ALL</option>")

        var sQuery = "Dev_ZS_GetSalesMen";
        var sConditions = "sDate:" + $("#dev_zs_txtFromDate").val();
        sConditions = sConditions + "|eDate:" + $("#dev_zs_txtToDate").val();
        sConditions = sConditions + "|Distributor:" + $("#dev_zs_btnDistributor").val();

        $.ajax({
            url: window.distributr_maps_baseurl + '/map/dev_zs',
            type: 'GET',
            data: {
                Query: sQuery,
                Conditions: sConditions
            },
            dataType: "json",
            contentType: 'application/json; charset=utf-8',
            success: function (response) {
                var json = JSON.parse(response);
                $.each(json, function (i, post) {
                    $("#dev_zs_btnSalesman").append("<option value='" + post.ID + "'>" + post.Name + "</option>")
                });
            },
            error: function () {
                //your error code
            },
            complete: function () {
                if (qid == load_index()) {
                    LoadingComplete();
                }
            }
        });
    }

    function dev_zs_GetRoute() {
        LoadingStart();
        var qid = load_index();
		
        // reset all to ALL
        $('#dev_zs_btnRoute').find('option').remove();
        $("#dev_zs_btnRoute").append("<option selected=selected value='ALL'>ALL</option>")

        $('#dev_zs_btnOutlet').find('option').remove();
        $("#dev_zs_btnOutlet").append("<option selected=selected value='ALL'>ALL</option>")

        var sQuery = "Dev_ZS_GetRoutes";
        var sConditions = "sDate:" + $("#dev_zs_txtFromDate").val();
        sConditions = sConditions + "|eDate:" + $("#dev_zs_txtToDate").val();
        sConditions = sConditions + "|Distributor:" + $("#dev_zs_btnDistributor").val();
        sConditions = sConditions + "|Salesman:" + $("#dev_zs_btnSalesman").val();

        $.ajax({
            url: window.distributr_maps_baseurl + '/map/dev_zs',
            type: 'GET',
            data: {
                Query: sQuery,
                Conditions: sConditions
            },
            dataType: "json",
            contentType: 'application/json; charset=utf-8',
            success: function (response) {
                var json = JSON.parse(response);
                $.each(json, function (i, post) {
                    $("#dev_zs_btnRoute").append("<option value='" + post.ID + "'>" + post.Name + "</option>")
                });
            },
            error: function () {
                //your error code
            },
            complete: function () {
                if (qid == load_index()) {
                    LoadingComplete();
                }
            }
        });
    }

    function dev_zs_GetOutlets() {
        LoadingStart();
        var qid = load_index();
		
        // reset all to ALL
        $('#dev_zs_btnOutlet').find('option').remove();
        $("#dev_zs_btnOutlet").append("<option selected=selected value='ALL'>ALL</option>")

        var sQuery = "Dev_ZS_GetOutlets";
        var sConditions = "sDate:" + $("#dev_zs_txtFromDate").val();
        sConditions = sConditions + "|eDate:" + $("#dev_zs_txtToDate").val();
        sConditions = sConditions + "|Distributor:" + $("#dev_zs_btnDistributor").val();
        sConditions = sConditions + "|Salesman:" + $("#dev_zs_btnSalesman").val();
        sConditions = sConditions + "|Route:" + $("#dev_zs_btnRoute").val();


        $.ajax({
            url: window.distributr_maps_baseurl + '/map/dev_zs',
            type: 'GET',
            data: {
                Query: sQuery,
                Conditions: sConditions
            },
            dataType: "json",
            contentType: 'application/json; charset=utf-8',
            success: function (response) {
                var json = JSON.parse(response);
                $.each(json, function (i, post) {
                    $("#dev_zs_btnOutlet").append("<option value='" + post.ID + "'>" + post.Name + "</option>")
                });
            },
            error: function () {
                //your error code
            },
            complete: function () {
                if (qid == load_index()) {
                    LoadingComplete();
                }
            }
        });
    }

    function dev_zs_GetPoints() {
        LoadingStart();
        var qid = load_index();
		
        deleteMarkers(); // deletes the markers from the map
        deletePoly();   // removes the polyline from the map

        res_markers = []; // loads the markers to the map
        var c_markers = [];
        var db_marker = [];

        var sQuery = "Dev_ZS_GetPoints";
        var sConditions = "sDate:" + $("#dev_zs_txtFromDate").val();
        sConditions = sConditions + "|eDate:" + $("#dev_zs_txtToDate").val();
        sConditions = sConditions + "|Distributor:" + $("#dev_zs_btnDistributor").val();
        sConditions = sConditions + "|Salesman:" + $("#dev_zs_btnSalesman").val();
        sConditions = sConditions + "|Route:" + $("#dev_zs_btnRoute").val();
        sConditions = sConditions + "|Outlet:" + $("#dev_zs_btnRoute").val();

        // change the current url to make the points 
        $.ajax({
            url: window.distributr_maps_baseurl + '/map/dev_zs',
            type: 'GET',
            data: {
                Query: sQuery,
                Conditions: sConditions
            },
            contentType: 'application/json; charset=utf-8',
            dataType: "json",
            success: function (response) {
                var cur = load_index();
                if (qid == cur) {
                    var json = JSON.parse(response);
                    deleteMarkers();
                    deletePoly();
                    $.each(json, function (i, post) {
                        db_marker = [];
                        db_marker = [post.Lat, post.Lon, post.Full_Details, post.isDeviation, post.ID];
                        c_markers.push(db_marker);
                    });

                    for (var i = 0; i < c_markers.length; i++) {
                        res_markers.push(c_markers[i]);
                    }
                    setMarkerResource(res_markers);
                    ReadLoadAll();
                }
            },
            error: function (response) {
                deleteMarkers();
                deletePoly();
                db_marker = [-1.287846, 36.819373, 'No Data Available'];
                res_markers.push(db_marker);
                LoadMarkers();
                //your error code
            },
            complete: function () {
                if (qid == load_index()) {
                    LoadingComplete();
                }
            }
        });
    }

});