import { useAuth } from "../hooks/useAuth";
import { useCart } from "../hooks/useCart";
import { Link } from "react-router-dom";

function Navbar() {
  const { user, logout } = useAuth();
  const { itemCount } = useCart();

  return (
    <nav
      style={{
        display: "flex",
        justifyContent: "space-between",
        alignItems: "center",
        padding: "1rem 2rem",
        backgroundColor: "var(--card-bg)",
        borderBottom: "1px solid var(--accent)",
      }}
    >
      <Link to="/home" style={{ textDecoration: "none" }}>
        <h2 style={{ margin: 0 }}>Beauty Store</h2>
      </Link>
      <div style={{ display: "flex", alignItems: "center", gap: "1rem" }}>
        {user?.username === "sara" && (
          <Link to="/admin/products">Manage Products</Link>
        )}
        <Link
          to="/cart"
          style={{ display: "flex", alignItems: "center", gap: "1px" }}
        >
          Cart
          {itemCount > 0 && (
            <span
              style={{
                backgroundColor: "var(--card-bg)",
                color: "var(--text-main)",
                width: "24px",
                height: "24px",
                fontSize: "1rem",
                fontWeight: "bold",
                lineHeight: 1,
                display: "flex",
                alignItems: "flex-start",
                justifyContent: "center",
              }}
            >
              {itemCount}
            </span>
          )}
        </Link>
        <Link to="/orders">Orders</Link>
        <Link to="/profile/edit">{user?.fullName}</Link>
        <button
          onClick={logout}
          style={{ padding: "6px 16px", fontSize: "0.9rem" }}
        >
          Logout
        </button>
      </div>
    </nav>
  );
}

export default Navbar;
