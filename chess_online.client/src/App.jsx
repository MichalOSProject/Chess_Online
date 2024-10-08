import { Route, Routes } from 'react-router-dom';
import Lobby from './Lobby.jsx';
import Game from './Game.jsx';
import Profile from './Profile.jsx'
import NotFound from './notFound.jsx'
import Register from './components/Logon/register.jsx'
import Login from './components/Logon/login.jsx'
import SiteProtect from './components/Security/SiteProtect.jsx'

function App() {

    return (
        <div>
            <Routes>
                <Route path="*" element={<NotFound />} />
                <Route path="/register" element={<Register />} />
                <Route path="/Login" element={<Login />} />
                <Route element={<SiteProtect />}>
                    <Route path="/" element={<Lobby />} />
                    <Route path="/Game" element={<Game />} />
                    <Route path="/Profile" element={<Profile />} />
                </Route>
            </Routes>
        </div>
    );
}

export default App;