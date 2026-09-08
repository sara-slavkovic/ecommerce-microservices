import { useState } from "react";
import { login } from "../api/userService";
import { getErrorMessage } from "../api/errorHandling";
import { useNavigate } from "react-router-dom";

function Login() {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");
  const [message, setMessage] = useState("");
  const navigate = useNavigate();

  const handleLogin = async (e) => {
    e.preventDefault();
    setError("");
    try {
      const user = await login({ username, password });
      localStorage.setItem("user", JSON.stringify(user));
      setMessage(`Welcome back, ${user.fullName}! ✨`);
      setTimeout(() => navigate("/home"), 1200);
    } catch (err) {
      setError(
        getErrorMessage(
          err,
          "Login currently unavailable. Please try again soon.",
        ),
      );
    }
  };

  return (
    <div
      style={{
        maxWidth: "400px",
        margin: "80px auto",
        padding: "30px",
        backgroundColor: "var(--card-bg)",
        borderRadius: "8px",
        textAlign: "center",
      }}
    >
      <h2>Login</h2>
      {error && <p style={{ color: "#b33" }}>{error}</p>}
      {message && <p style={{ fontWeight: "bold" }}>{message}</p>}
      <form onSubmit={handleLogin}>
        <input
          type="text"
          placeholder="Username"
          value={username}
          onChange={(e) => setUsername(e.target.value)}
          required
        />
        <input
          type="password"
          placeholder="Password"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          required
        />
        <button type="submit" style={{ width: "100%" }}>
          Login
        </button>
      </form>
    </div>
  );
}

export default Login;
