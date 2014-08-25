angular.module('ngTheoryBox')
.controller('CardsByColorCtrl', ['$scope', '$stateParams', '$http', function ($scope, $stateParams, $http) {
    $http({
        method: "GET",
        url: "/API/metaverseid/4635"
    }).success(function(data) {
        $scope.card = data;
    });
}]);