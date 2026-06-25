import { useState } from "react";

type LoginProps = {
    setToken: (token: string) => void;
    setUserRole: (role: string) => void;
}

function Login({setToken, setUserRole} : LoginProps){
    const[username, setUsername] = useState<string>('');
    const[password, setPassword] = useState<string>('');

    async function login(){
        const response = await fetch('http://localhost:5204/api/auth/login', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ username, password })
        });
        const data = await response.json();
        setToken(data.token);
        setUserRole(data.role);
    }

    return(
        <div style={{ textAlign: 'right' }}>
            Username: <input type="text" value={username} onChange={(e) => setUsername(e.target.value)} />
            <br></br>
            Password: <input type="password" value={password} onChange={(e) => setPassword(e.target.value)} />
            <button onClick={login}>Login</button>
        </div>
    );
}

export default Login;