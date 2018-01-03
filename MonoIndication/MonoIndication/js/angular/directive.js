
app.directive('autoComplete', function ($timeout) {
    return function (scope, iElement, iAttrs) {
        iElement.autocomplete({
            source: function (request, response) {
                response($.map(scope.search(request.term), function (item) {
                    return { label: item.Address, value: item };
                }));
            },
            minLength: 2,

            select: function (event, selectedItem) {
                // Do something with the selected item, e.g. 
                var marker = selectedItem.item.value;
                scope.searchCriteria = marker.Address;
                scope.$apply();

                if (scope.flagEdit == false) {
                    scope.selectedMarker = marker;
                    scope.$apply();
                    var pos = $("#mapcontainer table tr[id='tr" + marker.MarkerId + "']").position().top;
                    var scrolled = $('#mapcontainer').scrollTop();
                    $('#mapcontainer').scrollTop(scrolled+pos);
                }

                

                if (event.preventDefault) event.preventDefault();
                event.returnValue = false;

                //$scope.select(marker,event);

            },
            
            focus: function(event, ui) {
                if (event.preventDefault) event.preventDefault();
                event.returnValue = false;
            }
            
        });
    };
});