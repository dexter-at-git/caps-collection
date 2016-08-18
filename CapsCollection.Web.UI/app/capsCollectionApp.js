'use strict';
var capsCollectionApp = angular.module('capsCollectionApp', ['ui.router', 'ui.bootstrap', 'cgBusy', 'ui.select', 'ngSanitize']);

capsCollectionApp.config(function ($stateProvider, $urlRouterProvider) {

    $urlRouterProvider.otherwise('/collection');

    $stateProvider
        .state('collection', {
            url: '/collection?{countryId:int}&beerName',
            templateUrl: 'app/views/collection.html',
            controller: 'capsController',
            resolve: {
                filter: function ($stateParams) {
                    var filter = {};
                    filter.beerName = $stateParams.beerName;
                    filter.countryId = $stateParams.countryId;
                    return filter;
                }
            }
        });
});









