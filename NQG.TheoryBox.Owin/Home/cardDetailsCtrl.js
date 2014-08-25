angular.module('ngTheoryBox')
.controller('CardDetailsCtrl', ['$scope', '$stateParams', '$http', function ($scope, $stateParams, $http) {

    $http({
        method: "GET",
        url: "/API/metaverseid/" + $stateParams.metaverseId
    }).success(function(data) {
        $scope.card = data;
    });
}]);