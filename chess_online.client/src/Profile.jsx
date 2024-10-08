import { Button, TextField, Input} from "@mui/material";
import { useState, useEffect } from 'react';
import { jwtDecode } from "jwt-decode";

const Profile = () => {
    const token = localStorage.getItem('token')
    const decodedToken = jwtDecode(localStorage.getItem('token'))
    const [playerUsername, setPlayerUsername] = useState("");
    const [newUsername, setNewUsername] = useState("");
    const [playerStats, setPlayerStats] = useState("");
    const [file, setFile] = useState(null);
    const [myProfilePhoto, setMyProfilePhoto] = useState(null);
    const [profilePhoto, setProfilePhoto] = useState(null);

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
            setMyProfilePhoto(data)
            setProfilePhoto(data)
        }).catch(error => {
            console.log(error.message)
        });
    }, []);

    const fetchProfilePhoto = async () => {
        fetch(`https://localhost:7038/api/Player/getProfilePhoto?username=${playerUsername}`, {
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
            setProfilePhoto(data)
        }).catch(error => {
            console.log(error.message)
        });
    };

    const handleFileChange = (event) => {
        setFile(event.target.files[0]);
    };

    const handleUpload = async (event) => {
        event.preventDefault();
        if (!file) {
            alert("Please select a file.");
            return;
        }

        const formData = new FormData();
        formData.append('file', file);

        try {
            const response = await fetch('https://localhost:7038/api/Player/uploadProfilePicture', {
                method: 'POST',
                headers: {
                    'Authorization': token,
                },
                body: formData,
            });

            if (!response.ok) {
                throw new Error("Failed to upload profile picture.");
            }

            fetchProfilePhoto();
        } catch (error) {
            console.error("Error uploading profile picture", error);
        }
    };

    const handleUsernameChange = (event) => {
        setPlayerUsername(event.target.value);
    };

    const handleNewUsernameChange = (event) => {
        setNewUsername(event.target.value);
    };

    const handleUsernameUpdate = () => {
        //on success, delete token aka.logoff
    }

    const handlePasswordUpdate = () => {
        //move to reset password page
    }

    const findPlayer = () => {
        fetchProfilePhoto();
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
        <div className="body">
            <header className="header">
                <h1>Welcome, {decodedToken.sub}!</h1>
            </header>
            <div className="profileView">
                <div className="profileTab">
                    <h2>Check your statistics, or your friends!</h2>
                    <TextField
                        className="input_textfield"
                        required
                        name="name"
                        label="Username"
                        value={playerUsername}
                        onChange={handleUsernameChange}
                    />

                    <Button
                        className="default_button"
                        onClick={findPlayer}
                        variant="contained">
                        Find Player
                    </Button>
                    <h2>Results:</h2>

                    <div className="onlyStats-container">
                        <div className="StatsBanner">
                            {profilePhoto ? (
                                <img src={profilePhoto} alt="Profile" className="playerPhoto" />) : null}
                            <h2>{playerStats.username ? playerStats.username : ""}</h2>
                        </div>
                        <TextField
                            className="default_textfield"
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
                            className="default_textfield"
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
                            className="default_textfield"
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
                            className="default_textfield"
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
                            className="default_textfield"
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
                            className="default_textfield"
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
                </div>
                <div className="profileTab">
                    <h1>{decodedToken.sub}</h1>
                    {myProfilePhoto ? (<img src={myProfilePhoto} alt="Profile" className="playerPhoto" />) : ""}
                    <form onSubmit={handleUpload} className="option">
                        <h3>Change your Profile Photo</h3>
                        <Input type="file" onChange={handleFileChange} accept="image/*" />
                        <Button type="submit">Set new Picture!</Button>
                    </form>
                    <form onSubmit={handleUsernameUpdate} className="option">
                        <h3>Change your Username</h3>
                        <TextField
                            className="default_textfield"
                            label="New Username"
                            InputLabelProps={{
                                shrink: true,
                            }}
                            value={newUsername}
                            onChange={handleNewUsernameChange}
                        />
                        <Button type="submit">Set new Username!</Button>
                    </form>
                    <form onSubmit={handlePasswordUpdate} className="option">
                        <h3>Change your Password</h3>
                        <Button type="submit">Set new Password!</Button>
                    </form>
                </div>
                <div className="profileTab">last GAMES!</div>
            </div>
        </div>
    );
}

export default Profile;
