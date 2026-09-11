import { useState } from "react";
import { useNavigate } from "react-router-dom";
import Navbar from "../components/Navbar";
import AuthFormCard from "../components/AuthFormCard";
import { updateUser } from "../api/userService";
import { getErrorMessage } from "../api/errorHandling";
import { useAuth } from "../hooks/useAuth";
import { useToast } from "../hooks/useToast";

function EditProfile() {
  const { user } = useAuth();
  const navigate = useNavigate();
  const showToast = useToast();

  const [formData, setFormData] = useState({
    username: user.username || "",
    fullName: user.fullName || "",
    phone: user.phone || "",
    password: "",
  });
  const [error, setError] = useState("");
  const [saving, setSaving] = useState(false);

  const handleChange = (e) =>
    setFormData({ ...formData, [e.target.name]: e.target.value });

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError("");
    setSaving(true);
    try {
      const updatedUser = await updateUser(user.id, formData);
      localStorage.setItem("user", JSON.stringify(updatedUser));
      showToast("Profile updated successfully!", "success");
      navigate("/home");
    } catch (err) {
      setError(
        getErrorMessage(err, "Failed to update profile. Please try again."),
      );
    } finally {
      setSaving(false);
    }
  };

  return (
    <div style={{ minHeight: "100vh" }}>
      <Navbar />
      <AuthFormCard title="Edit Profile">
        {error && <p style={{ color: "#b33" }}>{error}</p>}
        <form onSubmit={handleSubmit}>
          <input
            type="text"
            name="fullName"
            placeholder="Full Name"
            value={formData.fullName}
            onChange={handleChange}
            required
          />
          <input
            type="text"
            name="username"
            placeholder="Username"
            value={formData.username}
            onChange={handleChange}
            required
          />
          <input
            type="text"
            name="phone"
            placeholder="Phone Number (+381...)"
            value={formData.phone}
            onChange={handleChange}
          />
          <input
            type="password"
            name="password"
            placeholder="New Password (leave blank to keep current)"
            value={formData.password}
            onChange={handleChange}
          />
          <button type="submit" disabled={saving} style={{ width: "100%" }}>
            {saving ? "Saving..." : "Save Changes"}
          </button>
        </form>
      </AuthFormCard>
    </div>
  );
}

export default EditProfile;
