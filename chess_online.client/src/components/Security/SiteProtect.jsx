import { Navigate, Outlet } from "react-router-dom";
import { jwtDecode } from "jwt-decode";

const SiteProtect = () => {
    const token = localStorage.getItem("token");

    if (token === undefined || token === null)
        return <Navigate to="/login" />;

    const decodedToken = jwtDecode(token);
    try {
        const isTokenExpired = decodedToken.exp * 1000 < Date.now();

        if (isTokenExpired) {
            localStorage.removeItem('token');
            return <Navigate to="/login" />;
        }
    } catch (error) {
        console.error('Invalid token:', error);
        return <Navigate to="/login" />;
    }

    const tokenAccess = decodedToken.position;

    return (<Outlet context={{ tokenAccess }} />);
};

export default SiteProtect;