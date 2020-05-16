
    var map;
    var marker;
    var userLocus = {};

    function fillPosition(marker) {

        var currentLocation = marker.getPosition();
        console.log(currentLocation);
        //fill hidden fields with marker location data
        document.getElementById('latitude').value = currentLocation.lat();
        document.getElementById('longitude').value = currentLocation.lng();

    }

    function setPosition(position) {
        userLocus = { lat: parseFloat(position.coords.latitude.toFixed(6)), lng: parseFloat(position.coords.longitude.toFixed(6)) };
        initMap();
        fillPosition(marker);
    }

    function errPosition() {
        //error callback, default to University of Malta campus
        userLocus = { lat: 35.902315, lng: 14.484792 };
        console.log("Geolocation failed.");
    }

    function initMap() {
        //Map options.
        var options = {
            center: new google.maps.LatLng(userLocus),
            zoom: 18,
            zoomControl: true,
            disableDefaultUI: true,
            mapTypeControl: true,
            streetViewControl: false,
            mapTypeControlOptions: {
                style: google.maps.MapTypeControlStyle.DROPDOWN_MENU,
            }
        };

        //Construct Map
        map = new google.maps.Map(document.getElementById('map'), options);
        //Create default marker
        marker = new google.maps.Marker({
            position: userLocus,
            map: map,
            draggable: false,
        });
        console.log("marker created");

        google.maps.event.addListener(map, 'click', function (event) {
            //Location on the map where the user clicked
            var clickedLocus = event.latLng;
            console.log(marker)
            if (!marker) {
                //marker not yet constructed
                marker = new google.maps.Marker({
                    position: clickedLocus,
                    map: map,
                    draggable: false,
                });

                console.log("marker created from click event");

                

            } else {
                

                //change location of marker to newly clicked location
               
                marker.setPosition(clickedLocus);
                console.log("marker changed");
            }

            fillPosition(marker);
            

        });
    }
    
    function locusMap() {
        navigator.geolocation.getCurrentPosition(setPosition, errPosition, {
            enableHighAccuracy: true,
            timeout: 5000,
            maximumAge: 0
        });
    }

    function editMap() {
        //get location data from hidden fields
        var dbLat = document.getElementById("latitude").value;
        var dbLng = document.getElementById("longitude").value;

        userLocus = { lat: parseFloat(dbLat), lng: parseFloat(dbLng) };
        initMap();
        fillPosition(marker);
       
    }

    //show map to details page
    function displayMap() {
        //get location data from hidden fields
        var dbLat = document.getElementById("latitude").value;
        var dbLng = document.getElementById("longitude").value;

        userLocus = { lat: parseFloat(dbLat), lng: parseFloat(dbLng) };

        //Map options.
        var options = {
            center: new google.maps.LatLng(userLocus),
            zoom: 18,
            zoomControl: true,
            disableDefaultUI: true,
            mapTypeControl: true,
            streetViewControl: false,
            mapTypeControlOptions: {
                style: google.maps.MapTypeControlStyle.DROPDOWN_MENU,
            }
        };

        //Construct Map
        map = new google.maps.Map(document.getElementById('map'), options);
        //Create default marker
        marker = new google.maps.Marker({
            position: userLocus,
            map: map,
            draggable: false,
        });
    }
    
 




