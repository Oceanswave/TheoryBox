angular.module('ngTheoryBox', ['ngSanitize', 'ngRoute', 'ui.bootstrap', 'ui.router'])
    .config([
        '$urlRouterProvider', '$stateProvider', function ($urlRouterProvider, $stateProvider) {

            $urlRouterProvider.otherwise("/");

            $stateProvider
                .state('Home', {
                    url: "/",
                    views: {
                        "navbar": {
                            templateUrl: "/NavBar"
                        },
                        "pageContent": {
                            templateUrl: "/Home",
                            controller: "HomeCtrl"
                        }
                    }
                })
                .state('CardsByColor', {
                    url: "/cards-by-color/{color}",
                    views: {
                        "navbar": {
                            templateUrl: "/NavBar"
                        },
                        "pageContent": {
                            templateUrl: "/CardsByColor",
                            controller: "CardsByColorCtrl"
                        }
                    }
                })
                .state('CardDetails', {
                    url: "/Details/{metaverseId}",
                    views: {
                        "navbar": {
                            templateUrl: "/NavBar"
                        },
                        "pageContent": {
                            templateUrl: "/CardDetails",
                            controller: "CardDetailsCtrl"
                        }
                    }
                })
                .state('NotFound', {
                    url: "/NotFound",
                    views: {
                        "navbar": {
                            templateUrl: "/StandardNavBar"
                        },
                        "pageContent": {
                            templateUrl: "/NotFound"
                        }
                    }
                });
        }
    ]);