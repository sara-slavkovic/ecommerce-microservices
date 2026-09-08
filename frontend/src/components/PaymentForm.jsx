import { useState } from "react";

function PaymentForm({ onSubmit, processing }) {
  const [cardNumber, setCardNumber] = useState("");
  const [expiryMonth, setExpiryMonth] = useState("");
  const [expiryYear, setExpiryYear] = useState("");
  const [cvv, setCvv] = useState("");

  const handleSubmit = (e) => {
    e.preventDefault();
    onSubmit();
  };

  return (
    <form
      onSubmit={handleSubmit}
      style={{
        backgroundColor: "var(--card-bg)",
        borderRadius: "8px",
        padding: "20px",
      }}
    >
      <h3 style={{ marginTop: 0 }}>Payment Details</h3>
      <input
        type="text"
        placeholder="Card Number"
        value={cardNumber}
        onChange={(e) =>
          setCardNumber(e.target.value.replace(/\D/g, "").slice(0, 16))
        }
        inputMode="numeric"
        required
      />
      <div style={{ display: "flex", gap: "10px" }}>
        <input
          type="text"
          placeholder="MM"
          value={expiryMonth}
          onChange={(e) =>
            setExpiryMonth(e.target.value.replace(/\D/g, "").slice(0, 2))
          }
          inputMode="numeric"
          style={{ flex: 1 }}
          required
        />
        <input
          type="text"
          placeholder="YY"
          value={expiryYear}
          onChange={(e) =>
            setExpiryYear(e.target.value.replace(/\D/g, "").slice(0, 2))
          }
          inputMode="numeric"
          style={{ flex: 1 }}
          required
        />
        <input
          type="password"
          placeholder="CVV"
          value={cvv}
          onChange={(e) =>
            setCvv(e.target.value.replace(/\D/g, "").slice(0, 3))
          }
          inputMode="numeric"
          style={{ flex: 1 }}
          required
        />
      </div>
      <button
        type="submit"
        disabled={processing}
        style={{ width: "100%", marginTop: "10px" }}
      >
        {processing ? "Processing Payment..." : "Pay"}
      </button>
    </form>
  );
}

export default PaymentForm;
