var request = angular.module("request",['ngWebSocket']);


request.controller("reqctrl", function ($scope, $websocket, $interval, dataServise) {

    $scope.objects = [];
    $scope.websocketconnect = false;
    $scope.updatedTime = new Date();

    // флажек выбрать все
    $scope.allchecked = false;

    $scope.prettyDate = function (date) {
        if (date != undefined) {
            if(date>new Date(2000,1,1))
            {
                //return date.toLocaleString('ru-RU', { day: 'numeric', month: 'short', year: 'numeric', hour: 'numeric', minute: 'numeric', second: 'numeric', timeZone: 'Europe/Minsk' });
                return moment(date).format("DD MMM YYYY, HH:mm:ss");
            }
        }
        return '';
    };

    $scope.checkall = function () {
        $scope.objects.forEach(function (e, i, array) {
            $scope.objects[i].checked = !$scope.allchecked;
        });
    };

    // отправка запроса выбраным объектам
    $scope.sendRequest = function () {
        if ($scope.websocketconnect) {
            $scope.objects.forEach(function (e, i, array) {
                if (e.checked) {
                    $scope.ws.send(e.phone);
                    $scope.objects[i].checked = false;
                    // post в базу
                    dataServise.saveRequest(e.phone)
                    .success(function (data) { })
                    .error(function (error) { console.log('Ошибка при записи записи запроса в базу ',error);});
                };
            });
            alert('Запросы по отмеченым объектам поставлены в очередь отправки. Подождите некоторое время для получения данных.')
        }else
        {
            alert('Нет связи с модулем передачи сообщений! Проверьте запущена ли программа!')
        }
    };

    $scope.connectMessage = function () {
        if( $scope.websocketconnect)
        {
            return "Связь с модулем отправки сообщений установлена!";
        }else
        {
            return "Нет всвязи с модулем отправки сообщений! Проверьте запущен ли модуль передачи";
        }
    };

    $scope.init=function(wsAddr){
        
        $scope.ws=$websocket(wsAddr);

        $scope.ws.onMessage(function(event) {
            console.log('ответ по websocketу: ', event.data);
           
        });

        $scope.ws.onError(function (event) {
            $scope.websocketconnect = false;
            console.log('websocket connection Error', event);
        });

        $scope.ws.onClose(function (event) {
            $scope.websocketconnect = false;
            console.log('websocket connection closed', event);
        });

        $scope.ws.onOpen(function () {
            $scope.websocketconnect = true;
            console.log('websocket connection open');

        });

        $scope.loadData();

        $interval(function () { $scope.loadData(); }, 8000);
    };

    


    $scope.loadData=function() {
        //$scope.objects=[];
        dataServise.getobjects()
            .success(function(data){

                for (var i = 0; i < data.length; i++) {

                    // ищу в массиве элемент по id
                    var item = $scope.objects.find(function (elem,index, array) {
                        return elem.id === data[i].MarkerId;
                    });
                    
                    if (item === undefined) {
                        $scope.objects.push(dataServise.mapTo(data[i]));
                    }else
                    {
                        var indx=$scope.objects.indexOf(item);
                        
                        $scope.objects[indx].addres = data[i].Address;
                        $scope.objects[indx].phone = data[i].Phone;
                        $scope.objects[indx].ngao = data[i].Ngao;
                        $scope.objects[indx].descr = data[i].Description;
                        $scope.objects[indx].reqTime=new Date(parseInt(data[i].TimeOfRequest.substr(6)));
                        $scope.objects[indx].dataTime=new Date(parseInt(data[i].TimeOfData.substr(6)));
                        $scope.objects[indx].reqStat = parseInt(data[i].RequestStatus);
                    }
                    
                }
                $scope.updatedTime = new Date();
                
            })
            .error(function(error){console.log(error);});

    };

});



// получить данные
request.service("dataServise", function ($http) {

    this.getobjects = function () {
        return $http({
            url: './././GetObjects',
            method: 'POST',
            dataType: 'json',
            cache: false
        });
    };

    this.saveRequest = function (phone) {
        return $http({
            url: './././SendRequest',
            method: 'POST',
            dataType: 'json',
            data:{'phone':phone},
            cache: false
        });
    };



    this.mapTo = function (item) {
        return {
            checked:false,
            id:item.MarkerId,
            addres:item.Address,
            phone: item.Phone,
            ngao:item.Ngao,
            descr:item.Description,
            reqTime: new Date(parseInt(item.TimeOfRequest.substr(6))),
            dataTime: new Date(parseInt(item.TimeOfData.substr(6))),
            reqStat: parseInt(item.RequestStatus)
        };
    };

   

});
