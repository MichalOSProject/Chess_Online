import { Route, Routes } from 'react-router-dom';
import Home from './Home.jsx';
import Game from './Game.jsx';
import NotFound from './notFound.jsx'
import Register from './components/Logon/register.jsx'
import Login from './components/Logon/login.jsx'

function App() {

    return (
        <div>
            <Routes>
                <Route path="/" element={<Home />} />
                <Route path="/Game" element={<Game />} />
                <Route path="/register" element={<Register />} />
                <Route path="/Login" element={<Login />} />
                <Route path="*" element={<NotFound />} />
            </Routes>
        </div>
    );
}

export default App;