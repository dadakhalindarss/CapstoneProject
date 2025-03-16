import { Link } from "react-router-dom";

const Home = () => {
    return (
        <div style={{ textAlign: "center", marginTop: "50px" }}>
            <h1>Welcome to Task Manager</h1>
            <p>Organize and track your tasks efficiently.</p>
            <Link to="/login"><button>Login</button></Link>
            <Link to="/register"><button>Register</button></Link>
        </div>
    );
};

export default Home;
