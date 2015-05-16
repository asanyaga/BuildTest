$(function () {
    // Deviation from planned route logic
    // This script is responsible for the map deviation reason not sold
    // convention dev_rns_xxxxx

    $("#btnMap").change(function () {
        $sel = $('#btnMap').find(":selected").val();

        if ($sel == 'dev_RNS') {
            dev_rns_Initialize();
        }
    });

    function dev_rns_Initialize() {
        LoadFilter_dev_rns();
    }

    function LoadFilter_dev_rns() {
        $('#filter-d').hide();
        $('#filter-a').empty();

        $('#filter-cd').append('<label for="dev_rns_txtFromDate">Start Date</label>');
        $('#filter-cd').append('<input id="dev_rns_txtFromDate">');
        $("#dev_rns_txtFromDate").datepicker({
            defaultDate: "+1w",
            changeMonth: true,
            changeYear: true,
            dateFormat: 'yy-mm-dd',
            defaultDate: new Date(),
            numberOfMonths: 1,
            onClose: function (selectedDate) {
                $("#dev_rns_txtToDate").datepicker("option", "minDate", selectedDate);
            }
        });
        $("#dev_rns_txtFromDate").val(getToday());
        $("#dev_rns_txtFromDate").change(function () {
            dev_rns_GetDistributors();
        });

        $('#filter-cd').append('<label for="dev_rns_txtToDate">End Date</label>');
        $('#filter-cd').append('<input id="dev_rns_txtToDate">');
        $("#dev_rns_txtToDate").datepicker({
            defaultDate: "+1w",
            changeMonth: true,
            changeYear: true,
            dateFormat: 'yy-mm-dd',
            defaultDate: new Date(),
            numberOfMonths: 1,
            onClose: function (selectedDate) {
                $("#dev_rns_txtFromDate").datepicker("option", "maxDate", selectedDate);
            }
        });
        $("#dev_rns_txtToDate").val(getToday());
        $("#dev_rns_txtToDate").change(function () {
            dev_rns_GetDistributors();
            dev_rns_GetPoints();
        });

        // ==================================================================================================================

        $('#filter-a').append('<label for="dev_rns_btnDistributor">Distributor</label>');

        var xa = '<select id="dev_rns_btnDistributor">';
        xa = xa + '<option selected="selected" value="ALL">ALL</option>';
        xa = xa + '</select>';
        $('#filter-a').append(xa);

        $("#dev_rns_btnDistributor").on("change", function () {
            dev_rns_GetSalesman();
            dev_rns_GetPoints();
        });

        var xb = '<label for="dev_rns_btnSalesman">SalesMan</label>';
        xb = xb + '<select id="dev_rns_btnSalesman">';
        xb = xb + '<option selected="selected" value="ALL">ALL</option>';
        xb = xb + '</select>';
        $('#filter-a').append(xb);

        $("#dev_rns_btnSalesman").on("change", function () {
            dev_rns_GetRoute();
            dev_rns_GetPoints();
        });

        var xc = '<label for="dev_rns_btnRoute">Route</label>';
        xc = xc + '<select id="dev_rns_btnRoute">';
        xc = xc + '<option selected="selected" value="ALL">ALL</option>';
        xc = xc + '</select>';
        $('#filter-a').append(xc);

        $("#dev_rns_btnRoute").on("change", function () {
            dev_rns_GetOutlets();
            dev_rns_GetPoints();
        });

        var xd = '<label for="dev_rns_btnOutlet">Outlet</label>';
        xd = xd + '<select id="dev_rns_btnOutlet">';
        xd = xd + '<option selected="selected" value="ALL">ALL</option>';
        xd = xd + '</select>';
        $('#filter-a').append(xd);

        $("#dev_rns_btnOutlet").on("change", function () {
            dev_rns_GetPoints();
        });

        dev_rns_GetDistributors();

    }

    function dev_rns_GetDistributors() {
        LoadingStart();
        var qid = load_index();
		
        // reset all to ALL
        $('#dev_rns_btnDistributor').find('option').remove();
        $("#dev_rns_btnDistributor").append("<option selected=selected value='ALL'>ALL</option>")

        $('#dev_rns_btnSalesman').find('option').remove();
        $("#dev_rns_btnSalesman").append("<option selected=selected value='ALL'>ALL</option>")

        $('#dev_rns_btnRoute').find('option').remove();
        $("#dev_rns_btnRoute").append("<option selected=selected value='ALL'>ALL</option>")

        $('#dev_rns_btnOutlet').find('option').remove();
        $("#dev_rns_btnOutlet").append("<option selected=selected value='ALL'>ALL</option>")

        var sQuery = "Dev_RNS_GetDistributors";
        var sConditions = "sDate:" + $("#dev_rns_txtFromDate").val();
        sConditions = sConditions + "|eDate:" + $("#dev_rns_txtToDate").val();

        $.ajax({
            url: window.distributr_maps_baseurl + '/map/Dev_RNS',
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
                    $("#dev_rns_btnDistributor").append("<option value='" + post.ID + "'>" + post.Name + "</option>")
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

    function dev_rns_GetSalesman() {
        LoadingStart();
        var qid = load_index();
		
        // reset all to ALL
        $('#dev_rns_btnSalesman').find('option').remove();
        $("#dev_rns_btnSalesman").append("<option selected=selected value='ALL'>ALL</option>")

        $('#dev_rns_btnRoute').find('option').remove();
        $("#dev_rns_btnRoute").append("<option selected=selected value='ALL'>ALL</option>")

        $('#dev_rns_btnOutlet').find('option').remove();
        $("#dev_rns_btnOutlet").append("<option selected=selected value='ALL'>ALL</option>")

        var sQuery = "Dev_RNS_GetSalesMen";
        var sConditions = "sDate:" + $("#dev_rns_txtFromDate").val();
        sConditions = sConditions + "|eDate:" + $("#dev_rns_txtToDate").val();
        sConditions = sConditions + "|Distributor:" + $("#dev_rns_btnDistributor").val();

        $.ajax({
            url: window.distributr_maps_baseurl + '/map/Dev_RNS',
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
                    $("#dev_rns_btnSalesman").append("<option value='" + post.ID + "'>" + post.Name + "</option>")
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

    function dev_rns_GetRoute() {
        LoadingStart();
        var qid = load_index();
		
        // reset all to ALL
        $('#dev_rns_btnRoute').find('option').remove();
        $("#dev_rns_btnRoute").append("<option selected=selected value='ALL'>ALL</option>")

        $('#dev_rns_btnOutlet').find('option').remove();
        $("#dev_rns_btnOutlet").append("<option selected=selected value='ALL'>ALL</option>")

        var sQuery = "Dev_RNS_GetRoutes";
        var sConditions = "sDate:" + $("#dev_rns_txtFromDate").val();
        sConditions = sConditions + "|eDate:" + $("#dev_rns_txtToDate").val();
        sConditions = sConditions + "|Distributor:" + $("#dev_rns_btnDistributor").val();
        sConditions = sConditions + "|Salesman:" + $("#dev_rns_btnSalesman").val();

        $.ajax({
            url: window.distributr_maps_baseurl + '/map/Dev_RNS',
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
                    $("#dev_rns_btnRoute").append("<option value='" + post.ID + "'>" + post.Name + "</option>")
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

    function dev_rns_GetOutlets() {
        LoadingStart();
        var qid = load_index();
		
        // reset all to ALL
        $('#dev_rns_btnOutlet').find('option').remove();
        $("#dev_rns_btnOutlet").append("<option selected=selected value='ALL'>ALL</option>")
       
        var sQuery = "Dev_RNS_GetOutlets";
        var sConditions = "sDate:" + $("#dev_rns_txtFromDate").val();
        sConditions = sConditions + "|eDate:" + $("#dev_rns_txtToDate").val();
        sConditions = sConditions + "|Distributor:" + $("#dev_rns_btnDistributor").val();
        sConditions = sConditions + "|Salesman:" + $("#dev_rns_btnSalesman").val();
        sConditions = sConditions + "|Route:" + $("#dev_rns_btnRoute").val();

        //alert(sConditions);

        $.ajax({
            url: window.distributr_maps_baseurl + '/map/Dev_RNS',
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
                    $("#dev_rns_btnOutlet").append("<option value='" + post.ID + "'>" + post.Name + "</option>")
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

    function dev_rns_GetPoints() {
        LoadingStart();
        var qid = load_index();
		
        deleteMarkers(); // deletes the markers from the map
        deletePoly();   // removes the polyline from the map

        res_markers = []; // loads the markers to the map
        var c_markers = [];
        var db_marker = [];

        var sQuery = "Dev_RNS_GetPoints";
        var sConditions = "sDate:" + $("#dev_rns_txtFromDate").val();
        sConditions = sConditions + "|eDate:" + $("#dev_rns_txtToDate").val();
        sConditions = sConditions + "|Distributor:" + $("#dev_rns_btnDistributor").val();
        sConditions = sConditions + "|Salesman:" + $("#dev_rns_btnSalesman").val();
        sConditions = sConditions + "|Route:" + $("#dev_rns_btnRoute").val();
        sConditions = sConditions + "|Outlet:" + $("#dev_rns_btnOutlet").val();

        // change the current url to make the points 
        $.ajax({
            url: window.distributr_maps_baseurl + '/map/Dev_RNS',
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
                if (qid == load_index) {
                    deleteMarkers();
                    deletePoly();
                    db_marker = [-1.287846, 36.819373, 'No Data Available'];
                    res_markers.push(db_marker);
                    LoadMarkers();
                    //your error code
                }
            },
            complete: function () {
                if (qid == load_index()) {
                    LoadingComplete();
                }
            }
        });
    }




});