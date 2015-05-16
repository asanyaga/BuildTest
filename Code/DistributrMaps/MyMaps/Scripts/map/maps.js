$(function () {
    // In the following example, markers appear when the user clicks on the map.
    // The markers are stored in an array.
    // The user can then click an option to hide, show or delete the markers.

    // finaly
    $("#txtFromDate").datepicker({
        defaultDate: "+1w",
        changeMonth: true,
        changeYear: true,
        dateFormat: 'yy-mm-dd',
        defaultDate: new Date(),
        numberOfMonths: 1,
        onClose: function (selectedDate) {
            $("#txtToDate").datepicker("option", "minDate", selectedDate);
        }
    });

    $("#txtToDate").datepicker({
        defaultDate: "+1w",
        changeMonth: true,
        changeYear: true,
        numberOfMonths: 1,
        dateFormat: 'yy-mm-dd',
        defaultDate: new Date(),
        onClose: function (selectedDate) {
            $("#txtFromDate").datepicker("option", "maxDate", selectedDate);
        }
    });

    // default map filters
    LoadFilter_default();
    InitializeOptions();
    var lastMap = $('#btnMap').find(":selected").val();
    window.lastMap = lastMap;

    function InitializeOptions() {
        //debugger;
        $("#accordion").accordion();
        $('#filter-cd').empty();

        var w_height = $(window).height() + 50;
        $("#left-sidebar").height(w_height);
        $("#map-canvas").height($(window).height());
        $("#info-list").hide();

        var currentDate = new Date();
        $("#txtFromDate").datepicker("setDate", currentDate);
        $("#txtToDate").datepicker("setDate", currentDate);
        $("#btnMap").val("default");
        $('#map_outlets_options').hide();
        //$("#user-key").dialog({ autoOpen: false });

        $('#spin').hide();
    }

    $.ajax({
        url: window.distributr_maps_baseurl + '/map/getdistributors',
        type: 'GET',
        data: {
            // req: 'Distributor',
            sDate: $("#txtFromDate").val(),
            eDate: $("#txtToDate").val()
        },
        dataType: "json",
        contentType: 'application/json; charset=utf-8',
        success: function (response) {
            $.each(response, function (i, post) {
                //alert(post.Name)
                $("#btnDistributor").append("<option value='" + post.ID + "'>" + post.Name + "</option>")
            });
        },
        error: function () {
            //your error code
        }
    });

    function isValidDate(value, userFormat) {
        userFormat = userFormat || 'yyyy-mm-dd', // default format

		delimiter = /[^mdy]/.exec(userFormat)[0],
		theFormat = userFormat.split(delimiter),
		theDate = value.split(delimiter),

		isDate = function (date, format) {
		    var m, d, y
		    for (var i = 0, len = format.length; i < len; i++) {
		        if (/m/.test(format[i])) m = date[i]
		        if (/d/.test(format[i])) d = date[i]
		        if (/y/.test(format[i])) y = date[i]
		    }
		    return (
			  m > 0 && m < 13 &&
			  y && y.length === 4 &&
			  d > 0 && d <= (new Date(y, m, 0)).getDate()
			)
		}

        return isDate(theDate, theFormat)

    }


    // -----------------------------------------------------------------------------
    // obj stru : title , lat , lon , positon_on_route , id

    var dv_planned_markers = [];    // a list of all the locations visited that were not from the intended route;
    var dv_target_markers = []; // a list of all locations where target sales were not achieved;
    var dv_reason_markers = []; // a list of all locations reasons for underachieved sales were reported;
    var dv_zero_sales_markers = []; // a list of all locations where no sales were made;
    var visited_markers = []; // a list of all markers that were visited;

    // -----------------------------------------------------------------------------

    var map; // global var of the map
    var markers = []; // collection of markers to be traslated to the map
    var res_markers = []; // a more detailed result of the markers
    var flightPath;
    var flightPlanCoordinates = [];
    var heatmap; 

    function initialize() {
        var haightAshbury = new google.maps.LatLng(-1.287846, 36.819373);

        var mapOptions = {
            zoom: 12,
            center: haightAshbury,
            mapTypeId: google.maps.MapTypeId.ROADMAP
        };
        map = new google.maps.Map(document.getElementById('map-canvas'),
			mapOptions);

        flightPath = new google.maps.Polyline({
            path: flightPlanCoordinates,
            geodesic: true,
            strokeColor: '#33CCFF',
            strokeOpacity: 1.0,
            strokeWeight: 2
        });

    }
    window.initialize = initialize;

    function setMarkerResource(resource) {
        res_markers = resource;
    }
    window.setMarkerResource = setMarkerResource;

    // Add a marker to the map and push to the array.
    function addMarker(location) {

        var infowindow = new google.maps.InfoWindow();
        stitle = location[1];
        stitle = stitle.replace("<br/>", " , ");
        stitle = stitle.replace("<br/>", " , ");
        stitle = stitle.replace("<br/>", " , ");
        stitle = stitle.replace("<br/>", " , ");

        var icon_index = location[2] + 1;
        var marker_type = $('#btnMap').find(":selected").val()

        var popup_content = location[1];

        if (marker_type == 'default') {
            // popup_content = location[1] + '<br/>' + location[5];
        }

        var marker = new google.maps.Marker({
            position: location[0],
            map: map,
            isdev : location[3],
            title: stitle,
            popup: popup_content,
            id: location[4]
        });

        var icon = GetMarkerColor(icon_index, marker_type, res_markers.length);
        marker.setIcon(icon);

        markers.push(marker);
        google.maps.event.addListener(marker, 'click', function (event) {
            // make a jquery call to load all transactions
            infowindow.setContent(this.popup);
            infowindow.open(map, this);

            if (this.isdev != 1) {
                // DisplayTransactions(marker.id);
            }
        });
    }

    function GetMarkerColor(icon_index, marker_type, marker_count) {
        res_str = "";
        str_gicon = "http://chart.apis.google.com/chart?chst=d_map_pin_letter&chld=";

        // orange start point
        if (icon_index == 1) {
            res_str = str_gicon + icon_index + "|D9B309|000000"
        }

        // grey end point
        if (icon_index == marker_count) {
            res_str = str_gicon + icon_index + "|828282|000000"
        }

        // blue deviation
        if (marker_type == 'dev_FPR' || marker_type == 'dev_TVM') {
            res_str = str_gicon + icon_index + "|5e68b6|FFFFFF"
        }

        // red unproductive
        if (marker_type == 'dev_RNS') {
            res_str = str_gicon + icon_index + "|E64500|000000"
        }

        // black no sale
        if (marker_type == 'dev_ZS') {
            res_str = str_gicon + icon_index + "|000000|FFFFFF"
        }

        // default is green
        if (res_str == "") {
            res_str = str_gicon + icon_index + "|20992A|000000"
        }

        return res_str;
    }

    function toggleHeatmap() {
        heatmap.setMap(heatmap.getMap() ? null : map);
    }

    window.toggleHeatmap = toggleHeatmap;

    // Loads the transactions
    function DisplayTransactions(markerid) {

        $.ajax({
            url: window.distributr_maps_baseurl + '/map/transactions',
            type: 'GET',
            data: {
                // req: 'Transactions',
                sDate: $("#txtFromDate").val(),
                eDate: $("#txtToDate").val(),
                resultID: markerid
            },
            dataType: "json",
            contentType: 'application/json; charset=utf-8',
            success: function (response) {
                $("#transactions").find('tbody').remove();
                var str = "<tbody>";
                var json = JSON.parse(response);
                $.each(json, function (i, post) {

                    str = str + "<tr>"
                    str = str + "<td>" + post.ID + "</td>"
                    str = str + "<td>" + post.DocumentDateIssued + "</td>"
                    str = str + "<td>" + post.OutLet + "</td>"
                    str = str + "<td>" + post.Salesman + "</td>"
                    str = str + "<td>" + post.SalesAmount + "</td>"
                    str = str + "<td>" + post.SaleDiscount + "</td>"
                    str = str + "<td>" + post.ProductDiscount + "</td>"

                    str = str + "</tr>"

                });

                str = str + "</tbody>"
                $("#transactions").append(str);
            },
            error: function (response) {
                //your error code

            }
        });
    }

    // Sets the map on all markers in the array.
    function setAllMap(map) {
        for (var i = 0; i < markers.length; i++) {
            markers[i].setMap(map);
        }
    }

    // Removes the markers from the map, but keeps them in the array.
    function clearMarkers() {
        setAllMap(null);
    }

    // Shows any markers currently in the array.
    function showMarkers() {
        setAllMap(map);
    }

    // Deletes all markers in the array by removing references to them.
    function deleteMarkers() {
        clearMarkers();
        markers = [];
    }
    window.deleteMarkers = deleteMarkers;

    function LoadMarkers() {
        var bounds = new google.maps.LatLngBounds();
        for (var i = 0; i < res_markers.length; i++) {
            var p = res_markers[i];
            var latlng = new google.maps.LatLng(p[0], p[1]);
            var title = p[2];
            var deviation = p[3];
            var markerid = p[4];
            var pos = i;
            var link = p[5];

            bounds.extend(latlng);
            var point = [latlng, title, pos, deviation, markerid, link];
            addMarker(point);
        }
        map.fitBounds(bounds);
    }
    window.LoadMarkers = LoadMarkers;

    function deletePoly() {

        flightPath.setMap(null);
    }
    window.deletePoly = deletePoly;

    function showPoly() {
        flightPath.setMap(map);
    }

    function LoadPoly() {
        flightPlanCoordinates = [];

        for (var i = 0; i < res_markers.length; i++) {
            var p = res_markers[i];
            var latlng = new google.maps.LatLng(p[0], p[1]);
            flightPlanCoordinates.push(latlng);
        }

        var lineSymbol = {
            path: google.maps.SymbolPath.CIRCLE,
            scale: 8,
            strokeColor: '#0000FF'
        };

        flightPath = new google.maps.Polyline({
            path: flightPlanCoordinates,
            geodesic: true,
            icons: [{
                icon: lineSymbol,
                offset: '100%'
            }],
            strokeColor: '#88E9FC',
            strokeOpacity: 1.0,
            strokeWeight: 2
        });

        flightPath.setMap(map);
        animateCircle();
    }

    function animateCircle() {
        var count = 0;
        window.setInterval(function () {
            count = (count + 1) % 400;

            var icons = flightPath.get('icons');
            icons[0].offset = (count / 2) + '%';
            flightPath.set('icons', icons);
        }, 20);
    }

    function ReadLoadAll() {

        $("#ckhMarkers").prop("checked", true);
        $("#ckhPoly").prop("checked", true);
        LoadPoly();
        LoadMarkers();
    }
    window.ReadLoadAll = ReadLoadAll;

    google.maps.event.addDomListener(window, 'load', initialize);

    // ui funtions

    function ReloadMapLocs() {
        LoadingStart();
        var qid = load_index();

        deleteMarkers(); // deletes the markers from the map
        deletePoly();   // removes the polyline from the map

        res_markers = []; // loads the markers to the map
        var c_markers = [];
        var db_marker = [];

        // change the current url to make the points 
        $.ajax({
            url: window.distributr_maps_baseurl + '/map/locations',
            type: 'GET',
            data: {
                // req: 'Locations',
                Distributor: $("#btnDistributor option:selected").val(),
                Salesman: $('#btnSalesman').find(":selected").val(),
                Route: $('#btnRoute').find(":selected").val(),
                Outlet: $('#btnOutlet').find(":selected").val(),
                sDate: $("#txtFromDate").val(),
                eDate: $("#txtToDate").val(),
                MapType: $('#btnMap').find(":selected").val()
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
                        db_marker = [post.Lat, post.Lon, post.Full_Details, post.isDeviation, post.ID, post.Link];
                        c_markers.push(db_marker);
                    });
                    for (var i = 0; i < c_markers.length; i++) {
                        res_markers.push(c_markers[i]);
                    }
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

    function reloadDistributor() {
        LoadingStart();
        var qid = load_index()

        // reset all to ALL
        $('#btnDistributor').find('option').remove();
        $("#btnDistributor").append("<option selected=selected value='ALL'>ALL</option>")

        $('#btnSalesman').find('option').remove();
        $("#btnSalesman").append("<option selected=selected value='ALL'>ALL</option>")

        $('#btnRoute').find('option').remove();
        $("#btnRoute").append("<option selected=selected value='ALL'>ALL</option>")

        $('#btnOutlet').find('option').remove();
        $("#btnOutlet").append("<option selected=selected value='ALL'>ALL</option>")
        $.ajax({
            url: window.distributr_maps_baseurl + '/map/getdistributors',
            type: 'GET',
            data: {
                // req: 'Distributor',
                sDate: $("#txtFromDate").val(),
                eDate: $("#txtToDate").val()
            },
            dataType: "json",
            contentType: 'application/json; charset=utf-8',
            success: function (response) {
                var json = JSON.parse(response);
                $.each(json, function (i, post) {
                    $("#btnDistributor").append("<option value='" + post.ID + "'>" + post.Name + "</option>")
                });
            },
            error: function (e) {
                //your error code
                alert(e);
            },
            complete: function () {
                if (qid == load_index()) {
                    LoadingComplete();
                }
            }
        });
    }

    function reloadSalesman() {
        LoadingStart();
        var qid = load_index()

        $('#btnSalesman').find('option').remove();
        $("#btnSalesman").append("<option selected=selected value='ALL'>ALL</option>")

        $('#btnRoute').find('option').remove();
        $("#btnRoute").append("<option selected=selected value='ALL'>ALL</option>")

        $('#btnOutlet').find('option').remove();
        $("#btnOutlet").append("<option selected=selected value='ALL'>ALL</option>")

        $.ajax({
            url: window.distributr_maps_baseurl + '/map/getSalesman',
            type: 'GET',
            data: {
                sDate: $("#txtFromDate").val(),
                eDate: $("#txtToDate").val(),
                Distributor: $("#btnDistributor option:selected").val()

            },
            dataType: "json",
            contentType: 'application/json; charset=utf-8',
            success: function (response) {
                var json = JSON.parse(response);
                $.each(json, function (i, post) {
                    $("#btnSalesman").append("<option value='" + post.ID + "'>" + post.Name + "</option>")
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

    function reloadRoutes() {
        debugger;
        LoadingStart();
        var qid = load_index()

        $('#btnRoute').find('option').remove();
        $("#btnRoute").append("<option selected=selected value='ALL'>ALL</option>")

        $('#btnOutlet').find('option').remove();
        $("#btnOutlet").append("<option selected=selected value='ALL'>ALL</option>")

        $.ajax({
            url: window.distributr_maps_baseurl + '/map/GetRoutes',
            type: 'GET',
            data: {
                // req: 'Routes',
                sDate: $("#txtFromDate").val(),
                eDate: $("#txtToDate").val(),
                Distributor: $("#btnDistributor option:selected").val(),
                Salesman: $("#btnSalesman option:selected").val()

            },
            dataType: "json",
            contentType: 'application/json; charset=utf-8',
            success: function (response) {
                var json = JSON.parse(response);
                $.each(json, function (i, post) {
                    $("#btnRoute").append("<option value='" + post.ID + "'>" + post.Name + "</option>")
                });
            },
            error: function (response) {
                //your error code
            },
            complete: function () {
                if (qid == load_index()) {
                    LoadingComplete();
                }
            }
        });
    }

    function reloadOutlet() {
        debugger;
        LoadingStart();
        var qid = load_index()

        $('#btnOutlet').find('option').remove();
        $("#btnOutlet").append("<option selected=selected value='ALL'>ALL</option>")

        $.ajax({
            url: window.distributr_maps_baseurl + '/map/GetOutlets',
            type: 'GET',
            data: {
                // req: 'Outlet',
                sDate: $("#txtFromDate").val(),
                eDate: $("#txtToDate").val(),
                Distributor: $("#btnDistributor option:selected").val(),
                Salesman: $("#btnSalesman option:selected").val(),
                Route: $("#btnRoute option:selected").val()

            },
            dataType: "json",
            contentType: 'application/json; charset=utf-8',
            success: function (response) {
                var json = JSON.parse(response);
                $.each(json, function (i, post) {
                    $("#btnOutlet").append("<option value='" + post.ID + "'>" + post.Name + "</option>")
                });
            },
            error: function (e) {
                //your error code
                alert(e);
            },
            complete: function () {
                if (qid == load_index()) {
                    LoadingComplete();
                }
            }
        });
    }

    function LoadFilter_default() {
        $('#filter-d').show();
        $('#filter-a').empty();

        $('#filter-a').append('<label for="btnDistributor">Distributor</label>');

        var xa = '<select id="btnDistributor">';
        xa = xa + '<option selected="selected" value="ALL">ALL</option>';
        xa = xa + '</select>';
        $('#filter-a').append(xa);

        $("#btnDistributor").on("change", function () {
            reloadSalesman();
            ReloadMapLocs();
        });

        var xb = '<label for="btnSalesman">SalesMan</label>';
        xb = xb + '<select id="btnSalesman">';
        xb = xb + '<option selected="selected" value="ALL">ALL</option>';
        xb = xb + '</select>';
        $('#filter-a').append(xb);

        $("#btnSalesman").on("change", function () {
            reloadRoutes();
            ReloadMapLocs();
        });

        var xc = '<label for="btnRoute">Route</label>';
        xc = xc + '<select id="btnRoute">';
        xc = xc + '<option selected="selected" value="ALL">ALL</option>';
        xc = xc + '</select>';
        $('#filter-a').append(xc);

        $("#btnRoute").on("change", function () {
            reloadOutlet();
            ReloadMapLocs();
        });

        var xd = '<label for="btnOutlet">Outlet</label>';
        xd = xd + '<select id="btnOutlet">';
        xd = xd + '<option selected="selected" value="ALL">ALL</option>';
        xd = xd + '</select>';
        $('#filter-a').append(xd);

        $("#btnOutlet").on("change", function () {
            ReloadMapLocs();
        });

    }

    $("#txtFromDate").change(function () {
        reloadDistributor();
        ReloadMapLocs();
    });

    $("#txtToDate").change(function () {
        reloadDistributor();
        ReloadMapLocs();
    });

    $("#btnClear").click(function () {
        clearMarkers();
        deletePoly();
    });

    $("#btnShow").click(function () {
        showMarkers();
        showPoly();
    });

    $('#ckhPoly').change(function () {
        if ($(this).is(":checked")) {
            showPoly();
        } else {
            deletePoly();
        }
    });

    $('#ckhMarkers').change(function () {
        if ($(this).is(":checked")) {
            showMarkers();
        } else {
            clearMarkers();
        }
    });

    $('#ckhShowKey').change(function () {
        if ($(this).is(":checked")) {
            $('#user-key').show();
        } else {
            $('#user-key').hide();
        }
    });

    $('#ckhSales').change(function () {
        if ($(this).is(":checked")) {
            $("#info-list").show("bounce", { times: 4 });
        } else {
            $("#info-list").hide("bounce", { times: 4 });
        }
    });

    $("#btnMap").change(function () {
        deleteMarkers();
        deletePoly();

        $('#filter-cd').empty();
        $sel = $('#btnMap').find(":selected").text();
        document.title = $sel;

        $val = $('#btnMap').find(":selected").val();

        if ($val == 'default') {
            LoadFilter_default();
        }

        if ($val != 'heat_visit' && $val != 'heat_value' && $val != 'map_outlets') {
            $('#filter-cb').show();
        } else {
            $('#filter-cb').hide();
        }

        if ($val == 'map_outlets') {
            $('#map_outlets_options').show();
        } else {
            $('#map_outlets_options').hide();
        }

    });

    $("#menu").click(function () {
        var txt = $(this).text();
        if (txt === 'Hide Menu') {
            $(this).text('Show Menu');
            $("#panel").hide();
        } else {
            $(this).text('Hide Menu');
            $("#panel").show();
        }
    });

    function getToday() {
        var today = new Date();
        var dd = today.getDate();
        var mm = today.getMonth() + 1; //January is 0!
        var yyyy = today.getFullYear();

        if (dd < 10) {
            dd = '0' + dd
        }

        if (mm < 10) {
            mm = '0' + mm
        }

        today = yyyy + '-' + mm + '-' + dd;
        return today;
    }
    window.getToday = getToday;

    var loading_id = 0;
    function load_index() {
        return loading_id;
    }
    window.load_index = load_index;

    var load_buffer = [];

    function LoadingStart() {
        load_buffer = []; // reset the buffer

        loading_id = loading_id + 1;
        load_buffer.push(loading_id);
        $('#spin').show();
    }
    window.LoadingStart = LoadingStart;

    function LoadingComplete() {
        load_buffer.pop(loading_id);
        if (load_buffer.length == 0) {
            $('#spin').hide();
        }
    }
    window.LoadingComplete = LoadingComplete;

    function LoadingUI(){
        // show the loading icon
        
    }

});