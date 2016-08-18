'use strict';
capsCollectionApp.controller('capsController', function ($scope, $state, $stateParams, capsService, filter) {
    $scope.currentState = $state.current;
    $scope.currentStateParams = $stateParams;
    $scope.filter = filter;
    $scope.filter.capType = {
        crown: true,
        pull: true,
        twist: true,
        ceramic: true,
    }

    $scope.countries = [];
    $scope.beers = [];

    function loadData() {
        $scope.countriesPromise =
            capsService.getCountries().then(function successCallback(result) {
                $scope.countries = result.data;
            }, function errorCallback(result) { });

        beerSearch();
    }

    function loadBeersByCountry(countryId) {
        $scope.beersPromise =
            capsService.getBeersByCountry(countryId).then(function successCallback(result) {
                $scope.beers = result.data;
            }, function errorCallback(result) { });
    }

    function loadBeersByName(beerName) {
        $scope.beersPromise =
            capsService.getBeersByName(beerName).then(function successCallback(result) {
                $scope.beers = result.data;
            }, function errorCallback(result) { });
    }

    function beerSearch() {
        if ($scope.filter.countryId) {
            loadBeersByCountry($scope.filter.countryId);
            return;
        }
        if (!$scope.filter.countryId && $scope.filter.beerName) {
            loadBeersByName($scope.filter.beerName);
            return;
        }
    }

    $scope.search = function () {
        $state.transitionTo('collection', { countryId: $scope.filter.countryId, beerName: $scope.filter.beerName }, { notify: false });
        beerSearch();
    }

    $scope.continentGroupFunction = function (item) {
        return item.Continent.EnglishContinentName;
    }

    $scope.filterByCapType = function (beer) {
        if ($scope.filter.capType.crown && beer.CapTypeId === 1) {
            console.log(beer.CapTypeId);
            return true;
        } else if ($scope.filter.capType.twist && beer.CapTypeId === 2) {
            console.log(beer.CapTypeId);
            return true;
        } else if ($scope.filter.capType.pull && beer.CapTypeId === 3) {
            console.log(beer.CapTypeId);
            return true;
        } else if ($scope.filter.capType.ceramic && beer.CapTypeId === 4) {
            console.log(beer.CapTypeId);
            return true;
        }
        return false;
    }

    loadData();

});