$(function () {
    // Deviation from planned route logic
    // This script is responsible for the map deviation reason not sold
    // convention dev_map_outlets_xxxxx

    $("#btnMap").change(function () {
        $sel = $('#btnMap').find(":selected").val();

        if ($sel == 'map_outlets') {
            dev_map_outlets_Initialize();
        }
    });

    $("#btnMap").change(function () {
        selVal =$('#btnMap').find(":selected").val()
        if (lastMap == 'map_outlets' && selVal != lastMap) {
            initialize();
        }

        if (lastMap != 'map_outlets' && selVal == 'map_outlets') {
            initOutletMap();
        }

        lastMap = $('#btnMap').find(":selected").val();

    });


    //  map
    var markersArray = [];
    var my_markersArray = [];
    var competitorLocsArray = [];
    var myOutlets = [];
    // [latlng, post.Outlet, post.OutletID, post.Competitor, post.CompetitorID];
    var my_color = '009933';
    var colours = ['666633', '0066FF', 'CCCC00', '333399', 'FF9999', 'CC0066', 'FFFF00', 'FFCC00', '993333', '523D66', '999966', '669999', 'CC99FF', 'FFCCFF'];
    var competitors = [];
    var exclude_competitors = [];
    var bounds = new google.maps.LatLngBounds();
    var load_mine = 0;


    function dev_map_outlets_Initialize() {
        LoadFilter_dev_map_outlets();

        $('#chkDisplayLocal').click(function () {

            if ($('#chkDisplayLocal').is(":checked")) {
                if (load_mine == 0) {
                    dev_map_outlets_MyOutlets();
                } else {
                    ReloadMyMarkers();
                }
            } else {
                HideMyMarkers();
            }

        });
    }


    function initOutletMap() {
        var mapOptions = {
            zoom: 13,
            center: new google.maps.LatLng(-1.287846, 36.819373),
            mapTypeId: google.maps.MapTypeId.ROADMAP
        };

        map = new google.maps.Map(document.getElementById('map-canvas'),  mapOptions);


        // add a click event handler to the map object
        google.maps.event.addListener(map, "click", function (event) {
            // place a marker
            placeMarker(event.latLng);

            // display the lat/lng in your form's lat/lng fields
            $lat = event.latLng.lat();
            $lon = event.latLng.lng();

            if ($('#ckhClickToAdd').is(":checked")) {

                $("#btnSaveMapOutletLat").val($lat);
                $("#btnSaveMapOutletLon").val($lon);
                $('#saveDialog').dialog("open");
            }
        });
    }

    function placeMarker(location) {
        // first remove all markers if there are any
        try{
            deleteOverlays();
        }catch(e){

        }

        var marker = new google.maps.Marker({
            position: location,
            map: map
        });

        // add marker in markers array
        markersArray.push(marker);

        //map.setCenter(location);
    }

    function LoadCompetitorMarkers() {
        for (var i = 0; i < competitorLocsArray.length; i++) {

                var ci = competitors.indexOf(competitorLocsArray[i][4]);
                if (exclude_competitors.indexOf(competitorLocsArray[i][4]) == -1) {
                    var infowindow = new google.maps.InfoWindow();
                    var top = 'Outlet : ' + competitorLocsArray[i][1] + '<br/>' + 'Competitor : ' + competitorLocsArray[i][3];
                    var marker = new google.maps.Marker({
                        position: competitorLocsArray[i][0],
                        map: map,
                        title: top
                    });
                    marker.setIcon('http://chart.apis.google.com/chart?chst=d_map_pin_letter&chld= |' + colours[ci] + '|' + colours[ci]);


                    markersArray.push(marker);
                    google.maps.event.addListener(marker, 'click', function (event) {
                        // make a jquery call to load all transactions
                        infowindow.setContent(this.title);
                        infowindow.open(map, this);
                    });

                }
            }
        ReloadAllMarkers();
    }

    function LoadMyMarkers() {
        for (var i = 0; i < myOutlets.length; i++) {

                var infowindow = new google.maps.InfoWindow();
                var top = 'Outlet : ' + myOutlets[i][1];
                var marker = new google.maps.Marker({
                    position: myOutlets[i][0],
                    map: map,
                    title: top
                });
                marker.setIcon('http://chart.apis.google.com/chart?chst=d_map_pin_letter&chld= |' + my_color + '|' + my_color);


                my_markersArray.push(marker);
                google.maps.event.addListener(marker, 'click', function (event) {
                    // make a jquery call to load all transactions
                    infowindow.setContent(this.title);
                    infowindow.open(map, this);
                });
        }
        ReloadMyMarkers();
    }

    function deleteOverlays() {
        if (markersArray.length > 0) {
            if (markersArray) {
                for (i in markersArray) {
                    markersArray[0].setMap(null);
                }
                markersArray.length = 0;
            }
        }
    }

    function ReloadAllMarkers() {
        bounds = new google.maps.LatLngBounds();
        for (var i = 0; i < markersArray.length; i++) {
            markersArray[i].setMap(map);
            bounds.extend(markersArray[i].position);
        }
        map.fitBounds(bounds);
    }

    function ReloadMyMarkers() {
        for (var i = 0; i < my_markersArray.length; i++) {
            my_markersArray[i].setMap(map);
            bounds.extend(my_markersArray[i].position);
        }
        map.fitBounds(bounds);
    }

    function HideMyMarkers() {
        if (my_markersArray.length > 0) {
            for (var i = 0; i < my_markersArray.length; i++) {
                my_markersArray[i].setMap(null);
            }
        }
    }

    function LoadFilter_dev_map_outlets() {
        $('#filter-d').hide();
        $('#filter-a').empty();
        $('#filter-cb').hide();
        $('#map_outlets_options').show();
        // ==================================================================================================================
        // initialize the ui menu
        dev_map_outlets_GetDistributors();

        // create a dialog to be used for saving the divs
        var diav = $('<div />', { id: 'saveDialog' });
        var dia = $('<form />', { id: 'savedist' });
        var competior = $('<select />', { id: 'btnSaveMapDistributorName' });
        var outlet = '<input id="btnSaveMapOutletName">';
        var save_btn = $('<button />', { id: 'btnSaveMapOutlet', text: 'Save Outlet' });
        var hlat = $('<hidden />', { id: 'btnSaveMapOutletLat' });
        var hlon = $('<hidden />', { id: 'btnSaveMapOutletLon' });

        dia.append('<label for="btnSaveMapDistributorName">Competitor</label>').append(competior);
        dia.append('<label for="btnSaveMapOutletName">Outlet Name</label>').append(outlet);
        dia.append(save_btn).addClass('pure-form pure-form-stacked');
        dia.append(hlat);
        dia.append(hlon);
        diav.append(dia);
        diav.dialog({ autoOpen: false });

        $("#btnSaveMapOutlet").on("click", function () {
            dev_map_outlets_SaveOutlet();
            $('#saveDialog').dialog("close");
            event.preventDefault();
        });

    }

    // DB Data

    function dev_map_outlets_GetDistributors() {

        var sQuery = "Map_Outlets_GetDistributors";
        var sConditions = "";

        $.ajax({
            url: window.distributr_maps_baseurl + '/map/Map_Outlets',
            type: 'GET',
            data: {
                Query: sQuery,
                Conditions: sConditions
            },
            dataType: "json",
            contentType: 'application/json; charset=utf-8',
            success: function (response) {
                var json = JSON.parse(response);
                $("#map_outlets_table").find('tbody').remove();
                tbody = $('<tbody />')
                count = 1;
                $.each(json, function (i, post) {

                    // General data
                    competitors.push(post.DistributorID);

                    // Select Competitor Data
                    $("#btnSaveMapDistributorName").append("<option value='" + post.DistributorID + "'>" + post.Distributor + "</option>")


                    // List Data
                    tr0 = $('<tr />');

                    $ctrl = $('<input />', { type: 'checkbox', id: post.DistributorID, value: post.DistributorID, text: post.Distributor, checked: true }).addClass('outlet_point');
                    td0 = $('<td />');
                    td0.append($ctrl)
                    td1 = $('<td />');
                    td1.append(post.Distributor)
                    td2 = $('<td />');
                    td2.append(post.HeatScore)

                    $(tr0).append(td0);
                    $(tr0).append(td1);
                    $(tr0).append(td2);
                    tbody.append(tr0);
                    count = count + 1;
                });

                $("#map_outlets_table").append(tbody);

                $('.outlet_point').change(function () {
                    if ($(this).is(":checked")) {
                        exclude_competitors.pop($(this).val());
                    } else {
                        exclude_competitors.push($(this).val());
                    }
                    deleteOverlays();
                    LoadCompetitorMarkers();
                });

                dev_map_outlets_GetPoints();
            },
            error: function () {
                //your error code
            }
        });
    }

    function dev_map_outlets_GetPoints() {
        deleteOverlays();
        competitorLocsArray = []

        var sQuery = "Map_Outlets_GetPoints";
        var sConditions = "";

        // change the current url to make the points 
        $.ajax({
            url: window.distributr_maps_baseurl + '/map/Map_Outlets',
            type: 'GET',
            data: {
                Query: sQuery,
                Conditions: sConditions
            },
            contentType: 'application/json; charset=utf-8',
            dataType: "json",
            success: function (response) {
                var json = JSON.parse(response);
                $.each(json, function (i, post) {
                    db_marker = [];
                    var latlng = new google.maps.LatLng(post.Lat, post.Lon);
                    db_marker = [latlng, post.Outlet, post.OutletID, post.Distributor, post.DistributorID];
                    competitorLocsArray.push(db_marker);
                });

                LoadCompetitorMarkers();
            },
            error: function (response) {
                db_marker = [-1.287846, 36.819373, 'No Data Available'];
                //your error code
            }
        });
    }

    function dev_map_outlets_SaveOutlet() {
        var sQuery = "Map_Outlets_SaveOutlet";
        var sConditions = "";
        sConditions = sConditions + "|uCompetitor:" + $("#btnSaveMapDistributorName").val();
        sConditions = sConditions + "|sOutletName:" + $("#btnSaveMapOutletName").val();
        sConditions = sConditions + "|sLongitude:" + $("#btnSaveMapOutletLon").val();
        sConditions = sConditions + "|sLatitute:" + $("#btnSaveMapOutletLat").val();

        // change the current url to make the points 
        $.ajax({
            url: window.distributr_maps_baseurl + '/map/Map_Outlets',
            type: 'GET',
            data: {
                Query: sQuery,
                Conditions: sConditions
            },
            contentType: 'application/json; charset=utf-8',
            success: function (response) {
                if (response == '1') {
                    alert('Outlet Saved');
                    dev_map_outlets_GetDistributors();
                }
            },
            error: function (response) {

                //your error code
            }
        });
    }

    function dev_map_outlets_MyOutlets() {

        var sQuery = "Map_Outlets_GetMyPoints";
        var sConditions = "";
        // change the current url to make the points 
        $.ajax({
            url: window.distributr_maps_baseurl + '/map/Map_Outlets',
            type: 'GET',
            data: {
                Query: sQuery,
                Conditions: sConditions
            },
            contentType: 'application/json; charset=utf-8',
            dataType: "json",
            success: function (response) {
                var json = JSON.parse(response);
                $.each(json, function (i, post) {
                    db_marker = [];
                    var latlng = new google.maps.LatLng(post.Lat, post.Lon);
                    db_marker = [latlng, post.Outlet];
                    myOutlets.push(db_marker);
                });
                load_mine = 1;
                LoadMyMarkers();
            },
            error: function (response) {
                db_marker = [-1.287846, 36.819373, 'No Data Available'];
                //your error code
            }
        });
    }

});