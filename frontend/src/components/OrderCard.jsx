import { useState } from "react";
import { getImageUrl } from "../api/catalogService";

const STATUS_COLORS = {
  Created: "#8a8a8a",
  Pending: "#c9973e",
  Paid: "#7a9d6f",
  Cancelled: "#c0392b",
};

function OrderCard({ order }) {
  const [expanded, setExpanded] = useState(false);
  const statusColor = STATUS_COLORS[order.status] || "var(--accent)";

  return (
    <div
      style={{
        backgroundColor: "var(--card-bg)",
        borderRadius: "8px",
        padding: "20px",
        marginBottom: "15px",
      }}
    >
      <div
        onClick={() => setExpanded(!expanded)}
        style={{
          display: "flex",
          justifyContent: "space-between",
          alignItems: "center",
          cursor: "pointer",
        }}
      >
        <div>
          <p style={{ margin: 0, fontSize: "0.9rem" }}>
            {new Date(order.createdAt).toDateString()}
          </p>
          <p style={{ margin: "4px 0 0 0", fontWeight: "bold" }}>
            Order #{order.id.slice(0, 8)}
          </p>
        </div>
        <div style={{ display: "flex", alignItems: "center", gap: "1rem" }}>
          <span
            style={{
              backgroundColor: statusColor,
              color: "#fff",
              padding: "4px 12px",
              borderRadius: "20px",
              fontSize: "0.85rem",
              fontWeight: "bold",
            }}
          >
            {order.status}
          </span>
          <h3 style={{ margin: 0 }}>${order.totalAmount.toFixed(2)}</h3>
          <span style={{ fontSize: "1.2rem" }}>{expanded ? "▲" : "▼"}</span>
        </div>
      </div>

      {expanded && (
        <div
          style={{
            marginTop: "15px",
            borderTop: "1px solid var(--accent)",
            paddingTop: "15px",
          }}
        >
          {order.orderItems.map((item) => (
            <div
              key={item.id}
              style={{
                display: "flex",
                alignItems: "center",
                gap: "1rem",
                padding: "8px 0",
              }}
            >
              <img
                src={getImageUrl(item.productImageUrl)}
                alt={item.productName}
                style={{
                  width: "45px",
                  height: "45px",
                  objectFit: "cover",
                  borderRadius: "5px",
                }}
              />
              <div style={{ flex: 1 }}>
                <p style={{ margin: 0 }}>{item.productName}</p>
                <p style={{ margin: 0, fontSize: "0.9rem" }}>
                  Qty: {item.quantity} × ${item.pricePerUnit.toFixed(2)}
                </p>
              </div>
              <p style={{ margin: 0, fontWeight: "bold" }}>
                ${item.totalPrice.toFixed(2)}
              </p>
            </div>
          ))}
          <div
            style={{
              marginTop: "10px",
              paddingTop: "10px",
              borderTop: "1px solid var(--accent)",
              fontSize: "0.9rem",
            }}
          >
            <p style={{ margin: 0 }}>
              {order.address}, {order.city} {order.postalCode}, {order.country}
            </p>
          </div>
        </div>
      )}
    </div>
  );
}

export default OrderCard;
