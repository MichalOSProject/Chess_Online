import { Button, TextField, Switch } from "@mui/material";
import { useNavigate } from "react-router-dom";
import { useState, useEffect, useRef } from 'react';
import { DataGrid } from '@mui/x-data-grid';
import { jwtDecode } from "jwt-decode";

const Lobby = () => {
    const navigate = useNavigate();
    const [isWsOpen, setIsWsOpen] = useState(false);
    const wsRef = useRef(null);
    const selectedId = useRef(0);
    const token = localStorage.getItem('token');
    const decodedToken = jwtDecode(localStorage.getItem('token'))
    const [rows, setRows] = useState([]);
    const [gamesList, setGamesList] = useState([]);
    const [selectedGame, setSelectedGame] = useState();
    const [isTeamSwitch, setIsTeamSwitch] = useState(false);
    const [myOwnGame, setMyOwnGame] = useState(0);

    useEffect(() => {
        if (!isWsOpen) {
            wsRef.current = new WebSocket(`wss://localhost:7038/api/lobby?token=${token}`);

            wsRef.current.onopen = async () => {
                setIsWsOpen(true);
                if (wsRef.current.readyState == 1) {
                    wsRef.current.send(JSON.stringify({ Action: "getLobby", Data: "" }));
                }
            };

            wsRef.current.onmessage = (event) => {
                const newData = JSON.parse(event.data);
                if (newData.action == 'lobbyUpdate') {
                    const lobbyArray = newData.Data != null ? Object.values(JSON.parse(newData.Data)) : [];
                    remapData(lobbyArray);
                    setGamesList(lobbyArray)
                    if (selectedId.current != 0) {
                        const tempGame = lobbyArray.find(x => x.Id == selectedId.current)
                        if (tempGame) {
                            setIsTeamSwitch(tempGame.SwitchedTeam)
                            setSelectedGame(tempGame)
                        }
                    }
                }
                if (newData.action == 'newGameId') {
                    navigate('/Game', { state: parseInt(newData.Data) });
                }
                if (newData.action == 'error') {
                    alert(newData.Data)
                }
            };
        }
    }, [location.state]);

    const columns = [
        { field: 'id', headerName: 'ID:', flex: 1 },
        { field: 'owner', headerName: 'Owner:', flex: 6 },
        { field: 'slots', headerName: 'Slots:', flex: 2 }
    ];

    const remapData = (lobbyData) => {
        const updatedRows = lobbyData.map((item) => ({
            id: item.Id,
            owner: item.Owner,
            slots: (item.PlayerOne != null && item.PlayerTwo != null) ? "2/2" : "1/2"
        }));
        if (myOwnGame == 0) {
            const temp = lobbyData.find(x => x.Owner == decodedToken.sub)
            if (temp) {
                setMyOwnGame(temp.Id)
                selectedId.current = temp.Id
                setSelectedGame(temp)
            }
        }
        setRows(updatedRows);
    };

    const getNewGame = () => {
        wsRef.current.send(JSON.stringify({ Action: "newSession", Data: "" }));
    }
    const delGame = () => {
        wsRef.current.send(JSON.stringify({ Action: "delSession", Data: "" }));
        setMyOwnGame(0)
    }
    const changeTeam = () => {
        if (myOwnGame == selectedId.current)
            wsRef.current.send(JSON.stringify({ Action: "switchTeam", Data: "" }));
    }
    const leaveGame = () => {
        wsRef.current.send(JSON.stringify({ Action: "leaveSession", Data: "" }));
    }
    const joinGame = () => {
        wsRef.current.send(JSON.stringify({ Action: "joinSession", Data: selectedId.current.toString() }));
    }
    const startGame = () => {
        wsRef.current.send(JSON.stringify({ Action: "startSession", Data: "" }));
    }
    const goToStats = () => {
        navigate('/profile',);
    }
    const logout = () => {
        localStorage.removeItem('token');
        navigate('/login',);
    }

    const handleRowSelection = (newSelection) => {
        selectedId.current = parseInt(newSelection[0])
        const tempGame = gamesList.find(x => x.Id == selectedId.current)
        if (tempGame) {
            setSelectedGame(tempGame)
            setIsTeamSwitch(tempGame.SwitchedTeam)
        }
    };

    return (
        <div className="body">
            <header className="header">
                <h1>Welcome to ChessWorld ONLINE! {decodedToken.sub}</h1>
                <Button
                    className="default_button"
                    onClick={goToStats}
                    variant="contained">
                    Profile
                </Button>
                <Button
                    className="default_button"
                    onClick={logout}
                    variant="contained">
                    Logout
                </Button>
            </header>
            <div className="lobby">
                <div className="buttonPanel">
                    <Button
                        className="default_button"
                        onClick={getNewGame}
                        variant="contained">
                        Create Game
                    </Button>
                    <Button
                        className="default_button"
                        onClick={delGame}
                        variant="contained">
                        Delete Game
                    </Button>
                    <Button
                        className="default_button"
                        onClick={leaveGame}
                        variant="contained">
                        Leave Game
                    </Button>
                    <Button
                        className="default_button"
                        onClick={joinGame}
                        variant="contained">
                        Join Game
                    </Button>
                    <Button
                        className="default_button"
                        onClick={startGame}
                        variant="contained">
                        Start Game
                    </Button>
                </div>
                <div className="lobbyView">
                    <div className="lobbyLeft">
                        <DataGrid
                            tablesort
                            rows={rows}
                            columns={columns}
                            autoHeight={false}
                            onRowSelectionModelChange={(newSelection) => handleRowSelection(newSelection)}
                            initialState={{
                                sorting: {
                                    sortModel: [{ field: 'id', sort: 'asc' }],
                                },
                                columns: {
                                    columnVisibilityModel: {
                                        LP: false,
                                    },
                                }
                            }}
                            pageSizeOptions={[5, 10, 15]}
                        />
                    </div>
                    <div className="lobbyRight">
                        <TextField
                            className="default_textfield"
                            label="Game ID"
                            value={selectedGame != null ? selectedGame.Id != null ? selectedGame.Id : "" : ""}
                            InputProps={{
                                readOnly: true,
                            }}
                            InputLabelProps={{
                                shrink: true,
                            }}
                        />
                        <TextField
                            className="default_textfield"
                            label="Game's Owner"
                            value={selectedGame != null ? selectedGame.Owner != null ? selectedGame.Owner : "" : ""}
                            InputProps={{
                                readOnly: true,
                            }}
                            InputLabelProps={{
                                shrink: true,
                            }}
                        />
                        <TextField
                            className="default_textfield"
                            label="Team White"
                            value={selectedGame != null ? selectedGame.PlayerOne != null ? selectedGame.PlayerOne : "" : ""}
                            InputProps={{
                                readOnly: true,
                            }}
                            InputLabelProps={{
                                shrink: true,
                            }}
                        />
                        <TextField
                            className="default_textfield"
                            label="Team Black"
                            value={selectedGame != null ? selectedGame.PlayerTwo != null ? selectedGame.PlayerTwo : "" : ""}
                            InputProps={{
                                readOnly: true,
                            }}
                            InputLabelProps={{
                                shrink: true,
                            }}
                        />Change teams
                        <Switch
                            checked={isTeamSwitch}
                            onClick={changeTeam}
                        />
                    </div>
                </div>
            </div>
        </div>
    );
}

export default Lobby;
