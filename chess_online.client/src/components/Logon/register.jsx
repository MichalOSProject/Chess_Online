import { useState } from 'react';
import { Box, TextField, Button } from "@mui/material";
import { useForm } from 'react-hook-form';
import { useNavigate } from 'react-router-dom';
import { Link} from "react-router-dom";

const Login = () => {
    const { register, handleSubmit, getValues, formState: { errors } } = useForm();
    const navigate = useNavigate();
    const [errorText, setErrorText] = useState(null);

    const onSubmit = async () => {
        const loginData = getValues();
        console.log(loginData)
        fetch('https://localhost:7038/api/account/register', {
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
        }).then(() => {
            setErrorText(null)
            navigate('/login',);
        }).catch(error => {
            setErrorText(error.message)
        });
    };
    return (
        <div
            style={{
                display: 'flex',
                flexDirection: 'column',
                alignItems: 'center',
                justifyContent: 'center',
                height: '100vh',
                padding: '1rem',
                boxSizing: 'border-box'
            }}
        >
            <h1>Create Account at Chess World</h1>

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
                    type="email"
                    {...register("Email", { required: true })}
                    error={!!errors.login}
                    helpertext={errors.login ? 'Email is required' : ''}
                />

                <TextField
                    required
                    name="Username"
                    label="Username"
                    {...register("Username", { required: true })}
                    error={!!errors.login}
                    helpertext={errors.login ? 'Username is required' : ''}
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
                    Register
                </Button>
            </Box>
            <br />
            <Link to='/login'><h3>Have an account, log in!</h3></Link>
        </div>
    );
};

export default Login;
