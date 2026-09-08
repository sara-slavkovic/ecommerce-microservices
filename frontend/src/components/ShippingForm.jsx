import { useState } from "react";

function ShippingForm({ onSubmit, submitting }) {
  const [formData, setFormData] = useState({
    address: "",
    city: "",
    postalCode: "",
    country: "",
  });

  const handleChange = (e) =>
    setFormData({ ...formData, [e.target.name]: e.target.value });

  const handleSubmit = (e) => {
    e.preventDefault();
    onSubmit(formData);
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
      <h3 style={{ marginTop: 0 }}>Shipping Details</h3>
      <input
        type="text"
        name="address"
        placeholder="Address"
        value={formData.address}
        onChange={handleChange}
        required
      />
      <input
        type="text"
        name="city"
        placeholder="City"
        value={formData.city}
        onChange={handleChange}
        required
      />
      <input
        type="text"
        name="postalCode"
        placeholder="Postal Code"
        value={formData.postalCode}
        onChange={handleChange}
        required
      />
      <input
        type="text"
        name="country"
        placeholder="Country"
        value={formData.country}
        onChange={handleChange}
        required
      />
      <button
        type="submit"
        disabled={submitting}
        style={{ width: "100%", marginTop: "10px" }}
      >
        {submitting ? "Placing order..." : "Place Order"}
      </button>
    </form>
  );
}

export default ShippingForm;
