var app = angular.module("app", []);



app.controller('MarkerController',function MarkerController($scope,$http,dataService)
{
    
    $scope.init = function () {
        $scope.loadMarkers();
    };

    $scope.markers = [];
    $scope.flagEdit = false;
    $scope.selectedMarker = null;

    $scope.loadMarkers = function () {
        dataService.getmarkers()
            .success(function (data) {
                $scope.markers = data;
            })
            .error(function (error) { console.log(error); alert('При загрузке списка объектов возникла ошибка!'); }); 
    };
 

    // метод вставки записи метки в базу 
    $scope.insertMarker = function (marker) {
        // вставка через метод сервиса
        dataService.insertmarker(marker)
        .success(function (id) {
            // в случае успеха получить объект из базы
            if (id > 0) {
                dataService.getmarkerbyid(id)
                .success(function (data) {
                   // добавляем в массив
                   $scope.markers.push(data);
               })
               .error(function (error) { console.log(error); alert('Ошибка при получении ответа от сервера!'); });
            }
            else
            {
                alert('Информация не была добавлена! Проверьте правильность заполнения данных. Либо обратитесь к администратору.');
            }
           
        })
        .error(function (error) { console.log(error); alert('Ошибка при получении ответа от сервера!'); });
    };

    $scope.updateMarker = function (marker) {
        // вставка через метод сервиса
        dataService.updatemarker(marker)
        .success(function (updated) {
            // если обновлено > 0 записей
            if (updated > 0) {
                // получить обновленный объект из базы
                dataService.getmarkerbyid(marker.MarkerId)
                .success(function (data) {
                    // обновляем маркер в массиве
                    for (var i = 0; i < $scope.markers.length; i++) {
                        if ($scope.markers[i].MarkerId == data.MarkerId)
                            $scope.markers[i] = angular.copy(data);
                    }
                })
                .error(function (error) { console.log(error); alert('Ошибка при получении ответа от сервера!'); });
            }else
            {
                alert('Информация не была обновлена! Проверьте правильность заполнения данных. Либо обратитесь к администратору.');
            }
        })
        .error(function (error) { console.log(error); alert('Ошибка при получении ответа от сервера!'); });
    };

    $scope.deleteMarker = function (MarkerId) {
        dataService.deletemarker(MarkerId)
        .success(function (deleted) {
            // обновляем маркер в массиве
            if (deleted > 0)
            {
                // обновляем маркер в массиве
                for (var i = 0; i < $scope.markers.length; i++) {
                    if ($scope.markers[i].MarkerId == MarkerId) {
                        var index = $scope.markers.indexOf($scope.markers[i]);
                        $scope.markers.splice(index, 1);
                    }
                }
                        
            } else {
                alert('Информация не была удалена! Попытайтесь ещё раз, либо обратитесь к администратору.');
            }
        })
             .error(function (error) { console.log(error); alert('Ошибка при получении ответа от сервера!'); });
    };

		

	$scope.initMarker=function(){
	    $scope.editMarker={
	        MarkerId:0,
	        Address: '',
	        MarkerType:0,
	        Phone:'',
	        Px:0,
	        Py: 0,
	        Description:''
		 };
		};

	$scope.add=function(){
			$scope.initMarker();
			$scope.selectedMarker = null;
			$scope.flagEdit = true;
			$scope.searchCriteria = null;
		};

	$scope.edit=function(){
			$scope.editMarker=angular.copy($scope.selectedMarker);
			$scope.selectedMarker=null;
			$scope.flagEdit = true;
		};

	$scope.details=function(){

			if($scope.selectedMarker!=null) {

			    window.location.href = '././Itp/Index?id=' + $scope.selectedMarker.MarkerId;

			}
			
		};

	$scope.cancelAdd=function(){
			$scope.flagEdit=false;
			$scope.initMarker();
			$scope.selectedMarker=null;
		};


	$scope.reverseSort = false;
	$scope.sortField = "Address";

	$scope.headerSortClick=function(header) {
	    $scope.reverseSort=!$scope.reverseSort;
	    $scope.sortField=header;
	}

		// функция обработки нажатия на маркер
	$scope.select=function(marker,event){
			if($scope.flagEdit==false)
				{
					$scope.selectedMarker=marker;
					$scope.searchCriteria=null;
				}

			// прекращаем всплытие события к родителю
			if (event.stopPropagation) event.stopPropagation();
		    if (event.preventDefault) event.preventDefault();
		    event.cancelBubble = true;
		    event.returnValue = false;

		};



    $scope.deleteMark = function(marker) {
        if (confirm('Вы подтверждаете удаление информации?')) {
            $scope.deleteMarker(marker.MarkerId);
            $scope.selectedMarker = null;
        }
        ;
    };

		// логика добавления - редактирования
	$scope.submitEditForm=function(){
			if ($scope.editform.$valid) {

				if($scope.editMarker.MarkerId==0) // вставка
				{
				    $scope.insertMarker($scope.editMarker);
				}
                else  // обновление
				{
				    $scope.updateMarker($scope.editMarker);
                    
                }
                
                $scope.flagEdit=false;
			    $scope.initMarker();
			    $scope.selectedMarker=null;

            }else
            {
            	alert('Не все данные указаны верно!');
            }
		};

    $scope.search = function(term) {
        var array = [];
        var pattern = new RegExp($.trim(term), 'i');
        angular.forEach($scope.markers, function(value, key) {
            if (pattern.test(value.Address))
                this.push(value);
        }, array);
        return array.sort(function (a, b) {
            if (a.Address > b.Address) {
                return 1;
            } else if (a.Address < b.Address) {
                return -1
            } else {
                return 0;
            }
        });
    };

});