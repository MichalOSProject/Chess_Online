import { useState } from 'react';
import { Box, TextField, Button } from "@mui/material";
import { useForm } from 'react-hook-form';
import { Link} from "react-router-dom";
import { useNavigate } from 'react-router-dom';

const Login = () => {
    const { register, handleSubmit, getValues, formState: { errors } } = useForm();
    const navigate = useNavigate();
    const [errorText, setErrorText] = useState(null);

    const onSubmit = async () => {
        const loginData = getValues();

        fetch('https://localhost:7038/api/account/login', {
            method: 'POST',
            mode: 'cors',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(loginData)
        }).then(response => {
            if (!response.ok) {
                return response.text().then(errorData => {
                    throw new Error(errorData);
                });
            }
            return response.json();
        }).then(data => {
            const { token } = data;
            console.log(token)
            localStorage.setItem('token', token);
            setErrorText(null)
            navigate('/',);
        }).catch(error => {
            setErrorText(error.message)
        });
    };

    return (
        <div className="logonBody">
        <div className="loginSection">
            <h1>Login to Chess World</h1>

            <h2 style={{ color: 'red' }}>{errorText != null ? errorText : ''}</h2>
            <Box
                component="form"
                sx={{
                    display: 'flex',
                    flexDirection: 'column',
                    alignItems: 'center',
                    '& .MuiTextField-root': { m: 1, width: '25ch' },
                }}
                noValidate
                autoComplete="off"
                onSubmit={handleSubmit(onSubmit)}
            >
                <TextField
                    required
                    name="Email"
                    label="Email"
                    {...register("Email", { required: true })}
                    error={!!errors.login}
                    helpertext={errors.login ? 'Email is required' : ''}
                />
                <br />
                <TextField
                    required
                    name="password"
                    label="Password"
                    type="password"
                    {...register("Password", { required: true })}
                    error={!!errors.password}
                    helpertext={errors.password ? 'Password is required' : ''}
                />
                <br />

                <Button type="submit" variant="contained" color="primary">
                    Login
                </Button>
            </Box>
            <br />
            <Link to='/register'><h3>Dont have an account? Sign Up!</h3></Link>
            </div>
        </div>
    );
};

export default Login;
