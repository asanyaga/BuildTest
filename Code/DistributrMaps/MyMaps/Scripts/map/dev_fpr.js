$(function () {
    // Deviation from planned route logic
    // This script is responsible for the map deviation from planned route
    // convention dev_frp_xxxxx

    $("#btnMap").change(function () {
        $sel = $('#btnMap').find(":selected").val();

        if ($sel == 'dev_FPR') {
            dev_fpr_Initialize();
        }
    });

    function dev_fpr_Initialize() {
        LoadFilter_dev_fpr();
    }

    function LoadFilter_dev_fpr() {
        $('#filter-d').hide();
        $('#filter-a').empty();

        $('#filter-cd').append('<label for="dev_fpr_txtFromDate">Start Date</label>');
        $('#filter-cd').append('<input id="dev_fpr_txtFromDate">');
        $("#dev_fpr_txtFromDate").datepicker({
            defaultDate: "+1w",
            changeMonth: true,
            changeYear: true,
            dateFormat: 'yy-mm-dd',
            defaultDate: new Date(),
            numberOfMonths: 1,
            onClose: function (selectedDate) {
                $("#dev_fpr_txtToDate").datepicker("option", "minDate", selectedDate);
            }
        });
        $("#dev_fpr_txtFromDate").val(getToday());
        $("#dev_fpr_txtFromDate").change(function () {
            dev_fpr_GetDistributors();
            dev_fpr_GetPoints();
        });

        $('#filter-cd').append('<label for="dev_fpr_txtToDate">End Date</label>');
        $('#filter-cd').append('<input id="dev_fpr_txtToDate">');
        $("#dev_fpr_txtToDate").datepicker({
            defaultDate: "+1w",
            changeMonth: true,
            changeYear: true,
            dateFormat: 'yy-mm-dd',
            defaultDate: new Date(),
            numberOfMonths: 1,
            onClose: function (selectedDate) {
                $("#dev_fpr_txtFromDate").datepicker("option", "maxDate", selectedDate);
            }
        });
        $("#dev_fpr_txtToDate").val(getToday());
        $("#dev_fpr_txtToDate").change(function () {
            dev_fpr_GetDistributors();
            dev_fpr_GetPoints();
        });

        // ==================================================================================================================

        $('#filter-a').append('<label for="dev_fpr_btnDistributor">Distributor</label>');

        var xa = '<select id="dev_fpr_btnDistributor">';
        xa = xa + '<option selected="selected" value="ALL">ALL</option>';
        xa = xa + '</select>';
        $('#filter-a').append(xa);

        $("#dev_fpr_btnDistributor").on("change", function () {
            dev_fpr_GetSalesman();
            dev_fpr_GetPoints();
        });

        var xb = '<label for="dev_fpr_btnSalesman">SalesMan</label>';
        xb = xb + '<select id="dev_fpr_btnSalesman">';
        xb = xb + '<option selected="selected" value="ALL">ALL</option>';
        xb = xb + '</select>';
        $('#filter-a').append(xb);

        $("#dev_fpr_btnSalesman").on("change", function () {
            dev_fpr_GetRoute();
            dev_fpr_GetPoints();
        });

        var xc = '<label for="dev_fpr_btnRoute">Route</label>';
        xc = xc + '<select id="dev_fpr_btnRoute">';
        xc = xc + '<option selected="selected" value="ALL">ALL</option>';
        xc = xc + '</select>';
        $('#filter-a').append(xc);

        $("#dev_fpr_btnRoute").on("change", function () {
            dev_fpr_GetOutlets();
            dev_fpr_GetPoints();
        });

        var xd = '<label for="dev_fpr_btnOutlet">Outlet</label>';
        xd = xd + '<select id="dev_fpr_btnOutlet">';
        xd = xd + '<option selected="selected" value="ALL">ALL</option>';
        xd = xd + '</select>';
        $('#filter-a').append(xd);

        $("#dev_fpr_btnOutlet").on("change", function () {
            dev_fpr_GetPoints();
        });

        dev_fpr_GetDistributors();
        dev_fpr_GetPoints();

    }

    function dev_fpr_GetDistributors() {
        LoadingStart();
        var qid = load_index();
		
        // reset all to ALL
        $('#dev_fpr_btnDistributor').find('option').remove();
        $("#dev_fpr_btnDistributor").append("<option selected=selected value='ALL'>ALL</option>")

        $('#dev_fpr_btnSalesman').find('option').remove();
        $("#dev_fpr_btnSalesman").append("<option selected=selected value='ALL'>ALL</option>")

        $('#dev_fpr_btnRoute').find('option').remove();
        $("#dev_fpr_btnRoute").append("<option selected=selected value='ALL'>ALL</option>")

        $('#dev_fpr_btnOutlet').find('option').remove();
        $("#dev_fpr_btnOutlet").append("<option selected=selected value='ALL'>ALL</option>")

        var sQuery = "Dev_FPR_GetDistributors";
        var sConditions = "sDate:" + $("#dev_fpr_txtFromDate").val();
        sConditions = sConditions + "|eDate:" + $("#dev_fpr_txtToDate").val();

        $.ajax({
            url: window.distributr_maps_baseurl + '/map/Dev_FPR',
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
                    $("#dev_fpr_btnDistributor").append("<option value='" + post.ID + "'>" + post.Name + "</option>")
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

    function dev_fpr_GetSalesman() {
        LoadingStart();
        var qid = load_index()
		
        // reset all to ALL
        $('#dev_fpr_btnSalesman').find('option').remove();
        $("#dev_fpr_btnSalesman").append("<option selected=selected value='ALL'>ALL</option>")

        $('#dev_fpr_btnRoute').find('option').remove();
        $("#dev_fpr_btnRoute").append("<option selected=selected value='ALL'>ALL</option>")

        $('#dev_fpr_btnOutlet').find('option').remove();
        $("#dev_fpr_btnOutlet").append("<option selected=selected value='ALL'>ALL</option>")

        var sQuery = "Dev_FPR_GetSalesMen";
        var sConditions = "sDate:" + $("#dev_fpr_txtFromDate").val();
        sConditions = sConditions + "|eDate:" + $("#dev_fpr_txtToDate").val();
        sConditions = sConditions + "|Distributor:" + $("#dev_fpr_btnDistributor").val();

        $.ajax({
            url: window.distributr_maps_baseurl + '/map/Dev_FPR',
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
                    $("#dev_fpr_btnSalesman").append("<option value='" + post.ID + "'>" + post.Name + "</option>")
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

    function dev_fpr_GetRoute() {
        LoadingStart();
        var qid = load_index()
		
        // reset all to ALL
        $('#dev_fpr_btnRoute').find('option').remove();
        $("#dev_fpr_btnRoute").append("<option selected=selected value='ALL'>ALL</option>")

        $('#dev_fpr_btnOutlet').find('option').remove();
        $("#dev_fpr_btnOutlet").append("<option selected=selected value='ALL'>ALL</option>")

        var sQuery = "Dev_FPR_GetRoutes";
        var sConditions = "sDate:" + $("#dev_fpr_txtFromDate").val();
        sConditions = sConditions + "|eDate:" + $("#dev_fpr_txtToDate").val();
        sConditions = sConditions + "|Distributor:" + $("#dev_fpr_btnDistributor").val();
        sConditions = sConditions + "|Salesman:" + $("#dev_fpr_btnSalesman").val();

        $.ajax({
            url: window.distributr_maps_baseurl + '/map/Dev_FPR',
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
                    $("#dev_fpr_btnRoute").append("<option value='" + post.ID + "'>" + post.Name + "</option>")
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

    function dev_fpr_GetOutlets() {
        LoadingStart();
        var qid = load_index()
		
        // reset all to ALL
        $('#dev_fpr_btnOutlet').find('option').remove();
        $("#dev_fpr_btnOutlet").append("<option selected=selected value='ALL'>ALL</option>")

        var sQuery = "Dev_FPR_GetOutlets";
        var sConditions = "sDate:" + $("#dev_fpr_txtFromDate").val();
        sConditions = sConditions + "|eDate:" + $("#dev_fpr_txtToDate").val();
        sConditions = sConditions + "|Distributor:" + $("#dev_fpr_btnDistributor").val();
        sConditions = sConditions + "|Salesman:" + $("#dev_fpr_btnSalesman").val();
        sConditions = sConditions + "|Route:" + $("#dev_fpr_btnRoute").val();

        $.ajax({
            url: window.distributr_maps_baseurl + '/map/Dev_FPR',
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
                    $("#dev_fpr_btnOutlet").append("<option value='" + post.ID + "'>" + post.Name + "</option>")
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

    function dev_fpr_GetPoints() {
        //debugger;
        LoadingStart();
        var qid = load_index();
		
        deleteMarkers(); // deletes the markers from the map
        deletePoly();   // removes the polyline from the map

        res_markers = []; // loads the markers to the map
        var c_markers = [];
        var db_marker = [];

        var sQuery = "Dev_FPR_GetPoints";
        var sConditions = "sDate:" + $("#dev_fpr_txtFromDate").val();
        sConditions = sConditions + "|eDate:" + $("#dev_fpr_txtToDate").val();
        sConditions = sConditions + "|Distributor:" + $("#dev_fpr_btnDistributor").val();
        sConditions = sConditions + "|Salesman:" + $("#dev_fpr_btnSalesman").val();
        sConditions = sConditions + "|Route:" + $("#dev_fpr_btnRoute").val();
        sConditions = sConditions + "|Outlet:" + $("#dev_fpr_btnOutlet").val();

        // change the current url to make the points 
        debugger;
        $.ajax({
            url: window.distributr_maps_baseurl + '/map/Dev_FPR',
            type: 'GET',
            data: {
                Query: sQuery,
                Conditions: sConditions
            },
            contentType: 'application/json; charset=utf-8',
            dataType: "json",
            success: function (response) {
                debugger;
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
                    LoadingComplete();
                }
                LoadingComplete();
            },
           
            error: function (response) {
                debugger;
                if (qid == load_index) {
                    deleteMarkers();
                    deletePoly();
                    db_marker = [-1.287846, 36.819373, 'No Data Available'];
                    res_markers.push(db_marker);
                    LoadMarkers();
                    alert("No Data Available");
                    LoadingComplete();
                    //your error code
                }
            },
            complete: function () {
                debugger;
                if (qid == load_index()) {
                    LoadingComplete();
                }
            }
        });
    }

});