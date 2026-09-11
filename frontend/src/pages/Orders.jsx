import { useEffect, useState } from "react";
import Navbar from "../components/Navbar";
import OrderCard from "../components/OrderCard";
import Spinner from "../components/Spinner";
import { getOrdersByUserId } from "../api/orderService";
import { getErrorMessage } from "../api/errorHandling";
import { useAuth } from "../hooks/useAuth";

function Orders() {
  const { user } = useAuth();
  const [orders, setOrders] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    getOrdersByUserId(user.id)
      .then(setOrders)
      .catch((err) =>
        setError(
          getErrorMessage(
            err,
            "Orders are currently unavailable. Thanks for your patience!",
          ),
        ),
      )
      .finally(() => setLoading(false));
  }, [user.id]);

  if (loading) {
    return (
      <div style={{ minHeight: "100vh" }}>
        <Navbar />
        <Spinner text="Loading orders..." />
      </div>
    );
  }

  if (error) {
    return (
      <div style={{ minHeight: "100vh" }}>
        <Navbar />
        <div
          style={{
            maxWidth: "500px",
            margin: "80px auto",
            padding: "30px",
            backgroundColor: "var(--card-bg)",
            borderRadius: "8px",
            textAlign: "center",
          }}
        >
          <h2 style={{ marginTop: 0 }}>Oops!</h2>
          <p>{error}</p>
        </div>
      </div>
    );
  }

  return (
    <div style={{ minHeight: "100vh" }}>
      <Navbar />
      <div style={{ padding: "2rem", maxWidth: "700px", margin: "0 auto" }}>
        <h1
          style={{
            borderBottom: "1px solid var(--accent)",
            paddingBottom: "10px",
          }}
        >
          Your Orders
        </h1>
        {orders.length === 0 ? (
          <p style={{ fontSize: "1.2rem", marginTop: "20px" }}>
            You haven't placed any orders yet.
          </p>
        ) : (
          orders
            .slice()
            .sort((a, b) => new Date(b.createdAt) - new Date(a.createdAt))
            .map((order) => <OrderCard key={order.id} order={order} />)
        )}
      </div>
    </div>
  );
}

export default Orders;
