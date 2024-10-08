import './index.css';
import { useEffect, useState, useRef } from 'react';
import { Button } from "@mui/material";
import { useLocation } from 'react-router-dom';
import { jwtDecode } from "jwt-decode";

const Game = () => {
    const [mapData, setMapData] = useState([]);
    const [turn, setTurn] = useState([]);
    const [isInverted, setIsInverted] = useState(false);
    const [playerPhotoURL, setPlayerPhotoURL] = useState("");
    const [oponentUsername, setOponentUsername] = useState("");
    const [oponentPhotoURL, setOponentPhotoURL] = useState("");
    const [gameEnded, setGameEnded] = useState(false);
    const [buttonsMap, setButtonsMap] = useState([]);
    let possibleMoves = [];
    let movingPieceCoords = [];
    const [isWsOpen, setIsWsOpen] = useState(false);
    const location = useLocation();
    const wsRef = useRef(null);
    const token = localStorage.getItem('token');
    const decodedToken = jwtDecode(localStorage.getItem('token'))

    useEffect(() => {
        if (!isWsOpen) {
            wsRef.current = new WebSocket(`wss://localhost:7038/api/GameWS?token=${token}`);

            wsRef.current.onopen = async () => {
                console.log("Connected to WebSocket server");
                setIsWsOpen(true);
                if (wsRef.current.readyState == 1)
                    wsRef.current.send(JSON.stringify({ gameId: location.state }));
            };

            wsRef.current.onmessage = (event) => {
                const newData = JSON.parse(event.data);

                if (newData.action == 'gameUpdate' && newData.Data != null) {
                    const gameData = JSON.parse(newData.Data)
                    console.log(gameData)
                    setMapData(gameData.Pieces)
                    setTurn(gameData.PlayerTurn)
                    setGameEnded(gameData.GameEnded)
                    setIsInverted(gameData.TeamBlack == decodedToken.sub)
                    if (oponentUsername == "")
                        if (gameData.TeamBlack == decodedToken.sub) {
                            setOponentUsername(gameData.TeamWhite)
                            fetchProfilePhoto(gameData.TeamWhite)
                        }
                        else {
                            setOponentUsername(gameData.TeamBlack)
                            fetchProfilePhoto(gameData.TeamBlack)
                        }

                    if (gameData.Warning)
                        alert(gameData.Message)
                }

                if (newData.action == 'errorMessage') {
                    alert(newData.Data)
                }
            };
        }
    }, [location.state]);

    useEffect(() => {
        if (mapData.length !== 0) {
            updateButtons(null)
        }
    }, [mapData]);

    const fetchProfilePhoto = async (oponentName) => {
        fetch(`https://localhost:7038/api/Player/getProfilePhoto?username=${decodedToken.sub}`, {
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
            return response.text();
        }).then(data => {
            setPlayerPhotoURL(data)
        }).catch(error => {
            console.log(error.message)
        });
        fetch(`https://localhost:7038/api/Player/getProfilePhoto?username=${oponentName}`, {
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
            return response.text();
        }).then(data => {
            setOponentPhotoURL(data)
        }).catch(error => {
            console.log(error.message)
        });
    };

    const updateButtons = (checkedMoves) => {
        const newButtons = [];
        if (isInverted) {
            for (let i = 7; i >= 0; i--) {
                for (let j = 7; j >= 0; j--) {
                    let coordsColor = (i % 2 === j % 2) ? 'Black' : 'White';
                    if (checkedMoves) {
                        checkedMoves.forEach((item) => {
                            if (item.Item1 == i && item.Item2 == j) {
                                coordsColor = 'Gray';
                            }
                        });
                    }
                    newButtons.push(
                        <Button
                            className="chess_Button"
                            key={`${i}-${j}`}
                            onClick={() => Action(i, j)}
                            variant="contained"
                            sx={{
                                backgroundImage: `url(${GetPieceTexture(mapData[i][j].PieceType, mapData[i][j].Team)})`,
                                backgroundSize: 'contain',
                                backgroundPosition: 'center',
                                backgroundRepeat: 'no-repeat',
                                backgroundColor: coordsColor,
                            }}
                        />
                    );
                }
            }
        }
        else {
            for (let i = 0; i < 8; i++) {
                for (let j = 0; j < 8; j++) {
                    let coordsColor = (i % 2 === j % 2) ? 'Black' : 'White';
                    if (checkedMoves) {
                        checkedMoves.forEach((item) => {
                            if (item.Item1 == i && item.Item2 == j) {
                                coordsColor = 'Gray';
                            }
                        });
                    }
                    newButtons.push(
                        <Button
                            className="chess_Button"
                            key={`${i}-${j}`}
                            onClick={() => Action(i, j)}
                            variant="contained"
                            sx={{
                                backgroundImage: `url(${GetPieceTexture(mapData[i][j].PieceType, mapData[i][j].Team)})`,
                                backgroundSize: 'contain',
                                backgroundPosition: 'center',
                                backgroundRepeat: 'no-repeat',
                                backgroundColor: coordsColor,
                            }}
                        />
                    );
                }
            }

        }
        setButtonsMap(newButtons);
    };


    const Action = (i, j) => {
        if (possibleMoves.length == 0) {
            if (mapData[i][j].PieceType != 'Nomad') {
                possibleMoves = mapData[i][j].CheckedMoves;
                movingPieceCoords = { i, j }
                updateButtons(possibleMoves);
            }
        }
        else {
            possibleMoves.forEach((item) => {
                if (item.Item1 == i && item.Item2 == j && !gameEnded) {
                    sendMotionRequest(movingPieceCoords, { i, j })
                }
            })
            possibleMoves = [];
            movingPieceCoords = [];
            updateButtons(null);
        }
    };

    const sendMotionRequest = async (who, where) => {
        let req = {
            CoordsPiece: [who.i, who.j],
            CoordsDestination: [where.i, where.j]
        }
        wsRef.current.send(JSON.stringify(req));
    }

    function GetPieceTexture(pieceType, team) {
        if (pieceType === 'NOMAD') return null;
        return (`./piecesTexture/${pieceType}${team}.png`);
    }

    return (
        <div className="body">
            <header className="header">
                <h1>Game #{location.state}</h1>
            </header>
            <div className="game">
                <div className="GameBanner">
                    <div className="MyBanner">
                        {playerPhotoURL ? (
                            <img src={playerPhotoURL} alt="Profile" className="playerPhoto" />
                        ) : (
                            <p>No profile picture found.</p>
                        )}
                        <div className="nickname">{decodedToken.sub}</div>
                    </div>
                    <div className="InfoBanner">
                        <div className="TurnInfo">Turn: Team {turn}</div>
                        <h2 style={{ color: 'red' }}>{gameEnded ? 'Game OVER!' : ''}</h2>
                    </div>
                    <div className="OponentBanner">
                        <div className="nickname">{oponentUsername}</div>
                        {oponentPhotoURL ? (
                            <img src={oponentPhotoURL} alt="Profile" className="playerPhoto" />
                        ) : (
                            <p>No profile picture found.</p>
                        )}
                    </div>
                </div>
                <div className="button-container">
                    {buttonsMap}
                </div>
            </div>
        </div>

    );
}


export default Game;