var CommodityReleaseApp = angular.module("CommodityReleaseApp", ['ngRoute', 'ngResource']);
console.log("==> Application started...");

CommodityReleaseApp.config(function($routeProvider){
    console.log("==> Configuring routes...");

    $routeProvider.when('/', {
        controller: "releaseCtrl",
        templateUrl: 'release.html'
    });
    console.log("==> Finished configuring routes...")
});

CommodityReleaseApp.controller("releaseCtrl",function($scope, releaseService){
    console.log("==> releaseCtrl initialized");
    $scope.message = "Commodity Release";

    $scope.store = 0;
    $scope.commodity = 0;
    $scope.grade = 0;
    $scope.lineItems = null;
    $scope.lineItemsStatus = false;

    releaseService.getStores;
    $scope.stores = releaseService.Stores;

    $scope.storeChanged = function(){
        $scope.commodity = 0;
        releaseService.getCommodities($scope.store.Id);
        $scope.commodities = releaseService.Commodities;
    };

    $scope.commodityChanged = function(){
        $scope.grade = 0;
        releaseService.getGrades($scope.commodity.Id,$scope.store.Id);
        $scope.grades = releaseService.Grades;
    };


    $scope.gradeChanged = function(){
        $scope.lineItems = null;
        releaseService.getLineItems($scope.commodity.Id,$scope.store.Id, $scope.grade.Id);
        $scope.lineItems = releaseService.LineItems;
        $scope.lineItemsStatus = true;
    };

    $scope.release = function(){
        var transferList = [];
        angular.forEach($scope.lineItems, function(lineItem){
            if(lineItem.IsSelected){
              transferList.push({ItemId : lineItem.Id, StoreId : $scope.store.Id, CommodityId : lineItem.Commodity.Id, GradeId : lineItem.Grade.Id, BatchNo : lineItem.BatchNo, Weight : lineItem.Weight});
            };
        });
        transferList["StoreID"] =  $scope.store.Id;
        console.log(transferList);
        releaseService.release(transferList);
    };

});

CommodityReleaseApp.factory("releaseService", function($http){
    var _stores = [];

    var _getStores = function(){
        var url = '/api/release/storelist';
        $http.get(url)
            .then(function(response){
                angular.copy(response.data, _stores);
            },function(response){
                console.log("Error", + response.data, + response.status);
            })
    };

    var _commodities = [];

    var _getCommodities = function(storeID){
        if(angular.isUndefined(storeID)) return;
        var url = '/api/release/commoditylist?' + storeID;
        $http.get(url)
            .then(function(response){
                angular.copy(response.data, _commodities);
            },function(response){
                console.log("Error", + response.data, + response.status);
            })
    };

    var _grades = [];

    var _getGrades = function(commodityID,storeID){
        if(angular.isUndefined(commodityID) || angular.isUndefined(storeID)) return;
        var url = '/api/release/gradelist?commodityId=' + commodityID + '&storeId='+storeID;
        $http.get(url)
            .then(function(response){
                angular.copy(response.data, _grades);
            },function(response){
                console.log("Error", + response.data, + response.status);
            })
    };

    var _lineItems = [];

    var _getLineItems = function(commodityID, storeID, gradeID){
        if(angular.isUndefined(commodityID) || angular.isUndefined(storeID) || angular.isUndefined(gradeID)) return;
        var url = '/api/release/lineItemList?commodityId=' + commodityID + '&storeId='+storeID + '&gradeId='+gradeID;
        $http.get(url)
            .then(function(response){
                angular.copy(response.data, _lineItems);
            },function(response){
                console.log("Error", + response.data, + response.status);
            })
    };

    var _release = function(lineItems){
        if(angular.isUndefined(lineItems)) return;
        var url = '/api/release/transfer';
        $http.post(url,lineItems)
            .then(function(response){
                console.log(response.data);
            },function(response){
                console.log("Error", + response.data, + response.status);
            })
    };

    return{
        Stores : _stores,
        getStores : _getStores(),
        Commodities : _commodities,
        getCommodities : _getCommodities,
        Grades : _grades,
        getGrades : _getGrades,
        LineItems : _lineItems,
        getLineItems : _getLineItems,
        release : _release
    };
});