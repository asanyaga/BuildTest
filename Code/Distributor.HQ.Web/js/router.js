define([
  'jquery',
  'underscore',
  'backbone'
  
   
], function ($,
    _, Backbone
   
) {
   
  
    
    var AppRouter = Backbone.Router.extend({
        routes: {
            '': 'index',
            'users': 'showUsers'
        },
    });


    var initialize = function () {
       
        var appRouter = new AppRouter;
      
      
        //appRouter.on("route:index", function () {
        //   // debugger;
        //    appRouter.getLoggedInUser();
        //    var homeViewModel = new HomeViewModel();
        //    homeViewModel.render();
        //    console.log('Home render');
        //});
       
        
        appRouter.on('defaultAction', function (actions) {
           
            // We have no matching route, lets just log what the URL was
            console.log('No route:', actions);
        });
       // appRouter.getLoggedInUser();
        Backbone.history.start();
        console.log('Router initialized');
        return appRouter;
    };
    return {
        initialize: initialize
    };
});