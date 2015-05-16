require.config({
    //baseUrl: '<%= @Url.Content("~")  %>',
    urlArgs: "bust=v2",
    shim: {
        'underscore': {
            exports: '_'
        },
        'backbone': {
            deps: ['underscore', 'jquery'],
            exports: 'Backbone'
        },
        'backbone-pageable': {
            deps: ['underscore', 'backbone'],
            exports: 'Backbone.PageableCollection'
        },
        'bootstrap': {
            deps: ['jquery'],
            exports: 'bootstrap'
        },
        'jRating': {
            deps: ['jquery'],
            exports: 'jRating'
        },
        "backgrid": {
            deps: ['jquery', 'backbone', 'underscore'],
            exports: "Backgrid"
        },
        "backgrid-paginator": {
            deps: ['backgrid', 'backbone-pageable']
        },
        
        "backbone-stickit": {
            deps: ['backbone']
        },
        'maps': { exports: "Microsoft.Maps" },
        "backgrid-filter": {
            deps: ['backgrid', "lunr"]
        },
        "magnific-popup": {
            deps: ['jquery']
        },
        "toastr": {
            deps: ['jquery']
        }
       
       
        
        
    },
    paths: {
        jquery: 'libs/jquery/jquery-1.9.1',
        underscore: 'libs/underscore/underscore',
        backbone: 'libs/backbone/backbone',
        bootstrap: 'libs/bootstrap/bootstrap.min',
        backgrid: 'libs/backgrid/backgrid',
        accounting: 'libs/util/accounting.min',
        'magnific-popup': 'libs/jquery/jquery.magnific-popup.min',
        'backbone-pageable': 'libs/backbone/backbone-pageable',
        'backgrid-paginator': 'libs/backgrid/backgrid-paginator',
        'backgrid-filter': 'libs/backgrid/backgrid-filter',
        'lunr': 'libs/lunr/lunr',
        'backbone-stickit': 'libs/backbone/backbone.stickit',
        'md5': 'libs/util/md5',
        'moment': 'libs/moment/moment.min',
        'toastr': 'libs/toastr/toastr.min',
        //jqueryui: 'libs/jquery/jquery-ui-1.8.24',
        jqueryui: 'libs/jquery/jquery-ui-1.10.3.custom.min',
        
        text: 'libs/text/text',
        app: 'app',
        apputil: 'apputil',
        router: 'router',
       
        addupdatepurchaseorderviewmodel: 'views/addupdatepurchaseorder',
        productpopupviewmodel: 'views/productpopup',
     
    }

});

require(['app'], function (App) {
    // The "app" dependency is passed in as "App"
    App.initialize();
    //var Common = App;
    console.log("Application started.....");
    window.pageload();
});
