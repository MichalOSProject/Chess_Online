import { Button, TextField} from "@mui/material";
import { useState, useEffect } from 'react';
import { jwtDecode } from "jwt-decode";

const Statistics = () => {
    const decodedToken = jwtDecode(localStorage.getItem('token'))
    const [playerUsername, setPlayerUsername] = useState("");
    const [playerStats, setPlayerStats] = useState("");

    useEffect(() => {
        setPlayerUsername(decodedToken.sub)
        fetch(`https://localhost:7038/api/Player/PlayerStats?username=${decodedToken.sub}`, {
            method: 'GET',
            mode: 'cors',
            headers: {
                'Content-Type': 'application/json'
            },
        }).then(response => {
            if (!response.ok) {
                return response.text().then(errorData => {
                    throw new Error(errorData);
                });
            }
            return response.json();
        }).then(data => {
            setPlayerStats(data)
        }).catch(error => {
            console.log(error.message)
        });
    }, []);

    const handleUsernameChange = (event) => {
        setPlayerUsername(event.target.value);
    };

    const findPlayer = () => {
        fetch(`https://localhost:7038/api/Player/PlayerStats?username=${playerUsername}`, {
            method: 'GET',
            mode: 'cors',
            headers: {
                'Content-Type': 'application/json'
            },
        }).then(response => {
            if (!response.ok) {
                return response.text().then(errorData => {
                    throw new Error(errorData);
                });
            }
            return response.json();
        }).then(data => {
            setPlayerStats(data)
        }).catch(error => {
            alert(error.message)
        });
    }

    return (
        <div>
            <h1>{decodedToken.sub}, Check your account Statistics!</h1>
            <h2>or yours friends!</h2>
                <TextField
                    required
                    name="name"
                    label="Username"
                    value={playerUsername}
                    onChange={handleUsernameChange}
                />
                <Button
                    onClick={findPlayer}
                    variant="contained">
                    Find Player
            </Button>
            <br />
            <br />
                <TextField
                    label="Username"
                value={playerStats.username || ""}
                    InputProps={{
                        readOnly: true,
                    }}
                    InputLabelProps={{
                        shrink: true,
                    }}
                />
                <TextField
                    label="All Games"
                value={playerStats.totalGames || 0}
                    InputProps={{
                        readOnly: true,
                    }}
                    InputLabelProps={{
                        shrink: true,
                    }}
                />
                <TextField
                    label="Ended Games"
                value={playerStats.endedGames || 0}
                    InputProps={{
                        readOnly: true,
                    }}
                    InputLabelProps={{
                        shrink: true,
                    }}
                />
                <TextField
                    label="Won Games"
                value={playerStats.winnings || 0}
                    InputProps={{
                        readOnly: true,
                    }}
                    InputLabelProps={{
                        shrink: true,
                    }}
                />
                <TextField
                    label="Lost Games"
                value={playerStats.lostGames || 0}
                    InputProps={{
                        readOnly: true,
                    }}
                    InputLabelProps={{
                        shrink: true,
                    }}
                />
                <TextField
                    label="Games in White Team"
                value={playerStats.gamesAsWhite || 0}
                    InputProps={{
                        readOnly: true,
                    }}
                    InputLabelProps={{
                        shrink: true,
                    }}
                />
                <TextField
                    label="Games in Black Team"
                value={playerStats.gamesAsBlack || 0}
                    InputProps={{
                        readOnly: true,
                    }}
                    InputLabelProps={{
                        shrink: true,
                    }}
                />
        </div>
    );
}

export default Statistics;
