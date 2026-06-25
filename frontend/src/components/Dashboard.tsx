import { useState } from "react";

type DashboardProps = {
    lat: number;
    lon: number;
    setLat: (lat: number) => void;
    setLon: (lon: number) => void;
    onSearch: (lat: number, lon: number) => void;
    token: string | null;
    userRole: string | null;
    triggerMapRefresh: () => void; 
}

type DashboardResponse = {
    totalActiveIncidents: number;
    openAreas: number;
    restrictedAreas: number;
    closedAreas: number;
    reportedIncidents: number;
    acknowledgedIncidents: number;
    inProgressIncidents: number;
    climbingAreas: ClimbingAreaResponse[];
}

type ClimbingAreaResponse = {
    id: number;
    name: string;
    description: string | null;
    accessNotes: string | null;
    status: number;
    latitude: number;
    longitude: number;
    createdAt: string;
    updatedAt: string;
    incidents: IncidentSummary[];
    routes: RouteSummary[];
}

type IncidentSummary = {
    id: number;
    title: string;
    description: string;
    incidentType: number;
    incidentStatus: number;
}

type RouteSummary = {
    id: number;
    name: string;
    grade: string;
    type: number;
    status: number;
}

type CreateIncidentForm = {
    title: string;
    description: string;
    incidentType: number;
    climbingAreaId: number;
    routeId: number | null;
}

type RegisterRequest = {
    username: string;
    password: string;
    role: number;
}

function Dashboard({ lat, lon, setLat, setLon, onSearch, token, userRole, triggerMapRefresh }: DashboardProps) {
    
    const [radiusMiles, setRadiusMiles] = useState<number>(10);
    const [dashboardData, setDashboardData] = useState<DashboardResponse | null>(null);
    const [activeTab, setActiveTab] = useState<number>(0);
    const [pendingStatuses, setPendingStatuses] = useState<Record<number, number>>({});
    const [incidentForm, setIncidentForm] = useState<CreateIncidentForm>({
        title: '',
        description: '',
        incidentType: 0,
        climbingAreaId: 0,
        routeId: null
    });
    const [registerForm, setRegistrationForm] = useState<RegisterRequest>({
        username: '',
        password: '',
        role: 0
    });
    const [expandedArea, setExpandedArea] = useState<number | null>(null);
    const [showIncidentForm, setShowIncidentForm] = useState<boolean>(false);
    

    async function fetchDashboard() {
        const response = await fetch(`http://localhost:5204/api/dashboard?lat=${lat}&lon=${lon}&radiusMiles=${radiusMiles}`, {
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`
            }
        });
        const data = await response.json();
        onSearch(lat, lon);
        setDashboardData(data);
    }

    async function updateStatus(id: number, status: number){
        await fetch(`http://localhost:5204/api/incidents/${id}`, {
            method: 'PATCH',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`
            },
            body: JSON.stringify({ status: status })
        });
        triggerMapRefresh();
    }

    async function createIncident(form: CreateIncidentForm){
        await fetch(`http://localhost:5204/api/incidents/`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`
            },
            body: JSON.stringify({
            title: form.title,
            description: form.description,
            incidentType: form.incidentType,
            climbingAreaId: form.climbingAreaId,
            routeId: form.routeId
            })
        });
        setShowIncidentForm(false);
    }

    async function registerUser(form: RegisterRequest){
        await fetch(`http://localhost:5204/api/auth/register`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`
            },
            body: JSON.stringify({
            Username: form.username,
            Password: form.password,
            Role: form.role
            })
        });
        setShowRegistrationForm(false);
    }

    const [showRegistrationForm, setShowRegistrationForm] = useState<boolean>(false);
    
    return (
    <div style={{ height: '100vh', width: '30%', color: 'black', display: 'flex', flexDirection: 'column' }}>
        <div style={{ textAlign: 'left' }}>
            Latitude: <input type="number" value={lat !== null ? lat.toFixed(4) : 39.5} onChange={(e) => setLat(Number(e.target.value))} />
            Longitude: <input type="number" value={lon !== null ? lon.toFixed(4) : 39.5} onChange={(e) => setLon(Number(e.target.value))} />
            Radius (miles): <input type="number" value={radiusMiles} onChange={(e) => setRadiusMiles(Number(e.target.value))} />
            <button onClick={fetchDashboard}>Search</button>
            <button onClick={() => setShowIncidentForm(true)}>Report Incident</button>
        </div>
        {showIncidentForm && <div style={{ textAlign: 'left' , border: '1px solid #000000', padding: '8px', marginBottom: '8px', backgroundColor: '#f5f5f5cc'}}>
            Title: <input type="text" value={incidentForm.title} onChange={(e) => setIncidentForm({ ...incidentForm, title: e.target.value })} />
            Descrption: <textarea value={incidentForm.description} onChange={(e) => setIncidentForm({ ...incidentForm, description: e.target.value })} />
            Type: <select value={incidentForm.incidentType}
                onChange={(e) => setIncidentForm({ ...incidentForm, incidentType: Number(e.target.value) })}>
                <option value={0}>Rockfall</option>
                <option value={1}>Trail Blockage</option>
                <option value={2}>Unsafe Route</option>
                <option value={3}>Weather Hazard</option>
                <option value={4}>Access Restriction</option>
                <option value={5}>Other</option>
            </select>
            <select value={incidentForm.climbingAreaId} 
                onChange={(e) => setIncidentForm({ ...incidentForm, climbingAreaId: Number(e.target.value) })}>
                    <option value={0} disabled>Select Area</option>
                    {dashboardData?.climbingAreas.map(area => (
                        <option key={area.id} value={area.id}>{area.name}</option>
                    ))}
            </select>
            <select value={incidentForm.routeId ?? ''}
                onChange={(e) => setIncidentForm({ ...incidentForm, routeId: Number(e.target.value) })}>
                    <option value={0} disabled>Select Route</option>
                    {dashboardData?.climbingAreas.find(area => area.id === incidentForm.climbingAreaId)?.routes.map(route => (
                        <option key={route.id} value={route.id}>{route.name}</option>
                ))}
            </select>
            <button onClick={() => createIncident(incidentForm)}>Submit</button>
            <button onClick={() => {setShowIncidentForm(false); setIncidentForm({ title: '', description: '', incidentType: 0, climbingAreaId: 0, routeId: null});
            }}>Cancel</button>
        </div>}
        <div style={{display: 'flex', flexDirection: 'row'}}>
            <button onClick={() => setActiveTab(0)}>Areas</button>
            {(userRole === 'Operator' || userRole === 'Admin') && <button onClick={() => setActiveTab(1)}>Incidents</button>}
        </div>
        <div style={{ textAlign: 'left' , border: '1px solid #000000', padding: '8px', marginBottom: '8px', backgroundColor: '#f5f5f5cc'}}> 
                {!dashboardData ? <div>Search to load dashboard</div> : activeTab === 0 ? 
                    dashboardData.climbingAreas.map(area => (
                        <div key={area.id} style={{cursor: 'pointer'}} onClick={() => setExpandedArea(expandedArea === area.id ? null : area.id)}
>
                            <p style={{ fontWeight: 'bold', fontSize: '22px' }}>{area.name}{' '}
                                {area.status === 0 && <span style={{color: 'green'}}>{getStatusLabel(area.status)}</span>}
                                {area.status === 1 && <span style={{color: 'goldenrod'}}>{getStatusLabel(area.status)}</span>}
                                {area.status === 2 && <span style={{color: 'red'}}>{getStatusLabel(area.status)}</span>}
                            </p>
                            {expandedArea === area.id &&  <div>{area.routes.map(route => (
                                <p>{route.name} 
                                    <div style={{ marginLeft: '16px', fontSize: '16px' }}>Type: {getRouteType(route.type)} Grade: {route.grade} Status: {getStatusLabel(route.status)}</div>
                                </p>
                            ))}
                            </div>}
                        </div>
                    ))
                : dashboardData.climbingAreas.flatMap(area => area.incidents).map(incident => (
                    <div key={incident.id}>
                        <p>{incident.title} {incident.incidentStatus === 4 && <span style={{ color: 'green' }}>Complete</span>}</p>
                        <p>Type: {incident.incidentType}</p>
                        <select 
                            value={pendingStatuses[incident.id] ?? incident.incidentStatus} 
                            onChange={(e) => setPendingStatuses({ ...pendingStatuses, [incident.id]: Number(e.target.value) })}>
                            <option value={incident.incidentStatus}>{getIncidentStatusLabel(incident.incidentStatus)}</option>
                            {incident.incidentStatus < 4 && <option value={incident.incidentStatus + 1}>{getIncidentStatusLabel(incident.incidentStatus + 1)}</option>}
                        </select>
                        <button onClick={() => updateStatus(incident.id, pendingStatuses[incident.id] ?? incident.incidentStatus)}>Update</button>
                        <p>Description: {incident.description}</p>
                    </div>
                    ))
                }
            </div>
        <div style={{marginTop: 'auto'}} >
            {((userRole === 'Admin') && !showRegistrationForm) && <button  onClick={()=> setShowRegistrationForm(true)}>Register New User</button>}
            {showRegistrationForm && <div>
                Register a New User
                Username: <input type="text" value={registerForm.username} onChange={(e) => setRegistrationForm({ ...registerForm, username: e.target.value })} />
                Password: <input type="password" value={registerForm.password} onChange={(e) => setRegistrationForm({ ...registerForm, password: e.target.value })} /> 
                <br></br>
                Role: <select value={registerForm.role}
                    onChange={(e) => setRegistrationForm({ ...registerForm, role: Number(e.target.value) })}>
                    <option value={0}>Viewer</option>
                    <option value={1}>Operator</option>
                    <option value={2}>Administrator</option>
                </select>
                <button onClick={()=> registerUser(registerForm)}>Register</button>
            </div>}
        </div>
    </div>
    );
}

function getStatusLabel(status: number): string{
    if(status == 0)
        return "Open"
    else if(status == 1)
        return "Restricted"
    else
        return "Closed" 
}

function getIncidentStatusLabel(status: number): string{
    if(status == 0)
        return "Reported";
    else if(status == 1)
        return "Acknowledged";
    else if(status == 2)
        return "InProgress";
    else if(status == 3)
        return "Resolved";
    else
        return "Closed"   
}

function getRouteType(type: number): string{
    if(type == 0)
        return "Boulder";
    else if(type == 1)
        return "Sport";
    else if(type == 2)
        return "Trad";
    else{
        return "Top Rope";
    }
}


export default Dashboard;