import { getImageUrl } from "../api/catalogService";

function OrderSummary({ items, total }) {
  return (
    <div
      style={{
        backgroundColor: "var(--card-bg)",
        borderRadius: "8px",
        padding: "20px",
      }}
    >
      <h3 style={{ marginTop: 0 }}>Order Summary</h3>
      {items.map((item) => (
        <div
          key={item.id}
          style={{
            display: "flex",
            alignItems: "center",
            gap: "1rem",
            padding: "10px 0",
            borderBottom: "1px solid var(--accent)",
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
              Qty: {item.quantity}
            </p>
          </div>
          <p style={{ margin: 0, fontWeight: "bold" }}>
            ${item.totalPrice.toFixed(2)}
          </p>
        </div>
      ))}
      <div style={{ textAlign: "right", marginTop: "15px" }}>
        <h3>Total: ${total.toFixed(2)}</h3>
      </div>
    </div>
  );
}

export default OrderSummary;
