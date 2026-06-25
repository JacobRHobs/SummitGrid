import { useEffect, useRef } from 'react';
import L from 'leaflet';
import 'leaflet/dist/leaflet.css';

type MapProps = {
    onLocationSelect: (lat: number, lon: number) => void;
    searchLocation: { lat:number, lon:number };
    refreshMap: number; 
}

function Map({ onLocationSelect, searchLocation, refreshMap }: MapProps) {
    
    const mapRef = useRef<HTMLDivElement>(null);
    const mapInstanceRef = useRef<L.Map | null>(null);
    const markersRef = useRef<L.CircleMarker[]>([]);

    useEffect(() => {
        if (!mapRef.current) return;

        const map = L.map(mapRef.current, { minZoom: 3.26 }).setView([37.8, -83.6], 8);

        L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
            attribution: '© OpenStreetMap contributors'
        }).addTo(map);

        mapInstanceRef.current = map;

        map.on('click', (e) => {
            onLocationSelect(e.latlng.lat, e.latlng.lng);
        });

        return () => {
            map.remove();
        };
    }, []);

    useEffect(() => {
        if (mapInstanceRef.current) {
            mapInstanceRef.current.setView([searchLocation.lat, searchLocation.lon]);
        }
    }, [searchLocation]);

    useEffect(() => {
        markersRef.current.forEach(marker => marker.remove());
        markersRef.current = [];
        fetch('http://localhost:5204/api/climbingareas')
            .then(response => response.json())
            .then(data => {
                if(mapInstanceRef.current != null){
                    data.forEach((area: any) => {
                        const color = getMarkerColor(area.status);
                        const marker = L.circleMarker([area.latitude, area.longitude], {
                            color: 'black',
                            weight: 2,
                            fillColor: color,
                            fillOpacity: 0.8,
                            radius: 8
                        }).bindPopup(`<b><span style="font-size: 16px">${area.name}</span></b><br>
                            Description: ${area.description ?? 'No description'}<br>
                            Access Notes: ${area.accessNotes}<br>
                            Status: ${getStatusLabel(area.status)}<br>
                            Location: ${area.latitude}, ${area.longitude}<br>
                            Active Incidents(${area.incidents.filter(isIncidentActive).length}): ${area.incidents.filter(isIncidentActive).map((i: any) => `${i.title}`).join(', ')}`)
                        .addTo(mapInstanceRef.current!);
                        markersRef.current.push(marker);
                    });
                }
            });
    }, [refreshMap]);

    return <div ref={mapRef} style={{ height: '100vh', width: '100%' }} />;
}

function getMarkerColor(status: number): string {
    if(status == 0)
        return "green"
    else if(status == 1)
        return "yellow"
    else
        return "red"
}

function getStatusLabel(status: number): string{
    if(status == 0)
        return "Open"
    else if(status == 1)
        return "Restricted"
    else
        return "Closed" 
}

function isIncidentActive(status: number): boolean{
    if(status <= 2)
        return true;

    return false;
}



export default Map;

