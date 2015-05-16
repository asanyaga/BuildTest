	$(function() {
	    $("#tablesorter-demo").tablesorter({ widgets: ['zebra'] });
		$("#options").tablesorter({sortList: [[0,0]], headers: { 3:{sorter: false}, 4:{sorter: false}}});
	});	