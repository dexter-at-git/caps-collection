'use strict';
capsCollectionApp.factory('capsService', function ($http) {

    var factory = {};

    var geoUrl = "http://capsservice.cloudapp.net/Services/GeographyService.svc/rest";
    var beerUrl = "http://capsservice.cloudapp.net/Services/BeerService.svc/rest";

    factory.getBeersByCountry = function (countryId) {
        return $http({
            method: 'GET',
            url: beerUrl + '/GetCountryBeers?countryId=' + countryId
        });
    }

    factory.getBeersByName = function (beerName) {
        return $http({
            method: 'GET',
            url: beerUrl + '/GetBeerByName?beerName=' + beerName
        });
    }

    factory.getAllBeers = function () {
        return $http({
            method: 'GET',
            url: beerUrl + '/GetAllBeers'
        });
    }

    factory.getCountries = function () {
        return $http({
            method: 'GET',
            url: geoUrl + '/GetBeerCountries'
        });
    }

    return factory;
});