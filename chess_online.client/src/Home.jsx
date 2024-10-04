import { Button} from "@mui/material";
import { useNavigate } from "react-router-dom";
import { useState, useEffect, useRef } from 'react';
import { DataGrid } from '@mui/x-data-grid';
import { jwtDecode } from "jwt-decode";

const Home = () => {
    const navigate = useNavigate();
    const [isWsOpen, setIsWsOpen] = useState(false);
    const wsRef = useRef(null);
    const [selectedId, setSelectedId] = useState([]);
    const token = localStorage.getItem('token');
    const decodedToken = jwtDecode(localStorage.getItem('token'))
    const [rows, setRows] = useState([]);

    useEffect(() => {
        if (!isWsOpen) {
            wsRef.current = new WebSocket(`wss://localhost:7038/api/lobby?token=${token}`);

            wsRef.current.onopen = async () => {
                console.log("Connected to WebSocket server");
                setIsWsOpen(true);
                if (wsRef.current.readyState == 1) {
                    wsRef.current.send(JSON.stringify({ Action: "getLobby", Data: "" }));
                }
            };

            wsRef.current.onmessage = (event) => {
                const newData = JSON.parse(event.data);
                console.log(newData)
                if (newData.action == 'lobbyUpdate') {
                    const lobbyArray = newData.Data != null ? Object.values(JSON.parse(newData.Data)) : [];
                    remapData(lobbyArray);
                }
                if (newData.action == 'newGameId') {
                    navigate('/Game', { state: parseInt(newData.Data) });
                }
            };
        }
    }, [location.state]);

    const columns = [
        { field: 'id', headerName: 'ID:' },
        { field: 'owner', headerName: 'Owner:' },
        { field: 'slots', headerName: 'Slots:' }
    ];

    const remapData = (lobbyData) => {
        const updatedRows = lobbyData.map((item) => ({
            id: item.Id,
            owner: item.PlayerOne,
            slots: (item.PlayerTwoId == null) ? "1/2" : "2/2"
        }));
        setRows(updatedRows);
    };

    const getNewGame = () => {
        wsRef.current.send(JSON.stringify({ Action: "newSession", Data: "" }));
    }
    const delGame = () => {
        wsRef.current.send(JSON.stringify({ Action: "delSession", Data: selectedId.toString() }));
    }
    const leaveGame = () => {
        wsRef.current.send(JSON.stringify({ Action: "leaveSession", Data: selectedId.toString() }));
    }
    const joinGame = () => {
        wsRef.current.send(JSON.stringify({ Action: "joinSession", Data: selectedId.toString() }));
    }
    const startGame = () => {
        wsRef.current.send(JSON.stringify({ Action: "startSession", Data: selectedId.toString() }));
    }

    const handleRowSelection = (newSelection) => {
        const selectedId = parseInt(newSelection[0]);
        setSelectedId(selectedId);
    };


    return (
        <div>
            <h1>Welcome to ChessWorld ONLINE! {decodedToken.sub}</h1>
            <Button
                onClick={getNewGame}
                variant="contained">
                Create Game
            </Button>
            <Button
                onClick={delGame}
                variant="contained">
                Delete Game
            </Button>
            <Button
                onClick={leaveGame}
                variant="contained">
                Leave Game
            </Button>
            <Button
                onClick={joinGame}
                variant="contained">
                Join Game
            </Button>
            <Button
                onClick={startGame}
                variant="contained">
                Start Game
            </Button>
            <DataGrid
                tablesort
                rows={rows}
                columns={columns}
                autoHeight
                onRowSelectionModelChange={(newSelection) => handleRowSelection(newSelection)}
                initialState={{
                    sorting: {
                        sortModel: [{ field: 'id', sort: 'asc' }],
                    },
                    columns: {
                        columnVisibilityModel: {
                            LP: false,
                            employeePartnerId: false
                        },
                    },
                    pagination: {
                        paginationModel: { page: 0, pageSize: 15 },
                    },
                }}
                pageSizeOptions={[5, 10, 15]}
            />
        </div>
    );
}

export default Home;
