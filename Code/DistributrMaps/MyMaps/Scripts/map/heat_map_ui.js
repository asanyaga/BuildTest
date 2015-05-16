$(function () {
    /**
    $("#btnMapType").toggleSwitch({
        highlight: true, // default
        width: 25, // default
        change: function (e) {
            // default null
        },
        stop: function (e, val) {
            // default null
        }
    });
    **/
    $("input:radio[name=btnMapType]").click(function () {
        $sel = $('#btnMap').empty();
        $val = $(this).val();
        if ($val == 'routes') {
            initialize();
            $sel.append('<option value="default">Sales & Orders</option>');
            $sel.append('<option value="dev_FPR">Deviation (From Planned Route)</option>');
            $sel.append('<option value="dev_TVM">Deviation (Target Value Missed)</option>');
            $sel.append('<option value="dev_RNS">Non Productive</option>');
            $sel.append('<option value="dev_ZS">Zero Sales</option>');
            
            // $sel.append('<option value="map_outlets">Competitor Outlets</option>');

        } else {
            initializeHeatMap();

            $sel.append('<option value="heat_visit">Visit to client</option>');
            $sel.append('<option value="heat_value">Value of Sale</option>');
        }

        $("#btnMap").change();
    });

    var map, pointArray, heatmap;
    var bounds = new google.maps.LatLngBounds();
    var LocData = [];

    function initializeHeatMap() {
        var mapOptions = {
            zoom: 13,
            center: new google.maps.LatLng(-1.287846, 36.819373),
            mapTypeId: google.maps.MapTypeId.ROADMAP
        };

        map = new google.maps.Map(document.getElementById('map-canvas'),
            mapOptions);

        pointArray = new google.maps.MVCArray(LocData);

        heatmap = new google.maps.visualization.HeatmapLayer({
            data: pointArray
        });

        heatmap.setMap(map);
        LoadFilter_heat_map();
    }

    function toggleHeatmap() {
        heatmap.setMap(heatmap.getMap() ? null : map);
    }

    function changeGradient() {
        var gradient = [
          'rgba(0, 255, 255, 0)',
          'rgba(0, 255, 255, 1)',
          'rgba(0, 191, 255, 1)',
          'rgba(0, 127, 255, 1)',
          'rgba(0, 63, 255, 1)',
          'rgba(0, 0, 255, 1)',
          'rgba(0, 0, 223, 1)',
          'rgba(0, 0, 191, 1)',
          'rgba(0, 0, 159, 1)',
          'rgba(0, 0, 127, 1)',
          'rgba(63, 0, 91, 1)',
          'rgba(127, 0, 63, 1)',
          'rgba(191, 0, 31, 1)',
          'rgba(255, 0, 0, 1)'
        ]
        heatmap.set('gradient', heatmap.get('gradient') ? null : gradient);
    }

    function changeRadius() {
        heatmap.set('radius', heatmap.get('radius') ? 15 : 30);
    }

    function changeOpacity() {
        heatmap.set('opacity', heatmap.get('opacity') ? null : 0.2);
    }

    function heat_map_Initialize() {
        LoadFilter_heat_map();
    }

    function LoadFilter_heat_map() {
        $('#filter-cb').hide();
        $('#filter-d').hide();
        $('#filter-a').empty();

        $('#filter-a').append('<label for="heat_map_txtFromDate">Start Date</label>');
        $('#filter-a').append('<input id="heat_map_txtFromDate">');
        $("#heat_map_txtFromDate").datepicker({
            defaultDate: "+1w",
            changeMonth: true,
            changeYear: true,
            dateFormat: 'yy-mm-dd',
            defaultDate: new Date(),
            numberOfMonths: 1,
            onClose: function (selectedDate) {
                $("#heat_map_txtToDate").datepicker("option", "minDate", selectedDate);
            }
        });
        $("#heat_map_txtFromDate").val(getToday());
        $("#heat_map_txtFromDate").change(function () {
            heat_map_GetDistributors();
            heat_map_GetPoints();
        });

        $('#filter-a').append('<label for="heat_map_txtToDate">End Date</label>');
        $('#filter-a').append('<input id="heat_map_txtToDate">');
        $("#heat_map_txtToDate").datepicker({
            defaultDate: "+1w",
            changeMonth: true,
            changeYear: true,
            dateFormat: 'yy-mm-dd',
            defaultDate: new Date(),
            numberOfMonths: 1,
            onClose: function (selectedDate) {
                $("#heat_map_txtFromDate").datepicker("option", "maxDate", selectedDate);
            }
        });
        $("#heat_map_txtToDate").val(getToday());
        $("#heat_map_txtToDate").change(function () {
            heat_map_GetDistributors();
            heat_map_GetPoints();
        });

        // ==================================================================================================================

        $('#filter-a').append('<label for="heat_map_btnDistributor">Distributor</label>');

        var xa = '<select id="heat_map_btnDistributor">';
        xa = xa + '<option selected="selected" value="ALL">ALL</option>';
        xa = xa + '</select>';
        $('#filter-a').append(xa);

        $("#heat_map_btnDistributor").on("change", function () {
            heat_map_GetSalesman();
            heat_map_GetPoints();
        });

        var xb = '<label for="heat_map_btnSalesman">SalesMan</label>';
        xb = xb + '<select id="heat_map_btnSalesman">';
        xb = xb + '<option selected="selected" value="ALL">ALL</option>';
        xb = xb + '</select>';
        $('#filter-a').append(xb);

        $("#heat_map_btnSalesman").on("change", function () {
            heat_map_GetRoute();
            heat_map_GetPoints();
        });

        var xc = '<label for="heat_map_btnRoute">Route</label>';
        xc = xc + '<select id="heat_map_btnRoute">';
        xc = xc + '<option selected="selected" value="ALL">ALL</option>';
        xc = xc + '</select>';
        $('#filter-a').append(xc);

        $("#heat_map_btnRoute").on("change", function () {
            heat_map_GetOutlets();
            heat_map_GetPoints();
        });

        var xd = '<label for="heat_map_btnOutlet">Outlet</label>';
        xd = xd + '<select id="heat_map_btnOutlet">';
        xd = xd + '<option selected="selected" value="ALL">ALL</option>';
        xd = xd + '</select>';
        $('#filter-a').append(xd);

        $("#heat_map_btnOutlet").on("change", function () {
            heat_map_GetPoints();
        });

        heat_map_GetDistributors();
        heat_map_GetPoints();

    }

    function heat_map_GetDistributors() {
        LoadingStart();
        var qid = load_index();

        // reset all to ALL
        $('#heat_map_btnDistributor').find('option').remove();
        $("#heat_map_btnDistributor").append("<option selected=selected value='ALL'>ALL</option>")

        $('#heat_map_btnSalesman').find('option').remove();
        $("#heat_map_btnSalesman").append("<option selected=selected value='ALL'>ALL</option>")

        $('#heat_map_btnRoute').find('option').remove();
        $("#heat_map_btnRoute").append("<option selected=selected value='ALL'>ALL</option>")

        $('#heat_map_btnOutlet').find('option').remove();
        $("#heat_map_btnOutlet").append("<option selected=selected value='ALL'>ALL</option>")

        var sQuery = "Sales_Map_GetDistributors";
        var sConditions = "sDate:" + $("#heat_map_txtFromDate").val();
        sConditions = sConditions + "|eDate:" + $("#heat_map_txtToDate").val();
 
        $.ajax({
            url: window.distributr_maps_baseurl + '/map/Sales_Map',
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
                    $("#heat_map_btnDistributor").append("<option value='" + post.ID + "'>" + post.Name + "</option>")
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

    function heat_map_GetSalesman() {
        LoadingStart();
        var qid = load_index();

        // reset all to ALL
        $('#heat_map_btnSalesman').find('option').remove();
        $("#heat_map_btnSalesman").append("<option selected=selected value='ALL'>ALL</option>")

        $('#heat_map_btnRoute').find('option').remove();
        $("#heat_map_btnRoute").append("<option selected=selected value='ALL'>ALL</option>")

        $('#heat_map_btnOutlet').find('option').remove();
        $("#heat_map_btnOutlet").append("<option selected=selected value='ALL'>ALL</option>")

        var sQuery = "Sales_Map_GetSalesMen";
        var sConditions = "sDate:" + $("#heat_map_txtFromDate").val();
        sConditions = sConditions + "|eDate:" + $("#heat_map_txtToDate").val();
        sConditions = sConditions + "|Distributor:" + $("#heat_map_btnDistributor").val();

        $.ajax({
            url: window.distributr_maps_baseurl + '/map/Sales_Map',
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
                    $("#heat_map_btnSalesman").append("<option value='" + post.ID + "'>" + post.Name + "</option>")
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

    function heat_map_GetRoute() {
        LoadingStart();
        var qid = load_index();

        // reset all to ALL
        $('#heat_map_btnRoute').find('option').remove();
        $("#heat_map_btnRoute").append("<option selected=selected value='ALL'>ALL</option>")

        $('#heat_map_btnOutlet').find('option').remove();
        $("#heat_map_btnOutlet").append("<option selected=selected value='ALL'>ALL</option>")

        var sQuery = "Sales_Map_GetRoutes";
        var sConditions = "sDate:" + $("#heat_map_txtFromDate").val();
        sConditions = sConditions + "|eDate:" + $("#heat_map_txtToDate").val();
        sConditions = sConditions + "|Distributor:" + $("#heat_map_btnDistributor").val();
        sConditions = sConditions + "|Salesman:" + $("#heat_map_btnSalesman").val();

        $.ajax({
            url: window.distributr_maps_baseurl + '/map/Sales_Map',
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
                    $("#heat_map_btnRoute").append("<option value='" + post.ID + "'>" + post.Name + "</option>")
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

    function heat_map_GetOutlets() {
        LoadingStart();
        var qid = load_index();

        // reset all to ALL
        $('#heat_map_btnOutlet').find('option').remove();
        $("#heat_map_btnOutlet").append("<option selected=selected value='ALL'>ALL</option>")

        var sQuery = "Sales_Map_GetOutlets";
        var sConditions = "sDate:" + $("#heat_map_txtFromDate").val();
        sConditions = sConditions + "|eDate:" + $("#heat_map_txtToDate").val();
        sConditions = sConditions + "|Distributor:" + $("#heat_map_btnDistributor").val();
        sConditions = sConditions + "|Salesman:" + $("#heat_map_btnSalesman").val();
        sConditions = sConditions + "|Route:" + $("#heat_map_btnRoute").val();


        $.ajax({
            url: window.distributr_maps_baseurl + '/map/Sales_Map',
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
                    $("#heat_map_btnOutlet").append("<option value='" + post.ID + "'>" + post.Name + "</option>")
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

    function heat_map_GetPoints() {
        
        LoadingStart();
        var qid = load_index();

        var mapType = $('#btnMap').val();

        var sQuery = "Sales_Map_GetPoints";
        var sConditions = "sDate:" + $("#heat_map_txtFromDate").val();
        sConditions = sConditions + "|eDate:" + $("#heat_map_txtToDate").val();
        sConditions = sConditions + "|Distributor:" + $("#heat_map_btnDistributor").val();
        sConditions = sConditions + "|Salesman:" + $("#heat_map_btnSalesman").val();
        sConditions = sConditions + "|Route:" + $("#heat_map_btnRoute").val();
        sConditions = sConditions + "|Outlet:" + $("#heat_map_btnOutlet").val();

        // change the current url to make the points 
        $.ajax({
            url: window.distributr_maps_baseurl + '/map/Sales_Map',
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
                    ClearHeatMap(); // Deletes all the points

                    var json = JSON.parse(response);
                    $.each(json, function (i, post) {
                        var point_weight = post.HeatScore;
                        if (mapType == 'heat_visit') {
                            point_weight = 1;
                        }
                        AddMarker(post.Lat, post.Lon, point_weight);
                        map.fitBounds(bounds);
                    });
                }
            },
            error: function (response) {
                if (qid == load_index) {
                    DeleteAll();
                    // AddMarker(new google.maps.LatLng(-1.287846, 36.819373));
                    // LoadMarkers();
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

    function DeleteAll() {
        pointArray = [];
        bounds = new google.maps.LatLngBounds();
    }

    function AddMarker(Lat, Lon, point_weight) {
        var marker = new google.maps.LatLng(Lat, Lon);
        pointArray.push({ location: marker, weight: point_weight });
        bounds.extend(marker);
    }

    function ClearHeatMap() {
        pointArray.clear();
    }
});