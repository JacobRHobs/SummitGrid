import Map from './components/Map';
import Dashboard from './components/Dashboard';
import Login from './components/Login';
import { useState } from 'react';

function App() {
    const [isSidebarOpen, setIsSidebarOpen] = useState(false);
    const [lat, setLat] = useState<number>(39.5);
    const [lon, setLon] = useState<number>(-98.35);
    const [searchLocation, setSearchLocation] = useState({ lat: 39.5, lon: -98.35 });
    const [token, setToken] = useState<string | null>(null);
    const [userRole, setUserRole] = useState<string | null>(null);
    const [refreshMap, setRefreshMap] = useState<number>(0);

    function triggerMapRefresh(){
        return setRefreshMap(refreshMap + 1);
    }

    return (
    <div style={{ display: 'flex', height: '100vh' }}>
        <Map onLocationSelect={(lat, lon) => { setLat(lat); setLon(lon); }} searchLocation={searchLocation} refreshMap={refreshMap} />
        {isSidebarOpen && <Dashboard 
            lat={lat} 
            lon={lon}
            setLat={setLat} 
            setLon={setLon}
            onSearch={(lat, lon) => setSearchLocation({ lat, lon })}
            token={token}
            userRole={userRole}
            triggerMapRefresh={triggerMapRefresh}
        />}
        <div style={{ display: 'flex', flexDirection: 'column' }}>
            {token === null ? <Login setToken={setToken} setUserRole={setUserRole} /> : <p>{userRole}</p>}
            {(userRole !== null) && <button onClick={()=> setIsSidebarOpen(!isSidebarOpen)}>
                {isSidebarOpen ? 'Close Dashboard' : 'Open Dashboard'}
            </button>}
            {userRole !== null && <button style={{position: 'fixed', bottom: '10px', right: '10px'}} onClick={() => {setToken(null); setUserRole(null); setIsSidebarOpen(!isSidebarOpen)}}>Logout</button>}
        </div>
    </div>
    );
}

export default App;
