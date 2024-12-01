<script context="module">
  import CryptoJS from 'crypto-js';

    export async function hashStringSHA256(input) {
        return CryptoJS.SHA256(input).toString(CryptoJS.enc.Hex);
        const encoder = new TextEncoder();
        const data = encoder.encode(input);
        const hashBuffer = await crypto.subtle.digest('SHA-256', data);
        const hashArray = Array.from(new Uint8Array(hashBuffer));
        const hashHex = hashArray.map(b => b.toString(16).padStart(2, '0')).join('');
        return hashHex;
    }
    
    export function saveSession(sessionId, localStorage) {
        localStorage.session = sessionId;
    }
    
    export function dateDiff(date1, date2) {
        const diffMs =  date1.getTime() - date2.getTime();
        const seconds = Math.floor(diffMs / 1000);
        const minutes = Math.floor((seconds - seconds % 60  ) / 60);


        return `${minutes}:${(seconds % 60).toString().padStart(2, '0')}`;
    }
    
    export function getRandomString() {
        const minLength = 4;
        const maxLength = 6;
        const characters = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
        let result = '';
        const charactersLength = characters.length;
    
        // Generate a random length between minLength and maxLength (inclusive)
        const length = Math.floor(Math.random() * (maxLength - minLength + 1)) + minLength;
    
        for (let i = 0; i < length; i++) {
            result += characters.charAt(Math.floor(Math.random() * charactersLength));
        }
    
        return result;
    }

    export function flyToGeofence(map, geofence) {
        let topLeft = [180, 90];
        let bottomRight = [-180, -90];
        for(const p of geofence.Points) {
            if(p.Latitude < topLeft[1]) {
                topLeft[1] = p.Latitude;
            }
            if(p.Latitude > bottomRight[1]) {
                bottomRight[1] = p.Latitude;
            }
            if(p.Longitude < topLeft[0]) {
                topLeft[0] = p.Longitude;
            }
            if(p.Longitude > bottomRight[0]) {
                bottomRight[0] = p.Longitude;
            }
        }
        map.fitBounds([topLeft, bottomRight], {
            padding: 100
        });
    }

    export function toPoint(coords) {
        return {Latitude: coords[0], Longitude: coords[1]};
    }

    export function toCoords(point) {
        return [point.Latitude,point.Longitude];
    }

    export function getFullBackendUrl(path, sessionStorage) {
        if(path.startsWith("/")) {
            path = path.substring(1);
        }
        if(sessionStorage.getItem("urlPrefix") != null) {
            path = sessionStorage.urlPrefix + path;
        } else {
            path = "/" + path;
        }
        return path;
    }
    
    export function fetchJson(path, params, localStorage) {
        return authFetch(path, params, localStorage, true)
    }

    function authFetch(path, params, localStorage, doJson= false) {
        //console.log(params)
        if(!params) {
            params = {};
        }
        if(!params["headers"]) {
            params["headers"] = {};
        }
        params["credentials"] = "include";
        if(localStorage) {
            if(localStorage.session) {
                console.log("session cookie is " + localStorage.getItem("session"))
                params["headers"]["Authorization"] = `Bearer ${localStorage.session}`;
            }
        } else {
            console.log("Local storage not passed! Is this intentional?")
        }
        if(path.startsWith("/")) {
            path = path.substring(1);
        }
        if(sessionStorage.getItem("urlPrefix") != null) {
            path = sessionStorage.urlPrefix + path;
        } else {
            return new Promise((resolve, reject) => {
                fetch("/api/v1/alive").then((res) => {
                    if(res.ok) {
                        sessionStorage.urlPrefix = "/";
                    } else {
                        sessionStorage.urlPrefix = "http://" + location.host.split(':')[0] + ":8383/";
                    }
                    return fetchJson(path, params, localStorage).then(resolve).catch(reject);
                });
            });
            
        }
        return fetch(path, params).then(response => {
            if(!response.ok) {
                return response.json().then((json) => {
                    json.status = response.status;
                    json.ok = response.ok;
                    return Promise.resolve(json)
                });
            }
            return response.json().then((json) => {
                json.status = response.status;
                json.ok = response.ok;
                return Promise.resolve(json)
            });
        }).catch((error) => {
            console.error("Error fetching " + path + ": " + error);
            if(params["alert"] == undefined || params["alert"]) alert("There's been an error during a web request. Please try again later.");
            throw error;
        });
    }
</script>